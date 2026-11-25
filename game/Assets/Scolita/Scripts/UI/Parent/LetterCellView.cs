using Scolita.Core.Progress;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Parent
{
    /// <summary>
    /// One letter of the parent area's grid (design screen 7): learned, filled with ink; learning, half filled (a lighter violet, so its ink letter reads on both halves); new,
    /// outlined, its letter grey. Made from a template by <see cref="ParentAreaScreen"/> when the area is drawn.
    /// </summary>
    public sealed class LetterCellView : MonoBehaviour
    {
        [SerializeField] private Image outline;
        [SerializeField] private Image fill;
        [SerializeField] private TMP_Text letter;
        [SerializeField] private Color ink = new Color32(0x2A, 0x21, 0x40, 0xFF);
        [SerializeField] private Color lightFill = new Color32(0xE4, 0xDC, 0xF5, 0xFF);
        [SerializeField] private Color learningHalf = new Color32(0xA9, 0x9A, 0xD9, 0xFF);
        [SerializeField] private Color newOutline = new Color32(0xC9, 0xBF, 0xE3, 0xFF);
        [SerializeField] private Color newLetter = new Color32(0x8A, 0x80, 0xA6, 0xFF);

        /// <summary>The state drawn.</summary>
        public ItemState State { get; private set; }

        /// <summary>Draws <paramref name="lower"/> in <paramref name="state"/>; called by <see cref="ParentAreaScreen"/>.</summary>
        public void Show(string lower, ItemState state)
        {
            State = state;
            letter.SetText(lower);
            switch (state)
            {
                case ItemState.Learned:
                    outline.color = ink;
                    fill.fillAmount = 1f;
                    fill.color = ink;
                    letter.color = Color.white;
                    break;
                case ItemState.Learning:
                    outline.color = lightFill;
                    fill.fillAmount = 0.5f;
                    fill.color = learningHalf;
                    letter.color = ink;
                    break;
                default:
                    outline.color = newOutline;
                    fill.fillAmount = 1f;
                    fill.color = Color.white;
                    letter.color = newLetter;
                    break;
            }
        }
    }
}
