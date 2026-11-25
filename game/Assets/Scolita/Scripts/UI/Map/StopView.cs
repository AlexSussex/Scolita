using Scolita.Core.Map;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scolita.UI.Map
{
    /// <summary>
    /// A stop on the galaxy map's path (design/screens.md, screen 1): a white dot with a green tick when done,
    /// the big yellow dot when it is next, a faint dot when ahead; its tap area is larger than the dot (at least
    /// 64 dp). A tap goes to the map screen (one ahead says "not yet" and wobbles). Made from a template by
    /// <see cref="MapScreen"/>.
    /// </summary>
    public sealed class StopView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private RectTransform dot;
        [SerializeField] private Image dotImage;
        [SerializeField] private Sprite done;
        [SerializeField] private Sprite next;
        [SerializeField] private Sprite ahead;
        [SerializeField] private float nextSize = 48f;
        [SerializeField] private float otherSize = 36f;
        [SerializeField] private float wobbleSeconds = 0.5f;
        [SerializeField] private float wobbleDegrees = 14f;
        [SerializeField] private float wobblesPerSecond = 6f;

        private MapScreen screen;
        private Wobble wobble;

        /// <summary>The stop shown.</summary>
        public MapStop Stop { get; private set; }

        /// <summary>Shows <paramref name="stop"/>, sending its taps to <paramref name="mapScreen"/>; called once by <see cref="MapScreen.Show"/>.</summary>
        public void Show(MapStop stop, MapScreen mapScreen)
        {
            Stop = stop;
            screen = mapScreen;
            wobble = new Wobble(wobbleSeconds, wobbleDegrees, wobblesPerSecond);
            dotImage.sprite = stop.State == StopState.Done ? done : stop.State == StopState.Next ? next : ahead;
            float size = stop.State == StopState.Next ? nextSize : otherSize;
            dot.sizeDelta = new Vector2(size, size);
        }

        /// <summary>Wobbles gently (a "not yet"); called by <see cref="MapScreen"/>.</summary>
        public void StartWobble()
        {
            wobble?.Start(Time.unscaledTime);
        }

        /// <summary>Sends the tap to the map screen; called by the EventSystem.</summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (screen != null && !eventData.dragging)
            {
                screen.StopTapped(this);
            }
        }

        private void Update()
        {
            if (wobble != null && wobble.TryGetAngle(Time.unscaledTime, out float angle))
            {
                dot.localRotation = Quaternion.Euler(0f, 0f, angle);
            }
        }
    }
}
