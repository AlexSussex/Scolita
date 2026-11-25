using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Lessons;
using Scolita.Core.Randomness;
using Scolita.Core.Rewards;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Rewards
{
    /// <summary>Tests <see cref="RewardGiver"/> on the real lessons, with the test rewards of <see cref="RewardTestContent"/>.</summary>
    public sealed class RewardGiverTests
    {
        /// <summary>Tests that <see cref="RewardGiver.LessonFinished"/> gives three stars and lights the lesson.</summary>
        [Test]
        public void TestLessonFinished_ThreeStarsForEveryLesson()
        {
            Curriculum curriculum = RealContent.Read();
            LessonOrder order = RewardTestContent.Order(curriculum);
            RewardGiver giver = new RewardGiver(RewardTestContent.Of(5), RewardTestContent.Rules(), new FixedRandom());
            RewardBook book = RewardBook.Empty();

            LessonRewards rewards = giver.LessonFinished(book, order.Playable[0], order);

            Assert.That(rewards.Stars, Is.EqualTo(3));
            Assert.That(book.StarsEarned, Is.EqualTo(3));
            Assert.That(book.IsLit(order.Playable[0].Id), Is.True);
            Assert.That(rewards.Sticker, Is.Null);
            Assert.That(rewards.Trail, Is.Null);
        }

        /// <summary>Tests that <see cref="RewardGiver.LessonFinished"/> gives a replay its stars again, lighting its places once.</summary>
        [Test]
        public void TestLessonFinished_AReplayGivesItsStarsAgain()
        {
            Curriculum curriculum = RealContent.Read();
            LessonOrder order = RewardTestContent.Order(curriculum);
            RewardGiver giver = new RewardGiver(RewardTestContent.Of(5), RewardTestContent.Rules(), new FixedRandom());
            RewardBook book = RewardBook.Empty();

            giver.LessonFinished(book, order.Playable[0], order);
            LessonRewards again = giver.LessonFinished(book, order.Playable[0], order);

            Assert.That(again.Stars, Is.EqualTo(3));
            Assert.That(book.StarsEarned, Is.EqualTo(6));
            Assert.That(book.LitLessons, Is.EqualTo(new[] { order.Playable[0].Id }));
            Assert.That(book.LessonsFinished, Is.EqualTo(2));
        }

        /// <summary>Tests that <see cref="RewardGiver.LessonFinished"/> gives a sticker every second lesson.</summary>
        [Test]
        public void TestLessonFinished_AStickerEverySecondLesson()
        {
            Curriculum curriculum = RealContent.Read();
            LessonOrder order = RewardTestContent.Order(curriculum);
            RewardGiver giver = new RewardGiver(RewardTestContent.Of(5), RewardTestContent.Rules(), new FixedRandom());
            RewardBook book = RewardBook.Empty();

            bool[] stickers = order.Playable
                .Take(4)
                .Select(lesson => giver.LessonFinished(book, lesson, order).Sticker != null)
                .ToArray();

            Assert.That(stickers, Is.EqualTo(new[] { false, true, false, true }));
            Assert.That(book.GivenStickers.Count, Is.EqualTo(2));
        }

        /// <summary>Tests that <see cref="RewardGiver.LessonFinished"/> gives no sticker twice until every one is given, round after round.</summary>
        [Test]
        public void TestLessonFinished_NoStickerRepeatsUntilAllAreGiven()
        {
            Curriculum curriculum = RealContent.Read();
            LessonOrder order = RewardTestContent.Order(curriculum);
            RewardGiver giver = new RewardGiver(RewardTestContent.Of(5), RewardTestContent.Rules(), new SeededRandom(7));
            RewardBook book = RewardBook.Empty();
            Lesson lesson = order.Playable[0];

            for (int finished = 0; finished < 30; finished++)
            {
                giver.LessonFinished(book, lesson, order);
            }

            Assert.That(book.GivenStickers.Count, Is.EqualTo(15));
            for (int round = 0; round < 3; round++)
            {
                IEnumerable<ContentId> given = book.GivenStickers.Skip(round * 5).Take(5);
                Assert.That(given.Distinct().Count(), Is.EqualTo(5), "round " + (round + 1));
            }
        }

        /// <summary>Tests that <see cref="RewardGiver.LessonFinished"/> never gives a retired sticker.</summary>
        [Test]
        public void TestLessonFinished_ARetiredStickerIsNeverGiven()
        {
            Curriculum curriculum = RealContent.Read();
            LessonOrder order = RewardTestContent.Order(curriculum);
            RewardGiver giver = new RewardGiver(RewardTestContent.Of(3, "s2"), RewardTestContent.Rules(), new SeededRandom(3));
            RewardBook book = RewardBook.Empty();

            for (int finished = 0; finished < 12; finished++)
            {
                giver.LessonFinished(book, order.Playable[0], order);
            }

            Assert.That(book.GivenStickers, Has.None.EqualTo(ContentId.Create("s2")));
            Assert.That(book.GivenStickers.Count, Is.EqualTo(6));
        }

        /// <summary>Tests that <see cref="RewardGiver.LessonFinished"/> gives a planet's end a trail, chosen at once, and a sticker.</summary>
        [Test]
        public void TestLessonFinished_APlanetsEndGivesATrailAndASticker()
        {
            Curriculum curriculum = RealContent.Read();
            LessonOrder order = RewardTestContent.Order(curriculum);
            RewardGiver giver = new RewardGiver(RewardTestContent.Of(40), RewardTestContent.Rules(), new FixedRandom());
            RewardBook book = RewardBook.Empty();
            Lesson[] planet = RewardTestContent.LessonsOf(order, "planet-1");

            LessonRewards[] given = planet.Select(lesson => giver.LessonFinished(book, lesson, order)).ToArray();

            Assert.That(given.Take(given.Length - 1).Select(rewards => rewards.Trail), Is.All.Null);
            LessonRewards last = given.Last();
            Assert.That(last.FinishedPlanet, Is.True);
            Assert.That(last.Trail.Id.ToString(), Is.EqualTo("rainbow"));
            Assert.That(last.Sticker, Is.Not.Null);
            Assert.That(book.ChosenTrail.ToString(), Is.EqualTo("rainbow"));
            Assert.That(book.RewardedUnits.Select(unit => unit.ToString()), Is.EqualTo(new[] { "planet-1" }));
        }

        /// <summary>Tests that <see cref="RewardGiver.LessonFinished"/> gives the trails in their order, once per planet, and keeps the one chosen.</summary>
        [Test]
        public void TestLessonFinished_TrailsInOrderOncePerPlanet()
        {
            Curriculum curriculum = RealContent.Read();
            LessonOrder order = RewardTestContent.Order(curriculum);
            RewardGiver giver = new RewardGiver(RewardTestContent.Of(40), RewardTestContent.Rules(), new FixedRandom());
            RewardBook book = RewardBook.Empty();
            Lesson[] first = RewardTestContent.LessonsOf(order, "planet-1");

            foreach (Lesson lesson in first.Concat(first).Concat(RewardTestContent.LessonsOf(order, "planet-2")))
            {
                giver.LessonFinished(book, lesson, order);
            }

            Assert.That(book.Trails.Select(trail => trail.ToString()), Is.EqualTo(new[] { "rainbow", "stars" }));
            Assert.That(book.ChosenTrail.ToString(), Is.EqualTo("rainbow"));
        }

        /// <summary>Tests that <see cref="RewardGiver.LessonFinished"/> raises <see cref="RewardGiver.RewardEarned"/> for each reward.</summary>
        [Test]
        public void TestLessonFinished_RaisesEachReward()
        {
            Curriculum curriculum = RealContent.Read();
            LessonOrder order = RewardTestContent.Order(curriculum);
            RewardGiver giver = new RewardGiver(RewardTestContent.Of(40), RewardTestContent.Rules(), new FixedRandom());
            RewardBook book = RewardBook.Empty();
            List<RewardKind> raised = new List<RewardKind>();
            giver.RewardEarned += earned => raised.Add(earned.Kind);

            foreach (Lesson lesson in RewardTestContent.LessonsOf(order, "planet-1"))
            {
                raised.Clear();
                giver.LessonFinished(book, lesson, order);
            }

            Assert.That(raised, Is.EqualTo(new[] { RewardKind.Stars, RewardKind.Trail, RewardKind.Sticker }));
        }

        /// <summary>Tests that <see cref="RewardGiver.CatchUp"/> gives a profile's done lessons their stars and finished planets their trails, and no sticker.</summary>
        [Test]
        public void TestCatchUp_StarsAndTrailsForWhatWasDone()
        {
            Curriculum curriculum = RealContent.Read();
            LessonOrder order = RewardTestContent.Order(curriculum);
            RewardGiver giver = new RewardGiver(RewardTestContent.Of(5), RewardTestContent.Rules(), new FixedRandom());
            string[] done = RewardTestContent.LessonsOf(order, "planet-1")
                .Select(lesson => lesson.Id.ToString())
                .Append(RewardTestContent.LessonsOf(order, "planet-2")[0].Id.ToString())
                .ToArray();

            RewardBook book = giver.CatchUp(MapRoutes.Progress(null, done), order);

            Assert.That(book.LitLessons.Select(lesson => lesson.ToString()), Is.EqualTo(done));
            Assert.That(book.StarsEarned, Is.EqualTo(done.Length * 3));
            Assert.That(book.Trails.Select(trail => trail.ToString()), Is.EqualTo(new[] { "rainbow" }));
            Assert.That(book.ChosenTrail.ToString(), Is.EqualTo("rainbow"));
            Assert.That(book.GivenStickers, Is.Empty);
        }
    }
}
