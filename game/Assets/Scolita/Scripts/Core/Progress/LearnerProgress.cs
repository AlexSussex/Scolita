using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Map;
using Scolita.Core.Timing;

namespace Scolita.Core.Progress
{
    /// <summary>
    /// One profile's progress (PRD §3.5, the <c>progress</c> save): each item she met with its answers,
    /// state and reviews (<see cref="ItemProgress"/>; ids the content no longer knows are kept, so a later
    /// version can still use them), the lessons she finished and the stop where the map last showed the
    /// rocket (<see cref="Lessons"/>, which decides the trip), and the days she played. The map asks it
    /// which lessons are done (<see cref="IDoneLessons"/>). Raises <see cref="ItemMastered"/>.
    /// </summary>
    public sealed class LearnerProgress : IDoneLessons
    {
        /// <summary>How many of the latest days she played are kept (the parent area shows the last two weeks).</summary>
        public const int MostDaysPlayedKept = 100;

        private readonly Dictionary<LearningItem, ItemProgress> items = new Dictionary<LearningItem, ItemProgress>();
        private readonly List<ItemProgress> itemsInOrder = new List<ItemProgress>();
        private readonly List<CalendarDay> daysPlayed = new List<CalendarDay>();

        /// <summary>
        /// Starts from <paramref name="lessons"/>, <paramref name="itemProgress"/> (an item listed twice keeps
        /// its first entry) and <paramref name="playedOn"/> (put in order, each once, the latest
        /// <see cref="MostDaysPlayedKept"/> kept). Called by the save file's reader, the migration and the tests.
        /// </summary>
        public LearnerProgress(MapProgress lessons, IEnumerable<ItemProgress> itemProgress, IEnumerable<CalendarDay> playedOn)
        {
            Lessons = lessons ?? throw new ArgumentNullException(nameof(lessons));
            foreach (ItemProgress item in itemProgress ?? Enumerable.Empty<ItemProgress>())
            {
                if (!items.ContainsKey(item.Item))
                {
                    items.Add(item.Item, item);
                    itemsInOrder.Add(item);
                }
            }

            daysPlayed.AddRange((playedOn ?? Enumerable.Empty<CalendarDay>()).Distinct().OrderBy(day => day));
            TrimDaysPlayed();
        }

        /// <summary>Raised when an item becomes learned.</summary>
        public event Action<ItemMastered> ItemMastered;

        /// <summary>The lessons she finished and the rocket's stop on the map.</summary>
        public MapProgress Lessons { get; }

        /// <summary>Every item she met, in the order she first met it.</summary>
        public IReadOnlyList<ItemProgress> Items => itemsInOrder;

        /// <summary>The days she finished at least one step on, in order (the latest <see cref="MostDaysPlayedKept"/>).</summary>
        public IReadOnlyList<CalendarDay> DaysPlayed => daysPlayed;

        /// <inheritdoc/>
        public bool IsDone(ContentId lessonId)
        {
            return Lessons.IsDone(lessonId);
        }

        /// <summary>
        /// The progress of <paramref name="item"/>; false when she never met it (it is new). Called by the
        /// warm-ups, the parent area later, and the tests.
        /// </summary>
        public bool TryGet(LearningItem item, out ItemProgress progress)
        {
            return items.TryGetValue(item, out progress);
        }

        /// <summary>The state of <paramref name="item"/>: new when she never met it. Called by the tests, later the parent area.</summary>
        public ItemState StateOf(LearningItem item)
        {
            return items.TryGetValue(item, out ItemProgress progress) ? progress.State : ItemState.New;
        }

        /// <summary>
        /// The learned items due for review on <paramref name="today"/>, most overdue first (those due at once
        /// first of all), each kind and language in the order she met them. Called by the warm-ups and the tests.
        /// </summary>
        public IReadOnlyList<ItemProgress> DueOn(CalendarDay today)
        {
            return itemsInOrder
                .Where(item => item.IsDue(today))
                .OrderBy(item => item.NextReview.HasValue ? item.NextReview.Value.DayNumber : int.MinValue)
                .ToArray();
        }

        /// <summary>
        /// Records that she met <paramref name="item"/> (a first contact, no answer): a new item becomes
        /// learning. Called by <see cref="LessonProgress"/>, the migration and the tests.
        /// </summary>
        public void Meet(LearningItem item)
        {
            Ensure(item);
        }

        /// <summary>
        /// Records one right answer to <paramref name="item"/> in <paramref name="kind"/> on
        /// <paramref name="today"/>; when it makes the item learned (<see cref="Mastery"/>), schedules its
        /// first review (<see cref="ReviewSchedule"/>) and raises <see cref="ItemMastered"/>. A learned item
        /// stays learned. Called by <see cref="LessonProgress"/> and the tests.
        /// </summary>
        public void RecordRight(LearningItem item, ActivityKind kind, CalendarDay today, ProgressRules rules)
        {
            ItemProgress progress = Ensure(item);
            progress.AddRight(kind, today);
            if (progress.State == ItemState.Learned || !Mastery.IsReached(progress, rules))
            {
                return;
            }

            progress.BecomeLearned(today, ReviewSchedule.FirstReview(today, rules));
            ItemMastered?.Invoke(new ItemMastered(item, today));
        }

        /// <summary>
        /// Records that she did a step of <paramref name="item"/> with the answer shown (practice, not a right
        /// answer). Called by <see cref="LessonProgress"/> and the tests.
        /// </summary>
        public void RecordPractised(LearningItem item)
        {
            Ensure(item).AddPractised();
        }

        /// <summary>
        /// Records a review of the learned <paramref name="item"/> on <paramref name="today"/>: one step on the
        /// schedule, or back when <paramref name="missed"/>; nothing for an item that isn't learned. Called by
        /// <see cref="LessonProgress"/> after a warm-up, and the tests.
        /// </summary>
        public void RecordReview(LearningItem item, bool missed, CalendarDay today, ProgressRules rules)
        {
            if (!items.TryGetValue(item, out ItemProgress progress) || progress.State != ItemState.Learned)
            {
                return;
            }

            int step = ReviewSchedule.StepAfter(progress.ReviewStep, missed, rules);
            progress.Reviewed(step, ReviewSchedule.NextReview(step, today, rules));
        }

        /// <summary>Records that she played on <paramref name="today"/>; called by <see cref="LessonProgress"/> and the tests.</summary>
        public void RecordDayPlayed(CalendarDay today)
        {
            int at = daysPlayed.BinarySearch(today);
            if (at < 0)
            {
                daysPlayed.Insert(~at, today);
                TrimDaysPlayed();
            }
        }

        private ItemProgress Ensure(LearningItem item)
        {
            if (!items.TryGetValue(item, out ItemProgress progress))
            {
                progress = new ItemProgress(item, ItemState.Learning, null, 0, null, null, 0, null);
                items.Add(item, progress);
                itemsInOrder.Add(progress);
            }

            return progress;
        }

        private void TrimDaysPlayed()
        {
            if (daysPlayed.Count > MostDaysPlayedKept)
            {
                daysPlayed.RemoveRange(0, daysPlayed.Count - MostDaysPlayedKept);
            }
        }
    }
}
