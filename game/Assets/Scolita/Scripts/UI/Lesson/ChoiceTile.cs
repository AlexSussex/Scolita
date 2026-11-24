using Scolita.Core.Activities;
using Scolita.Core.Content;
using Scolita.Game.Lessons;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scolita.UI.Lesson
{
    /// <summary>
    /// One card she can tap in an activity (a letter, a picture): it shows its choice with its language
    /// badge, sends a tap to the activity's input, and plays its feedback: a sparkle when right, a
    /// small bounce when heard, a gentle wobble on a miss, faded after a miss, glowing yellow when it
    /// is the answer shown. Its animations run on plain arithmetic in <c>Update</c>, allocating nothing.
    /// </summary>
    public sealed class ChoiceTile : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private RectTransform body;
        [SerializeField] private CanvasGroup group;
        [SerializeField] private Image glow;
        [SerializeField] private Image sparkle;
        [SerializeField] private TMP_Text label;
        [SerializeField] private TMP_Text upperLabel;
        [SerializeField] private Image picture;
        [SerializeField] private LanguageBadge badge;
        [SerializeField] private float wobbleSeconds = 0.5f;
        [SerializeField] private float wobbleDegrees = 9f;
        [SerializeField] private float wobblesPerSecond = 6f;
        [SerializeField] private float bounceSeconds = 0.3f;
        [SerializeField] private float bounceScale = 0.08f;
        [SerializeField] private float sparkleSeconds = 0.8f;
        [SerializeField] private float sparkleScale = 1.6f;
        [SerializeField] private float fadedAlpha = 0.45f;
        [SerializeField] private float glowPulsesPerSecond = 1.2f;

        private IActivityInput input;
        private int index;
        private float wobbleLeft;
        private float bounceLeft;
        private float sparkleLeft;
        private bool glowing;

        /// <summary>The item it shows.</summary>
        public ContentId Item { get; private set; }

        /// <summary>Its index among the round's choices.</summary>
        public int Index => index;

        /// <summary>Whether it is faded (after a miss).</summary>
        public bool IsFaded => group.alpha < 1f;

        /// <summary>Whether it glows (the answer shown).</summary>
        public bool IsGlowing => glowing;

        /// <summary>
        /// Shows <paramref name="choice"/> as choice <paramref name="choiceIndex"/>, its text
        /// <paramref name="text"/> (rich text allowed) and its picture <paramref name="pictureSprite"/>
        /// (null for none), sending taps to <paramref name="activityInput"/>. Called by the activity
        /// views when a step begins.
        /// </summary>
        public void Show(int choiceIndex, ActivityChoice choice, string text, Sprite pictureSprite, IActivityInput activityInput)
        {
            index = choiceIndex;
            input = activityInput;
            Item = choice.Item;
            label.SetText(text);
            if (upperLabel != null)
            {
                upperLabel.SetText(choice.UpperText);
            }

            if (picture != null)
            {
                picture.sprite = pictureSprite;
                picture.enabled = pictureSprite != null;
            }

            badge.Show(choice.Language);
            wobbleLeft = 0f;
            bounceLeft = 0f;
            sparkleLeft = 0f;
            SetFadedTo(false);
            SetGlowingTo(false);
            body.localRotation = Quaternion.identity;
            body.localScale = Vector3.one;
            gameObject.SetActive(true);
        }

        /// <summary>Wobbles gently (a miss); called by the activity views.</summary>
        public void Wobble()
        {
            wobbleLeft = wobbleSeconds;
        }

        /// <summary>Bounces a little (heard, not an answer); called by the activity views.</summary>
        public void Bounce()
        {
            bounceLeft = bounceSeconds;
        }

        /// <summary>Sparkles (right); called by the activity views.</summary>
        public void Sparkle()
        {
            sparkleLeft = sparkleSeconds;
        }

        /// <summary>Fades it, or brings it back; called by the activity views.</summary>
        public void SetFaded(bool faded)
        {
            SetFadedTo(faded);
        }

        /// <summary>Makes it glow yellow (the answer shown), or stop; called by the activity views.</summary>
        public void SetGlowing(bool on)
        {
            SetGlowingTo(on);
        }

        /// <summary>Sends the tap to the activity; called by the EventSystem.</summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            input?.Answer(ActivityAnswer.Tap(index));
        }

        private void Awake()
        {
            sparkle.enabled = false;
            glow.enabled = false;
        }

        private void Update()
        {
            float delta = Time.unscaledDeltaTime;
            AnimateWobble(delta);
            AnimateBounceAndSparkle(delta);
            if (glowing)
            {
                Color colour = glow.color;
                colour.a = 0.65f + 0.35f * Mathf.Sin(Time.unscaledTime * glowPulsesPerSecond * 2f * Mathf.PI);
                glow.color = colour;
            }
        }

        private void AnimateWobble(float delta)
        {
            if (wobbleLeft <= 0f)
            {
                return;
            }

            wobbleLeft = Mathf.Max(0f, wobbleLeft - delta);
            float share = wobbleLeft / wobbleSeconds;
            float angle = wobbleDegrees * share * Mathf.Sin((wobbleSeconds - wobbleLeft) * wobblesPerSecond * 2f * Mathf.PI);
            body.localRotation = Quaternion.Euler(0f, 0f, angle);
        }

        private void AnimateBounceAndSparkle(float delta)
        {
            if (bounceLeft > 0f)
            {
                bounceLeft = Mathf.Max(0f, bounceLeft - delta);
                float grow = bounceScale * Mathf.Sin(bounceLeft / bounceSeconds * Mathf.PI);
                body.localScale = new Vector3(1f + grow, 1f + grow, 1f);
            }

            if (sparkleLeft > 0f)
            {
                sparkleLeft = Mathf.Max(0f, sparkleLeft - delta);
                float done = 1f - sparkleLeft / sparkleSeconds;
                sparkle.enabled = sparkleLeft > 0f;
                sparkle.rectTransform.localScale = Vector3.one * Mathf.Lerp(0.4f, sparkleScale, done);
                sparkle.rectTransform.localRotation = Quaternion.Euler(0f, 0f, done * 90f);
                Color colour = sparkle.color;
                colour.a = 1f - done;
                sparkle.color = colour;
                float grow = bounceScale * Mathf.Sin(done * Mathf.PI);
                body.localScale = new Vector3(1f + grow, 1f + grow, 1f);
            }
        }

        private void SetFadedTo(bool faded)
        {
            group.alpha = faded ? fadedAlpha : 1f;
        }

        private void SetGlowingTo(bool on)
        {
            glowing = on;
            glow.enabled = on;
        }
    }
}
