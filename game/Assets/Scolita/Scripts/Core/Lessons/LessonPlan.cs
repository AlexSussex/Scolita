using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Languages;

namespace Scolita.Core.Lessons
{
    /// <summary>
    /// The steps a lesson run plays, in order: the review steps first (given by
    /// <c>progress-review</c>; none until then), then the lesson's own steps from the one it resumes
    /// at, without the steps in a language she doesn't learn and those of a kind that can't be played
    /// yet (no rules or no view: the human, 2026-11-24).
    /// </summary>
    public sealed class LessonPlan
    {
        private LessonPlan(Lesson lesson, IReadOnlyList<PlannedStep> steps)
        {
            Lesson = lesson;
            Steps = steps;
        }

        /// <summary>The lesson.</summary>
        public Lesson Lesson { get; }

        /// <summary>The steps to play, in order.</summary>
        public IReadOnlyList<PlannedStep> Steps { get; }

        /// <summary>
        /// Plans <paramref name="lesson"/> for a learner of <paramref name="mix"/>, playing only
        /// <paramref name="playableKinds"/>, after <paramref name="reviewSteps"/>, from its step number
        /// <paramref name="resumeAt"/> (from 1; below 1 means 1). Called by the Lesson scene's
        /// composition root, <see cref="LessonOrder"/> and the tests.
        /// </summary>
        public static LessonPlan Of(Lesson lesson, LanguageMix mix, IReadOnlyCollection<ActivityKind> playableKinds,
            IReadOnlyList<Step> reviewSteps, int resumeAt)
        {
            IEnumerable<PlannedStep> reviews = reviewSteps
                .Where(step => IsPlayable(step, mix, playableKinds))
                .Select(step => new PlannedStep(step, true));
            IEnumerable<PlannedStep> own = lesson.Steps
                .Where(step => step.Number >= resumeAt)
                .Where(step => IsPlayable(step, mix, playableKinds))
                .Select(step => new PlannedStep(step, false));
            return new LessonPlan(lesson, reviews.Concat(own).ToArray());
        }

        private static bool IsPlayable(Step step, LanguageMix mix, IReadOnlyCollection<ActivityKind> playableKinds)
        {
            return playableKinds.Contains(step.Kind)
                && step.Languages.Languages().All(language => mix.Includes(language));
        }
    }
}
