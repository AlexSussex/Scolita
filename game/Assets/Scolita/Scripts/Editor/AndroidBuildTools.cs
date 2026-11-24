using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Scolita.Editor
{
    /// <summary>
    /// Finds and runs the Android SDK's build tools (<c>aapt</c>, <c>apksigner</c>) that Unity
    /// installs, for the checks <see cref="BuildScript"/> makes on every APK.
    /// </summary>
    public static class AndroidBuildTools
    {
        private const int TimeoutMilliseconds = 120000;

        /// <summary>
        /// The file <paramref name="relativePath"/> in the newest <c>build-tools</c> version under
        /// <paramref name="sdkRoot"/> that has it. Throws an <see cref="InvalidOperationException"/>
        /// when none has. Called by <see cref="ApkPermissions"/>, <see cref="ApkBadging"/> and
        /// <see cref="ApkSigner"/>.
        /// </summary>
        public static string FindNewest(string sdkRoot, string relativePath)
        {
            string buildTools = Path.Combine(sdkRoot ?? string.Empty, "build-tools");
            string found = Directory.Exists(buildTools)
                ? Directory.GetDirectories(buildTools)
                    .OrderByDescending(VersionOfFolder)
                    .Select(folder => Path.Combine(folder, relativePath))
                    .FirstOrDefault(File.Exists)
                : null;
            if (found == null)
            {
                throw new InvalidOperationException(relativePath + " was not found under " + buildTools
                    + "; install Android Build Support's SDK in Unity Hub.");
            }

            return found;
        }

        /// <summary>
        /// Runs <paramref name="executable"/> with <paramref name="arguments"/> and returns what it
        /// printed. Throws an <see cref="InvalidOperationException"/> naming
        /// <paramref name="toolName"/> when it can't start, fails or takes over two minutes. Called
        /// by <see cref="ApkPermissions"/>, <see cref="ApkBadging"/> and <see cref="ApkSigner"/>.
        /// </summary>
        public static string Run(string executable, string arguments, string toolName)
        {
            ProcessStartInfo start = new ProcessStartInfo(executable)
            {
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            using (Process tool = Process.Start(start))
            {
                if (tool == null)
                {
                    throw new InvalidOperationException(toolName + " could not be started.");
                }

                Task<string> errors = tool.StandardError.ReadToEndAsync();
                string output = tool.StandardOutput.ReadToEnd();
                if (!tool.WaitForExit(TimeoutMilliseconds) || tool.ExitCode != 0)
                {
                    throw new InvalidOperationException(toolName + " failed: " + errors.Result.Trim());
                }

                return output;
            }
        }

        private static Version VersionOfFolder(string folder)
        {
            return Version.TryParse(Path.GetFileName(folder), out Version version) ? version : new Version(0, 0);
        }
    }
}
