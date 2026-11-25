using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Map;
using Scolita.Core.Progress;
using Scolita.Core.Timing;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Progress
{
    /// <summary>Tests <see cref="LearnerProgress"/>: its items, its reviews over simulated days, the days played and the map's lessons.</summary>
    public sealed class LearnerProgressTests
    {
        /// <summary>
        /// Tests that <see cref="LearnerProgress.DueOn"/> brings a learned item back 1 day after it was learned, then 3
        /// days after that review, then 7 (the brief's acceptance), each review recorded by
        /// <see cref="LearnerProgress.RecordReview"/>.
        /// </summary>
        [Test]
        public void TestDueOn_OneThenThreeThenSevenDays()
        {
            ProgressRules rules = TestProgressRules.Default();
            FakeClock clock = new FakeClock(CalendarDay.FromDate(2026, 11, 25));
            LearnerProgress progress = Progresses.Learned(clock.Today, Language.Romanian, "m");
            LearningItem m = LearningItem.Letter(ContentId.Create("m"), Language.Romanian);

            Assert.That(progress.DueOn(clock.Today), Is.Empty, "the day it was learned");
            foreach (int days in new[] { 1, 3, 7 })
            {
                for (int day = 1; day < days; day++)
                {
                    clock.PassDays(1);
                    Assert.That(progress.DueOn(clock.Today), Is.Empty, "day " + day + " of " + days);
                }

                clock.PassDays(1);
                Assert.That(progress.DueOn(clock.Today).Select(item => item.Item), Is.EqualTo(new[] { m }), "after " + days);
                progress.RecordReview(m, false, clock.Today, rules);
            }
        }

        /// <summary>Tests that <see cref="LearnerProgress.RecordReview"/> moves an item back one step after a miss, so it comes again sooner.</summary>
        [Test]
        public void TestRecordReview_AMissComesSooner()
        {
            ProgressRules rules = TestProgressRules.Default();
            CalendarDay learned = CalendarDay.FromDate(2026, 11, 25);
            LearnerProgress progress = Progresses.Learned(learned, Language.Romanian, "m");
            LearningItem m = LearningItem.Letter(ContentId.Create("m"), Language.Romanian);
            progress.RecordReview(m, false, learned.AddDays(1), rules);
            progress.RecordReview(m, false, learned.AddDays(4), rules);

            progress.RecordReview(m, true, learned.AddDays(11), rules);

            progress.TryGet(m, out ItemProgress item);
            Assert.That(item.ReviewStep, Is.EqualTo(1));
            Assert.That(item.NextReview, Is.EqualTo(learned.AddDays(14)));
            Assert.That(item.State, Is.EqualTo(ItemState.Learned));
        }

        /// <summary>Tests that <see cref="LearnerProgress.RecordReview"/> does nothing to an item that isn't learned.</summary>
        [Test]
        public void TestRecordReview_NotLearnedUnchanged()
        {
            LearnerProgress progress = Progresses.Empty();
            LearningItem m = LearningItem.Letter(ContentId.Create("m"), Language.Romanian);
            progress.Meet(m);

            progress.RecordReview(m, false, CalendarDay.FromDate(2026, 11, 25), TestProgressRules.Default());
            progress.RecordReview(LearningItem.Letter(ContentId.Create("t"), Language.Romanian), false,
                CalendarDay.FromDate(2026, 11, 25), TestProgressRules.Default());

            progress.TryGet(m, out ItemProgress item);
            Assert.That(item.NextReview, Is.Null);
            Assert.That(progress.StateOf(LearningItem.Letter(ContentId.Create("t"), Language.Romanian)), Is.EqualTo(ItemState.New));
        }

        /// <summary>Tests that <see cref="LearnerProgress.DueOn"/> puts the most overdue first, and leaves out items not due or not learned.</summary>
        [Test]
        public void TestDueOn_MostOverdueFirst()
        {
            CalendarDay today = CalendarDay.FromDate(2026, 11, 30);
            LearnerProgress later = Progresses.Learned(today.AddDays(-2), Language.Romanian, "t");
            LearnerProgress progress = new LearnerProgress(new MapProgress(new ContentId[0], default),
                Progresses.Learned(today.AddDays(-1), Language.Romanian, "p").Items
                    .Concat(later.Items)
                    .Concat(Progresses.Learned(today, Language.Romanian, "s").Items)
                    .Concat(Progresses.Learned(today.AddDays(-4), Language.English, "a").Items),
                null);
            progress.Meet(LearningItem.Letter(ContentId.Create("n"), Language.Romanian));

            Assert.That(progress.DueOn(today).Select(item => item.Item.ToString()),
                Is.EqualTo(new[] { "letter a (en)", "letter t (ro)", "letter p (ro)" }));
        }

        /// <summary>Tests that <see cref="LearnerProgress.Meet"/> makes a new item learning with no answer, and keeps an item met once.</summary>
        [Test]
        public void TestMeet_NewBecomesLearning()
        {
            LearnerProgress progress = Progresses.Empty();
            LearningItem m = LearningItem.Letter(ContentId.Create("m"), Language.Romanian);

            ItemState before = progress.StateOf(m);
            progress.Meet(m);
            progress.Meet(m);

            Assert.That(before, Is.EqualTo(ItemState.New));
            Assert.That(progress.StateOf(m), Is.EqualTo(ItemState.Learning));
            Assert.That(progress.Items.Count, Is.EqualTo(1));
            Assert.That(progress.Items[0].TotalRights, Is.EqualTo(0));
        }

        /// <summary>Tests that <see cref="LearnerProgress.RecordDayPlayed"/> keeps each day once, in order, and only the latest 100.</summary>
        [Test]
        public void TestRecordDayPlayed_EachDayOnceTheLatestKept()
        {
            CalendarDay first = CalendarDay.FromDate(2026, 1, 1);
            LearnerProgress progress = Progresses.Empty();

            for (int day = 120; day >= 0; day--)
            {
                progress.RecordDayPlayed(first.AddDays(day));
                progress.RecordDayPlayed(first.AddDays(day));
            }

            Assert.That(progress.DaysPlayed.Count, Is.EqualTo(LearnerProgress.MostDaysPlayedKept));
            Assert.That(progress.DaysPlayed[0], Is.EqualTo(first.AddDays(21)));
            Assert.That(progress.DaysPlayed[progress.DaysPlayed.Count - 1], Is.EqualTo(first.AddDays(120)));
        }

        /// <summary>
        /// Tests that <see cref="LearnerProgress.IsDone"/> answers the map from its lessons, and an item or a lesson the
        /// content doesn't know is kept.
        /// </summary>
        [Test]
        public void TestIsDone_FromItsLessons()
        {
            LearningItem unknown = new LearningItem(ItemKind.Letter, ContentId.Create("planet-99-letter"), Language.Romanian);
            LearnerProgress progress = new LearnerProgress(
                new MapProgress(new[] { ContentId.Create("ro-a"), ContentId.Create("lesson-from-later") }, ContentId.Create("en-a")),
                new[] { new ItemProgress(unknown, ItemState.Learning, null, 0, null, null, 0, null) }, null);

            progress.Lessons.MarkDone(ContentId.Create("en-a"));

            Assert.That(progress.IsDone(ContentId.Create("ro-a")), Is.True);
            Assert.That(progress.IsDone(ContentId.Create("en-a")), Is.True);
            Assert.That(progress.IsDone(ContentId.Create("ro-m")), Is.False);
            Assert.That(progress.IsDone(ContentId.Create("lesson-from-later")), Is.True);
            Assert.That(progress.StateOf(unknown), Is.EqualTo(ItemState.Learning));
        }
    }
}
