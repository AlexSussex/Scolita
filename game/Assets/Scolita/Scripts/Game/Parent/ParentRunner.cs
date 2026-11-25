using System.Collections.Generic;
using System.Linq;
using Scolita.Content;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;
using Scolita.Core.Map;
using Scolita.Core.Parent;
using Scolita.Core.Profiles;
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
    /// parent gate (on the map or "Who's playing?"): it wakes the rocket of the profile playing (today's play starts
    /// again) and shows the <b>chosen</b> profile (at first the one to open, <see cref="ProfileList.ProfileToOpen"/>): the
    /// report of its progress (<see cref="ParentReport"/>) and its settings, every change saved at once (the profile's
    /// learning settings, the tablet's settings; one that can't be saved stays as it was, and the screen shows it so).
    /// Reset asks twice, then deletes the chosen profile's progress (<see cref="ProgressReset"/>), keeping the settings.
    /// The profiles page (design screen 10) lists every profile, chooses one, adds a child, changes a profile, deletes a
    /// child after two questions (the list saved first, then only that folder deleted), and makes, opens and resets
    /// the tester's profile. Back and Android Back say no to a question, close the page, then go back: to the map of the
    /// one playing (or of the only child), or to "Who's playing?" when nobody is and there are two or more children.
    /// </summary>
    public sealed class ParentRunner : MonoBehaviour, IParentAreaInput, IParentProfilesInput
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
        [SerializeField] private ParentProfilesView profilesPage;
        [SerializeField] private AvatarCatalog avatars;

        private Curriculum curriculum;
        private ParentRules rules;
        private IClock clock;
        private SaveStore tabletFiles;
        private SaveStore profileFiles;
        private TabletSettingsData tablet;
        private LearningSettingsData learning;
        private ResetQuestion question;
        private ProfileStore profiles;
        private ProfileList list;
        private Profile chosen;
        private bool editingNew;
        private string typedName;
        private ContentId editedAvatar;
        private LanguageMix editedMix;
        private ResetQuestion deleteQuestion;
        private bool leaving;

        /// <summary>The report shown now; null before the area is built.</summary>
        public ParentReport Report { get; private set; }

        /// <summary>The reset question asked now.</summary>
        public ResetQuestion Question => question;

        /// <summary>The deletion question asked now.</summary>
        public ResetQuestion DeleteQuestion => deleteQuestion;

        /// <summary>The profile the area shows; null before the area is built.</summary>
        public Profile Chosen => chosen;

        /// <summary>The profile list as the area has it; null before the area is built.</summary>
        public ProfileList Profiles => list;

        /// <inheritdoc/>
        public void Back()
        {
            if (question != ResetQuestion.None)
            {
                Ask(ResetQuestion.None);
                return;
            }

            if (profilesPage.IsOpen)
            {
                CloseThePage();
                return;
            }

            LeaveTheArea();
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

        /// <inheritdoc/>
        public void OpenProfiles()
        {
            profilesPage.SetOpen(true);
            profilesPage.ShowMessage(ProfilesMessage.None);
            BeginEditing(chosen);
            DrawThePage(true);
        }

        /// <inheritdoc/>
        public void CloseProfiles()
        {
            if (deleteQuestion != ResetQuestion.None)
            {
                AskDeleting(ResetQuestion.None);
                return;
            }

            CloseThePage();
        }

        /// <inheritdoc/>
        public void ChooseProfile(ProfileId profile)
        {
            if (deleteQuestion != ResetQuestion.None || !list.TryGet(profile, out Profile found))
            {
                return;
            }

            Choose(found);
            profilesPage.ShowMessage(ProfilesMessage.None);
            BeginEditing(found);
            DrawThePage(true);
        }

        /// <inheritdoc/>
        public void StartNewChild()
        {
            if (deleteQuestion != ResetQuestion.None || !list.CanAddChild)
            {
                return;
            }

            editingNew = true;
            typedName = string.Empty;
            editedAvatar = list.FirstFreeAvatar(profiles.Avatars);
            editedMix = LanguageMix.Both;
            profilesPage.ShowMessage(ProfilesMessage.None);
            DrawThePage(true);
        }

        /// <inheritdoc/>
        public void TypeName(string typed)
        {
            typedName = typed ?? string.Empty;
            profilesPage.ShowEditor(Editor(), false);
        }

        /// <inheritdoc/>
        public void ChooseAvatar(ContentId avatar)
        {
            if (list.IsAvatarFree(avatar, EditedId()))
            {
                editedAvatar = avatar;
                profilesPage.ShowEditor(Editor(), false);
            }
        }

        /// <inheritdoc/>
        public void ChooseEditedMix(LanguageMix mix)
        {
            editedMix = mix;
            profilesPage.ShowEditor(Editor(), false);
        }

        /// <inheritdoc/>
        public void SaveProfile()
        {
            if (ProfileName.Read(typedName, profiles.Rules, out ProfileName name) != ProfileNameProblem.None
                || !list.IsAvatarFree(editedAvatar, EditedId()))
            {
                profilesPage.ShowEditor(Editor(), false);
                return;
            }

            Profile saved = editingNew ? list.AddChild(name, editedAvatar, clock.Today) : list.Change(chosen.Id, name, editedAvatar);
            if (!profiles.Save(list))
            {
                list = profiles.Load();
                profilesPage.ShowMessage(ProfilesMessage.NotSaved);
                DrawThePage(false);
                return;
            }

            Choose(saved);
            if (saved.IsChild)
            {
                SaveLearning(learning.WithLanguageMix(editedMix));
            }

            BeginEditing(saved);
            profilesPage.ShowMessage(ProfilesMessage.Saved);
            DrawThePage(true);
        }

        /// <inheritdoc/>
        public void AskDelete()
        {
            if (!editingNew && list.CanDelete(chosen.Id))
            {
                AskDeleting(ResetQuestion.First);
            }
        }

        /// <inheritdoc/>
        public void AnswerDelete(bool yes)
        {
            if (!yes || deleteQuestion == ResetQuestion.None)
            {
                AskDeleting(ResetQuestion.None);
                return;
            }

            if (deleteQuestion == ResetQuestion.First)
            {
                AskDeleting(ResetQuestion.Second);
                return;
            }

            AskDeleting(ResetQuestion.None);
            ProfileId deleted = chosen.Id;
            list.Delete(deleted);
            if (!profiles.Save(list))
            {
                list = profiles.Load();
                profilesPage.ShowMessage(ProfilesMessage.NotSaved);
                DrawThePage(false);
                return;
            }

            bool folderGone = profiles.DeleteFolder(list, deleted);
            Profile next = list.ProfileToOpen;
            Choose(next);
            BeginEditing(next);
            profilesPage.ShowMessage(folderGone ? ProfilesMessage.Deleted : ProfilesMessage.FilesLeft);
            DrawThePage(true);
        }

        /// <inheritdoc/>
        public void MakeTester()
        {
            if (deleteQuestion != ResetQuestion.None || list.Tester != null)
            {
                return;
            }

            Profile tester = list.AddTester(list.FirstFreeAvatar(profiles.Avatars), clock.Today);
            if (!profiles.Save(list))
            {
                list = profiles.Load();
                profilesPage.ShowMessage(ProfilesMessage.NotSaved);
                DrawThePage(false);
                return;
            }

            Choose(tester);
            BeginEditing(tester);
            profilesPage.ShowMessage(ProfilesMessage.Saved);
            DrawThePage(true);
        }

        /// <inheritdoc/>
        public void PlayAsTester()
        {
            Profile tester = list.Tester;
            if (deleteQuestion != ResetQuestion.None || tester == null || leaving)
            {
                return;
            }

            list.Play(tester.Id);
            if (!profiles.Save(list))
            {
                list = profiles.Load();
                profilesPage.ShowMessage(ProfilesMessage.NotSaved);
                return;
            }

            Leave(AppScene.Map);
        }

        /// <inheritdoc/>
        public void ResetTester()
        {
            Profile tester = list.Tester;
            if (tester == null)
            {
                return;
            }

            bool deleted = ProgressReset.Run(profiles.FilesOf(tester));
            profilesPage.ShowMessage(deleted ? ProfilesMessage.TesterReset : ProfilesMessage.TesterNotReset);
            Redraw();
            DrawThePage(false);
        }

        private void Start()
        {
            curriculum = CurriculumAssets.Build(domains, letters, words, pictures, voiceLines, shapes);
            rules = learningRules.ToParentRules();
            clock = new SystemClock();
            string saveRoot = Application.persistentDataPath;
            tabletFiles = SaveStore.ForTablet(saveRoot);
            tablet = tabletFiles.Load(TabletSettingsData.FileName, TabletSettingsData.CreateDefaults()).Data;
            AppLocale.Select(tablet.AppLanguage);
            profiles = ProfileStore.ForApp(avatars, learningRules);
            list = profiles.Load();
            if (list.Playing.IsSet)
            {
                new PlayClock(profiles.FilesOf(list.ProfileToOpen), clock).Wake();
            }

            Choose(list.ProfileToOpen);
            if (curriculum.Domains.Count == 0)
            {
                Debug.LogError("Parent area: the content has no domain, so there is no progress to show.");
                return;
            }

            screen.Bind(this);
            profilesPage.Bind(this);
            profilesPage.SetOpen(false);
            Redraw();
        }

        private void Update()
        {
            if (!AndroidBack.WasPressed())
            {
                return;
            }

            if (profilesPage.IsOpen)
            {
                CloseProfiles();
                return;
            }

            Back();
        }

        private void LeaveTheArea()
        {
            if (list.Playing.IsSet || !list.StartsWithPicker)
            {
                list.Play(list.ProfileToOpen.Id);
                if (!profiles.Save(list))
                {
                    Debug.LogError("Parent area: who is playing could not be saved; the map opens the profile the list names.");
                }

                Leave(AppScene.Map);
                return;
            }

            Leave(AppScene.Profiles);
        }

        private void Leave(AppScene scene)
        {
            if (leaving)
            {
                return;
            }

            leaving = true;
            SceneNavigator.Load(scene);
        }

        private void Choose(Profile profile)
        {
            chosen = profile;
            profileFiles = profiles.FilesOf(profile);
            learning = profileFiles.Load(LearningSettingsData.FileName, LearningSettingsData.CreateDefaults()).Data;
            if (Report != null)
            {
                Redraw();
            }
        }

        private void BeginEditing(Profile profile)
        {
            editingNew = false;
            typedName = profile.Name.Text;
            editedAvatar = profile.Avatar;
            editedMix = learning.LanguageMix;
        }

        private ProfileId EditedId()
        {
            return editingNew ? default : chosen.Id;
        }

        private void CloseThePage()
        {
            AskDeleting(ResetQuestion.None);
            profilesPage.SetOpen(false);
            Redraw();
        }

        private void AskDeleting(ResetQuestion asked)
        {
            deleteQuestion = asked;
            profilesPage.AskDelete(asked);
        }

        private ProfileEditorShown Editor()
        {
            ProfileNameProblem problem = ProfileName.Read(typedName, profiles.Rules, out ProfileName _);
            ProfileId edited = EditedId();
            AvatarChoice[] choices = profiles.Avatars
                .Select(avatar => new AvatarChoice(avatar, avatars.SpriteOf(avatar), list.IsAvatarFree(avatar, edited),
                    avatar == editedAvatar))
                .ToArray();
            bool isTester = !editingNew && !chosen.IsChild;
            return new ProfileEditorShown(editingNew, isTester, typedName, choices, editedMix,
                !editingNew && list.CanDelete(chosen.Id), problem, profiles.Rules.NameMostCharacters);
        }

        private void DrawThePage(bool writeName)
        {
            List<ParentProfileRow> rows = new List<ParentProfileRow>();
            foreach (Profile profile in list.All)
            {
                SaveStore files = profiles.FilesOf(profile);
                LanguageMix mix = files.Load(LearningSettingsData.FileName, LearningSettingsData.CreateDefaults()).Data.LanguageMix;
                LearnerProgress progress = ProgressFile.Load(files, curriculum, OrderFor(mix));
                ProfileSummary summary = ProfileSummary.Of(ReportOf(progress, mix), progress);
                int daysAgo = summary.HasPlayed ? clock.Today.DaysSince(summary.LastPlayed) : -1;
                rows.Add(new ParentProfileRow(profile, avatars.SpriteOf(profile.Avatar), summary, daysAgo,
                    !editingNew && profile.Id == chosen.Id));
            }

            profilesPage.ShowList(rows, list.CanAddChild);
            profilesPage.ShowEditor(Editor(), writeName);
        }

        private LessonOrder OrderFor(LanguageMix mix)
        {
            return new LessonOrder(curriculum.Domains[0], mix, new PlayableSteps(registry.PlayableKinds(), curriculum));
        }

        private ParentReport ReportOf(LearnerProgress progress, LanguageMix mix)
        {
            Domain domain = curriculum.Domains[0];
            return new ParentReport(curriculum, domain, new MapRoute(curriculum, domain, OrderFor(mix), progress), progress,
                clock.Today);
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
            LearnerProgress progress = ProgressFile.Load(profileFiles, curriculum, OrderFor(learning.LanguageMix));
            Report = ReportOf(progress, learning.LanguageMix);
            ParentSettingsShown settings = new ParentSettingsShown(learning.LanguageMix, learning.SessionLimitBy(rules),
                rules.SessionLimitMinutes, tablet.VoiceVolume, tablet.AppLanguage);
            screen.Show(Report, settings, Application.version);
            screen.ShowChosen(avatars.SpriteOf(chosen.Avatar), chosen.Kind == ProfileKind.Tester, chosen.Name.Text);
        }
    }
}
