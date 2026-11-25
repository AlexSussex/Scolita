using System.Collections.Generic;
using Scolita.Core.Content;
using Scolita.Game.Rewards;
using UnityEngine;

namespace Scolita.UI.Rewards
{
    /// <summary>
    /// The trail picker ("Dâre"): a card per trail she has earned, the one the rocket wears with a yellow ring; tapping a
    /// card gives the rocket that trail. The cards are made when it first opens and reused.
    /// </summary>
    public sealed class TrailPickerView : BookPage
    {
        [SerializeField] private TrailCard cardTemplate;

        private readonly List<TrailCard> cards = new List<TrailCard>();

        /// <summary>The cards shown, in the order earned.</summary>
        public IReadOnlyList<TrailCard> Cards => cards;

        /// <summary>Opens the picker on <paramref name="trails"/>, <paramref name="chosen"/> ringed; called by <see cref="RewardScreens"/>.</summary>
        public void Show(IReadOnlyList<TrailChoice> trails, ContentId chosen)
        {
            Open();
            for (int index = 0; index < trails.Count; index++)
            {
                if (index >= cards.Count)
                {
                    cards.Add(Instantiate(cardTemplate, cardTemplate.transform.parent));
                }

                cards[index].gameObject.SetActive(true);
                cards[index].Show(trails[index], trails[index].Id == chosen, this);
            }

            for (int index = trails.Count; index < cards.Count; index++)
            {
                cards[index].gameObject.SetActive(false);
            }
        }

        /// <summary>She tapped <paramref name="trail"/>; called by <see cref="TrailCard"/>.</summary>
        public void CardTapped(ContentId trail)
        {
            Input?.ChooseTrail(trail);
        }

        private void Awake()
        {
            cardTemplate.gameObject.SetActive(false);
        }
    }
}
