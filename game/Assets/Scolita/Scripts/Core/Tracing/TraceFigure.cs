using System;
using System.Collections.Generic;

namespace Scolita.Core.Tracing
{
    /// <summary>
    /// What she traces on the writing board: one letter's shape (<see cref="OfLetter"/>, print or handwritten) or a
    /// word's handwritten letters joined (<see cref="JoinedWord.Of"/>). Its strokes are in the order she writes them;
    /// each knows its letter, its number within the letter, whether it continues the stroke before (it begins where
    /// that one ended, so both may be written in one line), whether it ends its letter's line, and whether it is a
    /// mark (a dot or a bar, written after the word). Made once when a step is built, so judging and drawing only
    /// read it. The board, <see cref="LineCheck"/> and the tracing rules see only figures.
    /// </summary>
    public sealed class TraceFigure
    {
        /// <summary>How close a stroke's start must be to the end of the stroke before for it to continue it.</summary>
        public const float ContinuesWithin = 0.5f;

        private readonly Stroke[] strokes;
        private readonly int[] letterOf;
        private readonly int[] numberInLetter;
        private readonly bool[] marks;
        private readonly bool[] continues;
        private readonly bool[] endsLine;
        private readonly StrokePath[] chains;
        private readonly float[][] chainEnds;

        /// <summary>
        /// Creates a figure of <paramref name="strokes"/> with what each one is; throws an
        /// <see cref="ArgumentException"/> for no stroke or lists of different lengths. Called by
        /// <see cref="OfLetter"/> and <see cref="JoinedWord"/>.
        /// </summary>
        internal TraceFigure(LetterForm form, float slant, IReadOnlyList<Stroke> strokes, IReadOnlyList<int> letterOf,
            IReadOnlyList<int> numberInLetter, IReadOnlyList<bool> marks)
        {
            if (strokes == null || strokes.Count == 0)
            {
                throw new ArgumentException("A figure has at least one stroke.", nameof(strokes));
            }

            if (letterOf.Count != strokes.Count || numberInLetter.Count != strokes.Count || marks.Count != strokes.Count)
            {
                throw new ArgumentException("A figure says what each of its strokes is.", nameof(letterOf));
            }

            Form = form;
            Slant = slant;
            int count = strokes.Count;
            this.strokes = new Stroke[count];
            this.letterOf = new int[count];
            this.numberInLetter = new int[count];
            this.marks = new bool[count];
            continues = new bool[count];
            endsLine = new bool[count];
            MinX = float.MaxValue;
            MaxX = float.MinValue;
            for (int index = 0; index < count; index++)
            {
                this.strokes[index] = strokes[index];
                this.letterOf[index] = letterOf[index];
                this.numberInLetter[index] = numberInLetter[index];
                this.marks[index] = marks[index];
                LetterCount = Math.Max(LetterCount, letterOf[index] + 1);
                continues[index] = index > 0 && Meets(strokes[index - 1].Path, strokes[index].Path);
                StrokePath path = strokes[index].Path;
                for (int sample = 0; sample < path.Count; sample++)
                {
                    MinX = Math.Min(MinX, path.X(sample));
                    MaxX = Math.Max(MaxX, path.X(sample));
                }
            }

            for (int index = 0; index < count; index++)
            {
                endsLine[index] = !marks[index]
                    && (index + 1 == count || marks[index + 1] || letterOf[index + 1] != letterOf[index]);
            }

            chains = new StrokePath[count];
            chainEnds = new float[count][];
            for (int index = 0; index < count; index++)
            {
                MakeChain(index);
            }
        }

        /// <summary>Its form: print, or handwritten (the board's "tip I" lines).</summary>
        public LetterForm Form { get; }

        /// <summary>How much it leans: x += (baseline - y) * slant (the board's slant guides follow it).</summary>
        public float Slant { get; }

        /// <summary>Its strokes, in the order she writes them.</summary>
        public IReadOnlyList<Stroke> Strokes => strokes;

        /// <summary>How many letters it has (1 for a letter).</summary>
        public int LetterCount { get; }

        /// <summary>Its leftmost x, slanted (the board centres the figure with it).</summary>
        public float MinX { get; }

        /// <summary>Its rightmost x, slanted.</summary>
        public float MaxX { get; }

        /// <summary>
        /// Whether its first stroke begins on the baseline (a handwritten letter with an entry stroke: „Linia de jos:
        /// de aici pornești.”), not at the middle line as m and n do.
        /// </summary>
        public bool StartsOnTheBaseline => strokes[0].Path.EntryLength > 0f;

        /// <summary>
        /// The figure of one letter's <paramref name="shape"/>: its strokes in the shape's order, the strokes after
        /// the one holding the exit run being its marks. Called by the tracing rules, the <c>MeetLetter</c> rules and
        /// the tests.
        /// </summary>
        public static TraceFigure OfLetter(LetterShape shape)
        {
            if (shape == null)
            {
                throw new ArgumentNullException(nameof(shape));
            }

            int count = shape.Strokes.Count;
            int lineEnd = LineEnd(shape);
            int[] letters = new int[count];
            int[] numbers = new int[count];
            bool[] marks = new bool[count];
            for (int index = 0; index < count; index++)
            {
                numbers[index] = index;
                marks[index] = index > lineEnd;
            }

            return new TraceFigure(shape.Form, shape.Slant, shape.Strokes, letters, numbers, marks);
        }

        /// <summary>
        /// The index of the stroke of <paramref name="shape"/> that ends its line: the one holding its exit run, or its
        /// last stroke when it has none. Called by <see cref="OfLetter"/>, <see cref="JoinedWord"/> and the shape rules.
        /// </summary>
        public static int LineEndOf(LetterShape shape)
        {
            return LineEnd(shape);
        }

        private static int LineEnd(LetterShape shape)
        {
            for (int index = 0; index < shape.Strokes.Count; index++)
            {
                foreach (StrokeRun run in shape.Strokes[index].Runs)
                {
                    if (run.Kind == StrokeRunKind.Exit)
                    {
                        return index;
                    }
                }
            }

            return shape.Strokes.Count - 1;
        }

        /// <summary>Which letter stroke <paramref name="stroke"/> belongs to, from 0.</summary>
        public int LetterOf(int stroke)
        {
            return letterOf[stroke];
        }

        /// <summary>Stroke <paramref name="stroke"/>'s number within its letter, from 0 (its start dot shows it plus one).</summary>
        public int NumberInLetter(int stroke)
        {
            return numberInLetter[stroke];
        }

        /// <summary>Whether stroke <paramref name="stroke"/> is a mark (a dot or a bar), written after the word's line.</summary>
        public bool IsMark(int stroke)
        {
            return marks[stroke];
        }

        /// <summary>Whether stroke <paramref name="stroke"/> begins where the stroke before it ended.</summary>
        public bool Continues(int stroke)
        {
            return continues[stroke];
        }

        /// <summary>Whether stroke <paramref name="stroke"/> is the last of its letter's line (the letter's checkpoint).</summary>
        public bool EndsLine(int stroke)
        {
            return endsLine[stroke];
        }

        /// <summary>
        /// Whether stroke <paramref name="stroke"/> begins at a join: it is the first stroke of a letter after the first,
        /// starting where the letter before left off.
        /// </summary>
        public bool BeginsAtJoin(int stroke)
        {
            return stroke > 0 && !marks[stroke] && letterOf[stroke] > 0 && numberInLetter[stroke] == 0;
        }

        /// <summary>
        /// The path from the start of stroke <paramref name="stroke"/> through every stroke that continues it, in one
        /// line (the stroke's own path when none does). Called by <see cref="LineCheck"/>.
        /// </summary>
        public StrokePath ChainFrom(int stroke)
        {
            return chains[stroke];
        }

        /// <summary>How many strokes the chain from stroke <paramref name="stroke"/> holds (1 when none continues it).</summary>
        public int ChainCount(int stroke)
        {
            return chainEnds[stroke].Length;
        }

        /// <summary>
        /// How far along the chain from stroke <paramref name="stroke"/> its stroke number <paramref name="within"/>
        /// (from 0) ends. Called by <see cref="LineCheck"/>.
        /// </summary>
        public float ChainEnd(int stroke, int within)
        {
            return chainEnds[stroke][within];
        }

        private void MakeChain(int first)
        {
            int last = first;
            while (last + 1 < strokes.Length && continues[last + 1])
            {
                last++;
            }

            List<StrokePath> paths = new List<StrokePath>();
            float[] ends = new float[last - first + 1];
            float along = 0f;
            for (int index = first; index <= last; index++)
            {
                paths.Add(strokes[index].Path);
                along += strokes[index].Path.Length;
                ends[index - first] = along;
            }

            chains[first] = paths.Count == 1 ? paths[0] : StrokePath.Chain(paths);
            ends[ends.Length - 1] = chains[first].Length;
            chainEnds[first] = ends;
        }

        private static bool Meets(StrokePath before, StrokePath after)
        {
            return !before.IsDot && !after.IsDot
                && Math.Abs(after.Start.X - before.End.X) <= ContinuesWithin
                && Math.Abs(after.Start.Y - before.End.Y) <= ContinuesWithin;
        }
    }
}
