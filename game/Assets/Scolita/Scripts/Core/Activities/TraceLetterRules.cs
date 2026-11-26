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
    /// she traces the letter's strokes in order, from each start dot in its direction, in print or by hand (the
    /// handwritten form with its entry and exit, design screen 8), in up to three rounds (on dots, on a faint
    /// shadow, from the start dot only; the step's <c>rounds</c> option plays the first ones), each round more
    /// exacting (<see cref="TracingTuning"/>). A line through strokes that continue each other does them all
    /// (<see cref="LineCheck"/>); one that strays after a stroke it did keeps that stroke and is reminded to stay
    /// on the line, without a miss. Each stroke is one part of the step: a good one is
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
        private readonly VoiceSequence hintFromLine;
        private readonly VoiceSequence shown;
        private readonly VoiceSequence startPrompt;

        /// <summary>
        /// Creates the rules for tracing <paramref name="shape"/>, a shape of <paramref name="letter"/>, in
        /// <paramref name="language"/>, with <paramref name="tuning"/>'s tolerances, in <paramref name="rounds"/>
        /// rounds. Called by <see cref="ActivityRulesFactory"/> and the tests.
        /// </summary>
        public TraceLetterRules(Letter letter, LetterShape shape, Language language, TracingTuning tuning, int rounds)
        {
            Language = language;
            this.tuning = tuning ?? throw new ArgumentNullException(nameof(tuning));
            state = new TraceState(TraceFigure.OfLetter(shape ?? throw new ArgumentNullException(nameof(shape))), rounds, tuning);
            bool byHand = shape.Form == LetterForm.Handwritten;
            ActivityChoice choice = shape.Case == LetterCase.Upper
                ? ActivityChoice.OfCapital(letter, language)
                : ActivityChoice.OfLetter(letter, language);
            Round = new ActivityRound(ActivityKind.TraceLetter, language, new[] { choice }, letter.Id);
            Round.Attach(state);

            VoiceFill sound = VoiceFill.OfLine(letter.SoundLine(language), choice.Text);
            Instruction = VoiceSequence.Of(Line(byHand ? "trace-hand" : "trace-print"));
            if (byHand && state.Figure.StartsOnTheBaseline)
            {
                Instruction.Then(Line("trace-baseline"));
            }

            Instruction.Then(Line("trace-start"));
            string done = byHand ? "trace-done-hand" : "trace-done";
            startPrompt = VoiceSequence.Of(Line("trace-start"));
            nextStroke = new VoiceSequence[shape.Strokes.Count];
            for (int stroke = 1; stroke < nextStroke.Length; stroke++)
            {
                nextStroke[stroke] = VoiceSequence.Of(Line("trace-next-" + (stroke + 1)));
            }

            roundDone = new VoiceSequence[state.Rounds];
            for (int round = 0; round + 1 < state.Rounds; round++)
            {
                roundDone[round] = VoiceSequence.Of(Line(done)).With(VoiceSlot.X, sound)
                    .Then(Line("trace-round-" + (round + 2)));
            }

            finished = VoiceSequence.Of(Line(done)).With(VoiceSlot.X, sound);
            hintDot = VoiceSequence.Of(Line("trace-hint-dot"));
            hintArrow = VoiceSequence.Of(Line("trace-hint-arrow"));
            hintStay = VoiceSequence.Of(Line("trace-hint-stay"));
            hintSlow = VoiceSequence.Of(Line("trace-hint-slow"));
            hintFromLine = VoiceSequence.Of(Line("trace-hint-from-line"));
            shown = VoiceSequence.Of(Line("trace-hint-watch"));
            List<VoiceSequence> lines = new List<VoiceSequence>
            {
                Instruction, startPrompt, finished, hintDot, hintArrow, hintStay, hintSlow, hintFromLine, shown
            };
            for (int stroke = 1; stroke < nextStroke.Length; stroke++)
            {
                lines.Add(nextStroke[stroke]);
            }

            for (int round = 0; round + 1 < state.Rounds; round++)
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

        /// <summary>
        /// What is said again when she asks: "start at the dot and follow the arrow" while the arrow is shown (rounds
        /// 1 and 2), "start at the green dot" in round 3, where only the dot is.
        /// </summary>
        public VoiceSequence Prompt => state.Guide == TraceGuide.StartDotOnly ? hintDot : startPrompt;

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

            if (state.IsDone)
            {
                throw new InvalidOperationException("The letter is traced already.");
            }

            LineResult result = TraceJudging.Judge(state, tuning, answer.Points);
            if (result.Verdict != StrokeVerdict.Good)
            {
                Response = null;
                return AnswerVerdict.Wrong;
            }

            int roundBefore = state.RoundsDone;
            state.StrokesDone(result.StrokesDone);
            if (state.IsDone)
            {
                Response = finished;
                return AnswerVerdict.Done;
            }

            if (state.RoundsDone > roundBefore)
            {
                Response = roundDone[roundBefore];
            }
            else if (result.PointsKept < answer.Points.Count)
            {
                Response = state.Guide == TraceGuide.Dotted ? hintStay : hintSlow;
            }
            else
            {
                Response = nextStroke[state.Stroke];
            }

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
                case StrokeVerdict.SkippedEntry:
                    return hintFromLine;
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
