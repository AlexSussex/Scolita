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
using Scolita.Game.Lessons;
using Scolita.Game.Map;
using Scolita.Game.Saving;
using Scolita.Tests.EditMode.Fakes;
using UnityEngine;
using UnityEngine.TestTools;

namespace Scolita.Tests.EditMode.Saving
{
    /// <summary>Tests <see cref="MapProgressData"/> and <see cref="MapProgressFile"/>: the save's shape, its upgrade, and reading it safely.</summary>
    public sealed class MapProgressDataTests
    {
        /// <summary>Tests that <see cref="MapProgressData.CreateDefaults"/> holds nothing done and no rocket stop.</summary>
        [Test]
        public void TestCreateDefaults_NothingDone()
        {
            MapProgress progress = MapProgressData.CreateDefaults().ToProgress();

            Assert.That(MapProgressData.CreateDefaults().Version, Is.EqualTo(1));
            Assert.That(progress.DoneLessons, Is.Empty);
            Assert.That(progress.RocketStop.IsSet, Is.False);
        }

        /// <summary>Tests that <see cref="MapProgressData"/> survives JSON with its lessons in order and its rocket stop.</summary>
        [Test]
        public void TestToProgress_RoundTrips()
        {
            MapProgress saved = new MapProgress(new[] { ContentId.Create("ro-a"), ContentId.Create("en-a") }, ContentId.Create("ro-m"));

            string json = JsonUtility.ToJson(new MapProgressData(saved));
            MapProgress read = JsonUtility.FromJson<MapProgressData>(json).Upgraded().ToProgress();

            Assert.That(json, Does.Contain("\"doneLessons\"").And.Contain("\"rocketStop\""));
            Assert.That(read.DoneLessons.Select(id => id.ToString()), Is.EqualTo(new[] { "ro-a", "en-a" }));
            Assert.That(read.RocketStop.ToString(), Is.EqualTo("ro-m"));
        }

        /// <summary>
        /// Tests that <see cref="MapProgressData.Upgraded"/> keeps an id the content doesn't know, drops a text that
        /// can't be an id, merges duplicates, and reads a file with no fields as nothing done.
        /// </summary>
        [Test]
        public void TestUpgraded_KeepsUnknownDropsMalformed()
        {
            MapProgressData odd = JsonUtility.FromJson<MapProgressData>(
                "{\"version\":1,\"doneLessons\":[\"ro-a\",\"Not An Id\",\"planet-99-lesson\",\"ro-a\",\"\"],\"rocketStop\":\"??\"}").Upgraded();
            MapProgressData empty = JsonUtility.FromJson<MapProgressData>("{\"version\":1}").Upgraded();

            Assert.That(odd.DoneLessons, Is.EqualTo(new[] { "ro-a", "planet-99-lesson" }));
            Assert.That(odd.ToProgress().RocketStop.IsSet, Is.False);
            Assert.That(empty.DoneLessons, Is.Empty);
            Assert.That(empty.ToProgress().RocketStop.IsSet, Is.False);
        }

        /// <summary>Tests that <see cref="MapProgressFile.RecordDone"/> adds a lesson to the profile's file, once, keeping the rocket stop.</summary>
        [Test]
        public void TestRecordDone_AddsToTheFile()
        {
            string root = CreateTemporaryFolder();
            try
            {
                SaveStore store = SaveStore.ForProfile(root, ProfileId.Create("child-1"));
                MapProgressFile.Save(store, new MapProgress(new[] { ContentId.Create("ro-a") }, ContentId.Create("en-a")));

                LessonOrder order = Order();
                bool first = MapProgressFile.RecordDone(store, order, ContentId.Create("en-a"));
                bool again = MapProgressFile.RecordDone(store, order, ContentId.Create("en-a"));
                MapProgress read = MapProgressFile.Load(store, order);

                Assert.That(first && again, Is.True);
                Assert.That(read.DoneLessons.Select(id => id.ToString()), Is.EqualTo(new[] { "ro-a", "en-a" }));
                Assert.That(read.RocketStop.ToString(), Is.EqualTo("en-a"));
                Assert.That(File.Exists(Path.Combine(root, "profiles", "child-1", "map-progress.json")), Is.True);
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>
        /// Tests that <see cref="MapProgressFile.Load"/> sets a damaged file aside, dated, and keeps where she is from
        /// <c>lesson-run.json</c> (the lessons before the one saved there are done).
        /// </summary>
        [Test]
        public void TestLoad_DamagedFileKeepsWhereSheWas()
        {
            string root = CreateTemporaryFolder();
            try
            {
                string folder = Path.Combine(root, "profiles", "child-1");
                Directory.CreateDirectory(folder);
                File.WriteAllText(Path.Combine(folder, "map-progress.json"), "{ not json");
                LogAssert.Expect(LogType.Warning, new Regex("map-progress\\.json could not be read"));

                SaveStore store = SaveStore.ForProfile(root, ProfileId.Create("child-1"));
                store.Save(LessonRunData.FileName, new LessonRunData(ContentId.Create("ro-m"), 1, string.Empty));

                MapProgress read = MapProgressFile.Load(store, Order());

                Assert.That(read.DoneLessons.Select(id => id.ToString()), Is.EqualTo(new[] { "ro-a", "en-a" }));
                Assert.That(Directory.GetFiles(folder, "map-progress.broken-*.json"), Has.Length.EqualTo(1));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>
        /// Tests that a file a newer version wrote is read as nothing done and never saved over:
        /// <see cref="MapProgressFile.RecordDone"/> refuses and the file stays as it was.
        /// </summary>
        [Test]
        public void TestRecordDone_NeverOverANewerFile()
        {
            string root = CreateTemporaryFolder();
            try
            {
                string folder = Path.Combine(root, "profiles", "child-1");
                Directory.CreateDirectory(folder);
                string path = Path.Combine(folder, "map-progress.json");
                string newer = "{\"version\":99,\"doneLessons\":[\"ro-a\"],\"rocketStop\":\"en-a\"}";
                File.WriteAllText(path, newer);
                SaveStore store = SaveStore.ForProfile(root, ProfileId.Create("child-1"));
                LessonOrder order = Order();
                LogAssert.Expect(LogType.Error, new Regex(@"map-progress\.json could not be read"));
                LogAssert.Expect(LogType.Error, new Regex(@"map-progress\.json could not be read"));
                LogAssert.Expect(LogType.Error, new Regex("was not saved over"));
                LogAssert.Expect(LogType.Warning, new Regex("could not be saved"));

                MapProgress read = MapProgressFile.Load(store, order);
                bool recorded = MapProgressFile.RecordDone(store, order, ContentId.Create("en-a"));

                Assert.That(read.DoneLessons, Is.Empty);
                Assert.That(recorded, Is.False);
                Assert.That(File.ReadAllText(path), Is.EqualTo(newer));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>
        /// Tests that <see cref="MapProgressFile.Load"/> with no file yet (a profile from before the map) takes the
        /// lessons before the one saved in <c>lesson-run.json</c> as done, and nothing without one.
        /// </summary>
        [Test]
        public void TestLoad_NoFileYetKeepsWhereSheWas()
        {
            string root = CreateTemporaryFolder();
            try
            {
                SaveStore store = SaveStore.ForProfile(root, ProfileId.Create("child-1"));
                LessonOrder order = Order();
                MapProgress fresh = MapProgressFile.Load(store, order);
                store.Save(LessonRunData.FileName, new LessonRunData(ContentId.Create("ro-m"), 3, "0123456789abcdef"));

                MapProgress updated = MapProgressFile.Load(store, order);

                Assert.That(fresh.DoneLessons, Is.Empty);
                Assert.That(updated.DoneLessons.Select(id => id.ToString()), Is.EqualTo(new[] { "ro-a", "en-a" }));
                Assert.That(updated.RocketStop.IsSet, Is.False);
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        private static LessonOrder Order()
        {
            Curriculum curriculum = RealContent.Read();
            return new LessonOrder(curriculum.Domains[0], LanguageMix.Both, new PlayableSteps(ActivityRulesFactory.Kinds, curriculum));
        }

        private static string CreateTemporaryFolder()
        {
            string folder = Path.Combine(Path.GetTempPath(), "scolita-map-tests-" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
            return folder;
        }
    }
}
