using System;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Scolita.Core.Languages;
using Scolita.Core.Parent;
using Scolita.Core.Profiles;
using Scolita.Core.Timing;
using Scolita.Game.Lessons;
using Scolita.Game.Parent;
using Scolita.Game.Saving;
using Scolita.Game.Settings;
using UnityEngine;
using UnityEngine.TestTools;

namespace Scolita.Tests.EditMode.Saving
{
    /// <summary>Tests <see cref="ProgressReset"/> and <see cref="SaveStore.Delete"/> in a fresh temporary folder per test.</summary>
    public sealed class ProgressResetTests
    {
        /// <summary>Tests that <see cref="ProgressReset.Run"/> deletes her progress, the map's older file, the lesson in progress and her rewards together.</summary>
        [Test]
        public void TestRun_DeletesTheProgressFilesTogether()
        {
            string root = CreateTemporaryFolder();
            try
            {
                SaveStore store = SaveStore.ForProfile(root, ProfileId.Create("child-1"));
                Directory.CreateDirectory(store.Folder);
                foreach (string name in ProgressReset.FileNames)
                {
                    File.WriteAllText(Path.Combine(store.Folder, name + ".json"), "{\"version\":1}");
                }

                bool gone = ProgressReset.Run(store);

                Assert.That(gone, Is.True);
                Assert.That(ProgressReset.FileNames, Is.EqualTo(new[] { "progress", "map-progress", "lesson-run", "rewards" }));
                foreach (string name in ProgressReset.FileNames)
                {
                    Assert.That(File.Exists(Path.Combine(store.Folder, name + ".json")), Is.False, name);
                }
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>Tests that <see cref="ProgressReset.Run"/> keeps the learning settings, the play time and the damaged copies.</summary>
        [Test]
        public void TestRun_KeepsTheSettingsPlayTimeAndCopies()
        {
            string root = CreateTemporaryFolder();
            try
            {
                SaveStore store = SaveStore.ForProfile(root, ProfileId.Create("child-1"));
                store.Save(LearningSettingsData.FileName, new LearningSettingsData(LanguageMix.EnglishOnly));
                store.Save(PlayTimeData.FileName,
                    new PlayTimeData(new DailyPlay(CalendarDay.FromDate(2026, 11, 25), TimeSpan.FromMinutes(5))));
                string copy = Path.Combine(store.Folder, "progress.broken-20261125-101500.json");
                File.WriteAllText(copy, "not json");
                store.Save(LessonRunData.FileName, LessonRunData.CreateDefaults());

                ProgressReset.Run(store);

                Assert.That(File.Exists(Path.Combine(store.Folder, "learning-settings.json")), Is.True);
                Assert.That(File.Exists(Path.Combine(store.Folder, "play-time.json")), Is.True);
                Assert.That(File.Exists(copy), Is.True);
                Assert.That(store.Load(LearningSettingsData.FileName, LearningSettingsData.CreateDefaults()).Data.LanguageMix,
                    Is.EqualTo(LanguageMix.EnglishOnly));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>Tests that <see cref="SaveStore.Delete"/> is true for a file that was never there.</summary>
        [Test]
        public void TestDelete_MissingFileIsGone()
        {
            string root = CreateTemporaryFolder();
            try
            {
                SaveStore store = SaveStore.ForProfile(root, ProfileId.Create("child-1"));

                Assert.That(store.Delete(LessonRunData.FileName), Is.True);
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>Tests that <see cref="SaveStore.Delete"/> removes a file kept as it is (a newer version's), and the store saves the name again after.</summary>
        [Test]
        public void TestDelete_KeptFileCanBeSavedAgain()
        {
            string root = CreateTemporaryFolder();
            try
            {
                SaveStore store = SaveStore.ForProfile(root, ProfileId.Create("child-1"));
                Directory.CreateDirectory(store.Folder);
                string path = Path.Combine(store.Folder, LessonRunData.FileName + ".json");
                File.WriteAllText(path, "{\"version\":99}");
                LogAssert.Expect(LogType.Error, new Regex("lesson-run.json could not be read"));
                SaveLoadResult<LessonRunData> loaded = store.Load(LessonRunData.FileName, LessonRunData.CreateDefaults());
                Assert.That(loaded.Outcome, Is.EqualTo(SaveLoadOutcome.Unreadable));

                bool deleted = store.Delete(LessonRunData.FileName);
                bool saved = store.Save(LessonRunData.FileName, LessonRunData.CreateDefaults());

                Assert.That(deleted, Is.True);
                Assert.That(saved, Is.True);
                Assert.That(File.ReadAllText(path), Does.Contain("\"version\": 1"));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>Tests that <see cref="SaveStore.Delete"/> refuses a name that isn't a save file's.</summary>
        [Test]
        public void TestDelete_RefusesAPath()
        {
            string root = CreateTemporaryFolder();
            try
            {
                SaveStore store = SaveStore.ForProfile(root, ProfileId.Create("child-1"));

                Assert.Throws<ArgumentException>(() => store.Delete("../settings"));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        private static string CreateTemporaryFolder()
        {
            string root = Path.Combine(Path.GetTempPath(), "scolita-reset-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(root);
            return root;
        }
    }
}
