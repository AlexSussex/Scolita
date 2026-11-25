using System;

namespace Scolita.Core.Tracing
{
    /// <summary>
    /// The points of the stroke she is drawing, in the shape's units: one buffer, made once and reused
    /// for every stroke, so drawing allocates nothing. A point closer than <see cref="Closest"/> to the
    /// last one is dropped, and once it is full later points are dropped too (a stroke of
    /// <see cref="Capacity"/> points is far longer than any letter).
    /// </summary>
    public sealed class TracePoints
    {
        /// <summary>How many points it holds.</summary>
        public const int Capacity = 2048;

        /// <summary>The smallest distance kept between two points, in the shape's units.</summary>
        public const float Closest = 0.5f;

        private readonly float[] xs = new float[Capacity];
        private readonly float[] ys = new float[Capacity];

        /// <summary>How many points it has.</summary>
        public int Count { get; private set; }

        /// <summary>The length of her line, in the shape's units.</summary>
        public float Length { get; private set; }

        /// <summary>Point <paramref name="index"/>'s x; called by <see cref="StrokeCheck"/> and the views.</summary>
        public float X(int index)
        {
            return xs[index];
        }

        /// <summary>Point <paramref name="index"/>'s y; called by <see cref="StrokeCheck"/> and the views.</summary>
        public float Y(int index)
        {
            return ys[index];
        }

        /// <summary>
        /// Adds the point (<paramref name="x"/>, <paramref name="y"/>); false when it was dropped (too close
        /// to the last one, or the buffer full). Called by the tracing view (<c>TraceLetterView</c>) and the tests.
        /// </summary>
        public bool Add(float x, float y)
        {
            if (Count == Capacity)
            {
                return false;
            }

            if (Count > 0)
            {
                float dx = x - xs[Count - 1];
                float dy = y - ys[Count - 1];
                float step = (float)Math.Sqrt(dx * dx + dy * dy);
                if (step < Closest)
                {
                    return false;
                }

                Length += step;
            }

            xs[Count] = x;
            ys[Count] = y;
            Count++;
            return true;
        }

        /// <summary>Forgets every point, for the next stroke; called by the tracing view (<c>TraceLetterView</c>) and the tests.</summary>
        public void Clear()
        {
            Count = 0;
            Length = 0f;
        }
    }
}
