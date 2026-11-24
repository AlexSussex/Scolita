using System.IO;
using System.Text.RegularExpressions;

namespace Scolita.Editor
{
    /// <summary>
    /// Reads whether an APK is debuggable, with <c>aapt dump badging</c> from the Android SDK that
    /// Unity installs. Used by <see cref="BuildScript"/> to refuse a debuggable release APK.
    /// </summary>
    public static class ApkBadging
    {
        private static readonly Regex DebuggableLine = new Regex(
            @"^application-debuggable\s*$", RegexOptions.CultureInvariant | RegexOptions.Multiline);

        /// <summary>
        /// Whether <c>aapt dump badging</c>'s output has the <c>application-debuggable</c> line.
        /// Called by <see cref="ReadIsDebuggable"/> and by <c>ApkBadgingTests</c>.
        /// </summary>
        public static bool IsDebuggable(string aaptOutput)
        {
            return IsDebuggableOutput(aaptOutput);
        }

        /// <summary>
        /// Runs the newest <c>aapt</c> under <paramref name="sdkRoot"/> on <paramref name="apkPath"/>
        /// and says whether the APK is debuggable. Throws an
        /// <see cref="System.InvalidOperationException"/> when <c>aapt</c> is missing or fails.
        /// Called by <see cref="BuildScript"/> after a release build.
        /// </summary>
        public static bool ReadIsDebuggable(string sdkRoot, string apkPath)
        {
            string aapt = AndroidBuildTools.FindNewest(sdkRoot, Path.DirectorySeparatorChar == '\\' ? "aapt.exe" : "aapt");
            return IsDebuggableOutput(AndroidBuildTools.Run(aapt, "dump badging \"" + apkPath + "\"", "aapt dump badging"));
        }

        private static bool IsDebuggableOutput(string aaptOutput)
        {
            return DebuggableLine.IsMatch(aaptOutput ?? string.Empty);
        }
    }
}
