namespace Scolita.Core.Tracing
{
    /// <summary>
    /// Where the writing lines are, in a letter shape's units, the same for every shape (the
    /// <c>curriculum-research</c> convention): a small letter fills the space between the middle line
    /// and the baseline, t, d and the capitals reach the top line, p reaches the line below.
    /// </summary>
    public static class WritingLines
    {
        /// <summary>The top line.</summary>
        public const float Top = 0f;

        /// <summary>The middle line.</summary>
        public const float Middle = 100f;

        /// <summary>The baseline, where a letter sits.</summary>
        public const float Baseline = 200f;

        /// <summary>The line below the baseline, where p's stem ends.</summary>
        public const float Below = 300f;
    }
}
