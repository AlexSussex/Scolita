using System;
using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Activities;
using Scolita.Core.Content;
using Scolita.Core.Lessons;
using Scolita.Game.Lessons;
using Scolita.Game.Profiles;
using Scolita.Game.Saving;
using Scolita.Game.Scenes;
using Scolita.Game.Settings;
using Scolita.Game.Voice;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Scolita.Tests.PlayMode.Lessons
{
    /// <summary>
    /// Helpers the lesson PlayMode tests share: keeping and restoring the Editor's own <c>child-1</c>
    /// files, opening the Lesson scene at a lesson (and at one of its steps), waiting for the run, and
    /// tapping. Used by <see cref="LessonFlowTests"/> and <see cref="LetterActivitiesTests"/>.
    /// </summary>
    public static class LessonScene
    {
        /// <summary>The content of the profile file <paramref name="name"/>, or null when there is none; used by the lesson tests.</summary>
        public static string KeepFile(string name)
        {
            return File.Exists(FilePath(name)) ? File.ReadAllText(FilePath(name)) : null;
        }

        /// <summary>Puts the profile file <paramref name="name"/> back as <paramref name="kept"/> was; used by the lesson tests.</summary>
        public static void RestoreFile(string name, string kept)
        {
            if (kept == null)
            {
                File.Delete(FilePath(name));
                return;
            }

            File.WriteAllText(FilePath(name), kept);
        }

        /// <summary>
        /// Saves the default learning settings (both languages) and the lesson <paramref name="lessonId"/>
        /// to open at its first step; used by the lesson tests.
        /// </summary>
        public static void SaveStartingFiles(string lessonId)
        {
            SaveLesson(lessonId, 1, string.Empty);
        }

        /// <summary>Opens the Lesson scene and waits for its run; used by the lesson tests.</summary>
        public static IEnumerator Open()
        {
            yield return OpenScene();
        }

        /// <summary>
        /// Opens the Lesson scene at the first step of <paramref name="kind"/> in the lesson
        /// <paramref name="lessonId"/>, as if she had left it there: it opens the lesson once to learn its
        /// steps' signature, then saves where she is and opens it again. Used by the lesson tests.
        /// </summary>
        public static IEnumerator OpenAtStep(string lessonId, ActivityKind kind)
        {
            SaveLesson(lessonId, 1, string.Empty);
            yield return OpenScene();
            Lesson lesson = SceneSearch.FindInActiveScene<LessonRunner>().Run.Plan.Lesson;
            Step step = lesson.Steps.First(candidate => candidate.Kind == kind);
            SaveLesson(lessonId, step.Number, LessonSignature.Of(lesson));
            yield return OpenScene();
            LessonRunner runner = SceneSearch.FindInActiveScene<LessonRunner>();
            Assert.That(runner.Run.Current.Kind, Is.EqualTo(kind));
        }

        /// <summary>Waits until the run plays a step of <paramref name="kind"/>; used by the lesson tests.</summary>
        public static IEnumerator WaitForStep(LessonRunner runner, ActivityKind kind)
        {
            yield return WaitFor(() => runner.Run.State == LessonRunState.Playing && runner.Run.Current.Kind == kind);
        }

        /// <summary>
        /// Waits at most 10 seconds for <paramref name="condition"/>, stopping the voice each frame so the
        /// lines don't hold the lesson up, and fails the test if it never holds. Used by the lesson tests.
        /// </summary>
        public static IEnumerator WaitUntil(Func<bool> condition)
        {
            yield return WaitFor(condition);
        }

        /// <summary>
        /// Answers every step left right, through the composition root as a view would, until the lesson is
        /// finished; used by the lesson tests to get past the steps they don't look at.
        /// </summary>
        public static IEnumerator FinishLesson(LessonRunner runner)
        {
            for (int step = 0; step < runner.Run.StepCount && runner.Run.State != LessonRunState.Finished; step++)
            {
                yield return WaitFor(() => runner.Run.State == LessonRunState.Playing || runner.Run.State == LessonRunState.Finished);
                if (runner.Run.State == LessonRunState.Playing)
                {
                    AnswerRight(runner);
                }
            }

            yield return WaitFor(() => runner.Run.State == LessonRunState.Finished);
        }

        /// <summary>Taps <paramref name="target"/>; used by the lesson tests.</summary>
        public static void Tap(IPointerClickHandler target)
        {
            target.OnPointerClick(new PointerEventData(EventSystem.current));
        }

        /// <summary>The lesson the profile's lesson file names; used by the lesson tests.</summary>
        public static string SavedLesson()
        {
            LessonRunData data = Store().Load(LessonRunData.FileName, LessonRunData.CreateDefaults()).Data;
            return data.TryGetLessonId(out ContentId id) ? id.ToString() : string.Empty;
        }

        private static void AnswerRight(LessonRunner runner)
        {
            ActivityRound round = runner.Run.Current.Round;
            switch (runner.Run.Current.Kind)
            {
                case ActivityKind.MeetLetter:
                    runner.Answer(ActivityAnswer.Tap(MeetLetterRules.LetterChoice));
                    runner.Answer(ActivityAnswer.Tap(MeetLetterRules.PictureChoice));
                    break;
                case ActivityKind.LetterHunt:
                    for (int star = 0; star < round.Choices.Count; star++)
                    {
                        if (round.Choices[star].Item == round.AnswerItem && round.StateOf(star) != ChoiceState.Done)
                        {
                            runner.Answer(ActivityAnswer.Tap(star));
                        }
                    }

                    break;
                case ActivityKind.TwoSounds:
                    runner.Answer(ActivityAnswer.Tap(TwoSoundsRules.RomanianPlanet));
                    runner.Answer(ActivityAnswer.Tap(TwoSoundsRules.EnglishPlanet));
                    for (int picture = 0; picture < round.Choices.Count && round.Current >= 0; picture++)
                    {
                        runner.Answer(ActivityAnswer.Drop(round.Answer));
                    }

                    break;
                case ActivityKind.ListenAndPick:
                case ActivityKind.FirstSound:
                    runner.Answer(ActivityAnswer.Tap(round.Answer));
                    break;
                default:
                    Assert.Fail("FinishLesson can't answer a " + runner.Run.Current.Kind + " step yet: add it to LessonScene.AnswerRight.");
                    break;
            }
        }

        private static IEnumerator OpenScene()
        {
            SceneManager.LoadScene(AppScene.Lesson.ToString());
            yield return SceneSearch.WaitForScene(AppScene.Lesson);
            yield return WaitFor(() => SceneSearch.FindInActiveScene<LessonRunner>().Run != null);
        }

        private static IEnumerator WaitFor(Func<bool> condition)
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

        private static void SaveLesson(string lessonId, int stepNumber, string signature)
        {
            Assert.That(Store().Save(LearningSettingsData.FileName, LearningSettingsData.CreateDefaults()), Is.True);
            Assert.That(Store().Save(LessonRunData.FileName, new LessonRunData(ContentId.Create(lessonId), stepNumber, signature)),
                Is.True);
        }

        private static SaveStore Store()
        {
            return SaveStore.ForProfile(Application.persistentDataPath, BuiltInProfiles.FirstChild);
        }

        private static string FilePath(string name)
        {
            return Path.Combine(Store().Folder, name + ".json");
        }
    }
}
