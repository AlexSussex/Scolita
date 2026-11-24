namespace Scolita.Core.Tracing
{
    /// <summary>What a run of a stroke is (<see cref="StrokeRun.Kind"/>).</summary>
    public enum StrokeRunKind
    {
        /// <summary>The letter itself.</summary>
        Body,

        /// <summary>A handwritten letter's entry stroke, from the line into the letter.</summary>
        Entry,

        /// <summary>A handwritten letter's exit stroke, towards the next letter.</summary>
        Exit
    }
}
