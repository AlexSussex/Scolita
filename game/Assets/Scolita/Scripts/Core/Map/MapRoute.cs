using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Lessons;

namespace Scolita.Core.Map
{
    /// <summary>
    /// The galaxy map as she sees it (PRD §3.1), from the content and what she has done: a planet per
    /// unit she can play something on, its stops (the lessons <see cref="LessonOrder"/> lets her play,
    /// in teaching order), the next stop (the first not done: the rocket stands on it and Go opens it),
    /// and each planet's state and letters. Built by the Map scene's composition root each time the map
    /// opens, and by the tests.
    /// </summary>
    public sealed class MapRoute
    {
        private readonly Dictionary<ContentId, MapStop> stopsByLesson = new Dictionary<ContentId, MapStop>();

        /// <summary>
        /// Lays out <paramref name="domain"/>'s units and the lessons of <paramref name="order"/> on them,
        /// with <paramref name="done"/> saying which are finished; letters are read from
        /// <paramref name="curriculum"/>. Called by <c>MapRunner</c> and the tests.
        /// </summary>
        public MapRoute(Curriculum curriculum, Domain domain, LessonOrder order, IDoneLessons done)
        {
            Lesson[] lessons = order.Playable.ToArray();
            int nextIndex = FirstNotDone(lessons, done);
            AllDone = lessons.Length > 0 && nextIndex < 0;
            ContentId nextId = nextIndex >= 0 ? lessons[nextIndex].Id : default;
            Next = null;

            List<MapStop> stops = new List<MapStop>();
            List<MapPlanet> planets = new List<MapPlanet>();
            foreach (Unit unit in domain.Units.Where(unit => !unit.Retired))
            {
                Lesson[] onUnit = lessons.Where(lesson => lesson.UnitId == unit.Id).ToArray();
                if (onUnit.Length == 0)
                {
                    continue;
                }

                List<MapStop> planetStops = new List<MapStop>();
                foreach (Lesson lesson in onUnit)
                {
                    int index = stops.Count;
                    StopState state = done.IsDone(lesson.Id) ? StopState.Done
                        : lesson.Id == nextId ? StopState.Next
                        : StopState.Ahead;
                    MapStop stop = new MapStop(lesson, planets.Count, planetStops.Count, index, state);
                    planetStops.Add(stop);
                    stops.Add(stop);
                    stopsByLesson[lesson.Id] = stop;
                    if (state == StopState.Next)
                    {
                        Next = stop;
                    }
                }

                planets.Add(new MapPlanet(unit, planets.Count, PlanetStateOf(planetStops), planetStops,
                    LettersOf(curriculum, planetStops, AllDone)));
            }

            Planets = planets;
            Stops = stops;
            GoStop = Next ?? (stops.Count > 0 ? stops[0] : null);
            NextLetter = Next != null && Next.Lesson.Introduces.IsSet
                && curriculum.Letters.TryGet(Next.Lesson.Introduces, out Letter letter)
                    ? letter
                    : null;
        }

        /// <summary>The planets, in map order; empty when she can play nothing.</summary>
        public IReadOnlyList<MapPlanet> Planets { get; }

        /// <summary>Every stop, planet after planet, in teaching order.</summary>
        public IReadOnlyList<MapStop> Stops { get; }

        /// <summary>The first stop not done; null when every stop is done (or there is none).</summary>
        public MapStop Next { get; }

        /// <summary>
        /// Where the rocket stands and what Go opens: the next stop, or the first stop again when every
        /// stop is done; null only when there is no stop at all.
        /// </summary>
        public MapStop GoStop { get; }

        /// <summary>Whether she has done every stop (and there is at least one).</summary>
        public bool AllDone { get; }

        /// <summary>The letter the next stop introduces; null for a stop with no new letter, or when all are done.</summary>
        public Letter NextLetter { get; }

        /// <summary>
        /// Finds the stop of the lesson <paramref name="lessonId"/>; false when it isn't on the map (unknown,
        /// retired, outside her languages, nothing to play). Called by <see cref="RocketMove.Between"/>,
        /// <c>MapRunner</c> and the tests.
        /// </summary>
        public bool TryFindStop(ContentId lessonId, out MapStop stop)
        {
            return stopsByLesson.TryGetValue(lessonId, out stop);
        }

        private static int FirstNotDone(IReadOnlyList<Lesson> lessons, IDoneLessons done)
        {
            for (int index = 0; index < lessons.Count; index++)
            {
                if (!done.IsDone(lessons[index].Id))
                {
                    return index;
                }
            }

            return -1;
        }

        private static PlanetState PlanetStateOf(IReadOnlyList<MapStop> stops)
        {
            if (stops.All(stop => stop.State == StopState.Done))
            {
                return PlanetState.Beaming;
            }

            return stops.Any(stop => stop.State == StopState.Next) ? PlanetState.Awake : PlanetState.Sleepy;
        }

        private static int FirstNotMet(IReadOnlyList<MapStop> stops)
        {
            foreach (MapStop stop in stops)
            {
                if (stop.State != StopState.Done)
                {
                    return stop.IndexOnPlanet;
                }
            }

            return stops.Count;
        }

        private static IReadOnlyList<PlanetLetter> LettersOf(Curriculum curriculum, IReadOnlyList<MapStop> stops, bool allDone)
        {
            List<PlanetLetter> letters = new List<PlanetLetter>();
            HashSet<ContentId> seen = new HashSet<ContentId>();
            ContentId[] introducedNext = stops
                .Where(stop => stop.State == StopState.Next)
                .Select(stop => stop.Lesson.Introduces)
                .ToArray();
            int firstNotMet = FirstNotMet(stops);
            foreach (MapStop stop in stops)
            {
                ContentId letterId = stop.Lesson.Introduces;
                if (!letterId.IsSet || !seen.Add(letterId) || !curriculum.Letters.TryGet(letterId, out Letter letter))
                {
                    continue;
                }

                LetterState state = allDone ? LetterState.Met
                    : introducedNext.Contains(letterId) ? LetterState.Current
                    : stop.IndexOnPlanet < firstNotMet ? LetterState.Met
                    : LetterState.Ahead;
                letters.Add(new PlanetLetter(letter, state));
            }

            return letters;
        }
    }
}
