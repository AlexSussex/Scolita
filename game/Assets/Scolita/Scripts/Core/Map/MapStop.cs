using Scolita.Core.Content;

namespace Scolita.Core.Map
{
    /// <summary>A stop on the galaxy map's path: one lesson she can play. Made by <see cref="MapRoute"/>.</summary>
    public sealed class MapStop
    {
        /// <summary>Creates a stop; called by <see cref="MapRoute"/>.</summary>
        public MapStop(Lesson lesson, int planetIndex, int indexOnPlanet, int index, StopState state)
        {
            Lesson = lesson;
            PlanetIndex = planetIndex;
            IndexOnPlanet = indexOnPlanet;
            Index = index;
            State = state;
        }

        /// <summary>Its lesson.</summary>
        public Lesson Lesson { get; }

        /// <summary>Its planet's place on the map, from 0.</summary>
        public int PlanetIndex { get; }

        /// <summary>Its place among its planet's stops, from 0.</summary>
        public int IndexOnPlanet { get; }

        /// <summary>Its place among all the map's stops, from 0.</summary>
        public int Index { get; }

        /// <summary>Done, next or ahead.</summary>
        public StopState State { get; }

        /// <summary>Whether tapping it opens its lesson: a stop done, or the next one (never one ahead).</summary>
        public bool CanOpen => State != StopState.Ahead;
    }
}
