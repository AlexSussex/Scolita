using System.Collections.Generic;
using Scolita.Core.Content;

namespace Scolita.Core.Map
{
    /// <summary>A planet of the galaxy map: one unit of the domain, its stops and its letters. Made by <see cref="MapRoute"/>.</summary>
    public sealed class MapPlanet
    {
        /// <summary>Creates a planet; called by <see cref="MapRoute"/>.</summary>
        public MapPlanet(Unit unit, int index, PlanetState state, IReadOnlyList<MapStop> stops,
            IReadOnlyList<PlanetLetter> letters)
        {
            Unit = unit;
            Index = index;
            State = state;
            Stops = stops;
            Letters = letters;
        }

        /// <summary>Its unit.</summary>
        public Unit Unit { get; }

        /// <summary>Its place on the map, from 0 (its colour follows it).</summary>
        public int Index { get; }

        /// <summary>Sleepy, awake or beaming.</summary>
        public PlanetState State { get; }

        /// <summary>Its stops, in teaching order (never empty).</summary>
        public IReadOnlyList<MapStop> Stops { get; }

        /// <summary>The letters its stops introduce, in the order she meets them.</summary>
        public IReadOnlyList<PlanetLetter> Letters { get; }

        /// <summary>Whether tapping it says "not yet" (a sleepy planet never opens).</summary>
        public bool IsAsleep => State == PlanetState.Sleepy;
    }
}
