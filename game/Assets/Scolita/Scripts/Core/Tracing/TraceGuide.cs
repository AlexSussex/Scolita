namespace Scolita.Core.Tracing
{
    /// <summary>What a round of <c>TraceLetter</c> shows of the letter (<see cref="TraceState.Guide"/>).</summary>
    public enum TraceGuide
    {
        /// <summary>Round 1: the letter as a dotted guide, the start dot with its number, and the arrow.</summary>
        Dotted,

        /// <summary>Round 2: a faint shadow of the letter, the start dot and the arrow.</summary>
        Faint,

        /// <summary>Round 3: the start dot only ("fără ajutor").</summary>
        StartDotOnly
    }
}
