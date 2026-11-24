namespace Scolita.Core.Lessons
{
    /// <summary>Where a <see cref="LessonRun"/> is.</summary>
    public enum LessonRunState
    {
        /// <summary>Built, not started.</summary>
        NotStarted,

        /// <summary>A step's rules wait for an answer.</summary>
        Playing,

        /// <summary>The current part's answer is shown (it glows); her next right answer counts as practice.</summary>
        AnswerShown,

        /// <summary>The step is done; the praise is said, then <see cref="LessonRun.Next"/> moves on.</summary>
        StepDone,

        /// <summary>Every step is done.</summary>
        Finished
    }
}
