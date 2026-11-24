namespace Scolita.Core.Activities
{
    /// <summary>How she gave an answer (<see cref="ActivityAnswer.Gesture"/>).</summary>
    public enum AnswerGesture
    {
        /// <summary>She tapped a choice.</summary>
        Tap,

        /// <summary>She let go of what she was dragging on a choice.</summary>
        Drop,

        /// <summary>She dragged a choice to the place waiting for it (<c>BuildWord</c>'s tile to the rocket's next window).</summary>
        Place,

        /// <summary>She joined two choices with a line (<c>WordTwins</c>' words).</summary>
        Join,

        /// <summary>She drew a stroke with her finger (<c>TraceLetter</c>): its points are <see cref="ActivityAnswer.Points"/>.</summary>
        Trace
    }
}
