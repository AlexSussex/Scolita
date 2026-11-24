using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Content;
using Scolita.Core.Activities;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;
using Scolita.Core.Randomness;
using Scolita.Core.Voice;
using Scolita.Game.Profiles;
using Scolita.Game.Saving;
using Scolita.Game.Scenes;
using Scolita.Game.Settings;
using Scolita.Game.Voice;
using UnityEngine;

namespace Scolita.Game.Lessons
{
    /// <summary>
    /// The Lesson scene's composition root (AGENTS.md → "How the app is put together"), the only place
    /// a lesson is wired together. It builds the curriculum and the voice, picks the lesson Go opens
    /// (the one in progress at its step, else the next one: the human, 2026-11-24), builds Core's
    /// <see cref="LessonRun"/> and each kind's view from <see cref="ActivityRegistry"/>, passes her taps
    /// to Core and says what Core decides, saves where she is after each step and when the app goes to
    /// the background, and keeps the screen awake while the lesson is open.
    /// </summary>
    public sealed class LessonRunner : MonoBehaviour, IActivityInput, ILessonScreenInput
    {
        [SerializeField] private DomainCatalog domains;
        [SerializeField] private LetterCatalog letters;
        [SerializeField] private WordCatalog words;
        [SerializeField] private PictureCatalog pictures;
        [SerializeField] private VoiceLineCatalog voiceLines;
        [SerializeField] private VoiceHost voice;
        [SerializeField] private LessonScreenView screen;
        [SerializeField] private ActivityRegistry registry;
        [SerializeField] private LearningRulesAsset learningRules;
        [SerializeField] private Sprite missingPicture;
        [SerializeField] private float secondsAfterAStep = 0.6f;

        private readonly Dictionary<ActivityKind, ActivityView> views = new Dictionary<ActivityKind, ActivityView>();
        private LessonRun run;
        private LessonOrder order;
        private LessonVoice lessonVoice;
        private ActivityViewContext context;
        private SaveStore profileFiles;
        private ActivityView shownView;
        private string lessonSignature;
        private int stepsDoneBefore;
        private bool movingOn;
        private float stepDoneAt;

        /// <summary>The lesson being played; null before it is built, or when there is nothing to play.</summary>
        public LessonRun Run => run;

        /// <summary>The view of the step being played; null between steps.</summary>
        public ActivityView ShownView => shownView;

        /// <summary>The lesson's voice; null before it is built.</summary>
        public LessonVoice LessonVoice => lessonVoice;

        /// <inheritdoc/>
        public void Answer(ActivityAnswer answer)
        {
            if (!WaitsForAnswer())
            {
                return;
            }

            StepFeedback feedback = run.Answer(answer);
            shownView.ShowFeedback(feedback);
            screen.ShowMood(feedback.Outcome == FeedbackOutcome.TryAgain || feedback.Outcome == FeedbackOutcome.ShowAnswer
                ? GuideMood.Thinking
                : GuideMood.Happy);
            lessonVoice.Say(feedback.First, feedback.Then);
            if (feedback.Outcome == FeedbackOutcome.Done)
            {
                movingOn = true;
                stepDoneAt = Time.unscaledTime;
            }
        }

        /// <inheritdoc/>
        public void RepeatPrompt()
        {
            if (WaitsForAnswer())
            {
                lessonVoice.Say(run.Current.Prompt, null);
            }
        }

        /// <inheritdoc/>
        public void RepeatLastLine()
        {
            lessonVoice?.SayAgain();
        }

        /// <inheritdoc/>
        public void Continue()
        {
            SceneNavigator.Load(AppScene.Map);
        }

        private void Start()
        {
            Curriculum curriculum = CurriculumAssets.Build(domains, letters, words, pictures, voiceLines);
            voice.Begin(curriculum);
            profileFiles = SaveStore.ForProfile(Application.persistentDataPath, BuiltInProfiles.FirstChild);
            LanguageMix mix = profileFiles.Load(LearningSettingsData.FileName, LearningSettingsData.CreateDefaults()).Data.LanguageMix;
            IReadOnlyCollection<ActivityKind> playable = PlayableKinds();
            if (curriculum.Domains.Count == 0)
            {
                Debug.LogError("Lesson: the content has no domain, so there is no lesson to play.");
                return;
            }

            order = new LessonOrder(curriculum.Domains[0], mix, playable);
            LessonPlan plan = ChoosePlan();
            if (plan == null)
            {
                Debug.LogError("Lesson: no lesson has a step that can be played yet.");
                return;
            }

            Build(curriculum, plan);
            run.Start();
            ShowStep();
        }

        private void Update()
        {
            if (run == null)
            {
                return;
            }

            lessonVoice.Tick();
            screen.SetTalking(lessonVoice.IsSpeaking);
            if (movingOn && !lessonVoice.IsBusy && Time.unscaledTime >= stepDoneAt + secondsAfterAStep)
            {
                movingOn = false;
                run.Next();
                if (run.State != LessonRunState.Finished)
                {
                    ShowStep();
                }
            }
        }

        private void OnEnable()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        private void OnDisable()
        {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }

        private void OnApplicationPause(bool paused)
        {
            if (run == null)
            {
                return;
            }

            if (paused)
            {
                SaveWhereSheIs();
                lessonVoice.Pause();
            }
            else
            {
                lessonVoice.Resume();
            }
        }

        private void OnDestroy()
        {
            if (run == null)
            {
                return;
            }

            run.StepCompleted -= OnStepCompleted;
            run.LessonCompleted -= OnLessonCompleted;
            voice.PartStarted -= OnPartStarted;
        }

        private IReadOnlyCollection<ActivityKind> PlayableKinds()
        {
            IReadOnlyList<ActivityKind> withViews = registry.KindsWithViews();
            ActivityKind[] playable = ActivityRulesFactory.Kinds
                .Where(kind => withViews.Contains(kind))
                .ToArray();
            if (registry.NotBuiltYet.Count > 0)
            {
                Debug.Log("Lesson: the steps of the kinds not built yet are skipped: " + string.Join(", ", registry.NotBuiltYet) + ".");
            }

            return playable;
        }

        private LessonPlan ChoosePlan()
        {
            LessonRunData saved = profileFiles.Load(LessonRunData.FileName, LessonRunData.CreateDefaults()).Data;
            saved.TryGetLessonId(out ContentId savedId);
            LessonResume resume = LessonResume.Choose(order, savedId, saved.StepNumber, saved.StepsSignature);
            if (resume == null)
            {
                return null;
            }

            if (resume.Reason == ResumeReason.StepsChanged || resume.Reason == ResumeReason.PastTheEnd)
            {
                Debug.LogWarning("Lesson: " + resume.Plan.Lesson.Id + " starts again (" + resume.Reason + ").");
            }

            stepsDoneBefore = resume.StepsDoneBefore;
            return resume.Plan;
        }

        private void Build(Curriculum curriculum, LessonPlan plan)
        {
            LearningRules rules = learningRules.ToRules();
            run = new LessonRun(plan, curriculum, rules, new SeededRandom((ulong)DateTime.UtcNow.Ticks));
            lessonSignature = LessonSignature.Of(plan.Lesson);
            voice.LoadLines(LessonVoiceLines.Of(curriculum, plan.Lesson));
            lessonVoice = new LessonVoice(voice.Voice, screen.Bubble);
            lessonVoice.Prepare(curriculum, run.Lines);
            context = new ActivityViewContext(this, new PictureSprites(pictures, missingPicture), rules);
            foreach (PlannedStep step in plan.Steps)
            {
                ActivityKind kind = step.Step.Kind;
                if (!views.ContainsKey(kind) && registry.TryGetView(kind, out ActivityView prefab))
                {
                    ActivityView view = Instantiate(prefab, screen.ActivityArea);
                    view.Hide();
                    views.Add(kind, view);
                }
            }

            run.StepCompleted += OnStepCompleted;
            run.LessonCompleted += OnLessonCompleted;
            voice.PartStarted += OnPartStarted;
            screen.Bind(this);
        }

        private void ShowStep()
        {
            ActivityView view = views[run.Current.Kind];
            if (shownView != null && shownView != view)
            {
                shownView.Hide();
            }

            shownView = view;
            view.Show(run.Current.Round, context);
            screen.ShowProgress(stepsDoneBefore + run.StepIndex, stepsDoneBefore + run.StepCount);
            screen.ShowMood(GuideMood.Happy);
            lessonVoice.Say(run.Current.Instruction, null);
            SaveWhereSheIs();
        }

        private bool WaitsForAnswer()
        {
            return run != null && (run.State == LessonRunState.Playing || run.State == LessonRunState.AnswerShown);
        }

        private void OnStepCompleted(StepCompleted completed)
        {
            SaveWhereSheIs();
        }

        private void OnLessonCompleted(LessonCompleted completed)
        {
            if (shownView != null)
            {
                shownView.Hide();
                shownView = null;
            }
            screen.ShowProgress(stepsDoneBefore + run.StepCount, stepsDoneBefore + run.StepCount);
            screen.ShowMood(GuideMood.Cheering);
            screen.ShowEnd();
            lessonVoice.Say(run.Ending, null);
        }

        private void OnPartStarted(VoiceUtterance part)
        {
            lessonVoice.PartStarted();
        }

        private void SaveWhereSheIs()
        {
            Lesson lesson = run.Plan.Lesson;
            LessonRunData data;
            if (run.TryGetResumePoint(out int stepNumber))
            {
                data = new LessonRunData(lesson.Id, stepNumber, lessonSignature);
            }
            else
            {
                Lesson next = order.After(lesson.Id);
                data = new LessonRunData(next.Id, 1, LessonSignature.Of(next));
            }

            if (!profileFiles.Save(LessonRunData.FileName, data))
            {
                Debug.LogWarning("Lesson: where she is could not be saved; the lesson starts from its beginning next time.");
            }
        }
    }
}
