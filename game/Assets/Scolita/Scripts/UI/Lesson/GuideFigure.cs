using Scolita.Game.Lessons;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scolita.UI.Lesson
{
    /// <summary>
    /// The mascot in a lesson (design/README.md → "The mascot"; the unicorn guide's frames from
    /// <c>look</c>, standing in for the guide she chooses): happy, thinking or cheering, its mouth
    /// moving while it speaks, floating slowly; tapping it says its last line again. Its frames are
    /// swapped by time in <c>Update</c>, allocating nothing.
    /// </summary>
    public sealed class GuideFigure : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image image;
        [SerializeField] private RectTransform floating;
        [SerializeField] private Sprite happyIdle;
        [SerializeField] private Sprite happyTalk;
        [SerializeField] private Sprite thinkingIdle;
        [SerializeField] private Sprite thinkingTalk;
        [SerializeField] private Sprite cheering0;
        [SerializeField] private Sprite cheering1;
        [SerializeField] private float framesPerSecond = 5f;
        [SerializeField] private float floatPixels = 6f;
        [SerializeField] private float floatsPerSecond = 0.5f;

        private IGuideInput input;
        private GuideMood mood;
        private bool talking;
        private Vector2 restingPosition;

        /// <summary>Its mood.</summary>
        public GuideMood Mood => mood;

        /// <summary>Sends its taps to <paramref name="screenInput"/>; called by the lesson screen and the map.</summary>
        public void Bind(IGuideInput screenInput)
        {
            input = screenInput;
        }

        /// <summary>Shows <paramref name="newMood"/>; called by the lesson screen and the map.</summary>
        public void ShowMood(GuideMood newMood)
        {
            mood = newMood;
        }

        /// <summary>Moves its mouth, or stops; called every frame by the lesson screen and the map.</summary>
        public void SetTalking(bool isTalking)
        {
            talking = isTalking;
        }

        /// <summary>Says the last line again; called by the EventSystem on a tap.</summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            input?.RepeatLastLine();
        }

        private void Awake()
        {
            restingPosition = floating.anchoredPosition;
        }

        private void Update()
        {
            bool second = Mathf.FloorToInt(Time.unscaledTime * framesPerSecond) % 2 == 1;
            image.sprite = FrameFor(second);
            float lift = floatPixels * Mathf.Sin(Time.unscaledTime * floatsPerSecond * 2f * Mathf.PI);
            floating.anchoredPosition = restingPosition + new Vector2(0f, lift);
        }

        private Sprite FrameFor(bool second)
        {
            switch (mood)
            {
                case GuideMood.Cheering:
                    return second ? cheering1 : cheering0;
                case GuideMood.Thinking:
                    return talking && second ? thinkingTalk : thinkingIdle;
                default:
                    return talking && second ? happyTalk : happyIdle;
            }
        }
    }
}
