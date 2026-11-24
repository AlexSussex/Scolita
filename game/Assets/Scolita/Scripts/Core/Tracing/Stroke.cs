using System.Collections.Generic;

namespace Scolita.Core.Tracing
{
    /// <summary>
    /// One stroke of a letter, from finger down to finger up: its runs, drawn without lifting, and the
    /// path she must follow (<see cref="StrokePath"/>), made from them once.
    /// </summary>
    public sealed class Stroke
    {
        /// <summary>
        /// Creates a stroke of <paramref name="runs"/>, leaned by <paramref name="slant"/>; throws an
        /// <see cref="System.ArgumentException"/> for no run. Called by <see cref="LetterShapeReader"/>, the
        /// shape asset and the tests.
        /// </summary>
        public Stroke(IReadOnlyList<StrokeRun> runs, float slant)
        {
            Path = StrokePath.Of(runs, slant);
            Runs = runs;
        }

        /// <summary>Its runs, in order.</summary>
        public IReadOnlyList<StrokeRun> Runs { get; }

        /// <summary>The path she follows, slanted and sampled.</summary>
        public StrokePath Path { get; }
    }
}
