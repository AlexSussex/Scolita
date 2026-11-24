using System;
using Scolita.Core.Tracing;

namespace Scolita.Tests.EditMode.Fakes
{
    /// <summary>
    /// Strokes and drawn lines made in code for the tracing tests: a stroke through given points, and her
    /// line along a stroke (part of it, forwards or backwards, beside it). Used by the tracing and
    /// <c>TraceLetter</c> tests.
    /// </summary>
    public static class TestStrokes
    {
        /// <summary>
        /// A stroke of one run through the points <paramref name="xy"/> (x, y, x, y, …), upright; called by the
        /// tracing tests.
        /// </summary>
        public static StrokePath Path(params float[] xy)
        {
            return new Stroke(new[] { new StrokeRun(StrokeRunKind.Body, Points(xy)) }, 0f).Path;
        }

        /// <summary>
        /// A stroke of one run per point list in <paramref name="runs"/>, each run's points x, y, x, y, …;
        /// called by the tracing tests.
        /// </summary>
        public static StrokePath Runs(params float[][] runs)
        {
            StrokeRun[] made = new StrokeRun[runs.Length];
            for (int index = 0; index < runs.Length; index++)
            {
                made[index] = new StrokeRun(StrokeRunKind.Body, Points(runs[index]));
            }

            return new Stroke(made, 0f).Path;
        }

        /// <summary>Her line through the points <paramref name="xy"/> (x, y, x, y, …); called by the tracing tests.</summary>
        public static TracePoints Drawn(params float[] xy)
        {
            TracePoints points = new TracePoints();
            for (int index = 0; index + 1 < xy.Length; index += 2)
            {
                points.Add(xy[index], xy[index + 1]);
            }

            return points;
        }

        /// <summary>
        /// Her line along <paramref name="path"/> from the share <paramref name="from"/> of its length to the
        /// share <paramref name="to"/> (from above to to: backwards), a point every <paramref name="step"/>
        /// units, <paramref name="beside"/> units to its left; called by the tracing and <c>TraceLetter</c> tests.
        /// </summary>
        public static TracePoints Along(StrokePath path, float from, float to, float step, float beside)
        {
            TracePoints points = new TracePoints();
            AddAlong(points, path, from, to, step, beside);
            return points;
        }

        /// <summary>
        /// Adds her line along <paramref name="path"/> to <paramref name="points"/>, as <see cref="Along"/>
        /// makes it; called by the tracing and <c>TraceLetter</c> tests.
        /// </summary>
        public static void AddAlong(TracePoints points, StrokePath path, float from, float to, float step, float beside)
        {
            float start = from * path.Length;
            float end = to * path.Length;
            int count = Math.Max(1, (int)Math.Ceiling(Math.Abs(end - start) / step));
            for (int index = 0; index <= count; index++)
            {
                float distance = start + (end - start) * index / count;
                ShapePoint at = path.PointAt(distance);
                ShapePoint ahead = path.PointAt(distance + 1f);
                ShapePoint behind = path.PointAt(distance - 1f);
                float dx = ahead.X - behind.X;
                float dy = ahead.Y - behind.Y;
                float length = (float)Math.Sqrt(dx * dx + dy * dy);
                float nx = length > 0f ? dy / length : 0f;
                float ny = length > 0f ? -dx / length : 0f;
                points.Add(at.X + nx * beside, at.Y + ny * beside);
            }
        }

        private static ShapePoint[] Points(float[] xy)
        {
            ShapePoint[] points = new ShapePoint[xy.Length / 2];
            for (int index = 0; index < points.Length; index++)
            {
                points[index] = new ShapePoint(xy[index * 2], xy[index * 2 + 1]);
            }

            return points;
        }
    }
}
