using NUnit.Framework;
using Scolita.Core.Tracing;

namespace Scolita.Tests.EditMode.Tracing
{
    /// <summary>Tests <see cref="TracePoints"/>, the reused buffer of her line.</summary>
    public sealed class TracePointsTests
    {
        /// <summary>Tests that <see cref="TracePoints.Add"/> keeps points and their length, and drops one too close to the last.</summary>
        [Test]
        public void TestAdd_KeepsPointsAndDropsCloseOnes()
        {
            TracePoints points = new TracePoints();

            Assert.That(points.Add(0f, 0f), Is.True);
            Assert.That(points.Add(0.2f, 0f), Is.False);
            Assert.That(points.Add(3f, 4f), Is.True);

            Assert.That(points.Count, Is.EqualTo(2));
            Assert.That(points.Length, Is.EqualTo(5f).Within(0.001f));
            Assert.That(points.X(1), Is.EqualTo(3f));
            Assert.That(points.Y(1), Is.EqualTo(4f));
        }

        /// <summary>Tests that <see cref="TracePoints.Add"/> drops points once it is full, and <see cref="TracePoints.Clear"/> empties it.</summary>
        [Test]
        public void TestAdd_FullThenClear()
        {
            TracePoints points = new TracePoints();
            for (int index = 0; index < TracePoints.Capacity; index++)
            {
                points.Add(index, 0f);
            }

            Assert.That(points.Add(-10f, -10f), Is.False);
            Assert.That(points.Count, Is.EqualTo(TracePoints.Capacity));

            points.Clear();

            Assert.That(points.Count, Is.EqualTo(0));
            Assert.That(points.Length, Is.EqualTo(0f));
        }

        /// <summary>Tests that <see cref="TracePoints.Add"/> allocates nothing.</summary>
        [Test]
        public void TestAdd_AllocatesNothing()
        {
            TracePoints points = new TracePoints();
            points.Add(0f, 0f);

            TestDelegate add = () => points.Add(5f, 5f);

            Assert.That(add, UnityEngine.TestTools.Constraints.ConstraintExtensions.AllocatingGCMemory(Is.Not));
        }
    }
}
