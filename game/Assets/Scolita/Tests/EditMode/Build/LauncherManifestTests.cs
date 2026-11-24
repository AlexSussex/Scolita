using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace Scolita.Tests.EditMode.Build
{
    /// <summary>
    /// Tests the custom launcher manifest that keeps the INTERNET permission out of every APK
    /// (<c>Assets/Plugins/Android/LauncherManifest.xml</c>).
    /// </summary>
    public sealed class LauncherManifestTests
    {
        /// <summary>
        /// Tests that the launcher manifest has the line removing INTERNET, and that Player Settings use
        /// that manifest.
        /// </summary>
        [Test]
        public void TestLauncherManifest_RemovesInternetAndIsUsed()
        {
            string projectFolder = Directory.GetParent(Application.dataPath).FullName;

            string manifest = File.ReadAllText(Path.Combine(Application.dataPath, "Plugins/Android/LauncherManifest.xml"));
            string playerSettings = File.ReadAllText(Path.Combine(projectFolder, "ProjectSettings/ProjectSettings.asset"));

            Assert.That(manifest, Does.Contain(
                "<uses-permission android:name=\"android.permission.INTERNET\" tools:node=\"remove\"/>"));
            Assert.That(playerSettings, Does.Contain("useCustomLauncherManifest: 1"));
        }
    }
}
