namespace Scolita.Core.Tracing
{
    /// <summary>
    /// What <see cref="LineCheck.Check"/> found about one line she drew from the stroke she is at: the verdict, how
    /// many strokes it did (more than one when she wrote on through strokes that continue each other), how many of
    /// her points to keep in ink, and whether it went on part-way into the next stroke before she lifted.
    /// </summary>
    public readonly struct LineResult
    {
        /// <summary>Creates a result; called by <see cref="LineCheck"/> and the tests.</summary>
        public LineResult(StrokeVerdict verdict, int strokesDone, int pointsKept, bool wentOn)
        {
            Verdict = verdict;
            StrokesDone = strokesDone;
            PointsKept = pointsKept;
            WentOn = wentOn;
        }

        /// <summary>The verdict: <see cref="StrokeVerdict.Good"/> when at least one stroke is done.</summary>
        public StrokeVerdict Verdict { get; }

        /// <summary>How many strokes the line did, from the one she was at (0 when it missed).</summary>
        public int StrokesDone { get; }

        /// <summary>How many of her points, from the first, lie along what she did (the ink kept).</summary>
        public int PointsKept { get; }

        /// <summary>Whether the line went on part-way into the stroke after the last one done before she lifted.</summary>
        public bool WentOn { get; }
    }
}
