namespace Scolita.Core.Tracing
{
    /// <summary>
    /// Where a <c>TraceLetter</c> step is, for its view to draw (<c>ActivityRound.Trace</c>): the shape,
    /// the round and what it shows of the letter, the stroke she is at, how many rounds are done, and
    /// whether the answer is shown for this stroke. Owned and changed by <c>TraceLetterRules</c>; the view
    /// only reads it.
    /// </summary>
    public sealed class TraceState
    {
        /// <summary>Creates the state of tracing <paramref name="shape"/>; called by <c>TraceLetterRules</c>.</summary>
        public TraceState(LetterShape shape)
        {
            Shape = shape;
        }

        /// <summary>The letter's shape.</summary>
        public LetterShape Shape { get; }

        /// <summary>The round in play, from 0 (the last round once the step is done).</summary>
        public int Round { get; private set; }

        /// <summary>How many rounds are done (<see cref="TracingTuning.Rounds"/> once the step is done).</summary>
        public int RoundsDone { get; private set; }

        /// <summary>The stroke she is at in the round, from 0.</summary>
        public int Stroke { get; private set; }

        /// <summary>What the round shows of the letter.</summary>
        public TraceGuide Guide => (TraceGuide)Round;

        /// <summary>
        /// Whether the answer is shown for the stroke she is at (it drew itself once): its guide stays shown
        /// until she has traced it.
        /// </summary>
        public bool AnswerShown { get; private set; }

        /// <summary>The verdict on the last stroke she drew; <see cref="StrokeVerdict.Good"/> before any.</summary>
        public StrokeVerdict LastVerdict { get; private set; }

        /// <summary>Back to the first stroke of the first round; called by <c>TraceLetterRules</c> when a step begins.</summary>
        internal void Reset()
        {
            Round = 0;
            RoundsDone = 0;
            Stroke = 0;
            AnswerShown = false;
            LastVerdict = StrokeVerdict.Good;
        }

        /// <summary>Records the verdict on the stroke she just drew; called by <c>TraceLetterRules</c>.</summary>
        internal void Judged(StrokeVerdict verdict)
        {
            LastVerdict = verdict;
        }

        /// <summary>The answer is shown for the stroke she is at; called by <c>TraceLetterRules</c>.</summary>
        internal void ShowAnswer()
        {
            AnswerShown = true;
        }

        /// <summary>
        /// Moves on to the next stroke, or to the next round after the last one (staying on the last round
        /// once all are done); called by <c>TraceLetterRules</c> on a good stroke.
        /// </summary>
        internal void StrokeDone()
        {
            AnswerShown = false;
            if (Stroke + 1 < Shape.Strokes.Count)
            {
                Stroke++;
                return;
            }

            Stroke = 0;
            RoundsDone++;
            if (RoundsDone < TracingTuning.Rounds)
            {
                Round = RoundsDone;
            }
        }
    }
}
