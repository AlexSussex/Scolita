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
    /// (<see cref="Subject"/>), where each choice stands (<see cref="StateOf"/>) and which one is in
    /// play (<see cref="Current"/>). Owned and set up by the step's rules (<see cref="IActivityRules.Begin"/>,
    /// <see cref="IActivityRules.Judge"/>); the view only reads it.
    /// </summary>
    public sealed class ActivityRound
    {
        private readonly ActivityChoice[] choices;
        private readonly ChoiceState[] states;

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
            AnswerItem = answerItem;
            Subject = subject;
            Answer = IndexOf(answerItem);
            Current = -1;
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
        /// The index of the answer in <see cref="Choices"/> (for the part in play, <see cref="Current"/>);
        /// -1 when the round has no single answer.
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
            for (int last = choices.Length - 1; last > first; last--)
            {
                int other = random.NextInt(first, last + 1);
                ActivityChoice kept = choices[last];
                choices[last] = choices[other];
                choices[other] = kept;
                ChoiceState keptState = states[last];
                states[last] = states[other];
                states[other] = keptState;
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
            }

            Current = -1;
        }

        /// <summary>Sets where the choice <paramref name="choice"/> stands; called by the activities' rules.</summary>
        internal void Mark(int choice, ChoiceState state)
        {
            states[choice] = state;
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
