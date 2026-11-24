using System;
using System.Collections.Generic;
using Scolita.Core.Content;
using Scolita.Core.Content.Rules;
using Scolita.Core.Languages;
using Scolita.Core.Randomness;
using Scolita.Core.Voice;

namespace Scolita.Core.Activities
{
    /// <summary>
    /// The rules of <see cref="ActivityKind.FirstSound"/> (<c>docs/curriculum.md</c> §4.2): she sees a
    /// picture (the round's <see cref="ActivityRound.Subject"/>, its word not written, so its first
    /// letter isn't shown), hears its word, and taps the letter it starts with among the step's 2 to 4
    /// letters (taught ones), shuffled. A miss says the word again with its first sound stretched; the
    /// answer shown glows and says which letter it is.
    /// </summary>
    public sealed class FirstSoundRules : IActivityRules
    {
        private readonly VoiceSequence[] hints;
        private readonly VoiceSequence shown;

        /// <summary>
        /// Creates the rules for the picture of <paramref name="word"/> among <paramref name="letters"/>, in
        /// <paramref name="language"/>, its stretched line shown in the bubble as <paramref name="stretchedText"/>.
        /// Throws an <see cref="ArgumentException"/> for fewer than two letters, or a word that starts with
        /// none of them (the validator makes both impossible). Called by <see cref="ActivityRulesFactory"/>
        /// and the tests.
        /// </summary>
        public FirstSoundRules(IReadOnlyList<Letter> letters, Word word, Language language, string stretchedText)
        {
            if (letters.Count < 2)
            {
                throw new ArgumentException("FirstSound needs at least two letters to choose from.", nameof(letters));
            }

            Language = language;
            Letter answer = FirstLetterOf(word, letters);
            ActivityChoice[] choices = new ActivityChoice[letters.Count];
            for (int index = 0; index < letters.Count; index++)
            {
                choices[index] = ActivityChoice.OfLetter(letters[index], language);
            }

            Round = new ActivityRound(ActivityKind.FirstSound, language, choices, answer.Id, ActivityChoice.OfWord(word));
            VoiceFill sound = VoiceFill.OfLine(answer.SoundLine(language), answer.Lower);
            Instruction = VoiceSequence.Of(Line("first-sound")).Then(word.WordLine).Then(Line("first-sound-tap"));
            Prompt = VoiceSequence.Of(word.WordLine);
            hints = new[]
            {
                VoiceSequence.Of(Line("first-hint-stretch")).With(VoiceSlot.Stretched,
                    VoiceFill.OfLine(word.StretchedLine, stretchedText)),
                VoiceSequence.Of(Line("first-hint-which")).Then(answer.SoundLine(language))
            };
            shown = VoiceSequence.Of(Line("pick-hint-show")).With(VoiceSlot.X, sound);
            List<VoiceSequence> lines = new List<VoiceSequence> { Instruction, Prompt, shown };
            lines.AddRange(hints);
            Lines = lines;
        }

        /// <inheritdoc/>
        public ActivityKind Kind => ActivityKind.FirstSound;

        /// <inheritdoc/>
        public Language Language { get; }

        /// <inheritdoc/>
        public ActivityRound Round { get; }

        /// <inheritdoc/>
        public VoiceSequence Instruction { get; }

        /// <inheritdoc/>
        public VoiceSequence Prompt { get; }

        /// <inheritdoc/>
        public VoiceSequence Response => null;

        /// <inheritdoc/>
        public IReadOnlyList<VoiceSequence> Lines { get; }

        /// <summary>
        /// The letter <paramref name="word"/> starts with among <paramref name="letters"/>: the longest
        /// that matches its spelling (a group such as <c>ch</c> before <c>c</c>). Throws an
        /// <see cref="ArgumentException"/> when none does. Called by the constructor and the tests.
        /// </summary>
        public static Letter FirstLetterOf(Word word, IReadOnlyList<Letter> letters)
        {
            string spelling = WordRules.Spelling(word.Text);
            Letter found = null;
            foreach (Letter letter in letters)
            {
                bool starts = letter.Lower.Length > 0 && spelling.StartsWith(letter.Lower, StringComparison.Ordinal);
                if (starts && (found == null || letter.Lower.Length > found.Lower.Length))
                {
                    found = letter;
                }
            }

            return found ?? throw new ArgumentException("The word " + word.Id + " starts with none of the step's letters.",
                nameof(word));
        }

        /// <inheritdoc/>
        public void Begin(IRandom random)
        {
            Round.Shuffle(random);
        }

        /// <inheritdoc/>
        public AnswerVerdict Judge(in ActivityAnswer answer)
        {
            if (answer.Gesture != AnswerGesture.Tap || answer.Choice < 0 || answer.Choice >= Round.Choices.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(answer), answer.Choice,
                    "FirstSound takes taps on its " + Round.Choices.Count + " letters.");
            }

            return Round.Choices[answer.Choice].Item == Round.AnswerItem ? AnswerVerdict.Done : AnswerVerdict.Wrong;
        }

        /// <inheritdoc/>
        public ActivityHint HintFor(int missNumber, in ActivityAnswer answer)
        {
            if (missNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(missNumber), missNumber, "Misses are counted from 1.");
            }

            int rung = Math.Min(missNumber, hints.Length) - 1;
            return new ActivityHint(hints[rung], HintCue.FadeChoice, answer.Choice);
        }

        /// <inheritdoc/>
        public ActivityHint AnswerShown()
        {
            return new ActivityHint(shown, HintCue.GlowAnswer, Round.Answer);
        }

        private VoiceLineId Line(string key)
        {
            return VoiceLineIds.For(Language, VoiceLineKind.Line, key);
        }
    }
}
