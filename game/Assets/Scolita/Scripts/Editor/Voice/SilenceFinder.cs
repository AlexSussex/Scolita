using System;
using System.Linq;

namespace Scolita.Editor.Voice
{
    /// <summary>
    /// Finds sound and silence in a recording by the level of its 10 ms frames: the silence at a
    /// clip's ends, and the threshold between a take's noise floor and its speech.
    /// </summary>
    public static class SilenceFinder
    {
        /// <summary>The length of a frame.</summary>
        public const double FrameSeconds = 0.01;

        private const double NoisePercentile = 0.1;
        private const double SpeechPercentile = 0.95;
        private const double ThresholdShare = 0.3;
        private const double LeastAboveNoise = 6.0;
        private const double MostBelowSpeech = 40.0;

        /// <summary>The number of samples in a frame at <paramref name="sampleRate"/>.</summary>
        public static int FrameLength(int sampleRate)
        {
            return Math.Max(1, (int)Math.Round(FrameSeconds * sampleRate));
        }

        /// <summary>Each frame's level (RMS) in dBFS; called by the kit and <c>SilenceFinderTests</c>.</summary>
        public static double[] FrameLevels(MonoAudio audio)
        {
            int frame = FrameLength(audio.SampleRate);
            int count = (audio.Samples.Length + frame - 1) / frame;
            double[] levels = new double[count];
            for (int index = 0; index < count; index++)
            {
                int start = index * frame;
                int end = Math.Min(audio.Samples.Length, start + frame);
                double sum = 0.0;
                for (int sample = start; sample < end; sample++)
                {
                    sum += audio.Samples[sample] * (double)audio.Samples[sample];
                }

                levels[index] = Loudness.Decibels(Math.Sqrt(sum / Math.Max(1, end - start)));
            }

            return levels;
        }

        /// <summary>
        /// The level between a take's noise floor (its quiet frames) and its speech (its loud frames),
        /// at least 6 dB above the noise and at most 40 dB under the speech (so a take in digital
        /// silence doesn't count faint artefacts as sound): what counts as sound when splitting a take. Called by
        /// <see cref="TakeSplitter"/> and <c>SilenceFinderTests</c>.
        /// </summary>
        public static double ThresholdOf(double[] levels)
        {
            if (levels.Length == 0)
            {
                return Loudness.Silent;
            }

            double[] sorted = levels.OrderBy(level => level).ToArray();
            double noise = sorted[(int)(NoisePercentile * (sorted.Length - 1))];
            double speech = sorted[(int)(SpeechPercentile * (sorted.Length - 1))];
            return Math.Max(Math.Max(noise + LeastAboveNoise, noise + ThresholdShare * (speech - noise)), speech - MostBelowSpeech);
        }

        /// <summary>The silence at the start of <paramref name="audio"/>, in seconds, under <paramref name="threshold"/> dBFS; called by the kit.</summary>
        public static double LeadingSilence(MonoAudio audio, double threshold)
        {
            double[] levels = FrameLevels(audio);
            int first = Array.FindIndex(levels, level => level > threshold);
            return first < 0 ? audio.Seconds : first * (double)FrameLength(audio.SampleRate) / audio.SampleRate;
        }

        /// <summary>The silence at the end of <paramref name="audio"/>, in seconds, under <paramref name="threshold"/> dBFS; called by the kit.</summary>
        public static double TrailingSilence(MonoAudio audio, double threshold)
        {
            double[] levels = FrameLevels(audio);
            int last = Array.FindLastIndex(levels, level => level > threshold);
            if (last < 0)
            {
                return audio.Seconds;
            }

            double soundEnd = (last + 1) * (double)FrameLength(audio.SampleRate) / audio.SampleRate;
            return Math.Max(0.0, audio.Seconds - soundEnd);
        }
    }
}
