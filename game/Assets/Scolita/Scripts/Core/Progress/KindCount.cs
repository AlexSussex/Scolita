using Scolita.Core.Content;

namespace Scolita.Core.Progress
{
    /// <summary>How many right answers an item has from one activity kind; part of <see cref="ItemProgress"/>.</summary>
    public readonly struct KindCount
    {
        /// <summary>Creates a count; called by <see cref="ItemProgress"/>, the save's reader and the tests.</summary>
        public KindCount(ActivityKind kind, int count)
        {
            Kind = kind;
            Count = count;
        }

        /// <summary>The activity kind.</summary>
        public ActivityKind Kind { get; }

        /// <summary>The right answers from it.</summary>
        public int Count { get; }
    }
}
