using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Scolita.Core.Activities;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;
using Scolita.Core.Map;
using Scolita.Core.Profiles;
using Scolita.Core.Progress;
using Scolita.Core.Timing;
using Scolita.Game.Lessons;
using Scolita.Game.Map;
using Scolita.Game.Progress;
using Scolita.Game.Saving;
using Scolita.Tests.EditMode.Fakes;
using UnityEngine;
using UnityEngine.TestTools;

namespace Scolita.Tests.EditMode.Saving
{
    /// <summary>
    /// Tests <see cref="ProgressFile"/>: her <c>progress</c> save, made from every earlier shape (0.10.0's
    /// <c>map-progress</c>, 0.9.0's lesson in progress, nothing), set aside when damaged, never saved over when newer.
    /// </summary>
    public sealed class ProgressFileTests
    {
        /// <summary>
        /// Tests that <see cref="ProgressFile.Load"/> makes an update from 0.10.0 keep her place: the map progress's lessons
        /// and rocket stop, the letters they introduced met; <c>progress.json</c> saved and <c>map-progress.json</c> left as
        /// it is.
        /// </summary>
        [Test]
        public void TestLoad_FromMapProgress()
        {
            string root = CreateTemporaryFolder();
            try
            {
                SaveStore store = Store(root);
                store.Save(MapProgressData.FileName, new MapProgressData(MapRoutes.Progress("ro-m", "ro-a", "en-a")));
                store.Save(LessonRunData.FileName, new LessonRunData(ContentId.Create("ro-m"), 2, "0123456789abcdef"));
                LogAssert.Expect(LogType.Log, new Regex("progress.json is made from the earlier saves"));

                LearnerProgress progress = ProgressFile.Load(Store(root), RealContent.Read(), Order(LanguageMix.Both));
                LearnerProgress again = ProgressFile.Load(Store(root), RealContent.Read(), Order(LanguageMix.Both));

                Assert.That(progress.Lessons.DoneLessons.Select(id => id.ToString()), Is.EqualTo(new[] { "ro-a", "en-a" }));
                Assert.That(progress.Lessons.RocketStop.ToString(), Is.EqualTo("ro-m"));
                Assert.That(progress.Items.Select(item => item.Item.ToString()), Is.EqualTo(new[] { "letter a (ro)", "letter a (en)" }));
                Assert.That(again.Lessons.RocketStop.ToString(), Is.EqualTo("ro-m"));
                Assert.That(File.Exists(Path.Combine(Folder(root), "progress.json")), Is.True);
                Assert.That(File.Exists(Path.Combine(Folder(root), "map-progress.json")), Is.True);
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>
        /// Tests that <see cref="ProgressFile.Load"/> makes an update from 0.9.0 or earlier (no map progress) keep her place:
        /// the lessons before the one in <c>lesson-run.json</c> are done; with nothing saved, nothing is.
        /// </summary>
        [Test]
        public void TestLoad_FromBeforeTheMap()
        {
            string root = CreateTemporaryFolder();
            string fresh = CreateTemporaryFolder();
            try
            {
                Store(root).Save(LessonRunData.FileName, new LessonRunData(ContentId.Create("ro-t"), 1, string.Empty));
                LogAssert.Expect(LogType.Log, new Regex("progress.json is made from the earlier saves"));
                LogAssert.Expect(LogType.Log, new Regex("progress.json is made from the earlier saves"));

                LearnerProgress progress = ProgressFile.Load(Store(root), RealContent.Read(), Order(LanguageMix.Both));
                LearnerProgress nothing = ProgressFile.Load(Store(fresh), RealContent.Read(), Order(LanguageMix.Both));

                Assert.That(progress.Lessons.DoneLessons.Select(id => id.ToString()), Is.EqualTo(new[] { "ro-a", "en-a", "ro-m", "en-m" }));
                Assert.That(progress.Lessons.RocketStop.IsSet, Is.False);
                Assert.That(nothing.Lessons.DoneLessons, Is.Empty);
                Assert.That(nothing.Items, Is.Empty);
            }
            finally
            {
                Directory.Delete(root, true);
                Directory.Delete(fresh, true);
            }
        }

        /// <summary>
        /// Tests that <see cref="ProgressFile.Load"/> sets a damaged <c>progress.json</c> aside, dated, with a warning, and
        /// keeps her place from the earlier saves instead of starting from the first lesson.
        /// </summary>
        [Test]
        public void TestLoad_DamagedFileKeepsHerPlace()
        {
            string root = CreateTemporaryFolder();
            try
            {
                SaveStore store = Store(root);
                store.Save(MapProgressData.FileName, new MapProgressData(MapRoutes.Progress("ro-m", "ro-a", "en-a")));
                File.WriteAllText(Path.Combine(Folder(root), "progress.json"), "{ not json");
                LogAssert.Expect(LogType.Warning, new Regex(@"progress\.json could not be read"));
                LogAssert.Expect(LogType.Warning, new Regex(@"progress\.json was damaged and set aside"));

                LearnerProgress progress = ProgressFile.Load(Store(root), RealContent.Read(), Order(LanguageMix.Both));

                Assert.That(progress.Lessons.DoneLessons.Select(id => id.ToString()), Is.EqualTo(new[] { "ro-a", "en-a" }));
                Assert.That(Directory.GetFiles(Folder(root), "progress.broken-*.json"), Has.Length.EqualTo(1));
                Assert.That(File.ReadAllText(Path.Combine(Folder(root), "progress.json")), Does.Contain("\"ro-a\""));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>Tests that a <c>progress.json</c> a newer version wrote is never saved over.</summary>
        [Test]
        public void TestSave_NeverOverANewerFile()
        {
            string root = CreateTemporaryFolder();
            try
            {
                Directory.CreateDirectory(Folder(root));
                string path = Path.Combine(Folder(root), "progress.json");
                string newer = "{\"version\":99,\"doneLessons\":[\"ro-a\"]}";
                File.WriteAllText(path, newer);
                SaveStore store = Store(root);
                LogAssert.Expect(LogType.Error, new Regex(@"progress\.json could not be read"));
                LogAssert.Expect(LogType.Error, new Regex("was not saved over"));
                LogAssert.Expect(LogType.Warning, new Regex("her progress could not be saved"));

                LearnerProgress progress = ProgressFile.Load(store, RealContent.Read(), Order(LanguageMix.Both));
                bool saved = ProgressFile.Save(store, progress);

                Assert.That(saved, Is.False);
                Assert.That(File.ReadAllText(path), Is.EqualTo(newer));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>
        /// Tests that switching the mix to Romanian only hides the English lessons on the map and keeps her English
        /// progress (the brief's acceptance): saved and read again in Romanian only, then shown again in both.
        /// </summary>
        [Test]
        public void TestLoad_RomanianOnlyKeepsTheEnglishProgress()
        {
            string root = CreateTemporaryFolder();
            try
            {
                Curriculum curriculum = RealContent.Read();
                CalendarDay day = CalendarDay.FromDate(2026, 11, 25);
                LearnerProgress progress = Progresses.Learned(day, Language.English, "a");
                progress.Lessons.MarkDone(ContentId.Create("ro-a"));
                progress.Lessons.MarkDone(ContentId.Create("en-a"));
                Assert.That(ProgressFile.Save(Store(root), progress), Is.True);

                LearnerProgress romanianOnly = ProgressFile.Load(Store(root), curriculum, Order(LanguageMix.RomanianOnly));
                MapRoute hidden = MapRoutes.Of(curriculum, LanguageMix.RomanianOnly, romanianOnly);
                ProgressFile.Save(Store(root), romanianOnly);
                LearnerProgress both = ProgressFile.Load(Store(root), curriculum, Order(LanguageMix.Both));
                MapRoute shown = MapRoutes.Of(curriculum, LanguageMix.Both, both);

                Assert.That(hidden.Stops.Select(stop => stop.Lesson.Languages), Has.None.EqualTo(LanguageMix.EnglishOnly));
                Assert.That(hidden.GoStop.Lesson.Id.ToString(), Is.EqualTo("ro-m"));
                Assert.That(both.StateOf(LearningItem.Letter(ContentId.Create("a"), Language.English)), Is.EqualTo(ItemState.Learned));
                Assert.That(shown.TryFindStop(ContentId.Create("en-a"), out MapStop english), Is.True);
                Assert.That(english.State, Is.EqualTo(StopState.Done));
                Assert.That(shown.GoStop.Lesson.Id.ToString(), Is.EqualTo("ro-m"));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        private static SaveStore Store(string root)
        {
            return SaveStore.ForProfile(root, ProfileId.Create("child-1"));
        }

        private static string Folder(string root)
        {
            return Path.Combine(root, "profiles", "child-1");
        }

        private static LessonOrder Order(LanguageMix mix)
        {
            Curriculum curriculum = RealContent.Read();
            return new LessonOrder(curriculum.Domains[0], mix, new PlayableSteps(ActivityRulesFactory.Kinds, curriculum));
        }

        private static string CreateTemporaryFolder()
        {
            string folder = Path.Combine(Path.GetTempPath(), "scolita-progress-tests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
            return folder;
        }
    }
}
