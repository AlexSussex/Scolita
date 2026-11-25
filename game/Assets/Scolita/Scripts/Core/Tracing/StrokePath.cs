using System;
using System.Collections.Generic;

namespace Scolita.Core.Tracing
{
    /// <summary>
    /// A stroke as she must draw it: its runs made into smooth curves (Catmull-Rom through their points,
    /// a corner between two runs), slanted, and sampled every <see cref="Spacing"/> units along it, with
    /// each sample's distance from the start. Made once when the content loads, so checking a stroke and
    /// drawing it only read arrays. A stroke of one point is a dot. A handwritten stroke that begins with an
    /// entry run knows where along it the entry ends (<see cref="EntryLength"/>), so a skipped entry can be told.
    /// </summary>
    public sealed class StrokePath
    {
        /// <summary>The distance between two samples, in the shape's units (a small letter is 100 high).</summary>
        public const float Spacing = 2f;

        private const float CurveStep = 0.5f;

        private readonly float[] xs;
        private readonly float[] ys;
        private readonly float[] distances;

        private StrokePath(float[] xs, float[] ys, float[] distances, float entryLength)
        {
            this.xs = xs;
            this.ys = ys;
            this.distances = distances;
            EntryLength = entryLength;
        }

        /// <summary>How many samples it has (1 for a dot).</summary>
        public int Count => xs.Length;

        /// <summary>Its length, in the shape's units (0 for a dot).</summary>
        public float Length => distances[distances.Length - 1];

        /// <summary>
        /// How far along it its entry run ends (where the letter's body begins), in the shape's units; 0 for a
        /// stroke that doesn't begin with an entry run.
        /// </summary>
        public float EntryLength { get; }

        /// <summary>Whether it is a dot (one point).</summary>
        public bool IsDot => xs.Length == 1;

        /// <summary>The start: where the start dot is.</summary>
        public ShapePoint Start => new ShapePoint(xs[0], ys[0]);

        /// <summary>The end.</summary>
        public ShapePoint End => new ShapePoint(xs[xs.Length - 1], ys[ys.Length - 1]);

        /// <summary>
        /// Makes the path of <paramref name="runs"/> leaned by <paramref name="slant"/>
        /// (x += (baseline - y) * slant); throws an <see cref="ArgumentException"/> for no run. Called by
        /// <see cref="Stroke"/> and the tests.
        /// </summary>
        public static StrokePath Of(IReadOnlyList<StrokeRun> runs, float slant)
        {
            if (runs == null || runs.Count == 0)
            {
                throw new ArgumentException("A stroke has at least one run.", nameof(runs));
            }

            List<ShapePoint> dense = new List<ShapePoint>();
            int entryEnd = -1;
            for (int index = 0; index < runs.Count; index++)
            {
                AddCurve(runs[index].Points, slant, dense);
                if (runs[index].Kind == StrokeRunKind.Entry && index == 0)
                {
                    entryEnd = dense.Count - 1;
                }
            }

            return Resampled(dense, entryEnd < 0 ? 0f : LengthTo(dense, entryEnd));
        }

        /// <summary>
        /// The path along <paramref name="paths"/> one after the other, each beginning where the one before ends
        /// (strokes that continue each other, written in one line); its entry is the first path's. Throws an
        /// <see cref="ArgumentException"/> for no path. Called by <see cref="TraceFigure"/> and the tests.
        /// </summary>
        public static StrokePath Chain(IReadOnlyList<StrokePath> paths)
        {
            if (paths == null || paths.Count == 0)
            {
                throw new ArgumentException("A chain has at least one path.", nameof(paths));
            }

            List<ShapePoint> dense = new List<ShapePoint>();
            foreach (StrokePath path in paths)
            {
                for (int index = 0; index < path.Count; index++)
                {
                    Add(new ShapePoint(path.xs[index], path.ys[index]), dense);
                }
            }

            return Resampled(dense, paths[0].EntryLength);
        }

        /// <summary>Sample <paramref name="index"/>'s x; called by <see cref="StrokeCheck"/> and the views.</summary>
        public float X(int index)
        {
            return xs[index];
        }

        /// <summary>Sample <paramref name="index"/>'s y; called by <see cref="StrokeCheck"/> and the views.</summary>
        public float Y(int index)
        {
            return ys[index];
        }

        /// <summary>Sample <paramref name="index"/>'s distance from the start; called by the views (the rainbow, the arrow).</summary>
        public float DistanceAt(int index)
        {
            return distances[index];
        }

        /// <summary>
        /// The point <paramref name="distance"/> along it (clamped to its ends), allocating nothing. Called
        /// by the views (the stroke drawing itself, the arrow) and the tests.
        /// </summary>
        public ShapePoint PointAt(float distance)
        {
            if (xs.Length == 1 || distance <= 0f)
            {
                return Start;
            }

            if (distance >= Length)
            {
                return End;
            }

            int index = Math.Min((int)(distance / Spacing), xs.Length - 2);
            while (index > 0 && distances[index] > distance)
            {
                index--;
            }

            while (index < xs.Length - 2 && distances[index + 1] < distance)
            {
                index++;
            }

            float span = distances[index + 1] - distances[index];
            float share = span > 0f ? (distance - distances[index]) / span : 0f;
            return new ShapePoint(xs[index] + (xs[index + 1] - xs[index]) * share, ys[index] + (ys[index + 1] - ys[index]) * share);
        }

        private static void AddCurve(IReadOnlyList<ShapePoint> points, float slant, List<ShapePoint> dense)
        {
            if (points.Count == 1)
            {
                Add(Slanted(points[0], slant), dense);
                return;
            }

            for (int segment = 0; segment < points.Count - 1; segment++)
            {
                ShapePoint from = Slanted(points[segment], slant);
                ShapePoint to = Slanted(points[segment + 1], slant);
                ShapePoint before = segment > 0 ? Slanted(points[segment - 1], slant) : Mirrored(to, from);
                ShapePoint after = segment + 2 < points.Count ? Slanted(points[segment + 2], slant) : Mirrored(from, to);
                int pieces = Math.Max(1, (int)Math.Ceiling(Distance(from, to) / CurveStep));
                for (int piece = 0; piece <= pieces; piece++)
                {
                    Add(CatmullRom(before, from, to, after, piece / (float)pieces), dense);
                }
            }
        }

        private static void Add(ShapePoint point, List<ShapePoint> dense)
        {
            if (dense.Count == 0 || Distance(dense[dense.Count - 1], point) > 0.0001f)
            {
                dense.Add(point);
            }
        }

        private static float LengthTo(List<ShapePoint> dense, int end)
        {
            float length = 0f;
            for (int index = 1; index <= end; index++)
            {
                length += Distance(dense[index - 1], dense[index]);
            }

            return length;
        }

        private static StrokePath Resampled(List<ShapePoint> dense, float entryLength)
        {
            if (dense.Count == 1)
            {
                return new StrokePath(new[] { dense[0].X }, new[] { dense[0].Y }, new[] { 0f }, 0f);
            }

            float total = 0f;
            for (int index = 1; index < dense.Count; index++)
            {
                total += Distance(dense[index - 1], dense[index]);
            }

            int count = Math.Max(2, (int)Math.Ceiling(total / Spacing) + 1);
            float step = total / (count - 1);
            float[] xs = new float[count];
            float[] ys = new float[count];
            float[] distances = new float[count];
            int along = 0;
            float before = 0f;
            for (int sample = 0; sample < count; sample++)
            {
                float wanted = sample == count - 1 ? total : sample * step;
                while (along < dense.Count - 2 && before + Distance(dense[along], dense[along + 1]) < wanted)
                {
                    before += Distance(dense[along], dense[along + 1]);
                    along++;
                }

                float span = Distance(dense[along], dense[along + 1]);
                float share = span > 0f ? Math.Min(1f, Math.Max(0f, (wanted - before) / span)) : 0f;
                xs[sample] = dense[along].X + (dense[along + 1].X - dense[along].X) * share;
                ys[sample] = dense[along].Y + (dense[along + 1].Y - dense[along].Y) * share;
                distances[sample] = wanted;
            }

            return new StrokePath(xs, ys, distances, Math.Min(entryLength, total));
        }

        private static ShapePoint CatmullRom(ShapePoint before, ShapePoint from, ShapePoint to, ShapePoint after, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;
            float x = 0.5f * (2f * from.X + (to.X - before.X) * t + (2f * before.X - 5f * from.X + 4f * to.X - after.X) * t2
                + (3f * from.X - before.X - 3f * to.X + after.X) * t3);
            float y = 0.5f * (2f * from.Y + (to.Y - before.Y) * t + (2f * before.Y - 5f * from.Y + 4f * to.Y - after.Y) * t2
                + (3f * from.Y - before.Y - 3f * to.Y + after.Y) * t3);
            return new ShapePoint(x, y);
        }

        private static ShapePoint Mirrored(ShapePoint other, ShapePoint end)
        {
            return new ShapePoint(2f * end.X - other.X, 2f * end.Y - other.Y);
        }

        private static ShapePoint Slanted(ShapePoint point, float slant)
        {
            return new ShapePoint(point.X + (WritingLines.Baseline - point.Y) * slant, point.Y);
        }

        private static float Distance(ShapePoint a, ShapePoint b)
        {
            float dx = b.X - a.X;
            float dy = b.Y - a.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
