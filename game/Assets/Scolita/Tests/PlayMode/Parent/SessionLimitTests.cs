using System;
using System.Collections;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Game.Lessons;
using Scolita.Game.Scenes;
using Scolita.Tests.PlayMode.Lessons;
using Scolita.Tests.PlayMode.Map;
using Scolita.UI.Lesson;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Scolita.Tests.PlayMode.Parent
{
    /// <summary>
    /// Tests the daily limit in the scenes (<see cref="Game.Map.MapRunner"/>, <see cref="LessonRunner"/>): the map
    /// sleeps when it opens at the limit and when the limit is reached on it, Go and the stops then open nothing, a
    /// lesson begun always plays to its end, and the parent gate wakes the rocket. Each test keeps the Editor's own files.
    /// </summary>
    public sealed class SessionLimitTests
    {
        /// <summary>Tests that the map opens asleep at the limit: the mascot sleeps and Go opens no lesson.</summary>
        [UnityTest]
        public IEnumerator TestMap_AtTheLimitSleeps()
        {
            KeptFiles kept = ParentScene.Keep();
            try
            {
                yield return ParentScene.LeaveTheOpenScene();
                MapScene.SaveProgress(null);
                ParentScene.SaveLimit(10);
                ParentScene.SavePlayedToday(TimeSpan.FromMinutes(10));
                yield return MapScene.Open();

                MapScene.Screen().GoButton.onClick.Invoke();
                MapScene.Runner().TapStop(MapScene.Runner().Route.GoStop);
                for (int frame = 0; frame < 10; frame++)
                {
                    yield return null;
                }

                Assert.That(MapScene.Runner(), Is.Not.Null, "still on the map");
                Assert.That(MapScene.Runner().IsResting, Is.True);
                Assert.That(MapScene.Screen().Guide.Mood, Is.EqualTo(GuideMood.Sleepy));

                yield return ParentScene.LeaveTheOpenScene();
            }
            finally
            {
                ParentScene.Restore(kept);
            }
        }

        /// <summary>Tests that the limit reached while the map is shown puts it to sleep at once.</summary>
        [UnityTest]
        public IEnumerator TestMap_ReachingTheLimitSleeps()
        {
            KeptFiles kept = ParentScene.Keep();
            try
            {
                yield return ParentScene.LeaveTheOpenScene();
                MapScene.SaveProgress(null);
                ParentScene.SaveLimit(10);
                ParentScene.SavePlayedToday(TimeSpan.FromMinutes(10) - TimeSpan.FromSeconds(6));
                yield return MapScene.Open();
                bool restingAtFirst = MapScene.Runner().IsResting;

                yield return MapScene.WaitFor(() => MapScene.Runner().IsResting, 20f);

                Assert.That(restingAtFirst, Is.False);
                Assert.That(ParentScene.SavedPlayToday(), Is.GreaterThanOrEqualTo(TimeSpan.FromMinutes(10)));

                yield return ParentScene.LeaveTheOpenScene();
            }
            finally
            {
                ParentScene.Restore(kept);
            }
        }

        /// <summary>Tests that a lesson begun at the limit plays to its end, and the map after it sleeps.</summary>
        [UnityTest]
        public IEnumerator TestLesson_PlaysToItsEndAtTheLimit()
        {
            KeptFiles kept = ParentScene.Keep();
            try
            {
                yield return ParentScene.LeaveTheOpenScene();
                MapScene.SaveProgress(null);
                ParentScene.SavePlayedToday(TimeSpan.FromMinutes(10));
                LessonScene.SaveStartingFiles("ro-a");
                ParentScene.SaveLimit(10);
                yield return LessonScene.Open();
                LessonRunner lesson = SceneSearch.FindInActiveScene<LessonRunner>();

                yield return LessonScene.FinishLesson(lesson);
                SceneSearch.FindInActiveScene<LessonScreen>().ContinueButton.onClick.Invoke();
                yield return MapScene.WaitForMap();

                Assert.That(MapScene.SavedProgress().IsDone(ContentId.Create("ro-a")), Is.True);
                Assert.That(MapScene.Runner().IsResting, Is.True);

                yield return ParentScene.LeaveTheOpenScene();
            }
            finally
            {
                ParentScene.Restore(kept);
            }
        }

        /// <summary>Tests that passing the gate on a sleeping map wakes it: back from the parent area, Go opens a lesson again.</summary>
        [UnityTest]
        public IEnumerator TestGate_WakesTheRocket()
        {
            KeptFiles kept = ParentScene.Keep();
            try
            {
                yield return ParentScene.LeaveTheOpenScene();
                MapScene.SaveProgress(null);
                ParentScene.SaveLimit(10);
                ParentScene.SavePlayedToday(TimeSpan.FromMinutes(12));
                yield return MapScene.Open();
                Assert.That(MapScene.Runner().IsResting, Is.True);
                ParentScene.AskAtOnce();

                ParentScene.GatePanel().ChoiceButtons[ParentScene.RightPlace()].onClick.Invoke();
                yield return ParentScene.WaitForArea();
                ParentScene.Screen().BackButton.onClick.Invoke();
                yield return MapScene.WaitForMap();

                Assert.That(MapScene.Runner().IsResting, Is.False);
                MapScene.Screen().GoButton.onClick.Invoke();
                yield return SceneSearch.WaitForScene(AppScene.Lesson);
                Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(AppScene.Lesson.ToString()));

                yield return ParentScene.LeaveTheOpenScene();
            }
            finally
            {
                ParentScene.Restore(kept);
            }
        }
    }
}
