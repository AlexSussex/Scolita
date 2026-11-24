using System;
using System.Collections.Generic;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Randomness;
using Scolita.Core.Voice;

namespace Scolita.Core.Activities
{
    /// <summary>
    /// The rules of <see cref="ActivityKind.TwoSounds"/> (<c>docs/curriculum.md</c> §4.2, design screen
    /// 4): one letter (the round's <see cref="ActivityRound.Subject"/>) on two little planets, the
    /// Romanian one (<see cref="RomanianPlanet"/>) and the English one (<see cref="EnglishPlanet"/>); she
    /// taps each to hear its sound, and only once both are <see cref="ChoiceState.Heard"/> do the
    /// pictures come, one by one (<see cref="ActivityRound.Current"/>, its word said), for her to drag to
    /// the planet whose sound it starts with: the planet of the word's language. Each picture is one part
    /// of the step. A miss says both sounds again, one after the other; the answer shown glows the right
    /// planet. It speaks the language of its lesson (<see cref="Language"/>), while each planet and each
    /// picture has its own.
    /// </summary>
    public sealed class TwoSoundsRules : IActivityRules
    {
        /// <summary>The index of the Romanian planet in the round's choices.</summary>
        public const int RomanianPlanet = 0;

        /// <summary>The index of the English planet in the round's choices.</summary>
        public const int EnglishPlanet = 1;

        /// <summary>The index of the first picture in the round's choices; the pictures follow it.</summary>
        public const int FirstPicture = 2;

        private readonly VoiceSequence tapPrompt;
        private readonly VoiceSequence[] sounds;
        private readonly Dictionary<ContentId, VoiceSequence> wordLines = new Dictionary<ContentId, VoiceSequence>();
        private readonly Dictionary<ContentId, VoiceSequence[]> soundThenSort = new Dictionary<ContentId, VoiceSequence[]>();
        private readonly Dictionary<ContentId, VoiceSequence> startHints = new Dictionary<ContentId, VoiceSequence>();
        private readonly VoiceSequence againHint;
        private readonly VoiceSequence badgesHint;
        private readonly VoiceSequence shown;

        /// <summary>
        /// Creates the rules for sorting the pictures of <paramref name="words"/> by the first sound of
        /// <paramref name="letter"/> in each language, speaking <paramref name="speaks"/>. Throws an
        /// <see cref="ArgumentException"/> for a letter that isn't in both alphabets or no word (the
        /// validator makes both impossible). Called by <see cref="ActivityRulesFactory"/> and the tests.
        /// </summary>
        public TwoSoundsRules(Letter letter, IReadOnlyList<Word> words, Language speaks)
        {
            if (!letter.HasLanguage(Language.Romanian) || !letter.HasLanguage(Language.English) || words.Count == 0)
            {
                throw new ArgumentException("TwoSounds needs a letter of both alphabets and pictures to sort.", nameof(letter));
            }

            Language = speaks;
            ActivityChoice[] choices = new ActivityChoice[FirstPicture + words.Count];
            choices[RomanianPlanet] = ActivityChoice.OfLetter(letter, Language.Romanian);
            choices[EnglishPlanet] = ActivityChoice.OfLetter(letter, Language.English);
            for (int index = 0; index < words.Count; index++)
            {
                choices[FirstPicture + index] = ActivityChoice.OfWord(words[index]);
            }

            Round = new ActivityRound(ActivityKind.TwoSounds, speaks, choices, default, ActivityChoice.OfLetter(letter, speaks));
            sounds = new[]
            {
                VoiceSequence.Of(choices[RomanianPlanet].HeardBy),
                VoiceSequence.Of(choices[EnglishPlanet].HeardBy)
            };
            Instruction = VoiceSequence.Of(Line("two-same-letter")).Then(Line("two-tap"));
            tapPrompt = VoiceSequence.Of(Line("two-tap"));
            againHint = VoiceSequence.Of(Line("two-hint-again")).Then(choices[RomanianPlanet].HeardBy)
                .Then(choices[EnglishPlanet].HeardBy);
            badgesHint = VoiceSequence.Of(Line("two-hint-badges"));
            shown = VoiceSequence.Of(Line("two-hint-show"));
            List<VoiceSequence> lines = new List<VoiceSequence>
            {
                Instruction, tapPrompt, sounds[RomanianPlanet], sounds[EnglishPlanet], againHint, badgesHint, shown
            };
            foreach (Word word in words)
            {
                wordLines[word.Id] = VoiceSequence.Of(word.WordLine);
                startHints[word.Id] = VoiceSequence.Of(Line("two-hint-start")).Then(word.WordLine);
                soundThenSort[word.Id] = new[]
                {
                    VoiceSequence.Of(choices[RomanianPlanet].HeardBy).Then(Line("two")).Then(word.WordLine),
                    VoiceSequence.Of(choices[EnglishPlanet].HeardBy).Then(Line("two")).Then(word.WordLine)
                };
                lines.Add(wordLines[word.Id]);
                lines.Add(startHints[word.Id]);
                lines.AddRange(soundThenSort[word.Id]);
            }

            Lines = lines;
        }

        /// <inheritdoc/>
        public ActivityKind Kind => ActivityKind.TwoSounds;

        /// <inheritdoc/>
        public Language Language { get; }

        /// <inheritdoc/>
        public ActivityRound Round { get; }

        /// <inheritdoc/>
        public VoiceSequence Instruction { get; }

        /// <inheritdoc/>
        public VoiceSequence Prompt => Round.Current < 0 ? tapPrompt : wordLines[Round.Choices[Round.Current].Item];

        /// <inheritdoc/>
        public VoiceSequence Response { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyList<VoiceSequence> Lines { get; }

        /// <inheritdoc/>
        public void Begin(IRandom random)
        {
            Response = null;
            Round.Reset();
            Round.ShuffleFrom(FirstPicture, random);
            Round.Play(-1, -1);
        }

        /// <inheritdoc/>
        public AnswerVerdict Judge(in ActivityAnswer answer)
        {
            if (answer.Choice < 0 || answer.Choice >= Round.Choices.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(answer), answer.Choice,
                    "TwoSounds has " + Round.Choices.Count + " choices.");
            }

            if (answer.Gesture == AnswerGesture.Drop)
            {
                return Dropped(answer.Choice);
            }

            return answer.Choice < FirstPicture ? PlanetTapped(answer.Choice) : PictureTapped(answer.Choice);
        }

        /// <inheritdoc/>
        public ActivityHint HintFor(int missNumber, in ActivityAnswer answer)
        {
            if (missNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(missNumber), missNumber, "Misses are counted from 1.");
            }

            switch (missNumber)
            {
                case 1:
                    return new ActivityHint(againHint, HintCue.None, -1);
                case 2:
                    return new ActivityHint(startHints[Round.Choices[Round.Current].Item], HintCue.None, -1);
                default:
                    return new ActivityHint(badgesHint, HintCue.None, -1);
            }
        }

        /// <inheritdoc/>
        public ActivityHint AnswerShown()
        {
            return new ActivityHint(shown, HintCue.GlowAnswer, Round.Answer);
        }

        private AnswerVerdict PlanetTapped(int planet)
        {
            bool wasSorting = IsSorting;
            if (Round.StateOf(planet) == ChoiceState.Open)
            {
                Round.Mark(planet, ChoiceState.Heard);
            }

            if (!wasSorting && Round.StateOf(RomanianPlanet) == ChoiceState.Heard && Round.StateOf(EnglishPlanet) == ChoiceState.Heard)
            {
                PlayPicture(FirstPicture);
                Response = soundThenSort[Round.Choices[FirstPicture].Item][planet];
                return AnswerVerdict.Neutral;
            }

            Response = sounds[planet];
            return AnswerVerdict.Neutral;
        }

        private AnswerVerdict PictureTapped(int picture)
        {
            Response = picture == Round.Current ? wordLines[Round.Choices[picture].Item] : null;
            return AnswerVerdict.Neutral;
        }

        private AnswerVerdict Dropped(int target)
        {
            if (!IsSorting)
            {
                Response = null;
                return AnswerVerdict.Neutral;
            }

            if (target >= FirstPicture)
            {
                throw new ArgumentOutOfRangeException(nameof(target), target, "A picture is dropped on a planet.");
            }

            if (target != Round.Answer)
            {
                Response = null;
                return AnswerVerdict.Wrong;
            }

            Round.Mark(Round.Current, ChoiceState.Done);
            Round.Place(Round.Current, target);
            int next = Round.Current + 1;
            if (next >= Round.Choices.Count)
            {
                Round.Play(-1, -1);
                Response = null;
                return AnswerVerdict.Done;
            }

            PlayPicture(next);
            Response = wordLines[Round.Choices[next].Item];
            return AnswerVerdict.Right;
        }

        private void PlayPicture(int picture)
        {
            Round.Play(picture, Round.Choices[picture].Language == Language.Romanian ? RomanianPlanet : EnglishPlanet);
        }

        private bool IsSorting => Round.Current >= 0;

        private VoiceLineId Line(string key)
        {
            return VoiceLineIds.For(Language, VoiceLineKind.Line, key);
        }
    }
}
