using System.Collections.Generic;
using Scolita.Core.Languages;
using Scolita.Game.Rewards;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Rewards
{
    /// <summary>
    /// The end of a lesson (design/screens.md, screen 6): "Bravo!", the planet's constellation with the lesson's stars
    /// flying into their places one after another (each place lights as its star lands), then, when earned, the gift box
    /// opening and the new sticker popping out, and the new trail; "Mai departe" (the lesson screen's) and "În cartea
    /// mea" (the sticker book, when a sticker came). Moved in <c>Update</c> from cached values, allocating nothing.
    /// </summary>
    public sealed class LessonEndScreen : LessonEndView
    {
        [SerializeField] private TMP_Text title;
        [SerializeField] private string romanianTitle = "Bravo!";
        [SerializeField] private string englishTitle = "Well done!";
        [SerializeField] private GameObject constellationGroup;
        [SerializeField] private ConstellationView constellation;
        [SerializeField] private RectTransform flightStart;
        [SerializeField] private RectTransform flyerTemplate;
        [SerializeField] private GameObject giftGroup;
        [SerializeField] private RectTransform giftLid;
        [SerializeField] private RectTransform sticker;
        [SerializeField] private Image stickerPicture;
        [SerializeField] private GameObject trailCard;
        [SerializeField] private Image trailPicture;
        [SerializeField] private Button bookButton;
        [SerializeField] private float secondsPerStar = 0.9f;
        [SerializeField] private float secondsBetweenStars = 0.35f;
        [SerializeField] private float arcHeight = 120f;
        [SerializeField] private float giftOpensAfter = 2.2f;
        [SerializeField] private float giftSeconds = 0.8f;
        [SerializeField] private float lidLift = 110f;
        [SerializeField] private float stickerRise = 150f;

        private readonly List<RectTransform> flyers = new List<RectTransform>();
        private bool[] landed = new bool[0];
        private Vector2 lidClosed;
        private Vector2 stickerInBox;
        private IRewardsInput input;
        private LessonEnd shown;
        private float shownAt;

        /// <inheritdoc/>
        public override bool IsShown => gameObject.activeSelf;

        /// <summary>The constellation drawn.</summary>
        public ConstellationView Constellation => constellation;

        /// <summary>"În cartea mea".</summary>
        public Button BookButton => bookButton;

        /// <summary>Whether every star of the lesson has landed (and its place is lit).</summary>
        public bool AllLanded
        {
            get
            {
                for (int index = 0; index < landed.Length; index++)
                {
                    if (!landed[index])
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <inheritdoc/>
        public override void Show(LessonEnd end, IRewardsInput rewardsInput)
        {
            gameObject.SetActive(true);
            shown = end;
            input = rewardsInput;
            shownAt = Time.unscaledTime;
            title.SetText(end.Language == Language.English ? englishTitle : romanianTitle);
            constellationGroup.SetActive(end.Constellation != null);
            int flying = end.Constellation == null ? 0 : end.PlaceCount;
            if (end.Constellation != null)
            {
                constellation.Show(end.Constellation, end.Constellation.LitCount);
                for (int index = 0; index < flying; index++)
                {
                    constellation.SetLit(end.FirstPlace + index, false);
                }
            }

            while (flyers.Count < flying)
            {
                flyers.Add(Instantiate(flyerTemplate, flyerTemplate.parent));
            }

            landed = new bool[flying];
            for (int index = 0; index < flyers.Count; index++)
            {
                flyers[index].gameObject.SetActive(false);
            }

            giftGroup.SetActive(end.Sticker != null);
            stickerPicture.sprite = end.Sticker;
            giftLid.anchoredPosition = lidClosed;
            giftLid.localRotation = Quaternion.identity;
            sticker.anchoredPosition = stickerInBox;
            sticker.localScale = Vector3.zero;
            trailCard.SetActive(false);
            trailPicture.sprite = end.Trail;
            bookButton.gameObject.SetActive(end.Sticker != null && rewardsInput != null);
        }

        /// <inheritdoc/>
        public override void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Awake()
        {
            flyerTemplate.gameObject.SetActive(false);
            lidClosed = giftLid.anchoredPosition;
            stickerInBox = sticker.anchoredPosition;
        }

        private void OnEnable()
        {
            bookButton.onClick.AddListener(OnBook);
        }

        private void OnDisable()
        {
            bookButton.onClick.RemoveListener(OnBook);
        }

        private void Update()
        {
            if (shown == null)
            {
                return;
            }

            float elapsed = Time.unscaledTime - shownAt;
            for (int index = 0; index < landed.Length; index++)
            {
                MoveFlyer(index, (elapsed - index * secondsBetweenStars) / secondsPerStar);
            }

            if (shown.Sticker != null)
            {
                OpenGift(Mathf.Clamp01((elapsed - giftOpensAfter) / giftSeconds));
            }

            bool trailTime = elapsed >= giftOpensAfter + (shown.Sticker != null ? giftSeconds : 0f);
            if (shown.Trail != null && trailTime != trailCard.activeSelf)
            {
                trailCard.SetActive(trailTime);
            }
        }

        private void MoveFlyer(int index, float share)
        {
            RectTransform flyer = flyers[index];
            if (share <= 0f || landed[index])
            {
                return;
            }

            if (share >= 1f)
            {
                landed[index] = true;
                flyer.gameObject.SetActive(false);
                constellation.SetLit(shown.FirstPlace + index, true);
                return;
            }

            if (!flyer.gameObject.activeSelf)
            {
                flyer.gameObject.SetActive(true);
            }

            float eased = share * share * (3f - 2f * share);
            Vector3 from = flightStart.position;
            Vector3 to = constellation.Place(shown.FirstPlace + index).position;
            Vector3 position = Vector3.LerpUnclamped(from, to, eased);
            position.y += arcHeight * flyer.lossyScale.y * Mathf.Sin(share * Mathf.PI);
            flyer.position = position;
            float scale = Mathf.Lerp(1.6f, 0.6f, eased);
            flyer.localScale = new Vector3(scale, scale, 1f);
        }

        private void OpenGift(float share)
        {
            float eased = 1f - (1f - share) * (1f - share);
            giftLid.anchoredPosition = lidClosed + new Vector2(0f, lidLift * eased);
            giftLid.localRotation = Quaternion.Euler(0f, 0f, 18f * eased);
            float pop = share <= 0f ? 0f : Mathf.Min(1.15f, eased * 1.15f) - Mathf.Max(0f, share - 0.8f) * 0.75f;
            sticker.localScale = new Vector3(pop, pop, 1f);
            sticker.anchoredPosition = stickerInBox + new Vector2(0f, stickerRise * eased);
        }

        private void OnBook()
        {
            input?.OpenStickerBook();
        }
    }
}
