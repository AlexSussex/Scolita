using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Scolita.Core.Languages;
using Scolita.Core.Profiles;
using Scolita.Game.Saving;
using Scolita.Game.Settings;
using UnityEngine;
using UnityEngine.TestTools;

namespace Scolita.Tests.EditMode.Saving
{
    /// <summary>
    /// Tests <see cref="SaveStore"/> with the tablet's settings and a profile's learning settings, in
    /// a fresh temporary folder per test.
    /// </summary>
    public sealed class SaveStoreTests
    {
        /// <summary>Tests that <see cref="SaveStore.Save{T}"/> then <see cref="SaveStore.Load{T}"/> gives back the same settings.</summary>
        [Test]
        public void TestLoad_RoundTripKeepsEveryValue()
        {
            string root = CreateTemporaryFolder();
            try
            {
                SaveStore store = SaveStore.ForTablet(root);
                TabletSettingsData saved = new TabletSettingsData(false, true, 0.25f, false, Language.English);

                bool written = store.Save(TabletSettingsData.FileName, saved);
                SaveLoadResult<TabletSettingsData> loaded = store.Load(TabletSettingsData.FileName, TabletSettingsData.CreateDefaults());

                Assert.That(written, Is.True);
                Assert.That(loaded.Outcome, Is.EqualTo(SaveLoadOutcome.Loaded));
                Assert.That(loaded.Data.Version, Is.EqualTo(1));
                Assert.That(loaded.Data.MusicOn, Is.False);
                Assert.That(loaded.Data.SoundsOn, Is.True);
                Assert.That(loaded.Data.VoiceVolume, Is.EqualTo(0.25f).Within(0.0001f));
                Assert.That(loaded.Data.VibrationOn, Is.False);
                Assert.That(loaded.Data.AppLanguage, Is.EqualTo(Language.English));
                Assert.That(File.Exists(Path.Combine(root, "settings.json")), Is.True);
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>Tests that <see cref="SaveStore.ForProfile"/> keeps a profile's files in <c>profiles/&lt;id&gt;/</c>.</summary>
        [Test]
        public void TestForProfile_SavesInTheProfilesFolder()
        {
            string root = CreateTemporaryFolder();
            try
            {
                SaveStore store = SaveStore.ForProfile(root, ProfileId.Create("child-1"));

                bool written = store.Save(LearningSettingsData.FileName, new LearningSettingsData(LanguageMix.RomanianOnly));
                SaveLoadResult<LearningSettingsData> loaded =
                    store.Load(LearningSettingsData.FileName, LearningSettingsData.CreateDefaults());

                Assert.That(written, Is.True);
                Assert.That(store.Folder, Is.EqualTo(Path.Combine(root, "profiles", "child-1")));
                Assert.That(File.Exists(Path.Combine(root, "profiles", "child-1", "learning-settings.json")), Is.True);
                Assert.That(loaded.Outcome, Is.EqualTo(SaveLoadOutcome.Loaded));
                Assert.That(loaded.Data.LanguageMix, Is.EqualTo(LanguageMix.RomanianOnly));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>
        /// Tests that <see cref="SaveStore.Save{T}"/> for one profile never touches another's file, nor
        /// the tablet's file of the same name.
        /// </summary>
        [Test]
        public void TestSave_OneProfileLeavesAnotherAlone()
        {
            string root = CreateTemporaryFolder();
            try
            {
                SaveStore first = SaveStore.ForProfile(root, ProfileId.Create("child-1"));
                SaveStore second = SaveStore.ForProfile(root, ProfileId.Create("child-2"));
                first.Save(LearningSettingsData.FileName, new LearningSettingsData(LanguageMix.EnglishOnly));

                second.Save(LearningSettingsData.FileName, new LearningSettingsData(LanguageMix.RomanianOnly));
                SaveLoadResult<LearningSettingsData> firstLoaded =
                    first.Load(LearningSettingsData.FileName, LearningSettingsData.CreateDefaults());
                SaveLoadResult<LearningSettingsData> tabletLoaded =
                    SaveStore.ForTablet(root).Load(LearningSettingsData.FileName, LearningSettingsData.CreateDefaults());

                Assert.That(firstLoaded.Data.LanguageMix, Is.EqualTo(LanguageMix.EnglishOnly));
                Assert.That(tabletLoaded.Outcome, Is.EqualTo(SaveLoadOutcome.Missing));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>Tests that <see cref="SaveStore.Save{T}"/> replaces an existing file and leaves no temporary file.</summary>
        [Test]
        public void TestSave_OverwritesTheExistingFile()
        {
            string root = CreateTemporaryFolder();
            try
            {
                SaveStore store = SaveStore.ForTablet(root);
                store.Save(TabletSettingsData.FileName, TabletSettingsData.CreateDefaults());

                store.Save(TabletSettingsData.FileName, new TabletSettingsData(false, false, 0.5f, false, Language.English));
                SaveLoadResult<TabletSettingsData> loaded = store.Load(TabletSettingsData.FileName, TabletSettingsData.CreateDefaults());

                Assert.That(loaded.Data.MusicOn, Is.False);
                Assert.That(loaded.Data.AppLanguage, Is.EqualTo(Language.English));
                Assert.That(Directory.GetFiles(root), Has.Length.EqualTo(1));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>Tests that <see cref="SaveStore.Save{T}"/> writes UTF-8 without a byte order mark.</summary>
        [Test]
        public void TestSave_WritesUtf8WithoutByteOrderMark()
        {
            string root = CreateTemporaryFolder();
            try
            {
                SaveStore.ForTablet(root).Save(TabletSettingsData.FileName, TabletSettingsData.CreateDefaults());

                byte[] bytes = File.ReadAllBytes(Path.Combine(root, "settings.json"));

                Assert.That(bytes[0], Is.EqualTo((byte)'{'));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>Tests that <see cref="SaveStore.Save{T}"/> creates a profile's folder the first time.</summary>
        [Test]
        public void TestSave_CreatesAMissingFolder()
        {
            string root = CreateTemporaryFolder();
            try
            {
                string missingRoot = Path.Combine(root, "not-yet");
                SaveStore store = SaveStore.ForProfile(missingRoot, ProfileId.Create("child-1"));

                bool written = store.Save(LearningSettingsData.FileName, LearningSettingsData.CreateDefaults());

                Assert.That(written, Is.True);
                Assert.That(File.Exists(Path.Combine(missingRoot, "profiles", "child-1", "learning-settings.json")), Is.True);
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>Tests that <see cref="SaveStore.Load{T}"/> gives the defaults when the file doesn't exist.</summary>
        [Test]
        public void TestLoad_MissingFileGivesDefaults()
        {
            string root = CreateTemporaryFolder();
            try
            {
                SaveStore store = SaveStore.ForTablet(root);
                TabletSettingsData defaults = TabletSettingsData.CreateDefaults();

                SaveLoadResult<TabletSettingsData> loaded = store.Load(TabletSettingsData.FileName, defaults);

                Assert.That(loaded.Outcome, Is.EqualTo(SaveLoadOutcome.Missing));
                Assert.That(loaded.Data, Is.SameAs(defaults));
                Assert.That(Directory.GetFiles(root), Is.Empty);
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>Tests that <see cref="SaveStore.Load{T}"/> sets aside a file that isn't JSON, and gives the defaults.</summary>
        [Test]
        public void TestLoad_BrokenFileIsSetAsideAndGivesDefaults()
        {
            string root = CreateTemporaryFolder();
            try
            {
                File.WriteAllText(Path.Combine(root, "settings.json"), "{{{");
                SaveStore store = SaveStore.ForTablet(root);
                TabletSettingsData defaults = TabletSettingsData.CreateDefaults();
                LogAssert.Expect(LogType.Warning, new Regex("settings\\.json could not be read"));

                SaveLoadResult<TabletSettingsData> loaded = store.Load(TabletSettingsData.FileName, defaults);

                Assert.That(loaded.Outcome, Is.EqualTo(SaveLoadOutcome.SetAside));
                Assert.That(loaded.Data, Is.SameAs(defaults));
                Assert.That(File.Exists(Path.Combine(root, "settings.json")), Is.False);
                string[] broken = BrokenCopies(root, "settings");
                Assert.That(broken, Has.Length.EqualTo(1));
                Assert.That(Path.GetFileName(broken[0]), Does.Match(@"^settings\.broken-\d{8}-\d{6}\.json$"));
                Assert.That(File.ReadAllText(broken[0]), Is.EqualTo("{{{"));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>Tests that <see cref="SaveStore.Load{T}"/> sets aside a broken file in a profile's folder, there.</summary>
        [Test]
        public void TestLoad_BrokenProfileFileIsSetAsideInItsFolder()
        {
            string root = CreateTemporaryFolder();
            try
            {
                string folder = Path.Combine(root, "profiles", "child-1");
                Directory.CreateDirectory(folder);
                File.WriteAllText(Path.Combine(folder, "learning-settings.json"), "not json");
                SaveStore store = SaveStore.ForProfile(root, ProfileId.Create("child-1"));
                LogAssert.Expect(LogType.Warning, new Regex("learning-settings\\.json could not be read"));

                SaveLoadResult<LearningSettingsData> loaded =
                    store.Load(LearningSettingsData.FileName, LearningSettingsData.CreateDefaults());

                Assert.That(loaded.Outcome, Is.EqualTo(SaveLoadOutcome.SetAside));
                Assert.That(loaded.Data.LanguageMix, Is.EqualTo(LanguageMix.Both));
                Assert.That(BrokenCopies(folder, "learning-settings"), Has.Length.EqualTo(1));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>Tests that <see cref="SaveStore.Load{T}"/> sets aside an empty file.</summary>
        [Test]
        public void TestLoad_EmptyFileIsSetAside()
        {
            string root = CreateTemporaryFolder();
            try
            {
                File.WriteAllText(Path.Combine(root, "settings.json"), string.Empty);
                LogAssert.Expect(LogType.Warning, new Regex("settings\\.json could not be read"));

                SaveLoadResult<TabletSettingsData> loaded =
                    SaveStore.ForTablet(root).Load(TabletSettingsData.FileName, TabletSettingsData.CreateDefaults());

                Assert.That(loaded.Outcome, Is.EqualTo(SaveLoadOutcome.SetAside));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>Tests that <see cref="SaveStore.Load{T}"/> sets aside a file of version 0, which no app writes.</summary>
        [Test]
        public void TestLoad_VersionZeroIsSetAside()
        {
            string root = CreateTemporaryFolder();
            try
            {
                File.WriteAllText(Path.Combine(root, "settings.json"), "{\"version\":0,\"musicOn\":false}");
                LogAssert.Expect(LogType.Warning, new Regex("settings\\.json could not be read"));

                SaveLoadResult<TabletSettingsData> loaded =
                    SaveStore.ForTablet(root).Load(TabletSettingsData.FileName, TabletSettingsData.CreateDefaults());

                Assert.That(loaded.Outcome, Is.EqualTo(SaveLoadOutcome.SetAside));
                Assert.That(loaded.Data.MusicOn, Is.True);
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>
        /// Tests that <see cref="SaveStore.Load{T}"/> keeps a file a newer version of the app wrote,
        /// gives the defaults, and that <see cref="SaveStore.Save{T}"/> then refuses to save over it.
        /// </summary>
        [Test]
        public void TestLoad_NewerVersionIsKeptAndNotSavedOver()
        {
            string root = CreateTemporaryFolder();
            try
            {
                string path = Path.Combine(root, "settings.json");
                const string newer = "{\"version\":2,\"musicOn\":false}";
                File.WriteAllText(path, newer);
                SaveStore store = SaveStore.ForTablet(root);
                LogAssert.Expect(LogType.Error, new Regex("a newer version of the app wrote it"));
                LogAssert.Expect(LogType.Error, new Regex("was not saved over"));

                SaveLoadResult<TabletSettingsData> loaded = store.Load(TabletSettingsData.FileName, TabletSettingsData.CreateDefaults());
                bool written = store.Save(TabletSettingsData.FileName, TabletSettingsData.CreateDefaults());

                Assert.That(loaded.Outcome, Is.EqualTo(SaveLoadOutcome.Unreadable));
                Assert.That(loaded.Data.MusicOn, Is.True);
                Assert.That(written, Is.False);
                Assert.That(File.ReadAllText(path), Is.EqualTo(newer));
                Assert.That(Directory.GetFiles(root), Has.Length.EqualTo(1));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>
        /// Tests that <see cref="SaveStore.Load{T}"/> keeps a file it can't open right now (locked),
        /// rather than setting a good save aside, and that <see cref="SaveStore.Save{T}"/> refuses to save over it.
        /// </summary>
        [Test]
        public void TestLoad_LockedFileIsKeptAndNotSavedOver()
        {
            string root = CreateTemporaryFolder();
            try
            {
                string path = Path.Combine(root, "settings.json");
                SaveStore store = SaveStore.ForTablet(root);
                store.Save(TabletSettingsData.FileName, new TabletSettingsData(false, false, 0.5f, false, Language.English));
                SaveLoadResult<TabletSettingsData> loaded;
                bool written;
                LogAssert.Expect(LogType.Error, new Regex("it could not be opened"));
                LogAssert.Expect(LogType.Error, new Regex("was not saved over"));
                using (new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    loaded = store.Load(TabletSettingsData.FileName, TabletSettingsData.CreateDefaults());
                    written = store.Save(TabletSettingsData.FileName, TabletSettingsData.CreateDefaults());
                }

                SaveLoadResult<TabletSettingsData> afterUnlock =
                    store.Load(TabletSettingsData.FileName, TabletSettingsData.CreateDefaults());

                Assert.That(loaded.Outcome, Is.EqualTo(SaveLoadOutcome.Unreadable));
                Assert.That(written, Is.False);
                Assert.That(BrokenCopies(root, "settings"), Is.Empty);
                Assert.That(afterUnlock.Outcome, Is.EqualTo(SaveLoadOutcome.Loaded));
                Assert.That(afterUnlock.Data.MusicOn, Is.False);
                Assert.That(store.Save(TabletSettingsData.FileName, afterUnlock.Data), Is.True);
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>
        /// Tests that <see cref="SaveStore.Load{T}"/> keeps every damaged copy, dated, beside the older ones
        /// (a second one in the same second numbered), and leaves an older version's <c>.broken.json</c> alone.
        /// </summary>
        [Test]
        public void TestLoad_EachBrokenFileIsKept()
        {
            string root = CreateTemporaryFolder();
            try
            {
                File.WriteAllText(Path.Combine(root, "settings.broken.json"), "older version");
                SaveStore store = SaveStore.ForTablet(root);
                LogAssert.Expect(LogType.Warning, new Regex("settings\\.json could not be read"));
                LogAssert.Expect(LogType.Warning, new Regex("settings\\.json could not be read"));

                File.WriteAllText(Path.Combine(root, "settings.json"), "first");
                store.Load(TabletSettingsData.FileName, TabletSettingsData.CreateDefaults());
                File.WriteAllText(Path.Combine(root, "settings.json"), "second");
                store.Load(TabletSettingsData.FileName, TabletSettingsData.CreateDefaults());

                string[] broken = BrokenCopies(root, "settings");
                Assert.That(broken.Select(file => File.ReadAllText(file)), Is.EquivalentTo(new[] { "first", "second" }));
                Assert.That(broken.Select(file => Path.GetFileName(file)), Has.All.Match(@"^settings\.broken-\d{8}-\d{6}(-\d+)?\.json$"));
                Assert.That(File.ReadAllText(Path.Combine(root, "settings.broken.json")), Is.EqualTo("older version"));
                Assert.That(File.Exists(Path.Combine(root, "settings.json")), Is.False);
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>Tests that <see cref="SaveStore.Load{T}"/> brings a loaded file's values into range.</summary>
        [Test]
        public void TestLoad_ClampsValuesOutOfRange()
        {
            string root = CreateTemporaryFolder();
            try
            {
                File.WriteAllText(Path.Combine(root, "settings.json"),
                    "{\"version\":1,\"musicOn\":true,\"soundsOn\":true,\"voiceVolume\":7.5,\"vibrationOn\":true,\"appLanguage\":\"xx\"}");

                SaveLoadResult<TabletSettingsData> loaded =
                    SaveStore.ForTablet(root).Load(TabletSettingsData.FileName, TabletSettingsData.CreateDefaults());

                Assert.That(loaded.Outcome, Is.EqualTo(SaveLoadOutcome.Loaded));
                Assert.That(loaded.Data.VoiceVolume, Is.EqualTo(1f));
                Assert.That(loaded.Data.AppLanguage, Is.EqualTo(Language.Romanian));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>Tests that <see cref="SaveStore.Load{T}"/> gives the defaults, logging an error, for a missing or relative folder.</summary>
        [TestCase(null)]
        [TestCase("")]
        [TestCase("relative/folder")]
        public void TestLoad_UnusableFolderGivesDefaults(string root)
        {
            SaveStore store = SaveStore.ForTablet(root);
            TabletSettingsData defaults = TabletSettingsData.CreateDefaults();
            LogAssert.Expect(LogType.Error, new Regex("can't be used"));

            SaveLoadResult<TabletSettingsData> loaded = store.Load(TabletSettingsData.FileName, defaults);

            Assert.That(loaded.Outcome, Is.EqualTo(SaveLoadOutcome.Unreadable));
            Assert.That(loaded.Data, Is.SameAs(defaults));
        }

        /// <summary>Tests that <see cref="SaveStore.Save{T}"/> returns false, logging an error, for a missing or relative folder.</summary>
        [TestCase(null)]
        [TestCase("relative/folder")]
        public void TestSave_UnusableFolderReturnsFalse(string root)
        {
            SaveStore store = SaveStore.ForProfile(root, ProfileId.Create("child-1"));
            LogAssert.Expect(LogType.Error, new Regex("can't be used"));

            bool written = store.Save(LearningSettingsData.FileName, LearningSettingsData.CreateDefaults());

            Assert.That(written, Is.False);
        }

        /// <summary>Tests that <see cref="SaveStore.Save{T}"/> returns false, logging an error, when the folder is a file.</summary>
        [Test]
        public void TestSave_FolderThatIsAFileReturnsFalse()
        {
            string root = CreateTemporaryFolder();
            try
            {
                string file = Path.Combine(root, "file");
                File.WriteAllText(file, "x");
                LogAssert.Expect(LogType.Error, new Regex("could not save settings\\.json"));

                bool written = SaveStore.ForTablet(file).Save(TabletSettingsData.FileName, TabletSettingsData.CreateDefaults());

                Assert.That(written, Is.False);
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>Tests that <see cref="SaveStore.Load{T}"/> refuses a name with a folder, an extension or capitals.</summary>
        [TestCase(null)]
        [TestCase("")]
        [TestCase("Settings")]
        [TestCase("settings.json")]
        [TestCase("../settings")]
        [TestCase("settings\n")]
        public void TestLoad_InvalidNameThrows(string name)
        {
            SaveStore store = SaveStore.ForTablet(Path.GetTempPath());

            Assert.That(() => store.Load(name, TabletSettingsData.CreateDefaults()), Throws.ArgumentException);
            Assert.That(() => store.Save(name, TabletSettingsData.CreateDefaults()), Throws.ArgumentException);
        }

        private static string[] BrokenCopies(string folder, string name)
        {
            return Directory.GetFiles(folder, name + ".broken-*.json");
        }

        private static string CreateTemporaryFolder()
        {
            string folder = Path.Combine(Path.GetTempPath(), "scolita-tests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
            return folder;
        }
    }
}
