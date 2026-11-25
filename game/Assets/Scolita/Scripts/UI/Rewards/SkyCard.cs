using Scolita.Core.Rewards;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scolita.UI.Rewards
{
    /// <summary>
    /// A planet's card in the sky book: its constellation over the planet's picture (dim before its first star, beaming
    /// once complete); a tap says its name.
    /// </summary>
    public sealed class SkyCard : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private ConstellationView constellation;
        [SerializeField] private Image planet;
        [SerializeField, Range(0f, 1f)] private float dimPlanet = 0.45f;

        private SkyBookView book;
        private int index;

        /// <summary>The constellation drawn.</summary>
        public ConstellationView Constellation => constellation;

        /// <summary>
        /// Shows <paramref name="sky"/> over <paramref name="planetPicture"/> as card <paramref name="cardIndex"/> of
        /// <paramref name="owner"/>; called by <see cref="SkyBookView"/>.
        /// </summary>
        public void Show(SkyConstellation sky, Sprite planetPicture, int cardIndex, SkyBookView owner)
        {
            book = owner;
            index = cardIndex;
            planet.sprite = planetPicture;
            planet.enabled = planetPicture != null;
            planet.color = new Color(1f, 1f, 1f, sky.LitCount > 0 ? 1f : dimPlanet);
            constellation.Show(sky, sky.LitCount);
        }

        /// <inheritdoc/>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (book != null)
            {
                book.CardTapped(index);
            }
        }
    }
}
