using System;

namespace Scolita.Core.Tracing
{
    /// <summary>
    /// Where a tracing step (<c>TraceLetter</c>, <c>TraceWord</c>) is, for its view to draw (<c>ActivityRound.Trace</c>):
    /// the figure, the round and what it shows, the stroke she is at, how many rounds are done, whether the answer is
    /// shown for this stroke, and what her last line did. Owned and changed by the tracing rules; the view only reads it.
    /// </summary>
    public sealed class TraceState
    {
        /// <summary>
        /// Creates the state of tracing <paramref name="figure"/> in <paramref name="rounds"/> rounds (1 to
        /// <see cref="TracingTuning.Rounds"/>, the first ones); throws an <see cref="ArgumentOutOfRangeException"/>
        /// outside them. Called by the tracing rules.
        /// </summary>
        public TraceState(TraceFigure figure, int rounds)
        {
            if (rounds < 1 || rounds > TracingTuning.Rounds)
            {
                throw new ArgumentOutOfRangeException(nameof(rounds), rounds, "A tracing step has 1 to " + TracingTuning.Rounds + " rounds.");
            }

            Figure = figure ?? throw new ArgumentNullException(nameof(figure));
            Rounds = rounds;
        }

        /// <summary>What she traces: a letter, or a joined word.</summary>
        public TraceFigure Figure { get; }

        /// <summary>How many rounds the step has.</summary>
        public int Rounds { get; }

        /// <summary>The round in play, from 0 (the last round once the step is done).</summary>
        public int Round { get; private set; }

        /// <summary>How many rounds are done (<see cref="Rounds"/> once the step is done).</summary>
        public int RoundsDone { get; private set; }

        /// <summary>Whether every round is done.</summary>
        public bool IsDone => RoundsDone >= Rounds;

        /// <summary>The stroke she is at in the round, from 0.</summary>
        public int Stroke { get; private set; }

        /// <summary>What the round shows of the figure.</summary>
        public TraceGuide Guide => (TraceGuide)Round;

        /// <summary>
        /// Whether the answer is shown for the stroke she is at (it drew itself once): its guide stays shown
        /// until she has traced it.
        /// </summary>
        public bool AnswerShown { get; private set; }

        /// <summary>The verdict on the last line she drew; <see cref="StrokeVerdict.Good"/> before any.</summary>
        public StrokeVerdict LastVerdict { get; private set; }

        /// <summary>How many of her last line's points lie along what it did (the ink the view keeps).</summary>
        public int LastPointsKept { get; private set; }

        /// <summary>Back to the first stroke of the first round; called by the tracing rules when a step begins.</summary>
        internal void Reset()
        {
            Round = 0;
            RoundsDone = 0;
            Stroke = 0;
            AnswerShown = false;
            LastVerdict = StrokeVerdict.Good;
            LastPointsKept = 0;
        }

        /// <summary>Records what her last line did; called by the tracing rules.</summary>
        internal void Judged(in LineResult result)
        {
            LastVerdict = result.Verdict;
            LastPointsKept = result.PointsKept;
        }

        /// <summary>The answer is shown for the stroke she is at; called by the tracing rules.</summary>
        internal void ShowAnswer()
        {
            AnswerShown = true;
        }

        /// <summary>
        /// Moves on by <paramref name="strokes"/> strokes, to the next round after the last one (staying on the last
        /// round once all are done); called by the tracing rules on a good line.
        /// </summary>
        internal void StrokesDone(int strokes)
        {
            AnswerShown = false;
            if (Stroke + strokes < Figure.Strokes.Count)
            {
                Stroke += strokes;
                return;
            }

            Stroke = 0;
            RoundsDone++;
            if (RoundsDone < Rounds)
            {
                Round = RoundsDone;
            }
        }
    }
}
