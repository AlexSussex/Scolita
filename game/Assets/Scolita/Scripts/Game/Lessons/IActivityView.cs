using Scolita.Core.Activities;
using Scolita.Core.Lessons;

namespace Scolita.Game.Lessons
{
    /// <summary>
    /// How one activity kind is drawn (AGENTS.md, seam 3): it shows a step's round, sends her taps to
    /// its input, and shows the feedback Core gives back. It never judges an answer and never speaks:
    /// the lesson's composition root says the feedback's lines. Each kind's view is a prefab whose root
    /// has an <see cref="ActivityView"/>, found through <see cref="ActivityRegistry"/>.
    /// </summary>
    public interface IActivityView
    {
        /// <summary>
        /// Shows <paramref name="round"/> (a step beginning) and takes her taps from now on, sending them
        /// to <paramref name="context"/>'s input. Called by <see cref="LessonRunner"/>.
        /// </summary>
        void Show(ActivityRound round, ActivityViewContext context);

        /// <summary>
        /// Shows <paramref name="feedback"/>: a sparkle on right, a wobble and a fade on a miss, a glow
        /// on the answer shown. Called by <see cref="LessonRunner"/> after each answer.
        /// </summary>
        void ShowFeedback(in StepFeedback feedback);

        /// <summary>Hides the view (its step is over); called by <see cref="LessonRunner"/>.</summary>
        void Hide();
    }
}
