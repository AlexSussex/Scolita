using System;
using System.Collections.Generic;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;
using Scolita.Core.Randomness;
using Scolita.Core.Voice;

namespace Scolita.Core.Activities
{
    /// <summary>
    /// The rules of <see cref="ActivityKind.LetterHunt"/> (<c>docs/curriculum.md</c> §4.2): stars float
    /// by, some carrying the letter to find (the step's first letter, small and capital), the rest the
    /// step's other letters (taught ones, in both cases too); she pops every star with the letter. Each
    /// right star is one part of the step, said with its sound; the last one finishes it. The first
    /// miss makes the right stars twinkle; the answer shown glows one of them. How many stars there are
    /// comes from <see cref="LetterHuntTuning"/>.
    /// </summary>
    public sealed class LetterHuntRules : IActivityRules
    {
        private readonly ContentId target;
        private readonly int rightStars;
        private readonly VoiceSequence findPrompt;
        private readonly VoiceSequence morePrompt;
        private readonly VoiceSequence sound;
        private readonly VoiceSequence twinkleHint;
        private readonly VoiceSequence capitalHint;
        private readonly VoiceSequence notHint;
        private readonly VoiceSequence shown;
        private int found;

        /// <summary>
        /// Creates the rules for finding the first of <paramref name="letters"/> among the others, in
        /// <paramref name="language"/>, with <paramref name="tuning"/>'s stars. Throws an
        /// <see cref="ArgumentException"/> for fewer than two letters (the validator makes it impossible).
        /// Called by <see cref="ActivityRulesFactory"/> and the tests.
        /// </summary>
        public LetterHuntRules(IReadOnlyList<Letter> letters, Language language, LetterHuntTuning tuning)
        {
            if (letters.Count < 2)
            {
                throw new ArgumentException("LetterHunt needs the letter to find and at least one other.", nameof(letters));
            }

            Language = language;
            Letter find = letters[0];
            target = find.Id;
            rightStars = tuning.RightStars;
            Round = new ActivityRound(ActivityKind.LetterHunt, language, Stars(letters, language, tuning), target);

            VoiceFill letterSound = VoiceFill.OfLine(find.SoundLine(language), find.Lower);
            Instruction = VoiceSequence.Of(Line("hunt")).With(VoiceSlot.X, letterSound).Then(Line("hunt-capitals"));
            findPrompt = VoiceSequence.Of(Line("hunt")).With(VoiceSlot.X, letterSound);
            morePrompt = VoiceSequence.Of(Line("hunt-hint-more")).With(VoiceSlot.X, letterSound);
            sound = VoiceSequence.Of(find.SoundLine(language));
            twinkleHint = VoiceSequence.Of(Line("hunt-hint-twinkle")).With(VoiceSlot.X, letterSound);
            capitalHint = VoiceSequence.Of(Line("hunt-hint-capital")).Then(Line("hunt-hint-twinkle")).With(VoiceSlot.X, letterSound);
            notHint = VoiceSequence.Of(Line("hunt-hint-not"));
            shown = VoiceSequence.Of(Line("pick-hint-show")).With(VoiceSlot.X, letterSound);
            Lines = new[] { Instruction, findPrompt, morePrompt, sound, twinkleHint, capitalHint, notHint, shown };
        }

        /// <inheritdoc/>
        public ActivityKind Kind => ActivityKind.LetterHunt;

        /// <inheritdoc/>
        public Language Language { get; }

        /// <inheritdoc/>
        public ActivityRound Round { get; }

        /// <inheritdoc/>
        public VoiceSequence Instruction { get; }

        /// <inheritdoc/>
        public VoiceSequence Prompt => found == 0 ? findPrompt : morePrompt;

        /// <inheritdoc/>
        public VoiceSequence Response { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyList<VoiceSequence> Lines { get; }

        /// <inheritdoc/>
        public void Begin(IRandom random)
        {
            found = 0;
            Response = null;
            Round.Reset();
            Round.Shuffle(random);
        }

        /// <inheritdoc/>
        public AnswerVerdict Judge(in ActivityAnswer answer)
        {
            if (answer.Gesture != AnswerGesture.Tap || answer.Choice < 0 || answer.Choice >= Round.Choices.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(answer), answer.Choice,
                    "LetterHunt takes taps on its " + Round.Choices.Count + " stars.");
            }

            if (Round.StateOf(answer.Choice) == ChoiceState.Done)
            {
                Response = null;
                return AnswerVerdict.Neutral;
            }

            if (Round.Choices[answer.Choice].Item != target)
            {
                Response = null;
                return AnswerVerdict.Wrong;
            }

            Round.Mark(answer.Choice, ChoiceState.Done);
            found++;
            Response = sound;
            return found >= rightStars ? AnswerVerdict.Done : AnswerVerdict.Right;
        }

        /// <inheritdoc/>
        public ActivityHint HintFor(int missNumber, in ActivityAnswer answer)
        {
            if (missNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(missNumber), missNumber, "Misses are counted from 1.");
            }

            VoiceSequence line = missNumber > 1 ? notHint : OnlyCapitalsLeft() ? capitalHint : twinkleHint;
            return new ActivityHint(line, HintCue.TwinkleAnswers, -1);
        }

        /// <inheritdoc/>
        public ActivityHint AnswerShown()
        {
            return new ActivityHint(shown, HintCue.GlowAnswer, FirstStarLeft());
        }

        private static ActivityChoice[] Stars(IReadOnlyList<Letter> letters, Language language, LetterHuntTuning tuning)
        {
            ActivityChoice[] stars = new ActivityChoice[tuning.Stars];
            int capitals = tuning.RightStars / 2;
            for (int index = 0; index < tuning.RightStars; index++)
            {
                stars[index] = index < tuning.RightStars - capitals
                    ? ActivityChoice.OfLetter(letters[0], language)
                    : ActivityChoice.OfCapital(letters[0], language);
            }

            int others = letters.Count - 1;
            for (int wrong = 0; wrong < tuning.Stars - tuning.RightStars; wrong++)
            {
                Letter letter = letters[1 + wrong % others];
                bool capital = wrong / others % 2 == 1;
                stars[tuning.RightStars + wrong] = capital
                    ? ActivityChoice.OfCapital(letter, language)
                    : ActivityChoice.OfLetter(letter, language);
            }

            return stars;
        }

        private bool OnlyCapitalsLeft()
        {
            for (int index = 0; index < Round.Choices.Count; index++)
            {
                ActivityChoice star = Round.Choices[index];
                if (star.Item == target && Round.StateOf(index) != ChoiceState.Done && !star.IsCapital)
                {
                    return false;
                }
            }

            return true;
        }

        private int FirstStarLeft()
        {
            for (int index = 0; index < Round.Choices.Count; index++)
            {
                if (Round.Choices[index].Item == target && Round.StateOf(index) != ChoiceState.Done)
                {
                    return index;
                }
            }

            return -1;
        }

        private VoiceLineId Line(string key)
        {
            return VoiceLineIds.For(Language, VoiceLineKind.Line, key);
        }
    }
}
