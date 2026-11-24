using System;
using System.Collections.Generic;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Randomness;
using Scolita.Core.Voice;

namespace Scolita.Core.Activities
{
    /// <summary>
    /// The rules of <see cref="ActivityKind.ListenAndPick"/> (<c>docs/curriculum.md</c> §4.2): she
    /// hears a sound (or a word) and taps the right one of 2 to 4 letters (or pictures). One question
    /// per step (the human, 2026-11-24): the step's first item is the answer, and all its items are
    /// the choices, shuffled. A miss fades the choice and says the sound again; then "try another";
    /// the answer shown glows and says which one it is.
    /// </summary>
    public sealed class ListenAndPickRules : IActivityRules
    {
        private readonly VoiceSequence[] hints;
        private readonly VoiceSequence shown;

        /// <summary>
        /// Creates the rules for picking among <paramref name="letters"/> in <paramref name="language"/>,
        /// the first being the answer. Called by <see cref="ActivityRulesFactory"/> and the tests.
        /// </summary>
        public static ListenAndPickRules OfLetters(IReadOnlyList<Letter> letters, Language language)
        {
            ActivityChoice[] choices = new ActivityChoice[letters.Count];
            for (int index = 0; index < letters.Count; index++)
            {
                choices[index] = ActivityChoice.OfLetter(letters[index], language);
            }

            return new ListenAndPickRules(choices, language, true);
        }

        /// <summary>
        /// Creates the rules for picking among the pictures of <paramref name="words"/>, the first being
        /// the answer. Called by <see cref="ActivityRulesFactory"/> and the tests.
        /// </summary>
        public static ListenAndPickRules OfWords(IReadOnlyList<Word> words, Language language)
        {
            ActivityChoice[] choices = new ActivityChoice[words.Count];
            for (int index = 0; index < words.Count; index++)
            {
                choices[index] = ActivityChoice.OfWord(words[index]);
            }

            return new ListenAndPickRules(choices, language, false);
        }

        private ListenAndPickRules(ActivityChoice[] choices, Language language, bool letters)
        {
            if (choices.Length < 2)
            {
                throw new ArgumentException("ListenAndPick needs at least two choices.", nameof(choices));
            }

            Language = language;
            ActivityChoice answer = choices[0];
            Round = new ActivityRound(ActivityKind.ListenAndPick, language, choices, answer.Item);
            if (letters)
            {
                VoiceFill sound = VoiceFill.OfLine(answer.HeardBy, answer.Text);
                Instruction = VoiceSequence.Of(Line("pick-letter")).Then(Line("pick-letter-x")).With(VoiceSlot.X, sound);
                Prompt = VoiceSequence.Of(Line("pick-letter-x")).With(VoiceSlot.X, sound);
                hints = new[]
                {
                    VoiceSequence.Of(Line("listen-again")).Then(answer.HeardBy),
                    VoiceSequence.Of(Line("pick-hint-other")),
                    VoiceSequence.Of(Line("pick-hint-shape"))
                };
                shown = VoiceSequence.Of(Line("pick-hint-show")).With(VoiceSlot.X, sound);
            }
            else
            {
                Instruction = VoiceSequence.Of(Line("pick-picture")).Then(answer.HeardBy);
                Prompt = VoiceSequence.Of(answer.HeardBy);
                hints = new[] { VoiceSequence.Of(Line("listen-again")).Then(answer.HeardBy) };
                shown = VoiceSequence.Of(Line("show-answer")).Then(answer.HeardBy);
            }

            List<VoiceSequence> lines = new List<VoiceSequence> { Instruction, Prompt, shown };
            lines.AddRange(hints);
            Lines = lines;
        }

        /// <inheritdoc/>
        public ActivityKind Kind => ActivityKind.ListenAndPick;

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

        /// <inheritdoc/>
        public void Begin(IRandom random)
        {
            Round.Shuffle(random);
        }

        /// <inheritdoc/>
        public AnswerVerdict Judge(in ActivityAnswer answer)
        {
            if (answer.Choice < 0 || answer.Choice >= Round.Choices.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(answer), answer.Choice,
                    "ListenAndPick has " + Round.Choices.Count + " choices.");
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
