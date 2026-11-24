using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Editor.Voice;

namespace Scolita.Tests.EditMode.Fakes
{
    /// <summary>
    /// Sounds made in code for the recording kit's tests, so no audio is committed: tones, silence,
    /// quiet noise, joined end to end, at 44.1 kHz.
    /// </summary>
    public static class TestSignals
    {
        /// <summary>The tests' sample rate.</summary>
        public const int Rate = 44100;

        /// <summary>A 997 Hz sine of peak <paramref name="amplitude"/>, <paramref name="seconds"/> long; called by the kit's tests.</summary>
        public static float[] Tone(double amplitude, double seconds, double frequency = 997.0)
        {
            int count = (int)Math.Round(seconds * Rate);
            return Enumerable.Range(0, count)
                .Select(index => (float)(amplitude * Math.Sin(2.0 * Math.PI * frequency * index / Rate)))
                .ToArray();
        }

        /// <summary>Silence, <paramref name="seconds"/> long; called by the kit's tests.</summary>
        public static float[] Silence(double seconds)
        {
            return new float[(int)Math.Round(seconds * Rate)];
        }

        /// <summary>
        /// <paramref name="parts"/> joined, with a quiet noise floor (−70 dBFS, a fixed seed) added, as
        /// a room would; called by the kit's tests.
        /// </summary>
        public static MonoAudio Join(params float[][] parts)
        {
            float[] joined = parts.SelectMany(part => part).ToArray();
            Random random = new Random(7);
            for (int index = 0; index < joined.Length; index++)
            {
                joined[index] += (float)((random.NextDouble() * 2.0 - 1.0) * 0.0003);
            }

            return new MonoAudio(joined, Rate);
        }

        /// <summary>
        /// A take: the bursts <paramref name="burstSeconds"/> (tones of a third of full scale), each
        /// followed by <paramref name="pauseSeconds"/> of silence, after half a second; called by the kit's tests.
        /// </summary>
        public static MonoAudio Take(IEnumerable<double> burstSeconds, double pauseSeconds = 2.0)
        {
            List<float[]> parts = new List<float[]> { Silence(0.5) };
            foreach (double seconds in burstSeconds)
            {
                parts.Add(Tone(0.3, seconds, 440.0));
                parts.Add(Silence(pauseSeconds));
            }

            return Join(parts.ToArray());
        }
    }
}
