using System.Globalization;
using Scolita.Game.Rewards;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Rewards
{
    /// <summary>
    /// The map's bottom bar (design/screens.md, screen 1): a darker band with "Cerul meu" (the sky book, its star count
    /// in a yellow pill), "Abțibilduri" (the sticker book, once she has a sticker) and "Dâre" (the trails, once she has
    /// one). Its buttons go to <see cref="IRewardsInput"/>.
    /// </summary>
    public sealed class RewardBar : MonoBehaviour
    {
        [SerializeField] private GameObject band;
        [SerializeField] private Button skyButton;
        [SerializeField] private TMP_Text starCount;
        [SerializeField] private Button stickerButton;
        [SerializeField] private Button trailButton;

        private IRewardsInput input;

        /// <summary>The sky book's button.</summary>
        public Button SkyButton => skyButton;

        /// <summary>The sticker book's button.</summary>
        public Button StickerButton => stickerButton;

        /// <summary>The trails' button.</summary>
        public Button TrailButton => trailButton;

        /// <summary>The star count as shown.</summary>
        public string StarCount => starCount.text;

        /// <summary>Sends its taps to <paramref name="rewardsInput"/>; called by <see cref="RewardScreens"/>.</summary>
        public void Bind(IRewardsInput rewardsInput)
        {
            input = rewardsInput;
        }

        /// <summary>
        /// Shows the bar with <paramref name="stars"/>, the sticker book's button when she <paramref name="hasStickers"/>
        /// and the trails' when she <paramref name="hasTrails"/>; called by <see cref="RewardScreens"/>.
        /// </summary>
        public void Show(int stars, bool hasStickers, bool hasTrails)
        {
            band.SetActive(true);
            skyButton.gameObject.SetActive(true);
            starCount.SetText(stars.ToString(CultureInfo.InvariantCulture));
            stickerButton.gameObject.SetActive(hasStickers);
            trailButton.gameObject.SetActive(hasTrails);
        }

        private void Awake()
        {
            band.SetActive(false);
            skyButton.gameObject.SetActive(false);
            stickerButton.gameObject.SetActive(false);
            trailButton.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            skyButton.onClick.AddListener(OnSky);
            stickerButton.onClick.AddListener(OnStickers);
            trailButton.onClick.AddListener(OnTrails);
        }

        private void OnDisable()
        {
            skyButton.onClick.RemoveListener(OnSky);
            stickerButton.onClick.RemoveListener(OnStickers);
            trailButton.onClick.RemoveListener(OnTrails);
        }

        private void OnSky()
        {
            input?.OpenSkyBook();
        }

        private void OnStickers()
        {
            input?.OpenStickerBook();
        }

        private void OnTrails()
        {
            input?.OpenTrails();
        }
    }
}
