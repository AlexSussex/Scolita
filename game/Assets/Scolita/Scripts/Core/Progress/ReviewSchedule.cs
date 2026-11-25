using System;
using Scolita.Core.Timing;

namespace Scolita.Core.Progress
{
    /// <summary>
    /// When a learned item comes back (<c>docs/curriculum.md</c> §5), the one owner of this rule: its first
    /// review comes <see cref="ProgressRules.ReviewIntervalDays"/>' first number of days after it was learned;
    /// a review done without a miss moves it one step on (the last step repeats); a miss moves it
    /// <see cref="ProgressRules.ReviewStepsBackAfterMiss"/> steps back (never before the first); the next
    /// review is that step's days after the review.
    /// </summary>
    public static class ReviewSchedule
    {
        /// <summary>
        /// The first review of an item learned on <paramref name="learnedOn"/>. Called by
        /// <see cref="LearnerProgress"/> and the tests.
        /// </summary>
        public static CalendarDay FirstReview(CalendarDay learnedOn, ProgressRules rules)
        {
            return learnedOn.AddDays(rules.ReviewIntervalDays[0]);
        }

        /// <summary>
        /// The step after a review at <paramref name="step"/>, <paramref name="missed"/> or not. Called by
        /// <see cref="LearnerProgress"/> and the tests.
        /// </summary>
        public static int StepAfter(int step, bool missed, ProgressRules rules)
        {
            int last = rules.ReviewIntervalDays.Count - 1;
            int current = Math.Min(Math.Max(0, step), last);
            return missed ? Math.Max(0, current - rules.ReviewStepsBackAfterMiss) : Math.Min(last, current + 1);
        }

        /// <summary>
        /// The review after one on <paramref name="reviewedOn"/> that left the item at <paramref name="step"/>.
        /// Called by <see cref="LearnerProgress"/> and the tests.
        /// </summary>
        public static CalendarDay NextReview(int step, CalendarDay reviewedOn, ProgressRules rules)
        {
            int last = rules.ReviewIntervalDays.Count - 1;
            return reviewedOn.AddDays(rules.ReviewIntervalDays[Math.Min(Math.Max(0, step), last)]);
        }
    }
}
