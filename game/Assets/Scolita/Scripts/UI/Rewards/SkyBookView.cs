using System.Collections.Generic;
using Scolita.Core.Rewards;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Rewards
{
    /// <summary>
    /// The sky book ("Cerul meu"): a card per planet, its constellation lit star by star over its planet's picture (dim
    /// until its first star, beaming once complete), in a row that scrolls sideways; tapping a card says the constellation's name. The cards are
    /// made when it first opens and reused.
    /// </summary>
    public sealed class SkyBookView : BookPage
    {
        [SerializeField] private SkyCard cardTemplate;
        [SerializeField] private ScrollRect scroll;
        [SerializeField] private Sprite[] planetPictures = new Sprite[0];
        [SerializeField] private Sprite[] finishedPlanetPictures = new Sprite[0];

        private readonly List<SkyCard> cards = new List<SkyCard>();

        /// <summary>The cards shown, in planet order.</summary>
        public IReadOnlyList<SkyCard> Cards => cards;

        /// <summary>Opens the book on <paramref name="sky"/>; called by <see cref="RewardScreens"/>.</summary>
        public void Show(SkyChart sky)
        {
            Open();
            for (int index = 0; index < sky.Constellations.Count; index++)
            {
                if (index >= cards.Count)
                {
                    cards.Add(Instantiate(cardTemplate, cardTemplate.transform.parent));
                }

                SkyConstellation constellation = sky.Constellations[index];
                Sprite[] pictures = constellation.IsComplete ? finishedPlanetPictures : planetPictures;
                Sprite planet = pictures.Length == 0 ? null : pictures[constellation.PlanetIndex % pictures.Length];
                cards[index].gameObject.SetActive(true);
                cards[index].Show(constellation, planet, index, this);
            }

            for (int index = sky.Constellations.Count; index < cards.Count; index++)
            {
                cards[index].gameObject.SetActive(false);
            }

            scroll.horizontalNormalizedPosition = 0f;
        }

        /// <summary>She tapped the card number <paramref name="index"/>; called by <see cref="SkyCard"/>.</summary>
        public void CardTapped(int index)
        {
            Input?.TapConstellation(index);
        }

        private void Awake()
        {
            cardTemplate.gameObject.SetActive(false);
        }
    }
}
