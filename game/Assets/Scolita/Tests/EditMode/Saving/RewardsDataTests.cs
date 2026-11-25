using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Lessons;
using Scolita.Core.Profiles;
using Scolita.Core.Rewards;
using Scolita.Game.Rewards;
using Scolita.Game.Saving;
using Scolita.Tests.EditMode.Fakes;
using UnityEngine;
using UnityEngine.TestTools;

namespace Scolita.Tests.EditMode.Saving
{
    /// <summary>Tests <see cref="RewardsData"/> and <see cref="RewardsFile"/>: the save's shape, reading it safely, and a profile from before rewards.</summary>
    public sealed class RewardsDataTests
    {
        /// <summary>Tests that <see cref="RewardsData.CreateDefaults"/> is version 1 with nothing earned.</summary>
        [Test]
        public void TestCreateDefaults_NothingEarned()
        {
            RewardsData defaults = RewardsData.CreateDefaults();
            RewardBook book = defaults.ToBook();

            Assert.That(defaults.Version, Is.EqualTo(1));
            Assert.That(book.StarsEarned, Is.EqualTo(0));
            Assert.That(book.LitLessons, Is.Empty);
            Assert.That(book.GivenStickers, Is.Empty);
            Assert.That(book.ChosenTrail.IsSet, Is.False);
        }

        /// <summary>Tests that <see cref="RewardsData"/> survives JSON with everything in its order, the stickers on the scene too.</summary>
        [Test]
        public void TestToBook_RoundTrips()
        {
            ContentId guitar = ContentId.Create("guitar");
            RewardBook book = new RewardBook(9, new[] { ContentId.Create("ro-a"), ContentId.Create("en-a") }, 3,
                new[] { guitar, ContentId.Create("whale-rainbow") }, new[] { ContentId.Create("planet-1") },
                new[] { ContentId.Create("rainbow") }, ContentId.Create("rainbow"), new[] { new PlacedSticker(guitar, 0.25f, 0.75f, 1.5f) });

            string json = JsonUtility.ToJson(new RewardsData(book));
            RewardBook read = JsonUtility.FromJson<RewardsData>(json).Upgraded().ToBook();

            Assert.That(json, Does.Contain("\"starsEarned\"").And.Contain("\"litLessons\"").And.Contain("\"placed\""));
            Assert.That(read.StarsEarned, Is.EqualTo(9));
            Assert.That(read.LessonsFinished, Is.EqualTo(3));
            Assert.That(read.LitLessons.Select(id => id.ToString()), Is.EqualTo(new[] { "ro-a", "en-a" }));
            Assert.That(read.GivenStickers.Select(id => id.ToString()), Is.EqualTo(new[] { "guitar", "whale-rainbow" }));
            Assert.That(read.RewardedUnits.Select(id => id.ToString()), Is.EqualTo(new[] { "planet-1" }));
            Assert.That(read.ChosenTrail.ToString(), Is.EqualTo("rainbow"));
            Assert.That(read.Placed.Count, Is.EqualTo(1));
            Assert.That(read.Placed[0].StickerId, Is.EqualTo(guitar));
            Assert.That(read.Placed[0].X, Is.EqualTo(0.25f));
            Assert.That(read.Placed[0].Y, Is.EqualTo(0.75f));
            Assert.That(read.Placed[0].Size, Is.EqualTo(1.5f));
        }

        /// <summary>
        /// Tests that <see cref="RewardsData.Upgraded"/> keeps ids the content doesn't know, drops texts that can't be
        /// ids, sets negative counts to 0, keeps a placement inside the scene, and reads a file with no fields as nothing.
        /// </summary>
        [Test]
        public void TestUpgraded_KeepsUnknownDropsMalformed()
        {
            RewardsData odd = JsonUtility.FromJson<RewardsData>("{\"version\":1,\"starsEarned\":-5,\"litLessons\":[\"ro-a\",\"Not An Id\","
                + "\"planet-99-lesson\"],\"lessonsFinished\":-1,\"stickers\":[\"future-sticker\",\"??\"],\"trails\":[\"rainbow\"],"
                + "\"trail\":\"stars\",\"placed\":[{\"sticker\":\"future-sticker\",\"x\":3,\"y\":-1,\"size\":0}]}").Upgraded();
            RewardsData empty = JsonUtility.FromJson<RewardsData>("{\"version\":1}").Upgraded();

            RewardBook book = odd.ToBook();
            Assert.That(book.StarsEarned, Is.EqualTo(0));
            Assert.That(book.LessonsFinished, Is.EqualTo(0));
            Assert.That(book.LitLessons.Select(id => id.ToString()), Is.EqualTo(new[] { "ro-a", "planet-99-lesson" }));
            Assert.That(book.GivenStickers.Select(id => id.ToString()), Is.EqualTo(new[] { "future-sticker" }));
            Assert.That(book.ChosenTrail.IsSet, Is.False);
            Assert.That(book.Placed[0].X, Is.EqualTo(1f));
            Assert.That(book.Placed[0].Y, Is.EqualTo(0f));
            Assert.That(book.Placed[0].Size, Is.EqualTo(1f));
            Assert.That(empty.ToBook().LitLessons, Is.Empty);
            Assert.That(empty.ToBook().Placed, Is.Empty);
        }

        /// <summary>
        /// Tests that <see cref="RewardsFile.Load"/> gives a profile from before rewards the stars of the lessons she
        /// finished and the trail of her finished planet, and saves them at once.
        /// </summary>
        [Test]
        public void TestLoad_AProfileFromBeforeRewardsCatchesUp()
        {
            string root = CreateTemporaryFolder();
            try
            {
                SaveStore store = SaveStore.ForProfile(root, ProfileId.Create("child-1"));
                LessonOrder order = RewardTestContent.Order(RealContent.Read());
                string[] done = RewardTestContent.LessonsOf(order, "planet-1").Select(lesson => lesson.Id.ToString()).ToArray();
                RewardGiver giver = new RewardGiver(RewardTestContent.Of(5), RewardTestContent.Rules(), new FixedRandom());

                RewardBook book = RewardsFile.Load(store, giver, MapRoutes.Progress(null, done), order);
                RewardBook again = RewardsFile.Load(store, giver, MapRoutes.Progress(null), order);

                Assert.That(book.LitLessons.Count, Is.EqualTo(done.Length));
                Assert.That(book.Trails.Select(trail => trail.ToString()), Is.EqualTo(new[] { "rainbow" }));
                Assert.That(again.LitLessons.Count, Is.EqualTo(done.Length), "read back from the file, not caught up again");
                Assert.That(File.Exists(Path.Combine(root, "profiles", "child-1", "rewards.json")), Is.True);
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>Tests that <see cref="RewardsFile.Load"/> sets a damaged file aside, dated, and catches up from the map.</summary>
        [Test]
        public void TestLoad_ADamagedFileIsSetAside()
        {
            string root = CreateTemporaryFolder();
            try
            {
                string folder = Path.Combine(root, "profiles", "child-1");
                Directory.CreateDirectory(folder);
                File.WriteAllText(Path.Combine(folder, "rewards.json"), "{ not json");
                LogAssert.Expect(LogType.Warning, new Regex("rewards\\.json could not be read"));
                SaveStore store = SaveStore.ForProfile(root, ProfileId.Create("child-1"));
                LessonOrder order = RewardTestContent.Order(RealContent.Read());
                RewardGiver giver = new RewardGiver(RewardTestContent.Of(5), RewardTestContent.Rules(), new FixedRandom());

                RewardBook book = RewardsFile.Load(store, giver, MapRoutes.Progress(null, "ro-a"), order);

                Assert.That(book.LitLessons.Select(id => id.ToString()), Is.EqualTo(new[] { "ro-a" }));
                Assert.That(Directory.GetFiles(folder, "rewards.broken-*.json"), Has.Length.EqualTo(1));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>Tests that <see cref="RewardsFile.Save"/> writes what a later <see cref="RewardsFile.Load"/> reads.</summary>
        [Test]
        public void TestSave_ReadBackByLoad()
        {
            string root = CreateTemporaryFolder();
            try
            {
                SaveStore store = SaveStore.ForProfile(root, ProfileId.Create("child-1"));
                LessonOrder order = RewardTestContent.Order(RealContent.Read());
                RewardGiver giver = new RewardGiver(RewardTestContent.Of(5), RewardTestContent.Rules(), new FixedRandom());
                RewardBook book = RewardsFile.Load(store, giver, MapRoutes.Progress(null), order);
                giver.LessonFinished(book, order.Playable[0], order);
                giver.LessonFinished(book, order.Playable[1], order);
                book.TryPlace(book.GivenStickers[0], 0.5f, 0.5f, 2f, RewardTestContent.Rules());

                bool saved = RewardsFile.Save(store, book);
                RewardBook read = RewardsFile.Load(store, giver, MapRoutes.Progress(null), order);

                Assert.That(saved, Is.True);
                Assert.That(read.StarsEarned, Is.EqualTo(6));
                Assert.That(read.GivenStickers, Is.EqualTo(book.GivenStickers));
                Assert.That(read.Placed.Count, Is.EqualTo(1));
                Assert.That(read.Placed[0].Size, Is.EqualTo(2f));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        private static string CreateTemporaryFolder()
        {
            string folder = Path.Combine(Path.GetTempPath(), "scolita-rewards-tests-" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
            return folder;
        }
    }
}
