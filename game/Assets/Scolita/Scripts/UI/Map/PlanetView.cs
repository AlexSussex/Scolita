using System.Collections.Generic;
using Scolita.Core.Map;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scolita.UI.Map
{
    /// <summary>
    /// A planet on the galaxy map (design/README.md → "Each planet is a character"): <c>look</c>'s whole picture
    /// for its colour and state (sleepy and dimmed, awake with its ring, beaming with its sparkle), larger when
    /// awake, and its letters on tiles below it. Its ball (about 60% of the picture, a little above its middle)
    /// takes the taps, which go to the map screen (a sleepy one says "not yet" and wobbles). Made from a template
    /// by <see cref="MapScreen"/>.
    /// </summary>
    public sealed class PlanetView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private RectTransform body;
        [SerializeField] private CanvasGroup group;
        [SerializeField] private Image picture;
        [SerializeField] private RectTransform ball;
        [SerializeField] private RectTransform tiles;
        [SerializeField] private LetterTile tileTemplate;
        [SerializeField] private float awakeSize = 280f;
        [SerializeField] private float otherSize = 230f;
        [SerializeField] private float ballShare = 0.6f;
        [SerializeField] private float ballRise = 0.05f;
        [SerializeField] private float tilesBelow = 14f;
        [SerializeField] private float sleepyAlpha = 0.55f;
        [SerializeField] private float wobbleSeconds = 0.5f;
        [SerializeField] private float wobbleDegrees = 6f;
        [SerializeField] private float wobblesPerSecond = 5f;

        private readonly List<LetterTile> letterTiles = new List<LetterTile>();
        private MapScreen screen;
        private Wobble wobble;

        /// <summary>The planet shown.</summary>
        public MapPlanet Planet { get; private set; }

        /// <summary>The letters' tiles, in order.</summary>
        public IReadOnlyList<LetterTile> Tiles => letterTiles;

        /// <summary>
        /// Shows <paramref name="planet"/> in <paramref name="look"/>'s colour, sending its taps to
        /// <paramref name="mapScreen"/>; called once by <see cref="MapScreen.Show"/>.
        /// </summary>
        public void Show(MapPlanet planet, PlanetLook look, MapScreen mapScreen)
        {
            Planet = planet;
            screen = mapScreen;
            wobble = new Wobble(wobbleSeconds, wobbleDegrees, wobblesPerSecond);
            float size = planet.State == PlanetState.Awake ? awakeSize : otherSize;
            body.sizeDelta = new Vector2(size, size);
            picture.sprite = look.PictureFor(planet.State);
            ball.sizeDelta = new Vector2(size * ballShare, size * ballShare);
            ball.anchoredPosition = new Vector2(0f, size * ballRise);
            tiles.anchoredPosition = new Vector2(0f, -size * 0.5f - tilesBelow);
            group.alpha = planet.IsAsleep ? sleepyAlpha : 1f;
            foreach (PlanetLetter letter in planet.Letters)
            {
                LetterTile tile = Instantiate(tileTemplate, tiles);
                tile.gameObject.SetActive(true);
                tile.Show(letter);
                letterTiles.Add(tile);
            }
        }

        /// <summary>Wobbles gently (a "not yet"); called by <see cref="MapScreen"/>.</summary>
        public void StartWobble()
        {
            wobble?.Start(Time.unscaledTime);
        }

        /// <summary>Sends the tap to the map screen; called by the EventSystem.</summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (screen != null && !eventData.dragging)
            {
                screen.PlanetTapped(this);
            }
        }

        private void Update()
        {
            if (wobble != null && wobble.TryGetAngle(Time.unscaledTime, out float angle))
            {
                body.localRotation = Quaternion.Euler(0f, 0f, angle);
            }
        }
    }
}
