using Scolita.Core.Content;

namespace Scolita.Core.Map
{
    /// <summary>A stop on the galaxy map's path: one lesson she can play. Made by <see cref="MapRoute"/>.</summary>
    public sealed class MapStop
    {
        /// <summary>
        /// Creates a stop; <paramref name="opensAhead"/> lets it open even ahead of the rocket (the tester's map). Called by
        /// <see cref="MapRoute"/>.
        /// </summary>
        public MapStop(Lesson lesson, int planetIndex, int indexOnPlanet, int index, StopState state, bool opensAhead)
        {
            OpensAhead = opensAhead;
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

        /// <summary>Whether it opens even when it is ahead of the rocket (every stop on the tester's map).</summary>
        public bool OpensAhead { get; }

        /// <summary>
        /// Whether tapping it opens its lesson: a stop done, or the next one (never one ahead, except on the tester's
        /// map).
        /// </summary>
        public bool CanOpen => OpensAhead || State != StopState.Ahead;
    }
}
