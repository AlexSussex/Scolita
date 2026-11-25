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
    /// Tests <see cref="WritingBoardView"/> and its <see cref="TraceFinger"/> in the real Lesson scene, on
    /// <c>ro-a</c>'s print a: strokes drawn through the finger trace the letter's three rounds to the step's
    /// end; a second finger and a palm are ignored; a stroke drawn backwards gets its hint; and drawing
    /// allocates nothing. By hand: the handwritten a written in one line, mama letter by letter, and
    /// <see cref="HandwritingView"/> drawing exactly the stroke data. Each test keeps the Editor's own <c>child-1</c> files as they were.
    /// </summary>
    public sealed class TracingTests
    {
        /// <summary>
        /// Tests that strokes drawn through <see cref="TraceFinger"/> trace the print a in its three rounds (two
        /// strokes each), and that the step is then done.
        /// </summary>
        [UnityTest]
        public IEnumerator TestShow_ThreeRoundsToTheEnd()
        {
            string keptRun = LessonScene.KeepFile(LessonRunData.FileName);
            string keptSettings = LessonScene.KeepFile(LearningSettingsData.FileName);
            try
            {
                yield return LessonScene.OpenAtStep("ro-a", ActivityKind.TraceLetter);
                LessonRunner runner = SceneSearch.FindInActiveScene<LessonRunner>();
                WritingBoardView view = (WritingBoardView)runner.ShownView;
                TraceState trace = runner.Run.Current.Round.Trace;

                Assert.That(trace.Figure.Form, Is.EqualTo(LetterForm.Print));
                Assert.That(trace.Figure.Strokes.Count, Is.EqualTo(2));
                for (int stroke = 0; stroke < TracingTuning.Rounds * trace.Figure.Strokes.Count; stroke++)
                {
                    Draw(view, 1, trace.Figure.Strokes[trace.Stroke].Path, false);
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
        public IEnumerator TestPress_OneFingerNoPalm()
        {
            string keptRun = LessonScene.KeepFile(LessonRunData.FileName);
            string keptSettings = LessonScene.KeepFile(LearningSettingsData.FileName);
            try
            {
                yield return LessonScene.OpenAtStep("ro-a", ActivityKind.TraceLetter);
                LessonRunner runner = SceneSearch.FindInActiveScene<LessonRunner>();
                WritingBoardView view = (WritingBoardView)runner.ShownView;
                TraceState trace = runner.Run.Current.Round.Trace;
                StrokePath round = trace.Figure.Strokes[0].Path;
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

                StrokePath stem = trace.Figure.Strokes[1].Path;
                view.Finger.Press(1, view.ScreenPointOf(stem.End), 0f);
                view.Finger.Move(1, view.ScreenPointOf(stem.PointAt(stem.Length * 0.5f)));
                view.Finger.Move(1, view.ScreenPointOf(stem.Start));
                view.Finger.Release(1, false);
                yield return null;

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
        public IEnumerator TestMove_DrawingAllocatesNothing()
        {
            string keptRun = LessonScene.KeepFile(LessonRunData.FileName);
            string keptSettings = LessonScene.KeepFile(LearningSettingsData.FileName);
            try
            {
                yield return LessonScene.OpenAtStep("ro-a", ActivityKind.TraceLetter);
                LessonRunner runner = SceneSearch.FindInActiveScene<LessonRunner>();
                WritingBoardView view = (WritingBoardView)runner.ShownView;
                StrokePath round = runner.Run.Current.Round.Trace.Figure.Strokes[0].Path;
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

        /// <summary>
        /// Tests that the handwritten a of <c>ro-a</c>, written in one line through its three strokes each round, is done
        /// in three rounds, on the "tip I" lines.
        /// </summary>
        [UnityTest]
        public IEnumerator TestShow_AHandwrittenLetterInOneLine()
        {
            string keptRun = LessonScene.KeepFile(LessonRunData.FileName);
            string keptSettings = LessonScene.KeepFile(LearningSettingsData.FileName);
            try
            {
                yield return LessonScene.OpenAtStep("ro-a", ActivityKind.TraceLetter, "handwritten");
                LessonRunner runner = SceneSearch.FindInActiveScene<LessonRunner>();
                WritingBoardView view = (WritingBoardView)runner.ShownView;
                TraceState trace = runner.Run.Current.Round.Trace;

                Assert.That(trace.Figure.Form, Is.EqualTo(LetterForm.Handwritten));
                Assert.That(trace.Figure.Strokes.Count, Is.EqualTo(3));
                Assert.That(view.ShowsHandwrittenLines, Is.True);
                for (int round = 0; round < TracingTuning.Rounds; round++)
                {
                    Draw(view, 1, trace.Figure.ChainFrom(0), false);
                    yield return null;
                }

                Assert.That(trace.IsDone, Is.True);
                Assert.That(runner.Run.State, Is.EqualTo(LessonRunState.StepDone));
            }
            finally
            {
                LessonScene.RestoreFile(LessonRunData.FileName, keptRun);
                LessonScene.RestoreFile(LearningSettingsData.FileName, keptSettings);
            }
        }

        /// <summary>
        /// Tests that mama, written letter by letter through the finger (a lift at each join), is done in three rounds,
        /// and that each letter's line is answered by its checkpoint.
        /// </summary>
        [UnityTest]
        public IEnumerator TestShow_MamaLetterByLetter()
        {
            string keptRun = LessonScene.KeepFile(LessonRunData.FileName);
            string keptSettings = LessonScene.KeepFile(LearningSettingsData.FileName);
            try
            {
                yield return LessonScene.OpenAtStep("ro-words-1", ActivityKind.TraceWord);
                LessonRunner runner = SceneSearch.FindInActiveScene<LessonRunner>();
                WritingBoardView view = (WritingBoardView)runner.ShownView;
                TraceState trace = runner.Run.Current.Round.Trace;
                Assert.That(trace.Figure.LetterCount, Is.EqualTo(4));

                for (int stroke = 0; stroke < TracingTuning.Rounds * trace.Figure.Strokes.Count; stroke++)
                {
                    Draw(view, 1, trace.Figure.Strokes[trace.Stroke].Path, false);
                    yield return null;
                }

                Assert.That(trace.IsDone, Is.True);
                Assert.That(runner.Run.State, Is.EqualTo(LessonRunState.StepDone));
            }
            finally
            {
                LessonScene.RestoreFile(LessonRunData.FileName, keptRun);
                LessonScene.RestoreFile(LearningSettingsData.FileName, keptSettings);
            }
        }

        /// <summary>
        /// Tests that <see cref="HandwritingView"/> draws the handwritten a exactly from its stroke data: every point of its
        /// pen line is a sample of the strokes' paths, in order.
        /// </summary>
        [UnityTest]
        public IEnumerator TestShow_HandwritingMatchesTheStrokes()
        {
            string keptRun = LessonScene.KeepFile(LessonRunData.FileName);
            string keptSettings = LessonScene.KeepFile(LearningSettingsData.FileName);
            try
            {
                yield return LessonScene.OpenAtStep("ro-a", ActivityKind.TraceLetter, "handwritten");
                LessonRunner runner = SceneSearch.FindInActiveScene<LessonRunner>();
                TraceFigure figure = runner.Run.Current.Round.Trace.Figure;
                GameObject host = new GameObject("Handwriting", typeof(RectTransform), typeof(CanvasRenderer), typeof(StrokeLineGraphic));
                host.transform.SetParent(runner.ShownView.transform, false);
                ((RectTransform)host.transform).sizeDelta = new Vector2(400f, 400f);
                HandwritingView handwriting = host.AddComponent<HandwritingView>();

                handwriting.Show(figure);

                int index = 0;
                foreach (Stroke stroke in figure.Strokes)
                {
                    for (int sample = 0; sample < stroke.Path.Count; sample++)
                    {
                        Vector2 expected = handwriting.Local(new ShapePoint(stroke.Path.X(sample), stroke.Path.Y(sample)));
                        Assert.That(Vector2.Distance(handwriting.Pen.PointAt(index), expected), Is.LessThan(0.001f), "point " + index);
                        index++;
                    }
                }

                Assert.That(handwriting.Pen.Count, Is.EqualTo(index));
                Object.Destroy(host);
            }
            finally
            {
                LessonScene.RestoreFile(LessonRunData.FileName, keptRun);
                LessonScene.RestoreFile(LearningSettingsData.FileName, keptSettings);
            }
        }

        private static void Draw(WritingBoardView view, int touchId, StrokePath path, bool keepDown)
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
