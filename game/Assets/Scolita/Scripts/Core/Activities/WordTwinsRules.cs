using System;
using System.Collections.Generic;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Randomness;
using Scolita.Core.Voice;

namespace Scolita.Core.Activities
{
    /// <summary>
    /// The rules of <see cref="ActivityKind.WordTwins"/> (<c>docs/curriculum.md</c> §4.2, §6): the Romanian
    /// words in one column (the round's first choices, <see cref="RomanianWords"/> of them) and their
    /// English partners in the other, each column shuffled on its own; a word she can read is written
    /// (<see cref="ActivityChoiceLook.Word"/>), a listen-only one is its picture. A tap on a word says it and
    /// puts it in play (<see cref="ActivityRound.Current"/>, its partner <see cref="ActivityRound.Answer"/>); a tap
    /// on its partner then joins them, while a tap on another word only says that one and puts it in play (she
    /// may be listening, so it is never a miss). A line from one word to another of the other column
    /// (<see cref="AnswerGesture.Join"/>) joins them. Partners (twins, or false friends) stay joined
    /// (<see cref="ActivityRound.PlacedOn"/>) with a line about which they are: each pair is one part of the
    /// step. A line to a word that isn't the partner is a miss: the pictures of the word in play and of its
    /// partner appear; the answer shown glows the partner. A line within one column, or to a joined word, is no
    /// answer. It speaks the language of its lesson (<see cref="Language"/>), while each word
    /// has its own.
    /// </summary>
    public sealed class WordTwinsRules : IActivityRules
    {
        private readonly Dictionary<ContentId, ContentId> partners = new Dictionary<ContentId, ContentId>();
        private readonly Dictionary<ContentId, VoiceSequence> heard = new Dictionary<ContentId, VoiceSequence>();
        private readonly HashSet<ContentId> falseFriends = new HashSet<ContentId>();
        private readonly VoiceSequence twinsJoined;
        private readonly VoiceSequence falseFriendsJoined;
        private readonly VoiceSequence picturesHint;
        private readonly VoiceSequence spelledHint;
        private readonly VoiceSequence soundHint;
        private readonly VoiceSequence shown;
        private int pairsLeft;

        /// <summary>
        /// Creates the rules for joining <paramref name="words"/> to their partners (each one's twin or false
        /// friend, among them), speaking <paramref name="speaks"/>. Throws an <see cref="ArgumentException"/> for
        /// a word whose partner isn't among them, or columns of different lengths (the validator makes both
        /// impossible). Called by <see cref="ActivityRulesFactory"/> and the tests.
        /// </summary>
        public WordTwinsRules(IReadOnlyList<Word> words, Language speaks)
        {
            Language = speaks;
            List<Word> romanian = new List<Word>();
            List<Word> english = new List<Word>();
            foreach (Word word in words)
            {
                (word.Language == Language.Romanian ? romanian : english).Add(word);
            }

            if (romanian.Count == 0 || romanian.Count != english.Count)
            {
                throw new ArgumentException("WordTwins needs as many Romanian words as English ones.", nameof(words));
            }

            RomanianWords = romanian.Count;
            ActivityChoice[] choices = new ActivityChoice[words.Count];
            for (int index = 0; index < romanian.Count; index++)
            {
                choices[index] = Choice(romanian[index], words);
                choices[RomanianWords + index] = Choice(english[index], words);
            }

            Round = new ActivityRound(ActivityKind.WordTwins, speaks, choices, default);
            Instruction = VoiceSequence.Of(Line("twins"));
            twinsJoined = VoiceSequence.Of(Line("twins-done"));
            falseFriendsJoined = VoiceSequence.Of(Line("false-friends"));
            picturesHint = VoiceSequence.Of(Line("twins-hint-pictures"));
            spelledHint = VoiceSequence.Of(Line("twins-hint-spelled"));
            soundHint = VoiceSequence.Of(Line("twins-hint-sound"));
            shown = VoiceSequence.Of(Line("twins-hint-show"));
            List<VoiceSequence> lines = new List<VoiceSequence>
            {
                Instruction, twinsJoined, falseFriendsJoined, picturesHint, spelledHint, soundHint, shown
            };
            lines.AddRange(heard.Values);
            Lines = lines;
        }

        /// <summary>How many Romanian words there are: the first column; the English one follows it.</summary>
        public int RomanianWords { get; }

        /// <inheritdoc/>
        public ActivityKind Kind => ActivityKind.WordTwins;

        /// <inheritdoc/>
        public Language Language { get; }

        /// <inheritdoc/>
        public ActivityRound Round { get; }

        /// <inheritdoc/>
        public VoiceSequence Instruction { get; }

        /// <inheritdoc/>
        public VoiceSequence Prompt => Instruction;

        /// <inheritdoc/>
        public VoiceSequence Response { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyList<VoiceSequence> Lines { get; }

        /// <inheritdoc/>
        public void Begin(IRandom random)
        {
            Response = null;
            pairsLeft = RomanianWords;
            Round.Reset();
            Round.ShuffleRange(0, RomanianWords, random);
            Round.ShuffleRange(RomanianWords, RomanianWords, random);
            Round.Play(-1, -1);
        }

        /// <inheritdoc/>
        public AnswerVerdict Judge(in ActivityAnswer answer)
        {
            bool known = answer.Gesture == AnswerGesture.Tap || answer.Gesture == AnswerGesture.Join;
            if (!known || !IsChoice(answer.Choice) || (answer.Gesture == AnswerGesture.Join && !IsChoice(answer.Other)))
            {
                throw new ArgumentOutOfRangeException(nameof(answer), answer.Choice,
                    "WordTwins takes taps and joins on its " + Round.Choices.Count + " words.");
            }

            if (answer.Gesture == AnswerGesture.Join)
            {
                return CanJoin(answer.Choice, answer.Other) ? Joined(answer.Choice, answer.Other) : Ignored();
            }

            int word = answer.Choice;
            if (Round.StateOf(word) != ChoiceState.Done && CanJoin(Round.Current, word) && PartnerOf(Round.Current) == word)
            {
                return Joined(Round.Current, word);
            }

            Response = heard[Round.Choices[word].Item];
            if (Round.StateOf(word) != ChoiceState.Done)
            {
                Round.Play(word, PartnerOf(word));
            }

            return AnswerVerdict.Neutral;
        }

        /// <inheritdoc/>
        public ActivityHint HintFor(int missNumber, in ActivityAnswer answer)
        {
            if (missNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(missNumber), missNumber, "Misses are counted from 1.");
            }

            VoiceSequence line = missNumber > 1 ? soundHint
                : falseFriends.Contains(Round.Choices[Round.Current].Item) ? spelledHint : picturesHint;
            return new ActivityHint(line, HintCue.RevealPictures, Round.Current);
        }

        /// <inheritdoc/>
        public ActivityHint AnswerShown()
        {
            return new ActivityHint(shown, HintCue.GlowAnswer, Round.Answer);
        }

        private ActivityChoice Choice(Word word, IReadOnlyList<Word> words)
        {
            ContentId partner = PartnerIn(word, words);
            if (!partner.IsSet)
            {
                throw new ArgumentException("The word " + word.Id + " has no twin or false friend in the step.", nameof(words));
            }

            partners.Add(word.Id, partner);
            if (partner == word.FalseFriend)
            {
                falseFriends.Add(word.Id);
            }

            heard.Add(word.Id, VoiceSequence.Of(word.WordLine));
            return word.ListenOnly ? ActivityChoice.OfWord(word) : ActivityChoice.OfWordToRead(word);
        }

        private static ContentId PartnerIn(Word word, IReadOnlyList<Word> words)
        {
            foreach (Word other in words)
            {
                if (other.Id == word.Twin || other.Id == word.FalseFriend)
                {
                    return other.Id;
                }
            }

            return default;
        }

        private bool IsChoice(int choice)
        {
            return choice >= 0 && choice < Round.Choices.Count;
        }

        private bool CanJoin(int word, int other)
        {
            return IsChoice(word) && IsChoice(other)
                && Round.StateOf(word) != ChoiceState.Done && Round.StateOf(other) != ChoiceState.Done
                && Round.Choices[word].Language != Round.Choices[other].Language;
        }

        private AnswerVerdict Ignored()
        {
            Response = null;
            return AnswerVerdict.Neutral;
        }

        private AnswerVerdict Joined(int word, int other)
        {
            if (PartnerOf(word) != other)
            {
                Response = null;
                Round.Play(word, PartnerOf(word));
                return AnswerVerdict.Wrong;
            }

            Round.Mark(word, ChoiceState.Done);
            Round.Mark(other, ChoiceState.Done);
            Round.Place(word, other);
            Round.Place(other, word);
            Round.Play(-1, -1);
            pairsLeft--;
            Response = falseFriends.Contains(Round.Choices[word].Item) ? falseFriendsJoined : twinsJoined;
            return pairsLeft == 0 ? AnswerVerdict.Done : AnswerVerdict.Right;
        }

        private int PartnerOf(int word)
        {
            ContentId partner = partners[Round.Choices[word].Item];
            for (int index = 0; index < Round.Choices.Count; index++)
            {
                if (Round.Choices[index].Item == partner)
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
