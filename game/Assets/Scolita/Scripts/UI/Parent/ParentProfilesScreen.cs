using System.Collections.Generic;
using Scolita.Core.Languages;
using Scolita.Core.Profiles;
using Scolita.Game.Parent;
using Scolita.Game.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Parent
{
    /// <summary>
    /// The parent area's profiles page (design screen 10, <c>Prefabs/Parent/ParentProfiles.prefab</c>, over the area): "Cine
    /// are profil" (a row per profile, the tester's buttons, "Adaugă un copil") and the editor (the name, the 14 avatars,
    /// the language mix, "Șterge profilul…", "Salvează"), and a deletion's two questions. For adults: text from 18 px,
    /// every target 64 px or more. Its taps go to <see cref="IParentProfilesInput"/> (the Parent scene's <c>ParentRunner</c>).
    /// </summary>
    public sealed class ParentProfilesScreen : ParentProfilesView
    {
        [SerializeField] private GameObject page;
        [SerializeField] private Button backButton;
        [SerializeField] private RectTransform rows;
        [SerializeField] private ProfileRowView rowTemplate;
        [SerializeField] private Button addChildButton;
        [SerializeField] private GameObject testerButtons;
        [SerializeField] private Button playAsTesterButton;
        [SerializeField] private Button resetTesterButton;
        [SerializeField] private Button makeTesterButton;
        [SerializeField] private TMP_Text editorTitle;
        [SerializeField] private GameObject nameBlock;
        [SerializeField] private TMP_Text nameLabel;
        [SerializeField] private TMP_InputField nameField;
        [SerializeField] private TMP_Text nameProblem;
        [SerializeField] private RectTransform avatarGrid;
        [SerializeField] private AvatarChoiceView avatarTemplate;
        [SerializeField] private GameObject mixBlock;
        [SerializeField] private PillButton bothPill;
        [SerializeField] private PillButton romanianPill;
        [SerializeField] private PillButton englishPill;
        [SerializeField] private Button deleteButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private TMP_Text saveLabel;
        [SerializeField] private TMP_Text message;
        [SerializeField] private GameObject questionPanel;
        [SerializeField] private TMP_Text questionTitle;
        [SerializeField] private TMP_Text questionBody;
        [SerializeField] private Button noButton;
        [SerializeField] private Button yesButton;
        [SerializeField] private TMP_Text yesLabel;

        private readonly List<ProfileRowView> rowViews = new List<ProfileRowView>();
        private readonly List<AvatarChoiceView> avatarViews = new List<AvatarChoiceView>();
        private IParentProfilesInput input;
        private bool writingName;

        /// <inheritdoc/>
        public override bool IsOpen => page.activeSelf;

        /// <summary>The rows drawn, in order.</summary>
        public IReadOnlyList<ProfileRowView> Rows => rowViews;

        /// <summary>The avatars drawn in the editor, in order.</summary>
        public IReadOnlyList<AvatarChoiceView> Avatars => avatarViews;

        /// <summary>The name in the field.</summary>
        public string NameTyped => nameField.text;

        /// <summary>The words under the name field (why it can't be saved), or an empty text.</summary>
        public string NameProblemText => nameProblem.gameObject.activeSelf ? nameProblem.text : string.Empty;

        /// <summary>The language mix pills of the editor: both, Romanian only, English only.</summary>
        public IReadOnlyList<PillButton> MixPills => new[] { bothPill, romanianPill, englishPill };

        /// <summary>"Adaugă un copil".</summary>
        public Button AddChildButton => addChildButton;

        /// <summary>"Joacă ca tester".</summary>
        public Button PlayAsTesterButton => playAsTesterButton;

        /// <summary>"Șterge progresul de test".</summary>
        public Button ResetTesterButton => resetTesterButton;

        /// <summary>"Fă un profil de test".</summary>
        public Button MakeTesterButton => makeTesterButton;

        /// <summary>"Șterge profilul…".</summary>
        public Button DeleteButton => deleteButton;

        /// <summary>"Salvează" / "Adaugă".</summary>
        public Button SaveButton => saveButton;

        /// <summary>The page's back button.</summary>
        public Button BackButton => backButton;

        /// <summary>The editor's title.</summary>
        public string EditorTitle => editorTitle.text;

        /// <summary>The message under the buttons, or an empty text.</summary>
        public string MessageText => message.gameObject.activeSelf ? message.text : string.Empty;

        /// <summary>Whether a deletion question is asked.</summary>
        public bool IsAskingDelete => questionPanel.activeSelf;

        /// <summary>The question's words.</summary>
        public string QuestionText => questionTitle.text + " " + questionBody.text;

        /// <summary>The question's "yes".</summary>
        public Button YesButton => yesButton;

        /// <summary>The question's "no".</summary>
        public Button NoButton => noButton;

        /// <summary>Types <paramref name="typed"/> into the name field, as the tablet's keyboard does; called by the tests.</summary>
        public void Type(string typed)
        {
            nameField.text = typed;
        }

        /// <inheritdoc/>
        public override void Bind(IParentProfilesInput profilesInput)
        {
            input = profilesInput;
        }

        /// <inheritdoc/>
        public override void SetOpen(bool open)
        {
            page.SetActive(open);
            if (!open)
            {
                questionPanel.SetActive(false);
            }
        }

        /// <inheritdoc/>
        public override void ShowList(IReadOnlyList<ParentProfileRow> shown, bool canAddChild)
        {
            foreach (ProfileRowView old in rowViews)
            {
                Destroy(old.gameObject);
            }

            rowViews.Clear();
            bool hasTester = false;
            foreach (ParentProfileRow row in shown)
            {
                ProfileRowView view = Instantiate(rowTemplate, rows);
                view.gameObject.SetActive(true);
                view.Show(row);
                ProfileId id = row.Profile.Id;
                view.Button.onClick.AddListener(() => input?.ChooseProfile(id));
                rowViews.Add(view);
                hasTester |= row.Profile.Kind == ProfileKind.Tester;
            }

            addChildButton.gameObject.SetActive(canAddChild);
            testerButtons.SetActive(hasTester);
            makeTesterButton.gameObject.SetActive(!hasTester);
        }

        /// <inheritdoc/>
        public override void ShowEditor(ProfileEditorShown editor, bool writeName)
        {
            editorTitle.SetText(AppLocale.Text(editor.IsNew ? "profile-new-title" : "profile-edit-title"));
            nameBlock.SetActive(!editor.IsTester);
            mixBlock.SetActive(!editor.IsTester);
            if (writeName)
            {
                writingName = true;
                nameField.SetTextWithoutNotify(editor.Name);
                writingName = false;
            }

            nameLabel.SetText(AppLocale.Text("profile-name-label", editor.NameMostCharacters));
            nameField.characterLimit = editor.NameMostCharacters;
            ShowProblem(editor.NameProblem, editor.NameMostCharacters);
            DrawAvatars(editor.Avatars);
            bothPill.SetChosen(editor.Mix == LanguageMix.Both);
            romanianPill.SetChosen(editor.Mix == LanguageMix.RomanianOnly);
            englishPill.SetChosen(editor.Mix == LanguageMix.EnglishOnly);
            deleteButton.gameObject.SetActive(editor.CanDelete);
            saveButton.interactable = editor.NameProblem == ProfileNameProblem.None;
            saveLabel.SetText(AppLocale.Text(editor.IsNew ? "profile-add" : "profile-save"));
        }

        /// <inheritdoc/>
        public override void AskDelete(ResetQuestion question)
        {
            questionPanel.SetActive(question != ResetQuestion.None);
            if (question == ResetQuestion.None)
            {
                return;
            }

            bool first = question == ResetQuestion.First;
            questionTitle.SetText(AppLocale.Text(first ? "profile-delete-first-title" : "profile-delete-second-title"));
            questionBody.SetText(AppLocale.Text(first ? "profile-delete-first-body" : "profile-delete-second-body"));
            yesLabel.SetText(AppLocale.Text(first ? "reset-first-yes" : "profile-delete-second-yes"));
        }

        /// <inheritdoc/>
        public override void ShowMessage(ProfilesMessage shown)
        {
            message.gameObject.SetActive(shown != ProfilesMessage.None);
            if (shown != ProfilesMessage.None)
            {
                message.SetText(AppLocale.Text(MessageKey(shown)));
            }
        }

        private static string MessageKey(ProfilesMessage shown)
        {
            switch (shown)
            {
                case ProfilesMessage.Saved:
                    return "profile-saved";
                case ProfilesMessage.NotSaved:
                    return "profile-not-saved";
                case ProfilesMessage.Deleted:
                    return "profile-deleted";
                case ProfilesMessage.FilesLeft:
                    return "profile-files-left";
                case ProfilesMessage.TesterReset:
                    return "profile-tester-reset";
                default:
                    return "profile-tester-not-reset";
            }
        }

        private void Awake()
        {
            rowTemplate.gameObject.SetActive(false);
            avatarTemplate.gameObject.SetActive(false);
            page.SetActive(false);
            questionPanel.SetActive(false);
            message.gameObject.SetActive(false);
            backButton.onClick.AddListener(() => input?.CloseProfiles());
            addChildButton.onClick.AddListener(() => input?.StartNewChild());
            playAsTesterButton.onClick.AddListener(() => input?.PlayAsTester());
            resetTesterButton.onClick.AddListener(() => input?.ResetTester());
            makeTesterButton.onClick.AddListener(() => input?.MakeTester());
            nameField.onValueChanged.AddListener(OnNameChanged);
            bothPill.Button.onClick.AddListener(() => input?.ChooseEditedMix(LanguageMix.Both));
            romanianPill.Button.onClick.AddListener(() => input?.ChooseEditedMix(LanguageMix.RomanianOnly));
            englishPill.Button.onClick.AddListener(() => input?.ChooseEditedMix(LanguageMix.EnglishOnly));
            deleteButton.onClick.AddListener(() => input?.AskDelete());
            saveButton.onClick.AddListener(() => input?.SaveProfile());
            noButton.onClick.AddListener(() => input?.AnswerDelete(false));
            yesButton.onClick.AddListener(() => input?.AnswerDelete(true));
        }

        private void OnNameChanged(string typed)
        {
            if (!writingName)
            {
                input?.TypeName(typed);
            }
        }

        private void ShowProblem(ProfileNameProblem problem, int mostCharacters)
        {
            nameProblem.gameObject.SetActive(problem != ProfileNameProblem.None);
            if (problem != ProfileNameProblem.None)
            {
                nameProblem.SetText(problem == ProfileNameProblem.TooLong
                    ? AppLocale.Text("profile-name-too-long", mostCharacters)
                    : AppLocale.Text("profile-name-letters"));
            }
        }

        private void DrawAvatars(IReadOnlyList<AvatarChoice> choices)
        {
            while (avatarViews.Count < choices.Count)
            {
                AvatarChoiceView view = Instantiate(avatarTemplate, avatarGrid);
                view.gameObject.SetActive(true);
                AvatarChoiceView captured = view;
                view.Button.onClick.AddListener(() => input?.ChooseAvatar(captured.Id));
                avatarViews.Add(view);
            }

            for (int index = 0; index < avatarViews.Count; index++)
            {
                bool shown = index < choices.Count;
                avatarViews[index].gameObject.SetActive(shown);
                if (shown)
                {
                    avatarViews[index].Show(choices[index]);
                }
            }
        }
    }
}
