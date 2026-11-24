using NUnit.Framework;
using Scolita.Core.Tracing;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Tracing
{
    /// <summary>Tests <see cref="StrokePath"/>: its samples, its length, its points along it, a dot, and the slant.</summary>
    public sealed class StrokePathTests
    {
        /// <summary>Tests that <see cref="StrokePath.Of"/> samples a straight line evenly, from its start to its end.</summary>
        [Test]
        public void TestOf_SamplesAStraightLineEvenly()
        {
            StrokePath path = TestStrokes.Path(0f, 100f, 0f, 200f);

            Assert.That(path.Length, Is.EqualTo(100f).Within(0.01f));
            Assert.That(path.Count, Is.EqualTo(51));
            Assert.That(path.Start.Y, Is.EqualTo(100f).Within(0.01f));
            Assert.That(path.End.Y, Is.EqualTo(200f).Within(0.01f));
            Assert.That(path.DistanceAt(10), Is.EqualTo(20f).Within(0.01f));
            Assert.That(path.X(25), Is.EqualTo(0f).Within(0.01f));
            Assert.That(path.IsDot, Is.False);
        }

        /// <summary>Tests that <see cref="StrokePath.Of"/> makes a smooth curve through every point of a run.</summary>
        [Test]
        public void TestOf_PassesThroughThePoints()
        {
            StrokePath path = TestStrokes.Path(0f, 100f, 50f, 150f, 0f, 200f);
            float nearest = float.MaxValue;
            for (int index = 0; index < path.Count; index++)
            {
                float dx = path.X(index) - 50f;
                float dy = path.Y(index) - 150f;
                nearest = System.Math.Min(nearest, dx * dx + dy * dy);
            }

            Assert.That(System.Math.Sqrt(nearest), Is.LessThan(1.5));
            Assert.That(path.Length, Is.GreaterThan(140f));
        }

        /// <summary>Tests that <see cref="StrokePath.PointAt"/> finds the point at a distance, clamped to the ends.</summary>
        [Test]
        public void TestPointAt_AlongAndClamped()
        {
            StrokePath path = TestStrokes.Path(0f, 100f, 0f, 200f);

            Assert.That(path.PointAt(25f).Y, Is.EqualTo(125f).Within(0.01f));
            Assert.That(path.PointAt(-5f).Y, Is.EqualTo(100f).Within(0.01f));
            Assert.That(path.PointAt(500f).Y, Is.EqualTo(200f).Within(0.01f));
        }

        /// <summary>Tests that a stroke of one point is a dot, of length 0.</summary>
        [Test]
        public void TestOf_OnePointIsADot()
        {
            StrokePath dot = TestStrokes.Path(10f, 60f);

            Assert.That(dot.IsDot, Is.True);
            Assert.That(dot.Count, Is.EqualTo(1));
            Assert.That(dot.Length, Is.EqualTo(0f));
            Assert.That(dot.PointAt(3f).X, Is.EqualTo(10f));
        }

        /// <summary>Tests that <see cref="StrokePath.Of"/> leans the points by the slant, measured from the baseline.</summary>
        [Test]
        public void TestOf_Slant()
        {
            StrokeRun run = new StrokeRun(StrokeRunKind.Body, new[] { new ShapePoint(0f, 100f), new ShapePoint(0f, 200f) });

            StrokePath path = StrokePath.Of(new[] { run }, 0.2f);

            Assert.That(path.Start.X, Is.EqualTo(20f).Within(0.01f));
            Assert.That(path.End.X, Is.EqualTo(0f).Within(0.01f));
        }
    }
}
