using Scolita.Core.Content;

namespace Scolita.Core.Lessons
{
    /// <summary>
    /// Which lesson Go opens and where (the human, 2026-11-24): the saved lesson at its saved step
    /// while its steps' signature is the same, else from its start; the next lesson when the saved
    /// one can't be played; the first lesson when nothing is saved. Also how many of the lesson's
    /// steps to play are already done (the step dots show them done).
    /// </summary>
    public sealed class LessonResume
    {
        private LessonResume(LessonPlan plan, int stepsDoneBefore, ResumeReason reason)
        {
            Plan = plan;
            StepsDoneBefore = stepsDoneBefore;
            Reason = reason;
        }

        /// <summary>The steps to play.</summary>
        public LessonPlan Plan { get; }

        /// <summary>How many of the lesson's steps to play come before <see cref="Plan"/>'s first (done before).</summary>
        public int StepsDoneBefore { get; }

        /// <summary>Why this lesson and this step.</summary>
        public ResumeReason Reason { get; }

        /// <summary>
        /// Chooses from <paramref name="order"/> with what was saved: <paramref name="savedLesson"/> (not
        /// set for nothing), <paramref name="savedStep"/> (from 1) and <paramref name="savedSignature"/>
        /// (<see cref="LessonSignature"/>). Null when no lesson can be played. Called by the Lesson scene's
        /// composition root and <c>LessonResumeTests</c>.
        /// </summary>
        public static LessonResume Choose(LessonOrder order, ContentId savedLesson, int savedStep, string savedSignature)
        {
            if (savedLesson.IsSet && order.TryFind(savedLesson, out Lesson saved))
            {
                if (savedStep <= 1)
                {
                    return FromStart(order, saved, ResumeReason.FromTheStart);
                }

                if (savedSignature != LessonSignature.Of(saved))
                {
                    return FromStart(order, saved, ResumeReason.StepsChanged);
                }

                LessonPlan resumed = LessonPlan.Of(saved, order.Mix, order.PlayableSteps, new Step[0], savedStep);
                if (resumed.Steps.Count == 0)
                {
                    return FromStart(order, saved, ResumeReason.PastTheEnd);
                }

                int whole = LessonPlan.Of(saved, order.Mix, order.PlayableSteps, new Step[0], 1).Steps.Count;
                return new LessonResume(resumed, whole - resumed.Steps.Count, ResumeReason.Resumed);
            }

            Lesson next = order.After(savedLesson);
            if (next == null)
            {
                return null;
            }

            return FromStart(order, next, savedLesson.IsSet ? ResumeReason.NextLesson : ResumeReason.FirstLesson);
        }

        private static LessonResume FromStart(LessonOrder order, Lesson lesson, ResumeReason reason)
        {
            return new LessonResume(LessonPlan.Of(lesson, order.Mix, order.PlayableSteps, new Step[0], 1), 0, reason);
        }
    }
}
