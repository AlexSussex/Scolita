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
    /// The rules of <see cref="ActivityKind.MeetLetter"/> (<c>docs/curriculum.md</c> §4.2, design
    /// screen 2): the mascot says the letter's sound twice and a word that starts with it; she taps
    /// the letter to hear it again, then the picture to hear the word, and the step is done when she
    /// has done both, in either order. There are no wrong answers, so no hints. Its round's choices
    /// are the letter (<see cref="LetterChoice"/>) and the picture (<see cref="PictureChoice"/>), and, when the
    /// letter has its handwritten form, the "de mână" button (<see cref="ByHandChoice"/>): a tap writes the letter
    /// by hand on the card (<see cref="ActivityRound.Handwriting"/>) and says so, and changes nothing else.
    /// </summary>
    public sealed class MeetLetterRules : IActivityRules
    {
        /// <summary>The index of the letter card in the round's choices.</summary>
        public const int LetterChoice = 0;

        /// <summary>The index of the picture card in the round's choices.</summary>
        public const int PictureChoice = 1;

        /// <summary>The index of the "de mână" button in the round's choices, when the letter has its handwritten form.</summary>
        public const int ByHandChoice = 2;

        private readonly VoiceSequence tapLetter;
        private readonly VoiceSequence tapPicture;
        private readonly VoiceSequence soundThenTapPicture;
        private readonly VoiceSequence soundOnly;
        private readonly VoiceSequence wordThenTapLetter;
        private readonly VoiceSequence wordOnly;
        private readonly VoiceSequence byHand;
        private bool letterHeard;
        private bool pictureHeard;

        /// <summary>
        /// Creates the rules for meeting <paramref name="letter"/> in <paramref name="language"/>, with
        /// the picture of <paramref name="word"/>, and the "de mână" button when <paramref name="handwritten"/> (the
        /// letter's handwritten lower-case shape) is given. Called by <see cref="ActivityRulesFactory"/> and the tests.
        /// </summary>
        public MeetLetterRules(Letter letter, Word word, Language language, LetterShape handwritten)
        {
            Language = language;
            ActivityChoice letterCard = ActivityChoice.OfLetter(letter, language);
            ActivityChoice pictureCard = ActivityChoice.OfWord(word);
            ActivityChoice[] choices = handwritten == null
                ? new[] { letterCard, pictureCard }
                : new[] { letterCard, pictureCard, ActivityChoice.OfControl(language) };
            Round = new ActivityRound(ActivityKind.MeetLetter, language, choices, default);
            if (handwritten != null)
            {
                Round.ShowHandwriting(TraceFigure.OfLetter(handwritten));
            }

            VoiceFill sound = VoiceFill.OfLine(letterCard.HeardBy, letter.Lower);
            Instruction = VoiceSequence.Of(Line("meet-new"))
                .Then(Line("meet-listen")).With(VoiceSlot.X, sound).With(VoiceSlot.Word, VoiceFill.OfLine(word.WordLine, word.Text))
                .Then(Line("meet-tap-letter"));
            tapLetter = VoiceSequence.Of(Line("meet-tap-letter"));
            tapPicture = VoiceSequence.Of(Line("meet-tap-picture"));
            soundThenTapPicture = VoiceSequence.Of(letterCard.HeardBy).Then(Line("meet-tap-picture"));
            soundOnly = VoiceSequence.Of(letterCard.HeardBy);
            wordThenTapLetter = VoiceSequence.Of(word.WordLine).Then(Line("meet-tap-letter"));
            wordOnly = VoiceSequence.Of(word.WordLine);
            byHand = VoiceSequence.Of(Line("meet-by-hand"));
            Lines = new[] { Instruction, tapLetter, tapPicture, soundThenTapPicture, soundOnly, wordThenTapLetter, wordOnly, byHand };
        }

        /// <inheritdoc/>
        public ActivityKind Kind => ActivityKind.MeetLetter;

        /// <inheritdoc/>
        public Language Language { get; }

        /// <inheritdoc/>
        public ActivityRound Round { get; }

        /// <inheritdoc/>
        public VoiceSequence Instruction { get; }

        /// <inheritdoc/>
        public VoiceSequence Prompt => letterHeard ? tapPicture : tapLetter;

        /// <inheritdoc/>
        public VoiceSequence Response { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyList<VoiceSequence> Lines { get; }

        /// <inheritdoc/>
        public void Begin(IRandom random)
        {
            letterHeard = false;
            pictureHeard = false;
            Response = null;
        }

        /// <inheritdoc/>
        public AnswerVerdict Judge(in ActivityAnswer answer)
        {
            if (answer.Gesture != AnswerGesture.Tap)
            {
                throw new ArgumentOutOfRangeException(nameof(answer), answer.Gesture, "MeetLetter takes taps.");
            }

            switch (answer.Choice)
            {
                case LetterChoice:
                    return LetterTapped();
                case PictureChoice:
                    return PictureTapped();
                case ByHandChoice when Round.Handwriting != null:
                    Response = byHand;
                    return AnswerVerdict.Neutral;
                default:
                    throw new ArgumentOutOfRangeException(nameof(answer), answer.Choice, "MeetLetter has no such choice.");
            }
        }

        /// <inheritdoc/>
        public ActivityHint HintFor(int missNumber, in ActivityAnswer answer)
        {
            throw new InvalidOperationException("MeetLetter has no wrong answers, so no hints.");
        }

        /// <inheritdoc/>
        public ActivityHint AnswerShown()
        {
            throw new InvalidOperationException("MeetLetter has no wrong answers, so no answer to show.");
        }

        private AnswerVerdict LetterTapped()
        {
            if (letterHeard)
            {
                Response = soundOnly;
                return AnswerVerdict.Neutral;
            }

            letterHeard = true;
            Response = pictureHeard ? soundOnly : soundThenTapPicture;
            return pictureHeard ? AnswerVerdict.Done : AnswerVerdict.Right;
        }

        private AnswerVerdict PictureTapped()
        {
            if (pictureHeard || !letterHeard)
            {
                Response = letterHeard ? wordOnly : wordThenTapLetter;
                pictureHeard = true;
                return AnswerVerdict.Neutral;
            }

            pictureHeard = true;
            Response = wordOnly;
            return AnswerVerdict.Done;
        }

        private VoiceLineId Line(string key)
        {
            return VoiceLineIds.For(Language, VoiceLineKind.Line, key);
        }
    }
}
