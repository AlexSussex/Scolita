using System;
using System.Collections.Generic;

namespace Scolita.Core.Rewards
{
    /// <summary>
    /// Where a constellation's stars go: as many places as its planet's lessons give stars, spread over its figure
    /// (its own stars first, in order, then points along its lines, shared out by their length and evenly spaced on
    /// each), so a planet with more lessons fills the same figure more densely and any number of lessons fits.
    /// </summary>
    public static class ConstellationPlaces
    {
        /// <summary>
        /// The <paramref name="count"/> places of <paramref name="constellation"/>, in its frame, in the order they
        /// light. With fewer places than stars, the first stars; with no lines, the extra places go between
        /// consecutive stars. Called by <see cref="SkyChart"/> and <c>ConstellationPlacesTests</c>.
        /// </summary>
        public static IReadOnlyList<StarPoint> Spread(Constellation constellation, int count)
        {
            List<StarPoint> places = new List<StarPoint>(Math.Max(0, count));
            IReadOnlyList<StarPoint> stars = constellation.Stars;
            for (int index = 0; index < stars.Count && places.Count < count; index++)
            {
                places.Add(stars[index]);
            }

            int extra = count - places.Count;
            if (extra <= 0 || stars.Count < 2)
            {
                return places;
            }

            IReadOnlyList<StarLine> segments = constellation.Lines.Count > 0 ? constellation.Lines : Consecutive(stars.Count);
            int[] onEach = ShareOut(extra, segments, stars);
            for (int segment = 0; segment < segments.Count; segment++)
            {
                StarPoint from = stars[segments[segment].From];
                StarPoint to = stars[segments[segment].To];
                for (int step = 1; step <= onEach[segment]; step++)
                {
                    float share = step / (float)(onEach[segment] + 1);
                    places.Add(new StarPoint(from.X + (to.X - from.X) * share, from.Y + (to.Y - from.Y) * share));
                }
            }

            return places;
        }

        private static IReadOnlyList<StarLine> Consecutive(int starCount)
        {
            List<StarLine> lines = new List<StarLine>(starCount - 1);
            for (int index = 1; index < starCount; index++)
            {
                lines.Add(new StarLine(index - 1, index));
            }

            return lines;
        }

        private static int[] ShareOut(int extra, IReadOnlyList<StarLine> segments, IReadOnlyList<StarPoint> stars)
        {
            double[] lengths = new double[segments.Count];
            double total = 0d;
            for (int index = 0; index < segments.Count; index++)
            {
                StarPoint from = stars[segments[index].From];
                StarPoint to = stars[segments[index].To];
                lengths[index] = Math.Sqrt((to.X - from.X) * (double)(to.X - from.X) + (to.Y - from.Y) * (double)(to.Y - from.Y));
                total += lengths[index];
            }

            int[] counts = new int[segments.Count];
            double[] remainders = new double[segments.Count];
            int given = 0;
            for (int index = 0; index < segments.Count; index++)
            {
                double quota = total > 0d ? extra * lengths[index] / total : extra / (double)segments.Count;
                counts[index] = (int)Math.Floor(quota);
                remainders[index] = quota - counts[index];
                given += counts[index];
            }

            for (; given < extra; given++)
            {
                int largest = 0;
                for (int index = 1; index < segments.Count; index++)
                {
                    if (remainders[index] > remainders[largest])
                    {
                        largest = index;
                    }
                }

                counts[largest]++;
                remainders[largest] = -1d;
            }

            return counts;
        }
    }
}
