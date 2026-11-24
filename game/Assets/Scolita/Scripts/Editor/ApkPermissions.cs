using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Scolita.Editor
{
    /// <summary>
    /// Reads the Android permissions an APK asks for, with <c>aapt dump permissions</c> from the
    /// Android SDK that Unity installs. Used by <see cref="BuildScript"/> to refuse an APK that asks
    /// for the internet.
    /// </summary>
    public static class ApkPermissions
    {
        /// <summary>The permission Școlița never asks for: it is offline by design.</summary>
        public const string Internet = "android.permission.INTERNET";

        private static readonly Regex UsesPermissionLine = new Regex(
            @"^uses-permission: name='([^']+)'", RegexOptions.CultureInvariant | RegexOptions.Multiline);

        /// <summary>
        /// Returns the permissions listed on the <c>uses-permission</c> lines of
        /// <c>aapt dump permissions</c>' output. Called by <see cref="Read"/> and by
        /// <c>ApkPermissionsTests</c>.
        /// </summary>
        public static IReadOnlyList<string> Parse(string aaptOutput)
        {
            return ParseOutput(aaptOutput);
        }

        /// <summary>
        /// Runs the newest <c>aapt</c> under <paramref name="sdkRoot"/> on <paramref name="apkPath"/>
        /// and returns the permissions it asks for. Throws an
        /// <see cref="System.InvalidOperationException"/> when <c>aapt</c> is missing or fails.
        /// Called by <see cref="BuildScript"/> after each build.
        /// </summary>
        public static IReadOnlyList<string> Read(string sdkRoot, string apkPath)
        {
            string aapt = AndroidBuildTools.FindNewest(sdkRoot, Path.DirectorySeparatorChar == '\\' ? "aapt.exe" : "aapt");
            return ParseOutput(AndroidBuildTools.Run(aapt, "dump permissions \"" + apkPath + "\"", "aapt dump permissions"));
        }

        private static IReadOnlyList<string> ParseOutput(string aaptOutput)
        {
            return UsesPermissionLine.Matches(aaptOutput ?? string.Empty)
                .Cast<Match>()
                .Select(match => match.Groups[1].Value)
                .ToArray();
        }
    }
}
