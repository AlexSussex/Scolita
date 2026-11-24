using System;
using System.Linq;
using Scolita.Core.Tracing;
using UnityEngine;

namespace Scolita.Content
{
    /// <summary>
    /// A run of a stroke as stored in a <see cref="LetterShapeAsset"/>: its kind as a code and its points,
    /// upright, in the shape's units.
    /// </summary>
    [Serializable]
    public sealed class StrokeRunData
    {
        [SerializeField] private string kind;
        [SerializeField] private Vector2[] points;

        /// <summary>An empty run, for Unity's serialization.</summary>
        public StrokeRunData()
        {
        }

        /// <summary>The stored form of <paramref name="run"/>; called by <see cref="StrokeData"/>.</summary>
        public StrokeRunData(StrokeRun run)
        {
            kind = LetterShapeCodes.CodeOf(run.Kind);
            points = run.Points.Select(point => new Vector2(point.X, point.Y)).ToArray();
        }

        /// <summary>
        /// The run for Core, or null with a problem in <paramref name="fields"/> when it can't be read; called
        /// by <see cref="StrokeData"/>.
        /// </summary>
        public StrokeRun ToRun(AssetFields fields)
        {
            if (!LetterShapeCodes.TryParseRunKind(kind, out StrokeRunKind runKind))
            {
                fields.Problem("A run's kind '" + kind + "' is not body, entry or exit.");
                return null;
            }

            if (points == null || points.Length == 0)
            {
                fields.Problem("A run has no point.");
                return null;
            }

            return new StrokeRun(runKind, points.Select(point => new ShapePoint(point.x, point.y)).ToArray());
        }
    }
}
