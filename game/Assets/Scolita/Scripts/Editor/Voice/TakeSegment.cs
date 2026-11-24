namespace Scolita.Editor.Voice
{
    /// <summary>One piece of sound in a take, between two pauses, as <see cref="TakeSplitter"/> finds it: its first and last sample.</summary>
    public readonly struct TakeSegment
    {
        /// <summary>Creates a piece; called by <see cref="TakeSplitter"/> and tests.</summary>
        public TakeSegment(int start, int end)
        {
            Start = start;
            End = end;
        }

        /// <summary>Its first sample of sound.</summary>
        public int Start { get; }

        /// <summary>The sample just after its sound.</summary>
        public int End { get; }

        /// <summary>Its length in samples.</summary>
        public int Length => End - Start;
    }
}
