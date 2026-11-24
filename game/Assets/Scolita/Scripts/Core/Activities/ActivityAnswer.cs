namespace Scolita.Core.Activities
{
    /// <summary>
    /// One answer she gives in an activity: for now, which of the round's choices she tapped
    /// (<see cref="ActivityRound.Choices"/>). A small value, so passing it allocates nothing.
    /// </summary>
    public readonly struct ActivityAnswer
    {
        private ActivityAnswer(int choice)
        {
            Choice = choice;
        }

        /// <summary>The index of the choice tapped, in <see cref="ActivityRound.Choices"/>.</summary>
        public int Choice { get; }

        /// <summary>A tap on the choice <paramref name="choice"/>; called by the activity views and the tests.</summary>
        public static ActivityAnswer Tap(int choice)
        {
            return new ActivityAnswer(choice);
        }
    }
}
