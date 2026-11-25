using System.Collections.Generic;
using Scolita.Core.Activities;
using Scolita.Core.Content;

namespace Scolita.Core.Lessons
{
    /// <summary>
    /// Which steps can be played now (the human, 2026-11-24: the ones that can't are skipped): those of a
    /// kind with rules and a view, whose content is there (<see cref="ActivityRulesFactory.CanCreate"/>: a
    /// <c>TraceLetter</c> step needs its letter's shape, so the handwritten steps wait for theirs).
    /// </summary>
    public sealed class PlayableSteps
    {
        private readonly HashSet<ActivityKind> kinds;
        private readonly Curriculum curriculum;

        /// <summary>
        /// Creates the playable steps of <paramref name="kindsWithViews"/> (the kinds the scene has a view for)
        /// in <paramref name="curriculum"/>; throws an <see cref="System.ArgumentNullException"/> for none. Called by the Lesson scene's composition root and the tests.
        /// </summary>
        public PlayableSteps(IEnumerable<ActivityKind> kindsWithViews, Curriculum curriculum)
        {
            kinds = new HashSet<ActivityKind>(kindsWithViews);
            this.curriculum = curriculum ?? throw new System.ArgumentNullException(nameof(curriculum));
        }

        /// <summary>Whether <paramref name="step"/> can be played; called by <see cref="LessonPlan"/> and the tests.</summary>
        public bool Includes(Step step)
        {
            return kinds.Contains(step.Kind) && ActivityRulesFactory.CanCreate(step, curriculum);
        }
    }
}
