using System;
using System.Collections.Generic;

namespace Scolita.Core.Tracing
{
    /// <summary>
    /// Joins a word's handwritten letters into one figure (<c>TraceWord</c>, design screen 8,
    /// <c>design/research/joined-words-draft.svg</c>): each letter after the first is moved right so it starts
    /// where the letter before left off, the join being part of it. A letter whose entry run starts near the height
    /// where the exit before ends (a after m) has the entry's first point moved onto that exit's end; a letter with
    /// no entry (m, n), or after an exit that leaves high (o's), gets a join run from the exit's end to its body, a
    /// small gap to the right. Points are placed upright and slanted after, so the word leans as one. Each letter's
    /// line comes letter by letter, then every mark (dots and bars) in letter order, as the manuals write "tati".
    /// </summary>
    public static class JoinedWord
    {
        /// <summary>How far right of the exit before a letter without an entry begins, upright, in a shape's units.</summary>
        public const float JoinGap = 6f;

        /// <summary>
        /// How far apart in height an exit's end and the next entry's start may be for the entry to be kept as the
        /// join (with its first point moved); farther (o leaving at the middle line), a join run replaces it.
        /// </summary>
        public const float EntryKeptWithin = 30f;

        /// <summary>
        /// The figure of the word whose letters have the handwritten lower-case <paramref name="shapes"/>, in order;
        /// throws an <see cref="ArgumentException"/> for no letter, a shape that isn't handwritten, shapes of different
        /// slants, or a letter before the last without an exit run. Called by the <c>TraceWord</c> rules and the tests.
        /// </summary>
        public static TraceFigure Of(IReadOnlyList<LetterShape> shapes)
        {
            if (shapes == null || shapes.Count == 0)
            {
                throw new ArgumentException("A word has at least one letter.", nameof(shapes));
            }

            List<Stroke> lines = new List<Stroke>();
            List<int> lineLetters = new List<int>();
            List<int> lineNumbers = new List<int>();
            List<Stroke> marks = new List<Stroke>();
            List<int> markLetters = new List<int>();
            List<int> markNumbers = new List<int>();
            ShapePoint join = default;
            for (int letter = 0; letter < shapes.Count; letter++)
            {
                LetterShape shape = shapes[letter];
                if (shape.Slant != shapes[0].Slant)
                {
                    throw new ArgumentException("A joined word leans as one: " + shape.Id + " has another slant.", nameof(shapes));
                }

                if (shape.Form != LetterForm.Handwritten)
                {
                    throw new ArgumentException("A joined word is written by hand: " + shape.Id + " is not.", nameof(shapes));
                }

                int lineEnd = TraceFigure.LineEndOf(shape);
                ShapePoint? exitEnd = ExitEndOf(shape.Strokes[lineEnd]);
                if (exitEnd == null && letter + 1 < shapes.Count)
                {
                    throw new ArgumentException("The letter " + shape.Id + " has no exit run to join the next letter to.",
                        nameof(shapes));
                }

                float offset = letter == 0 ? 0f : OffsetAfter(join, shape, lineEnd);
                for (int stroke = 0; stroke < shape.Strokes.Count; stroke++)
                {
                    bool joined = letter > 0 && stroke == 0;
                    Stroke placed = new Stroke(Placed(shape.Strokes[stroke].Runs, offset, joined, join), shape.Slant);
                    if (stroke <= lineEnd)
                    {
                        lines.Add(placed);
                        lineLetters.Add(letter);
                        lineNumbers.Add(stroke);
                    }
                    else
                    {
                        marks.Add(placed);
                        markLetters.Add(letter);
                        markNumbers.Add(stroke);
                    }
                }

                if (exitEnd != null)
                {
                    join = new ShapePoint(exitEnd.Value.X + offset, exitEnd.Value.Y);
                }
            }

            List<bool> isMark = new List<bool>();
            for (int index = 0; index < lines.Count; index++)
            {
                isMark.Add(false);
            }

            for (int index = 0; index < marks.Count; index++)
            {
                isMark.Add(true);
            }

            lines.AddRange(marks);
            lineLetters.AddRange(markLetters);
            lineNumbers.AddRange(markNumbers);
            return new TraceFigure(LetterForm.Handwritten, shapes[0].Slant, lines, lineLetters, lineNumbers, isMark);
        }

        private static float OffsetAfter(ShapePoint join, LetterShape shape, int lineEnd)
        {
            StrokeRun first = shape.Strokes[0].Runs[0];
            if (KeepsEntry(first, join))
            {
                return join.X - first.Points[0].X;
            }

            float bodyLeft = float.MaxValue;
            for (int stroke = 0; stroke <= lineEnd; stroke++)
            {
                IReadOnlyList<StrokeRun> runs = shape.Strokes[stroke].Runs;
                for (int run = 0; run < runs.Count; run++)
                {
                    if (stroke == 0 && run == 0 && runs[run].Kind == StrokeRunKind.Entry)
                    {
                        continue;
                    }

                    foreach (ShapePoint point in runs[run].Points)
                    {
                        bodyLeft = Math.Min(bodyLeft, point.X);
                    }
                }
            }

            return join.X + JoinGap - bodyLeft;
        }

        private static bool KeepsEntry(StrokeRun first, ShapePoint join)
        {
            return first.Kind == StrokeRunKind.Entry && Math.Abs(first.Points[0].Y - join.Y) <= EntryKeptWithin;
        }

        private static IReadOnlyList<StrokeRun> Placed(IReadOnlyList<StrokeRun> runs, float offset, bool joined, ShapePoint join)
        {
            List<StrokeRun> placed = new List<StrokeRun>();
            for (int index = 0; index < runs.Count; index++)
            {
                StrokeRun run = runs[index];
                if (!joined || index > 0)
                {
                    placed.Add(new StrokeRun(run.Kind, Shifted(run.Points, offset, 0)));
                }
                else if (KeepsEntry(run, join))
                {
                    List<ShapePoint> points = Shifted(run.Points, offset, 1);
                    points.Insert(0, join);
                    placed.Add(new StrokeRun(StrokeRunKind.Entry, points));
                }
                else
                {
                    ShapePoint bodyStart = run.Kind == StrokeRunKind.Entry ? run.Points[run.Points.Count - 1] : run.Points[0];
                    placed.Add(new StrokeRun(StrokeRunKind.Entry,
                        new[] { join, new ShapePoint(bodyStart.X + offset, bodyStart.Y) }));
                    if (run.Kind != StrokeRunKind.Entry)
                    {
                        placed.Add(new StrokeRun(run.Kind, Shifted(run.Points, offset, 0)));
                    }
                }
            }

            return placed;
        }

        private static List<ShapePoint> Shifted(IReadOnlyList<ShapePoint> points, float offset, int from)
        {
            List<ShapePoint> shifted = new List<ShapePoint>(points.Count);
            for (int index = from; index < points.Count; index++)
            {
                shifted.Add(new ShapePoint(points[index].X + offset, points[index].Y));
            }

            return shifted;
        }

        private static ShapePoint? ExitEndOf(Stroke stroke)
        {
            StrokeRun last = stroke.Runs[stroke.Runs.Count - 1];
            return last.Kind == StrokeRunKind.Exit ? last.Points[last.Points.Count - 1] : (ShapePoint?)null;
        }
    }
}
