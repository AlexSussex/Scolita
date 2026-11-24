using System;

namespace Scolita.Editor.Voice
{
    /// <summary>
    /// Makes one clip from a piece of a take, to <see cref="VoiceClipStandard"/>: the sound with
    /// 100 ms of the take's own silence at each end (less when a neighbour is nearer), 5 ms fades so
    /// it never clicks, levelled to −16 LUFS, with a peak limiter (5 ms look-ahead, 50 ms release)
    /// keeping its peaks under −1 dBFS.
    /// </summary>
    public static class ClipShaper
    {
        /// <summary>The most a piece is raised; one that needs more was recorded too quietly, and is flagged.</summary>
        public const double MaximumGainDb = 30.0;

        private const double PeakHeadroomDb = 0.1;
        private const double LookAheadSeconds = 0.005;
        private const double ReleaseSeconds = 0.05;
        private const int LevellingPasses = 2;

        /// <summary>
        /// The clip of <paramref name="segment"/> in <paramref name="take"/>, whose neighbours' sound
        /// ends at <paramref name="previousEnd"/> and starts at <paramref name="nextStart"/> (samples),
        /// and the gain it was given, in dB. Called by the session import and <c>ClipShaperTests</c>.
        /// </summary>
        public static MonoAudio Shape(MonoAudio take, TakeSegment segment, int previousEnd, int nextStart, out double gainDb)
        {
            int margin = (int)Math.Round(VoiceClipStandard.TargetSilenceSeconds * take.SampleRate);
            int start = Math.Max(Math.Max(0, previousEnd), segment.Start - margin);
            int end = Math.Min(Math.Min(take.Samples.Length, nextStart), segment.End + margin);
            MonoAudio source = take.Slice(start, Math.Max(0, end - start));
            gainDb = 0.0;
            MonoAudio clip = source;
            for (int pass = 0; pass < LevellingPasses; pass++)
            {
                double lufs = Loudness.IntegratedLufs(clip);
                if (lufs <= Loudness.Silent)
                {
                    break;
                }

                gainDb = Math.Min(MaximumGainDb, gainDb + VoiceClipStandard.TargetLufs - lufs);
                clip = Levelled(source, gainDb);
            }

            Fade(clip);
            return clip;
        }

        private static MonoAudio Levelled(MonoAudio source, double gainDb)
        {
            float gain = (float)Math.Pow(10.0, gainDb / 20.0);
            float[] samples = new float[source.Samples.Length];
            for (int index = 0; index < samples.Length; index++)
            {
                samples[index] = source.Samples[index] * gain;
            }

            Limit(samples, source.SampleRate);
            return new MonoAudio(samples, source.SampleRate);
        }

        private static void Limit(float[] samples, int sampleRate)
        {
            float ceiling = (float)Math.Pow(10.0, (VoiceClipStandard.MaximumPeakDbfs - PeakHeadroomDb) / 20.0);
            int lookAhead = Math.Max(1, (int)Math.Round(LookAheadSeconds * sampleRate));
            double recovery = 1.0 / Math.Max(1.0, ReleaseSeconds * sampleRate);
            float[] needed = new float[samples.Length];
            for (int index = 0; index < samples.Length; index++)
            {
                float level = Math.Abs(samples[index]);
                needed[index] = level > ceiling ? ceiling / level : 1f;
            }

            double gain = 1.0;
            for (int index = 0; index < samples.Length; index++)
            {
                float lowest = 1f;
                int last = Math.Min(samples.Length - 1, index + lookAhead);
                for (int ahead = index; ahead <= last; ahead++)
                {
                    lowest = Math.Min(lowest, needed[ahead]);
                }

                gain = Math.Min(lowest, gain + (1.0 - gain) * recovery);
                samples[index] = (float)(samples[index] * gain);
            }
        }

        private static void Fade(MonoAudio clip)
        {
            float[] samples = clip.Samples;
            int fade = Math.Max(1, (int)Math.Round(VoiceClipStandard.FadeSeconds * clip.SampleRate));
            for (int index = 0; index < Math.Min(fade, samples.Length); index++)
            {
                float envelope = (float)index / fade;
                samples[index] *= envelope;
                samples[samples.Length - 1 - index] *= envelope;
            }
        }
    }
}
