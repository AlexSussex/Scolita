using Scolita.Core.Languages;
using Scolita.Core.Map;
using Scolita.Game.Voice;
using UnityEngine;

namespace Scolita.Game.Map
{
    /// <summary>
    /// The galaxy map screen as <see cref="MapRunner"/> sees it (the planets, the stops, the rocket, the
    /// mascot and its bubble, Go, the trip): drawn by <c>Scolita.UI.Map.MapScreen</c> on
    /// <c>Prefabs/UI/MapScreen.prefab</c>.
    /// </summary>
    public abstract class MapScreenView : MonoBehaviour
    {
        /// <summary>The mascot's bubble, which shows every line said, in its words.</summary>
        public abstract SpeechBubbleView Bubble { get; }

        /// <summary>Whether the trip is playing.</summary>
        public abstract bool IsTripPlaying { get; }

        /// <summary>Sends the screen's taps to <paramref name="input"/>; called once by <see cref="MapRunner"/>.</summary>
        public abstract void Bind(IMapInput input);

        /// <summary>
        /// Draws <paramref name="route"/>'s planets and stops, Go's word in <paramref name="appLanguage"/>, and
        /// scrolls to the go stop. Called once by <see cref="MapRunner"/> when the map opens.
        /// </summary>
        public abstract void Show(MapRoute route, Language appLanguage);

        /// <summary>
        /// Brings the rocket to its stop as <paramref name="move"/> says: at once, along the path, or by the trip
        /// (which ends with <see cref="IMapInput.TripEnded"/>). Called once by <see cref="MapRunner"/> after
        /// <see cref="Show"/>.
        /// </summary>
        public abstract void MoveRocket(RocketMove move);

        /// <summary>Gives a gentle wobble to <paramref name="stop"/> (a "not yet"); called by <see cref="MapRunner"/>.</summary>
        public abstract void Wobble(MapStop stop);

        /// <summary>Gives a gentle wobble to <paramref name="planet"/> (a "not yet"); called by <see cref="MapRunner"/>.</summary>
        public abstract void Wobble(MapPlanet planet);

        /// <summary>Moves the mascot's mouth while it speaks; called every frame by <see cref="MapRunner"/>.</summary>
        public abstract void SetTalking(bool talking);
    }
}
