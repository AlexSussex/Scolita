using System.Collections.Generic;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Randomness;

namespace Scolita.Core.Activities
{
    /// <summary>
    /// What an activity's view draws for one step: its kind, its language and its choices, in the
    /// order they are shown, and which one is the answer. Owned and set up by the step's rules
    /// (<see cref="IActivityRules.Begin"/>); the view only reads it.
    /// </summary>
    public sealed class ActivityRound
    {
        private readonly ActivityChoice[] choices;

        /// <summary>
        /// Creates a round of <paramref name="choices"/>, the answer being the choice of
        /// <paramref name="answerItem"/> (not set for a round with no single answer). Called by the
        /// activities' rules when a lesson is built.
        /// </summary>
        public ActivityRound(ActivityKind kind, Language language, ActivityChoice[] choices, ContentId answerItem)
        {
            Kind = kind;
            Language = language;
            this.choices = choices;
            AnswerItem = answerItem;
            Answer = IndexOf(answerItem);
        }

        /// <summary>The activity kind.</summary>
        public ActivityKind Kind { get; }

        /// <summary>The step's language.</summary>
        public Language Language { get; }

        /// <summary>The choices, in the order they are shown.</summary>
        public IReadOnlyList<ActivityChoice> Choices => choices;

        /// <summary>The item that is the answer; not set when the round has no single answer.</summary>
        public ContentId AnswerItem { get; }

        /// <summary>The index of the answer in <see cref="Choices"/>; -1 when the round has no single answer.</summary>
        public int Answer { get; private set; }

        /// <summary>
        /// Puts the choices in a new order taken from <paramref name="random"/> (the answer is found
        /// again), allocating nothing. Called by the activities' rules when a step begins.
        /// </summary>
        internal void Shuffle(IRandom random)
        {
            for (int last = choices.Length - 1; last > 0; last--)
            {
                int other = random.NextInt(0, last + 1);
                ActivityChoice kept = choices[last];
                choices[last] = choices[other];
                choices[other] = kept;
            }

            Answer = IndexOf(AnswerItem);
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
