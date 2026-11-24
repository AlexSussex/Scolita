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

        /// <inheritdoc/>
        public void Show(string text, Language language)
        {
            label.SetText(text);
            badge.sprite = language == Language.Romanian ? romanianBadge : englishBadge;
            bubble.SetActive(true);
        }

        /// <inheritdoc/>
        public void Hide()
        {
            bubble.SetActive(false);
        }

        private void Awake()
        {
            bubble.SetActive(false);
        }
    }
}
