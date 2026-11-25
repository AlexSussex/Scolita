using System;
using NUnit.Framework;
using Scolita.Core.Timing;

namespace Scolita.Tests.EditMode.Core
{
    /// <summary>Tests <see cref="FrameStats"/>.</summary>
    public sealed class FrameStatsTests
    {
        private const float Tolerance = 0.001f;

        /// <summary>Tests that <see cref="FrameStats.Add"/> says nothing new until a window is full, then gives its numbers.</summary>
        [Test]
        public void TestAdd_AWindowGivesItsNumbers()
        {
            FrameStats stats = new FrameStats(1f);
            bool early = false;
            for (int frame = 0; frame < 59; frame++)
            {
                early |= stats.Add(1f / 60f, frame == 10 ? 512L : 0L);
            }

            bool closed = stats.Add(1f / 60f + 0.001f, 0L);

            Assert.That(early, Is.False);
            Assert.That(closed, Is.True);
            Assert.That(stats.FramesPerSecond, Is.EqualTo(60f).Within(0.1f));
            Assert.That(stats.SlowestFrameMilliseconds, Is.EqualTo(1000f / 60f + 1f).Within(Tolerance));
            Assert.That(stats.MostAllocatedInAFrame, Is.EqualTo(512L));
        }

        /// <summary>Tests that <see cref="FrameStats.Add"/> starts each window afresh: a slow frame or an allocation counts only in its own.</summary>
        [Test]
        public void TestAdd_EachWindowStartsAfresh()
        {
            FrameStats stats = new FrameStats(0.5f);
            stats.Add(0.1f, 4096L);
            stats.Add(0.4f, 0L);

            stats.Add(0.25f, 0L);
            bool closed = stats.Add(0.25f, 0L);

            Assert.That(closed, Is.True);
            Assert.That(stats.FramesPerSecond, Is.EqualTo(4f).Within(Tolerance));
            Assert.That(stats.SlowestFrameMilliseconds, Is.EqualTo(250f).Within(Tolerance));
            Assert.That(stats.MostAllocatedInAFrame, Is.EqualTo(0L));
        }

        /// <summary>Tests that the <see cref="FrameStats"/> constructor refuses a window of 0 seconds.</summary>
        [Test]
        public void TestFrameStats_RefusesAnEmptyWindow()
        {
            Assert.That(() => new FrameStats(0f), Throws.TypeOf<ArgumentOutOfRangeException>());
        }
    }
}
