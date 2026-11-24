using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;
using Scolita.Game.Lessons;
using Scolita.Game.Profiles;
using Scolita.Game.Saving;
using Scolita.Game.Scenes;
using Scolita.Game.Settings;
using Scolita.Game.Voice;
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
    /// simulated taps: it runs to its end through a hint and an answer shown, and home mid-lesson then
    /// Go resumes at the same step. Each test keeps the Editor's own <c>child-1</c> lesson and learning settings files as they were.
    /// </summary>
    public sealed class LessonFlowTests
    {
        /// <summary>
        /// Tests that the lesson runs to its end with taps: <c>MeetLetter</c> (letter, then picture), then
        /// <c>ListenAndPick</c> with a miss (it fades), a second miss (the answer glows) and the answer;
        /// the end shows "Mai departe", which opens the map, and Go will open the next lesson.
        /// </summary>
        [UnityTest]
        public IEnumerator TestLessonRunner_RunsToItsEnd()
        {
            string keptRun = KeepFile(LessonRunData.FileName);
            string keptSettings = KeepFile(LearningSettingsData.FileName);
            try
            {
                SaveStartingFiles("ro-t");
                yield return OpenLesson();
                LessonRunner runner = SceneSearch.FindInActiveScene<LessonRunner>();
                LessonScreen screen = SceneSearch.FindInActiveScene<LessonScreen>();

                Assert.That(runner.Run.Plan.Lesson.Id.ToString(), Is.EqualTo("ro-t"));
                Assert.That(runner.Run.Current.Kind, Is.EqualTo(ActivityKind.MeetLetter));
                Assert.That(screen.Bubble.transform.GetChild(0).gameObject.activeSelf, Is.True, "the instruction's bubble");
                Assert.That(Tiles(runner).All(tile => tile.GetComponentInChildren<LanguageBadge>().Shown == Language.Romanian),
                    Is.True);
                Tap(Tile(runner, "t"));
                Tap(Tile(runner, "ro-toba"));
                Assert.That(runner.Run.State, Is.EqualTo(LessonRunState.StepDone));
                yield return WaitForStep(runner, ActivityKind.ListenAndPick);

                Tap(Tile(runner, "a"));
                Assert.That(Tile(runner, "a").IsFaded, Is.True);
                Assert.That(runner.Run.State, Is.EqualTo(LessonRunState.Playing));
                Tap(Tile(runner, "m"));
                Assert.That(runner.Run.State, Is.EqualTo(LessonRunState.AnswerShown));
                Assert.That(Tile(runner, "t").IsGlowing, Is.True);
                Tap(Tile(runner, "t"));
                yield return WaitUntil(() => runner.Run.State == LessonRunState.Finished);

                Assert.That(screen.IsEndShown, Is.True);
                Assert.That(screen.Dots.ColourOf(1), Is.EqualTo(screen.Dots.ColourOf(0)));
                Assert.That(SavedLesson(), Is.EqualTo("en-t"));
                screen.ContinueButton.onClick.Invoke();
                yield return SceneSearch.WaitForScene(AppScene.Map);
                Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(AppScene.Map.ToString()));
            }
            finally
            {
                RestoreFile(LessonRunData.FileName, keptRun);
                RestoreFile(LearningSettingsData.FileName, keptSettings);
            }
        }

        /// <summary>
        /// Tests that home in the middle of a lesson, then Go, opens the same lesson at the same step,
        /// with the steps before it shown done.
        /// </summary>
        [UnityTest]
        public IEnumerator TestLessonRunner_HomeThenGoResumes()
        {
            string keptRun = KeepFile(LessonRunData.FileName);
            string keptSettings = KeepFile(LearningSettingsData.FileName);
            try
            {
                SaveStartingFiles("ro-t");
                yield return OpenLesson();
                LessonRunner runner = SceneSearch.FindInActiveScene<LessonRunner>();
                Tap(Tile(runner, "t"));
                Tap(Tile(runner, "ro-toba"));
                yield return WaitForStep(runner, ActivityKind.ListenAndPick);

                SceneSearch.FindInActiveScene<LessonHud>().GetComponentInChildren<Button>().onClick.Invoke();
                yield return SceneSearch.WaitForScene(AppScene.Map);
                SceneSearch.FindInActiveScene<MapScreen>().GetComponentInChildren<Button>().onClick.Invoke();
                yield return SceneSearch.WaitForScene(AppScene.Lesson);
                yield return WaitUntil(() => SceneSearch.FindInActiveScene<LessonRunner>().Run != null);
                LessonRunner resumed = SceneSearch.FindInActiveScene<LessonRunner>();
                LessonScreen screen = SceneSearch.FindInActiveScene<LessonScreen>();

                Assert.That(resumed.Run.Plan.Lesson.Id.ToString(), Is.EqualTo("ro-t"));
                Assert.That(resumed.Run.Current.Kind, Is.EqualTo(ActivityKind.ListenAndPick));
                Assert.That(screen.Dots.Count, Is.EqualTo(2));
                Assert.That(screen.Dots.ColourOf(0), Is.Not.EqualTo(screen.Dots.ColourOf(1)));
            }
            finally
            {
                RestoreFile(LessonRunData.FileName, keptRun);
                RestoreFile(LearningSettingsData.FileName, keptSettings);
            }
        }

        private static IEnumerator OpenLesson()
        {
            SceneManager.LoadScene(AppScene.Lesson.ToString());
            yield return SceneSearch.WaitForScene(AppScene.Lesson);
            yield return WaitUntil(() => SceneSearch.FindInActiveScene<LessonRunner>().Run != null);
        }

        private static IEnumerator WaitForStep(LessonRunner runner, ActivityKind kind)
        {
            yield return WaitUntil(() => runner.Run.State == LessonRunState.Playing && runner.Run.Current.Kind == kind);
        }

        private static IEnumerator WaitUntil(System.Func<bool> condition)
        {
            const float maxSeconds = 10f;
            float until = Time.realtimeSinceStartup + maxSeconds;
            while (!condition() && Time.realtimeSinceStartup < until)
            {
                LessonRunner runner = SceneSearch.FindInActiveScene<LessonRunner>();
                if (runner != null && runner.Run != null)
                {
                    SceneSearch.FindInActiveScene<VoiceHost>().Voice.Stop();
                }

                yield return null;
            }

            Assert.That(condition(), Is.True, "waited " + maxSeconds + " seconds");
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

        private static void Tap(ChoiceTile tile)
        {
            tile.OnPointerClick(new PointerEventData(EventSystem.current));
        }

        private static SaveStore Store()
        {
            return SaveStore.ForProfile(Application.persistentDataPath, BuiltInProfiles.FirstChild);
        }

        private static string FilePath(string name)
        {
            return Path.Combine(Store().Folder, name + ".json");
        }

        private static string KeepFile(string name)
        {
            return File.Exists(FilePath(name)) ? File.ReadAllText(FilePath(name)) : null;
        }

        private static void RestoreFile(string name, string kept)
        {
            if (kept == null)
            {
                File.Delete(FilePath(name));
                return;
            }

            File.WriteAllText(FilePath(name), kept);
        }

        private static void SaveStartingFiles(string lessonId)
        {
            Assert.That(Store().Save(LearningSettingsData.FileName, LearningSettingsData.CreateDefaults()), Is.True);
            Assert.That(Store().Save(LessonRunData.FileName, new LessonRunData(ContentId.Create(lessonId), 1, string.Empty)), Is.True);
        }

        private static string SavedLesson()
        {
            LessonRunData data = Store().Load(LessonRunData.FileName, LessonRunData.CreateDefaults()).Data;
            return data.TryGetLessonId(out ContentId id) ? id.ToString() : string.Empty;
        }
    }
}
