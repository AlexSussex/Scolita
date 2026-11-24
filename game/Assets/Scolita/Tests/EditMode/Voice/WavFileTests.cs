using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using Scolita.Editor.Voice;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Voice
{
    /// <summary>Tests <see cref="WavFile"/>, in a temporary folder.</summary>
    public sealed class WavFileTests
    {
        /// <summary>Tests that <see cref="WavFile.Write"/> and <see cref="WavFile.Read"/> keep a clip's samples (to 16-bit precision) and rate.</summary>
        [Test]
        public void TestWrite_ThenRead()
        {
            string path = Path.Combine(Path.GetTempPath(), "scolita-wav-" + Guid.NewGuid().ToString("N") + ".wav");
            MonoAudio tone = new MonoAudio(TestSignals.Tone(0.5, 0.2), TestSignals.Rate);
            try
            {
                WavFile.Write(path, tone);
                MonoAudio read = WavFile.Read(path);

                Assert.That(read.SampleRate, Is.EqualTo(TestSignals.Rate));
                Assert.That(read.Samples.Length, Is.EqualTo(tone.Samples.Length));
                Assert.That(read.Samples[100], Is.EqualTo(tone.Samples[100]).Within(0.0001f));
            }
            finally
            {
                File.Delete(path);
            }
        }

        /// <summary>Tests that <see cref="WavFile.Read"/> mixes a stereo 32-bit float WAV to mono, and refuses a file that isn't a WAV.</summary>
        [Test]
        public void TestRead_StereoFloatAndNotAWav()
        {
            string path = Path.Combine(Path.GetTempPath(), "scolita-wav-" + Guid.NewGuid().ToString("N") + ".wav");
            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Create(path)))
                {
                    writer.Write(Encoding.ASCII.GetBytes("RIFF"));
                    writer.Write(36 + 16);
                    writer.Write(Encoding.ASCII.GetBytes("WAVEfmt "));
                    writer.Write(16);
                    writer.Write((ushort)3);
                    writer.Write((ushort)2);
                    writer.Write(48000);
                    writer.Write(48000 * 8);
                    writer.Write((ushort)8);
                    writer.Write((ushort)32);
                    writer.Write(Encoding.ASCII.GetBytes("data"));
                    writer.Write(16);
                    writer.Write(0.5f);
                    writer.Write(0.1f);
                    writer.Write(-1f);
                    writer.Write(0f);
                }

                MonoAudio read = WavFile.Read(path);
                File.WriteAllText(path, "not a wav");

                Assert.That(read.SampleRate, Is.EqualTo(48000));
                Assert.That(read.Samples, Is.EqualTo(new[] { 0.3f, -0.5f }).Within(0.0001f));
                Assert.That(() => WavFile.Read(path), Throws.TypeOf<InvalidDataException>());
            }
            finally
            {
                File.Delete(path);
            }
        }
    }
}
