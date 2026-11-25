using Scolita.Core.Languages;
using Scolita.Core.Voice;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.Game.Voice
{
    /// <summary>
    /// The speech bubble (<see cref="ISpeechBubble"/>) on the Voice prefab: the text of a line no voice
    /// can say, with its language's badge (a colour and a shape, design/README.md). Hidden otherwise.
    /// </summary>
    public sealed class SpeechBubbleView : MonoBehaviour, ISpeechBubble
    {
        [SerializeField] private GameObject bubble;
        [SerializeField] private TMP_Text label;
        [SerializeField] private Image badge;
        [SerializeField] private Sprite romanianBadge;
        [SerializeField] private Sprite englishBadge;

        /// <summary>The words shown in the bubble now (the last line shown).</summary>
        public string Words => label.text;

        /// <inheritdoc/>
        public void Show(string text, Language language)
        {
            label.SetText(text);
            badge.sprite = language == Language.Romanian ? romanianBadge : englishBadge;
            bubble.SetActive(true);
        }

        /// <summary>
        /// Hides the bubble; nothing when the bubble is already gone (the voice stops as its scene
        /// closes, perhaps after the bubble's screen was destroyed).
        /// </summary>
        public void Hide()
        {
            if (bubble != null)
            {
                bubble.SetActive(false);
            }
        }

        private void Awake()
        {
            bubble.SetActive(false);
        }
    }
}
