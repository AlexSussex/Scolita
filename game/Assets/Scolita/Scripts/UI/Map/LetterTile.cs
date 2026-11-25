using Scolita.Core.Map;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Map
{
    /// <summary>
    /// A letter standing below its planet on a cream tile (design/screens.md, screen 1): full when met, ringed
    /// in yellow when the next stop teaches it, faint when it is still to come. Set once when the map opens.
    /// </summary>
    public sealed class LetterTile : MonoBehaviour
    {
        [SerializeField] private CanvasGroup group;
        [SerializeField] private Image ring;
        [SerializeField] private TMP_Text label;
        [SerializeField] private float aheadAlpha = 0.55f;

        /// <summary>The letter shown.</summary>
        public string Shown => label.text;

        /// <summary>Whether it is ringed (the next stop teaches it).</summary>
        public bool IsRinged => ring.enabled;

        /// <summary>Shows <paramref name="letter"/>; called by <see cref="PlanetView.Show"/>.</summary>
        public void Show(PlanetLetter letter)
        {
            label.SetText(letter.Letter.Lower);
            ring.enabled = letter.State == LetterState.Current;
            group.alpha = letter.State == LetterState.Ahead ? aheadAlpha : 1f;
        }
    }
}
