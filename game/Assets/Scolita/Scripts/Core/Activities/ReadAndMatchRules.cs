using System;
using System.Collections.Generic;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Randomness;
using Scolita.Core.Voice;

namespace Scolita.Core.Activities
{
    /// <summary>
    /// The rules of <see cref="ActivityKind.ReadAndMatch"/> (<c>docs/curriculum.md</c> §4.2): she reads a
    /// word on a card (the round's <see cref="ActivityRound.Subject"/>: the step's first word, written) and
    /// taps its picture among the pictures of the step's 2 to 3 words, shuffled. The word is never said
    /// before her first try: until she misses, the prompt is the instruction alone; after it, a tap on the
    /// card says the word. The first miss lights the word's letters one by one while their sounds are said;
    /// the answer shown glows the right picture and blends the word.
    /// </summary>
    public sealed class ReadAndMatchRules : IActivityRules
    {
        private readonly VoiceSequence wordPrompt;
        private readonly VoiceSequence[] hints;
        private readonly HintCue[] cues;
        private readonly VoiceSequence shown;
        private bool tried;

        /// <summary>
        /// Creates the rules for reading the first of <paramref name="words"/> and finding its picture among
        /// theirs, speaking <paramref name="language"/>, its slow line shown in the bubble as
        /// <paramref name="slowText"/>. Throws an <see cref="ArgumentException"/> for fewer than two words (the
        /// validator makes it impossible). Called by <see cref="ActivityRulesFactory"/> and the tests.
        /// </summary>
        public ReadAndMatchRules(IReadOnlyList<Word> words, Language language, string slowText)
        {
            if (words.Count < 2)
            {
                throw new ArgumentException("ReadAndMatch needs at least two words.", nameof(words));
            }

            Language = language;
            Word word = words[0];
            ActivityChoice[] pictures = new ActivityChoice[words.Count];
            for (int index = 0; index < words.Count; index++)
            {
                pictures[index] = ActivityChoice.OfWord(words[index]);
            }

            Round = new ActivityRound(ActivityKind.ReadAndMatch, language, pictures, word.Id, ActivityChoice.OfWordToRead(word));
            Instruction = VoiceSequence.Of(Line("read"));
            wordPrompt = VoiceSequence.Of(word.WordLine);
            VoiceFill slow = VoiceFill.OfLine(word.SlowLine, slowText);
            hints = new[]
            {
                VoiceSequence.Of(Line("read-hint-sounds")).Then(word.SlowLine),
                VoiceSequence.Of(Line("read-hint-tap"))
            };
            cues = new[] { HintCue.SpellOut, HintCue.FadeChoice };
            shown = VoiceSequence.Of(Line("read-hint-blend")).With(VoiceSlot.Slow, slow)
                .With(VoiceSlot.Word, VoiceFill.OfLine(word.WordLine, word.Text));
            List<VoiceSequence> lines = new List<VoiceSequence> { Instruction, wordPrompt, shown };
            lines.AddRange(hints);
            Lines = lines;
        }

        /// <inheritdoc/>
        public ActivityKind Kind => ActivityKind.ReadAndMatch;

        /// <inheritdoc/>
        public Language Language { get; }

        /// <inheritdoc/>
        public ActivityRound Round { get; }

        /// <inheritdoc/>
        public VoiceSequence Instruction { get; }

        /// <inheritdoc/>
        public VoiceSequence Prompt => tried ? wordPrompt : Instruction;

        /// <inheritdoc/>
        public VoiceSequence Response => null;

        /// <inheritdoc/>
        public IReadOnlyList<VoiceSequence> Lines { get; }

        /// <inheritdoc/>
        public void Begin(IRandom random)
        {
            tried = false;
            Round.Shuffle(random);
        }

        /// <inheritdoc/>
        public AnswerVerdict Judge(in ActivityAnswer answer)
        {
            if (answer.Gesture != AnswerGesture.Tap || answer.Choice < 0 || answer.Choice >= Round.Choices.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(answer), answer.Choice,
                    "ReadAndMatch takes taps on its " + Round.Choices.Count + " pictures.");
            }

            if (Round.Choices[answer.Choice].Item == Round.AnswerItem)
            {
                return AnswerVerdict.Done;
            }

            tried = true;
            return AnswerVerdict.Wrong;
        }

        /// <inheritdoc/>
        public ActivityHint HintFor(int missNumber, in ActivityAnswer answer)
        {
            if (missNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(missNumber), missNumber, "Misses are counted from 1.");
            }

            int rung = Math.Min(missNumber, hints.Length) - 1;
            return new ActivityHint(hints[rung], cues[rung], answer.Choice);
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
