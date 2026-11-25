using System.Collections.Generic;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Progress;
using Scolita.Core.Timing;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Progress
{
    /// <summary>Tests <see cref="Mastery"/>, directly and through <see cref="LearnerProgress.RecordRight"/> with a fake clock's days.</summary>
    public sealed class MasteryTests
    {
        /// <summary>
        /// Tests that <see cref="Mastery.IsReached"/> makes an item learned after 6 right answers across 2 kinds on 2
        /// days (the brief's acceptance), raising <see cref="LearnerProgress.ItemMastered"/> once, on the sixth.
        /// </summary>
        [Test]
        public void TestRecordRight_SixRightsAcrossTwoKindsOnTwoDays()
        {
            FakeClock clock = new FakeClock(CalendarDay.FromDate(2026, 11, 25));
            LearnerProgress progress = Progresses.Empty();
            ProgressRules rules = TestProgressRules.Default();
            LearningItem m = LearningItem.Letter(ContentId.Create("m"), Language.Romanian);
            List<ItemMastered> mastered = new List<ItemMastered>();
            progress.ItemMastered += mastered.Add;

            for (int answer = 0; answer < 3; answer++)
            {
                progress.RecordRight(m, ActivityKind.ListenAndPick, clock.Today, rules);
            }

            clock.PassDays(1);
            for (int answer = 0; answer < 2; answer++)
            {
                progress.RecordRight(m, ActivityKind.FirstSound, clock.Today, rules);
            }

            ItemState beforeTheSixth = progress.StateOf(m);
            progress.RecordRight(m, ActivityKind.FirstSound, clock.Today, rules);
            progress.RecordRight(m, ActivityKind.LetterHunt, clock.Today, rules);

            Assert.That(beforeTheSixth, Is.EqualTo(ItemState.Learning));
            Assert.That(progress.StateOf(m), Is.EqualTo(ItemState.Learned));
            Assert.That(mastered.Count, Is.EqualTo(1));
            Assert.That(mastered[0].Item, Is.EqualTo(m));
            Assert.That(mastered[0].Day, Is.EqualTo(clock.Today));
        }

        /// <summary>Tests that <see cref="Mastery.IsReached"/> keeps an item answered right 6 times on one day learning.</summary>
        [Test]
        public void TestRecordRight_SixRightsOnOneDayStaysLearning()
        {
            CalendarDay today = CalendarDay.FromDate(2026, 11, 25);
            LearnerProgress progress = Progresses.Empty();
            ProgressRules rules = TestProgressRules.Default();
            LearningItem m = LearningItem.Letter(ContentId.Create("m"), Language.Romanian);

            for (int answer = 0; answer < 3; answer++)
            {
                progress.RecordRight(m, ActivityKind.ListenAndPick, today, rules);
                progress.RecordRight(m, ActivityKind.FirstSound, today, rules);
            }

            Assert.That(progress.StateOf(m), Is.EqualTo(ItemState.Learning));
        }

        /// <summary>Tests that <see cref="Mastery.IsReached"/> keeps an item answered right in one kind only learning, whatever the days.</summary>
        [Test]
        public void TestRecordRight_OneKindStaysLearning()
        {
            FakeClock clock = new FakeClock(CalendarDay.FromDate(2026, 11, 25));
            LearnerProgress progress = Progresses.Empty();
            ProgressRules rules = TestProgressRules.Default();
            LearningItem m = LearningItem.Letter(ContentId.Create("m"), Language.Romanian);

            for (int answer = 0; answer < 8; answer++)
            {
                progress.RecordRight(m, ActivityKind.ListenAndPick, clock.Today, rules);
                clock.PassDays(1);
            }

            Assert.That(progress.StateOf(m), Is.EqualTo(ItemState.Learning));
        }

        /// <summary>
        /// Tests that <see cref="Mastery.IsReached"/> takes an English item's own number of right answers, so English can be
        /// given more practice, while the Romanian item of the same letter is learned at 6.
        /// </summary>
        [Test]
        public void TestRecordRight_EnglishHasItsOwnNumber()
        {
            CalendarDay day = CalendarDay.FromDate(2026, 11, 25);
            LearnerProgress progress = Progresses.Empty();
            ProgressRules rules = TestProgressRules.WithEnglishRights(8);
            LearningItem romanian = LearningItem.Letter(ContentId.Create("m"), Language.Romanian);
            LearningItem english = LearningItem.Letter(ContentId.Create("m"), Language.English);

            for (int answer = 0; answer < 3; answer++)
            {
                foreach (LearningItem item in new[] { romanian, english })
                {
                    progress.RecordRight(item, ActivityKind.LetterHunt, day, rules);
                    progress.RecordRight(item, ActivityKind.FirstSound, day.AddDays(1), rules);
                }
            }

            Assert.That(progress.StateOf(romanian), Is.EqualTo(ItemState.Learned));
            Assert.That(progress.StateOf(english), Is.EqualTo(ItemState.Learning));
            Assert.That(Mastery.IsReached(Item(progress, english), TestProgressRules.Default()), Is.True);
        }

        /// <summary>Tests that <see cref="Mastery.IsReached"/> never counts practice (an answer shown) as right.</summary>
        [Test]
        public void TestRecordPractised_PracticeNeverCounts()
        {
            LearnerProgress progress = Progresses.Empty();
            LearningItem m = LearningItem.Letter(ContentId.Create("m"), Language.Romanian);

            for (int answer = 0; answer < 10; answer++)
            {
                progress.RecordPractised(m);
            }

            Assert.That(progress.StateOf(m), Is.EqualTo(ItemState.Learning));
            Assert.That(Item(progress, m).Practised, Is.EqualTo(10));
            Assert.That(Item(progress, m).TotalRights, Is.EqualTo(0));
            Assert.That(Item(progress, m).RightDays, Is.Empty);
        }

        /// <summary>
        /// Tests that <see cref="Mastery.IsReached"/> needs all three: the number of right answers, the kinds and the days,
        /// each one short keeping an item learning.
        /// </summary>
        [Test]
        public void TestIsReached_NeedsRightsKindsAndDays()
        {
            CalendarDay day = CalendarDay.FromDate(2026, 11, 25);
            ProgressRules rules = TestProgressRules.Default();
            LearningItem m = LearningItem.Letter(ContentId.Create("m"), Language.Romanian);
            CalendarDay[] twoDays = { day, day.AddDays(1) };

            ItemProgress reached = Saved(m, new[] { new KindCount(ActivityKind.LetterHunt, 3), new KindCount(ActivityKind.FirstSound, 3) }, twoDays);
            ItemProgress fewRights = Saved(m, new[] { new KindCount(ActivityKind.LetterHunt, 3), new KindCount(ActivityKind.FirstSound, 2) }, twoDays);
            ItemProgress oneKind = Saved(m, new[] { new KindCount(ActivityKind.LetterHunt, 6) }, twoDays);
            ItemProgress oneDay = Saved(m, new[] { new KindCount(ActivityKind.LetterHunt, 3), new KindCount(ActivityKind.FirstSound, 3) }, new[] { day });

            Assert.That(Mastery.IsReached(reached, rules), Is.True);
            Assert.That(Mastery.IsReached(fewRights, rules), Is.False);
            Assert.That(Mastery.IsReached(oneKind, rules), Is.False);
            Assert.That(Mastery.IsReached(oneDay, rules), Is.False);
        }

        private static ItemProgress Saved(LearningItem item, KindCount[] rights, CalendarDay[] days)
        {
            return new ItemProgress(item, ItemState.Learning, rights, 0, days, null, 0, null);
        }

        private static ItemProgress Item(LearnerProgress progress, LearningItem item)
        {
            Assert.That(progress.TryGet(item, out ItemProgress found), Is.True, "no progress for " + item);
            return found;
        }
    }
}
