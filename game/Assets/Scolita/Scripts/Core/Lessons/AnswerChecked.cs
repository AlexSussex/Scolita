using Scolita.Core.Content;

namespace Scolita.Core.Lessons
{
    /// <summary>
    /// Raised by <see cref="LessonRun.AnswerChecked"/> for every answer that is right, wrong or
    /// practised (not for a tap that is no answer). <c>progress-review</c> counts these.
    /// </summary>
    public readonly struct AnswerChecked
    {
        /// <summary>Creates the event; called by <see cref="LessonRun"/>.</summary>
        public AnswerChecked(int stepIndex, PlannedStep step, AnswerResult result, int misses)
        {
            StepIndex = stepIndex;
            Step = step;
            Result = result;
            Misses = misses;
        }

        /// <summary>The step's place in the run, from 0.</summary>
        public int StepIndex { get; }

        /// <summary>The step.</summary>
        public PlannedStep Step { get; }

        /// <summary>How the answer counts.</summary>
        public AnswerResult Result { get; }

        /// <summary>The misses of the part so far, this one included.</summary>
        public int Misses { get; }

        /// <summary>The step's kind.</summary>
        public ActivityKind Kind => Step.Step.Kind;
    }
}
