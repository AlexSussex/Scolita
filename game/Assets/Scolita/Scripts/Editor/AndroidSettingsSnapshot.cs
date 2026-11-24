using UnityEditor;

namespace Scolita.Editor
{
    /// <summary>
    /// The Android settings a build changes (the version, the version code, whether Unity signs
    /// with a keystore of its own, and the Gradle build type) as they were before it, so
    /// <see cref="BuildScript"/> can put them back afterwards and the project's settings file stays
    /// as committed.
    /// </summary>
    public sealed class AndroidSettingsSnapshot
    {
        private readonly string bundleVersion;
        private readonly int bundleVersionCode;
        private readonly bool useCustomKeystore;
        private readonly AndroidBuildType androidBuildType;

        private AndroidSettingsSnapshot()
        {
            bundleVersion = PlayerSettings.bundleVersion;
            bundleVersionCode = PlayerSettings.Android.bundleVersionCode;
            useCustomKeystore = PlayerSettings.Android.useCustomKeystore;
            androidBuildType = EditorUserBuildSettings.androidBuildType;
        }

        /// <summary>
        /// Records the current settings; called by <see cref="BuildScript"/> before a build and by
        /// <c>AndroidSettingsSnapshotTests</c>.
        /// </summary>
        public static AndroidSettingsSnapshot Take()
        {
            return new AndroidSettingsSnapshot();
        }

        /// <summary>
        /// Puts back each recorded setting that the build changed, leaving the others untouched so
        /// Unity doesn't rewrite them. Called by <see cref="BuildScript"/> after a build and by
        /// <c>AndroidSettingsSnapshotTests</c>.
        /// </summary>
        public void Restore()
        {
            if (PlayerSettings.bundleVersion != bundleVersion)
            {
                PlayerSettings.bundleVersion = bundleVersion;
            }

            if (PlayerSettings.Android.bundleVersionCode != bundleVersionCode)
            {
                PlayerSettings.Android.bundleVersionCode = bundleVersionCode;
            }

            if (PlayerSettings.Android.useCustomKeystore != useCustomKeystore)
            {
                PlayerSettings.Android.useCustomKeystore = useCustomKeystore;
            }

            if (EditorUserBuildSettings.androidBuildType != androidBuildType)
            {
                EditorUserBuildSettings.androidBuildType = androidBuildType;
            }
        }
    }
}
