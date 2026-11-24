using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Activities;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Lessons
{
    /// <summary>
    /// Tests <see cref="LessonRun"/> on the real <c>ro-t</c> lesson with the kinds built so far
    /// (<c>MeetLetter</c> then <c>ListenAndPick</c> with t, a and m), the choices kept in their order
    /// (<see cref="FixedRandom"/>: the answer t is choice 0).
    /// </summary>
    public sealed class LessonRunTests
    {
        /// <summary>Tests that <see cref="LessonRun.Start"/> begins the first step, once.</summary>
        [Test]
        public void TestStart_BeginsTheFirstStep()
        {
            LessonRun run = RomanianT(TestLearningRules.Default());

            run.Start();

            Assert.That(run.State, Is.EqualTo(LessonRunState.Playing));
            Assert.That(run.StepIndex, Is.EqualTo(0));
            Assert.That(run.StepCount, Is.EqualTo(2));
            Assert.That(run.Current.Kind, Is.EqualTo(ActivityKind.MeetLetter));
            Assert.That(run.Lines, Has.Member(run.Current.Instruction));
            Assert.That(run.Lines, Has.Member(run.Ending));
            Assert.That(() => run.Start(), Throws.InstanceOf<InvalidOperationException>());
        }

        /// <summary>
        /// Tests that <see cref="LessonRun.Answer"/> and <see cref="LessonRun.Next"/> play the lesson to its
        /// end: the steps in order, praise after each, the events raised, and the end line.
        /// </summary>
        [Test]
        public void TestAnswer_PlaysTheLessonToItsEnd()
        {
            LessonRun run = RomanianT(TestLearningRules.Default());
            List<StepCompleted> completed = new List<StepCompleted>();
            List<AnswerChecked> checkedAnswers = new List<AnswerChecked>();
            List<LessonCompleted> finished = new List<LessonCompleted>();
            run.StepCompleted += completed.Add;
            run.AnswerChecked += checkedAnswers.Add;
            run.LessonCompleted += finished.Add;
            run.Start();

            StepFeedback letter = run.Answer(ActivityAnswer.Tap(MeetLetterRules.LetterChoice));
            StepFeedback picture = run.Answer(ActivityAnswer.Tap(MeetLetterRules.PictureChoice));
            LessonRunState afterMeet = run.State;
            run.Next();
            ActivityKind second = run.Current.Kind;
            StepFeedback pick = run.Answer(ActivityAnswer.Tap(Answer));
            run.Next();

            Assert.That(letter.Outcome, Is.EqualTo(FeedbackOutcome.Right));
            Assert.That(SequenceLines.Of(letter.First), Is.EqualTo(new[] { "ro-sound-t", "ro-line-meet-tap-picture" }));
            Assert.That(letter.Then, Is.Null);
            Assert.That(picture.Outcome, Is.EqualTo(FeedbackOutcome.Done));
            Assert.That(SequenceLines.Of(picture.Then)[0], Does.StartWith("ro-line-praise-"));
            Assert.That(afterMeet, Is.EqualTo(LessonRunState.StepDone));
            Assert.That(second, Is.EqualTo(ActivityKind.ListenAndPick));
            Assert.That(pick.Outcome, Is.EqualTo(FeedbackOutcome.Done));
            Assert.That(pick.First, Is.Null);
            Assert.That(run.State, Is.EqualTo(LessonRunState.Finished));
            Assert.That(completed.Select(step => step.StepIndex), Is.EqualTo(new[] { 0, 1 }));
            Assert.That(completed.All(step => !step.AnswerWasShown), Is.True);
            Assert.That(checkedAnswers.Select(answer => answer.Result), Is.All.EqualTo(AnswerResult.Right));
            Assert.That(checkedAnswers.Count, Is.EqualTo(3));
            Assert.That(finished.Single().Lesson.Id.ToString(), Is.EqualTo("ro-t"));
            Assert.That(SequenceLines.Of(run.Ending), Is.EqualTo(new[] { "ro-line-lesson-done" }));
            Assert.That(run.EndingLanguage, Is.EqualTo(Language.Romanian));
        }

        /// <summary>
        /// Tests the ladder in <see cref="LessonRun.Answer"/>: the first miss gets the kind's hint, the
        /// second shows the answer, a miss after that shows it again, and the right answer then counts
        /// as practice.
        /// </summary>
        [Test]
        public void TestAnswer_HintThenShowThenPractice()
        {
            LessonRun run = AtThePick(TestLearningRules.Default());
            List<HintGiven> hints = new List<HintGiven>();
            List<AnswerChecked> checkedAnswers = new List<AnswerChecked>();
            List<StepCompleted> completed = new List<StepCompleted>();
            run.HintGiven += hints.Add;
            run.AnswerChecked += checkedAnswers.Add;
            run.StepCompleted += completed.Add;

            StepFeedback first = run.Answer(ActivityAnswer.Tap(Wrong));
            StepFeedback second = run.Answer(ActivityAnswer.Tap(OtherWrong));
            LessonRunState shown = run.State;
            StepFeedback third = run.Answer(ActivityAnswer.Tap(Wrong));
            StepFeedback right = run.Answer(ActivityAnswer.Tap(Answer));

            Assert.That(first.Outcome, Is.EqualTo(FeedbackOutcome.TryAgain));
            Assert.That(first.Hint.Cue, Is.EqualTo(HintCue.FadeChoice));
            Assert.That(first.Hint.Choice, Is.EqualTo(Wrong));
            Assert.That(SequenceLines.Of(first.First), Is.EqualTo(new[] { "ro-line-listen-again", "ro-sound-t" }));
            Assert.That(second.Outcome, Is.EqualTo(FeedbackOutcome.ShowAnswer));
            Assert.That(second.Hint.Cue, Is.EqualTo(HintCue.GlowAnswer));
            Assert.That(second.Hint.Choice, Is.EqualTo(Answer));
            Assert.That(SequenceLines.Of(second.First), Is.EqualTo(new[] { "ro-line-pick-hint-show" }));
            Assert.That(shown, Is.EqualTo(LessonRunState.AnswerShown));
            Assert.That(third.Outcome, Is.EqualTo(FeedbackOutcome.ShowAnswer));
            Assert.That(right.Outcome, Is.EqualTo(FeedbackOutcome.Done));
            Assert.That(hints.Select(hint => hint.ShowsAnswer), Is.EqualTo(new[] { false, true }));
            Assert.That(hints.Select(hint => hint.MissNumber), Is.EqualTo(new[] { 1, 2 }));
            Assert.That(checkedAnswers.Select(answer => answer.Result), Is.EqualTo(new[]
            {
                AnswerResult.Wrong, AnswerResult.Wrong, AnswerResult.WrongWithAnswerShown, AnswerResult.Practised
            }));
            Assert.That(completed.Single().AnswerWasShown, Is.True);
        }

        /// <summary>Tests that <see cref="LessonRun.Answer"/> takes the tries from the learning rules: with one, the first miss shows the answer.</summary>
        [Test]
        public void TestAnswer_TriesComeFromTheRules()
        {
            LessonRun run = AtThePick(TestLearningRules.WithTries(1));

            StepFeedback miss = run.Answer(ActivityAnswer.Tap(Wrong));

            Assert.That(miss.Outcome, Is.EqualTo(FeedbackOutcome.ShowAnswer));
            Assert.That(run.State, Is.EqualTo(LessonRunState.AnswerShown));
        }

        /// <summary>Tests that <see cref="LessonRun.Answer"/> counts misses per step: a new step starts with none.</summary>
        [Test]
        public void TestAnswer_MissesStartAgainEachStep()
        {
            Curriculum curriculum = RealContent.Read();
            LessonPlan plan = LessonPlan.Of(RealContent.Lesson(curriculum, "party-1"), LanguageMix.Both, new PlayableSteps(Built, curriculum),
                new Step[0], 1);
            LessonRun run = new LessonRun(plan, curriculum, TestLearningRules.Default(), new FixedRandom());
            run.Start();
            run.Answer(ActivityAnswer.Tap(Wrong));
            run.Answer(ActivityAnswer.Tap(Answer));
            run.Next();

            StepFeedback firstMissOfTheNextStep = run.Answer(ActivityAnswer.Tap(Wrong));

            Assert.That(run.Current.Language, Is.EqualTo(Language.English));
            Assert.That(firstMissOfTheNextStep.Outcome, Is.EqualTo(FeedbackOutcome.TryAgain));
        }

        /// <summary>
        /// Tests that <see cref="LessonRun.Answer"/> refuses an answer when no step waits for one, and
        /// <see cref="LessonRun.Next"/> refuses to move on before the step is done.
        /// </summary>
        [Test]
        public void TestAnswer_OnlyWhileAStepPlays()
        {
            LessonRun run = RomanianT(TestLearningRules.Default());

            Assert.That(() => run.Answer(ActivityAnswer.Tap(0)), Throws.InstanceOf<InvalidOperationException>());
            run.Start();
            Assert.That(() => run.Next(), Throws.InstanceOf<InvalidOperationException>());
            run.Answer(ActivityAnswer.Tap(MeetLetterRules.LetterChoice));
            run.Answer(ActivityAnswer.Tap(MeetLetterRules.PictureChoice));
            Assert.That(() => run.Answer(ActivityAnswer.Tap(0)), Throws.InstanceOf<InvalidOperationException>());
        }

        /// <summary>
        /// Tests that <see cref="LessonRun.TryGetResumePoint"/> gives the current step's number, the next
        /// one's once a step is done, and none once the last is done.
        /// </summary>
        [Test]
        public void TestTryGetResumePoint_FollowsTheSteps()
        {
            LessonRun run = RomanianT(TestLearningRules.Default());
            run.Start();

            bool atStart = run.TryGetResumePoint(out int startNumber);
            run.Answer(ActivityAnswer.Tap(MeetLetterRules.LetterChoice));
            run.Answer(ActivityAnswer.Tap(MeetLetterRules.PictureChoice));
            bool afterMeet = run.TryGetResumePoint(out int afterMeetNumber);
            run.Next();
            run.Answer(ActivityAnswer.Tap(Answer));
            bool afterPick = run.TryGetResumePoint(out int _);

            Assert.That(atStart, Is.True);
            Assert.That(startNumber, Is.EqualTo(1));
            Assert.That(afterMeet, Is.True);
            Assert.That(afterMeetNumber, Is.EqualTo(2));
            Assert.That(afterPick, Is.False);
        }

        /// <summary>Tests that a <see cref="LessonRun"/> resumed at step 2 plays only <c>ListenAndPick</c>, and that a review step resumes at the lesson's first step.</summary>
        [Test]
        public void TestTryGetResumePoint_ResumedAndReviewed()
        {
            Curriculum curriculum = RealContent.Read();
            Lesson lesson = RealContent.Lesson(curriculum, "ro-t");
            Step review = RealContent.FirstStep(curriculum, "ro-m", ActivityKind.ListenAndPick);

            LessonRun resumed = new LessonRun(LessonPlan.Of(lesson, LanguageMix.Both, new PlayableSteps(Built, curriculum), new Step[0], 2), curriculum,
                TestLearningRules.Default(), new FixedRandom());
            LessonRun reviewed = new LessonRun(LessonPlan.Of(lesson, LanguageMix.Both, new PlayableSteps(Built, curriculum), new[] { review }, 1), curriculum,
                TestLearningRules.Default(), new FixedRandom());
            resumed.Start();
            reviewed.Start();
            reviewed.TryGetResumePoint(out int duringReview);

            Assert.That(resumed.StepCount, Is.EqualTo(1));
            Assert.That(resumed.Current.Kind, Is.EqualTo(ActivityKind.ListenAndPick));
            Assert.That(reviewed.CurrentStep.IsReview, Is.True);
            Assert.That(duringReview, Is.EqualTo(1));
        }

        /// <summary>Tests that <see cref="LessonRun"/> refuses a plan with no step to play.</summary>
        [Test]
        public void TestLessonRun_NeedsAStep()
        {
            Curriculum curriculum = RealContent.Read();
            LessonPlan empty = LessonPlan.Of(RealContent.Lesson(curriculum, "ro-words-1"), LanguageMix.Both, new PlayableSteps(Built, curriculum),
                new Step[0], 1);

            Assert.That(() => new LessonRun(empty, curriculum, TestLearningRules.Default(), new FixedRandom()),
                Throws.ArgumentException);
        }

        private static int Answer => 0;

        private static int Wrong => 1;

        private static int OtherWrong => 2;

        private static IReadOnlyCollection<ActivityKind> Built => new[] { ActivityKind.MeetLetter, ActivityKind.ListenAndPick };

        private static LessonRun RomanianT(LearningRules rules)
        {
            Curriculum curriculum = RealContent.Read();
            LessonPlan plan = LessonPlan.Of(RealContent.Lesson(curriculum, "ro-t"), LanguageMix.Both, new PlayableSteps(Built, curriculum), new Step[0], 1);
            return new LessonRun(plan, curriculum, rules, new FixedRandom());
        }

        private static LessonRun AtThePick(LearningRules rules)
        {
            LessonRun run = RomanianT(rules);
            run.Start();
            run.Answer(ActivityAnswer.Tap(MeetLetterRules.LetterChoice));
            run.Answer(ActivityAnswer.Tap(MeetLetterRules.PictureChoice));
            run.Next();
            return run;
        }
    }
}
