using NUnit.Framework;
using Scolita.Core.Progress;
using Scolita.Core.Timing;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Progress
{
    /// <summary>Tests <see cref="ReviewSchedule"/> with the default schedule of 1, 3, 7, 14 and 30 days.</summary>
    public sealed class ReviewScheduleTests
    {
        /// <summary>Tests that <see cref="ReviewSchedule.FirstReview"/> comes the day after the item is learned.</summary>
        [Test]
        public void TestFirstReview_TheNextDay()
        {
            CalendarDay learned = CalendarDay.FromDate(2026, 11, 30);

            Assert.That(ReviewSchedule.FirstReview(learned, TestProgressRules.Default()), Is.EqualTo(CalendarDay.FromDate(2026, 12, 1)));
        }

        /// <summary>Tests that <see cref="ReviewSchedule.StepAfter"/> moves one step on after a review without a miss, and the last step repeats.</summary>
        [Test]
        public void TestStepAfter_OnAndTheLastRepeats()
        {
            ProgressRules rules = TestProgressRules.Default();

            Assert.That(ReviewSchedule.StepAfter(0, false, rules), Is.EqualTo(1));
            Assert.That(ReviewSchedule.StepAfter(3, false, rules), Is.EqualTo(4));
            Assert.That(ReviewSchedule.StepAfter(4, false, rules), Is.EqualTo(4));
            Assert.That(ReviewSchedule.StepAfter(9, false, rules), Is.EqualTo(4));
        }

        /// <summary>Tests that <see cref="ReviewSchedule.StepAfter"/> moves one step back after a miss, never before the first.</summary>
        [Test]
        public void TestStepAfter_AMissGoesBackOne()
        {
            ProgressRules rules = TestProgressRules.Default();

            Assert.That(ReviewSchedule.StepAfter(2, true, rules), Is.EqualTo(1));
            Assert.That(ReviewSchedule.StepAfter(0, true, rules), Is.EqualTo(0));
            Assert.That(ReviewSchedule.StepAfter(-3, true, rules), Is.EqualTo(0));
        }

        /// <summary>Tests that <see cref="ReviewSchedule.NextReview"/> counts the step's days from the review.</summary>
        [Test]
        public void TestNextReview_TheStepsDaysFromTheReview()
        {
            ProgressRules rules = TestProgressRules.Default();
            CalendarDay reviewed = CalendarDay.FromDate(2026, 11, 25);

            Assert.That(ReviewSchedule.NextReview(0, reviewed, rules), Is.EqualTo(reviewed.AddDays(1)));
            Assert.That(ReviewSchedule.NextReview(1, reviewed, rules), Is.EqualTo(reviewed.AddDays(3)));
            Assert.That(ReviewSchedule.NextReview(2, reviewed, rules), Is.EqualTo(reviewed.AddDays(7)));
            Assert.That(ReviewSchedule.NextReview(4, reviewed, rules), Is.EqualTo(reviewed.AddDays(30)));
            Assert.That(ReviewSchedule.NextReview(7, reviewed, rules), Is.EqualTo(reviewed.AddDays(30)));
        }
    }
}
