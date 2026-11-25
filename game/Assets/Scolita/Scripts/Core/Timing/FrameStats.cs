using System;

namespace Scolita.Core.Timing
{
    /// <summary>
    /// The performance pass's numbers (AGENTS.md → "Performance": 60 frames a second, 0 B of GC allocation per frame while
    /// an activity runs), gathered frame by frame over a window and shown on the tester's strip, so they can be read on
    /// the tablet with no tools: the frames per second over the last window, its slowest frame, and the most memory
    /// allocated in one of its frames. No allocation.
    /// </summary>
    public sealed class FrameStats
    {
        private readonly float windowSeconds;
        private float seconds;
        private int frames;
        private float slowest;
        private long mostAllocated;

        /// <summary>
        /// Gathers windows of <paramref name="windowSeconds"/> (above 0; else an <see cref="ArgumentOutOfRangeException"/>).
        /// Called by the tester's strip and <c>FrameStatsTests</c>.
        /// </summary>
        public FrameStats(float windowSeconds)
        {
            if (float.IsNaN(windowSeconds) || windowSeconds <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(windowSeconds), windowSeconds, "A window lasts more than 0 seconds.");
            }

            this.windowSeconds = windowSeconds;
        }

        /// <summary>The frames per second over the last full window; 0 before the first.</summary>
        public float FramesPerSecond { get; private set; }

        /// <summary>The slowest frame of the last full window, in milliseconds.</summary>
        public float SlowestFrameMilliseconds { get; private set; }

        /// <summary>The most memory allocated (GC) in one frame of the last full window, in bytes.</summary>
        public long MostAllocatedInAFrame { get; private set; }

        /// <summary>
        /// Adds a frame that took <paramref name="frameSeconds"/> and allocated <paramref name="allocatedBytes"/> (a negative
        /// value counts as 0). True when it closes a window, so the numbers above are new. Called every frame by the
        /// tester's strip, and <c>FrameStatsTests</c>.
        /// </summary>
        public bool Add(float frameSeconds, long allocatedBytes)
        {
            float frame = MathF.Max(0f, frameSeconds);
            seconds += frame;
            frames++;
            slowest = MathF.Max(slowest, frame);
            mostAllocated = Math.Max(mostAllocated, allocatedBytes);
            if (seconds < windowSeconds)
            {
                return false;
            }

            FramesPerSecond = frames / seconds;
            SlowestFrameMilliseconds = slowest * 1000f;
            MostAllocatedInAFrame = mostAllocated;
            seconds = 0f;
            frames = 0;
            slowest = 0f;
            mostAllocated = 0L;
            return true;
        }
    }
}
