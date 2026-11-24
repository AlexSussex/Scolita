namespace Scolita.Core.Activities
{
    /// <summary>
    /// One answer she gives in an activity: which of the round's choices she tapped
    /// (<see cref="ActivityRound.Choices"/>), or on which one she let go of what she was dragging
    /// (<c>TwoSounds</c>' picture on a planet). A small value, so passing it allocates nothing.
    /// </summary>
    public readonly struct ActivityAnswer
    {
        private ActivityAnswer(AnswerGesture gesture, int choice)
        {
            Gesture = gesture;
            Choice = choice;
        }

        /// <summary>How she gave it.</summary>
        public AnswerGesture Gesture { get; }

        /// <summary>The index of the choice tapped, or dropped on, in <see cref="ActivityRound.Choices"/>.</summary>
        public int Choice { get; }

        /// <summary>A tap on the choice <paramref name="choice"/>; called by the activity views and the tests.</summary>
        public static ActivityAnswer Tap(int choice)
        {
            return new ActivityAnswer(AnswerGesture.Tap, choice);
        }

        /// <summary>
        /// What she was dragging, let go on the choice <paramref name="target"/>; called by the activity
        /// views and the tests.
        /// </summary>
        public static ActivityAnswer Drop(int target)
        {
            return new ActivityAnswer(AnswerGesture.Drop, target);
        }
    }
}
