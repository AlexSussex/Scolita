using System.Collections;
using NUnit.Framework;
using Scolita.Game.Boot;
using Scolita.Game.Lessons;
using Scolita.Game.Scenes;
using Scolita.UI.Lesson;
using Scolita.UI.Map;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Scolita.Tests.PlayMode
{
    /// <summary>
    /// Tests the placeholder scene flow: <see cref="BootLoader"/> opens the Map, the
    /// <see cref="MapScreen"/>'s Go button opens the Lesson, and the <see cref="LessonHud"/>'s home
    /// button returns to the Map.
    /// </summary>
    public sealed class SceneFlowTests
    {
        /// <summary>Tests that <see cref="BootLoader"/> opens the Map by itself, letting the screen sleep.</summary>
        [UnityTest]
        public IEnumerator TestBootLoader_OpensTheMap()
        {
            SceneManager.LoadScene(AppScene.Boot.ToString());

            yield return SceneSearch.WaitForScene(AppScene.Map);

            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(AppScene.Map.ToString()));
            Assert.That(SceneSearch.FindInActiveScene<MapScreen>(), Is.Not.Null);
            Assert.That(Screen.sleepTimeout, Is.EqualTo(SleepTimeout.SystemSetting));
        }

        /// <summary>Tests that the <see cref="MapScreen"/>'s Go button opens the Lesson, which keeps the screen awake.</summary>
        [UnityTest]
        public IEnumerator TestMapScreen_GoOpensTheLesson()
        {
            SceneManager.LoadScene(AppScene.Map.ToString());
            yield return SceneSearch.WaitForScene(AppScene.Map);

            SceneSearch.FindInActiveScene<MapScreen>().GetComponentInChildren<Button>().onClick.Invoke();
            yield return SceneSearch.WaitForScene(AppScene.Lesson);

            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(AppScene.Lesson.ToString()));
            Assert.That(SceneSearch.FindInActiveScene<LessonHud>(), Is.Not.Null);
            Assert.That(SceneSearch.FindInActiveScene<LessonRunner>(), Is.Not.Null);
            Assert.That(Screen.sleepTimeout, Is.EqualTo(SleepTimeout.NeverSleep));
        }

        /// <summary>Tests that the <see cref="LessonHud"/>'s home button returns to the Map, which lets the screen sleep again.</summary>
        [UnityTest]
        public IEnumerator TestLessonHud_HomeReturnsToTheMap()
        {
            SceneManager.LoadScene(AppScene.Lesson.ToString());
            yield return SceneSearch.WaitForScene(AppScene.Lesson);

            SceneSearch.FindInActiveScene<LessonHud>().GetComponentInChildren<Button>().onClick.Invoke();
            yield return SceneSearch.WaitForScene(AppScene.Map);

            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(AppScene.Map.ToString()));
            Assert.That(Screen.sleepTimeout, Is.EqualTo(SleepTimeout.SystemSetting));
        }
    }
}
