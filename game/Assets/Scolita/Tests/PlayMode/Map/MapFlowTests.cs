using System.Collections;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Map;
using Scolita.Game.Lessons;
using Scolita.Game.Map;
using Scolita.Game.Scenes;
using Scolita.Game.Voice;
using Scolita.Tests.PlayMode.Lessons;
using Scolita.UI.Lesson;
using Scolita.UI.Map;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Scolita.Tests.PlayMode.Map
{
    /// <summary>
    /// Tests the Map scene (<see cref="MapRunner"/>, <see cref="MapScreen"/>) on the real content with simulated
    /// taps: it opens with the rocket on the next lesson, Go opens it and finishing it moves the rocket one stop
    /// on, finishing a planet plays the trip, a tap skips the trip, a sleepy planet and a stop ahead never open,
    /// and a stop done opens again. Each test keeps the Editor's own <c>child-1</c> files as they were.
    /// </summary>
    public sealed class MapFlowTests
    {
        /// <summary>
        /// Tests that the map opens with the rocket on the first lesson not done (a hop from the stop shown
        /// before), its stop the big yellow one, the planet awake, and the stop saved as shown.
        /// </summary>
        [UnityTest]
        public IEnumerator TestMapRunner_OpensWithTheRocketOnTheNextLesson()
        {
            string[] kept = MapScene.KeepFiles();
            try
            {
                MapScene.SaveProgress("en-a", "ro-a", "en-a");
                yield return MapScene.Open();
                MapRunner runner = MapScene.Runner();
                MapScreen screen = MapScene.Screen();

                Assert.That(runner.Route.GoStop.Lesson.Id.ToString(), Is.EqualTo("ro-m"));
                Assert.That(screen.Stops[runner.Route.GoStop.Index].Stop.State, Is.EqualTo(StopState.Next));
                Assert.That(screen.Planets[0].Planet.State, Is.EqualTo(PlanetState.Awake));
                Assert.That(screen.Planets[1].Planet.State, Is.EqualTo(PlanetState.Sleepy));
                Assert.That(screen.Planets[0].Tiles.Count, Is.EqualTo(4));
                Assert.That(screen.Planets[0].Tiles[1].IsRinged, Is.True, "m is the letter being learned");
                Assert.That(screen.Rocket.IsHopping, Is.True);
                Assert.That(screen.GoButton.gameObject.activeSelf, Is.True);
                Assert.That(MapScene.SavedProgress().RocketStop.ToString(), Is.EqualTo("ro-m"));
            }
            finally
            {
                MapScene.RestoreFiles(kept);
            }
        }

        /// <summary>
        /// Tests Map → Go → (finish the lesson) → Map: Go opens the next lesson, finishing it records it done, and
        /// the map comes back with the rocket one stop further.
        /// </summary>
        [UnityTest]
        public IEnumerator TestMapRunner_GoThenFinishMovesTheRocketOn()
        {
            string[] kept = MapScene.KeepFiles();
            try
            {
                MapScene.SaveProgress("ro-a", "ro-a");
                yield return MapScene.Open();
                Assert.That(MapScene.Runner().Route.GoStop.Lesson.Id.ToString(), Is.EqualTo("en-a"));

                MapScene.Screen().GoButton.onClick.Invoke();
                yield return LessonScene.WaitUntil(() => SceneSearch.FindInActiveScene<LessonRunner>()?.Run != null);
                LessonRunner lesson = SceneSearch.FindInActiveScene<LessonRunner>();
                Assert.That(lesson.Run.Plan.Lesson.Id.ToString(), Is.EqualTo("en-a"));
                yield return LessonScene.FinishLesson(lesson);
                SceneSearch.FindInActiveScene<LessonScreen>().ContinueButton.onClick.Invoke();
                yield return MapScene.WaitForMap();

                Assert.That(MapScene.SavedProgress().IsDone(ContentId.Create("en-a")), Is.True);
                Assert.That(MapScene.Runner().Route.GoStop.Lesson.Id.ToString(), Is.EqualTo("ro-m"));
                Assert.That(MapScene.Screen().Rocket.IsHopping, Is.True);
                Assert.That(MapScene.Screen().IsTripPlaying, Is.False);
            }
            finally
            {
                MapScene.RestoreFiles(kept);
            }
        }

        /// <summary>
        /// Tests that finishing planet 1's last lesson plays the trip when the map comes back, and that the rocket
        /// lands on planet 2's first stop, with the landing said, once it ends.
        /// </summary>
        [UnityTest]
        public IEnumerator TestMapRunner_FinishingAPlanetPlaysTheTrip()
        {
            string[] kept = MapScene.KeepFiles();
            try
            {
                MapScene.SaveProgress("party-1", PlanetOneBeforeItsParty());
                yield return MapScene.Open();
                Assert.That(MapScene.Runner().Route.GoStop.Lesson.Id.ToString(), Is.EqualTo("party-1"));

                MapScene.Screen().GoButton.onClick.Invoke();
                yield return LessonScene.WaitUntil(() => SceneSearch.FindInActiveScene<LessonRunner>()?.Run != null);
                yield return LessonScene.FinishLesson(SceneSearch.FindInActiveScene<LessonRunner>());
                SceneSearch.FindInActiveScene<LessonScreen>().ContinueButton.onClick.Invoke();
                yield return MapScene.WaitForMap();

                Assert.That(MapScene.Screen().IsTripPlaying, Is.True);
                Assert.That(MapScene.Runner().Route.Planets[0].State, Is.EqualTo(PlanetState.Beaming));
                Assert.That(MapScene.SavedProgress().RocketStop.ToString(), Is.EqualTo("party-1"), "saved when it lands");
                yield return MapScene.WaitFor(() => !MapScene.Screen().IsTripPlaying);

                Assert.That(MapScene.SavedProgress().RocketStop.ToString(), Is.EqualTo("ro-s"));
                Assert.That(MapScene.Runner().Route.GoStop.PlanetIndex, Is.EqualTo(1));
            }
            finally
            {
                MapScene.RestoreFiles(kept);
            }
        }

        /// <summary>Tests that a tap on the trip skips it: the rocket lands at once and the mascot says the landing.</summary>
        [UnityTest]
        public IEnumerator TestTripView_ATapSkipsIt()
        {
            string[] kept = MapScene.KeepFiles();
            try
            {
                MapScene.SaveProgress("party-1", PlanetOne());
                yield return MapScene.Open();
                MapScreen screen = MapScene.Screen();
                Assert.That(screen.IsTripPlaying, Is.True);

                LessonScene.Tap(screen.Trip);

                Assert.That(screen.IsTripPlaying, Is.False);
                Assert.That(screen.Trip.gameObject.activeSelf, Is.False);
                Assert.That(MapScene.SavedProgress().RocketStop.ToString(), Is.EqualTo("ro-s"));
                Assert.That(SceneSearch.FindInActiveScene<VoiceHost>().Voice.Current.ToString(), Is.EqualTo("ro-line-planet-landed"));
            }
            finally
            {
                MapScene.RestoreFiles(kept);
            }
        }

        /// <summary>Tests that a sleepy planet and a stop ahead say "not yet" and never open their lesson.</summary>
        [UnityTest]
        public IEnumerator TestMapRunner_NotYetNeverOpens()
        {
            string[] kept = MapScene.KeepFiles();
            try
            {
                MapScene.SaveProgress(null);
                yield return MapScene.Open();
                MapScreen screen = MapScene.Screen();
                VoiceHost voice = SceneSearch.FindInActiveScene<VoiceHost>();

                LessonScene.Tap(screen.Planets[1]);
                string afterPlanet = voice.Voice.Current.ToString();
                LessonScene.Tap(screen.Stops[3]);
                string afterStop = voice.Voice.Current.ToString();
                for (int frame = 0; frame < 30; frame++)
                {
                    yield return null;
                }

                Assert.That(afterPlanet, Is.EqualTo("ro-line-planet-asleep"));
                Assert.That(afterStop, Is.EqualTo("ro-line-stop-ahead"));
                Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(AppScene.Map.ToString()));
                Assert.That(LessonScene.SavedLesson(), Is.Empty);
            }
            finally
            {
                MapScene.RestoreFiles(kept);
            }
        }

        /// <summary>Tests that a stop done opens its lesson again, from its start.</summary>
        [UnityTest]
        public IEnumerator TestMapRunner_ADoneStopOpensAgain()
        {
            string[] kept = MapScene.KeepFiles();
            try
            {
                MapScene.SaveProgress("ro-m", "ro-a", "en-a");
                yield return MapScene.Open();

                LessonScene.Tap(MapScene.Screen().Stops[0]);
                yield return LessonScene.WaitUntil(() => SceneSearch.FindInActiveScene<LessonRunner>()?.Run != null);

                Assert.That(SceneSearch.FindInActiveScene<LessonRunner>().Run.Plan.Lesson.Id.ToString(), Is.EqualTo("ro-a"));
                Assert.That(SceneSearch.FindInActiveScene<LessonRunner>().Run.StepIndex, Is.EqualTo(0));
            }
            finally
            {
                MapScene.RestoreFiles(kept);
            }
        }

        private static string[] PlanetOneBeforeItsParty()
        {
            return new[] { "ro-a", "en-a", "ro-m", "en-m", "ro-t", "en-t", "ro-p", "en-p", "ro-words-1", "en-words-1", "capitals-1" };
        }

        private static string[] PlanetOne()
        {
            return PlanetOneBeforeItsParty().Concat(new[] { "party-1" }).ToArray();
        }
    }
}
