using System.Collections.Generic;
using Scolita.Core.Content;

namespace Scolita.Core.Rewards
{
    /// <summary>
    /// One planet's constellation in her sky: its figure, its places (<see cref="ConstellationPlaces"/>: the stars per
    /// lesson for each of the planet's lessons in her order), and which are lit: the places of the lessons with stars,
    /// in the order she lit them, from the first place on. Made by <see cref="SkyChart"/>.
    /// </summary>
    public sealed class SkyConstellation
    {
        private readonly IReadOnlyList<ContentId> litLessons;
        private readonly int starsPerLesson;

        /// <summary>Creates the constellation's state; called by <see cref="SkyChart"/>.</summary>
        public SkyConstellation(Constellation constellation, int planetIndex, IReadOnlyList<StarPoint> places,
            IReadOnlyList<ContentId> litLessons, int starsPerLesson)
        {
            Constellation = constellation;
            PlanetIndex = planetIndex;
            Places = places;
            this.litLessons = litLessons;
            this.starsPerLesson = starsPerLesson;
            LitCount = System.Math.Min(places.Count, litLessons.Count * starsPerLesson);
        }

        /// <summary>The constellation.</summary>
        public Constellation Constellation { get; }

        /// <summary>Its planet's place on the map, from 0 (the planets in the order of her lessons).</summary>
        public int PlanetIndex { get; }

        /// <summary>Its places, in its frame, in the order they light.</summary>
        public IReadOnlyList<StarPoint> Places { get; }

        /// <summary>How many places are lit, from the first.</summary>
        public int LitCount { get; }

        /// <summary>Whether every place is lit: every lesson of its planet has its stars.</summary>
        public bool IsComplete => LitCount >= Places.Count;

        /// <summary>
        /// The places the lesson <paramref name="lessonId"/>'s stars lit: the first and how many; false when it has
        /// no stars here. Called by the lesson's end, to fly the stars to them.
        /// </summary>
        public bool TryGetPlacesOf(ContentId lessonId, out int first, out int count)
        {
            for (int index = 0; index < litLessons.Count; index++)
            {
                if (litLessons[index] == lessonId)
                {
                    first = index * starsPerLesson;
                    count = System.Math.Max(0, System.Math.Min(starsPerLesson, Places.Count - first));
                    return count > 0;
                }
            }

            first = 0;
            count = 0;
            return false;
        }
    }
}
