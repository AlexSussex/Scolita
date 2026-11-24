using Scolita.Core.Activities;
using Scolita.Game.Lessons;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scolita.UI.Lesson
{
    /// <summary>
    /// One star of <c>LetterHunt</c>: a yellow star with a letter (small or capital, in the reading
    /// typeface) and its language badge, which sends a tap to the activity's input. Its own feedback
    /// runs on plain arithmetic in <c>Update</c>, allocating nothing: it pops (grows, spins and fades,
    /// then goes) when found, bounces back gently on a miss, twinkles for a moment as a hint, and glows
    /// steadily when it is the answer shown. <see cref="LetterHuntView"/> moves it.
    /// </summary>
    public sealed class HuntStar : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private RectTransform body;
        [SerializeField] private CanvasGroup group;
        [SerializeField] private StarGraphic glow;
        [SerializeField] private TMP_Text label;
        [SerializeField] private LanguageBadge badge;
        [SerializeField] private float popSeconds = 0.6f;
        [SerializeField] private float popScale = 0.6f;
        [SerializeField] private float bounceSeconds = 0.5f;
        [SerializeField] private float bounceScale = 0.18f;
        [SerializeField] private float bouncesPerSecond = 4f;
        [SerializeField] private float twinkleSeconds = 2.4f;
        [SerializeField] private float twinklesPerSecond = 2.5f;
        [SerializeField] private float twinkleScale = 0.12f;
        [SerializeField] private float glowPulsesPerSecond = 1.2f;

        private IActivityInput input;
        private int index;
        private float popLeft;
        private float bounceLeft;
        private float twinkleLeft;
        private bool glowing;

        /// <summary>Its index among the round's choices.</summary>
        public int Index => index;

        /// <summary>Whether it is still in the sky (not popped).</summary>
        public bool IsInTheSky => gameObject.activeSelf && popLeft <= 0f && group.alpha > 0f;

        /// <summary>Whether it glows (the answer shown).</summary>
        public bool IsGlowing => glowing;

        /// <summary>Whether it twinkles (a hint).</summary>
        public bool IsTwinkling => twinkleLeft > 0f;

        /// <summary>Its place in the star field, its parent.</summary>
        public RectTransform Rect => (RectTransform)transform;

        /// <summary>
        /// Shows <paramref name="choice"/> as choice <paramref name="choiceIndex"/>, sending taps to
        /// <paramref name="activityInput"/>. Called by <see cref="LetterHuntView"/> when a step begins.
        /// </summary>
        public void Show(int choiceIndex, ActivityChoice choice, IActivityInput activityInput)
        {
            index = choiceIndex;
            input = activityInput;
            label.SetText(choice.Text);
            badge.Show(choice.Language);
            popLeft = 0f;
            bounceLeft = 0f;
            twinkleLeft = 0f;
            glowing = false;
            glow.enabled = false;
            group.alpha = 1f;
            body.localScale = Vector3.one;
            body.localRotation = Quaternion.identity;
            gameObject.SetActive(true);
        }

        /// <summary>Pops: it was found. Called by <see cref="LetterHuntView"/>.</summary>
        public void Pop()
        {
            glowing = false;
            glow.enabled = false;
            twinkleLeft = 0f;
            popLeft = popSeconds;
        }

        /// <summary>Bounces back gently (a miss); called by <see cref="LetterHuntView"/>.</summary>
        public void Bounce()
        {
            bounceLeft = bounceSeconds;
        }

        /// <summary>Twinkles for a moment (the hint); called by <see cref="LetterHuntView"/>.</summary>
        public void Twinkle()
        {
            twinkleLeft = twinkleSeconds;
            glow.enabled = true;
        }

        /// <summary>Glows steadily (the answer shown), or stops; called by <see cref="LetterHuntView"/>.</summary>
        public void SetGlowing(bool on)
        {
            glowing = on;
            glow.enabled = on || twinkleLeft > 0f;
        }

        /// <summary>Sends the tap to the activity; called by the EventSystem.</summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (popLeft <= 0f)
            {
                input?.Answer(ActivityAnswer.Tap(index));
            }
        }

        private void Update()
        {
            float delta = Time.unscaledDeltaTime;
            if (popLeft > 0f)
            {
                AnimatePop(delta);
                return;
            }

            float grow = 0f;
            if (bounceLeft > 0f)
            {
                bounceLeft = Mathf.Max(0f, bounceLeft - delta);
                float share = bounceLeft / bounceSeconds;
                grow -= bounceScale * share * Mathf.Abs(Mathf.Sin((bounceSeconds - bounceLeft) * bouncesPerSecond * Mathf.PI));
            }

            if (twinkleLeft > 0f)
            {
                twinkleLeft = Mathf.Max(0f, twinkleLeft - delta);
                grow += twinkleScale * Mathf.Abs(Mathf.Sin(twinkleLeft * twinklesPerSecond * Mathf.PI));
                glow.enabled = twinkleLeft > 0f || glowing;
            }

            body.localScale = new Vector3(1f + grow, 1f + grow, 1f);
            if (glow.enabled)
            {
                float pulse = glowing ? 0.65f + 0.35f * Mathf.Sin(Time.unscaledTime * glowPulsesPerSecond * 2f * Mathf.PI) : 1f;
                glow.canvasRenderer.SetAlpha(pulse);
            }
        }

        private void AnimatePop(float delta)
        {
            popLeft = Mathf.Max(0f, popLeft - delta);
            float done = 1f - popLeft / popSeconds;
            float grow = 1f + popScale * done;
            body.localScale = new Vector3(grow, grow, 1f);
            body.localRotation = Quaternion.Euler(0f, 0f, done * 120f);
            group.alpha = 1f - done;
            if (popLeft <= 0f)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
