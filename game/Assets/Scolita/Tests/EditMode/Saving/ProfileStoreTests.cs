using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Profiles;
using Scolita.Core.Timing;
using Scolita.Game.Profiles;
using Scolita.Game.Rewards;
using Scolita.Game.Saving;
using Scolita.Game.Settings;
using Scolita.Tests.EditMode.Fakes;
using UnityEngine;
using UnityEngine.TestTools;

namespace Scolita.Tests.EditMode.Saving
{
    /// <summary>
    /// Tests <see cref="ProfileStore"/>: the profile list in the tablet's folder, the migration from a tablet of 0.13.0
    /// (her <c>child-1</c> folder, no list), a damaged list, the split between the tablet's files and each profile's, and
    /// deleting a profile's folder only. Each test uses a fresh temporary folder.
    /// </summary>
    public sealed class ProfileStoreTests
    {
        /// <summary>
        /// Tests that <see cref="ProfileStore.Load"/> on a tablet of 0.13.0 (her <c>child-1</c> folder with its files, no
        /// list) makes <c>child-1</c> the first child with everything in its folder kept, and saves the list.
        /// </summary>
        [Test]
        public void TestLoad_ATabletBeforeProfilesKeepsHerProgress()
        {
            string root = CreateTemporaryFolder();
            try
            {
                WriteOldTablet(root);
                ProfileStore store = Store(root);

                ProfileList list = store.Load();

                Profile first = list.ProfileToOpen;
                Assert.That(list.All.Select(profile => profile.Id.Value), Is.EqualTo(new[] { "child-1" }));
                Assert.That(first.Avatar.Value, Is.EqualTo("kitty"));
                Assert.That(first.Name.IsNone, Is.True);
                Assert.That(File.Exists(Path.Combine(root, "profiles.json")), Is.True);
                SaveStore files = store.FilesOf(first);
                Assert.That(files.Folder, Is.EqualTo(Path.Combine(root, "profiles", "child-1")));
                Assert.That(files.Load(RewardsData.FileName, RewardsData.CreateDefaults()).Data.ToBook().StarsEarned, Is.EqualTo(54));
                Assert.That(files.Load(LearningSettingsData.FileName, LearningSettingsData.CreateDefaults()).Data.LanguageMix,
                    Is.EqualTo(LanguageMix.EnglishOnly));
                Assert.That(File.ReadAllText(Path.Combine(root, "profiles", "child-1", "progress.json")), Does.Contain("\"ro-a\""));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>Tests that a list saved by <see cref="ProfileStore.Save"/> is loaded back as it was, in the tablet's folder.</summary>
        [Test]
        public void TestSave_RoundTrip()
        {
            string root = CreateTemporaryFolder();
            try
            {
                ProfileStore store = Store(root);
                ProfileList list = store.Load();
                ProfileName.Read("Ștefan", TestProfileRules.Default(), out ProfileName name);
                Profile stefan = list.AddChild(name, ContentId.Create("dog"), CalendarDay.FromDate(2026, 11, 25));
                list.AddTester(ContentId.Create("rocket"), CalendarDay.FromDate(2026, 11, 25));
                list.Play(stefan.Id);

                bool saved = store.Save(list);
                ProfileList loaded = Store(root).Load();

                Assert.That(saved, Is.True);
                Assert.That(loaded.All.Select(profile => profile.Id.Value), Is.EqualTo(new[] { "child-1", "child-2", "tester" }));
                Assert.That(loaded.Children[1].Name.Text, Is.EqualTo("Ștefan"));
                Assert.That(loaded.Children[1].Avatar.Value, Is.EqualTo("dog"));
                Assert.That(loaded.Children[1].Created, Is.EqualTo(CalendarDay.FromDate(2026, 11, 25)));
                Assert.That(loaded.Tester.Kind, Is.EqualTo(ProfileKind.Tester));
                Assert.That(loaded.Playing, Is.EqualTo(stefan.Id));
                Assert.That(loaded.LastChild, Is.EqualTo(stefan.Id));
                Assert.That(loaded.NextChildNumber, Is.EqualTo(3));
                Assert.That(Directory.GetFiles(root).Select(Path.GetFileName), Is.EquivalentTo(new[] { "profiles.json" }));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>Tests that a damaged list is set aside, dated, and the list is made again from the profile folders, losing no profile.</summary>
        [Test]
        public void TestLoad_ADamagedListIsMadeFromTheFolders()
        {
            string root = CreateTemporaryFolder();
            try
            {
                Directory.CreateDirectory(Path.Combine(root, "profiles", "child-1"));
                Directory.CreateDirectory(Path.Combine(root, "profiles", "child-2"));
                Directory.CreateDirectory(Path.Combine(root, "profiles", "tester"));
                File.WriteAllText(Path.Combine(root, "profiles.json"), "{ not json");

                ProfileList list = Store(root).Load();

                Assert.That(list.All.Select(profile => profile.Id.Value), Is.EqualTo(new[] { "child-1", "child-2", "tester" }));
                Assert.That(list.StartsWithPicker, Is.True);
                Assert.That(Directory.GetFiles(root, "profiles.broken-*.json").Length, Is.EqualTo(1));
                Assert.That(File.ReadAllText(Path.Combine(root, "profiles.json")), Does.Contain("child-2"));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>
        /// Tests that a list with an entry that can't be read is kept as a dated copy before it is saved over, and that a
        /// new child's id is above every folder on the tablet (never a folder left behind).
        /// </summary>
        [Test]
        public void TestLoad_AnUnreadableEntryIsKeptAndNoFolderIsReused()
        {
            string root = CreateTemporaryFolder();
            try
            {
                Directory.CreateDirectory(Path.Combine(root, "profiles", "child-1"));
                Directory.CreateDirectory(Path.Combine(root, "profiles", "child-7"));
                File.WriteAllText(Path.Combine(root, "profiles.json"), "{\"version\":1,\"profiles\":["
                    + "{\"id\":\"child-1\",\"name\":\"Ana\",\"avatar\":\"kitty\",\"kind\":\"child\",\"created\":\"2026-11-20\"},"
                    + "{\"id\":\"Child 7\",\"name\":\"Matei\",\"avatar\":\"cat\",\"kind\":\"child\",\"created\":\"2026-11-20\"}"
                    + "],\"lastChild\":\"child-1\",\"playing\":\"\",\"nextChild\":2}");

                ProfileList list = Store(root).Load();
                Profile next = list.AddChild(ProfileName.None, ContentId.Create("cat"), CalendarDay.FromDate(2026, 11, 26));

                Assert.That(list.All[0].Name.Text, Is.EqualTo("Ana"));
                Assert.That(next.Id.Value, Is.EqualTo("child-8"));
                string[] copies = Directory.GetFiles(root, "profiles.broken-*.json");
                Assert.That(copies.Length, Is.EqualTo(1));
                Assert.That(File.ReadAllText(copies[0]), Does.Contain("Matei"));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>Tests that <see cref="ProfileStore.DeleteFolder"/> deletes only that profile's folder, and only once it is off the list.</summary>
        [Test]
        public void TestDeleteFolder_OnlyThatProfilesFolder()
        {
            string root = CreateTemporaryFolder();
            try
            {
                ProfileStore store = Store(root);
                ProfileList list = store.Load();
                Profile ana = list.AddChild(ProfileName.None, ContentId.Create("cat"), CalendarDay.FromDate(2026, 11, 25));
                store.Save(list);
                Assert.That(store.FilesOf(ana).Save(LearningSettingsData.FileName, LearningSettingsData.CreateDefaults()), Is.True);
                Assert.That(store.FilesOf(list.Children[0]).Save(LearningSettingsData.FileName,
                    LearningSettingsData.CreateDefaults()), Is.True);
                File.WriteAllText(Path.Combine(root, "settings.json"), "{}");

                LogAssert.Expect(LogType.Error, new Regex("still listed"));
                bool refused = store.DeleteFolder(list, ana.Id);
                list.Delete(ana.Id);
                store.Save(list);
                bool deleted = store.DeleteFolder(list, ana.Id);

                Assert.That(refused, Is.False);
                Assert.That(deleted, Is.True);
                Assert.That(Directory.Exists(Path.Combine(root, "profiles", "child-2")), Is.False);
                Assert.That(File.Exists(Path.Combine(root, "profiles", "child-1", "learning-settings.json")), Is.True);
                Assert.That(File.Exists(Path.Combine(root, "settings.json")), Is.True);
                Assert.That(File.Exists(Path.Combine(root, "profiles.json")), Is.True);
                Assert.That(store.DeleteFolder(list, ana.Id), Is.True);
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        private static ProfileStore Store(string root)
        {
            ContentId[] avatars = new[] { "kitty", "cat", "unicorn", "rocket", "dog" }.Select(ContentId.Create).ToArray();
            return new ProfileStore(root, SaveStore.ForTablet(root, new FakeClock(CalendarDay.FromDate(2026, 11, 26)), "0.14.0"), avatars,
                TestProfileRules.Default(), new FakeClock(CalendarDay.FromDate(2026, 11, 26)));
        }

        private static void WriteOldTablet(string root)
        {
            string folder = Path.Combine(root, "profiles", "child-1");
            Directory.CreateDirectory(folder);
            File.WriteAllText(Path.Combine(root, "settings.json"),
                "{\n    \"version\": 1,\n    \"musicOn\": true,\n    \"soundsOn\": true,\n    \"voiceVolume\": 1.0,\n"
                + "    \"vibrationOn\": true,\n    \"appLanguage\": \"ro\"\n}");
            File.WriteAllText(Path.Combine(folder, "learning-settings.json"),
                "{\n    \"version\": 2,\n    \"languageMix\": \"en\",\n    \"sessionLimitMinutes\": 0\n}");
            File.WriteAllText(Path.Combine(folder, "play-time.json"),
                "{\n    \"version\": 1,\n    \"day\": \"2026-11-25\",\n    \"seconds\": 48.7774794\n}");
            File.WriteAllText(Path.Combine(folder, "progress.json"),
                "{\n    \"version\": 1,\n    \"doneLessons\": [\"ro-a\"],\n    \"rocketStop\": \"ro-a\",\n    \"daysPlayed\": [],\n"
                + "    \"items\": []\n}");
            File.WriteAllText(Path.Combine(folder, "rewards.json"),
                "{\n    \"version\": 1,\n    \"starsEarned\": 54,\n    \"litLessons\": [\"ro-a\", \"en-a\"],\n"
                + "    \"lessonsFinished\": 18,\n    \"stickers\": [\"rocket-heart\", \"bicycle\"],\n    \"rewardedUnits\": [],\n"
                + "    \"trails\": [],\n    \"trail\": \"\",\n    \"placed\": []\n}");
        }

        private static string CreateTemporaryFolder()
        {
            string folder = Path.Combine(Path.GetTempPath(), "scolita-profiles-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
            return folder;
        }
    }
}
