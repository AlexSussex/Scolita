using Scolita.Core.Content;

namespace Scolita.Core.Lessons
{
    /// <summary>A step a lesson run plays: one of the lesson's own steps, or a review step put before them.</summary>
    public sealed class PlannedStep
    {
        /// <summary>Creates a planned step; called by <see cref="LessonPlan"/>.</summary>
        public PlannedStep(Step step, bool isReview)
        {
            Step = step;
            IsReview = isReview;
        }

        /// <summary>The step.</summary>
        public Step Step { get; }

        /// <summary>Whether it is a review step (a warm-up, <see cref="IWarmUps"/>), not one of the lesson's own.</summary>
        public bool IsReview { get; }
    }
}
