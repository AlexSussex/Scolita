using System.Collections;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Lessons;
using Scolita.Core.Tracing;
using Scolita.Game.Lessons;
using Scolita.Game.Settings;
using Scolita.Game.Tracing;
using Scolita.UI.Tracing;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Scolita.Tests.PlayMode.Lessons
{
    /// <summary>
    /// Tests <see cref="TraceLetterView"/> and its <see cref="TraceFinger"/> in the real Lesson scene, on
    /// <c>ro-a</c>'s print a: strokes drawn through the finger trace the letter's three rounds to the step's
    /// end; a second finger and a palm are ignored; a stroke drawn backwards gets its hint; and drawing
    /// allocates nothing. Each test keeps the Editor's own <c>child-1</c> files as they were.
    /// </summary>
    public sealed class TracingTests
    {
        /// <summary>
        /// Tests that strokes drawn through <see cref="TraceFinger"/> trace the print a in its three rounds (two
        /// strokes each), and that the step is then done.
        /// </summary>
        [UnityTest]
        public IEnumerator TestTraceLetterView_ThreeRoundsToTheEnd()
        {
            string keptRun = LessonScene.KeepFile(LessonRunData.FileName);
            string keptSettings = LessonScene.KeepFile(LearningSettingsData.FileName);
            try
            {
                yield return LessonScene.OpenAtStep("ro-a", ActivityKind.TraceLetter);
                LessonRunner runner = SceneSearch.FindInActiveScene<LessonRunner>();
                TraceLetterView view = (TraceLetterView)runner.ShownView;
                TraceState trace = runner.Run.Current.Round.Trace;

                Assert.That(trace.Shape.Id.ToString(), Is.EqualTo("a-lower-print"));
                Assert.That(trace.Shape.Strokes.Count, Is.EqualTo(2));
                for (int stroke = 0; stroke < TracingTuning.Rounds * trace.Shape.Strokes.Count; stroke++)
                {
                    Draw(view, 1, trace.Shape.Strokes[trace.Stroke].Path, false);
                    yield return null;
                }

                Assert.That(trace.RoundsDone, Is.EqualTo(TracingTuning.Rounds));
                Assert.That(runner.Run.State, Is.EqualTo(LessonRunState.StepDone));
            }
            finally
            {
                LessonScene.RestoreFile(LessonRunData.FileName, keptRun);
                LessonScene.RestoreFile(LearningSettingsData.FileName, keptSettings);
            }
        }

        /// <summary>
        /// Tests that only the first finger on the board draws: a second finger moving elsewhere meanwhile, and a
        /// palm put down first, change nothing, and a stroke drawn backwards gets its hint.
        /// </summary>
        [UnityTest]
        public IEnumerator TestTraceFinger_OneFingerNoPalm()
        {
            string keptRun = LessonScene.KeepFile(LessonRunData.FileName);
            string keptSettings = LessonScene.KeepFile(LearningSettingsData.FileName);
            try
            {
                yield return LessonScene.OpenAtStep("ro-a", ActivityKind.TraceLetter);
                LessonRunner runner = SceneSearch.FindInActiveScene<LessonRunner>();
                TraceLetterView view = (TraceLetterView)runner.ShownView;
                TraceState trace = runner.Run.Current.Round.Trace;
                StrokePath round = trace.Shape.Strokes[0].Path;
                int hints = 0;
                runner.Run.HintGiven += hint => hints++;

                view.Finger.Press(9, view.ScreenPointOf(round.PointAt(round.Length * 0.5f)), 2000f);
                Draw(view, 1, round, true);
                view.Finger.Press(2, view.ScreenPointOf(round.End), 0f);
                view.Finger.Move(2, view.ScreenPointOf(round.PointAt(round.Length * 0.3f)));
                view.Finger.Release(2, false);
                view.Finger.Release(1, false);
                view.Finger.Release(9, false);
                yield return null;

                Assert.That(trace.Stroke, Is.EqualTo(1), "the first finger's stroke counted; the palm and the second finger didn't");
                Assert.That(hints, Is.EqualTo(0));

                StrokePath stem = trace.Shape.Strokes[1].Path;
                TracePoints backwards = LessonScene.Along(stem);
                view.Finger.Press(1, view.ScreenPointOf(stem.End), 0f);
                view.Finger.Move(1, view.ScreenPointOf(stem.PointAt(stem.Length * 0.5f)));
                view.Finger.Move(1, view.ScreenPointOf(stem.Start));
                view.Finger.Release(1, false);
                yield return null;

                Assert.That(backwards.Count, Is.GreaterThan(1));
                Assert.That(trace.LastVerdict, Is.EqualTo(StrokeVerdict.WrongDirection));
                Assert.That(hints, Is.EqualTo(1));
                Assert.That(runner.Run.State, Is.EqualTo(LessonRunState.Playing));
            }
            finally
            {
                LessonScene.RestoreFile(LessonRunData.FileName, keptRun);
                LessonScene.RestoreFile(LearningSettingsData.FileName, keptSettings);
            }
        }

        /// <summary>
        /// Tests that drawing on the board (the finger moving, the trail and its stardust growing, the lines'
        /// meshes rebuilt) allocates nothing.
        /// </summary>
        [UnityTest]
        public IEnumerator TestTraceFinger_DrawingAllocatesNothing()
        {
            string keptRun = LessonScene.KeepFile(LessonRunData.FileName);
            string keptSettings = LessonScene.KeepFile(LearningSettingsData.FileName);
            try
            {
                yield return LessonScene.OpenAtStep("ro-a", ActivityKind.TraceLetter);
                LessonRunner runner = SceneSearch.FindInActiveScene<LessonRunner>();
                TraceLetterView view = (TraceLetterView)runner.ShownView;
                StrokePath round = runner.Run.Current.Round.Trace.Shape.Strokes[0].Path;
                StrokeLineGraphic[] lines = view.GetComponentsInChildren<StrokeLineGraphic>();
                view.Finger.Press(1, view.ScreenPointOf(round.Start), 0f);
                float along = 0f;
                for (int frame = 0; frame < 5; frame++)
                {
                    along += 4f;
                    view.Finger.Move(1, view.ScreenPointOf(round.PointAt(along)));
                    Canvas.ForceUpdateCanvases();
                    yield return null;
                }

                for (int frame = 0; frame < 20; frame++)
                {
                    along += 4f;
                    Vector2 next = view.ScreenPointOf(round.PointAt(along));
                    TestDelegate move = () =>
                    {
                        view.Finger.Move(1, next);
                        foreach (StrokeLineGraphic line in lines)
                        {
                            line.Rebuild(CanvasUpdate.PreRender);
                        }
                    };

                    Assert.That(move, UnityEngine.TestTools.Constraints.ConstraintExtensions.AllocatingGCMemory(Is.Not));
                    yield return null;
                }

                view.Finger.Release(1, true);
            }
            finally
            {
                LessonScene.RestoreFile(LessonRunData.FileName, keptRun);
                LessonScene.RestoreFile(LearningSettingsData.FileName, keptSettings);
            }
        }

        private static void Draw(TraceLetterView view, int touchId, StrokePath path, bool keepDown)
        {
            view.Finger.Press(touchId, view.ScreenPointOf(path.Start), 0f);
            for (float along = 4f; along < path.Length; along += 4f)
            {
                view.Finger.Move(touchId, view.ScreenPointOf(path.PointAt(along)));
            }

            view.Finger.Move(touchId, view.ScreenPointOf(path.End));
            if (!keepDown)
            {
                view.Finger.Release(touchId, false);
            }
        }
    }
}
