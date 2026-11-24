using System;
using System.Collections.Generic;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Randomness;

namespace Scolita.Core.Activities
{
    /// <summary>
    /// What an activity's view draws for one step: its kind, the language it speaks, its choices in
    /// the order they are shown, which one is the answer, what the question is about
    /// (<see cref="Subject"/>), where each choice stands (<see cref="StateOf"/>, <see cref="PlacedOn"/>),
    /// which one is in play (<see cref="Current"/>) and the places it fills (<see cref="Places"/>,
    /// <see cref="PlacesFilled"/>). Owned and set up by the step's rules (<see cref="IActivityRules.Begin"/>,
    /// <see cref="IActivityRules.Judge"/>); the view only reads it.
    /// </summary>
    public sealed class ActivityRound
    {
        private readonly ActivityChoice[] choices;
        private readonly ChoiceState[] states;
        private readonly int[] placedOn;

        /// <summary>
        /// Creates a round of <paramref name="choices"/>, the answer being the choice of
        /// <paramref name="answerItem"/> (not set for a round with no single answer), with no subject.
        /// Called by the activities' rules when a lesson is built.
        /// </summary>
        public ActivityRound(ActivityKind kind, Language language, ActivityChoice[] choices, ContentId answerItem)
            : this(kind, language, choices, answerItem, null)
        {
        }

        /// <summary>
        /// Creates a round of <paramref name="choices"/> about <paramref name="subject"/> (null for none),
        /// the answer being the choice of <paramref name="answerItem"/> (not set for a round with no single
        /// answer). Called by the activities' rules when a lesson is built.
        /// </summary>
        public ActivityRound(ActivityKind kind, Language language, ActivityChoice[] choices, ContentId answerItem,
            ActivityChoice subject)
        {
            Kind = kind;
            Language = language;
            this.choices = choices;
            states = new ChoiceState[choices.Length];
            placedOn = new int[choices.Length];
            AnswerItem = answerItem;
            Subject = subject;
            Answer = IndexOf(answerItem);
            Current = -1;
            Reset();
        }

        /// <summary>The activity kind.</summary>
        public ActivityKind Kind { get; }

        /// <summary>The language the step speaks (each choice has its own, <see cref="ActivityChoice.Language"/>).</summary>
        public Language Language { get; }

        /// <summary>The choices, in the order they are shown.</summary>
        public IReadOnlyList<ActivityChoice> Choices => choices;

        /// <summary>
        /// The item that is the answer (for <c>LetterHunt</c>, the letter every right star carries); not set
        /// when the round has no single answer.
        /// </summary>
        public ContentId AnswerItem { get; }

        /// <summary>
        /// The index of the answer in <see cref="Choices"/> (for the part in play, <see cref="Current"/>; for
        /// <c>LetterHunt</c>, a star with the letter not found yet); -1 when there is none.
        /// </summary>
        public int Answer { get; private set; }

        /// <summary>
        /// What the question is about, shown but never an answer (<c>FirstSound</c>'s picture,
        /// <c>TwoSounds</c>' letter); a tap on it asks to hear the question again. Null for none.
        /// </summary>
        public ActivityChoice Subject { get; }

        /// <summary>
        /// The index of the choice in play (<c>TwoSounds</c>' picture in her hand); -1 when none is (before
        /// the sorting starts, and once it is over).
        /// </summary>
        public int Current { get; private set; }

        /// <summary>
        /// How many places the round has to fill (<c>BuildWord</c>'s windows, <c>Syllables</c>' syllables); 0 for
        /// none.
        /// </summary>
        public int Places { get; private set; }

        /// <summary>
        /// How many of the places are filled (<c>BuildWord</c>'s windows filled, in order; <c>Syllables</c>' drum
        /// beats tapped).
        /// </summary>
        public int PlacesFilled { get; private set; }

        /// <summary>
        /// Where the choice <paramref name="choice"/> stands; throws an <see cref="ArgumentOutOfRangeException"/>
        /// for an index out of the choices. Called by the activity views after each answer, and the tests.
        /// </summary>
        public ChoiceState StateOf(int choice)
        {
            if (choice < 0 || choice >= states.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(choice), choice, "The round has " + states.Length + " choices.");
            }

            return states[choice];
        }

        /// <summary>
        /// The choice the choice <paramref name="choice"/> was put on (<c>TwoSounds</c>: the planet a picture
        /// was sorted to; <c>WordTwins</c>: the word it was joined to), or the place it went to (<c>BuildWord</c>:
        /// the window a tile filled); -1 when it wasn't. Throws an <see cref="ArgumentOutOfRangeException"/> for an index
        /// out of the choices. Called by the activity views after each answer, and the tests.
        /// </summary>
        public int PlacedOn(int choice)
        {
            if (choice < 0 || choice >= placedOn.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(choice), choice, "The round has " + placedOn.Length + " choices.");
            }

            return placedOn[choice];
        }

        /// <summary>
        /// Puts the choices in a new order taken from <paramref name="random"/> (the answer is found
        /// again), allocating nothing. Called by the activities' rules when a step begins.
        /// </summary>
        internal void Shuffle(IRandom random)
        {
            ShuffleFrom(0, random);
        }

        /// <summary>
        /// Puts the choices from <paramref name="first"/> on in a new order taken from
        /// <paramref name="random"/>, the ones before staying in place. Called by the activities' rules
        /// when a step begins.
        /// </summary>
        internal void ShuffleFrom(int first, IRandom random)
        {
            ShuffleRange(first, choices.Length - first, random);
        }

        /// <summary>
        /// Puts the <paramref name="count"/> choices from <paramref name="first"/> on in a new order taken from
        /// <paramref name="random"/>, the others staying in place (<c>WordTwins</c>' columns). Called by the
        /// activities' rules when a step begins.
        /// </summary>
        internal void ShuffleRange(int first, int count, IRandom random)
        {
            for (int last = first + count - 1; last > first; last--)
            {
                int other = random.NextInt(first, last + 1);
                ActivityChoice kept = choices[last];
                choices[last] = choices[other];
                choices[other] = kept;
                ChoiceState keptState = states[last];
                states[last] = states[other];
                states[other] = keptState;
                int keptPlace = placedOn[last];
                placedOn[last] = placedOn[other];
                placedOn[other] = keptPlace;
            }

            Answer = IndexOf(AnswerItem);
        }

        /// <summary>
        /// Sets every choice back to <see cref="ChoiceState.Open"/> and none in play; called by the
        /// activities' rules when a step begins.
        /// </summary>
        internal void Reset()
        {
            for (int index = 0; index < states.Length; index++)
            {
                states[index] = ChoiceState.Open;
                placedOn[index] = -1;
            }

            Current = -1;
            PlacesFilled = 0;
        }

        /// <summary>Sets how many places the round has to fill; called by the activities' rules when a lesson is built.</summary>
        internal void SetPlaces(int places)
        {
            Places = places;
        }

        /// <summary>Sets how many of the places are filled; called by the activities' rules.</summary>
        internal void SetPlacesFilled(int filled)
        {
            PlacesFilled = filled;
        }

        /// <summary>Sets where the choice <paramref name="choice"/> stands; called by the activities' rules.</summary>
        internal void Mark(int choice, ChoiceState state)
        {
            states[choice] = state;
        }

        /// <summary>
        /// Records that the choice <paramref name="choice"/> was put on <paramref name="target"/> (a choice, or a
        /// place); called by the activities' rules.
        /// </summary>
        internal void Place(int choice, int target)
        {
            placedOn[choice] = target;
        }

        /// <summary>
        /// Puts the choice <paramref name="current"/> in play (-1 for none), <paramref name="answer"/> being
        /// its answer (-1 for none). Called by the activities' rules.
        /// </summary>
        internal void Play(int current, int answer)
        {
            Current = current;
            Answer = answer;
        }

        private int IndexOf(ContentId item)
        {
            if (!item.IsSet)
            {
                return -1;
            }

            for (int index = 0; index < choices.Length; index++)
            {
                if (choices[index].Item == item)
                {
                    return index;
                }
            }

            return -1;
        }
    }
}
