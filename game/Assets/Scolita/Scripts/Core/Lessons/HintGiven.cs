using Scolita.Core.Activities;

namespace Scolita.Core.Lessons
{
    /// <summary>Raised by <see cref="LessonRun.HintGiven"/> for each hint, and when the answer is shown.</summary>
    public readonly struct HintGiven
    {
        /// <summary>Creates the event; called by <see cref="LessonRun"/>.</summary>
        public HintGiven(int stepIndex, PlannedStep step, int missNumber, ActivityHint hint)
        {
            StepIndex = stepIndex;
            Step = step;
            MissNumber = missNumber;
            Hint = hint;
        }

        /// <summary>The step's place in the run, from 0.</summary>
        public int StepIndex { get; }

        /// <summary>The step.</summary>
        public PlannedStep Step { get; }

        /// <summary>The miss it follows, from 1.</summary>
        public int MissNumber { get; }

        /// <summary>The hint.</summary>
        public ActivityHint Hint { get; }

        /// <summary>Whether it shows the answer.</summary>
        public bool ShowsAnswer => Hint.Cue == HintCue.GlowAnswer;
    }
}
