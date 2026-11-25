using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Scolita.Core.Languages;
using Scolita.Core.Parent;
using Scolita.Game.Parent;
using Scolita.Game.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Parent
{
    /// <summary>
    /// The parent area (design/screens.md, screen 7), for adults: a lilac page with the back button, "Pentru părinți"
    /// and the version; "Ce a învățat" (the legend, a row of letters per language, the days played in the last two
    /// weeks, the next lessons, the recorded voices) and "Setări" (the lessons' languages, the daily limit, the voice's
    /// volume, the menus in English, and "Șterge progresul…", which asks twice). The fixed words are the string tables'
    /// (Localize String Event components); the rest is written here in the selected locale. Its taps go to
    /// <see cref="IParentAreaInput"/> (the Parent scene's <c>ParentRunner</c>).
    /// </summary>
    public sealed class ParentAreaScreen : ParentAreaView
    {
        [SerializeField] private Button backButton;
        [SerializeField] private TMP_Text versionLabel;
        [SerializeField] private RectTransform romanianCells;
        [SerializeField] private RectTransform englishCells;
        [SerializeField] private LetterCellView cellTemplate;
        [SerializeField] private Image[] days = new Image[0];
        [SerializeField] private Color playedDay = new Color32(0x4C, 0xD9, 0x7B, 0xFF);
        [SerializeField] private Color emptyDay = new Color32(0xEE, 0xE8, 0xFA, 0xFF);
        [SerializeField] private TMP_Text nextLessons;
        [SerializeField] private TMP_Text voices;
        [SerializeField] private PillButton bothPill;
        [SerializeField] private PillButton romanianPill;
        [SerializeField] private PillButton englishPill;
        [SerializeField] private RectTransform limitPills;
        [SerializeField] private PillButton limitTemplate;
        [SerializeField] private Slider voiceSlider;
        [SerializeField] private SliderRelease voiceSliderRelease;
        [SerializeField] private Toggle menusInEnglish;
        [SerializeField] private Button resetButton;
        [SerializeField] private TMP_Text resetDone;
        [SerializeField] private GameObject questionPanel;
        [SerializeField] private TMP_Text questionTitle;
        [SerializeField] private TMP_Text questionBody;
        [SerializeField] private Button noButton;
        [SerializeField] private Button yesButton;
        [SerializeField] private TMP_Text yesLabel;
        [SerializeField] private Button chosenButton;
        [SerializeField] private Image chosenAvatar;
        [SerializeField] private TMP_Text chosenName;
        [SerializeField] private GameObject chosenTestBadge;

        private readonly List<LetterCellView> cells = new List<LetterCellView>();
        private readonly List<PillButton> limits = new List<PillButton>();
        private readonly List<int> limitMinutes = new List<int>();
        private IParentAreaInput input;
        private bool drawing;

        /// <summary>The letters drawn, Romanian's then English's.</summary>
        public IReadOnlyList<LetterCellView> Cells => cells;

        /// <summary>The days drawn, the oldest first.</summary>
        public IReadOnlyList<Image> Days => days;

        /// <summary>Whether a day square is drawn as played.</summary>
        public bool IsPlayedDay(int index)
        {
            return days[index].color == playedDay;
        }

        /// <summary>The language mix pills: both, Romanian only, English only.</summary>
        public IReadOnlyList<PillButton> MixPills => new[] { bothPill, romanianPill, englishPill };

        /// <summary>The daily limit pills: none, then the minutes.</summary>
        public IReadOnlyList<PillButton> LimitPills => limits;

        /// <summary>The next lessons' text.</summary>
        public string NextLessonsText => nextLessons.text;

        /// <summary>The recorded voices' text.</summary>
        public string VoicesText => voices.text;

        /// <summary>The version's text.</summary>
        public string VersionText => versionLabel.text;

        /// <summary>The back button.</summary>
        public Button BackButton => backButton;

        /// <summary>The voice slider.</summary>
        public Slider VoiceSlider => voiceSlider;

        /// <summary>The "Meniurile în engleză" switch.</summary>
        public Toggle MenusInEnglish => menusInEnglish;

        /// <summary>"Șterge progresul…".</summary>
        public Button ResetButton => resetButton;

        /// <summary>Whether a reset question is shown.</summary>
        public bool IsAskingReset => questionPanel.activeSelf;

        /// <summary>The reset question's "yes".</summary>
        public Button YesButton => yesButton;

        /// <summary>The reset question's "no".</summary>
        public Button NoButton => noButton;

        /// <summary>The reset question's words, its title then its body.</summary>
        public string QuestionText => questionTitle.text + " " + questionBody.text;

        /// <summary>The chosen profile's button in the header (opens the profiles page).</summary>
        public Button ChosenButton => chosenButton;

        /// <summary>The chosen profile's name as the header shows it.</summary>
        public string ChosenNameText => chosenName.text;

        /// <summary>Whether "the progress has been deleted" is shown.</summary>
        public bool IsResetDoneShown => resetDone.gameObject.activeSelf;

        /// <inheritdoc/>
        public override void Bind(IParentAreaInput areaInput)
        {
            input = areaInput;
        }

        /// <inheritdoc/>
        public override void Show(ParentReport report, ParentSettingsShown settings, string version)
        {
            drawing = true;
            versionLabel.SetText(AppLocale.Text("parent-version", version));
            DrawCells(report);
            for (int index = 0; index < days.Length; index++)
            {
                bool played = index < report.DaysPlayed.Count && report.DaysPlayed[index];
                days[index].color = played ? playedDay : emptyDay;
            }

            nextLessons.SetText(NextLessonsOf(report));
            voices.SetText(AppLocale.Text("voices-status", report.Voices[0].Recorded, report.Voices[0].ToRecord,
                report.Voices[1].Recorded, report.Voices[1].ToRecord));
            bothPill.SetChosen(settings.Mix == LanguageMix.Both);
            romanianPill.SetChosen(settings.Mix == LanguageMix.RomanianOnly);
            englishPill.SetChosen(settings.Mix == LanguageMix.EnglishOnly);
            DrawLimits(settings);
            voiceSlider.SetValueWithoutNotify(settings.VoiceVolume);
            menusInEnglish.SetIsOnWithoutNotify(settings.AppLanguage == Language.English);
            drawing = false;
        }

        /// <inheritdoc/>
        public override void ShowChosen(Sprite avatar, bool isTester, string name)
        {
            chosenAvatar.sprite = avatar;
            chosenName.SetText(isTester ? AppLocale.Text("profile-tester-name")
                : name.Length == 0 ? AppLocale.Text("profile-no-name") : name);
            chosenTestBadge.SetActive(isTester);
        }

        /// <inheritdoc/>
        public override void AskReset(ResetQuestion question)
        {
            questionPanel.SetActive(question != ResetQuestion.None);
            if (question == ResetQuestion.None)
            {
                return;
            }

            resetDone.gameObject.SetActive(false);
            bool first = question == ResetQuestion.First;
            questionTitle.SetText(AppLocale.Text(first ? "reset-first-title" : "reset-second-title"));
            questionBody.SetText(AppLocale.Text(first ? "reset-first-body" : "reset-second-body"));
            yesLabel.SetText(AppLocale.Text(first ? "reset-first-yes" : "reset-second-yes"));
        }

        /// <inheritdoc/>
        public override void ShowResetDone(bool deleted)
        {
            resetDone.SetText(AppLocale.Text(deleted ? "reset-done" : "reset-failed"));
            resetDone.gameObject.SetActive(true);
        }

        private void Awake()
        {
            cellTemplate.gameObject.SetActive(false);
            limitTemplate.gameObject.SetActive(false);
            questionPanel.SetActive(false);
            resetDone.gameObject.SetActive(false);
            backButton.onClick.AddListener(() => input?.Back());
            bothPill.Button.onClick.AddListener(() => input?.ChooseMix(LanguageMix.Both));
            romanianPill.Button.onClick.AddListener(() => input?.ChooseMix(LanguageMix.RomanianOnly));
            englishPill.Button.onClick.AddListener(() => input?.ChooseMix(LanguageMix.EnglishOnly));
            voiceSliderRelease.Released += OnVoiceReleased;
            menusInEnglish.onValueChanged.AddListener(OnMenusChanged);
            resetButton.onClick.AddListener(() => input?.AskReset());
            noButton.onClick.AddListener(() => input?.AnswerReset(false));
            yesButton.onClick.AddListener(() => input?.AnswerReset(true));
            chosenButton.onClick.AddListener(() => input?.OpenProfiles());
        }

        private void OnDestroy()
        {
            voiceSliderRelease.Released -= OnVoiceReleased;
        }

        private void OnVoiceReleased()
        {
            input?.SetVoiceVolume(voiceSlider.value);
        }

        private void OnMenusChanged(bool english)
        {
            if (!drawing)
            {
                input?.SetMenusInEnglish(english);
            }
        }

        private void DrawCells(ParentReport report)
        {
            foreach (LetterCellView cell in cells)
            {
                Destroy(cell.gameObject);
            }

            cells.Clear();
            foreach (LetterRow row in report.Rows)
            {
                RectTransform parent = row.Language == Language.English ? englishCells : romanianCells;
                foreach (LetterCell cell in row.Cells)
                {
                    LetterCellView view = Instantiate(cellTemplate, parent);
                    view.gameObject.SetActive(true);
                    view.Show(cell.Letter.Lower, cell.State);
                    cells.Add(view);
                }
            }
        }

        private void DrawLimits(ParentSettingsShown settings)
        {
            List<int> choices = new List<int> { 0 };
            choices.AddRange(settings.LimitChoices);
            if (!limitMinutes.SequenceEqual(choices))
            {
                foreach (PillButton pill in limits)
                {
                    Destroy(pill.gameObject);
                }

                limits.Clear();
                limitMinutes.Clear();
                foreach (int minutes in choices)
                {
                    PillButton pill = Instantiate(limitTemplate, limitPills);
                    pill.gameObject.SetActive(true);
                    int chosen = minutes;
                    pill.Button.onClick.AddListener(() => input?.ChooseLimit(chosen));
                    limits.Add(pill);
                    limitMinutes.Add(minutes);
                }
            }

            for (int index = 0; index < limits.Count; index++)
            {
                int minutes = limitMinutes[index];
                limits[index].SetWords(minutes == 0
                    ? AppLocale.Text("limit-off")
                    : AppLocale.Text("limit-minutes", minutes.ToString(CultureInfo.InvariantCulture)));
                limits[index].SetChosen(minutes == settings.Limit.Minutes);
            }
        }

        private static string NextLessonsOf(ParentReport report)
        {
            if (report.NextLessons.Count == 0)
            {
                return AppLocale.Text("next-none");
            }

            return string.Join("\n", report.NextLessons.Select(NextLessonText));
        }

        private static string NextLessonText(NextLesson next)
        {
            string planet = next.PlanetNumber.ToString(CultureInfo.InvariantCulture);
            string languages = AppLocale.Text(LanguagesKey(next.Lesson));
            return next.Letter != null
                ? AppLocale.Text("next-letter", planet, next.Letter.Lower, languages)
                : AppLocale.Text("next-practice", planet, languages);
        }

        private static string LanguagesKey(Core.Content.Lesson lesson)
        {
            switch (lesson.Languages)
            {
                case LanguageMix.RomanianOnly:
                    return "next-in-ro";
                case LanguageMix.EnglishOnly:
                    return "next-in-en";
                default:
                    return "next-in-both";
            }
        }
    }
}
