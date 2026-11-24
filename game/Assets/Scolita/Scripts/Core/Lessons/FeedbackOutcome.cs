namespace Scolita.Core.Lessons
{
    /// <summary>What she gets for one answer (<see cref="StepFeedback.Outcome"/>), <c>docs/curriculum.md</c> §4.3.</summary>
    public enum FeedbackOutcome
    {
        /// <summary>Not an answer: the tapped thing is heard, nothing else.</summary>
        Neutral,

        /// <summary>Right, and the step goes on (a sparkle).</summary>
        Right,

        /// <summary>A miss: a wobble and a hint.</summary>
        TryAgain,

        /// <summary>A miss that shows the answer: it glows.</summary>
        ShowAnswer,

        /// <summary>Right, and the step is done: a sparkle and praise.</summary>
        Done
    }
}
