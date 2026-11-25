using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Lessons
{
    /// <summary>Tests <see cref="LessonResume"/> on the real planet 1 with the kinds built so far.</summary>
    public sealed class LessonResumeTests
    {
        /// <summary>Tests that <see cref="LessonResume.Choose"/> with nothing saved opens the first lesson from its start.</summary>
        [Test]
        public void TestChoose_NothingSaved()
        {
            LessonResume resume = LessonResume.Choose(Order(LanguageMix.Both), default, 1, string.Empty);

            Assert.That(resume.Plan.Lesson.Id.ToString(), Is.EqualTo("ro-a"));
            Assert.That(resume.Reason, Is.EqualTo(ResumeReason.FirstLesson));
            Assert.That(resume.StepsDoneBefore, Is.EqualTo(0));
        }

        /// <summary>Tests that <see cref="LessonResume.Choose"/> resumes at the saved step while the signature is the same, with the steps before done.</summary>
        [Test]
        public void TestChoose_SameSignatureResumes()
        {
            LessonOrder order = Order(LanguageMix.Both);
            order.TryFind(ContentId.Create("ro-t"), out Lesson lesson);

            LessonResume resume = LessonResume.Choose(order, lesson.Id, 2, LessonSignature.Of(lesson));

            Assert.That(resume.Reason, Is.EqualTo(ResumeReason.Resumed));
            Assert.That(resume.Plan.Steps[0].Step.Number, Is.EqualTo(2));
            Assert.That(resume.StepsDoneBefore, Is.EqualTo(1));
        }

        /// <summary>Tests that <see cref="LessonResume.Choose"/> starts a lesson again when its steps changed since it was saved.</summary>
        [Test]
        public void TestChoose_ChangedSignatureStartsAgain()
        {
            LessonResume resume = LessonResume.Choose(Order(LanguageMix.Both), ContentId.Create("ro-t"), 2, "0000000000000000");

            Assert.That(resume.Reason, Is.EqualTo(ResumeReason.StepsChanged));
            Assert.That(resume.Plan.Steps[0].Step.Number, Is.EqualTo(1));
            Assert.That(resume.StepsDoneBefore, Is.EqualTo(0));
        }

        /// <summary>Tests that <see cref="LessonResume.Choose"/> starts a lesson again when the saved step is past its last step to play.</summary>
        [Test]
        public void TestChoose_PastTheEndStartsAgain()
        {
            LessonOrder order = Order(LanguageMix.Both);
            order.TryFind(ContentId.Create("ro-t"), out Lesson lesson);

            LessonResume resume = LessonResume.Choose(order, lesson.Id, 40, LessonSignature.Of(lesson));

            Assert.That(resume.Reason, Is.EqualTo(ResumeReason.PastTheEnd));
            Assert.That(resume.Plan.Steps[0].Step.Number, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that <see cref="LessonResume.Choose"/> opens the next playable lesson for a saved lesson
        /// that can't be played: unknown, with nothing built, or outside her languages.
        /// </summary>
        [Test]
        public void TestChoose_NotPlayableGivesTheNext()
        {
            LessonResume unknown = LessonResume.Choose(Order(LanguageMix.Both), ContentId.Create("no-such-lesson"), 3, "x");
            LessonResume notBuilt = LessonResume.Choose(Order(LanguageMix.Both), ContentId.Create("ro-words-1"), 1, "x");
            LessonResume otherLanguage = LessonResume.Choose(Order(LanguageMix.RomanianOnly), ContentId.Create("en-m"), 2, "x");

            Assert.That(unknown.Plan.Lesson.Id.ToString(), Is.EqualTo("ro-a"));
            Assert.That(unknown.Reason, Is.EqualTo(ResumeReason.NextLesson));
            Assert.That(notBuilt.Plan.Lesson.Id.ToString(), Is.EqualTo("party-1"));
            Assert.That(otherLanguage.Plan.Lesson.Id.ToString(), Is.EqualTo("ro-t"));
        }

        /// <summary>
        /// Tests that <see cref="LessonResume.WithWarmUps"/> puts the lesson's warm-ups before its steps, whether it starts or
        /// resumes, keeping the steps done before.
        /// </summary>
        [Test]
        public void TestWithWarmUps_BeforeTheLessonsSteps()
        {
            Curriculum curriculum = RealContent.Read();
            LessonOrder order = Order(LanguageMix.Both);
            order.TryFind(ContentId.Create("ro-t"), out Lesson lesson);
            Step warmUp = RealContent.FirstStep(curriculum, "ro-m", ActivityKind.ListenAndPick);
            FakeWarmUps warmUps = new FakeWarmUps(warmUp);

            LessonPlan started = LessonResume.Choose(order, lesson.Id, 1, LessonSignature.Of(lesson)).WithWarmUps(warmUps);
            LessonResume resume = LessonResume.Choose(order, lesson.Id, 2, LessonSignature.Of(lesson));
            LessonPlan resumed = resume.WithWarmUps(warmUps);

            Assert.That(started.Steps.Select(step => step.IsReview), Is.EqualTo(new[] { true, false, false }));
            Assert.That(started.Steps[0].Step, Is.SameAs(warmUp));
            Assert.That(resumed.Steps.Select(step => step.IsReview), Is.EqualTo(new[] { true, false }));
            Assert.That(resumed.Steps[1].Step.Number, Is.EqualTo(2));
            Assert.That(resume.StepsDoneBefore, Is.EqualTo(1));
            Assert.That(warmUps.AskedFor.Select(asked => asked.Id.ToString()), Is.EqualTo(new[] { "ro-t", "ro-t" }));
        }

        /// <summary>Tests that <see cref="LessonResume.Choose"/> gives nothing when no kind can be played.</summary>
        [Test]
        public void TestChoose_NothingToPlay()
        {
            Curriculum curriculum = RealContent.Read();
            LessonOrder order = new LessonOrder(curriculum.Domains[0], LanguageMix.Both, new PlayableSteps(new ActivityKind[0], curriculum));

            Assert.That(LessonResume.Choose(order, default, 1, string.Empty), Is.Null);
        }

        private static LessonOrder Order(LanguageMix mix)
        {
            Curriculum curriculum = RealContent.Read();
            return new LessonOrder(curriculum.Domains[0], mix, new PlayableSteps(new[] { ActivityKind.MeetLetter, ActivityKind.ListenAndPick }, curriculum));
        }
    }
}
