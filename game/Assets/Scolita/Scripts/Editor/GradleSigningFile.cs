using System;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace Scolita.Editor
{
    /// <summary>
    /// Hands the release key to Gradle for one release build without Unity ever holding it in its
    /// settings: a JSON file (<see cref="GradleSigningContents"/>) in a folder of that build's own
    /// under <see cref="Root"/> (the user's temporary folder), named by the
    /// <see cref="PathVariable"/> environment variable, which the launcher's Gradle template
    /// (<c>Assets/Plugins/Android/launcherTemplate.gradle</c>) reads when Gradle configures the
    /// build. So the passwords are never written into <c>build.gradle</c>, nor into the caches Unity
    /// and Gradle build from it. <see cref="BuildScript"/> deletes the build's folder after the
    /// build, and a release build first deletes the folders left by release builds that were killed
    /// (their Unity process is gone); folders of builds still running, in any worktree, are left alone.
    /// </summary>
    public static class GradleSigningFile
    {
        /// <summary>The environment variable naming the file, read by the launcher's Gradle template.</summary>
        public const string PathVariable = "SCOLITA_SIGNING_FILE";

        private const string RootFolderName = "scolita-signing";
        private const string FileName = "release-signing.json";
        private const char FolderNameSeparator = '-';

        private static readonly UTF8Encoding Utf8WithoutMark = new UTF8Encoding(false);

        /// <summary>The folder holding one folder per release build: <c>scolita-signing</c> in the user's temporary folder.</summary>
        public static string Root => Path.Combine(Path.GetTempPath(), RootFolderName);

        /// <summary>
        /// A new folder path under <paramref name="root"/> for one build of the Unity process
        /// <paramref name="processId"/>: <c>&lt;processId&gt;-&lt;random&gt;</c>. Called by
        /// <see cref="BuildScript"/> and by <c>GradleSigningFileTests</c>.
        /// </summary>
        public static string NewBuildFolder(string root, int processId)
        {
            return Path.Combine(root, processId.ToString(CultureInfo.InvariantCulture) + FolderNameSeparator
                + Guid.NewGuid().ToString("N"));
        }

        /// <summary>
        /// Writes <paramref name="signing"/> to a new file in <paramref name="folder"/>, created if
        /// needed, with the keystore's full path, and returns the file's path. Called by
        /// <see cref="BuildScript"/> before a release build and by <c>GradleSigningFileTests</c>.
        /// </summary>
        public static string Write(string folder, ReleaseSigning signing)
        {
            if (signing == null)
            {
                throw new ArgumentNullException(nameof(signing));
            }

            Directory.CreateDirectory(folder);
            string path = Path.Combine(folder, FileName);
            File.WriteAllText(path, JsonUtility.ToJson(new GradleSigningContents(signing)), Utf8WithoutMark);
            return path;
        }

        /// <summary>
        /// Deletes <paramref name="folder"/> and everything in it, if it exists. Throws an
        /// <see cref="IOException"/> naming the folder when it can't be deleted, as it may hold the
        /// passwords. Called by <see cref="BuildScript"/> after a release build and by
        /// <c>GradleSigningFileTests</c>.
        /// </summary>
        public static void DeleteFolder(string folder)
        {
            DeleteFolderAndContents(folder);
        }

        /// <summary>
        /// Deletes what killed release builds left under <paramref name="root"/>: every build folder
        /// whose Unity process <paramref name="isProcessRunning"/> says is gone (or whose name isn't
        /// a build folder's), and any loose file. Throws an <see cref="IOException"/> naming a folder
        /// that can't be deleted. Called by <see cref="BuildScript"/> at the start of a release build
        /// and by <c>GradleSigningFileTests</c>.
        /// </summary>
        public static void DeleteLeftovers(string root, Func<int, bool> isProcessRunning)
        {
            if (!Directory.Exists(root))
            {
                return;
            }

            foreach (string file in Directory.GetFiles(root))
            {
                File.Delete(file);
            }

            foreach (string folder in Directory.GetDirectories(root))
            {
                if (!TryGetProcessId(Path.GetFileName(folder), out int processId) || !isProcessRunning(processId))
                {
                    DeleteFolderAndContents(folder);
                }
            }
        }

        private static bool TryGetProcessId(string folderName, out int processId)
        {
            int separator = folderName.IndexOf(FolderNameSeparator);
            processId = 0;
            return separator > 0 && int.TryParse(folderName.Substring(0, separator), NumberStyles.None,
                CultureInfo.InvariantCulture, out processId);
        }

        private static void DeleteFolderAndContents(string folder)
        {
            if (!Directory.Exists(folder))
            {
                return;
            }

            try
            {
                Directory.Delete(folder, true);
            }
            catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException)
            {
                throw new IOException(folder + ", which may hold the release key's passwords, could not be "
                    + "deleted (" + exception.Message + "); delete it by hand.", exception);
            }
        }
    }
}
