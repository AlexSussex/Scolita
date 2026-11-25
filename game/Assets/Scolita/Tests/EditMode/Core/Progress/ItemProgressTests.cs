using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Progress;
using Scolita.Core.Timing;

namespace Scolita.Tests.EditMode.Core.Progress
{
    /// <summary>Tests <see cref="ItemProgress"/>: an item's progress as saved, made safe.</summary>
    public sealed class ItemProgressTests
    {
        /// <summary>
        /// Tests that the <see cref="ItemProgress"/> constructor makes a saved item safe: negative counts become 0, a kind's
        /// counts are added, days are put in order once each, the latest 30 kept.
        /// </summary>
        [Test]
        public void TestConstructor_MakesTheSavedItemSafe()
        {
            CalendarDay first = CalendarDay.FromDate(2026, 1, 1);
            CalendarDay[] days = Enumerable.Range(0, 40).Select(day => first.AddDays(39 - day)).Concat(new[] { first }).ToArray();
            ItemProgress item = new ItemProgress(Letter("m"), ItemState.Learning,
                new[]
                {
                    new KindCount(ActivityKind.ListenAndPick, 2), new KindCount(ActivityKind.FirstSound, -4),
                    new KindCount(ActivityKind.ListenAndPick, 3)
                },
                -2, days, first, 3, first);

            Assert.That(item.RightsOf(ActivityKind.ListenAndPick), Is.EqualTo(5));
            Assert.That(item.RightsOf(ActivityKind.FirstSound), Is.EqualTo(0));
            Assert.That(item.KindsRight, Is.EqualTo(1));
            Assert.That(item.Practised, Is.EqualTo(0));
            Assert.That(item.RightDays.Count, Is.EqualTo(ItemProgress.MostRightDaysKept));
            Assert.That(item.RightDays[0], Is.EqualTo(first.AddDays(10)));
            Assert.That(item.RightDays, Is.Ordered);
        }

        /// <summary>Tests that a learning <see cref="ItemProgress"/> drops any review data, and a "new" state is read as learning.</summary>
        [Test]
        public void TestConstructor_LearningHasNoReview()
        {
            CalendarDay day = CalendarDay.FromDate(2026, 11, 25);

            ItemProgress learning = new ItemProgress(Letter("m"), ItemState.Learning, null, 0, null, day, 3, day);
            ItemProgress fromNew = new ItemProgress(Letter("t"), ItemState.New, null, 0, null, null, 0, null);

            Assert.That(learning.NextReview, Is.Null);
            Assert.That(learning.LearnedOn, Is.Null);
            Assert.That(learning.ReviewStep, Is.EqualTo(0));
            Assert.That(learning.IsDue(day.AddDays(100)), Is.False);
            Assert.That(fromNew.State, Is.EqualTo(ItemState.Learning));
        }

        /// <summary>Tests that <see cref="ItemProgress.IsDue"/> is due on and after its next review, and at once without one.</summary>
        [Test]
        public void TestIsDue_OnAndAfterItsDay()
        {
            CalendarDay day = CalendarDay.FromDate(2026, 11, 25);

            ItemProgress scheduled = new ItemProgress(Letter("m"), ItemState.Learned, null, 0, null, day, -2, day.AddDays(3));
            ItemProgress unscheduled = new ItemProgress(Letter("t"), ItemState.Learned, null, 0, null, null, 0, null);

            Assert.That(scheduled.IsDue(day.AddDays(2)), Is.False);
            Assert.That(scheduled.IsDue(day.AddDays(3)), Is.True);
            Assert.That(scheduled.IsDue(day.AddDays(9)), Is.True);
            Assert.That(scheduled.ReviewStep, Is.EqualTo(0));
            Assert.That(unscheduled.IsDue(day), Is.True);
        }

        private static LearningItem Letter(string id)
        {
            return LearningItem.Letter(ContentId.Create(id), Language.Romanian);
        }
    }
}
