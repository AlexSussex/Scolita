using Scolita.Core.Tracing;

namespace Scolita.Core.Activities
{
    /// <summary>
    /// One answer she gives in an activity: which of the round's choices she tapped
    /// (<see cref="ActivityRound.Choices"/>), on which one she let go of what she was dragging
    /// (<c>TwoSounds</c>' picture on a planet), which one she dragged to the place waiting for it
    /// (<c>BuildWord</c>'s tile), which two she joined (<c>WordTwins</c>' words), or the stroke she drew
    /// (<c>TraceLetter</c>, a reference to the view's reused buffer). A small value, so passing it allocates
    /// nothing.
    /// </summary>
    public readonly struct ActivityAnswer
    {
        private ActivityAnswer(AnswerGesture gesture, int choice, int other, TracePoints points)
        {
            Gesture = gesture;
            Choice = choice;
            Other = other;
            Points = points;
        }

        /// <summary>How she gave it.</summary>
        public AnswerGesture Gesture { get; }

        /// <summary>
        /// The index of the choice tapped, dropped on, placed, or joined from, in <see cref="ActivityRound.Choices"/>.
        /// </summary>
        public int Choice { get; }

        /// <summary>The index of the choice joined to (<see cref="AnswerGesture.Join"/>); -1 for the other gestures.</summary>
        public int Other { get; }

        /// <summary>The points of the stroke she drew (<see cref="AnswerGesture.Trace"/>); null for the other gestures.</summary>
        public TracePoints Points { get; }

        /// <summary>A tap on the choice <paramref name="choice"/>; called by the activity views and the tests.</summary>
        public static ActivityAnswer Tap(int choice)
        {
            return new ActivityAnswer(AnswerGesture.Tap, choice, -1, null);
        }

        /// <summary>
        /// What she was dragging, let go on the choice <paramref name="target"/>; called by the activity
        /// views and the tests.
        /// </summary>
        public static ActivityAnswer Drop(int target)
        {
            return new ActivityAnswer(AnswerGesture.Drop, target, -1, null);
        }

        /// <summary>
        /// The choice <paramref name="choice"/>, dragged to the place waiting for it (<c>BuildWord</c>: a tile
        /// let go on the rocket goes to its next window); called by the activity views and the tests.
        /// </summary>
        public static ActivityAnswer Place(int choice)
        {
            return new ActivityAnswer(AnswerGesture.Place, choice, -1, null);
        }

        /// <summary>
        /// A line from the choice <paramref name="choice"/> to the choice <paramref name="other"/>
        /// (<c>WordTwins</c>); called by the activity views and the tests.
        /// </summary>
        public static ActivityAnswer Join(int choice, int other)
        {
            return new ActivityAnswer(AnswerGesture.Join, choice, other, null);
        }

        /// <summary>
        /// The stroke she drew, its points in <paramref name="points"/> (the view's buffer, read while the
        /// answer is judged); called by the tracing view and the tests.
        /// </summary>
        public static ActivityAnswer Traced(TracePoints points)
        {
            return new ActivityAnswer(AnswerGesture.Trace, -1, -1, points);
        }
    }
}
