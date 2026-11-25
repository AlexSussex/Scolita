using System;
using NUnit.Framework;
using Scolita.Core.Activities;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;
using Scolita.Core.Progress;
using Scolita.Core.Timing;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Progress
{
    /// <summary>Tests <see cref="LessonProgress"/>: real lessons played through <see cref="LessonRun"/>, recorded into her progress.</summary>
    public sealed class LessonProgressTests
    {
        /// <summary>
        /// Tests that <see cref="LessonProgress"/> records each step done without its answer shown as one right answer of its
        /// kind for its item, today, the day as played, the lesson as done, and says so after each.
        /// </summary>
        [Test]
        public void TestLessonProgress_StepsDoneAreRightAnswers()
        {
            Curriculum curriculum = RealContent.Read();
            FakeClock clock = new FakeClock(CalendarDay.FromDate(2026, 11, 25));
            LearnerProgress progress = Progresses.Empty();
            LessonRun run = Run(curriculum, "ro-t", new Step[0]);
            LessonProgress recorder = new LessonProgress(run, progress, curriculum, TestProgressRules.Default(), clock);
            int recorded = 0;
            recorder.Recorded += () => recorded++;

            run.Start();
            AnswerRight(run);
            run.Next();
            AnswerRight(run);
            run.Next();

            Assert.That(progress.TryGet(Letter("t"), out ItemProgress t), Is.True);
            Assert.That(t.RightsOf(ActivityKind.ListenAndPick), Is.EqualTo(1));
            Assert.That(t.RightsOf(ActivityKind.FirstSound), Is.EqualTo(1));
            Assert.That(t.RightDays, Is.EqualTo(new[] { clock.Today }));
            Assert.That(progress.DaysPlayed, Is.EqualTo(new[] { clock.Today }));
            Assert.That(progress.IsDone(ContentId.Create("ro-t")), Is.True);
            Assert.That(recorded, Is.EqualTo(3));
        }

        /// <summary>Tests that <see cref="LessonProgress"/> records a step done with its answer shown as practice, never as right.</summary>
        [Test]
        public void TestLessonProgress_AnswerShownIsPractice()
        {
            Curriculum curriculum = RealContent.Read();
            LearnerProgress progress = Progresses.Empty();
            LessonRun run = Run(curriculum, "ro-t", new Step[0]);
            LessonProgress recorder = new LessonProgress(run, progress, curriculum, TestProgressRules.Default(),
                new FakeClock(CalendarDay.FromDate(2026, 11, 25)));

            run.Start();
            AnswerWrong(run);
            AnswerWrong(run);
            AnswerRight(run);
            recorder.Stop();

            progress.TryGet(Letter("t"), out ItemProgress t);
            Assert.That(t.Practised, Is.EqualTo(1));
            Assert.That(t.TotalRights, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests that <see cref="LessonProgress"/> moves a warm-up's item one step on the schedule when it is done without a
        /// miss, and back after a miss (then right with the hint).
        /// </summary>
        [Test]
        public void TestLessonProgress_AWarmUpMovesTheSchedule()
        {
            Curriculum curriculum = RealContent.Read();
            FakeClock clock = new FakeClock(CalendarDay.FromDate(2026, 11, 25));
            LearnerProgress progress = Progresses.Learned(clock.Today.AddDays(-1), Language.Romanian, "a", "m");
            Step warmUpA = WarmUp("a", "m");
            Step warmUpM = WarmUp("m", "a");
            LessonRun run = Run(curriculum, "ro-t", new[] { warmUpA, warmUpM });
            LessonProgress recorder = new LessonProgress(run, progress, curriculum, TestProgressRules.Default(), clock);

            run.Start();
            AnswerRight(run);
            run.Next();
            AnswerWrong(run);
            AnswerRight(run);
            recorder.Stop();

            progress.TryGet(Letter("a"), out ItemProgress a);
            progress.TryGet(Letter("m"), out ItemProgress m);
            Assert.That(a.ReviewStep, Is.EqualTo(1));
            Assert.That(a.NextReview, Is.EqualTo(clock.Today.AddDays(3)));
            Assert.That(m.ReviewStep, Is.EqualTo(0));
            Assert.That(m.NextReview, Is.EqualTo(clock.Today.AddDays(1)));
        }

        /// <summary>Tests that <see cref="LessonProgress"/> records a <c>MeetLetter</c> step as meeting its letter, with no right answer.</summary>
        [Test]
        public void TestLessonProgress_MeetLetterMeets()
        {
            Curriculum curriculum = RealContent.Read();
            LearnerProgress progress = Progresses.Empty();
            Lesson lesson = RealContent.Lesson(curriculum, "ro-m");
            LessonPlan plan = LessonPlan.Of(lesson, LanguageMix.Both,
                new PlayableSteps(new[] { ActivityKind.MeetLetter }, curriculum), new Step[0], 1);
            LessonRun run = new LessonRun(plan, curriculum, TestLearningRules.Default(), new FixedRandom());
            LessonProgress recorder = new LessonProgress(run, progress, curriculum, TestProgressRules.Default(),
                new FakeClock(CalendarDay.FromDate(2026, 11, 25)));

            run.Start();
            run.Answer(ActivityAnswer.Tap(MeetLetterRules.LetterChoice));
            run.Answer(ActivityAnswer.Tap(MeetLetterRules.PictureChoice));
            recorder.Stop();

            Assert.That(progress.StateOf(Letter("m")), Is.EqualTo(ItemState.Learning));
            Assert.That(progress.Items[0].TotalRights, Is.EqualTo(0));
            Assert.That(progress.Items[0].Practised, Is.EqualTo(0));
        }

        private static LessonRun Run(Curriculum curriculum, string lessonId, Step[] warmUps)
        {
            Lesson lesson = RealContent.Lesson(curriculum, lessonId);
            PlayableSteps playable = new PlayableSteps(new[] { ActivityKind.ListenAndPick, ActivityKind.FirstSound }, curriculum);
            LessonPlan plan = LessonPlan.Of(lesson, LanguageMix.Both, playable, warmUps, 1);
            return new LessonRun(plan, curriculum, TestLearningRules.Default(), new FixedRandom());
        }

        private static Step WarmUp(params string[] letters)
        {
            return new Step(ContentId.Create("ro-t"), 0, ActivityKind.ListenAndPick, LanguageMix.RomanianOnly,
                Array.ConvertAll(letters, ContentId.Create), new ContentId[0], new StepOption[0], new ContentSource(WarmUps.SourceName, 0));
        }

        private static void AnswerRight(LessonRun run)
        {
            run.Answer(ActivityAnswer.Tap(run.Current.Round.Answer));
        }

        private static void AnswerWrong(LessonRun run)
        {
            int wrong = run.Current.Round.Answer == 0 ? 1 : 0;
            run.Answer(ActivityAnswer.Tap(wrong));
        }

        private static LearningItem Letter(string id)
        {
            return LearningItem.Letter(ContentId.Create(id), Language.Romanian);
        }
    }
}
