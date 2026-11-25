using Scolita.Core.Timing;

namespace Scolita.Core.Progress
{
    /// <summary>Raised by <see cref="LearnerProgress.ItemMastered"/> when an item becomes learned.</summary>
    public readonly struct ItemMastered
    {
        /// <summary>Creates the event; called by <see cref="LearnerProgress"/>.</summary>
        public ItemMastered(LearningItem item, CalendarDay day)
        {
            Item = item;
            Day = day;
        }

        /// <summary>The item learned.</summary>
        public LearningItem Item { get; }

        /// <summary>The day it was learned.</summary>
        public CalendarDay Day { get; }
    }
}
