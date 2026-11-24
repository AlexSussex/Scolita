using NUnit.Framework;
using Scolita.Editor;
using UnityEditor;

namespace Scolita.Tests.EditMode.Build
{
    /// <summary>Tests <see cref="AndroidSettingsSnapshot"/>, which puts the build's setting changes back.</summary>
    public sealed class AndroidSettingsSnapshotTests
    {
        /// <summary>Tests that <see cref="AndroidSettingsSnapshot.Restore"/> puts back the version, the keystore switch and the build type.</summary>
        [Test]
        public void TestRestore_PutsBackVersionKeystoreSwitchAndBuildType()
        {
            string bundleVersion = PlayerSettings.bundleVersion;
            int bundleVersionCode = PlayerSettings.Android.bundleVersionCode;
            bool useCustomKeystore = PlayerSettings.Android.useCustomKeystore;
            AndroidBuildType androidBuildType = EditorUserBuildSettings.androidBuildType;
            AndroidSettingsSnapshot snapshot = AndroidSettingsSnapshot.Take();
            try
            {
                PlayerSettings.bundleVersion = "9.9.9";
                PlayerSettings.Android.bundleVersionCode = 99999;
                PlayerSettings.Android.useCustomKeystore = !useCustomKeystore;
                EditorUserBuildSettings.androidBuildType = androidBuildType == AndroidBuildType.Release
                    ? AndroidBuildType.Debug
                    : AndroidBuildType.Release;

                snapshot.Restore();

                Assert.That(PlayerSettings.bundleVersion, Is.EqualTo(bundleVersion));
                Assert.That(PlayerSettings.Android.bundleVersionCode, Is.EqualTo(bundleVersionCode));
                Assert.That(PlayerSettings.Android.useCustomKeystore, Is.EqualTo(useCustomKeystore));
                Assert.That(EditorUserBuildSettings.androidBuildType, Is.EqualTo(androidBuildType));
            }
            finally
            {
                PlayerSettings.bundleVersion = bundleVersion;
                PlayerSettings.Android.bundleVersionCode = bundleVersionCode;
                PlayerSettings.Android.useCustomKeystore = useCustomKeystore;
                EditorUserBuildSettings.androidBuildType = androidBuildType;
            }
        }
    }
}
