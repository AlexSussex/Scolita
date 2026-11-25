using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Languages;

namespace Scolita.Core.Progress
{
    /// <summary>
    /// The numbers of "learned" and of the reviews (<c>docs/curriculum.md</c> §5; filled from
    /// the learning rules asset, beside <c>LearningRules</c>): how many right answers make an item
    /// learned in each language, across how many activity kinds and on how many days; the review
    /// schedule and how far a miss moves an item back; how many warm-ups start a lesson and how many
    /// letters a warm-up offers; and how long the dated copies of a damaged save are kept.
    /// </summary>
    public sealed class ProgressRules
    {
        private readonly int[] reviewIntervals;

        /// <summary>
        /// Creates the numbers; throws an <see cref="ArgumentOutOfRangeException"/> for one out of its range
        /// (at least 1 right answer, kind and day, the days at most <see cref="ItemProgress.MostRightDaysKept"/>,
        /// at least one review interval, each of at least a day, steps back and warm-ups not negative, 2 to 4
        /// choices, at least 1 fine day) and an <see cref="ArgumentNullException"/> for no intervals. Called by the learning rules
        /// asset and the tests.
        /// </summary>
        public ProgressRules(int romanianRightsForLearned, int englishRightsForLearned, int kindsForLearned,
            int daysForLearned, IReadOnlyList<int> reviewIntervalDays, int reviewStepsBackAfterMiss, int warmUpsAtStart,
            int warmUpChoices, int damagedCopyFineDays)
        {
            RequireAtLeast(romanianRightsForLearned, 1, nameof(romanianRightsForLearned));
            RequireAtLeast(englishRightsForLearned, 1, nameof(englishRightsForLearned));
            RequireAtLeast(kindsForLearned, 1, nameof(kindsForLearned));
            RequireAtLeast(daysForLearned, 1, nameof(daysForLearned));
            if (daysForLearned > ItemProgress.MostRightDaysKept)
            {
                throw new ArgumentOutOfRangeException(nameof(daysForLearned), daysForLearned,
                    "Only the latest " + ItemProgress.MostRightDaysKept + " days of an item are kept.");
            }

            if (reviewIntervalDays == null)
            {
                throw new ArgumentNullException(nameof(reviewIntervalDays));
            }

            if (reviewIntervalDays.Count == 0 || reviewIntervalDays.Any(days => days < 1))
            {
                throw new ArgumentOutOfRangeException(nameof(reviewIntervalDays), reviewIntervalDays.Count,
                    "The review schedule has at least one step, each of at least a day.");
            }

            RequireAtLeast(reviewStepsBackAfterMiss, 0, nameof(reviewStepsBackAfterMiss));
            RequireAtLeast(warmUpsAtStart, 0, nameof(warmUpsAtStart));
            if (warmUpChoices < 2 || warmUpChoices > 4)
            {
                throw new ArgumentOutOfRangeException(nameof(warmUpChoices), warmUpChoices,
                    "A warm-up offers 2 to 4 letters (ListenAndPick's and FirstSound's range).");
            }

            RequireAtLeast(damagedCopyFineDays, 1, nameof(damagedCopyFineDays));
            RomanianRightsForLearned = romanianRightsForLearned;
            EnglishRightsForLearned = englishRightsForLearned;
            KindsForLearned = kindsForLearned;
            DaysForLearned = daysForLearned;
            reviewIntervals = reviewIntervalDays.ToArray();
            ReviewStepsBackAfterMiss = reviewStepsBackAfterMiss;
            WarmUpsAtStart = warmUpsAtStart;
            WarmUpChoices = warmUpChoices;
            DamagedCopyFineDays = damagedCopyFineDays;
        }

        /// <summary>How many right answers make a Romanian item learned (6).</summary>
        public int RomanianRightsForLearned { get; }

        /// <summary>How many right answers make an English item learned (6; English may be given more, §5).</summary>
        public int EnglishRightsForLearned { get; }

        /// <summary>Across how many activity kinds the right answers must be (2).</summary>
        public int KindsForLearned { get; }

        /// <summary>On how many different days the right answers must be (2).</summary>
        public int DaysForLearned { get; }

        /// <summary>The days from one review to the next, step by step (1, 3, 7, 14, 30); the last step repeats.</summary>
        public IReadOnlyList<int> ReviewIntervalDays => reviewIntervals;

        /// <summary>How many steps of the schedule a miss in a review moves an item back (1).</summary>
        public int ReviewStepsBackAfterMiss { get; }

        /// <summary>The most warm-ups (review steps) at a lesson's start (2).</summary>
        public int WarmUpsAtStart { get; }

        /// <summary>How many letters a warm-up offers, the one reviewed among them (3).</summary>
        public int WarmUpChoices { get; }

        /// <summary>
        /// On how many different days a newer version of the app must read and save a file fine before its damaged
        /// copies are removed (3; the human, 2026-11-24: "several days").
        /// </summary>
        public int DamagedCopyFineDays { get; }

        /// <summary>
        /// The right answers that make an item of <paramref name="language"/> learned. Called by
        /// <see cref="Mastery"/> and the tests.
        /// </summary>
        public int RightsForLearned(Language language)
        {
            return language == Language.English ? EnglishRightsForLearned : RomanianRightsForLearned;
        }

        private static void RequireAtLeast(int value, int least, string name)
        {
            if (value < least)
            {
                throw new ArgumentOutOfRangeException(name, value, name + " is at least " + least + ".");
            }
        }
    }
}
