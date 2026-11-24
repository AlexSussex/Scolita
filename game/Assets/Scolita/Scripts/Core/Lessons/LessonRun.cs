using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Activities;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Randomness;
using Scolita.Core.Voice;

namespace Scolita.Core.Lessons
{
    /// <summary>
    /// One lesson being played (AGENTS.md → "How the app is put together"): it steps through a
    /// <see cref="LessonPlan"/>'s steps, each run by its <see cref="IActivityRules"/>, counts the
    /// misses of each part and applies the one ladder for every kind (the first miss gets a hint, the
    /// miss that reaches <see cref="LearningRules.TriesBeforeAnswerShown"/> shows the answer, and a
    /// part done with the answer shown is practice), and raises its events. Everything it can say is
    /// prepared when it is built; an answer and its events allocate nothing.
    /// </summary>
    public sealed class LessonRun
    {
        private readonly IActivityRules[] rules;
        private readonly LearningRules learningRules;
        private readonly IRandom random;
        private readonly PraisePicker praise;
        private int missesInPart;
        private bool answerShownInStep;

        /// <summary>
        /// Builds the run of <paramref name="plan"/>: each step's rules from
        /// <paramref name="curriculum"/>, and its praise and its end. Throws an
        /// <see cref="ArgumentException"/> for a plan with no step. Called by the Lesson scene's
        /// composition root and the tests.
        /// </summary>
        public LessonRun(LessonPlan plan, Curriculum curriculum, LearningRules learningRules, IRandom random)
        {
            if (plan.Steps.Count == 0)
            {
                throw new ArgumentException("The lesson " + plan.Lesson.Id + " has no step to play.", nameof(plan));
            }

            Plan = plan;
            this.learningRules = learningRules;
            this.random = random;
            praise = new PraisePicker(curriculum, learningRules, random);
            rules = new IActivityRules[plan.Steps.Count];
            for (int index = 0; index < rules.Length; index++)
            {
                rules[index] = ActivityRulesFactory.Create(plan.Steps[index].Step, curriculum);
            }

            Language endLanguage = rules[rules.Length - 1].Language;
            Ending = VoiceSequence.Of(LessonLines.LessonDone(endLanguage));
            EndingLanguage = endLanguage;
            List<VoiceSequence> lines = new List<VoiceSequence>(rules.SelectMany(step => step.Lines));
            lines.AddRange(praise.Lines);
            lines.Add(Ending);
            Lines = lines;
            State = LessonRunState.NotStarted;
        }

        /// <summary>Raised for each answer that is right, wrong or practised.</summary>
        public event Action<AnswerChecked> AnswerChecked;

        /// <summary>Raised for each hint, and when an answer is shown.</summary>
        public event Action<HintGiven> HintGiven;

        /// <summary>Raised when a step is done.</summary>
        public event Action<StepCompleted> StepCompleted;

        /// <summary>Raised when the last step is done and <see cref="Next"/> is called.</summary>
        public event Action<LessonCompleted> LessonCompleted;

        /// <summary>The plan it plays.</summary>
        public LessonPlan Plan { get; }

        /// <summary>Where it is.</summary>
        public LessonRunState State { get; private set; }

        /// <summary>The current step's place in the plan, from 0 (the step dots).</summary>
        public int StepIndex { get; private set; }

        /// <summary>How many steps it plays (the step dots).</summary>
        public int StepCount => rules.Length;

        /// <summary>The current step.</summary>
        public PlannedStep CurrentStep => Plan.Steps[StepIndex];

        /// <summary>The current step's rules (its round, its lines).</summary>
        public IActivityRules Current => rules[StepIndex];

        /// <summary>What is said when the lesson is finished.</summary>
        public VoiceSequence Ending { get; }

        /// <summary>The language of <see cref="Ending"/> (the last step's).</summary>
        public Language EndingLanguage { get; }

        /// <summary>
        /// Every line the run can say (each step's, the praise and the end), so the Unity side prepares
        /// their bubble words when the lesson is built.
        /// </summary>
        public IReadOnlyList<VoiceSequence> Lines { get; }

        /// <summary>
        /// Begins the first step. Throws an <see cref="InvalidOperationException"/> if it was started
        /// already. Called by the Lesson scene's composition root and the tests.
        /// </summary>
        public void Start()
        {
            if (State != LessonRunState.NotStarted)
            {
                throw new InvalidOperationException("A lesson run starts once.");
            }

            BeginStep(0);
        }

        /// <summary>
        /// Judges <paramref name="answer"/> in the current step and returns what she gets; raises
        /// <see cref="AnswerChecked"/>, <see cref="HintGiven"/> and <see cref="StepCompleted"/> as
        /// they happen. Throws an <see cref="InvalidOperationException"/> when no step waits for an
        /// answer (the view takes no taps then). Called by the Lesson scene's composition root and the tests.
        /// </summary>
        public StepFeedback Answer(in ActivityAnswer answer)
        {
            if (State != LessonRunState.Playing && State != LessonRunState.AnswerShown)
            {
                throw new InvalidOperationException("No step waits for an answer (the run is " + State + ").");
            }

            IActivityRules step = rules[StepIndex];
            AnswerVerdict verdict = step.Judge(answer);
            switch (verdict)
            {
                case AnswerVerdict.Neutral:
                    return new StepFeedback(FeedbackOutcome.Neutral, answer.Choice, NoHint, step.Response, null);
                case AnswerVerdict.Wrong:
                    return Missed(step, answer);
                case AnswerVerdict.Right:
                    PartDone();
                    return new StepFeedback(FeedbackOutcome.Right, answer.Choice, NoHint, step.Response, null);
                case AnswerVerdict.Done:
                    PartDone();
                    State = LessonRunState.StepDone;
                    StepCompleted?.Invoke(new StepCompleted(StepIndex, CurrentStep, answerShownInStep));
                    return new StepFeedback(FeedbackOutcome.Done, answer.Choice, NoHint, step.Response,
                        praise.Next(step.Language));
                default:
                    throw new InvalidOperationException("Unknown verdict " + verdict + ".");
            }
        }

        /// <summary>
        /// Moves on once a step is done (its praise heard): begins the next step, or finishes the
        /// lesson and raises <see cref="LessonCompleted"/>. Throws an <see cref="InvalidOperationException"/>
        /// when the step isn't done. Called by the Lesson scene's composition root and the tests.
        /// </summary>
        public void Next()
        {
            if (State != LessonRunState.StepDone)
            {
                throw new InvalidOperationException("Next comes after a step is done (the run is " + State + ").");
            }

            if (StepIndex + 1 < rules.Length)
            {
                BeginStep(StepIndex + 1);
                return;
            }

            State = LessonRunState.Finished;
            LessonCompleted?.Invoke(new LessonCompleted(Plan.Lesson));
        }

        /// <summary>
        /// The lesson's own step number to resume at if she leaves now (the current step, or the next
        /// one once it is done; a review step resumes at the lesson's first step to play); false when
        /// the lesson's steps are all done. Called by the Lesson scene's composition root (saving) and
        /// the tests.
        /// </summary>
        public bool TryGetResumePoint(out int stepNumber)
        {
            int from = State == LessonRunState.StepDone || State == LessonRunState.Finished ? StepIndex + 1 : StepIndex;
            for (int index = from; index < rules.Length; index++)
            {
                if (!Plan.Steps[index].IsReview)
                {
                    stepNumber = Plan.Steps[index].Step.Number;
                    return true;
                }
            }

            stepNumber = 0;
            return false;
        }

        private static ActivityHint NoHint => new ActivityHint(null, HintCue.None, -1);

        private void BeginStep(int index)
        {
            StepIndex = index;
            missesInPart = 0;
            answerShownInStep = false;
            State = LessonRunState.Playing;
            rules[index].Begin(random);
        }

        private StepFeedback Missed(IActivityRules step, in ActivityAnswer answer)
        {
            missesInPart++;
            AnswerResult result = State == LessonRunState.AnswerShown ? AnswerResult.WrongWithAnswerShown : AnswerResult.Wrong;
            AnswerChecked?.Invoke(new AnswerChecked(StepIndex, CurrentStep, result, missesInPart));
            if (State == LessonRunState.AnswerShown)
            {
                ActivityHint again = step.AnswerShown();
                return new StepFeedback(FeedbackOutcome.ShowAnswer, answer.Choice, again, again.Line, null);
            }

            if (missesInPart >= learningRules.TriesBeforeAnswerShown)
            {
                State = LessonRunState.AnswerShown;
                answerShownInStep = true;
                ActivityHint shown = step.AnswerShown();
                HintGiven?.Invoke(new HintGiven(StepIndex, CurrentStep, missesInPart, shown));
                return new StepFeedback(FeedbackOutcome.ShowAnswer, answer.Choice, shown, shown.Line, null);
            }

            ActivityHint hint = step.HintFor(missesInPart, answer);
            HintGiven?.Invoke(new HintGiven(StepIndex, CurrentStep, missesInPart, hint));
            return new StepFeedback(FeedbackOutcome.TryAgain, answer.Choice, hint, hint.Line, null);
        }

        private void PartDone()
        {
            AnswerResult result = State == LessonRunState.AnswerShown ? AnswerResult.Practised : AnswerResult.Right;
            AnswerChecked?.Invoke(new AnswerChecked(StepIndex, CurrentStep, result, missesInPart));
            missesInPart = 0;
            State = LessonRunState.Playing;
        }
    }
}
