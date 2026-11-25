using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Timing;

namespace Scolita.Core.Progress
{
    /// <summary>
    /// Her progress with one item (PRD §3.5): its state, its right answers by activity kind, the answers she
    /// did with the answer shown (practice, never right), the days it was answered right, and once learned,
    /// the day it was learned, its step on the review schedule and its next review. Kept by
    /// <see cref="LearnerProgress"/>, which alone changes it.
    /// </summary>
    public sealed class ItemProgress
    {
        /// <summary>How many of the latest days with a right answer are kept (enough for any "days for learned").</summary>
        public const int MostRightDaysKept = 30;

        private readonly List<KindCount> rights = new List<KindCount>();
        private readonly List<CalendarDay> rightDays = new List<CalendarDay>();

        /// <summary>
        /// Creates an item's progress as saved, made safe: counts below 0 become 0 and a kind's counts are
        /// added together; the days are put in order, each once, the latest <see cref="MostRightDaysKept"/>
        /// kept; a state other than <see cref="ItemState.Learned"/> is <see cref="ItemState.Learning"/>; a
        /// review step below 0 is 0; a learned item without a next review is due at once. Called by the save
        /// file's reader, <see cref="LearnerProgress"/> and the tests.
        /// </summary>
        public ItemProgress(LearningItem item, ItemState state, IEnumerable<KindCount> rightsByKind, int practised,
            IEnumerable<CalendarDay> daysRight, CalendarDay? learnedOn, int reviewStep, CalendarDay? nextReview)
        {
            Item = item;
            State = state == ItemState.Learned ? ItemState.Learned : ItemState.Learning;
            foreach (KindCount count in rightsByKind ?? Enumerable.Empty<KindCount>())
            {
                Add(count.Kind, Math.Max(0, count.Count));
            }

            Practised = Math.Max(0, practised);
            foreach (CalendarDay day in (daysRight ?? Enumerable.Empty<CalendarDay>()).Distinct().OrderBy(day => day))
            {
                rightDays.Add(day);
            }

            TrimDays();
            LearnedOn = State == ItemState.Learned ? learnedOn : null;
            ReviewStep = State == ItemState.Learned ? Math.Max(0, reviewStep) : 0;
            NextReview = State == ItemState.Learned ? nextReview : null;
        }

        /// <summary>The item.</summary>
        public LearningItem Item { get; }

        /// <summary>Learning or learned (an item with no progress is new).</summary>
        public ItemState State { get; private set; }

        /// <summary>Its right answers by activity kind, in the order the kinds were first right; no kind with 0.</summary>
        public IReadOnlyList<KindCount> Rights => rights;

        /// <summary>How many of its steps she did with the answer shown.</summary>
        public int Practised { get; private set; }

        /// <summary>The days it was answered right, in order, each once (the latest <see cref="MostRightDaysKept"/>).</summary>
        public IReadOnlyList<CalendarDay> RightDays => rightDays;

        /// <summary>The day it became learned; null while learning (or unknown, in a damaged save).</summary>
        public CalendarDay? LearnedOn { get; private set; }

        /// <summary>Its step on the review schedule, from 0 (the first review); 0 while learning.</summary>
        public int ReviewStep { get; private set; }

        /// <summary>The day its next review is due; null while learning, and for a learned item due at once.</summary>
        public CalendarDay? NextReview { get; private set; }

        /// <summary>All its right answers.</summary>
        public int TotalRights => rights.Sum(count => count.Count);

        /// <summary>How many activity kinds it was answered right in.</summary>
        public int KindsRight => rights.Count;

        /// <summary>
        /// Whether it is learned and its review is due on <paramref name="today"/> (or earlier). Called by
        /// <see cref="LearnerProgress"/>, the warm-ups and the tests.
        /// </summary>
        public bool IsDue(CalendarDay today)
        {
            return State == ItemState.Learned && (!NextReview.HasValue || NextReview.Value.CompareTo(today) <= 0);
        }

        /// <summary>The right answers from <paramref name="kind"/>; called by the tests.</summary>
        public int RightsOf(ActivityKind kind)
        {
            foreach (KindCount count in rights)
            {
                if (count.Kind == kind)
                {
                    return count.Count;
                }
            }

            return 0;
        }

        internal void AddRight(ActivityKind kind, CalendarDay day)
        {
            Add(kind, 1);
            int at = rightDays.BinarySearch(day);
            if (at < 0)
            {
                rightDays.Insert(~at, day);
                TrimDays();
            }
        }

        internal void AddPractised()
        {
            Practised++;
        }

        internal void BecomeLearned(CalendarDay day, CalendarDay firstReview)
        {
            State = ItemState.Learned;
            LearnedOn = day;
            ReviewStep = 0;
            NextReview = firstReview;
        }

        internal void Reviewed(int step, CalendarDay nextReview)
        {
            ReviewStep = step;
            NextReview = nextReview;
        }

        private void Add(ActivityKind kind, int count)
        {
            for (int index = 0; index < rights.Count; index++)
            {
                if (rights[index].Kind == kind)
                {
                    rights[index] = new KindCount(kind, rights[index].Count + count);
                    return;
                }
            }

            if (count > 0)
            {
                rights.Add(new KindCount(kind, count));
            }
        }

        private void TrimDays()
        {
            if (rightDays.Count > MostRightDaysKept)
            {
                rightDays.RemoveRange(0, rightDays.Count - MostRightDaysKept);
            }
        }
    }
}
