using System.Collections.Generic;
using Scolita.Core.Rewards;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Rewards
{
    /// <summary>
    /// A constellation drawn from its data (design screen 6, the sky book): its figure's faint lines and its places,
    /// each a small faint dotted ring or a lit gold star, fitted into its area (its frame keeps its shape; the lines and stars are anchored at the area's centre). The lines and
    /// stars are made when it is shown and reused when it is shown again; lighting a place allocates nothing.
    /// </summary>
    public sealed class ConstellationView : MonoBehaviour
    {
        [SerializeField] private RectTransform area;
        [SerializeField] private Image placeTemplate;
        [SerializeField] private Image lineTemplate;
        [SerializeField] private Sprite litStar;
        [SerializeField] private Sprite emptyStar;
        [SerializeField, Range(0.02f, 0.2f)] private float placeShare = 0.075f;
        [SerializeField, Range(0.002f, 0.05f)] private float lineShare = 0.012f;
        [SerializeField, Range(0.2f, 1f)] private float emptyScale = 0.6f;
        [SerializeField, Range(0f, 1f)] private float emptyAlpha = 0.55f;

        private readonly List<Image> places = new List<Image>();
        private readonly List<Image> lines = new List<Image>();
        private int shownPlaces;
        private float placeSize;

        /// <summary>How many places are shown.</summary>
        public int PlaceCount => shownPlaces;

        /// <summary>
        /// Draws <paramref name="constellation"/>'s lines and places, the first <paramref name="litCount"/> lit. Called by
        /// the sky book and the lesson's end when they open.
        /// </summary>
        public void Show(SkyConstellation constellation, int litCount)
        {
            Rect frame = FrameRect();
            placeSize = frame.height * placeShare;
            float lineWidth = Mathf.Max(2f, frame.height * lineShare);
            IReadOnlyList<StarPoint> stars = constellation.Constellation.Stars;
            IReadOnlyList<StarLine> figure = constellation.Constellation.Lines;
            for (int index = 0; index < figure.Count; index++)
            {
                Image line = Take(lines, lineTemplate, index);
                Vector2 from = InFrame(frame, stars[figure[index].From]);
                Vector2 to = InFrame(frame, stars[figure[index].To]);
                RectTransform rect = line.rectTransform;
                rect.anchoredPosition = (from + to) * 0.5f;
                rect.sizeDelta = new Vector2(Vector2.Distance(from, to) + lineWidth, lineWidth);
                rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(to.y - from.y, to.x - from.x) * Mathf.Rad2Deg);
            }

            HideFrom(lines, figure.Count);
            for (int index = 0; index < constellation.Places.Count; index++)
            {
                Image place = Take(places, placeTemplate, index);
                place.rectTransform.anchoredPosition = InFrame(frame, constellation.Places[index]);
                place.rectTransform.SetAsLastSibling();
                Paint(place, index < litCount);
            }

            shownPlaces = constellation.Places.Count;
            HideFrom(places, shownPlaces);
        }

        /// <summary>Lights or empties the place <paramref name="index"/>; called by the lesson's end as each star lands.</summary>
        public void SetLit(int index, bool lit)
        {
            if (index >= 0 && index < shownPlaces)
            {
                Paint(places[index], lit);
            }
        }

        /// <summary>Whether the place <paramref name="index"/> is lit; called by the tests.</summary>
        public bool IsLit(int index)
        {
            return index >= 0 && index < shownPlaces && places[index].sprite == litStar;
        }

        /// <summary>The place <paramref name="index"/>'s transform, for a star to fly to; called by the lesson's end.</summary>
        public RectTransform Place(int index)
        {
            return places[index].rectTransform;
        }

        private void Paint(Image place, bool lit)
        {
            float side = lit ? placeSize : placeSize * emptyScale;
            place.sprite = lit ? litStar : emptyStar;
            place.color = new Color(1f, 1f, 1f, lit ? 1f : emptyAlpha);
            place.rectTransform.sizeDelta = new Vector2(side, side);
        }

        private static Image Take(List<Image> pool, Image template, int index)
        {
            if (index >= pool.Count)
            {
                Image made = Instantiate(template, template.transform.parent);
                pool.Add(made);
            }

            Image image = pool[index];
            image.gameObject.SetActive(true);
            return image;
        }

        private static void HideFrom(List<Image> pool, int first)
        {
            for (int index = first; index < pool.Count; index++)
            {
                pool[index].gameObject.SetActive(false);
            }
        }

        private static Vector2 InFrame(Rect frame, StarPoint point)
        {
            return new Vector2(
                frame.xMin + frame.width * (point.X / Constellation.FrameWidth),
                frame.yMax - frame.height * (point.Y / Constellation.FrameHeight));
        }

        private Rect FrameRect()
        {
            Rect rect = area.rect;
            float frameAspect = Constellation.FrameWidth / Constellation.FrameHeight;
            float width = Mathf.Min(rect.width, rect.height * frameAspect);
            float height = width / frameAspect;
            return new Rect(-width * 0.5f, -height * 0.5f, width, height);
        }

        private void Awake()
        {
            placeTemplate.gameObject.SetActive(false);
            lineTemplate.gameObject.SetActive(false);
        }
    }
}
