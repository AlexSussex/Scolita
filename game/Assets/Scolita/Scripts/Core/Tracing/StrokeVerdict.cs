namespace Scolita.Core.Tracing
{
    /// <summary>What <see cref="StrokeCheck"/> says about one stroke she drew.</summary>
    public enum StrokeVerdict
    {
        /// <summary>From the start dot, along the stroke, in its direction, far enough.</summary>
        Good,

        /// <summary>She began away from the start dot.</summary>
        StartedElsewhere,

        /// <summary>She drew it the other way round (from its end, or round the wrong way).</summary>
        WrongDirection,

        /// <summary>Her line wandered off the stroke.</summary>
        LeftThePath,

        /// <summary>A tap, or a line that stopped before the end of the stroke.</summary>
        TooShort,

        /// <summary>
        /// She began where a handwritten letter's entry run ends (the letter's body) instead of on the line,
        /// leaving out the entry stroke („Pornește de pe linie.”).
        /// </summary>
        SkippedEntry
    }
}
