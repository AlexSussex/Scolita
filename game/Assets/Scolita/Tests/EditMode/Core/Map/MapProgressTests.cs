using System.Linq;
using NUnit.Framework;
using Scolita.Core.Activities;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;
using Scolita.Core.Map;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Map
{
    /// <summary>Tests <see cref="MapProgress"/>.</summary>
    public sealed class MapProgressTests
    {
        /// <summary>Tests that the constructor merges duplicates, leaves out unset ids and keeps the order.</summary>
        [Test]
        public void TestConstructor_MergesDuplicates()
        {
            MapProgress progress = new MapProgress(new[]
            {
                ContentId.Create("en-a"), default, ContentId.Create("ro-a"), ContentId.Create("en-a")
            }, ContentId.Create("ro-m"));

            Assert.That(progress.DoneLessons.Select(id => id.ToString()), Is.EqualTo(new[] { "en-a", "ro-a" }));
            Assert.That(progress.RocketStop.ToString(), Is.EqualTo("ro-m"));
        }

        /// <summary>Tests that <see cref="MapProgress.MarkDone"/> records a lesson once, however often it is finished.</summary>
        [Test]
        public void TestMarkDone_Once()
        {
            MapProgress progress = new MapProgress(new ContentId[0], default);

            progress.MarkDone(ContentId.Create("ro-a"));
            progress.MarkDone(ContentId.Create("ro-a"));

            Assert.That(progress.IsDone(ContentId.Create("ro-a")), Is.True);
            Assert.That(progress.IsDone(ContentId.Create("en-a")), Is.False);
            Assert.That(progress.DoneLessons.Count, Is.EqualTo(1));
        }

        /// <summary>Tests that <see cref="MapProgress.ShowRocketAt"/> records the stop shown, unset at first.</summary>
        [Test]
        public void TestShowRocketAt_RecordsTheStop()
        {
            MapProgress progress = new MapProgress(new ContentId[0], default);
            bool setBefore = progress.RocketStop.IsSet;

            progress.ShowRocketAt(ContentId.Create("en-m"));

            Assert.That(setBefore, Is.False);
            Assert.That(progress.RocketStop.ToString(), Is.EqualTo("en-m"));
        }

        /// <summary>
        /// Tests that <see cref="MapProgress.Before"/> takes every playable lesson before the saved one as done (as Go
        /// opened them one after another before the map), the rocket never shown.
        /// </summary>
        [Test]
        public void TestBefore_EveryLessonBeforeTheSavedOne()
        {
            Curriculum curriculum = RealContent.Read();
            LessonOrder order = new LessonOrder(curriculum.Domains[0], LanguageMix.Both, new PlayableSteps(ActivityRulesFactory.Kinds, curriculum));

            MapProgress progress = MapProgress.Before(order, ContentId.Create("ro-s"));

            Assert.That(progress.DoneLessons.First().ToString(), Is.EqualTo("ro-a"));
            Assert.That(progress.DoneLessons.Last().ToString(), Is.EqualTo("party-1"));
            Assert.That(progress.IsDone(ContentId.Create("ro-s")), Is.False);
            Assert.That(progress.RocketStop.IsSet, Is.False);
        }

        /// <summary>Tests that <see cref="MapProgress.Before"/> takes nothing as done without a saved lesson, or with one that can't be played.</summary>
        [Test]
        public void TestBefore_NothingWithoutASavedLesson()
        {
            Curriculum curriculum = RealContent.Read();
            LessonOrder order = new LessonOrder(curriculum.Domains[0], LanguageMix.RomanianOnly, new PlayableSteps(ActivityRulesFactory.Kinds, curriculum));

            Assert.That(MapProgress.Before(order, default).DoneLessons, Is.Empty);
            Assert.That(MapProgress.Before(order, ContentId.Create("en-m")).DoneLessons, Is.Empty);
            Assert.That(MapProgress.Before(order, ContentId.Create("no-such-lesson")).DoneLessons, Is.Empty);
        }
    }
}
