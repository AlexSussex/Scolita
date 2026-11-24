using System;
using System.Collections.Generic;
using System.Linq;

namespace Scolita.Editor.Voice
{
    /// <summary>
    /// Loudness and peaks, as AGENTS.md → "Audio" measures them: integrated loudness by ITU-R
    /// BS.1770-4 (K-weighting, 400 ms blocks every 100 ms, gated at −70 LUFS and 10 LU under the
    /// mean; a clip shorter than a block is one block), the sample peak, and the longest run of
    /// clipped samples.
    /// </summary>
    public static class Loudness
    {
        /// <summary>The loudness given to silence, which has none.</summary>
        public const double Silent = -120.0;

        private const double BlockSeconds = 0.4;
        private const double StepSeconds = 0.1;
        private const double AbsoluteGate = -70.0;
        private const double RelativeGate = -10.0;
        private const double Offset = -0.691;

        /// <summary>The integrated loudness of <paramref name="audio"/> in LUFS; called by the kit and <c>LoudnessTests</c>.</summary>
        public static double IntegratedLufs(MonoAudio audio)
        {
            double[] weighted = KWeighted(audio);
            if (weighted.Length == 0)
            {
                return Silent;
            }

            List<double> blocks = BlockPowers(weighted, audio.SampleRate);
            double[] aboveAbsolute = blocks.Where(power => LoudnessOf(power) > AbsoluteGate).ToArray();
            if (aboveAbsolute.Length == 0)
            {
                return Silent;
            }

            double relative = LoudnessOf(aboveAbsolute.Average()) + RelativeGate;
            double[] gated = aboveAbsolute.Where(power => LoudnessOf(power) > relative).ToArray();
            return gated.Length == 0 ? Silent : LoudnessOf(gated.Average());
        }

        /// <summary>The highest sample of <paramref name="audio"/> in dBFS; called by the kit and <c>LoudnessTests</c>.</summary>
        public static double PeakDbfs(MonoAudio audio)
        {
            float peak = 0f;
            foreach (float sample in audio.Samples)
            {
                peak = Math.Max(peak, Math.Abs(sample));
            }

            return Decibels(peak);
        }

        /// <summary>The longest run of samples at or above <see cref="VoiceClipStandard.ClippedLevel"/>; called by <see cref="ClipChecks"/>.</summary>
        public static int LongestClippedRun(MonoAudio audio)
        {
            int longest = 0;
            int run = 0;
            foreach (float sample in audio.Samples)
            {
                run = Math.Abs(sample) >= VoiceClipStandard.ClippedLevel ? run + 1 : 0;
                longest = Math.Max(longest, run);
            }

            return longest;
        }

        /// <summary>An amplitude (of full scale) in dBFS; called by the kit.</summary>
        public static double Decibels(double amplitude)
        {
            return amplitude <= 0.0 ? Silent : Math.Max(Silent, 20.0 * Math.Log10(amplitude));
        }

        private static double LoudnessOf(double power)
        {
            return power <= 0.0 ? Silent : Offset + 10.0 * Math.Log10(power);
        }

        private static List<double> BlockPowers(double[] weighted, int sampleRate)
        {
            int block = (int)Math.Round(BlockSeconds * sampleRate);
            int step = (int)Math.Round(StepSeconds * sampleRate);
            List<double> powers = new List<double>();
            if (weighted.Length < block)
            {
                powers.Add(MeanSquare(weighted, 0, weighted.Length));
                return powers;
            }

            for (int start = 0; start + block <= weighted.Length; start += step)
            {
                powers.Add(MeanSquare(weighted, start, block));
            }

            return powers;
        }

        private static double MeanSquare(double[] samples, int start, int length)
        {
            double sum = 0.0;
            for (int index = start; index < start + length; index++)
            {
                sum += samples[index] * samples[index];
            }

            return sum / length;
        }

        private static double[] KWeighted(MonoAudio audio)
        {
            double rate = audio.SampleRate;
            double shelfK = Math.Tan(Math.PI * 1681.974450955533 / rate);
            double shelfQ = 0.7071752369554196;
            double gain = Math.Pow(10.0, 3.999843853973347 / 20.0);
            double bandGain = Math.Pow(gain, 0.4996667741545416);
            double shelfA0 = 1.0 + shelfK / shelfQ + shelfK * shelfK;
            double[] shelf =
            {
                (gain + bandGain * shelfK / shelfQ + shelfK * shelfK) / shelfA0,
                2.0 * (shelfK * shelfK - gain) / shelfA0,
                (gain - bandGain * shelfK / shelfQ + shelfK * shelfK) / shelfA0,
                2.0 * (shelfK * shelfK - 1.0) / shelfA0,
                (1.0 - shelfK / shelfQ + shelfK * shelfK) / shelfA0
            };
            double passK = Math.Tan(Math.PI * 38.13547087602444 / rate);
            double passQ = 0.5003270373238773;
            double passA0 = 1.0 + passK / passQ + passK * passK;
            double[] highPass =
            {
                1.0, -2.0, 1.0,
                2.0 * (passK * passK - 1.0) / passA0,
                (1.0 - passK / passQ + passK * passK) / passA0
            };
            double[] samples = audio.Samples.Select(sample => (double)sample).ToArray();
            return Biquad(Biquad(samples, shelf), highPass);
        }

        private static double[] Biquad(double[] input, double[] c)
        {
            double[] output = new double[input.Length];
            double x1 = 0.0;
            double x2 = 0.0;
            double y1 = 0.0;
            double y2 = 0.0;
            for (int index = 0; index < input.Length; index++)
            {
                double x = input[index];
                double y = c[0] * x + c[1] * x1 + c[2] * x2 - c[3] * y1 - c[4] * y2;
                x2 = x1;
                x1 = x;
                y2 = y1;
                y1 = y;
                output[index] = y;
            }

            return output;
        }
    }
}
