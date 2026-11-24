using NUnit.Framework;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Scolita.Tests.EditMode.Build
{
    /// <summary>
    /// Tests the committed Player Settings the brief asks for (<see cref="PlayerSettings"/>): the
    /// names, landscape only, IL2CPP on ARM64, Android 8.0 and up, no splash, offline.
    /// </summary>
    public sealed class PlayerSettingsTests
    {
        /// <summary>Tests that <see cref="PlayerSettings.productName"/> is Școlița with the comma-below Ș and ț, and the ids.</summary>
        [Test]
        public void TestProductName_IsScolitaWithCommaBelowLetters()
        {
            Assert.That(PlayerSettings.productName, Is.EqualTo("\u0218coli\u021Ba"));
            Assert.That(PlayerSettings.companyName, Is.EqualTo("softshack"));
            Assert.That(PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android), Is.EqualTo("com.softshack.scolita"));
        }

        /// <summary>Tests that <see cref="PlayerSettings.allowedAutorotateToPortrait"/> and its siblings allow landscape only.</summary>
        [Test]
        public void TestAllowedAutorotate_LandscapeLeftAndRightOnly()
        {
            Assert.That(PlayerSettings.defaultInterfaceOrientation, Is.EqualTo(UIOrientation.AutoRotation));
            Assert.That(PlayerSettings.allowedAutorotateToLandscapeLeft, Is.True);
            Assert.That(PlayerSettings.allowedAutorotateToLandscapeRight, Is.True);
            Assert.That(PlayerSettings.allowedAutorotateToPortrait, Is.False);
            Assert.That(PlayerSettings.allowedAutorotateToPortraitUpsideDown, Is.False);
        }

        /// <summary>Tests that <see cref="PlayerSettings.Android"/> builds IL2CPP for ARM64, from Android 8.0 (API 26).</summary>
        [Test]
        public void TestAndroid_Il2cppArm64FromApi26()
        {
            Assert.That(PlayerSettings.GetScriptingBackend(NamedBuildTarget.Android), Is.EqualTo(ScriptingImplementation.IL2CPP));
            Assert.That(PlayerSettings.Android.targetArchitectures, Is.EqualTo(AndroidArchitecture.ARM64));
            Assert.That(PlayerSettings.Android.minSdkVersion, Is.EqualTo(AndroidSdkVersions.AndroidApiLevel26));
            Assert.That(PlayerSettings.Android.targetSdkVersion, Is.EqualTo(AndroidSdkVersions.AndroidApiLevelAuto));
            Assert.That(PlayerSettings.Android.forceInternetPermission, Is.False);
            Assert.That(PlayerSettings.Android.useCustomKeystore, Is.False);
        }

        /// <summary>Tests that <see cref="PlayerSettings.SplashScreen"/> is off and only the Input System package reads input.</summary>
        [Test]
        public void TestSplashScreen_OffAndNewInputOnly()
        {
            SerializedObject player = new SerializedObject(Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings"));

            Assert.That(PlayerSettings.SplashScreen.show, Is.False);
            Assert.That(PlayerSettings.SplashScreen.showUnityLogo, Is.False);
            Assert.That(player.FindProperty("activeInputHandler").intValue, Is.EqualTo(1));
            Assert.That(player.FindProperty("submitAnalytics").boolValue, Is.False);
        }

        /// <summary>Tests that <see cref="EditorSettings.serializationMode"/> is Force Text, with visible meta files.</summary>
        [Test]
        public void TestSerializationMode_ForceTextWithVisibleMetaFiles()
        {
            Assert.That(EditorSettings.serializationMode, Is.EqualTo(SerializationMode.ForceText));
            Assert.That(VersionControlSettings.mode, Is.EqualTo("Visible Meta Files"));
        }
    }
}
