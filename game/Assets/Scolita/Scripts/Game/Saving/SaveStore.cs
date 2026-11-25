using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using Scolita.Core.Profiles;
using Scolita.Core.Saving;
using Scolita.Core.Timing;
using Scolita.Game.Timing;
using UnityEngine;

namespace Scolita.Game.Saving
{
    /// <summary>
    /// Reads and writes the app's save files: one versioned JSON file per concern
    /// (<c>&lt;name&gt;.json</c>) in one folder. The tablet's own files sit in the save root
    /// (<see cref="SaveRoot.Path"/> in the app); every file about a learner sits in
    /// its profile's folder, <c>profiles/&lt;profileId&gt;/</c> (<see cref="ForProfile"/>), so one
    /// profile never touches another's files. A damaged file is set aside as
    /// <c>&lt;name&gt;.broken-&lt;date-time&gt;.json</c> (each damaged copy kept, so her progress can be
    /// rescued by hand: the human, 2026-11-24) and the defaults are used; a file that can't be opened right
    /// now, or that a newer version of the app wrote, is kept as it is and never saved over; a
    /// folder that can't be used gives the defaults without saving. So a bad save never stops the
    /// app, and a good one is never thrown away. The dated copies are removed by themselves once a
    /// newer version of the app has read and saved their file fine on several days (the human,
    /// 2026-11-24): each folder's ledger, <c>broken-copies.json</c> (<see cref="DamagedCopiesData"/>),
    /// notes the version that set a copy aside and the days a newer one read and saved the file fine,
    /// and <see cref="RemoveSettledCopies"/> removes them (Core's <see cref="DamagedCopies"/> rule).
    /// </summary>
    public sealed class SaveStore
    {
        /// <summary>The folder under the save root that holds one folder per profile.</summary>
        public const string ProfilesFolderName = "profiles";

        private const string FileExtension = ".json";
        private const string BrokenFileMark = ".broken-";
        private const string BrokenFileTimeFormat = "yyyyMMdd-HHmmss";
        private const string TemporaryFileExtension = ".tmp";

        private static readonly Regex NamePattern = new Regex(@"^[a-z0-9_-]+\z", RegexOptions.CultureInvariant);
        private static readonly UTF8Encoding Utf8WithoutMark = new UTF8Encoding(false);

        private readonly string directory;
        private readonly bool directoryUsable;
        private readonly IClock clock;
        private readonly Version appVersion;
        private readonly HashSet<string> keptUnreadable = new HashSet<string>(StringComparer.Ordinal);
        private readonly HashSet<string> readFine = new HashSet<string>(StringComparer.Ordinal);
        private readonly HashSet<string> fineDayNoted = new HashSet<string>(StringComparer.Ordinal);

        private SaveStore(string directory, IClock clock, string appVersion)
        {
            this.directory = directory;
            directoryUsable = !string.IsNullOrWhiteSpace(directory) && Path.IsPathRooted(directory);
            this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
            this.appVersion = Version.TryParse(appVersion, out Version version) ? version : DamagedCopies.UnknownVersion;
        }

        private enum ReadProblem
        {
            None,
            Damaged,
            KeptAsItIs
        }

        /// <summary>The folder this store reads and writes.</summary>
        public string Folder => directory;

        /// <summary>
        /// A store for the tablet's own files (its settings, the profile list) directly in
        /// <paramref name="saveRoot"/>, which is created on the first save if it doesn't exist. A
        /// missing or relative folder makes every load give the defaults
        /// (<see cref="SaveLoadOutcome.Unreadable"/>) and every save fail, after logging an error.
        /// Called by <c>BootLoader</c> and by <c>SaveStoreTests</c>.
        /// </summary>
        public static SaveStore ForTablet(string saveRoot)
        {
            return new SaveStore(saveRoot, new SystemClock(), Application.version);
        }

        /// <summary>
        /// A store for the tablet's own files, as <see cref="ForTablet(string)"/>, that takes the day from
        /// <paramref name="clock"/> and the app's version from <paramref name="appVersion"/>
        /// (<c>MAJOR.MINOR.PATCH</c>) for its damaged copies' ledger. Called by <c>SaveStoreTests</c>.
        /// </summary>
        public static SaveStore ForTablet(string saveRoot, IClock clock, string appVersion)
        {
            return new SaveStore(saveRoot, clock, appVersion);
        }

        /// <summary>
        /// A store for <paramref name="profile"/>'s files, in <c>profiles/&lt;profileId&gt;/</c> under
        /// <paramref name="saveRoot"/>; created on the first save. A missing or relative root gives a
        /// store that can't be used, as in <see cref="ForTablet"/>. Called by <c>BootLoader</c> and by
        /// <c>SaveStoreTests</c>.
        /// </summary>
        public static SaveStore ForProfile(string saveRoot, ProfileId profile)
        {
            return new SaveStore(ProfileFolder(saveRoot, profile), new SystemClock(), Application.version);
        }

        /// <summary>
        /// A store for <paramref name="profile"/>'s files, as <see cref="ForProfile(string, ProfileId)"/>, that takes
        /// the day from <paramref name="clock"/> and the app's version from <paramref name="appVersion"/> for its
        /// damaged copies' ledger. Called by the saving tests.
        /// </summary>
        public static SaveStore ForProfile(string saveRoot, ProfileId profile, IClock clock, string appVersion)
        {
            return new SaveStore(ProfileFolder(saveRoot, profile), clock, appVersion);
        }

        /// <summary>
        /// The profiles with a folder under <paramref name="saveRoot"/>'s <c>profiles</c> folder (a folder whose name
        /// can't be a profile's id is left out); none when there is no such folder or it can't be read. Called by
        /// <c>BootLoader</c> (removing settled copies in every profile) and <c>SaveStoreTests</c>.
        /// </summary>
        public static IReadOnlyList<ProfileId> ProfilesIn(string saveRoot)
        {
            string profiles = string.IsNullOrWhiteSpace(saveRoot) ? null : Path.Combine(saveRoot, ProfilesFolderName);
            try
            {
                if (profiles == null || !Directory.Exists(profiles))
                {
                    return new ProfileId[0];
                }

                List<ProfileId> found = new List<ProfileId>();
                foreach (string folder in Directory.GetDirectories(profiles))
                {
                    if (ProfileId.TryCreate(Path.GetFileName(folder), out ProfileId profile))
                    {
                        found.Add(profile);
                    }
                }

                return found;
            }
            catch (Exception exception) when (IsFileSystemError(exception))
            {
                Debug.LogWarning("Scolita: the profiles' folders could not be listed: " + exception.Message);
                return new ProfileId[0];
            }
        }

        /// <summary>
        /// Copies <c>&lt;name&gt;.json</c> to a dated copy, <c>&lt;name&gt;.broken-&lt;date-time&gt;.json</c>, as a damaged file
        /// is set aside (watched by the copies' ledger, removed by itself later), leaving the file in place: for a file that
        /// reads but has a part that can't be used, before it is saved over. True when the copy is made (or there is no
        /// file); false, after logging the error, when it could not be. Called by the profile store and the tests.
        /// </summary>
        public bool KeepDatedCopy(string name)
        {
            RequireValidName(name);
            string path = PathOf(name, FileExtension);
            if (!directoryUsable || !File.Exists(path))
            {
                return directoryUsable;
            }

            try
            {
                string copy = FreeBrokenPath(name);
                File.Copy(path, copy);
                Debug.LogWarning("Scolita: " + name + FileExtension + " has a part that can't be read; it was copied to "
                    + Path.GetFileName(copy) + " before it is saved over.");
                NoteSetAside(name);
                return true;
            }
            catch (Exception exception) when (IsFileSystemError(exception))
            {
                Debug.LogError("Scolita: " + name + FileExtension + " could not be copied: " + exception.Message);
                return false;
            }
        }

        /// <summary>
        /// Deletes the folder of <paramref name="profile"/>, exactly <c>&lt;saveRoot&gt;/profiles/&lt;id&gt;</c>, and
        /// everything in it (a folder that is a link elsewhere is unlinked, never followed). True when it is gone (or was
        /// never there); false, after logging the error, when it could not be deleted or the root can't be used. Called
        /// by the profile store once the list without the profile is saved, and the tests.
        /// </summary>
        public static bool DeleteProfileFolder(string saveRoot, ProfileId profile)
        {
            string folder = ProfileFolder(saveRoot, profile);
            if (string.IsNullOrWhiteSpace(saveRoot) || !Path.IsPathRooted(saveRoot))
            {
                Debug.LogError("Scolita: the save folder '" + saveRoot + "' can't be used; the profile " + profile
                    + " was not deleted.");
                return false;
            }

            try
            {
                if (!Directory.Exists(folder))
                {
                    return true;
                }

                bool isLink = (File.GetAttributes(folder) & FileAttributes.ReparsePoint) != 0;
                Directory.Delete(folder, !isLink);
                return true;
            }
            catch (Exception exception) when (IsFileSystemError(exception))
            {
                Debug.LogError("Scolita: the folder of the profile " + profile + " could not be deleted: " + exception.Message);
                return false;
            }
        }

        /// <summary>
        /// Reads <c>&lt;name&gt;.json</c>. Returns <paramref name="defaults"/> when the file is
        /// missing; when it is damaged (empty, not JSON, version 0), after renaming it
        /// <c>&lt;name&gt;.broken-&lt;date-time&gt;.json</c> in the tablet's local time (never replacing an
        /// older copy: a second one in the same second gets <c>-2</c>, and so on); and when it can't be opened, was
        /// written by a newer version of the app, can't be set aside, or the folder can't be used,
        /// in which case the file is left as it is (<see cref="SaveLoadOutcome.Unreadable"/>) and
        /// this store refuses to save over it. A file that is read is brought to the current shape by
        /// <see cref="ISaveData{T}.Upgraded"/>. Called by <c>BootLoader</c> and by <c>SaveStoreTests</c>.
        /// </summary>
        public SaveLoadResult<T> Load<T>(string name, T defaults) where T : class, ISaveData<T>
        {
            return Read(name, defaults);
        }

        /// <summary>
        /// Writes <paramref name="data"/> to <c>&lt;name&gt;.json</c> through a temporary file of its
        /// own, flushed to the disk before it replaces the old one, so a write cut short never leaves
        /// a half-written save. Returns false, after logging the error, if the file could not be
        /// written, or if this store loaded it as <see cref="SaveLoadOutcome.Unreadable"/> (a file
        /// kept as it is is never saved over). A file this store read fine and now saved fine counts a fine
        /// day for its damaged copies, if it has any (once a day). Called by <c>BootLoader</c> and by
        /// <c>SaveStoreTests</c>.
        /// </summary>
        public bool Save<T>(string name, T data) where T : class, ISaveData<T>
        {
            bool written = Write(name, data);
            if (written && readFine.Contains(name) && fineDayNoted.Add(name))
            {
                NoteFineDay(name);
            }

            return written;
        }

        /// <summary>
        /// Deletes <c>&lt;name&gt;.json</c>, whatever it holds (a file kept as it is too, since the parent asked), so the
        /// next load gives the defaults; its damaged copies stay, for rescuing her progress by hand. Returns true when
        /// the file is gone (or was never there), false, after logging the error, when it could not be deleted or the
        /// folder can't be used. Called by <c>ProgressReset</c> and <c>SaveStoreTests</c>.
        /// </summary>
        public bool Delete(string name)
        {
            RequireValidName(name);
            if (!directoryUsable)
            {
                Debug.LogError("Scolita: the save folder '" + directory + "' can't be used; "
                    + name + FileExtension + " was not deleted.");
                return false;
            }

            string path = PathOf(name, FileExtension);
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception exception) when (IsFileSystemError(exception))
            {
                Debug.LogError("Scolita: could not delete " + name + FileExtension + ": " + exception.Message);
                return false;
            }

            keptUnreadable.Remove(name);
            readFine.Remove(name);
            return true;
        }

        /// <summary>
        /// Removes the damaged copies of every file in this folder that a newer version of the app has read and saved fine
        /// on at least <paramref name="fineDays"/> different days, and forgets them in the ledger; copies found without a
        /// watch (made before the ledger, or after a damaged ledger) are watched from now on, as set aside by this
        /// version, so only a later one removes them. Nothing is removed or written while the ledger can't be read or
        /// the folder can't be listed. Returns how many copies were removed. Called by <c>BootLoader</c> at each
        /// start, for the tablet's folder and each profile's, and by <c>SaveStoreTests</c>.
        /// </summary>
        public int RemoveSettledCopies(int fineDays)
        {
            if (!directoryUsable || !Directory.Exists(directory))
            {
                return 0;
            }

            SaveLoadResult<DamagedCopiesData> read = Read(DamagedCopiesData.FileName, DamagedCopiesData.CreateDefaults());
            if (read.Outcome == SaveLoadOutcome.Unreadable)
            {
                return 0;
            }

            DamagedCopies ledger = read.Data.ToCopies();
            if (read.Outcome == SaveLoadOutcome.Loaded)
            {
                ledger.ReadAndSavedFine(DamagedCopiesData.FileName, appVersion, clock.Today);
            }

            Dictionary<string, List<string>> copies = CopiesByFile();
            if (copies == null || (copies.Count == 0 && read.Outcome != SaveLoadOutcome.Loaded))
            {
                return 0;
            }

            foreach (string name in copies.Keys)
            {
                ledger.Found(name, appVersion);
            }

            foreach (string gone in ledger.Watches.Select(watch => watch.FileName).Where(name => !copies.ContainsKey(name)).ToArray())
            {
                ledger.Forget(gone);
            }

            int removed = 0;
            foreach (string settled in ledger.Settled(fineDays))
            {
                int deleted = DeleteCopies(copies[settled]);
                removed += deleted;
                if (deleted == copies[settled].Count)
                {
                    ledger.Forget(settled);
                    Debug.Log("Scolita: the damaged copies of " + settled + FileExtension + " were removed (" + deleted
                        + "): a newer version read and saved it fine on " + fineDays + " days.");
                }
            }

            Write(DamagedCopiesData.FileName, new DamagedCopiesData(ledger));
            return removed;
        }

        private SaveLoadResult<T> Read<T>(string name, T defaults) where T : class, ISaveData<T>
        {
            RequireValidName(name);
            if (!directoryUsable)
            {
                Debug.LogError("Scolita: the save folder '" + directory + "' can't be used; "
                    + name + FileExtension + " was not read and the defaults are used.");
                return new SaveLoadResult<T>(defaults, SaveLoadOutcome.Unreadable);
            }

            string path = PathOf(name, FileExtension);
            if (!File.Exists(path))
            {
                return new SaveLoadResult<T>(defaults, SaveLoadOutcome.Missing);
            }

            T loaded = TryRead<T>(path, out ReadProblem kind, out string problem);
            if (kind == ReadProblem.None)
            {
                keptUnreadable.Remove(name);
                readFine.Add(name);
                return new SaveLoadResult<T>(loaded.Upgraded(), SaveLoadOutcome.Loaded);
            }

            if (kind == ReadProblem.Damaged && TrySetAside(name, path, problem))
            {
                keptUnreadable.Remove(name);
                return new SaveLoadResult<T>(defaults, SaveLoadOutcome.SetAside);
            }

            if (kind == ReadProblem.KeptAsItIs)
            {
                Debug.LogError("Scolita: " + name + FileExtension + " could not be read (" + problem
                    + "); it is kept as it is and the defaults are used.");
            }

            keptUnreadable.Add(name);
            return new SaveLoadResult<T>(defaults, SaveLoadOutcome.Unreadable);
        }

        private bool Write<T>(string name, T data) where T : class, ISaveData<T>
        {
            RequireValidName(name);
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!directoryUsable)
            {
                Debug.LogError("Scolita: the save folder '" + directory + "' can't be used; "
                    + name + FileExtension + " was not saved.");
                return false;
            }

            if (keptUnreadable.Contains(name))
            {
                Debug.LogError("Scolita: " + name + FileExtension + " could not be read and is kept as it is; "
                    + "it was not saved over.");
                return false;
            }

            string path = PathOf(name, FileExtension);
            string temporaryPath = PathOf(name, "." + Guid.NewGuid().ToString("N") + FileExtension + TemporaryFileExtension);
            try
            {
                Directory.CreateDirectory(directory);
                WriteFlushed(temporaryPath, JsonUtility.ToJson(data, true));
                ReplaceOrMove(temporaryPath, path);
                return true;
            }
            catch (Exception exception) when (IsFileSystemError(exception))
            {
                Debug.LogError("Scolita: could not save " + name + FileExtension + ": " + exception.Message);
                DeleteIfLeft(temporaryPath);
                return false;
            }
        }

        private static int DeleteCopies(IEnumerable<string> paths)
        {
            int deleted = 0;
            foreach (string path in paths)
            {
                try
                {
                    File.Delete(path);
                    deleted++;
                }
                catch (Exception exception) when (IsFileSystemError(exception))
                {
                    Debug.LogWarning("Scolita: the damaged copy " + Path.GetFileName(path) + " could not be removed: "
                        + exception.Message);
                }
            }

            return deleted;
        }

        private static string ProfileFolder(string saveRoot, ProfileId profile)
        {
            return string.IsNullOrWhiteSpace(saveRoot) ? saveRoot : Path.Combine(saveRoot, ProfilesFolderName, profile.Value);
        }

        private static T TryRead<T>(string path, out ReadProblem kind, out string problem) where T : class, ISaveData<T>
        {
            string json;
            try
            {
                json = File.ReadAllText(path);
            }
            catch (Exception exception) when (IsFileSystemError(exception))
            {
                return Problem<T>(ReadProblem.KeptAsItIs, "it could not be opened: " + exception.Message, out kind, out problem);
            }

            if (string.IsNullOrWhiteSpace(json))
            {
                return Problem<T>(ReadProblem.Damaged, "the file is empty", out kind, out problem);
            }

            T loaded;
            try
            {
                loaded = JsonUtility.FromJson<T>(json);
            }
            catch (ArgumentException exception)
            {
                return Problem<T>(ReadProblem.Damaged, "the file is not valid JSON (" + exception.Message + ")",
                    out kind, out problem);
            }

            if (loaded == null || loaded.Version < 1)
            {
                return Problem<T>(ReadProblem.Damaged, "the file holds no data of a known version", out kind, out problem);
            }

            if (loaded.Version > loaded.CurrentVersion)
            {
                return Problem<T>(ReadProblem.KeptAsItIs, "a newer version of the app wrote it (version "
                    + loaded.Version + "; this one reads up to " + loaded.CurrentVersion + ")", out kind, out problem);
            }

            kind = ReadProblem.None;
            problem = null;
            return loaded;
        }

        private static T Problem<T>(ReadProblem found, string description, out ReadProblem kind, out string problem)
            where T : class
        {
            kind = found;
            problem = description;
            return null;
        }

        private static void WriteFlushed(string path, string text)
        {
            using (FileStream stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                byte[] bytes = Utf8WithoutMark.GetBytes(text);
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush(true);
            }
        }

        private static void ReplaceOrMove(string source, string destination)
        {
            if (File.Exists(destination))
            {
                File.Replace(source, destination, null);
            }
            else
            {
                File.Move(source, destination);
            }
        }

        private static void DeleteIfLeft(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception exception) when (IsFileSystemError(exception))
            {
                Debug.LogWarning("Scolita: the temporary file " + path + " could not be deleted: " + exception.Message);
            }
        }

        private static bool IsFileSystemError(Exception exception)
        {
            return exception is IOException
                || exception is UnauthorizedAccessException
                || exception is NotSupportedException
                || exception is SecurityException;
        }

        private static void RequireValidName(string name)
        {
            if (name == null || !NamePattern.IsMatch(name))
            {
                throw new ArgumentException("A save file's name is lower-case letters, digits, '_' and '-' "
                    + "only, without a folder or an extension: '" + name + "'.", nameof(name));
            }
        }

        private bool TrySetAside(string name, string path, string problem)
        {
            try
            {
                string brokenPath = FreeBrokenPath(name);
                File.Move(path, brokenPath);
                Debug.LogWarning("Scolita: " + name + FileExtension + " could not be read (" + problem
                    + "); it was set aside as " + Path.GetFileName(brokenPath) + " and the defaults are used.");
                NoteSetAside(name);
                return true;
            }
            catch (Exception exception) when (IsFileSystemError(exception))
            {
                Debug.LogError("Scolita: " + name + FileExtension + " could not be read (" + problem
                    + ") nor set aside (" + exception.Message + "); it is kept as it is and the defaults are used.");
                return false;
            }
        }

        private void NoteSetAside(string name)
        {
            if (name == DamagedCopiesData.FileName)
            {
                return;
            }

            SaveLoadResult<DamagedCopiesData> read = Read(DamagedCopiesData.FileName, DamagedCopiesData.CreateDefaults());
            if (read.Outcome == SaveLoadOutcome.Unreadable)
            {
                return;
            }

            DamagedCopies ledger = read.Data.ToCopies();
            ledger.SetAside(name, appVersion);
            Write(DamagedCopiesData.FileName, new DamagedCopiesData(ledger));
        }

        private void NoteFineDay(string name)
        {
            if (name == DamagedCopiesData.FileName || !File.Exists(PathOf(DamagedCopiesData.FileName, FileExtension)))
            {
                return;
            }

            SaveLoadResult<DamagedCopiesData> read = Read(DamagedCopiesData.FileName, DamagedCopiesData.CreateDefaults());
            DamagedCopies ledger = read.Data.ToCopies();
            if (read.Outcome == SaveLoadOutcome.Loaded && ledger.ReadAndSavedFine(name, appVersion, clock.Today))
            {
                Write(DamagedCopiesData.FileName, new DamagedCopiesData(ledger));
            }
        }

        private Dictionary<string, List<string>> CopiesByFile()
        {
            Dictionary<string, List<string>> copies = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            string[] paths;
            try
            {
                paths = Directory.GetFiles(directory, "*" + BrokenFileMark + "*" + FileExtension);
            }
            catch (Exception exception) when (IsFileSystemError(exception))
            {
                Debug.LogWarning("Scolita: the damaged copies in " + directory + " could not be listed: " + exception.Message);
                return null;
            }

            foreach (string path in paths)
            {
                string fileName = Path.GetFileName(path);
                string name = fileName.Substring(0, fileName.IndexOf(BrokenFileMark, StringComparison.Ordinal));
                if (!NamePattern.IsMatch(name) || !fileName.EndsWith(FileExtension, StringComparison.Ordinal))
                {
                    continue;
                }

                if (!copies.TryGetValue(name, out List<string> named))
                {
                    named = new List<string>();
                    copies.Add(name, named);
                }

                named.Add(path);
            }

            return copies;
        }

        private string FreeBrokenPath(string name)
        {
            string stamp = BrokenFileMark + DateTime.Now.ToString(BrokenFileTimeFormat, CultureInfo.InvariantCulture);
            string path = PathOf(name, stamp + FileExtension);
            for (int copy = 2; File.Exists(path); copy++)
            {
                path = PathOf(name, stamp + "-" + copy.ToString(CultureInfo.InvariantCulture) + FileExtension);
            }

            return path;
        }

        private string PathOf(string name, string extension)
        {
            return Path.Combine(directory, name + extension);
        }
    }
}
