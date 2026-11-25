using System;
using Scolita.Core.Randomness;

namespace Scolita.Editor.Sounds
{
    /// <summary>
    /// The few instruments the app's own sounds are made of (<see cref="SoundMaker"/>), written into a buffer of mono
    /// samples at <see cref="Rate"/>: a gliding sine, a bell (a few partials, the higher ones dying sooner), a soft pad
    /// note, a plucked note and filtered noise. Plain C#, deterministic for a given seed, so the sounds can be remade.
    /// </summary>
    public sealed class Synth
    {
        /// <summary>Samples per second of every sound made.</summary>
        public const int Rate = 44100;

        private static readonly double[] BellPartials = { 1.0, 2.0, 3.01, 4.2, 5.43 };
        private static readonly double[] BellLevels = { 1.0, 0.36, 0.14, 0.08, 0.04 };
        private static readonly double[] BellDecays = { 1.0, 0.6, 0.4, 0.25, 0.18 };

        private readonly float[] samples;
        private readonly IRandom random;

        /// <summary>A silent buffer of <paramref name="seconds"/>, its noise drawn from <paramref name="seed"/>; called by the recipes.</summary>
        public Synth(double seconds, ulong seed)
        {
            samples = new float[Math.Max(1, (int)Math.Ceiling(seconds * Rate))];
            random = new SeededRandom(seed);
        }

        /// <summary>The samples written so far.</summary>
        public float[] Samples => samples;

        /// <summary>The frequency of the MIDI note <paramref name="note"/> (69 is A4, 440 Hz); called by the recipes.</summary>
        public static double Hz(int note)
        {
            return 440.0 * Math.Pow(2.0, (note - 69) / 12.0);
        }

        /// <summary>
        /// A sine from <paramref name="fromHz"/> to <paramref name="toHz"/> (an exponential glide over <paramref name="glideSeconds"/>), rising in
        /// <paramref name="attack"/> seconds and dying away with the time constant <paramref name="decay"/>, with a vibrato of
        /// <paramref name="vibratoDepth"/> (of the frequency) at <paramref name="vibratoHz"/>; called by the recipes.
        /// </summary>
        public void Sine(double start, double seconds, double fromHz, double toHz, double glideSeconds, double gain,
            double attack, double decay, double vibratoDepth = 0.0, double vibratoHz = 0.0)
        {
            double phase = 0.0;
            int first = Index(start);
            int count = Count(seconds);
            for (int index = 0; index < count && first + index < samples.Length; index++)
            {
                double time = (double)index / Rate;
                double glide = glideSeconds > 0.0 ? Math.Min(1.0, time / glideSeconds) : 1.0;
                double hz = fromHz * Math.Pow(toHz / fromHz, glide);
                hz *= 1.0 + vibratoDepth * Math.Sin(2.0 * Math.PI * vibratoHz * time) * (1.0 - glide * 0.5);
                phase += 2.0 * Math.PI * hz / Rate;
                samples[first + index] += (float)(gain * Envelope(time, seconds, attack, decay) * Math.Sin(phase));
            }
        }

        /// <summary>A bell (or a celesta) at <paramref name="hz"/>, ringing for about <paramref name="decay"/> seconds; called by the recipes.</summary>
        public void Bell(double start, double hz, double gain, double decay)
        {
            double seconds = decay * 5.0;
            for (int partial = 0; partial < BellPartials.Length; partial++)
            {
                double partialHz = hz * BellPartials[partial];
                if (partialHz >= Rate * 0.45)
                {
                    continue;
                }

                Sine(start, seconds, partialHz, partialHz, 0.0, gain * BellLevels[partial], 0.003, decay * BellDecays[partial]);
            }
        }

        /// <summary>A plucked note (a soft marimba) at <paramref name="hz"/>; called by the music.</summary>
        public void Pluck(double start, double hz, double gain, double decay)
        {
            Sine(start, decay * 6.0, hz, hz, 0.0, gain, 0.004, decay);
            Sine(start, decay * 3.0, hz * 4.0, hz * 4.0, 0.0, gain * 0.12, 0.002, decay * 0.25);
            Sine(start, decay * 3.0, hz * 2.0, hz * 2.0, 0.0, gain * 0.2, 0.003, decay * 0.5);
        }

        /// <summary>
        /// A pad note at <paramref name="hz"/> held for <paramref name="seconds"/>: three slightly detuned voices with a slow
        /// swell in and a slow release; called by the music.
        /// </summary>
        public void Pad(double start, double seconds, double hz, double gain, double attack, double release)
        {
            double[] detune = { 1.0, 1.0035, 0.9968 };
            for (int voice = 0; voice < detune.Length; voice++)
            {
                double voiceHz = hz * detune[voice];
                double phase = voice * 1.7;
                int first = Index(start);
                int count = Count(seconds + release);
                for (int index = 0; index < count && first + index < samples.Length; index++)
                {
                    double time = (double)index / Rate;
                    double level = time < attack ? Smooth(time / attack)
                        : time < seconds ? 1.0
                        : 1.0 - Smooth((time - seconds) / release);
                    phase += 2.0 * Math.PI * voiceHz * (1.0 + 0.002 * Math.Sin(2.0 * Math.PI * 0.3 * time + voice)) / Rate;
                    double tone = Math.Sin(phase) + 0.18 * Math.Sin(2.0 * phase) + 0.06 * Math.Sin(3.0 * phase);
                    samples[first + index] += (float)(gain / detune.Length * level * tone);
                }
            }
        }

        /// <summary>
        /// Noise through a low-pass filter whose corner moves from <paramref name="fromHz"/> to <paramref name="toHz"/> and back
        /// to <paramref name="endHz"/> (at <paramref name="turnShare"/> of the time), with the loudness rising over
        /// <paramref name="attack"/> seconds and falling to the end; called by the recipes.
        /// </summary>
        public void Noise(double start, double seconds, double gain, double fromHz, double toHz, double endHz,
            double turnShare, double attack)
        {
            int first = Index(start);
            int count = Count(seconds);
            double low = 0.0;
            double lower = 0.0;
            for (int index = 0; index < count && first + index < samples.Length; index++)
            {
                double share = (double)index / count;
                double hz = share < turnShare
                    ? fromHz * Math.Pow(toHz / fromHz, share / turnShare)
                    : toHz * Math.Pow(endHz / toHz, (share - turnShare) / Math.Max(1e-6, 1.0 - turnShare));
                double coefficient = 1.0 - Math.Exp(-2.0 * Math.PI * hz / Rate);
                double white = random.NextInt(0, 65536) / 32768.0 - 1.0;
                low += coefficient * (white - low);
                lower += coefficient * (low - lower);
                double time = (double)index / Rate;
                double level = time < attack ? Smooth(time / attack) : 1.0 - Smooth((time - attack) / Math.Max(1e-6, seconds - attack));
                samples[first + index] += (float)(gain * level * lower * 3.0);
            }
        }

        /// <summary>Fades the last <paramref name="seconds"/> to silence, so no clip ends on a click; called by <see cref="SoundMaker"/>.</summary>
        public void FadeOutEnd(double seconds)
        {
            int count = Math.Min(samples.Length, Count(seconds));
            for (int index = 0; index < count; index++)
            {
                samples[samples.Length - 1 - index] *= (float)((double)index / count);
            }
        }

        /// <summary>
        /// Adds a small room (four damped combs and two all-passes, Schroeder's reverb), <paramref name="wet"/> of it mixed in;
        /// called by the music and the longer effects.
        /// </summary>
        public void Reverb(double wet, double roomFeedback)
        {
            int[] combs = { 1557, 1617, 1491, 1422 };
            int[] allPasses = { 225, 556 };
            float[] room = new float[samples.Length];
            foreach (int delay in combs)
            {
                float[] line = new float[delay];
                double damped = 0.0;
                for (int index = 0, at = 0; index < samples.Length; index++, at = (at + 1) % delay)
                {
                    double output = line[at];
                    damped = output * 0.7 + damped * 0.3;
                    line[at] = (float)(samples[index] + damped * roomFeedback);
                    room[index] += (float)(output / combs.Length);
                }
            }

            foreach (int delay in allPasses)
            {
                float[] line = new float[delay];
                for (int index = 0, at = 0; index < room.Length; index++, at = (at + 1) % delay)
                {
                    double delayed = line[at];
                    double input = room[index];
                    line[at] = (float)(input + delayed * 0.5);
                    room[index] = (float)(delayed - input * 0.5);
                }
            }

            for (int index = 0; index < samples.Length; index++)
            {
                samples[index] = (float)(samples[index] * (1.0 - wet * 0.5) + room[index] * wet);
            }
        }

        /// <summary>
        /// Folds everything after <paramref name="loopSeconds"/> back onto the start and cuts it there, so the music loops
        /// without a seam (the notes and the room ringing past the end sound again at the start). Called by the music.
        /// </summary>
        public float[] Looped(double loopSeconds)
        {
            int length = Math.Min(samples.Length, Count(loopSeconds));
            float[] loop = new float[length];
            for (int index = 0; index < samples.Length; index++)
            {
                loop[index % length] += samples[index];
            }

            return loop;
        }

        private static double Envelope(double time, double seconds, double attack, double decay)
        {
            double rise = attack > 0.0 && time < attack ? time / attack : 1.0;
            double fall = decay > 0.0 ? Math.Exp(-Math.Max(0.0, time - attack) / decay) : 1.0;
            double end = seconds - time < 0.005 ? Math.Max(0.0, (seconds - time) / 0.005) : 1.0;
            return rise * fall * end;
        }

        private static double Smooth(double share)
        {
            double clamped = Math.Max(0.0, Math.Min(1.0, share));
            return clamped * clamped * (3.0 - 2.0 * clamped);
        }

        private static int Index(double seconds)
        {
            return (int)Math.Round(seconds * Rate);
        }

        private static int Count(double seconds)
        {
            return Math.Max(1, (int)Math.Round(seconds * Rate));
        }
    }
}
