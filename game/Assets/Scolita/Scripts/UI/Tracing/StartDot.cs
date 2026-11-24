using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Tracing
{
    /// <summary>
    /// A stroke's start dot on the writing board (design/README.md: green with a white rim and its stroke
    /// number; the next stroke's dot outlined), which pulses after a stroke that missed. Its number's text is
    /// made once, so moving it allocates nothing.
    /// </summary>
    public sealed class StartDot : MonoBehaviour
    {
        private const float PulseSeconds = 1.6f;
        private const float PulsesPerSecond = 2.5f;

        [SerializeField] private RectTransform rect;
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text number;
        [SerializeField] private Sprite currentSprite;
        [SerializeField] private Sprite nextSprite;
        [SerializeField] private Color currentNumber = Color.white;
        [SerializeField] private Color nextNumber = new Color(0.91f, 0.933f, 0.973f);

        private float pulseLeft;

        /// <summary>
        /// Shows the dot of stroke <paramref name="label"/> (its number as text) at <paramref name="local"/>, as
        /// the stroke to draw now when <paramref name="current"/>, else as the next one; called by the tracing view.
        /// </summary>
        public void Show(Vector2 local, string label, bool current)
        {
            rect.anchoredPosition = local;
            image.sprite = current ? currentSprite : nextSprite;
            number.text = label;
            number.color = current ? currentNumber : nextNumber;
            rect.localScale = Vector3.one;
            pulseLeft = 0f;
            gameObject.SetActive(true);
        }

        /// <summary>Pulses the dot for a moment (after a stroke that missed); called by the tracing view.</summary>
        public void Pulse()
        {
            pulseLeft = PulseSeconds;
        }

        /// <summary>Hides the dot; called by the tracing view.</summary>
        public void Hide()
        {
            pulseLeft = 0f;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (pulseLeft <= 0f)
            {
                return;
            }

            pulseLeft -= Time.unscaledDeltaTime;
            float wave = Mathf.Sin((PulseSeconds - pulseLeft) * PulsesPerSecond * Mathf.PI * 2f);
            rect.localScale = Vector3.one * (pulseLeft > 0f ? 1f + 0.25f * Mathf.Abs(wave) : 1f);
        }
    }
}
