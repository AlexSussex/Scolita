using Scolita.Core.Languages;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Lesson
{
    /// <summary>
    /// The language badge (decision D6, design/README.md): the gold star "RO" for Romanian, the blue
    /// ringed planet "EN" for English, so a letter, word or picture card shows its language by a shape
    /// as well as a colour. Top right on every card.
    /// </summary>
    public sealed class LanguageBadge : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Sprite romanian;
        [SerializeField] private Sprite english;

        /// <summary>The language it shows.</summary>
        public Language Shown { get; private set; }

        /// <summary>Shows <paramref name="language"/>'s badge; called by the cards when a step begins.</summary>
        public void Show(Language language)
        {
            Shown = language;
            image.sprite = language == Language.Romanian ? romanian : english;
        }
    }
}
