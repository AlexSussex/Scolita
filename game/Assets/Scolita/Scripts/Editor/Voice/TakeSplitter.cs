using System.Collections.Generic;

namespace Scolita.Editor.Voice
{
    /// <summary>
    /// Splits a session's take at its pauses: the frames louder than the take's threshold
    /// (<see cref="SilenceFinder.ThresholdOf"/>) are sound; sound with a pause of
    /// <see cref="MinimumPauseSeconds"/> or more around it is one piece (a line); a piece shorter than
    /// <see cref="VoiceClipStandard.MinimumSoundSeconds"/> is a click, and dropped.
    /// </summary>
    public static class TakeSplitter
    {
        /// <summary>The shortest pause between two lines (the script asks for about two seconds); a shorter one is inside a line.</summary>
        public const double MinimumPauseSeconds = 0.9;

        /// <summary>The pieces of <paramref name="take"/>, in order; called by the session import and <c>TakeSplitterTests</c>.</summary>
        public static IReadOnlyList<TakeSegment> Split(MonoAudio take, out double threshold)
        {
            double[] levels = SilenceFinder.FrameLevels(take);
            threshold = SilenceFinder.ThresholdOf(levels);
            int frame = SilenceFinder.FrameLength(take.SampleRate);
            int pauseFrames = (int)(MinimumPauseSeconds / SilenceFinder.FrameSeconds);
            int shortestFrames = (int)(VoiceClipStandard.MinimumSoundSeconds / SilenceFinder.FrameSeconds);
            List<TakeSegment> segments = new List<TakeSegment>();
            int start = -1;
            int lastSound = -1;
            for (int index = 0; index < levels.Length; index++)
            {
                if (levels[index] <= threshold)
                {
                    continue;
                }

                if (start >= 0 && index - lastSound - 1 >= pauseFrames)
                {
                    Add(segments, start, lastSound, frame, take.Samples.Length, shortestFrames);
                    start = -1;
                }

                if (start < 0)
                {
                    start = index;
                }

                lastSound = index;
            }

            if (start >= 0)
            {
                Add(segments, start, lastSound, frame, take.Samples.Length, shortestFrames);
            }

            return segments;
        }

        private static void Add(List<TakeSegment> segments, int firstFrame, int lastFrame, int frame, int samples,
            int shortestFrames)
        {
            if (lastFrame - firstFrame + 1 < shortestFrames)
            {
                return;
            }

            int end = System.Math.Min(samples, (lastFrame + 1) * frame);
            segments.Add(new TakeSegment(firstFrame * frame, end));
        }
    }
}
