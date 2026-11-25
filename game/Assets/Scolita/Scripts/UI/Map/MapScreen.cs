using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Map;
using Scolita.Game.Lessons;
using Scolita.Game.Map;
using Scolita.Game.Parent;
using Scolita.Game.Rewards;
using Scolita.Game.Voice;
using Scolita.UI.Lesson;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Map
{
    /// <summary>
    /// The galaxy map (design/screens.md, screen 1), drawn from Core's <see cref="MapRoute"/>: the rainbow path
    /// across a map that scrolls sideways by drag, a planet per unit with its letters, the stops on the path,
    /// the rocket on the next stop, the mascot and its bubble, the big "Hai!" / "Go!" (its word from the string tables), the avatar button (to "Who's playing?") and the parent button;
    /// the trip plays over it. Its taps go to <see cref="IMapInput"/> (the Map scene's <c>MapRunner</c>). Everything is placed once
    /// when the map opens; nothing allocates per frame.
    /// </summary>
    public sealed class MapScreen : MapScreenView
    {
        [SerializeField] private ScrollRect scroll;
        [SerializeField] private RectTransform content;
        [SerializeField] private RainbowPath path;
        [SerializeField] private RectTransform planetLayer;
        [SerializeField] private RectTransform stopLayer;
        [SerializeField] private PlanetView planetTemplate;
        [SerializeField] private StopView stopTemplate;
        [SerializeField] private RocketView rocket;
        [SerializeField] private PlanetLook[] planetLooks = new PlanetLook[0];
        [SerializeField] private MapLayoutSettings layoutSettings = new MapLayoutSettings();
        [SerializeField] private GuideFigure guide;
        [SerializeField] private SpeechBubbleView bubble;
        [SerializeField] private Button goButton;
        [SerializeField] private TMP_Text goLabel;
        [SerializeField] private TripView trip;
        [SerializeField] private RewardsView rewards;
        [SerializeField] private ParentGateView parentGate;
        [SerializeField] private Button avatarButton;
        [SerializeField] private Image avatarImage;

        private readonly List<PlanetView> planets = new List<PlanetView>();
        private readonly List<StopView> stops = new List<StopView>();
        private IMapInput input;
        private MapLayout layout;

        /// <inheritdoc/>
        public override SpeechBubbleView Bubble => bubble;

        /// <inheritdoc/>
        public override RewardsView Rewards => rewards;

        /// <inheritdoc/>
        public override ParentGateView ParentGate => parentGate;

        /// <inheritdoc/>
        public override bool IsTripPlaying => trip.IsPlaying;

        /// <summary>The planets drawn, in map order.</summary>
        public IReadOnlyList<PlanetView> Planets => planets;

        /// <summary>The stops drawn, in the route's order.</summary>
        public IReadOnlyList<StopView> Stops => stops;

        /// <summary>The big "Hai!" / "Go!".</summary>
        public Button GoButton => goButton;

        /// <summary>Go's word, in the app's language.</summary>
        public string GoWord => goLabel.text;

        /// <summary>The mascot.</summary>
        public GuideFigure Guide => guide;

        /// <summary>The rocket.</summary>
        public RocketView Rocket => rocket;

        /// <summary>The trip.</summary>
        public TripView Trip => trip;

        /// <summary>The map's scrolling.</summary>
        public ScrollRect Scroll => scroll;

        /// <summary>The avatar button beside "Școlița".</summary>
        public Button AvatarButton => avatarButton;

        /// <inheritdoc/>
        public override void Bind(IMapInput mapInput)
        {
            input = mapInput;
            guide.Bind(mapInput);
        }

        /// <inheritdoc/>
        public override void ShowProfile(Sprite avatar, bool shown)
        {
            avatarImage.sprite = avatar;
            avatarButton.gameObject.SetActive(shown);
        }

        /// <inheritdoc/>
        public override void Show(MapRoute route)
        {
            layout = new MapLayout(route.Planets.Select(planet => planet.Stops.Count).ToArray(), layoutSettings);
            content.sizeDelta = new Vector2(layout.Width, content.sizeDelta.y);
            path.rectTransform.sizeDelta = new Vector2(layout.Width, path.rectTransform.sizeDelta.y);
            path.SetLine(layout.PathPoints());
            foreach (MapPlanet planet in route.Planets)
            {
                PlanetView view = Instantiate(planetTemplate, planetLayer);
                view.gameObject.SetActive(true);
                ((RectTransform)view.transform).anchoredPosition = layout.PlanetCentres[planet.Index];
                view.Show(planet, LookOf(planet), this);
                planets.Add(view);
            }

            foreach (MapStop stop in route.Stops)
            {
                StopView view = Instantiate(stopTemplate, stopLayer);
                view.gameObject.SetActive(true);
                ((RectTransform)view.transform).anchoredPosition = layout.StopPositions[stop.Index];
                view.Show(stop, this);
                stops.Add(view);
            }

            goButton.gameObject.SetActive(route.GoStop != null);
            ScrollTo(route.GoStop);
        }

        /// <inheritdoc/>
        public override void MoveRocket(RocketMove move)
        {
            if (move.To == null)
            {
                rocket.Hide();
                return;
            }

            Vector2 to = layout.StopPositions[move.To.Index];
            if (move.Kind == RocketMoveKind.Hop)
            {
                rocket.HopBetween(layout.StopPositions[move.From.Index], to);
                return;
            }

            rocket.PlaceAt(to);
            if (move.Kind == RocketMoveKind.Trip)
            {
                trip.Play(LookOf(planets[move.To.PlanetIndex].Planet).PictureFor(PlanetState.Awake), this);
            }
        }

        /// <inheritdoc/>
        public override void Wobble(MapStop stop)
        {
            stops[stop.Index].StartWobble();
        }

        /// <inheritdoc/>
        public override void Wobble(MapPlanet planet)
        {
            planets[planet.Index].StartWobble();
        }

        /// <inheritdoc/>
        public override void ShowResting()
        {
            guide.ShowMood(GuideMood.Sleepy);
        }

        /// <inheritdoc/>
        public override void SetTalking(bool talking)
        {
            guide.SetTalking(talking);
        }

        /// <summary>She tapped a stop; called by <see cref="StopView"/>.</summary>
        public void StopTapped(StopView view)
        {
            input?.TapStop(view.Stop);
        }

        /// <summary>She tapped a planet; called by <see cref="PlanetView"/>.</summary>
        public void PlanetTapped(PlanetView view)
        {
            input?.TapPlanet(view.Planet);
        }

        /// <summary>The trip ended or was skipped; called by <see cref="TripView"/>.</summary>
        public void TripFinished()
        {
            input?.TripEnded();
        }

        private void Awake()
        {
            goButton.gameObject.SetActive(false);
            avatarButton.gameObject.SetActive(false);
        }

        private void OnAvatarClicked()
        {
            input?.SwitchProfile();
        }

        private void OnEnable()
        {
            goButton.onClick.AddListener(OnGoClicked);
            avatarButton.onClick.AddListener(OnAvatarClicked);
        }

        private void OnDisable()
        {
            goButton.onClick.RemoveListener(OnGoClicked);
            avatarButton.onClick.RemoveListener(OnAvatarClicked);
        }

        private void OnGoClicked()
        {
            input?.Go();
        }

        private PlanetLook LookOf(MapPlanet planet)
        {
            return planetLooks[planet.Index % planetLooks.Length];
        }

        private void ScrollTo(MapStop stop)
        {
            Canvas.ForceUpdateCanvases();
            float visible = scroll.viewport.rect.width;
            float hidden = layout.Width - visible;
            if (stop == null || hidden <= 0f)
            {
                scroll.horizontalNormalizedPosition = 0f;
                return;
            }

            float between = Mathf.Lerp(layout.StopPositions[stop.Index].x, layout.PlanetCentres[stop.PlanetIndex].x, 0.5f);
            scroll.horizontalNormalizedPosition = Mathf.Clamp01((between - visible * 0.5f) / hidden);
        }
    }
}
