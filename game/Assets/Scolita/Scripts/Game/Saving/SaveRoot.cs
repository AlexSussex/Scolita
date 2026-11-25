using System;
using System.IO;
using UnityEngine;

namespace Scolita.Game.Saving
{
    /// <summary>
    /// The folder every save lives under (<see cref="SaveStore"/>): <see cref="Application.persistentDataPath"/> on the
    /// tablet. In the Editor only, the folder named by the environment variable <see cref="EditorVariable"/> when it is
    /// set, so the PlayMode tests (and an agent's own Play mode) never write into the Editor's own save root, which every
    /// worktree and the human's Editor share. A process's environment ends with it, so nothing is left set after a crash.
    /// </summary>
    public static class SaveRoot
    {
        /// <summary>The environment variable naming the Editor's save root, when it is set.</summary>
        public const string EditorVariable = "SCOLITA_SAVE_ROOT";

        private const string MadeForTestsVariable = "SCOLITA_SAVE_ROOT_MADE_FOR_TESTS";
        private const string BeforeTestsVariable = "SCOLITA_SAVE_ROOT_BEFORE_TESTS";
        private const string TemporaryFolderMark = "scolita-saves-";

        /// <summary>
        /// The save root: <see cref="Application.persistentDataPath"/>, or in the Editor the rooted folder in
        /// <see cref="EditorVariable"/> when it is set. Called by every scene's composition root, <c>ProfileStore</c> and the
        /// PlayMode tests.
        /// </summary>
        public static string Path
        {
            get
            {
                if (Application.isEditor)
                {
                    string chosen = Environment.GetEnvironmentVariable(EditorVariable);
                    if (!string.IsNullOrWhiteSpace(chosen) && System.IO.Path.IsPathRooted(chosen))
                    {
                        return chosen;
                    }
                }

                return Application.persistentDataPath;
            }
        }

        /// <summary>Whether a folder made by <see cref="UseTemporaryFolder"/> is in use and not yet given up.</summary>
        public static bool IsUsingTemporaryFolder => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(MadeForTestsVariable));

        /// <summary>
        /// Makes a new empty folder under the system's temporary folder and names it the Editor's save root for this process,
        /// keeping the folder named before (if any) for <see cref="StopUsingTemporaryFolder"/>; a folder an earlier run made
        /// and never gave up (a run cut short) is deleted first. Returns the new folder. Called by the PlayMode tests when
        /// their run starts.
        /// </summary>
        public static string UseTemporaryFolder()
        {
            if (IsUsingTemporaryFolder)
            {
                DeleteMadeFolder(Environment.GetEnvironmentVariable(MadeForTestsVariable));
            }
            else
            {
                Environment.SetEnvironmentVariable(BeforeTestsVariable, Environment.GetEnvironmentVariable(EditorVariable));
            }

            string folder = System.IO.Path.Combine(System.IO.Path.GetTempPath(), TemporaryFolderMark + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
            Environment.SetEnvironmentVariable(MadeForTestsVariable, folder);
            Environment.SetEnvironmentVariable(EditorVariable, folder);
            return folder;
        }

        /// <summary>
        /// Names the folder named before <see cref="UseTemporaryFolder"/> again and deletes the folder it made, with
        /// everything in it (only that folder, and only if it is in the system's temporary folder with its mark); nothing
        /// when no such folder is in use. Called by the PlayMode tests once their run has ended and the Editor is back in Edit
        /// mode (every scene closed, so nothing saves into it after).
        /// </summary>
        public static void StopUsingTemporaryFolder()
        {
            string made = Environment.GetEnvironmentVariable(MadeForTestsVariable);
            if (string.IsNullOrEmpty(made))
            {
                return;
            }

            Environment.SetEnvironmentVariable(EditorVariable, Environment.GetEnvironmentVariable(BeforeTestsVariable));
            Environment.SetEnvironmentVariable(BeforeTestsVariable, null);
            Environment.SetEnvironmentVariable(MadeForTestsVariable, null);
            DeleteMadeFolder(made);
        }

        private static void DeleteMadeFolder(string folder)
        {
            if (!IsTemporaryFolder(folder) || !Directory.Exists(folder))
            {
                return;
            }

            try
            {
                Directory.Delete(folder, true);
            }
            catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException)
            {
                Debug.LogWarning("Scolita: the tests' save folder " + folder + " could not be deleted: " + exception.Message);
            }
        }

        private static bool IsTemporaryFolder(string folder)
        {
            if (string.IsNullOrWhiteSpace(folder) || !System.IO.Path.IsPathRooted(folder))
            {
                return false;
            }

            string full = System.IO.Path.GetFullPath(folder);
            string temporary = System.IO.Path.GetFullPath(System.IO.Path.GetTempPath());
            return full.StartsWith(temporary, StringComparison.OrdinalIgnoreCase)
                && System.IO.Path.GetFileName(full).StartsWith(TemporaryFolderMark, StringComparison.Ordinal);
        }
    }
}
