using System.Linq;
using NUnit.Framework;
using Scolita.Core.Activities;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;
using Scolita.Core.Map;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Map
{
    /// <summary>Tests <see cref="MapRoute"/> on the real content, with every kind built so far.</summary>
    public sealed class MapRouteTests
    {
        /// <summary>Tests that <see cref="MapRoute.Planets"/> has a planet per unit with something to play, in the content's order.</summary>
        [Test]
        public void TestPlanets_OnePerUnitInContentOrder()
        {
            Curriculum curriculum = RealContent.Read();

            MapRoute route = MapRoutes.Of(curriculum, LanguageMix.Both);

            string[] units = curriculum.Domains[0].Units
                .Where(unit => !unit.Retired)
                .Where(unit => route.Stops.Any(stop => stop.Lesson.UnitId == unit.Id))
                .Select(unit => unit.Id.ToString())
                .ToArray();
            Assert.That(route.Planets.Select(planet => planet.Unit.Id.ToString()), Is.EqualTo(units));
            Assert.That(units.Take(3), Is.EqualTo(new[] { "planet-1", "planet-2", "planet-3" }));
            Assert.That(route.Planets.Select(planet => planet.Index), Is.EqualTo(Enumerable.Range(0, units.Length)));
        }

        /// <summary>Tests that <see cref="MapRoute.Stops"/> are a planet's playable lessons in teaching order, numbered across the map.</summary>
        [Test]
        public void TestStops_ThePlanetsLessonsInOrder()
        {
            MapRoute route = MapRoutes.Of(RealContent.Read(), LanguageMix.Both);

            MapPlanet first = route.Planets[0];
            Assert.That(first.Stops.Select(stop => stop.Lesson.Id.ToString()).Take(4),
                Is.EqualTo(new[] { "ro-a", "en-a", "ro-m", "en-m" }));
            Assert.That(first.Stops.Last().Lesson.Id.ToString(), Is.EqualTo("party-1"));
            Assert.That(route.Planets[1].Stops[0].Lesson.Id.ToString(), Is.EqualTo("ro-s"));
            Assert.That(route.Planets[1].Stops[0].Index, Is.EqualTo(first.Stops.Count));
            Assert.That(route.Planets[1].Stops[0].IndexOnPlanet, Is.EqualTo(0));
            Assert.That(route.Planets[1].Stops[0].PlanetIndex, Is.EqualTo(1));
            Assert.That(route.Stops.Select(stop => stop.Index), Is.EqualTo(Enumerable.Range(0, route.Stops.Count)));
        }

        /// <summary>Tests that with nothing done the first lesson is next, its planet awake and every other one asleep.</summary>
        [Test]
        public void TestNext_TheFirstLessonWhenNothingIsDone()
        {
            MapRoute route = MapRoutes.Of(RealContent.Read(), LanguageMix.Both);

            Assert.That(route.Next.Lesson.Id.ToString(), Is.EqualTo("ro-a"));
            Assert.That(route.GoStop, Is.SameAs(route.Next));
            Assert.That(route.AllDone, Is.False);
            Assert.That(route.Next.State, Is.EqualTo(StopState.Next));
            Assert.That(route.Stops.Skip(1).Select(stop => stop.State), Is.All.EqualTo(StopState.Ahead));
            Assert.That(route.Planets[0].State, Is.EqualTo(PlanetState.Awake));
            Assert.That(route.Planets.Skip(1).Select(planet => planet.State), Is.All.EqualTo(PlanetState.Sleepy));
            Assert.That(route.NextLetter.Id.ToString(), Is.EqualTo("a"));
        }

        /// <summary>Tests that <see cref="MapRoute.Next"/> is the first lesson not done, the ones before it done, and the letters met, current or ahead.</summary>
        [Test]
        public void TestNext_TheFirstNotDone()
        {
            MapRoute route = MapRoutes.Of(RealContent.Read(), LanguageMix.Both, "ro-a", "en-a");

            Assert.That(route.Next.Lesson.Id.ToString(), Is.EqualTo("ro-m"));
            Assert.That(route.Stops.Take(2).Select(stop => stop.State), Is.All.EqualTo(StopState.Done));
            Assert.That(route.Stops[3].State, Is.EqualTo(StopState.Ahead));
            Assert.That(route.Planets[0].State, Is.EqualTo(PlanetState.Awake));
            Assert.That(route.Planets[0].Letters.Select(letter => letter.Letter.Id.ToString()),
                Is.EqualTo(new[] { "a", "m", "t", "p" }));
            Assert.That(route.Planets[0].Letters.Select(letter => letter.State), Is.EqualTo(new[]
            {
                LetterState.Met, LetterState.Current, LetterState.Ahead, LetterState.Ahead
            }));
            Assert.That(route.NextLetter.Id.ToString(), Is.EqualTo("m"));
        }

        /// <summary>Tests that a letter's second lesson (its English one) keeps it current, and a stop with no new letter rings none.</summary>
        [Test]
        public void TestLetters_CurrentWhileTheNextStopIntroducesIt()
        {
            Curriculum curriculum = RealContent.Read();

            MapRoute englishA = MapRoutes.Before(curriculum, "en-a");
            MapRoute words = MapRoutes.Before(curriculum, "ro-words-1");

            Assert.That(englishA.Planets[0].Letters[0].State, Is.EqualTo(LetterState.Current));
            Assert.That(englishA.NextLetter.Id.ToString(), Is.EqualTo("a"));
            Assert.That(words.Planets[0].Letters.Select(letter => letter.State), Is.All.EqualTo(LetterState.Met));
            Assert.That(words.NextLetter, Is.Null);
        }

        /// <summary>Tests that a planet with every stop done beams, the next planet wakes and the one after sleeps.</summary>
        [Test]
        public void TestPlanetStates_AFinishedPlanetBeams()
        {
            MapRoute route = MapRoutes.Before(RealContent.Read(), "ro-s");

            Assert.That(route.Planets[0].State, Is.EqualTo(PlanetState.Beaming));
            Assert.That(route.Planets[1].State, Is.EqualTo(PlanetState.Awake));
            Assert.That(route.Planets[2].State, Is.EqualTo(PlanetState.Sleepy));
            Assert.That(route.Planets[2].IsAsleep, Is.True);
            Assert.That(route.Planets[0].Letters.Select(letter => letter.State), Is.All.EqualTo(LetterState.Met));
            Assert.That(route.Planets[2].Letters.Select(letter => letter.State), Is.All.EqualTo(LetterState.Ahead));
            Assert.That(route.GoStop.Lesson.Id.ToString(), Is.EqualTo("ro-s"));
        }

        /// <summary>Tests that <see cref="MapStop.CanOpen"/> holds for a stop done and the next stop, never for one ahead.</summary>
        [Test]
        public void TestCanOpen_DoneAndNextButNeverAhead()
        {
            MapRoute route = MapRoutes.Of(RealContent.Read(), LanguageMix.Both, "ro-a");

            Assert.That(route.Stops[0].CanOpen, Is.True);
            Assert.That(route.Stops[1].CanOpen, Is.True);
            Assert.That(route.Stops.Skip(2).Select(stop => stop.CanOpen), Is.All.False);
        }

        /// <summary>Tests that the map of one language leaves the other language's lessons off.</summary>
        [Test]
        public void TestStops_OnlyHerLanguages()
        {
            Curriculum curriculum = RealContent.Read();

            MapRoute romanian = MapRoutes.Of(curriculum, LanguageMix.RomanianOnly);
            MapRoute english = MapRoutes.Of(curriculum, LanguageMix.EnglishOnly);

            Assert.That(romanian.Stops.Select(stop => stop.Lesson.Languages), Has.None.EqualTo(LanguageMix.EnglishOnly));
            Assert.That(english.Stops.Select(stop => stop.Lesson.Languages), Has.None.EqualTo(LanguageMix.RomanianOnly));
            Assert.That(english.GoStop.Lesson.Id.ToString(), Is.EqualTo("en-a"));
        }

        /// <summary>Tests that with every stop done all planets beam, nothing is next, and Go opens the first stop again.</summary>
        [Test]
        public void TestAllDone_TheFirstStopAgain()
        {
            MapRoute route = MapRoutes.AllDone(RealContent.Read());

            Assert.That(route.AllDone, Is.True);
            Assert.That(route.Next, Is.Null);
            Assert.That(route.GoStop, Is.SameAs(route.Stops[0]));
            Assert.That(route.NextLetter, Is.Null);
            Assert.That(route.Planets.Select(planet => planet.State), Is.All.EqualTo(PlanetState.Beaming));
            Assert.That(route.Planets.SelectMany(planet => planet.Letters).Select(letter => letter.State),
                Is.All.EqualTo(LetterState.Met));
        }

        /// <summary>Tests that ids the content doesn't know are ignored.</summary>
        [Test]
        public void TestNext_UnknownDoneIdsIgnored()
        {
            MapRoute route = MapRoutes.Of(RealContent.Read(), LanguageMix.Both, "no-such-lesson", "ro-a");

            Assert.That(route.Next.Lesson.Id.ToString(), Is.EqualTo("en-a"));
        }

        /// <summary>Tests that a map with nothing to play has no planet, no stop and no go stop.</summary>
        [Test]
        public void TestPlanets_NothingToPlay()
        {
            Curriculum curriculum = RealContent.Read();
            Domain domain = curriculum.Domains[0];
            LessonOrder order = new LessonOrder(domain, LanguageMix.Both, new PlayableSteps(new ActivityKind[0], curriculum));

            MapRoute route = new MapRoute(curriculum, domain, order, MapRoutes.Progress(null));

            Assert.That(route.Planets, Is.Empty);
            Assert.That(route.Stops, Is.Empty);
            Assert.That(route.GoStop, Is.Null);
            Assert.That(route.AllDone, Is.False);
        }

        /// <summary>Tests that <see cref="MapRoute.TryFindStop"/> finds a stop on the map only.</summary>
        [Test]
        public void TestTryFindStop_OnTheMapOnly()
        {
            MapRoute route = MapRoutes.Of(RealContent.Read(), LanguageMix.RomanianOnly);

            Assert.That(route.TryFindStop(ContentId.Create("ro-t"), out MapStop found), Is.True);
            Assert.That(found.Lesson.Id.ToString(), Is.EqualTo("ro-t"));
            Assert.That(route.TryFindStop(ContentId.Create("en-t"), out MapStop _), Is.False);
            Assert.That(route.TryFindStop(ContentId.Create("no-such-lesson"), out MapStop _), Is.False);
        }
    }
}
