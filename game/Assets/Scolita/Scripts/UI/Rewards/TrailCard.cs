using Scolita.Core.Content;
using Scolita.Game.Rewards;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scolita.UI.Rewards
{
    /// <summary>A trail's card in the trail picker: the trail's picture on cream, with a yellow ring when the rocket wears it.</summary>
    public sealed class TrailCard : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image picture;
        [SerializeField] private GameObject ring;

        private TrailPickerView picker;
        private ContentId trail;

        /// <summary>The trail shown.</summary>
        public ContentId Trail => trail;

        /// <summary>Whether its ring is shown.</summary>
        public bool IsChosen => ring.activeSelf;

        /// <summary>Shows <paramref name="choice"/>, ringed when <paramref name="chosen"/>; called by <see cref="TrailPickerView"/>.</summary>
        public void Show(TrailChoice choice, bool chosen, TrailPickerView owner)
        {
            picker = owner;
            trail = choice.Id;
            picture.sprite = choice.Picture;
            ring.SetActive(chosen);
        }

        /// <inheritdoc/>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (picker != null)
            {
                picker.CardTapped(trail);
            }
        }
    }
}
