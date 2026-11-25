using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Map
{
    /// <summary>
    /// The rocket on the galaxy map, standing over the next stop (design/screens.md, screen 1; <c>look</c>'s
    /// <c>Art/Trip/rocket-standing</c>), floating slowly; it flies (<c>rocket-flying</c>, its flame lit) along the
    /// path to a later stop on the same planet. Moved in <c>Update</c> from cached values, allocating nothing.
    /// </summary>
    public sealed class RocketView : MonoBehaviour
    {
        [SerializeField] private RectTransform rocket;
        [SerializeField] private Image picture;
        [SerializeField] private Sprite standing;
        [SerializeField] private Sprite flying;
        [SerializeField] private Vector2 aboveStop = new Vector2(0f, 70f);
        [SerializeField] private float floatPixels = 5f;
        [SerializeField] private float floatsPerSecond = 0.6f;
        [SerializeField] private float hopSeconds = 1.2f;
        [SerializeField] private float hopHeight = 60f;

        private Vector2 from;
        private Vector2 to;
        private float hopStartedAt;
        private bool hopping;

        /// <summary>Whether it is hopping along the path.</summary>
        public bool IsHopping => hopping;

        /// <summary>Puts it over the stop at <paramref name="stop"/> at once; called by <see cref="MapScreen"/>.</summary>
        public void PlaceAt(Vector2 stop)
        {
            gameObject.SetActive(true);
            hopping = false;
            to = stop + aboveStop;
            rocket.anchoredPosition = to;
        }

        /// <summary>Hops it from the stop at <paramref name="fromStop"/> to the stop at <paramref name="toStop"/>; called by <see cref="MapScreen"/>.</summary>
        public void HopBetween(Vector2 fromStop, Vector2 toStop)
        {
            gameObject.SetActive(true);
            from = fromStop + aboveStop;
            to = toStop + aboveStop;
            hopStartedAt = Time.unscaledTime;
            hopping = true;
            rocket.anchoredPosition = from;
        }

        /// <summary>Hides it (a map with no stop); called by <see cref="MapScreen"/>.</summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            float now = Time.unscaledTime;
            Vector2 position = to;
            if (hopping)
            {
                float share = Mathf.Clamp01((now - hopStartedAt) / hopSeconds);
                float eased = share * share * (3f - 2f * share);
                position = Vector2.LerpUnclamped(from, to, eased);
                position.y += hopHeight * Mathf.Sin(share * Mathf.PI);
                hopping = share < 1f;
            }

            picture.sprite = hopping ? flying : standing;

            position.y += floatPixels * Mathf.Sin(now * floatsPerSecond * 2f * Mathf.PI);
            rocket.anchoredPosition = position;
        }
    }
}
