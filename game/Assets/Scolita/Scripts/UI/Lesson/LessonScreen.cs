using Scolita.Game.Lessons;
using Scolita.Game.Voice;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Lesson
{
    /// <summary>
    /// The lesson screen around the activity (<c>Prefabs/Lesson/LessonScreen.prefab</c>): the step dots
    /// at the top centre, the mascot top right with its bubble, the area where the step's view goes,
    /// and the end of the lesson with its big yellow "Mai departe" (until <c>rewards</c> builds design
    /// screen 6). The home button is the <c>LessonHud</c>'s, top left.
    /// </summary>
    public sealed class LessonScreen : LessonScreenView
    {
        [SerializeField] private RectTransform activityArea;
        [SerializeField] private SpeechBubbleView bubble;
        [SerializeField] private StepDots dots;
        [SerializeField] private GuideFigure guide;
        [SerializeField] private GameObject endPanel;
        [SerializeField] private Button continueButton;

        private ILessonScreenInput input;

        /// <inheritdoc/>
        public override RectTransform ActivityArea => activityArea;

        /// <inheritdoc/>
        public override SpeechBubbleView Bubble => bubble;

        /// <summary>The step dots.</summary>
        public StepDots Dots => dots;

        /// <summary>The mascot.</summary>
        public GuideFigure Guide => guide;

        /// <summary>The "Mai departe" button at the end.</summary>
        public Button ContinueButton => continueButton;

        /// <summary>Whether the end of the lesson is shown.</summary>
        public bool IsEndShown => endPanel.activeSelf;

        /// <inheritdoc/>
        public override void Bind(ILessonScreenInput screenInput)
        {
            input = screenInput;
            guide.Bind(screenInput);
        }

        /// <inheritdoc/>
        public override void ShowProgress(int current, int count)
        {
            dots.Show(current, count);
        }

        /// <inheritdoc/>
        public override void ShowMood(GuideMood mood)
        {
            guide.ShowMood(mood);
        }

        /// <inheritdoc/>
        public override void SetTalking(bool talking)
        {
            guide.SetTalking(talking);
        }

        /// <inheritdoc/>
        public override void ShowEnd()
        {
            endPanel.SetActive(true);
        }

        private void Awake()
        {
            endPanel.SetActive(false);
            continueButton.onClick.AddListener(OnContinue);
        }

        private void OnDestroy()
        {
            continueButton.onClick.RemoveListener(OnContinue);
        }

        private void OnContinue()
        {
            input?.Continue();
        }
    }
}
