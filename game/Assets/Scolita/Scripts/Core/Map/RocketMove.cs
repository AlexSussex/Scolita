using Scolita.Core.Content;

namespace Scolita.Core.Map
{
    /// <summary>
    /// How the rocket comes to the go stop when the map opens, from the stop where the map last showed
    /// it (the <c>map-progress</c> save): a trip when the go stop is on a later planet (finishing a
    /// planet's last lesson), a hop along the path when it is a later stop on the same planet, else it is
    /// simply there. Deciding from what was last shown, not from "a lesson just ended", means a trip cut
    /// short by closing the app plays again, and a replayed lesson moves nothing.
    /// </summary>
    public readonly struct RocketMove
    {
        private RocketMove(RocketMoveKind kind, MapStop from, MapStop to)
        {
            Kind = kind;
            From = from;
            To = to;
        }

        /// <summary>Placed, a hop or a trip.</summary>
        public RocketMoveKind Kind { get; }

        /// <summary>The stop it leaves (for a hop or a trip); null when it is placed.</summary>
        public MapStop From { get; }

        /// <summary>The stop it ends on (the route's go stop); null when the map has no stop.</summary>
        public MapStop To { get; }

        /// <summary>
        /// The move on <paramref name="route"/> from the stop <paramref name="shownAt"/> (not set the first
        /// time) to its go stop. Called by <c>MapRunner</c> when the map opens, and by the tests.
        /// </summary>
        public static RocketMove Between(MapRoute route, ContentId shownAt)
        {
            MapStop to = route.GoStop;
            if (to == null || !shownAt.IsSet || !route.TryFindStop(shownAt, out MapStop from) || from == to)
            {
                return new RocketMove(RocketMoveKind.Placed, null, to);
            }

            if (from.PlanetIndex < to.PlanetIndex)
            {
                return new RocketMove(RocketMoveKind.Trip, from, to);
            }

            if (from.PlanetIndex == to.PlanetIndex && from.Index < to.Index)
            {
                return new RocketMove(RocketMoveKind.Hop, from, to);
            }

            return new RocketMove(RocketMoveKind.Placed, null, to);
        }
    }
}
