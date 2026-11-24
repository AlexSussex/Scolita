using System;
using System.Collections.Generic;

namespace Scolita.Core.Tracing
{
    /// <summary>
    /// One run of a stroke: a smooth line through its points, drawn without lifting, meeting the next
    /// run at a corner (m's turn at the baseline). A run is the letter itself, or a handwritten
    /// letter's entry or exit (<see cref="StrokeRunKind"/>).
    /// </summary>
    public sealed class StrokeRun
    {
        /// <summary>
        /// Creates a run of <paramref name="points"/> (at least one); throws an <see cref="ArgumentException"/>
        /// for none. Called by <see cref="LetterShapeReader"/>, the shape asset and the tests.
        /// </summary>
        public StrokeRun(StrokeRunKind kind, IReadOnlyList<ShapePoint> points)
        {
            if (points == null || points.Count == 0)
            {
                throw new ArgumentException("A run has at least one point.", nameof(points));
            }

            Kind = kind;
            Points = points;
        }

        /// <summary>What the run is.</summary>
        public StrokeRunKind Kind { get; }

        /// <summary>Its points, upright (before the shape's slant), in the order it is drawn.</summary>
        public IReadOnlyList<ShapePoint> Points { get; }
    }
}
