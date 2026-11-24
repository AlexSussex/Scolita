using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Lessons
{
    /// <summary>Tests <see cref="LessonPlan"/> on the real planet 1 lessons.</summary>
    public sealed class LessonPlanTests
    {
        /// <summary>Tests that <see cref="LessonPlan.Of"/> leaves out the steps of kinds that can't be played yet.</summary>
        [Test]
        public void TestOf_SkipsKindsNotBuilt()
        {
            Curriculum curriculum = RealContent.Read();

            LessonPlan plan = LessonPlan.Of(RealContent.Lesson(curriculum, "ro-a"), LanguageMix.Both,
                new PlayableSteps(new[] { ActivityKind.MeetLetter, ActivityKind.ListenAndPick }, curriculum), new Step[0], 1);

            Assert.That(plan.Steps.Select(step => step.Step.Kind), Is.EqualTo(new[] { ActivityKind.MeetLetter }));
            Assert.That(plan.Steps.Single().IsReview, Is.False);
        }

        /// <summary>Tests that <see cref="LessonPlan.Of"/> starts at the step number it resumes at.</summary>
        [Test]
        public void TestOf_ResumesAtAStepNumber()
        {
            Curriculum curriculum = RealContent.Read();

            LessonPlan plan = LessonPlan.Of(RealContent.Lesson(curriculum, "ro-t"), LanguageMix.Both,
                new PlayableSteps(new[] { ActivityKind.MeetLetter, ActivityKind.ListenAndPick }, curriculum), new Step[0], 2);

            Assert.That(plan.Steps.Select(step => step.Step.Number), Is.EqualTo(new[] { 2 }));
        }

        /// <summary>Tests that <see cref="LessonPlan.Of"/> leaves out the steps in a language outside the learner's mix.</summary>
        [Test]
        public void TestOf_SkipsALanguageOutsideTheMix()
        {
            Curriculum curriculum = RealContent.Read();

            LessonPlan plan = LessonPlan.Of(RealContent.Lesson(curriculum, "party-1"), LanguageMix.EnglishOnly,
                new PlayableSteps(new[] { ActivityKind.ListenAndPick, ActivityKind.WordTwins }, curriculum), new Step[0], 1);

            Assert.That(plan.Steps.Select(step => step.Step.Languages), Is.EqualTo(new[] { LanguageMix.EnglishOnly }));
        }

        /// <summary>Tests that <see cref="LessonPlan.Of"/> puts the review steps first, marked as reviews.</summary>
        [Test]
        public void TestOf_ReviewsComeFirst()
        {
            Curriculum curriculum = RealContent.Read();
            Step review = RealContent.FirstStep(curriculum, "ro-m", ActivityKind.ListenAndPick);

            LessonPlan plan = LessonPlan.Of(RealContent.Lesson(curriculum, "ro-t"), LanguageMix.Both,
                new PlayableSteps(new[] { ActivityKind.MeetLetter, ActivityKind.ListenAndPick }, curriculum), new[] { review }, 1);

            Assert.That(plan.Steps.Select(step => step.IsReview), Is.EqualTo(new[] { true, false, false }));
            Assert.That(plan.Steps[0].Step, Is.SameAs(review));
        }
    }
}
