using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scolita.UI.Map
{
    /// <summary>
    /// The trip to the next planet (PRD §3.1; design/README.md → "The moon and the Earth appear on the trips
    /// between planets"): over the whole screen, the rocket (<c>look</c>'s <c>rocket-flying-fast</c>) flies past
    /// the moon and the Earth through the stars, and the new planet comes to meet it, awake; a few seconds long, and a tap anywhere skips it. Moved
    /// in <c>Update</c> from cached values, allocating nothing.
    /// </summary>
    public sealed class TripView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private RectTransform area;
        [SerializeField] private RectTransform rocket;
        [SerializeField] private RectTransform moon;
        [SerializeField] private RectTransform earth;
        [SerializeField] private RectTransform planet;
        [SerializeField] private Image planetPicture;
        [SerializeField] private RectTransform[] sparkles = new RectTransform[0];
        [SerializeField] private float seconds = 4f;
        [SerializeField] private float sparklePixelsPerSecond = 700f;
        [SerializeField] private float moonHeight = 190f;
        [SerializeField] private float earthHeight = -170f;
        [SerializeField] private float planetHeight = 20f;
        [SerializeField] private float rocketBob = 12f;

        private MapScreen screen;
        private float startedAt;
        private float halfWidth;
        private bool playing;

        /// <summary>Whether it is playing.</summary>
        public bool IsPlaying => playing;

        /// <summary>
        /// Plays the trip to the planet drawn as <paramref name="destination"/> (its awake picture), telling
        /// <paramref name="mapScreen"/> when it ends or is skipped. Called by <see cref="MapScreen.MoveRocket"/>.
        /// </summary>
        public void Play(Sprite destination, MapScreen mapScreen)
        {
            screen = mapScreen;
            planetPicture.sprite = destination;
            gameObject.SetActive(true);
            halfWidth = Mathf.Max(area.rect.width * 0.5f, 1f);
            startedAt = Time.unscaledTime;
            playing = true;
            Animate(0f, startedAt);
        }

        /// <summary>Skips the rest of the trip; called by the EventSystem on a tap anywhere.</summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            Finish();
        }

        private void Update()
        {
            if (!playing)
            {
                return;
            }

            float now = Time.unscaledTime;
            float share = (now - startedAt) / seconds;
            if (share >= 1f)
            {
                Finish();
                return;
            }

            Animate(share, now);
        }

        private void Animate(float share, float now)
        {
            rocket.anchoredPosition = new Vector2(
                Mathf.Lerp(-halfWidth * 0.35f, -halfWidth * 0.1f, Ease(Phase(share, 0.7f, 1f))),
                rocketBob * Mathf.Sin(now * 3f));
            moon.anchoredPosition = new Vector2(Pass(Phase(share, 0.05f, 0.6f)), moonHeight);
            earth.anchoredPosition = new Vector2(Pass(Phase(share, 0.3f, 0.9f)), earthHeight);
            float arrival = Ease(Phase(share, 0.6f, 1f));
            planet.anchoredPosition = new Vector2(Mathf.Lerp(halfWidth + 300f, halfWidth * 0.45f, arrival), planetHeight);
            planet.localScale = Vector3.one * Mathf.Lerp(0.4f, 1f, arrival);
            float travelled = (now - startedAt) * sparklePixelsPerSecond;
            float span = halfWidth * 2f + 100f;
            for (int index = 0; index < sparkles.Length; index++)
            {
                float start = index * span / Mathf.Max(1, sparkles.Length);
                float x = Mathf.Repeat(start - travelled, span) - span * 0.5f;
                sparkles[index].anchoredPosition = new Vector2(x, sparkles[index].anchoredPosition.y);
            }
        }

        private float Pass(float phase)
        {
            return Mathf.Lerp(halfWidth + 250f, -halfWidth - 250f, phase);
        }

        private static float Phase(float share, float from, float to)
        {
            return Mathf.Clamp01((share - from) / (to - from));
        }

        private static float Ease(float share)
        {
            return share * share * (3f - 2f * share);
        }

        private void Finish()
        {
            if (!playing)
            {
                return;
            }

            playing = false;
            gameObject.SetActive(false);
            screen.TripFinished();
        }
    }
}
