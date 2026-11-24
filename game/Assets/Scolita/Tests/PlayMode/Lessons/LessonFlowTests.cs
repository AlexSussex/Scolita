using System.Collections;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;
using Scolita.Game.Lessons;
using Scolita.Game.Scenes;
using Scolita.Game.Settings;
using Scolita.UI.Lesson;
using Scolita.UI.Map;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Scolita.Tests.PlayMode.Lessons
{
    /// <summary>
    /// Tests the Lesson scene's flow (<see cref="LessonRunner"/>) on the real <c>ro-t</c> lesson with
    /// simulated taps: it runs to its end through a hint and an answer shown (the steps after
    /// <c>ListenAndPick</c> answered right), and home mid-lesson then
    /// Go resumes at the same step. Each test keeps the Editor's own <c>child-1</c> lesson and learning settings files as they were.
    /// </summary>
    public sealed class LessonFlowTests
    {
        /// <summary>
        /// Tests that the lesson runs to its end with taps: <c>MeetLetter</c> (letter, then picture), then
        /// <c>ListenAndPick</c> with a miss (it fades), a second miss (the answer glows) and the answer, then
        /// the steps after it answered right; the end shows "Mai departe", which opens the map, and Go will open the next lesson.
        /// </summary>
        [UnityTest]
        public IEnumerator TestLessonRunner_RunsToItsEnd()
        {
            string keptRun = LessonScene.KeepFile(LessonRunData.FileName);
            string keptSettings = LessonScene.KeepFile(LearningSettingsData.FileName);
            try
            {
                LessonScene.SaveStartingFiles("ro-t");
                yield return LessonScene.Open();
                LessonRunner runner = SceneSearch.FindInActiveScene<LessonRunner>();
                LessonScreen screen = SceneSearch.FindInActiveScene<LessonScreen>();

                Assert.That(runner.Run.Plan.Lesson.Id.ToString(), Is.EqualTo("ro-t"));
                Assert.That(runner.Run.Current.Kind, Is.EqualTo(ActivityKind.MeetLetter));
                Assert.That(screen.Bubble.transform.GetChild(0).gameObject.activeSelf, Is.True, "the instruction's bubble");
                Assert.That(Tiles(runner).All(tile => tile.GetComponentInChildren<LanguageBadge>().Shown == Language.Romanian),
                    Is.True);
                LessonScene.Tap(Tile(runner, "t"));
                LessonScene.Tap(Tile(runner, "ro-toba"));
                Assert.That(runner.Run.State, Is.EqualTo(LessonRunState.StepDone));
                yield return LessonScene.WaitForStep(runner, ActivityKind.ListenAndPick);

                LessonScene.Tap(Tile(runner, "a"));
                Assert.That(Tile(runner, "a").IsFaded, Is.True);
                Assert.That(runner.Run.State, Is.EqualTo(LessonRunState.Playing));
                LessonScene.Tap(Tile(runner, "m"));
                Assert.That(runner.Run.State, Is.EqualTo(LessonRunState.AnswerShown));
                Assert.That(Tile(runner, "t").IsGlowing, Is.True);
                LessonScene.Tap(Tile(runner, "t"));
                yield return LessonScene.FinishLesson(runner);

                Assert.That(screen.IsEndShown, Is.True);
                Assert.That(screen.Dots.ColourOf(runner.Run.StepCount - 1), Is.EqualTo(screen.Dots.ColourOf(0)));
                Assert.That(LessonScene.SavedLesson(), Is.EqualTo("en-t"));
                screen.ContinueButton.onClick.Invoke();
                yield return SceneSearch.WaitForScene(AppScene.Map);
                Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(AppScene.Map.ToString()));
            }
            finally
            {
                LessonScene.RestoreFile(LessonRunData.FileName, keptRun);
                LessonScene.RestoreFile(LearningSettingsData.FileName, keptSettings);
            }
        }

        /// <summary>
        /// Tests that home in the middle of a lesson, then Go, opens the same lesson at the same step,
        /// with the steps before it shown done.
        /// </summary>
        [UnityTest]
        public IEnumerator TestLessonRunner_HomeThenGoResumes()
        {
            string keptRun = LessonScene.KeepFile(LessonRunData.FileName);
            string keptSettings = LessonScene.KeepFile(LearningSettingsData.FileName);
            try
            {
                LessonScene.SaveStartingFiles("ro-t");
                yield return LessonScene.Open();
                LessonRunner runner = SceneSearch.FindInActiveScene<LessonRunner>();
                LessonScene.Tap(Tile(runner, "t"));
                LessonScene.Tap(Tile(runner, "ro-toba"));
                yield return LessonScene.WaitForStep(runner, ActivityKind.ListenAndPick);

                SceneSearch.FindInActiveScene<LessonHud>().GetComponentInChildren<Button>().onClick.Invoke();
                yield return SceneSearch.WaitForScene(AppScene.Map);
                SceneSearch.FindInActiveScene<MapScreen>().GetComponentInChildren<Button>().onClick.Invoke();
                yield return SceneSearch.WaitForScene(AppScene.Lesson);
                yield return LessonScene.WaitUntil(() => SceneSearch.FindInActiveScene<LessonRunner>().Run != null);
                LessonRunner resumed = SceneSearch.FindInActiveScene<LessonRunner>();
                LessonScreen screen = SceneSearch.FindInActiveScene<LessonScreen>();

                Assert.That(resumed.Run.Plan.Lesson.Id.ToString(), Is.EqualTo("ro-t"));
                Assert.That(resumed.Run.Current.Kind, Is.EqualTo(ActivityKind.ListenAndPick));
                Assert.That(screen.Dots.Count, Is.EqualTo(resumed.Run.StepCount + 1));
                Assert.That(screen.Dots.ColourOf(0), Is.Not.EqualTo(screen.Dots.ColourOf(1)));
            }
            finally
            {
                LessonScene.RestoreFile(LessonRunData.FileName, keptRun);
                LessonScene.RestoreFile(LearningSettingsData.FileName, keptSettings);
            }
        }

        private static ChoiceTile[] Tiles(LessonRunner runner)
        {
            return runner.ShownView.GetComponentsInChildren<ChoiceTile>(false);
        }

        private static ChoiceTile Tile(LessonRunner runner, string item)
        {
            ChoiceTile tile = Tiles(runner).FirstOrDefault(candidate => candidate.Item.ToString() == item);
            Assert.That(tile, Is.Not.Null, "no card for " + item);
            return tile;
        }
    }
}
