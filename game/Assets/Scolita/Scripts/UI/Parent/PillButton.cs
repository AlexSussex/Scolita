using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Parent
{
    /// <summary>
    /// One pill of a choice in the parent area's settings (design screen 7: the chosen one filled with ink, the others
    /// light, with ink words). Its taps go to whoever listens to <see cref="Button"/>.
    /// </summary>
    public sealed class PillButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image fill;
        [SerializeField] private TMP_Text label;
        [SerializeField] private Color chosenFill = new Color32(0x2A, 0x21, 0x40, 0xFF);
        [SerializeField] private Color chosenText = Color.white;
        [SerializeField] private Color otherFill = new Color32(0xEE, 0xE8, 0xFA, 0xFF);
        [SerializeField] private Color otherText = new Color32(0x2A, 0x21, 0x40, 0xFF);

        /// <summary>The pill's button.</summary>
        public Button Button => button;

        /// <summary>The pill's words.</summary>
        public string Words => label.text;

        /// <summary>Whether it is drawn as the chosen one.</summary>
        public bool IsChosen { get; private set; }

        /// <summary>Shows <paramref name="words"/>; called by the parent area screen.</summary>
        public void SetWords(string words)
        {
            label.SetText(words);
        }

        /// <summary>Draws it chosen or not; called by the parent area screen.</summary>
        public void SetChosen(bool chosen)
        {
            IsChosen = chosen;
            fill.color = chosen ? chosenFill : otherFill;
            label.color = chosen ? chosenText : otherText;
        }
    }
}
