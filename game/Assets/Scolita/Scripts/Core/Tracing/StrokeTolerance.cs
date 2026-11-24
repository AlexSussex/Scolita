using System;

namespace Scolita.Core.Tracing
{
    /// <summary>
    /// How forgiving <see cref="StrokeCheck"/> is, in a shape's units (a small letter is 100 high): how far
    /// from the stroke her line may stray, how close to the start dot she must begin, and how much of the
    /// stroke she must cover. One per round of <c>TraceLetter</c> (<see cref="TracingTuning"/>), from the
    /// learning rules asset.
    /// </summary>
    public readonly struct StrokeTolerance
    {
        /// <summary>
        /// Creates a tolerance; throws an <see cref="ArgumentOutOfRangeException"/> for a corridor or a start
        /// radius not above 0, or a coverage outside (0, 1]. Called by the learning rules asset and the tests.
        /// </summary>
        public StrokeTolerance(float corridor, float startRadius, float coverage)
        {
            if (!(corridor > 0f))
            {
                throw new ArgumentOutOfRangeException(nameof(corridor), corridor, "The corridor is wider than 0.");
            }

            if (!(startRadius > 0f))
            {
                throw new ArgumentOutOfRangeException(nameof(startRadius), startRadius, "The start radius is above 0.");
            }

            if (!(coverage > 0f) || coverage > 1f)
            {
                throw new ArgumentOutOfRangeException(nameof(coverage), coverage, "The coverage is a share above 0, at most 1.");
            }

            Corridor = corridor;
            StartRadius = startRadius;
            Coverage = coverage;
        }

        /// <summary>How far her line may be from the stroke, either side.</summary>
        public float Corridor { get; }

        /// <summary>How close to the start dot her first point must be.</summary>
        public float StartRadius { get; }

        /// <summary>The share of the stroke's length she must cover, in its direction.</summary>
        public float Coverage { get; }
    }
}
