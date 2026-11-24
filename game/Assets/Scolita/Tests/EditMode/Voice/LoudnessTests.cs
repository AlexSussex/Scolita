using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Editor.Voice;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Voice
{
    /// <summary>Tests <see cref="Loudness"/> and <see cref="ClipChecks"/> on sounds made in code.</summary>
    public sealed class LoudnessTests
    {
        /// <summary>Tests that <see cref="Loudness.IntegratedLufs"/> reads a 997 Hz sine of peak A as 20·log10(A) − 3.01 LUFS (BS.1770's reference).</summary>
        [Test]
        public void TestIntegratedLufs_SineReference()
        {
            MonoAudio sine = new MonoAudio(TestSignals.Tone(0.5, 3.0), TestSignals.Rate);

            double lufs = Loudness.IntegratedLufs(sine);

            Assert.That(lufs, Is.EqualTo(20.0 * System.Math.Log10(0.5) - 3.01).Within(0.1));
        }

        /// <summary>Tests that <see cref="Loudness.IntegratedLufs"/> gates out silence, and gives silence alone no loudness.</summary>
        [Test]
        public void TestIntegratedLufs_GatesSilence()
        {
            MonoAudio padded = new MonoAudio(TestTone(0.5, 2.0, 4.0), TestSignals.Rate);
            MonoAudio silence = new MonoAudio(TestSignals.Silence(1.0), TestSignals.Rate);

            Assert.That(Loudness.IntegratedLufs(padded), Is.EqualTo(20.0 * System.Math.Log10(0.5) - 3.01).Within(0.5));
            Assert.That(Loudness.IntegratedLufs(silence), Is.EqualTo(Loudness.Silent));
        }

        /// <summary>Tests that <see cref="Loudness.PeakDbfs"/> and <see cref="Loudness.LongestClippedRun"/> read a sine's peak and a clipped square.</summary>
        [Test]
        public void TestPeakDbfs_AndClipping()
        {
            MonoAudio sine = new MonoAudio(TestSignals.Tone(0.5, 0.5), TestSignals.Rate);
            MonoAudio clipped = new MonoAudio(new[] { 0f, 1f, 1f, 1f, 1f, 0f }, TestSignals.Rate);

            Assert.That(Loudness.PeakDbfs(sine), Is.EqualTo(-6.02).Within(0.05));
            Assert.That(Loudness.LongestClippedRun(sine), Is.EqualTo(0));
            Assert.That(Loudness.LongestClippedRun(clipped), Is.EqualTo(4));
        }

        /// <summary>Tests that <see cref="ClipChecks.Measure"/> passes a clip at −16 LUFS with 100 ms of silence at each end.</summary>
        [Test]
        public void TestMeasure_AGoodClip()
        {
            MonoAudio clip = TestSignals.Join(TestSignals.Silence(0.1), TestSignals.Tone(GoodAmplitude(), 0.6),
                TestSignals.Silence(0.1));

            ClipMeasurement measurement = ClipChecks.Measure(clip, VoiceLineKind.Word);

            Assert.That(measurement.Flags, Is.Empty, measurement.ToString());
        }

        /// <summary>
        /// Tests that <see cref="ClipChecks.Measure"/> flags a clip that is too quiet, one that is
        /// clipped, and one with a second of silence at its start (the brief's acceptance).
        /// </summary>
        [Test]
        public void TestMeasure_FlagsQuietClippedAndSilentStart()
        {
            MonoAudio quiet = TestSignals.Join(TestSignals.Silence(0.1), TestSignals.Tone(0.02, 0.6), TestSignals.Silence(0.1));
            MonoAudio clipped = TestSignals.Join(TestSignals.Silence(0.1), TestSignals.Tone(1.5, 0.6), TestSignals.Silence(0.1));
            MonoAudio lateStart = TestSignals.Join(TestSignals.Silence(1.0), TestSignals.Tone(GoodAmplitude(), 0.6),
                TestSignals.Silence(0.1));

            Assert.That(ClipChecks.Measure(quiet, VoiceLineKind.Word).Flags, Does.Contain(ClipFlag.TooQuiet));
            Assert.That(ClipChecks.Measure(clipped, VoiceLineKind.Word).Flags, Does.Contain(ClipFlag.Clipped));
            Assert.That(ClipChecks.Measure(lateStart, VoiceLineKind.Word).Flags, Does.Contain(ClipFlag.LongSilenceAtStart));
        }

        private static double GoodAmplitude()
        {
            return System.Math.Pow(10.0, (VoiceClipStandard.TargetLufs + 3.01) / 20.0);
        }

        private static float[] TestTone(double amplitude, double toneSeconds, double silenceSeconds)
        {
            float[] tone = TestSignals.Tone(amplitude, toneSeconds);
            float[] padded = new float[tone.Length + (int)(silenceSeconds * TestSignals.Rate)];
            System.Array.Copy(tone, 0, padded, padded.Length - tone.Length, tone.Length);
            return padded;
        }
    }
}
