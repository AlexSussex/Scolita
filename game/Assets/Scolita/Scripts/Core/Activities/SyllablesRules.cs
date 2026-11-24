using System;
using System.Collections.Generic;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Randomness;
using Scolita.Core.Voice;

namespace Scolita.Core.Activities
{
    /// <summary>
    /// The rules of <see cref="ActivityKind.Syllables"/> (<c>docs/curriculum.md</c> §4.2): she hears a word
    /// (the round's <see cref="ActivityRound.Subject"/>: its picture, and its text when she can read it),
    /// taps the drum (<see cref="Drum"/>) once per syllable (Romanian) or beat (English), each beat counted
    /// in <see cref="ActivityRound.PlacesFilled"/>, then taps the tick (<see cref="Tick"/>). The tick is
    /// right when the beats equal the word's syllables (<see cref="ActivityRound.Places"/>, from the
    /// content: ma-ma 2, pe-pe-ne 3); another count is a miss, and the beats start again. A miss says the
    /// word in syllables; the answer shown says them again while the view draws one empty circle per
    /// syllable for her to fill (<see cref="HintCue.ShowPlaces"/>).
    /// </summary>
    public sealed class SyllablesRules : IActivityRules
    {
        /// <summary>The index of the drum in the round's choices.</summary>
        public const int Drum = 0;

        /// <summary>The index of the tick (she has finished) in the round's choices.</summary>
        public const int Tick = 1;

        /// <summary>
        /// The most beats the drum's row of dots shows; one more is counted (so too many beats for the longest word
        /// is still a miss), and taps beyond it are not.
        /// </summary>
        public const int MostBeats = ActivityLimits.MostSyllables;

        private readonly VoiceSequence[] hints;
        private readonly VoiceSequence shown;
        private readonly VoiceSequence beatFirst;
        private readonly VoiceSequence inSyllables;

        /// <summary>
        /// Creates the rules for counting the syllables of <paramref name="word"/>, speaking
        /// <paramref name="language"/>. Throws an <see cref="ArgumentException"/> for a word with no syllable
        /// or more than <see cref="MostBeats"/> (the validator makes both impossible). Called by
        /// <see cref="ActivityRulesFactory"/> and the tests.
        /// </summary>
        public SyllablesRules(Word word, Language language)
        {
            if (word.Syllables.Count < 1 || word.Syllables.Count > MostBeats)
            {
                throw new ArgumentException("Syllables needs a word of 1 to " + MostBeats + " syllables; " + word.Id
                    + " has " + word.Syllables.Count + ".", nameof(word));
            }

            Language = language;
            ActivityChoice[] choices = { ActivityChoice.OfControl(language), ActivityChoice.OfControl(language) };
            ActivityChoice subject = word.ListenOnly ? ActivityChoice.OfWord(word) : ActivityChoice.OfWordToRead(word);
            Round = new ActivityRound(ActivityKind.Syllables, language, choices, default, subject);
            Round.SetPlaces(word.Syllables.Count);
            Instruction = VoiceSequence.Of(Line("syll")).Then(word.WordLine).Then(Line("syll-tick"));
            Prompt = VoiceSequence.Of(word.WordLine);
            inSyllables = VoiceSequence.Of(word.SyllablesLine);
            beatFirst = VoiceSequence.Of(Line("syll-hint-once"));
            hints = new[]
            {
                VoiceSequence.Of(Line("syll-hint-listen")).Then(word.SyllablesLine),
                VoiceSequence.Of(Line("syll-hint-count")).Then(word.SyllablesLine)
            };
            shown = VoiceSequence.Of(word.SyllablesLine).Then(Line("syll-hint-once"));
            List<VoiceSequence> lines = new List<VoiceSequence> { Instruction, Prompt, inSyllables, beatFirst, shown };
            lines.AddRange(hints);
            Lines = lines;
        }

        /// <inheritdoc/>
        public ActivityKind Kind => ActivityKind.Syllables;

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
            Response = null;
            Round.Reset();
        }

        /// <inheritdoc/>
        public AnswerVerdict Judge(in ActivityAnswer answer)
        {
            if (answer.Gesture != AnswerGesture.Tap || (answer.Choice != Drum && answer.Choice != Tick))
            {
                throw new ArgumentOutOfRangeException(nameof(answer), answer.Choice, "Syllables takes taps on its drum and its tick.");
            }

            return answer.Choice == Drum ? Beat() : Finished();
        }

        /// <inheritdoc/>
        public ActivityHint HintFor(int missNumber, in ActivityAnswer answer)
        {
            if (missNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(missNumber), missNumber, "Misses are counted from 1.");
            }

            int rung = Math.Min(missNumber, hints.Length) - 1;
            return new ActivityHint(hints[rung], HintCue.None, -1);
        }

        /// <inheritdoc/>
        public ActivityHint AnswerShown()
        {
            return new ActivityHint(shown, HintCue.ShowPlaces, -1);
        }

        private AnswerVerdict Beat()
        {
            Response = null;
            if (Round.PlacesFilled <= MostBeats)
            {
                Round.SetPlacesFilled(Round.PlacesFilled + 1);
            }

            return AnswerVerdict.Neutral;
        }

        private AnswerVerdict Finished()
        {
            if (Round.PlacesFilled == 0)
            {
                Response = beatFirst;
                return AnswerVerdict.Neutral;
            }

            if (Round.PlacesFilled == Round.Places)
            {
                Response = inSyllables;
                return AnswerVerdict.Done;
            }

            Response = null;
            Round.SetPlacesFilled(0);
            return AnswerVerdict.Wrong;
        }

        private VoiceLineId Line(string key)
        {
            return VoiceLineIds.For(Language, VoiceLineKind.Line, key);
        }
    }
}
