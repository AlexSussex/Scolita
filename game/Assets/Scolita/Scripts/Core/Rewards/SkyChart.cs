using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Lessons;

namespace Scolita.Core.Rewards
{
    /// <summary>
    /// Her sky book: a constellation per planet of her lessons (the units of the <see cref="LessonOrder"/>, so her
    /// language mix, in map order), each with its places and the ones lit by her stars (<see cref="SkyConstellation"/>).
    /// </summary>
    public sealed class SkyChart
    {
        private SkyChart(IReadOnlyList<SkyConstellation> constellations)
        {
            Constellations = constellations;
        }

        /// <summary>The constellations, in the order of their planets.</summary>
        public IReadOnlyList<SkyConstellation> Constellations { get; }

        /// <summary>
        /// The sky of <paramref name="book"/>: for each planet of <paramref name="order"/> that has a constellation in
        /// <paramref name="content"/>, <see cref="RewardRules.StarsPerLesson"/> places per lesson of the planet in the
        /// order, lit by the planet's lessons with stars in the order she lit them (a lesson outside the order, after a
        /// change of her language mix, is left out, and kept). Called by <c>RewardsSession</c> and <c>SkyChartTests</c>.
        /// </summary>
        public static SkyChart Of(RewardContent content, LessonOrder order, RewardBook book, RewardRules rules)
        {
            List<SkyConstellation> constellations = new List<SkyConstellation>();
            ContentId[] units = order.Playable
                .Select(lesson => lesson.UnitId)
                .Distinct()
                .ToArray();
            for (int planet = 0; planet < units.Length; planet++)
            {
                if (!content.TryGetConstellation(units[planet], out Constellation constellation))
                {
                    continue;
                }

                HashSet<ContentId> planetLessons = new HashSet<ContentId>(order.Playable
                    .Where(lesson => lesson.UnitId == units[planet])
                    .Select(lesson => lesson.Id));
                ContentId[] lit = book.LitLessons
                    .Where(planetLessons.Contains)
                    .ToArray();
                IReadOnlyList<StarPoint> places = ConstellationPlaces.Spread(constellation, planetLessons.Count * rules.StarsPerLesson);
                constellations.Add(new SkyConstellation(constellation, planet, places, lit, rules.StarsPerLesson));
            }

            return new SkyChart(constellations);
        }

        /// <summary>The constellation of the unit <paramref name="unitId"/>; false when it isn't in this sky. Called by the lesson's end.</summary>
        public bool TryGet(ContentId unitId, out SkyConstellation constellation)
        {
            constellation = Constellations.FirstOrDefault(candidate => candidate.Constellation.UnitId == unitId);
            return constellation != null;
        }
    }
}
