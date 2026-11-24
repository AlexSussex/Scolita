namespace Scolita.Core.Tracing
{
    /// <summary>What <see cref="StrokeCheck.Check"/> found: the verdict, and how much of the stroke she covered.</summary>
    public readonly struct StrokeResult
    {
        /// <summary>Creates a result; called by <see cref="StrokeCheck"/>.</summary>
        public StrokeResult(StrokeVerdict verdict, float covered)
        {
            Verdict = verdict;
            Covered = covered;
        }

        /// <summary>The verdict.</summary>
        public StrokeVerdict Verdict { get; }

        /// <summary>
        /// The share of the stroke she covered from its start, in its direction (0 to 1), so a later kind
        /// can tell how far she got (a handwritten letter's entry stroke skipped).
        /// </summary>
        public float Covered { get; }
    }
}
