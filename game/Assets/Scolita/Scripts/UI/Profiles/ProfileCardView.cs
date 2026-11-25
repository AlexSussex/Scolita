using System.Globalization;
using Scolita.Core.Profiles;
using Scolita.Game.Profiles;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Profiles
{
    /// <summary>
    /// A child's card on "Who's playing?" (design screen 9): a cream card with her avatar, her name under it and her
    /// stars with a gold star; a yellow ring when she played last. The whole card is the button.
    /// </summary>
    public sealed class ProfileCardView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image avatar;
        [SerializeField] private TMP_Text nameLabel;
        [SerializeField] private TMP_Text stars;
        [SerializeField] private GameObject ring;
        [SerializeField] private CanvasGroup group;
        [SerializeField, Range(0f, 1f)] private float dimmedAlpha = 0.45f;

        /// <summary>The child whose card it is.</summary>
        public ProfileId Id { get; private set; }

        /// <summary>The card's button.</summary>
        public Button Button => button;

        /// <summary>The name shown.</summary>
        public string NameShown => nameLabel.text;

        /// <summary>The stars count shown.</summary>
        public string StarsShown => stars.text;

        /// <summary>Whether the card has the yellow ring (she played last).</summary>
        public bool IsRinged => ring.activeSelf;

        /// <summary>Draws <paramref name="card"/>; called by <see cref="ProfilesScreen"/>.</summary>
        public void Show(ProfileCard card)
        {
            Id = card.Id;
            avatar.sprite = card.Avatar;
            nameLabel.SetText(card.Name);
            nameLabel.gameObject.SetActive(card.Name.Length > 0);
            stars.SetText(card.Stars.ToString(CultureInfo.InvariantCulture));
            ring.SetActive(card.PlayedLast);
            group.alpha = 1f;
        }

        /// <summary>Dims the card (another one was chosen) or lifts it; called by <see cref="ProfilesScreen"/>.</summary>
        public void SetChosen(bool isChosen)
        {
            group.alpha = isChosen ? 1f : dimmedAlpha;
            transform.localScale = isChosen ? new Vector3(1.06f, 1.06f, 1f) : Vector3.one;
        }
    }
}
