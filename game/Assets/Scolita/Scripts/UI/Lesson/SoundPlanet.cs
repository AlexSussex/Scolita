using Scolita.Core.Activities;
using Scolita.Game.Lessons;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scolita.UI.Lesson
{
    /// <summary>
    /// One of <c>TwoSounds</c>' two little planets (design screen 4): the planet of one language with its
    /// badge and a speaker, a green tick once its sound has been heard, and its tray under it, where the
    /// pictures sorted to it go. A tap says its sound; a picture let go over the planet or its tray is
    /// dropped on it. It wobbles on a miss, glows when it is the answer shown, and sparkles when a
    /// picture lands; all on plain arithmetic in <c>Update</c>, allocating nothing.
    /// </summary>
    public sealed class SoundPlanet : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private RectTransform body;
        [SerializeField] private RectTransform dropArea;
        [SerializeField] private GameObject tick;
        [SerializeField] private Image glow;
        [SerializeField] private Image sparkle;
        [SerializeField] private LanguageBadge badge;
        [SerializeField] private Image[] traySlots = new Image[0];
        [SerializeField] private LanguageBadge[] trayBadges = new LanguageBadge[0];
        [SerializeField] private float wobbleSeconds = 0.5f;
        [SerializeField] private float wobbleDegrees = 8f;
        [SerializeField] private float wobblesPerSecond = 6f;
        [SerializeField] private float sparkleSeconds = 0.8f;
        [SerializeField] private float glowPulsesPerSecond = 1.2f;

        private IActivityInput input;
        private int index;
        private int filled;
        private float wobbleLeft;
        private float sparkleLeft;
        private bool glowing;

        /// <summary>Its index among the round's choices.</summary>
        public int Index => index;

        /// <summary>Whether its tick shows (its sound heard).</summary>
        public bool IsHeard => tick.activeSelf;

        /// <summary>Whether it glows (the answer shown).</summary>
        public bool IsGlowing => glowing;

        /// <summary>How many pictures are in its tray.</summary>
        public int PicturesInTray => filled;

        /// <summary>The planet, for a drag's aim (the PlayMode tests).</summary>
        public RectTransform Body => body;

        /// <summary>
        /// Shows the planet of <paramref name="choice"/> as choice <paramref name="choiceIndex"/>, its tray
        /// empty, sending taps to <paramref name="activityInput"/>. Called by <see cref="TwoSoundsView"/>
        /// when a step begins.
        /// </summary>
        public void Show(int choiceIndex, ActivityChoice choice, IActivityInput activityInput)
        {
            index = choiceIndex;
            input = activityInput;
            badge.Show(choice.Language);
            wobbleLeft = 0f;
            sparkleLeft = 0f;
            sparkle.enabled = false;
            body.localRotation = Quaternion.identity;
            SetGlowingTo(false);
            ClearTray();
            gameObject.SetActive(true);
        }

        /// <summary>Shows its tick, or hides it; called by <see cref="TwoSoundsView"/> after each answer.</summary>
        public void SetHeard(bool heard)
        {
            tick.SetActive(heard);
        }

        /// <summary>Glows (the answer shown), or stops; called by <see cref="TwoSoundsView"/>.</summary>
        public void SetGlowing(bool on)
        {
            SetGlowingTo(on);
        }

        /// <summary>Wobbles gently (a picture dropped on the wrong planet); called by <see cref="TwoSoundsView"/>.</summary>
        public void Wobble()
        {
            wobbleLeft = wobbleSeconds;
        }

        /// <summary>Sparkles (a picture landed); called by <see cref="TwoSoundsView"/>.</summary>
        public void Sparkle()
        {
            sparkleLeft = sparkleSeconds;
        }

        /// <summary>Empties its tray; called by <see cref="TwoSoundsView"/> before it fills the trays again.</summary>
        public void EmptyTray()
        {
            ClearTray();
        }

        /// <summary>
        /// Puts the picture <paramref name="sprite"/> of <paramref name="choice"/> in its tray's next slot (the
        /// last slot takes the rest); called by <see cref="TwoSoundsView"/> after each answer.
        /// </summary>
        public void AddToTray(ActivityChoice choice, Sprite sprite)
        {
            if (traySlots.Length == 0)
            {
                return;
            }

            int slot = Mathf.Min(filled, traySlots.Length - 1);
            traySlots[slot].sprite = sprite;
            traySlots[slot].gameObject.SetActive(true);
            trayBadges[slot].Show(choice.Language);
            filled++;
        }

        /// <summary>
        /// Whether <paramref name="screenPoint"/> is over the planet or its tray, seen by
        /// <paramref name="eventCamera"/>; called by <see cref="TwoSoundsView"/> when a picture is let go.
        /// </summary>
        public bool Holds(Vector2 screenPoint, Camera eventCamera)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(dropArea, screenPoint, eventCamera);
        }

        /// <summary>Sends the tap (hear its sound) to the activity; called by the EventSystem.</summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            input?.Answer(ActivityAnswer.Tap(index));
        }

        private void Update()
        {
            float delta = Time.unscaledDeltaTime;
            if (wobbleLeft > 0f)
            {
                wobbleLeft = Mathf.Max(0f, wobbleLeft - delta);
                float share = wobbleLeft / wobbleSeconds;
                float angle = wobbleDegrees * share * Mathf.Sin((wobbleSeconds - wobbleLeft) * wobblesPerSecond * 2f * Mathf.PI);
                body.localRotation = Quaternion.Euler(0f, 0f, angle);
            }

            if (sparkleLeft > 0f)
            {
                sparkleLeft = Mathf.Max(0f, sparkleLeft - delta);
                float done = 1f - sparkleLeft / sparkleSeconds;
                sparkle.enabled = sparkleLeft > 0f;
                sparkle.rectTransform.localScale = Vector3.one * Mathf.Lerp(0.4f, 1.6f, done);
                sparkle.canvasRenderer.SetAlpha(1f - done);
            }

            if (glowing)
            {
                glow.canvasRenderer.SetAlpha(0.65f + 0.35f * Mathf.Sin(Time.unscaledTime * glowPulsesPerSecond * 2f * Mathf.PI));
            }
        }

        private void ClearTray()
        {
            filled = 0;
            for (int slot = 0; slot < traySlots.Length; slot++)
            {
                traySlots[slot].gameObject.SetActive(false);
            }
        }

        private void SetGlowingTo(bool on)
        {
            glowing = on;
            glow.enabled = on;
        }
    }
}
