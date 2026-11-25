using System;
using System.Collections.Generic;
using Scolita.Core.Content;
using Scolita.Core.Lessons;
using Scolita.Core.Timing;

namespace Scolita.Core.Progress
{
    /// <summary>
    /// Records what a lesson run does into her <see cref="LearnerProgress"/> (PRD §3.5), listening to the run's
    /// events: each step done is one right answer for each item it practises (<see cref="PractisedItems"/>), of
    /// its kind, today, or practice when its answer was shown; a <c>MeetLetter</c> step meets its letter; a
    /// review step also moves its item on the review schedule, or back after a miss; the day is a day played;
    /// the lesson finished is done. Everything is recorded when a step ends, never during one, and
    /// <see cref="Recorded"/> then tells the Unity side to save.
    /// </summary>
    public sealed class LessonProgress
    {
        private readonly LessonRun run;
        private readonly LearnerProgress progress;
        private readonly ProgressRules rules;
        private readonly IClock clock;
        private readonly IReadOnlyList<LearningItem>[] itemsOfStep;
        private int missedStep = -1;

        /// <summary>
        /// Starts recording <paramref name="run"/> into <paramref name="learner"/>, its items found in
        /// <paramref name="curriculum"/>, by <paramref name="progressRules"/>, the days from
        /// <paramref name="today"/>. Called by the Lesson scene's composition root and the tests.
        /// </summary>
        public LessonProgress(LessonRun run, LearnerProgress learner, Curriculum curriculum, ProgressRules progressRules,
            IClock today)
        {
            this.run = run ?? throw new ArgumentNullException(nameof(run));
            progress = learner ?? throw new ArgumentNullException(nameof(learner));
            rules = progressRules ?? throw new ArgumentNullException(nameof(progressRules));
            clock = today ?? throw new ArgumentNullException(nameof(today));
            itemsOfStep = new IReadOnlyList<LearningItem>[run.StepCount];
            for (int index = 0; index < itemsOfStep.Length; index++)
            {
                itemsOfStep[index] = PractisedItems.Of(run.Plan.Steps[index].Step, curriculum);
            }

            run.AnswerChecked += OnAnswerChecked;
            run.StepCompleted += OnStepCompleted;
            run.LessonCompleted += OnLessonCompleted;
        }

        /// <summary>Raised after a step or the lesson was recorded, so her progress is saved.</summary>
        public event Action Recorded;

        /// <summary>
        /// Stops recording (the run's events are let go). Called by the Lesson scene's composition root when
        /// the scene closes, and the tests.
        /// </summary>
        public void Stop()
        {
            run.AnswerChecked -= OnAnswerChecked;
            run.StepCompleted -= OnStepCompleted;
            run.LessonCompleted -= OnLessonCompleted;
        }

        private void OnAnswerChecked(AnswerChecked answer)
        {
            if (answer.Result == AnswerResult.Wrong || answer.Result == AnswerResult.WrongWithAnswerShown)
            {
                missedStep = answer.StepIndex;
            }
        }

        private void OnStepCompleted(StepCompleted completed)
        {
            CalendarDay today = clock.Today;
            Step step = completed.Step.Step;
            bool missed = missedStep == completed.StepIndex;
            IReadOnlyList<LearningItem> items = itemsOfStep[completed.StepIndex];
            for (int index = 0; index < items.Count; index++)
            {
                LearningItem item = items[index];
                RecordItem(item, step.Kind, completed.AnswerWasShown, today);
                if (completed.Step.IsReview)
                {
                    progress.RecordReview(item, missed || completed.AnswerWasShown, today, rules);
                }
            }

            progress.RecordDayPlayed(today);
            Recorded?.Invoke();
        }

        private void RecordItem(LearningItem item, ActivityKind kind, bool answerShown, CalendarDay today)
        {
            if (!PractisedItems.IsAnswered(kind))
            {
                progress.Meet(item);
            }
            else if (answerShown)
            {
                progress.RecordPractised(item);
            }
            else
            {
                progress.RecordRight(item, kind, today, rules);
            }
        }

        private void OnLessonCompleted(LessonCompleted completed)
        {
            progress.Lessons.MarkDone(completed.Lesson.Id);
            Recorded?.Invoke();
        }
    }
}
