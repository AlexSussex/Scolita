using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Tracing;
using UnityEngine;

namespace Scolita.Content
{
    /// <summary>A stroke as stored in a <see cref="LetterShapeAsset"/>: its runs, in order.</summary>
    [Serializable]
    public sealed class StrokeData
    {
        [SerializeField] private StrokeRunData[] runs;

        /// <summary>An empty stroke, for Unity's serialization.</summary>
        public StrokeData()
        {
        }

        /// <summary>The stored form of <paramref name="stroke"/>; called by <see cref="LetterShapeAsset.Fill"/>.</summary>
        public StrokeData(Stroke stroke)
        {
            runs = stroke.Runs.Select(run => new StrokeRunData(run)).ToArray();
        }

        /// <summary>
        /// The stroke for Core, leaned by <paramref name="slant"/>, or null with a problem in
        /// <paramref name="fields"/> when it can't be read; called by <see cref="LetterShapeAsset"/>.
        /// </summary>
        public Stroke ToStroke(float slant, AssetFields fields)
        {
            if (runs == null || runs.Length == 0)
            {
                fields.Problem("A stroke has no run.");
                return null;
            }

            List<StrokeRun> read = runs.Select(run => run?.ToRun(fields)).ToList();
            return read.Any(run => run == null) ? null : new Stroke(read, slant);
        }
    }
}
