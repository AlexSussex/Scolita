using Scolita.Content;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;
using Scolita.Core.Map;
using Scolita.Core.Parent;
using Scolita.Core.Progress;
using Scolita.Core.Timing;
using Scolita.Game.Lessons;
using Scolita.Game.Profiles;
using Scolita.Game.Progress;
using Scolita.Game.Saving;
using Scolita.Game.Scenes;
using Scolita.Game.Settings;
using Scolita.Game.Timing;
using UnityEngine;

namespace Scolita.Game.Parent
{
    /// <summary>
    /// The Parent scene's composition root (AGENTS.md → "How the app is put together"), reached only through the
    /// parent gate on the map: it wakes the rocket (today's play starts again), builds the report of her progress
    /// (<see cref="ParentReport"/>) and shows it with the settings; every change is saved at once (the profile's
    /// learning settings, the tablet's settings; one that can't be saved stays as it was, and the screen shows it so)
    /// and the map reads it when it opens again. Reset asks twice, then deletes her progress (<see cref="ProgressReset"/>),
    /// keeping the settings, and says whether it could. Back and Android Back go back to the
    /// map, or say no to a reset question.
    /// </summary>
    public sealed class ParentRunner : MonoBehaviour, IParentAreaInput
    {
        [SerializeField] private DomainCatalog domains;
        [SerializeField] private LetterCatalog letters;
        [SerializeField] private WordCatalog words;
        [SerializeField] private PictureCatalog pictures;
        [SerializeField] private VoiceLineCatalog voiceLines;
        [SerializeField] private LetterShapeCatalog shapes;
        [SerializeField] private ActivityRegistry registry;
        [SerializeField] private LearningRulesAsset learningRules;
        [SerializeField] private ParentAreaView screen;

        private Curriculum curriculum;
        private ParentRules rules;
        private IClock clock;
        private SaveStore tabletFiles;
        private SaveStore profileFiles;
        private TabletSettingsData tablet;
        private LearningSettingsData learning;
        private ResetQuestion question;
        private bool leaving;

        /// <summary>The report shown now; null before the area is built.</summary>
        public ParentReport Report { get; private set; }

        /// <summary>The reset question asked now.</summary>
        public ResetQuestion Question => question;

        /// <inheritdoc/>
        public void Back()
        {
            if (question != ResetQuestion.None)
            {
                Ask(ResetQuestion.None);
                return;
            }

            if (leaving)
            {
                return;
            }

            leaving = true;
            SceneNavigator.Load(AppScene.Map);
        }

        /// <inheritdoc/>
        public void ChooseMix(LanguageMix mix)
        {
            SaveLearning(learning.WithLanguageMix(mix));
            Redraw();
        }

        /// <inheritdoc/>
        public void ChooseLimit(int minutes)
        {
            SaveLearning(learning.WithSessionLimit(SessionLimit.Of(minutes, rules)));
            Redraw();
        }

        /// <inheritdoc/>
        public void SetVoiceVolume(float volume)
        {
            SaveTablet(new TabletSettingsData(tablet.MusicOn, tablet.SoundsOn, volume, tablet.VibrationOn,
                tablet.AppLanguage));
            Redraw();
        }

        /// <inheritdoc/>
        public void SetMenusInEnglish(bool english)
        {
            Language language = english ? Language.English : Language.Romanian;
            SaveTablet(new TabletSettingsData(tablet.MusicOn, tablet.SoundsOn, tablet.VoiceVolume, tablet.VibrationOn,
                language));
            AppLocale.Select(tablet.AppLanguage);
            Redraw();
        }

        /// <inheritdoc/>
        public void AskReset()
        {
            Ask(ResetQuestion.First);
        }

        /// <inheritdoc/>
        public void AnswerReset(bool yes)
        {
            if (!yes || question == ResetQuestion.None)
            {
                Ask(ResetQuestion.None);
                return;
            }

            if (question == ResetQuestion.First)
            {
                Ask(ResetQuestion.Second);
                return;
            }

            Ask(ResetQuestion.None);
            bool deleted = ProgressReset.Run(profileFiles);
            Redraw();
            screen.ShowResetDone(deleted);
        }

        private void Start()
        {
            curriculum = CurriculumAssets.Build(domains, letters, words, pictures, voiceLines, shapes);
            rules = learningRules.ToParentRules();
            clock = new SystemClock();
            string saveRoot = Application.persistentDataPath;
            tabletFiles = SaveStore.ForTablet(saveRoot);
            tablet = tabletFiles.Load(TabletSettingsData.FileName, TabletSettingsData.CreateDefaults()).Data;
            profileFiles = SaveStore.ForProfile(saveRoot, BuiltInProfiles.FirstChild);
            learning = profileFiles.Load(LearningSettingsData.FileName, LearningSettingsData.CreateDefaults()).Data;
            AppLocale.Select(tablet.AppLanguage);
            new PlayClock(profileFiles, clock).Wake();
            if (curriculum.Domains.Count == 0)
            {
                Debug.LogError("Parent area: the content has no domain, so there is no progress to show.");
                return;
            }

            screen.Bind(this);
            Redraw();
        }

        private void Update()
        {
            if (AndroidBack.WasPressed())
            {
                Back();
            }
        }

        private void SaveLearning(LearningSettingsData changed)
        {
            if (profileFiles.Save(LearningSettingsData.FileName, changed))
            {
                learning = changed;
                return;
            }

            Debug.LogWarning("Parent area: the learning settings could not be saved; they stay as they were.");
        }

        private void SaveTablet(TabletSettingsData changed)
        {
            if (tabletFiles.Save(TabletSettingsData.FileName, changed))
            {
                tablet = changed;
                return;
            }

            Debug.LogWarning("Parent area: the tablet's settings could not be saved; they stay as they were.");
        }

        private void Ask(ResetQuestion asked)
        {
            question = asked;
            screen.AskReset(asked);
        }

        private void Redraw()
        {
            Domain domain = curriculum.Domains[0];
            LessonOrder order = new LessonOrder(domain, learning.LanguageMix,
                new PlayableSteps(registry.PlayableKinds(), curriculum));
            LearnerProgress progress = ProgressFile.Load(profileFiles, curriculum, order);
            Report = new ParentReport(curriculum, domain, new MapRoute(curriculum, domain, order, progress), progress,
                clock.Today);
            ParentSettingsShown settings = new ParentSettingsShown(learning.LanguageMix, learning.SessionLimitBy(rules),
                rules.SessionLimitMinutes, tablet.VoiceVolume, tablet.AppLanguage);
            screen.Show(Report, settings, Application.version);
        }
    }
}
