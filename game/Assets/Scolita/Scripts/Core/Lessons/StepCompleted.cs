namespace Scolita.Core.Lessons
{
    /// <summary>Raised by <see cref="LessonRun.StepCompleted"/> when a step is done.</summary>
    public readonly struct StepCompleted
    {
        /// <summary>Creates the event; called by <see cref="LessonRun"/>.</summary>
        public StepCompleted(int stepIndex, PlannedStep step, bool answerWasShown)
        {
            StepIndex = stepIndex;
            Step = step;
            AnswerWasShown = answerWasShown;
        }

        /// <summary>The step's place in the run, from 0.</summary>
        public int StepIndex { get; }

        /// <summary>The step.</summary>
        public PlannedStep Step { get; }

        /// <summary>Whether an answer of the step was shown before she did it.</summary>
        public bool AnswerWasShown { get; }
    }
}
