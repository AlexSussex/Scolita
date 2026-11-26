using NUnit.Framework;
using Scolita.Core.Tracing;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Tracing
{
    /// <summary>
    /// Tests <see cref="StrokeCheck"/> on strokes made in code: each verdict, a stroke that goes back over
    /// itself, a round stroke drawn the wrong way round, a dot, the rounds' tolerances, and that a check
    /// allocates nothing. The recorded strokes of planet 1's letters are <c>RecordedStrokeTests</c>.
    /// </summary>
    public sealed class StrokeCheckTests
    {
        /// <summary>Tests that <see cref="StrokeCheck.Check"/> passes a line drawn from the start dot to the end.</summary>
        [Test]
        public void TestCheck_GoodLine()
        {
            StrokePath path = TestStrokes.Path(0f, 100f, 0f, 200f);

            StrokeResult result = StrokeCheck.Check(path, TestStrokes.Along(path, 0f, 1f, 4f, 3f), Tight());

            Assert.That(result.Verdict, Is.EqualTo(StrokeVerdict.Good));
            Assert.That(result.Covered, Is.GreaterThan(0.95f));
        }

        /// <summary>Tests that <see cref="StrokeCheck.Check"/> calls a line drawn from its end <see cref="StrokeVerdict.WrongDirection"/>.</summary>
        [Test]
        public void TestCheck_BackwardsIsWrongDirection()
        {
            StrokePath path = TestStrokes.Path(0f, 100f, 0f, 200f);

            StrokeResult result = StrokeCheck.Check(path, TestStrokes.Along(path, 1f, 0f, 4f, 0f), Wide());

            Assert.That(result.Verdict, Is.EqualTo(StrokeVerdict.WrongDirection));
        }

        /// <summary>Tests that <see cref="StrokeCheck.Check"/> calls a tap on the start dot, and no point at all, <see cref="StrokeVerdict.TooShort"/>.</summary>
        [Test]
        public void TestCheck_TapIsTooShort()
        {
            StrokePath path = TestStrokes.Path(0f, 100f, 0f, 200f);

            Assert.That(StrokeCheck.Check(path, TestStrokes.Drawn(0f, 100f, 1f, 101f), Wide()).Verdict, Is.EqualTo(StrokeVerdict.TooShort));
            Assert.That(StrokeCheck.Check(path, new TracePoints(), Wide()).Verdict, Is.EqualTo(StrokeVerdict.TooShort));
        }

        /// <summary>Tests that <see cref="StrokeCheck.Check"/> calls a line begun right that stops half-way <see cref="StrokeVerdict.TooShort"/>.</summary>
        [Test]
        public void TestCheck_HalfWayIsTooShort()
        {
            StrokePath path = TestStrokes.Path(0f, 100f, 0f, 200f);

            StrokeResult result = StrokeCheck.Check(path, TestStrokes.Along(path, 0f, 0.5f, 4f, 0f), Wide());

            Assert.That(result.Verdict, Is.EqualTo(StrokeVerdict.TooShort));
            Assert.That(result.Covered, Is.EqualTo(0.5f).Within(0.05f));
        }

        /// <summary>Tests that <see cref="StrokeCheck.Check"/> calls a line begun in the middle of the stroke <see cref="StrokeVerdict.StartedElsewhere"/>.</summary>
        [Test]
        public void TestCheck_BegunInTheMiddleIsStartedElsewhere()
        {
            StrokePath path = TestStrokes.Path(0f, 0f, 0f, 200f);

            StrokeResult result = StrokeCheck.Check(path, TestStrokes.Along(path, 0.5f, 1f, 4f, 0f), Wide());

            Assert.That(result.Verdict, Is.EqualTo(StrokeVerdict.StartedElsewhere));
        }

        /// <summary>Tests that <see cref="StrokeCheck.Check"/> calls a line that wanders off the stroke and back <see cref="StrokeVerdict.LeftThePath"/>.</summary>
        [Test]
        public void TestCheck_WanderingOffIsLeftThePath()
        {
            StrokePath path = TestStrokes.Path(0f, 0f, 0f, 200f);
            TracePoints points = TestStrokes.Along(path, 0f, 0.4f, 4f, 0f);
            points.Add(40f, 90f);
            points.Add(70f, 100f);
            points.Add(40f, 110f);
            TestStrokes.AddAlong(points, path, 0.6f, 1f, 4f, 0f);

            StrokeResult result = StrokeCheck.Check(path, points, Wide());

            Assert.That(result.Verdict, Is.EqualTo(StrokeVerdict.LeftThePath));
        }

        /// <summary>
        /// Tests that <see cref="StrokeCheck.Check"/> follows a stroke that goes down and back up the same line
        /// (m's first stem) in order: only the way down is <see cref="StrokeVerdict.TooShort"/>, both ways are
        /// <see cref="StrokeVerdict.Good"/>.
        /// </summary>
        [Test]
        public void TestCheck_DownAndBackUpInOrder()
        {
            StrokePath path = TestStrokes.Runs(new[] { 0f, 100f, 0f, 200f }, new[] { 0f, 200f, 0f, 100f });

            Assert.That(StrokeCheck.Check(path, TestStrokes.Along(path, 0f, 0.5f, 4f, 0f), Wide()).Verdict,
                Is.EqualTo(StrokeVerdict.TooShort));
            Assert.That(StrokeCheck.Check(path, TestStrokes.Along(path, 0f, 1f, 4f, 0f), Wide()).Verdict,
                Is.EqualTo(StrokeVerdict.Good));
        }

        /// <summary>Tests that <see cref="StrokeCheck.Check"/> tells a round stroke drawn the wrong way round from one drawn the right way.</summary>
        [Test]
        public void TestCheck_RoundTheWrongWay()
        {
            StrokePath path = TestStrokes.Path(50f, 100f, 0f, 150f, 50f, 200f, 100f, 150f, 52f, 102f);

            Assert.That(StrokeCheck.Check(path, TestStrokes.Along(path, 0f, 1f, 4f, 2f), Tight()).Verdict,
                Is.EqualTo(StrokeVerdict.Good));
            Assert.That(StrokeCheck.Check(path, TestStrokes.Along(path, 1f, 0f, 4f, 2f), Tight()).Verdict,
                Is.EqualTo(StrokeVerdict.WrongDirection));
        }

        /// <summary>Tests that <see cref="StrokeCheck.Check"/> passes a dot touched near it, and not one touched far from it.</summary>
        [Test]
        public void TestCheck_Dot()
        {
            StrokePath dot = TestStrokes.Path(10f, 60f);

            Assert.That(StrokeCheck.Check(dot, TestStrokes.Drawn(15f, 64f), Tight()).Verdict, Is.EqualTo(StrokeVerdict.Good));
            Assert.That(StrokeCheck.Check(dot, TestStrokes.Drawn(80f, 64f), Tight()).Verdict,
                Is.EqualTo(StrokeVerdict.StartedElsewhere));
        }

        /// <summary>
        /// Tests that the rounds' tolerances tighten: a line drawn a little beside the stroke is good in round 1
        /// (the corridor 36) and has left the path in round 3 (the corridor 30).
        /// </summary>
        [Test]
        public void TestCheck_TheRoundsTighten()
        {
            StrokePath path = TestStrokes.Path(0f, 0f, 0f, 200f);
            TracePoints sloppy = TestStrokes.Along(path, 0f, 1f, 4f, 33f);
            TracingTuning tuning = TestLearningRules.Tracing();

            Assert.That(StrokeCheck.Check(path, sloppy, tuning.ForRound(0)).Verdict, Is.EqualTo(StrokeVerdict.Good));
            Assert.That(StrokeCheck.Check(path, sloppy, tuning.ForRound(2)).Verdict, Is.Not.EqualTo(StrokeVerdict.Good));
        }

        /// <summary>
        /// Tests that <see cref="StrokeCheck.Check"/> counts only what she drew on a short bar: begun half-way along
        /// (still within the start radius) it is <see cref="StrokeVerdict.TooShort"/>; drawn whole but lifted a little
        /// before its end it is <see cref="StrokeVerdict.Good"/>, even in round 3; drawn from its end it is
        /// <see cref="StrokeVerdict.WrongDirection"/>.
        /// </summary>
        [Test]
        public void TestCheck_AShortBar()
        {
            StrokePath bar = TestStrokes.Path(0f, 100f, 60f, 100f);
            StrokePath tick = TestStrokes.Path(0f, 100f, 20f, 100f);

            Assert.That(StrokeCheck.Check(bar, TestStrokes.Along(bar, 0.5f, 1f, 4f, 0f), Tight()).Verdict,
                Is.EqualTo(StrokeVerdict.TooShort));
            Assert.That(StrokeCheck.Check(bar, TestStrokes.Along(bar, 0f, 0.8f, 4f, 0f), Tight()).Verdict,
                Is.EqualTo(StrokeVerdict.Good));
            Assert.That(StrokeCheck.Check(bar, TestStrokes.Along(bar, 1f, 0f, 4f, 0f), Tight()).Verdict,
                Is.EqualTo(StrokeVerdict.WrongDirection));
            Assert.That(StrokeCheck.Check(tick, TestStrokes.Along(tick, 1f, 0f, 2f, 0f), Wide()).Verdict,
                Is.EqualTo(StrokeVerdict.WrongDirection));
        }

        /// <summary>Tests that <see cref="StrokeCheck.Check"/> allocates nothing, so judging a stroke never makes garbage.</summary>
        [Test]
        public void TestCheck_AllocatesNothing()
        {
            StrokePath path = TestStrokes.Runs(new[] { 0f, 100f, 0f, 200f }, new[] { 0f, 200f, 0f, 120f, 30f, 100f, 50f, 200f });
            TracePoints points = TestStrokes.Along(path, 0f, 1f, 4f, 5f);
            StrokeTolerance tolerance = Wide();
            StrokeCheck.Check(path, points, tolerance);

            TestDelegate check = () => StrokeCheck.Check(path, points, tolerance);

            Assert.That(check, UnityEngine.TestTools.Constraints.ConstraintExtensions.AllocatingGCMemory(Is.Not));
        }

        private static StrokeTolerance Wide()
        {
            return TestLearningRules.Tracing().ForRound(0);
        }

        private static StrokeTolerance Tight()
        {
            return TestLearningRules.Tracing().ForRound(2);
        }
    }
}
