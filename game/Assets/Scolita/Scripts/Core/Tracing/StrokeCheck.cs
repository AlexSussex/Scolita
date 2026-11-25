using System;

namespace Scolita.Core.Tracing
{
    /// <summary>
    /// Judges one stroke she drew against the stroke of the letter (<c>docs/curriculum.md</c> §4.2,
    /// <c>TraceLetter</c>): pure, and allocating nothing. It follows her line in order along the stroke:
    /// each of her points moves a "reached" mark forward to the nearest sample of the stroke within the
    /// corridor, never back and never further ahead than she moved, so a stroke that goes back over
    /// itself (m's stem down and up again) is reached in order, and an o drawn the wrong way round is
    /// told from one drawn the right way. Her line has left the path where it is farther than the corridor
    /// from the part of the stroke she has reached (not from any part: a line cutting across m to its last
    /// stem has left it). The same, followed from the stroke's end, tells a stroke drawn backwards.
    /// </summary>
    public static class StrokeCheck
    {
        /// <summary>
        /// The verdict on <paramref name="points"/> (her line, in the shape's units) for
        /// <paramref name="path"/> with <paramref name="tolerance"/>: a dot is <see cref="StrokeVerdict.Good"/>
        /// when touched near it; otherwise a tap is <see cref="StrokeVerdict.TooShort"/>; a line from the start
        /// dot, along the stroke, staying on it and covering enough of it is <see cref="StrokeVerdict.Good"/>;
        /// one begun at the stroke's end (for a round stroke, also its start) that covers enough of it backwards
        /// is <see cref="StrokeVerdict.WrongDirection"/>. "Enough" is the part she drew herself, from where her
        /// first point reached the stroke, and it is the coverage share of the stroke or all of it but half the
        /// start radius at its end, whichever is less (so a short bar isn't stricter than a long stroke). One
        /// begun away from the start dot is <see cref="StrokeVerdict.StartedElsewhere"/>; one that strays from
        /// the stroke for more than the corridor's width is <see cref="StrokeVerdict.LeftThePath"/>; and the
        /// rest (begun right, stayed on, stopped early) is <see cref="StrokeVerdict.TooShort"/>. A line begun where
        /// the stroke's entry run ends (the body of a handwritten letter) is <see cref="StrokeVerdict.SkippedEntry"/>,
        /// whichever way it goes. Called by <see cref="LineCheck"/> and the tests.
        /// </summary>
        public static StrokeResult Check(StrokePath path, TracePoints points, in StrokeTolerance tolerance)
        {
            if (points.Count == 0)
            {
                return new StrokeResult(StrokeVerdict.TooShort, 0f);
            }

            bool startedRight = DistanceSquared(points.X(0), points.Y(0), path.Start) <= tolerance.StartRadius * tolerance.StartRadius;
            if (path.IsDot)
            {
                return startedRight
                    ? new StrokeResult(StrokeVerdict.Good, 1f)
                    : new StrokeResult(StrokeVerdict.StartedElsewhere, 0f);
            }

            if (points.Length < Math.Min(tolerance.StartRadius, path.Length * 0.5f))
            {
                return new StrokeResult(StrokeVerdict.TooShort, 0f);
            }

            float enough = EnoughShare(path.Length, tolerance);
            float forwards = Followed(path, points, tolerance.Corridor, true, out float offLength);
            bool stayedOn = offLength <= tolerance.Corridor;
            if (forwards >= enough && startedRight && stayedOn)
            {
                return new StrokeResult(StrokeVerdict.Good, forwards);
            }

            if (!startedRight && path.EntryLength > 0f
                && DistanceSquared(points.X(0), points.Y(0), path.PointAt(path.EntryLength)) <= tolerance.StartRadius * tolerance.StartRadius)
            {
                return new StrokeResult(StrokeVerdict.SkippedEntry, forwards);
            }

            bool startedAtTheEnd = DistanceSquared(points.X(0), points.Y(0), path.End) <= tolerance.StartRadius * tolerance.StartRadius;
            float backwards = startedAtTheEnd ? Followed(path, points, tolerance.Corridor, false, out _) : 0f;
            if (backwards >= enough && backwards > forwards)
            {
                return new StrokeResult(StrokeVerdict.WrongDirection, forwards);
            }

            if (!startedRight)
            {
                return new StrokeResult(StrokeVerdict.StartedElsewhere, forwards);
            }

            return new StrokeResult(stayedOn ? StrokeVerdict.TooShort : StrokeVerdict.LeftThePath, forwards);
        }

        /// <summary>
        /// How far along <paramref name="path"/> her line <paramref name="points"/> went in its direction, followed as
        /// <see cref="Check"/> follows it, with <paramref name="tolerance"/>: where she first reached it, how far she
        /// had reached before she strayed from it (for longer than the corridor's width), and whether she began at
        /// its start. Pure and allocating nothing; called by <see cref="LineCheck"/> (a line through several strokes)
        /// and the tests.
        /// </summary>
        public static StrokeReach Reach(StrokePath path, TracePoints points, in StrokeTolerance tolerance)
        {
            if (points.Count == 0 || path.IsDot)
            {
                return new StrokeReach(0f, 0f, false, points.Count);
            }

            bool startedRight = DistanceSquared(points.X(0), points.Y(0), path.Start) <= tolerance.StartRadius * tolerance.StartRadius;
            Follow(path, points, tolerance.Corridor, true, out int firstReached, out int reachedOn, out int pointsOn, out _);
            return new StrokeReach(path.DistanceAt(firstReached), path.DistanceAt(reachedOn), startedRight, pointsOn);
        }

        /// <summary>
        /// How much of a stroke <paramref name="length"/> long is enough to be covered with <paramref name="tolerance"/>:
        /// the coverage share, or all of it but half the start radius at its end, whichever is less, never under half.
        /// Called by <see cref="Check"/> and <see cref="LineCheck"/>.
        /// </summary>
        public static float EnoughOf(float length, in StrokeTolerance tolerance)
        {
            return EnoughShare(length, tolerance);
        }

        private static float EnoughShare(float length, in StrokeTolerance tolerance)
        {
            float endAllowance = length - tolerance.StartRadius * 0.5f;
            return Math.Max(0.5f, Math.Min(tolerance.Coverage, endAllowance / length));
        }

        private static float Followed(StrokePath path, TracePoints points, float corridor, bool forwards, out float offLength)
        {
            int reached = Follow(path, points, corridor, forwards, out int firstReached, out _, out _, out offLength);
            return (Along(path, reached, forwards) - Along(path, firstReached, forwards)) / path.Length;
        }

        private static int Follow(StrokePath path, TracePoints points, float corridor, bool forwards, out int firstReached,
            out int reachedOn, out int pointsOn, out float offLength)
        {
            int last = path.Count - 1;
            float step = path.Length / last;
            float corridorSquared = corridor * corridor;
            int behind = (int)Math.Ceiling(2f * corridor / step);
            int reached = 0;
            reachedOn = 0;
            firstReached = 0;
            pointsOn = 0;
            offLength = 0f;
            for (int point = 0; point < points.Count; point++)
            {
                float x = points.X(point);
                float y = points.Y(point);
                float moved = point == 0 ? 0f : Distance(points.X(point - 1), points.Y(point - 1), x, y);
                int ahead = Math.Min(last, reached + (int)Math.Ceiling((moved + corridor) / step));
                int nearest = Nearest(path, x, y, reached, ahead, forwards, corridorSquared);
                if (nearest > reached)
                {
                    reached = nearest;
                }

                if (point == 0)
                {
                    firstReached = reached;
                }

                if (nearest < 0 && Nearest(path, x, y, Math.Max(0, reached - behind), reached, forwards, corridorSquared) < 0)
                {
                    offLength += moved;
                }

                if (offLength <= corridor)
                {
                    reachedOn = reached;
                    pointsOn = point + 1;
                }
            }

            return reached;
        }

        private static float Along(StrokePath path, int reached, bool forwards)
        {
            return forwards ? path.DistanceAt(reached) : path.Length - path.DistanceAt(path.Count - 1 - reached);
        }

        private static int Nearest(StrokePath path, float x, float y, int from, int to, bool forwards, float corridorSquared)
        {
            int last = path.Count - 1;
            int nearest = -1;
            float nearestSquared = corridorSquared;
            for (int along = from; along <= to; along++)
            {
                int sample = forwards ? along : last - along;
                float squared = DistanceSquared(x, y, path.X(sample), path.Y(sample));
                if (squared < nearestSquared)
                {
                    nearestSquared = squared;
                    nearest = along;
                }
            }

            return nearest;
        }

        private static float DistanceSquared(float x, float y, ShapePoint point)
        {
            return DistanceSquared(x, y, point.X, point.Y);
        }

        private static float DistanceSquared(float x1, float y1, float x2, float y2)
        {
            float dx = x2 - x1;
            float dy = y2 - y1;
            return dx * dx + dy * dy;
        }

        private static float Distance(float x1, float y1, float x2, float y2)
        {
            return (float)Math.Sqrt(DistanceSquared(x1, y1, x2, y2));
        }
    }
}
