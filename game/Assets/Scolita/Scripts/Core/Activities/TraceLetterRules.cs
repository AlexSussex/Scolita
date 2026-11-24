using System;
using System.Collections.Generic;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Randomness;
using Scolita.Core.Tracing;
using Scolita.Core.Voice;

namespace Scolita.Core.Activities
{
    /// <summary>
    /// The rules of <see cref="ActivityKind.TraceLetter"/> (<c>docs/curriculum.md</c> §4.2, design screen 3):
    /// she traces the letter's strokes in order, from each start dot in its direction, in three rounds
    /// (on dots, on a faint shadow, from the start dot only), each round more exacting
    /// (<see cref="TracingTuning"/>). Each stroke is one part of the step: a good one is
    /// <see cref="AnswerVerdict.Right"/> (the last of the last round <see cref="AnswerVerdict.Done"/>), any
    /// other a miss, so the lesson run's one ladder gives the hint for what went wrong, then shows the
    /// answer (the stroke draws itself once). Its round's one choice is the letter; where the tracing is,
    /// is the round's <see cref="ActivityRound.Trace"/>.
    /// </summary>
    public sealed class TraceLetterRules : IActivityRules
    {
        private readonly TraceState state;
        private readonly TracingTuning tuning;
        private readonly VoiceSequence[] nextStroke;
        private readonly VoiceSequence[] roundDone;
        private readonly VoiceSequence finished;
        private readonly VoiceSequence hintDot;
        private readonly VoiceSequence hintArrow;
        private readonly VoiceSequence hintStay;
        private readonly VoiceSequence hintSlow;
        private readonly VoiceSequence shown;

        /// <summary>
        /// Creates the rules for tracing <paramref name="shape"/>, a shape of <paramref name="letter"/>, in
        /// <paramref name="language"/>, with <paramref name="tuning"/>'s tolerances. Called by
        /// <see cref="ActivityRulesFactory"/> and the tests.
        /// </summary>
        public TraceLetterRules(Letter letter, LetterShape shape, Language language, TracingTuning tuning)
        {
            Language = language;
            this.tuning = tuning ?? throw new ArgumentNullException(nameof(tuning));
            state = new TraceState(shape ?? throw new ArgumentNullException(nameof(shape)));
            ActivityChoice choice = shape.Case == LetterCase.Upper
                ? ActivityChoice.OfCapital(letter, language)
                : ActivityChoice.OfLetter(letter, language);
            Round = new ActivityRound(ActivityKind.TraceLetter, language, new[] { choice }, letter.Id);
            Round.Attach(state);

            VoiceFill sound = VoiceFill.OfLine(letter.SoundLine(language), choice.Text);
            Instruction = VoiceSequence.Of(Line(shape.Form == LetterForm.Print ? "trace-print" : "trace-hand"))
                .Then(Line("trace-start"));
            Prompt = VoiceSequence.Of(Line("trace-start"));
            nextStroke = new VoiceSequence[shape.Strokes.Count];
            for (int stroke = 1; stroke < nextStroke.Length; stroke++)
            {
                nextStroke[stroke] = VoiceSequence.Of(Line("trace-next-" + (stroke + 1)));
            }

            roundDone = new VoiceSequence[TracingTuning.Rounds];
            for (int round = 0; round + 1 < TracingTuning.Rounds; round++)
            {
                roundDone[round] = VoiceSequence.Of(Line("trace-done")).With(VoiceSlot.X, sound)
                    .Then(Line("trace-round-" + (round + 2)));
            }

            finished = VoiceSequence.Of(Line("trace-done")).With(VoiceSlot.X, sound);
            hintDot = VoiceSequence.Of(Line("trace-hint-dot"));
            hintArrow = VoiceSequence.Of(Line("trace-hint-arrow"));
            hintStay = VoiceSequence.Of(Line("trace-hint-stay"));
            hintSlow = VoiceSequence.Of(Line("trace-hint-slow"));
            shown = VoiceSequence.Of(Line("trace-hint-watch"));
            List<VoiceSequence> lines = new List<VoiceSequence>
            {
                Instruction, Prompt, finished, hintDot, hintArrow, hintStay, hintSlow, shown
            };
            for (int stroke = 1; stroke < nextStroke.Length; stroke++)
            {
                lines.Add(nextStroke[stroke]);
            }

            for (int round = 0; round + 1 < TracingTuning.Rounds; round++)
            {
                lines.Add(roundDone[round]);
            }

            Lines = lines;
        }

        /// <inheritdoc/>
        public ActivityKind Kind => ActivityKind.TraceLetter;

        /// <inheritdoc/>
        public Language Language { get; }

        /// <inheritdoc/>
        public ActivityRound Round { get; }

        /// <inheritdoc/>
        public VoiceSequence Instruction { get; }

        /// <inheritdoc/>
        public VoiceSequence Prompt { get; }

        /// <inheritdoc/>
        public VoiceSequence Response { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyList<VoiceSequence> Lines { get; }

        /// <inheritdoc/>
        public void Begin(IRandom random)
        {
            state.Reset();
            Response = null;
        }

        /// <inheritdoc/>
        public AnswerVerdict Judge(in ActivityAnswer answer)
        {
            if (answer.Gesture != AnswerGesture.Trace || answer.Points == null)
            {
                throw new ArgumentException("TraceLetter takes a drawn stroke, not a " + answer.Gesture + ".", nameof(answer));
            }

            if (state.RoundsDone >= TracingTuning.Rounds)
            {
                throw new InvalidOperationException("The letter is traced already.");
            }

            StrokeTolerance tolerance = tuning.ForRound(state.AnswerShown ? 0 : state.Round);
            StrokeResult result = StrokeCheck.Check(state.Shape.Strokes[state.Stroke].Path, answer.Points, tolerance);
            state.Judged(result.Verdict);
            if (result.Verdict != StrokeVerdict.Good)
            {
                Response = null;
                return AnswerVerdict.Wrong;
            }

            int roundBefore = state.RoundsDone;
            state.StrokeDone();
            if (state.RoundsDone == TracingTuning.Rounds)
            {
                Response = finished;
                return AnswerVerdict.Done;
            }

            Response = state.RoundsDone > roundBefore ? roundDone[roundBefore] : nextStroke[state.Stroke];
            return AnswerVerdict.Right;
        }

        /// <inheritdoc/>
        public ActivityHint HintFor(int missNumber, in ActivityAnswer answer)
        {
            if (missNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(missNumber), missNumber, "Misses are counted from 1.");
            }

            return new ActivityHint(HintLineFor(state.LastVerdict), HintCue.PulseStart, -1);
        }

        /// <inheritdoc/>
        public ActivityHint AnswerShown()
        {
            state.ShowAnswer();
            return new ActivityHint(shown, HintCue.AnimateStroke, -1);
        }

        private VoiceSequence HintLineFor(StrokeVerdict verdict)
        {
            switch (verdict)
            {
                case StrokeVerdict.StartedElsewhere:
                    return hintDot;
                case StrokeVerdict.WrongDirection:
                case StrokeVerdict.TooShort:
                    return hintArrow;
                case StrokeVerdict.LeftThePath:
                    return state.Guide == TraceGuide.Dotted ? hintStay : hintSlow;
                default:
                    throw new InvalidOperationException("A hint follows a stroke that missed, not a " + verdict + " one.");
            }
        }

        private VoiceLineId Line(string key)
        {
            return VoiceLineIds.For(Language, VoiceLineKind.Line, key);
        }
    }
}
