using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Process = System.Diagnostics.Process;

namespace Scolita.Editor
{
    /// <summary>
    /// Builds the Android APKs in batch mode (README → "Build a debug APK" and "Build a release
    /// APK"). The version comes from the repository's <c>VERSION</c> file; the APK is written to
    /// <c>game/Builds/Android</c>. In batch mode any failure ends Unity with exit code 1.
    /// The release key never goes into Unity's settings: its four environment variables are read
    /// and removed from the process at the start of every build (Unity prints the whole environment
    /// to the log when Gradle fails), and a release build hands the key to Gradle through a
    /// <see cref="GradleSigningFile"/>, deleted afterwards. Every APK is checked not to ask for the
    /// internet, and a release APK to be signed with the release key and not debuggable.
    /// </summary>
    public static class BuildScript
    {
        private const string VersionFileName = "VERSION";
        private const string OutputFolder = "Builds/Android";
        private const string ApkNamePrefix = "Scolita-";
        private const string DebugApkSuffix = "-debug";
        private const string ApkExtension = ".apk";
        private const string BootScenePath = "Assets/Scolita/Scenes/Boot.unity";
        private const int FailureExitCode = 1;

        /// <summary>
        /// Builds <c>Scolita-&lt;version&gt;-debug.apk</c>: a development build signed with the
        /// debug key. Called from the command line with <c>-executeMethod</c>.
        /// </summary>
        public static void BuildAndroidDebug()
        {
            Run(AndroidBuildKind.Debug);
        }

        /// <summary>
        /// Builds <c>Scolita-&lt;version&gt;.apk</c>, signed with the release key named by the
        /// environment variables in <see cref="ReleaseSigning"/>; refuses to build without them.
        /// Called from the command line with <c>-executeMethod</c>.
        /// </summary>
        public static void BuildAndroidRelease()
        {
            Run(AndroidBuildKind.Release);
        }

        private static void Run(AndroidBuildKind kind)
        {
            try
            {
                BuildApk(kind);
            }
            catch (Exception exception) when (Application.isBatchMode)
            {
                Debug.LogError("Scolita build failed: " + exception.Message);
                Debug.LogException(exception);
                EditorApplication.Exit(FailureExitCode);
            }
        }

        private static void BuildApk(AndroidBuildKind kind)
        {
            if (kind == AndroidBuildKind.Release)
            {
                GradleSigningFile.DeleteLeftovers(GradleSigningFile.Root, IsProcessRunning);
            }

            ReleaseSigning signing = TakeSigningVariables(kind);
            string projectFolder = Directory.GetParent(Application.dataPath).FullName;
            AppVersion version = AppVersion.ReadFile(
                Path.Combine(Directory.GetParent(projectFolder).FullName, VersionFileName));
            if (signing != null && !File.Exists(Path.GetFullPath(signing.KeystorePath)))
            {
                throw new FileNotFoundException("The keystore named by " + ReleaseSigning.KeystorePathVariable
                    + " does not exist.");
            }

            string[] scenes = ScenesToBuild();
            string apkPath = Path.Combine(projectFolder, OutputFolder, ApkNameOf(version, kind));
            Directory.CreateDirectory(Path.GetDirectoryName(apkPath));
            BuildPlayer(kind, version, signing, scenes, apkPath);

            RequireNoInternetPermission(apkPath);
            if (kind == AndroidBuildKind.Release)
            {
                RequireReleaseKey(apkPath);
                RequireNotDebuggable(apkPath);
            }

            Debug.Log("Scolita " + version + " built: " + apkPath);
        }

        private static void BuildPlayer(AndroidBuildKind kind, AppVersion version, ReleaseSigning signing, string[] scenes,
            string apkPath)
        {
            string signingFolder = signing != null
                ? GradleSigningFile.NewBuildFolder(GradleSigningFile.Root, Process.GetCurrentProcess().Id)
                : null;
            AndroidSettingsSnapshot snapshot = AndroidSettingsSnapshot.Take();
            Exception cleanupFailure;
            try
            {
                ApplySettings(kind, version);
                if (signing != null)
                {
                    Environment.SetEnvironmentVariable(GradleSigningFile.PathVariable,
                        GradleSigningFile.Write(signingFolder, signing));
                }

                BuildPlayerOptions options = new BuildPlayerOptions
                {
                    scenes = scenes,
                    locationPathName = apkPath,
                    target = BuildTarget.Android,
                    targetGroup = BuildTargetGroup.Android,
                    options = kind == AndroidBuildKind.Debug ? BuildOptions.Development : BuildOptions.None
                };
                BuildReport report = BuildPipeline.BuildPlayer(options);
                if (report.summary.result != BuildResult.Succeeded)
                {
                    throw new BuildFailedException("The Android build ended as " + report.summary.result
                        + " with " + report.summary.totalErrors + " errors; see the log above.");
                }
            }
            finally
            {
                Environment.SetEnvironmentVariable(GradleSigningFile.PathVariable, null);
                cleanupFailure = TryDeleteSigningFolder(signingFolder);
                snapshot.Restore();
            }

            if (cleanupFailure != null)
            {
                throw cleanupFailure;
            }
        }

        private static void ApplySettings(AndroidBuildKind kind, AppVersion version)
        {
            PlayerSettings.bundleVersion = version.ToString();
            PlayerSettings.Android.bundleVersionCode = version.VersionCode;
            if (PlayerSettings.Android.useCustomKeystore)
            {
                PlayerSettings.Android.useCustomKeystore = false;
            }

            EditorUserBuildSettings.buildAppBundle = false;
            EditorUserBuildSettings.androidBuildType = kind == AndroidBuildKind.Debug
                ? AndroidBuildType.Development
                : AndroidBuildType.Release;
        }

        private static Exception TryDeleteSigningFolder(string signingFolder)
        {
            if (signingFolder == null)
            {
                return null;
            }

            try
            {
                GradleSigningFile.DeleteFolder(signingFolder);
                return null;
            }
            catch (IOException exception)
            {
                Debug.LogError("Scolita: " + exception.Message);
                return exception;
            }
        }

        private static bool IsProcessRunning(int processId)
        {
            try
            {
                using (Process process = Process.GetProcessById(processId))
                {
                    return !process.HasExited;
                }
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        private static ReleaseSigning TakeSigningVariables(AndroidBuildKind kind)
        {
            try
            {
                return kind == AndroidBuildKind.Release ? ReleaseSigning.Read(Environment.GetEnvironmentVariable) : null;
            }
            finally
            {
                foreach (string name in ReleaseSigning.VariableNames)
                {
                    Environment.SetEnvironmentVariable(name, null);
                }

                Environment.SetEnvironmentVariable(GradleSigningFile.PathVariable, null);
            }
        }

        private static void RequireNoInternetPermission(string apkPath)
        {
            IReadOnlyList<string> permissions = ApkPermissions.Read(AndroidExternalToolsSettings.sdkRootPath, apkPath);
            if (permissions.Contains(ApkPermissions.Internet))
            {
                File.Delete(apkPath);
                throw new BuildFailedException("The APK asked for " + ApkPermissions.Internet + ", which Școlița "
                    + "never needs; it was deleted. Check Assets/Plugins/Android/LauncherManifest.xml and Player "
                    + "Settings → Publishing Settings → Custom Launcher Manifest.");
            }
        }

        private static void RequireReleaseKey(string apkPath)
        {
            IReadOnlyList<string> signers = ApkSigner.Read(AndroidExternalToolsSettings.jdkRootPath,
                AndroidExternalToolsSettings.sdkRootPath, apkPath);
            if (signers.Count == 0 || signers.Any(signer => signer.Contains(ApkSigner.DebugCertificateName)))
            {
                File.Delete(apkPath);
                throw new BuildFailedException("The release APK was not signed with the release key (signers: "
                    + string.Join("; ", signers) + "); it was deleted. Check Assets/Plugins/Android/"
                    + "launcherTemplate.gradle and Player Settings → Publishing Settings → Custom Launcher Gradle Template.");
            }
        }

        private static void RequireNotDebuggable(string apkPath)
        {
            if (ApkBadging.ReadIsDebuggable(AndroidExternalToolsSettings.sdkRootPath, apkPath))
            {
                File.Delete(apkPath);
                throw new BuildFailedException("The release APK is debuggable; it was deleted. Check File → Build "
                    + "Profiles → Android → Development Build and the Android build type.");
            }
        }

        private static string[] ScenesToBuild()
        {
            string[] scenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();
            if (scenes.Length == 0 || scenes[0] != BootScenePath)
            {
                throw new BuildFailedException("The first scene in the build settings must be " + BootScenePath + ".");
            }

            return scenes;
        }

        private static string ApkNameOf(AppVersion version, AndroidBuildKind kind)
        {
            string suffix = kind == AndroidBuildKind.Debug ? DebugApkSuffix : string.Empty;
            return ApkNamePrefix + version + suffix + ApkExtension;
        }
    }
}
