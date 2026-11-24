namespace Scolita.Core.Content
{
    /// <summary>How many items (letters or words) a kind of step takes: from <see cref="Min"/> to <see cref="Max"/>.</summary>
    public readonly struct ItemRange
    {
        /// <summary>Creates a range; called by <see cref="ActivityKinds"/>.</summary>
        public ItemRange(int min, int max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>The fewest.</summary>
        public int Min { get; }

        /// <summary>The most.</summary>
        public int Max { get; }

        /// <summary>Whether <paramref name="count"/> is in the range; called by the step rule.</summary>
        public bool Holds(int count)
        {
            return count >= Min && count <= Max;
        }

        /// <summary><c>2 to 4</c>, or <c>exactly 1</c>, for messages.</summary>
        public override string ToString()
        {
            return Min == Max ? "exactly " + Min : Min + " to " + Max;
        }
    }
}
