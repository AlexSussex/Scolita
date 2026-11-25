using System;

namespace Scolita.Core.Tracing
{
    /// <summary>
    /// Judges one line she drew on a <see cref="TraceFigure"/>, from the stroke she is at (<c>docs/curriculum.md</c>
    /// §4.2, <c>TraceLetter</c> and <c>TraceWord</c>): pure, and allocating nothing. When strokes continue each other
    /// (a handwritten a's three strokes, the letters of a joined word), her line is followed along all of them in
    /// one go, and every stroke she covered enough of, from the first, is done at once; a line lifted part-way
    /// into the next keeps what it did; a line begun near a join (after a lift there) counts from the join.
    /// Otherwise, and when no stroke is done, the stroke she is at is judged by <see cref="StrokeCheck"/>, so print
    /// letters are judged exactly as before; a line drawn backwards through the whole chain is
    /// <see cref="StrokeVerdict.WrongDirection"/>.
    /// </summary>
    public static class LineCheck
    {
        /// <summary>
        /// The verdict on <paramref name="points"/> (her line, in the shape's units) drawn from stroke
        /// <paramref name="stroke"/> of <paramref name="figure"/>, with <paramref name="tolerance"/>. Called by the
        /// tracing rules and the tests.
        /// </summary>
        public static LineResult Check(TraceFigure figure, int stroke, TracePoints points, in StrokeTolerance tolerance)
        {
            int chain = figure.ChainCount(stroke);
            if (chain > 1 && points.Count > 0)
            {
                LineResult along = Along(figure, stroke, chain, points, tolerance);
                if (along.StrokesDone > 0)
                {
                    return along;
                }
            }

            StrokeResult own = StrokeCheck.Check(figure.Strokes[stroke].Path, points, tolerance);
            if (own.Verdict == StrokeVerdict.Good)
            {
                return new LineResult(StrokeVerdict.Good, 1, points.Count, false);
            }

            if (chain > 1 && own.Verdict != StrokeVerdict.SkippedEntry && own.Verdict != StrokeVerdict.WrongDirection
                && StrokeCheck.Check(figure.ChainFrom(stroke), points, tolerance).Verdict == StrokeVerdict.WrongDirection)
            {
                return new LineResult(StrokeVerdict.WrongDirection, 0, 0, false);
            }

            return new LineResult(own.Verdict, 0, 0, false);
        }

        private static LineResult Along(TraceFigure figure, int stroke, int chain, TracePoints points, in StrokeTolerance tolerance)
        {
            StrokeReach reach = StrokeCheck.Reach(figure.ChainFrom(stroke), points, tolerance);
            if (!reach.StartedRight)
            {
                return new LineResult(StrokeVerdict.TooShort, 0, 0, false);
            }

            int done = 0;
            float start = 0f;
            for (int within = 0; within < chain; within++)
            {
                float end = figure.ChainEnd(stroke, within);
                float length = end - start;
                float drawnFrom = within == 0 && figure.BeginsAtJoin(stroke) ? start : Math.Max(reach.From, start);
                if (reach.To - drawnFrom < StrokeCheck.EnoughOf(length, tolerance) * length)
                {
                    break;
                }

                done = within + 1;
                start = end;
            }

            if (done == 0)
            {
                return new LineResult(StrokeVerdict.TooShort, 0, 0, false);
            }

            bool wentOn = done < chain
                && (reach.To - figure.ChainEnd(stroke, done - 1) > tolerance.StartRadius * 0.5f || reach.PointsOnPath < points.Count);
            return new LineResult(StrokeVerdict.Good, done, reach.PointsOnPath, wentOn);
        }
    }
}
