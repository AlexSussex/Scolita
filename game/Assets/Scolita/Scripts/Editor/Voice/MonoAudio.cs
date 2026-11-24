using System;

namespace Scolita.Editor.Voice
{
    /// <summary>A mono recording as samples from -1 to 1, with its sample rate: what the recording kit measures, splits and shapes.</summary>
    public sealed class MonoAudio
    {
        /// <summary>Wraps <paramref name="samples"/> at <paramref name="sampleRate"/>; called by <see cref="WavFile"/>, the kit and its tests.</summary>
        public MonoAudio(float[] samples, int sampleRate)
        {
            if (sampleRate <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sampleRate), "A sample rate is positive.");
            }

            Samples = samples ?? throw new ArgumentNullException(nameof(samples));
            SampleRate = sampleRate;
        }

        /// <summary>The samples.</summary>
        public float[] Samples { get; }

        /// <summary>Samples per second.</summary>
        public int SampleRate { get; }

        /// <summary>Its length in seconds.</summary>
        public double Seconds => (double)Samples.Length / SampleRate;

        /// <summary>The part from sample <paramref name="start"/>, <paramref name="length"/> samples long; called by the kit.</summary>
        public MonoAudio Slice(int start, int length)
        {
            float[] part = new float[length];
            Array.Copy(Samples, start, part, 0, length);
            return new MonoAudio(part, SampleRate);
        }
    }
}
