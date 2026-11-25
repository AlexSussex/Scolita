using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using Scolita.Core.Profiles;
using UnityEngine;

namespace Scolita.Game.Saving
{
    /// <summary>
    /// Reads and writes the app's save files: one versioned JSON file per concern
    /// (<c>&lt;name&gt;.json</c>) in one folder. The tablet's own files sit in the save root
    /// (<see cref="Application.persistentDataPath"/> in the app); every file about a learner sits in
    /// its profile's folder, <c>profiles/&lt;profileId&gt;/</c> (<see cref="ForProfile"/>), so one
    /// profile never touches another's files. A damaged file is set aside as
    /// <c>&lt;name&gt;.broken-&lt;date-time&gt;.json</c> (each damaged copy kept, so her progress can be
    /// rescued by hand: the human, 2026-11-24) and the defaults are used; a file that can't be opened right
    /// now, or that a newer version of the app wrote, is kept as it is and never saved over; a
    /// folder that can't be used gives the defaults without saving. So a bad save never stops the
    /// app, and a good one is never thrown away.
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
        private readonly HashSet<string> keptUnreadable = new HashSet<string>(StringComparer.Ordinal);

        private SaveStore(string directory)
        {
            this.directory = directory;
            directoryUsable = !string.IsNullOrWhiteSpace(directory) && Path.IsPathRooted(directory);
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
        /// A store for the tablet's own files (its settings, later the profile list) directly in
        /// <paramref name="saveRoot"/>, which is created on the first save if it doesn't exist. A
        /// missing or relative folder makes every load give the defaults
        /// (<see cref="SaveLoadOutcome.Unreadable"/>) and every save fail, after logging an error.
        /// Called by <c>BootLoader</c> and by <c>SaveStoreTests</c>.
        /// </summary>
        public static SaveStore ForTablet(string saveRoot)
        {
            return new SaveStore(saveRoot);
        }

        /// <summary>
        /// A store for <paramref name="profile"/>'s files, in <c>profiles/&lt;profileId&gt;/</c> under
        /// <paramref name="saveRoot"/>; created on the first save. A missing or relative root gives a
        /// store that can't be used, as in <see cref="ForTablet"/>. Called by <c>BootLoader</c> and by
        /// <c>SaveStoreTests</c>.
        /// </summary>
        public static SaveStore ForProfile(string saveRoot, ProfileId profile)
        {
            string folder = string.IsNullOrWhiteSpace(saveRoot)
                ? saveRoot
                : Path.Combine(saveRoot, ProfilesFolderName, profile.Value);
            return new SaveStore(folder);
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

        /// <summary>
        /// Writes <paramref name="data"/> to <c>&lt;name&gt;.json</c> through a temporary file of its
        /// own, flushed to the disk before it replaces the old one, so a write cut short never leaves
        /// a half-written save. Returns false, after logging the error, if the file could not be
        /// written, or if this store loaded it as <see cref="SaveLoadOutcome.Unreadable"/> (a file
        /// kept as it is is never saved over). Called by <c>BootLoader</c> and by <c>SaveStoreTests</c>.
        /// </summary>
        public bool Save<T>(string name, T data) where T : class, ISaveData<T>
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
                return true;
            }
            catch (Exception exception) when (IsFileSystemError(exception))
            {
                Debug.LogError("Scolita: " + name + FileExtension + " could not be read (" + problem
                    + ") nor set aside (" + exception.Message + "); it is kept as it is and the defaults are used.");
                return false;
            }
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
