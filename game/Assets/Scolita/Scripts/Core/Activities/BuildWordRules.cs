using System;
using System.Collections.Generic;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Randomness;
using Scolita.Core.Voice;

namespace Scolita.Core.Activities
{
    /// <summary>
    /// The rules of <see cref="ActivityKind.BuildWord"/> (<c>docs/curriculum.md</c> §4.2, design screen 5):
    /// she hears a word (the round's <see cref="ActivityRound.Subject"/>, its picture) and drags letter
    /// tiles into the rocket's windows, one window per letter of the word
    /// (<see cref="ActivityRound.Places"/>), in order. The tiles are the round's choices: one per letter of
    /// the word and the step's 1 to 2 other letters (taught ones), shuffled. A tile placed
    /// (<see cref="AnswerGesture.Place"/>) with the next window's letter fills it
    /// (<see cref="ActivityRound.PlacesFilled"/>, <see cref="ActivityRound.PlacedOn"/>) and its sound is said:
    /// each window is one part of the step, the last one finishing it with the rocket's take-off, the word
    /// said slowly, then blended. Another tile is a miss: the word is said slowly; the answer shown makes a
    /// tile with the next letter glow (<see cref="ActivityRound.Answer"/>). A tap on a tile says its sound.
    /// </summary>
    public sealed class BuildWordRules : IActivityRules
    {
        /// <summary>The most letters a word may have: the rocket's windows.</summary>
        public const int MostWindows = ActivityLimits.MostWindows;

        private readonly ContentId[] windows;
        private readonly Dictionary<ContentId, VoiceSequence> sounds = new Dictionary<ContentId, VoiceSequence>();
        private readonly VoiceSequence slowHint;
        private readonly VoiceSequence[] nextHints;
        private readonly VoiceSequence shown;
        private readonly VoiceSequence takeOff;

        /// <summary>
        /// Creates the rules for building <paramref name="word"/> from the tiles of
        /// <paramref name="wordLetters"/> (its letters, in order) and <paramref name="otherLetters"/>, speaking
        /// <paramref name="language"/>, its slow line shown in the bubble as <paramref name="slowText"/>. Throws
        /// an <see cref="ArgumentException"/> for a word of no letter or more than <see cref="MostWindows"/>
        /// (the validator makes it impossible). Called by <see cref="ActivityRulesFactory"/> and the tests.
        /// </summary>
        public BuildWordRules(Word word, IReadOnlyList<Letter> wordLetters, IReadOnlyList<Letter> otherLetters,
            Language language, string slowText)
        {
            if (wordLetters.Count < 1 || wordLetters.Count > MostWindows)
            {
                throw new ArgumentException("BuildWord needs a word of 1 to " + MostWindows + " letters; " + word.Id
                    + " has " + wordLetters.Count + ".", nameof(wordLetters));
            }

            Language = language;
            windows = new ContentId[wordLetters.Count];
            ActivityChoice[] tiles = new ActivityChoice[wordLetters.Count + otherLetters.Count];
            for (int index = 0; index < wordLetters.Count; index++)
            {
                windows[index] = wordLetters[index].Id;
                tiles[index] = Tile(wordLetters[index]);
            }

            for (int index = 0; index < otherLetters.Count; index++)
            {
                tiles[wordLetters.Count + index] = Tile(otherLetters[index]);
            }

            Round = new ActivityRound(ActivityKind.BuildWord, language, tiles, default, ActivityChoice.OfWord(word));
            Round.SetPlaces(windows.Length);
            Instruction = VoiceSequence.Of(Line("build")).Then(word.WordLine);
            Prompt = VoiceSequence.Of(word.WordLine);
            slowHint = VoiceSequence.Of(Line("build-hint-slow")).With(VoiceSlot.Slow, VoiceFill.OfLine(word.SlowLine, slowText));
            nextHints = new VoiceSequence[windows.Length];
            nextHints[0] = VoiceSequence.Of(Line("build-hint-say"));
            for (int window = 1; window < windows.Length; window++)
            {
                Letter before = wordLetters[window - 1];
                nextHints[window] = VoiceSequence.Of(Line("build-hint-after"))
                    .With(VoiceSlot.X, VoiceFill.OfLine(before.SoundLine(language), before.Lower));
            }

            shown = VoiceSequence.Of(Line("build-hint-glow"));
            takeOff = VoiceSequence.Of(Line("build-liftoff")).Then(word.SlowLine).Then(word.WordLine);
            List<VoiceSequence> lines = new List<VoiceSequence> { Instruction, Prompt, slowHint, shown, takeOff };
            lines.AddRange(nextHints);
            lines.AddRange(sounds.Values);
            Lines = lines;
        }

        /// <inheritdoc/>
        public ActivityKind Kind => ActivityKind.BuildWord;

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
            Round.Shuffle(random);
            Round.Play(-1, OpenTileFor(0));
        }

        /// <inheritdoc/>
        public AnswerVerdict Judge(in ActivityAnswer answer)
        {
            bool known = answer.Gesture == AnswerGesture.Tap || answer.Gesture == AnswerGesture.Place;
            if (!known || answer.Choice < 0 || answer.Choice >= Round.Choices.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(answer), answer.Choice,
                    "BuildWord takes taps and places on its " + Round.Choices.Count + " tiles.");
            }

            if (answer.Gesture == AnswerGesture.Tap || Round.StateOf(answer.Choice) == ChoiceState.Done)
            {
                Response = answer.Gesture == AnswerGesture.Tap ? sounds[Round.Choices[answer.Choice].Item] : null;
                return AnswerVerdict.Neutral;
            }

            return Placed(answer.Choice);
        }

        /// <inheritdoc/>
        public ActivityHint HintFor(int missNumber, in ActivityAnswer answer)
        {
            if (missNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(missNumber), missNumber, "Misses are counted from 1.");
            }

            VoiceSequence line = missNumber == 1 ? slowHint : nextHints[Math.Min(Round.PlacesFilled, windows.Length - 1)];
            return new ActivityHint(line, HintCue.None, -1);
        }

        /// <inheritdoc/>
        public ActivityHint AnswerShown()
        {
            return new ActivityHint(shown, HintCue.GlowAnswer, Round.Answer);
        }

        private ActivityChoice Tile(Letter letter)
        {
            ActivityChoice tile = ActivityChoice.OfLetter(letter, Language);
            if (!sounds.ContainsKey(letter.Id))
            {
                sounds.Add(letter.Id, VoiceSequence.Of(letter.SoundLine(Language)));
            }

            return tile;
        }

        private AnswerVerdict Placed(int tile)
        {
            int window = Round.PlacesFilled;
            if (Round.Choices[tile].Item != windows[window])
            {
                Response = null;
                return AnswerVerdict.Wrong;
            }

            Round.Mark(tile, ChoiceState.Done);
            Round.Place(tile, window);
            Round.SetPlacesFilled(window + 1);
            if (window + 1 == windows.Length)
            {
                Round.Play(-1, -1);
                Response = takeOff;
                return AnswerVerdict.Done;
            }

            Round.Play(-1, OpenTileFor(window + 1));
            Response = sounds[windows[window]];
            return AnswerVerdict.Right;
        }

        private int OpenTileFor(int window)
        {
            for (int index = 0; index < Round.Choices.Count; index++)
            {
                if (Round.Choices[index].Item == windows[window] && Round.StateOf(index) != ChoiceState.Done)
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
