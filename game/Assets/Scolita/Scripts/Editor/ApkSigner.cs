using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Scolita.Editor
{
    /// <summary>
    /// Reads who signed an APK, with the Android SDK's <c>apksigner verify --print-certs</c> run on
    /// Unity's own JDK. Used by <see cref="BuildScript"/> to refuse a release APK signed with the
    /// debug key, which would not install over her copy and would lose her progress.
    /// </summary>
    public static class ApkSigner
    {
        /// <summary>The certificate subject of Android's debug key.</summary>
        public const string DebugCertificateName = "CN=Android Debug";

        private static readonly Regex CertificateLine = new Regex(
            @"^Signer #\d+ certificate DN: (.+?)\s*$", RegexOptions.CultureInvariant | RegexOptions.Multiline);

        /// <summary>
        /// Returns the certificate subjects (<c>CN=…, O=…</c>) on the signer lines of
        /// <c>apksigner verify --print-certs</c>' output. Called by <see cref="Read"/> and by
        /// <c>ApkSignerTests</c>.
        /// </summary>
        public static IReadOnlyList<string> Parse(string apksignerOutput)
        {
            return ParseOutput(apksignerOutput);
        }

        /// <summary>
        /// Verifies <paramref name="apkPath"/>'s signature with the newest <c>apksigner.jar</c> under
        /// <paramref name="sdkRoot"/>, run by the <c>java</c> under <paramref name="jdkRoot"/>, and
        /// returns its signers' certificate subjects. Throws an
        /// <see cref="InvalidOperationException"/> when a tool is missing or the signature isn't
        /// valid. Called by <see cref="BuildScript"/> after a release build.
        /// </summary>
        public static IReadOnlyList<string> Read(string jdkRoot, string sdkRoot, string apkPath)
        {
            string java = Path.Combine(jdkRoot ?? string.Empty, "bin", Path.DirectorySeparatorChar == '\\' ? "java.exe" : "java");
            if (!File.Exists(java))
            {
                throw new InvalidOperationException("java was not found at " + java
                    + "; install Android Build Support's OpenJDK in Unity Hub.");
            }

            string jar = AndroidBuildTools.FindNewest(sdkRoot, Path.Combine("lib", "apksigner.jar"));
            return ParseOutput(AndroidBuildTools.Run(java, "-jar \"" + jar + "\" verify --print-certs \"" + apkPath + "\"",
                "apksigner verify"));
        }

        private static IReadOnlyList<string> ParseOutput(string apksignerOutput)
        {
            return CertificateLine.Matches(apksignerOutput ?? string.Empty)
                .Cast<Match>()
                .Select(match => match.Groups[1].Value)
                .ToArray();
        }
    }
}
