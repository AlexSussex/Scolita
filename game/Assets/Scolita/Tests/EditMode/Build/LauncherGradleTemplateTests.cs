using System.IO;
using NUnit.Framework;
using Scolita.Editor;
using UnityEngine;

namespace Scolita.Tests.EditMode.Build
{
    /// <summary>
    /// Tests the launcher's Gradle template (<c>Assets/Plugins/Android/launcherTemplate.gradle</c>),
    /// which signs a release build from the <see cref="GradleSigningFile"/> at Gradle time.
    /// </summary>
    public sealed class LauncherGradleTemplateTests
    {
        /// <summary>
        /// Tests that the template reads the file named by <see cref="GradleSigningFile.PathVariable"/>,
        /// with the four names <see cref="GradleSigningContents"/> writes, that Player Settings use the
        /// template, and that no custom Gradle properties template is used (one could turn on Gradle's
        /// configuration cache, which would store the signing config, passwords included).
        /// </summary>
        [Test]
        public void TestLauncherTemplate_SignsFromTheSigningFileAndIsUsed()
        {
            string projectFolder = Directory.GetParent(Application.dataPath).FullName;

            string template = File.ReadAllText(Path.Combine(Application.dataPath, "Plugins/Android/launcherTemplate.gradle"));
            string playerSettings = File.ReadAllText(Path.Combine(projectFolder, "ProjectSettings/ProjectSettings.asset"));

            Assert.That(template, Does.Contain("System.getenv('" + GradleSigningFile.PathVariable + "')"));
            Assert.That(template, Does.Contain("storePassword scolitaSigning.keystorePassword"));
            Assert.That(template, Does.Contain("keyPassword scolitaSigning.keyPassword"));
            Assert.That(template, Does.Contain("storeFile new File(scolitaSigning.keystorePath)"));
            Assert.That(template, Does.Contain("keyAlias scolitaSigning.keyAlias"));
            Assert.That(template, Does.Contain("android.buildTypes.release.signingConfig android.signingConfigs.scolitaRelease"));
            Assert.That(playerSettings, Does.Contain("useCustomLauncherGradleManifest: 1"));
            Assert.That(playerSettings, Does.Contain("androidUseCustomKeystore: 0"));
            Assert.That(playerSettings, Does.Contain("useCustomGradlePropertiesTemplate: 0"));
            Assert.That(template, Does.Not.Contain("configuration-cache"));
        }
    }
}
