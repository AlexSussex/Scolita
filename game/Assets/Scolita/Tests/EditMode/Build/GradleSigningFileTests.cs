using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Scolita.Editor;
using UnityEngine;

namespace Scolita.Tests.EditMode.Build
{
    /// <summary>Tests <see cref="GradleSigningFile"/>, which hands the release key to Gradle for one build.</summary>
    public sealed class GradleSigningFileTests
    {
        /// <summary>Tests that <see cref="GradleSigningFile.Write"/> writes the four values as JSON that reads back unchanged.</summary>
        [Test]
        public void TestWrite_WritesTheFourValuesAsJson()
        {
            string root = NewRoot();
            try
            {
                ReleaseSigning signing = SigningOf(@"C:\keys\scolita release.keystore", "p\u0103ss \"quoted\" \\ word",
                    "scolita", "key-secret");

                string path = GradleSigningFile.Write(GradleSigningFile.NewBuildFolder(root, 42), signing);
                string json = File.ReadAllText(path);
                GradleSigningContents read = JsonUtility.FromJson<GradleSigningContents>(json);

                Assert.That(Path.GetDirectoryName(Path.GetDirectoryName(path)), Is.EqualTo(root));
                Assert.That(json, Does.Contain("\"keystorePath\"").And.Contain("\"keystorePassword\"")
                    .And.Contain("\"keyAlias\"").And.Contain("\"keyPassword\""));
                Assert.That(read.KeystorePath, Is.EqualTo(signing.KeystorePath));
                Assert.That(read.KeystorePassword, Is.EqualTo(signing.KeystorePassword));
                Assert.That(read.KeyAlias, Is.EqualTo(signing.KeyAlias));
                Assert.That(read.KeyPassword, Is.EqualTo(signing.KeyPassword));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>Tests that <see cref="GradleSigningFile.Write"/> gives Gradle the keystore's full path, never a relative one.</summary>
        [Test]
        public void TestWrite_MakesTheKeystorePathFull()
        {
            string root = NewRoot();
            try
            {
                string path = GradleSigningFile.Write(GradleSigningFile.NewBuildFolder(root, 42),
                    SigningOf("keys/release.keystore", "a", "b", "c"));

                GradleSigningContents read = JsonUtility.FromJson<GradleSigningContents>(File.ReadAllText(path));

                Assert.That(Path.IsPathRooted(read.KeystorePath), Is.True);
                Assert.That(read.KeystorePath, Is.EqualTo(Path.GetFullPath("keys/release.keystore")));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>Tests that <see cref="GradleSigningFile.NewBuildFolder"/> names a new folder per build, starting with the process id.</summary>
        [Test]
        public void TestNewBuildFolder_IsNewPerBuildAndNamesTheProcess()
        {
            string first = GradleSigningFile.NewBuildFolder("C:/root", 1234);
            string second = GradleSigningFile.NewBuildFolder("C:/root", 1234);

            Assert.That(first, Is.Not.EqualTo(second));
            Assert.That(Path.GetFileName(first), Does.StartWith("1234-"));
            Assert.That(Path.GetDirectoryName(first), Is.EqualTo(Path.GetDirectoryName("C:/root/x")));
        }

        /// <summary>Tests that <see cref="GradleSigningFile.DeleteFolder"/> removes the folder and the file in it, and ignores a missing one.</summary>
        [Test]
        public void TestDeleteFolder_RemovesTheFolderAndItsFile()
        {
            string root = NewRoot();
            try
            {
                string folder = GradleSigningFile.NewBuildFolder(root, 7);
                GradleSigningFile.Write(folder, SigningOf("C:/k.keystore", "a", "b", "c"));

                GradleSigningFile.DeleteFolder(folder);

                Assert.That(Directory.Exists(folder), Is.False);
                Assert.That(() => GradleSigningFile.DeleteFolder(folder), Throws.Nothing);
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>
        /// Tests that <see cref="GradleSigningFile.DeleteLeftovers"/> deletes the folders of builds whose
        /// process is gone and loose files, and leaves the folders of builds still running.
        /// </summary>
        [Test]
        public void TestDeleteLeftovers_KeepsOnlyRunningBuilds()
        {
            string root = NewRoot();
            try
            {
                string running = GradleSigningFile.NewBuildFolder(root, 100);
                string killed = GradleSigningFile.NewBuildFolder(root, 200);
                string strange = Path.Combine(root, "not-a-build");
                GradleSigningFile.Write(running, SigningOf("C:/k.keystore", "a", "b", "c"));
                GradleSigningFile.Write(killed, SigningOf("C:/k.keystore", "a", "b", "c"));
                Directory.CreateDirectory(strange);
                File.WriteAllText(Path.Combine(root, "release-signing.json"), "{}");

                GradleSigningFile.DeleteLeftovers(root, processId => processId == 100);

                Assert.That(Directory.Exists(running), Is.True);
                Assert.That(Directory.Exists(killed), Is.False);
                Assert.That(Directory.Exists(strange), Is.False);
                Assert.That(Directory.GetFiles(root), Is.Empty);
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>Tests that <see cref="GradleSigningFile.DeleteLeftovers"/> does nothing when the root doesn't exist.</summary>
        [Test]
        public void TestDeleteLeftovers_MissingRootIsFine()
        {
            string root = Path.Combine(Path.GetTempPath(), "scolita-signing-test-" + Guid.NewGuid().ToString("N"));

            Assert.That(() => GradleSigningFile.DeleteLeftovers(root, processId => false), Throws.Nothing);
        }

        /// <summary>Tests that <see cref="GradleSigningFile.Root"/> is in the user's temporary folder, outside the project.</summary>
        [Test]
        public void TestRoot_IsOutsideTheProject()
        {
            string projectFolder = Directory.GetParent(Application.dataPath).FullName;

            Assert.That(GradleSigningFile.Root, Does.StartWith(Path.GetTempPath()));
            Assert.That(Path.GetFullPath(GradleSigningFile.Root), Does.Not.StartWith(projectFolder));
        }

        private static string NewRoot()
        {
            string root = Path.Combine(Path.GetTempPath(), "scolita-signing-test-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(root);
            return root;
        }

        private static ReleaseSigning SigningOf(string path, string storePassword, string alias, string keyPassword)
        {
            Dictionary<string, string> variables = new Dictionary<string, string>
            {
                { ReleaseSigning.KeystorePathVariable, path },
                { ReleaseSigning.KeystorePasswordVariable, storePassword },
                { ReleaseSigning.KeyAliasVariable, alias },
                { ReleaseSigning.KeyPasswordVariable, keyPassword }
            };
            return ReleaseSigning.Read(name => variables.TryGetValue(name, out string value) ? value : null);
        }
    }
}
