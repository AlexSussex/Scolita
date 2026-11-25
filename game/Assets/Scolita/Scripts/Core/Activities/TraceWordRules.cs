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
    /// The rules of <see cref="ActivityKind.TraceWord"/> (<c>docs/curriculum.md</c> §4.2, design screen 8): a short
    /// word she can read, handwritten and joined (<see cref="JoinedWord"/>), said slowly; she writes it letter by
    /// letter, each letter's exit flowing into the next one's start, then its dots and bars, in up to three rounds
    /// as <see cref="TraceLetterRules"/>. Each stroke is one part of the step; a line through several strokes does
    /// them all (<see cref="LineCheck"/>). When a letter's line is done the mascot says so at once and names the
    /// next letter to join („Bravo, ai făcut litera m! Acum leag-o de litera a, fără să ridici degetul.”); strokes
    /// within a letter say nothing new, so that line plays to its end while she writes on. The letters are said by
    /// their sounds, as every line of planets 1–3.
    /// </summary>
    public sealed class TraceWordRules : IActivityRules
    {
        private readonly TraceState state;
        private readonly TracingTuning tuning;
        private readonly VoiceSequence[] letterDone;
        private readonly VoiceSequence[] markNext;
        private readonly VoiceSequence[] roundDone;
        private readonly VoiceSequence finished;
        private readonly VoiceSequence hintFromLine;
        private readonly VoiceSequence hintBack;
        private readonly VoiceSequence hintJoin;
        private readonly VoiceSequence hintDot;
        private readonly VoiceSequence hintArrow;
        private readonly VoiceSequence hintStay;
        private readonly VoiceSequence hintSlow;
        private readonly VoiceSequence shown;
        private VoiceSequence lastSaid;

        /// <summary>
        /// Creates the rules for writing <paramref name="word"/>, spelled <paramref name="letters"/>, whose handwritten
        /// lower-case shapes are <paramref name="shapes"/> (one per letter, in order), in <paramref name="language"/>,
        /// with <paramref name="tuning"/>'s tolerances, in <paramref name="rounds"/> rounds. Called by
        /// <see cref="ActivityRulesFactory"/> and the tests.
        /// </summary>
        public TraceWordRules(Word word, IReadOnlyList<Letter> letters, IReadOnlyList<LetterShape> shapes, Language language,
            TracingTuning tuning, int rounds)
        {
            if (word == null)
            {
                throw new ArgumentNullException(nameof(word));
            }

            if (letters == null || shapes == null || letters.Count != shapes.Count)
            {
                throw new ArgumentException("A traced word has one shape for each of its letters.", nameof(shapes));
            }

            Language = language;
            this.tuning = tuning ?? throw new ArgumentNullException(nameof(tuning));
            state = new TraceState(JoinedWord.Of(shapes), rounds);
            Round = new ActivityRound(ActivityKind.TraceWord, language, new[] { ActivityChoice.OfWordToRead(word) }, word.Id);
            Round.Attach(state);

            Instruction = VoiceSequence.Of(Line("word-write")).Then(word.SlowLine);
            letterDone = new VoiceSequence[letters.Count];
            for (int letter = 0; letter + 1 < letters.Count; letter++)
            {
                letterDone[letter] = VoiceSequence.Of(Line("word-join"))
                    .With(VoiceSlot.X, SoundOf(letters[letter]))
                    .With(VoiceSlot.Y, SoundOf(letters[letter + 1]));
            }

            markNext = new VoiceSequence[state.Figure.Strokes.Count];
            for (int stroke = 0; stroke < markNext.Length; stroke++)
            {
                if (state.Figure.IsMark(stroke))
                {
                    markNext[stroke] = SameMarkLine(stroke, letters) ?? VoiceSequence.Of(MarkLine(stroke, letters));
                }
            }

            VoiceFill written = VoiceFill.OfLine(word.WordLine, word.Text);
            roundDone = new VoiceSequence[state.Rounds];
            for (int round = 0; round + 1 < state.Rounds; round++)
            {
                roundDone[round] = VoiceSequence.Of(Line("word-done")).With(VoiceSlot.Word, written)
                    .Then(Line("trace-round-" + (round + 2)));
            }

            finished = VoiceSequence.Of(Line("word-done")).With(VoiceSlot.Word, written);
            hintFromLine = VoiceSequence.Of(Line("word-hint-from-line"));
            hintBack = VoiceSequence.Of(Line("word-hint-back"));
            hintJoin = VoiceSequence.Of(Line("word-hint-join"));
            hintDot = VoiceSequence.Of(Line("trace-hint-dot"));
            hintArrow = VoiceSequence.Of(Line("trace-hint-arrow"));
            hintStay = VoiceSequence.Of(Line("trace-hint-stay"));
            hintSlow = VoiceSequence.Of(Line("trace-hint-slow"));
            shown = VoiceSequence.Of(Line("word-hint-watch"));
            Lines = AllLines();
        }

        /// <inheritdoc/>
        public ActivityKind Kind => ActivityKind.TraceWord;

        /// <inheritdoc/>
        public Language Language { get; }

        /// <inheritdoc/>
        public ActivityRound Round { get; }

        /// <inheritdoc/>
        public VoiceSequence Instruction { get; }

        /// <summary>What is said again when she asks: the last checkpoint or hint said, or the instruction before any.</summary>
        public VoiceSequence Prompt => lastSaid ?? Instruction;

        /// <inheritdoc/>
        public VoiceSequence Response { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyList<VoiceSequence> Lines { get; }

        /// <inheritdoc/>
        public void Begin(IRandom random)
        {
            state.Reset();
            Response = null;
            lastSaid = null;
        }

        /// <inheritdoc/>
        public AnswerVerdict Judge(in ActivityAnswer answer)
        {
            if (answer.Gesture != AnswerGesture.Trace || answer.Points == null)
            {
                throw new ArgumentException("TraceWord takes a drawn line, not a " + answer.Gesture + ".", nameof(answer));
            }

            if (state.IsDone)
            {
                throw new InvalidOperationException("The word is written already.");
            }

            int first = state.Stroke;
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
                return Said(finished, AnswerVerdict.Done);
            }

            if (state.RoundsDone > roundBefore)
            {
                return Said(roundDone[roundBefore], AnswerVerdict.Right);
            }

            return Said(CheckpointAfter(first, result), AnswerVerdict.Right);
        }

        /// <inheritdoc/>
        public ActivityHint HintFor(int missNumber, in ActivityAnswer answer)
        {
            if (missNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(missNumber), missNumber, "Misses are counted from 1.");
            }

            VoiceSequence hint = HintLineFor(state.LastVerdict);
            lastSaid = hint;
            return new ActivityHint(hint, HintCue.PulseStart, -1);
        }

        /// <inheritdoc/>
        public ActivityHint AnswerShown()
        {
            state.ShowAnswer();
            lastSaid = shown;
            return new ActivityHint(shown, HintCue.AnimateStroke, -1);
        }

        private AnswerVerdict Said(VoiceSequence line, AnswerVerdict verdict)
        {
            Response = line;
            if (line != null)
            {
                lastSaid = line;
            }

            return verdict;
        }

        private VoiceSequence CheckpointAfter(int first, in LineResult result)
        {
            VoiceSequence checkpoint = null;
            for (int stroke = first; stroke < first + result.StrokesDone; stroke++)
            {
                if (state.Figure.EndsLine(stroke) && letterDone[state.Figure.LetterOf(stroke)] != null)
                {
                    checkpoint = letterDone[state.Figure.LetterOf(stroke)];
                }
            }

            if (state.Figure.IsMark(state.Stroke))
            {
                return markNext[state.Stroke];
            }

            return checkpoint != null || !result.WentOn ? checkpoint : hintBack;
        }

        private VoiceSequence HintLineFor(StrokeVerdict verdict)
        {
            switch (verdict)
            {
                case StrokeVerdict.SkippedEntry:
                    return state.Figure.LetterOf(state.Stroke) == 0 ? hintFromLine : hintBack;
                case StrokeVerdict.StartedElsewhere:
                    return hintDot;
                case StrokeVerdict.TooShort:
                    return state.Figure.BeginsAtJoin(state.Stroke) ? hintJoin : hintArrow;
                case StrokeVerdict.WrongDirection:
                    return hintArrow;
                case StrokeVerdict.LeftThePath:
                    return state.Guide == TraceGuide.Dotted ? hintStay : hintSlow;
                default:
                    throw new InvalidOperationException("A hint follows a line that missed, not a " + verdict + " one.");
            }
        }

        private VoiceLineId MarkLine(int stroke, IReadOnlyList<Letter> letters)
        {
            Letter letter = letters[state.Figure.LetterOf(stroke)];
            string mark = state.Figure.Strokes[stroke].Path.IsDot ? "trace-dot-" : "trace-bar-";
            return Line(mark + letter.Strokes);
        }

        private VoiceSequence SameMarkLine(int stroke, IReadOnlyList<Letter> letters)
        {
            VoiceLineId line = MarkLine(stroke, letters);
            for (int before = 0; before < stroke; before++)
            {
                if (markNext[before] != null && markNext[before].Lead == line)
                {
                    return markNext[before];
                }
            }

            return null;
        }

        private VoiceFill SoundOf(Letter letter)
        {
            return VoiceFill.OfLine(letter.SoundLine(Language), letter.Lower);
        }

        private IReadOnlyList<VoiceSequence> AllLines()
        {
            List<VoiceSequence> lines = new List<VoiceSequence>
            {
                Instruction, finished, hintFromLine, hintBack, hintJoin, hintDot, hintArrow, hintStay, hintSlow, shown
            };
            foreach (VoiceSequence line in letterDone)
            {
                if (line != null)
                {
                    lines.Add(line);
                }
            }

            foreach (VoiceSequence line in markNext)
            {
                if (line != null && !lines.Contains(line))
                {
                    lines.Add(line);
                }
            }

            for (int round = 0; round + 1 < state.Rounds; round++)
            {
                lines.Add(roundDone[round]);
            }

            return lines;
        }

        private VoiceLineId Line(string key)
        {
            return VoiceLineIds.For(Language, VoiceLineKind.Line, key);
        }
    }
}
