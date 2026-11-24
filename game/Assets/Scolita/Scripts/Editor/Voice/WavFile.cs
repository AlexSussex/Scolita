using System;
using System.IO;
using System.Text;

namespace Scolita.Editor.Voice
{
    /// <summary>
    /// Reads and writes WAV files in plain C#, with no tool: 16, 24 or 32-bit PCM and 32-bit float,
    /// mono or stereo (mixed to mono) at any rate when reading; 16-bit mono PCM when writing.
    /// </summary>
    public static class WavFile
    {
        private const ushort PcmFormat = 1;
        private const ushort FloatFormat = 3;
        private const ushort ExtensibleFormat = 0xFFFE;

        /// <summary>
        /// Reads <paramref name="path"/>, mixing its channels to mono. Throws an
        /// <see cref="InvalidDataException"/> for a file that isn't a WAV this can read. Called by the
        /// recording kit and <c>WavFileTests</c>.
        /// </summary>
        public static MonoAudio Read(string path)
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
            {
                if (Tag(reader) != "RIFF")
                {
                    throw new InvalidDataException(path + " is not a WAV file.");
                }

                reader.ReadUInt32();
                if (Tag(reader) != "WAVE")
                {
                    throw new InvalidDataException(path + " is not a WAV file.");
                }

                ushort format = 0;
                ushort channels = 0;
                int sampleRate = 0;
                ushort bits = 0;
                while (reader.BaseStream.Position + 8 <= reader.BaseStream.Length)
                {
                    string chunk = Tag(reader);
                    uint size = reader.ReadUInt32();
                    long next = reader.BaseStream.Position + size + (size % 2);
                    if (chunk == "fmt ")
                    {
                        format = reader.ReadUInt16();
                        channels = reader.ReadUInt16();
                        sampleRate = reader.ReadInt32();
                        reader.ReadUInt32();
                        reader.ReadUInt16();
                        bits = reader.ReadUInt16();
                        if (format == ExtensibleFormat && size >= 26)
                        {
                            reader.ReadUInt16();
                            reader.ReadUInt16();
                            reader.ReadUInt32();
                            format = reader.ReadUInt16();
                        }
                    }
                    else if (chunk == "data")
                    {
                        if (channels == 0)
                        {
                            throw new InvalidDataException(path + " has its data before its format.");
                        }

                        byte[] data = reader.ReadBytes((int)Math.Min(size, reader.BaseStream.Length - reader.BaseStream.Position));
                        return new MonoAudio(Decode(path, data, format, channels, bits), sampleRate);
                    }

                    reader.BaseStream.Position = Math.Min(next, reader.BaseStream.Length);
                }

                throw new InvalidDataException(path + " has no sound data.");
            }
        }

        /// <summary>Writes <paramref name="audio"/> to <paramref name="path"/> as 16-bit mono PCM; called by the recording kit and <c>WavFileTests</c>.</summary>
        public static void Write(string path, MonoAudio audio)
        {
            int dataSize = audio.Samples.Length * 2;
            using (BinaryWriter writer = new BinaryWriter(File.Create(path)))
            {
                writer.Write(Encoding.ASCII.GetBytes("RIFF"));
                writer.Write(36 + dataSize);
                writer.Write(Encoding.ASCII.GetBytes("WAVEfmt "));
                writer.Write(16);
                writer.Write(PcmFormat);
                writer.Write((ushort)1);
                writer.Write(audio.SampleRate);
                writer.Write(audio.SampleRate * 2);
                writer.Write((ushort)2);
                writer.Write((ushort)16);
                writer.Write(Encoding.ASCII.GetBytes("data"));
                writer.Write(dataSize);
                foreach (float sample in audio.Samples)
                {
                    float clamped = Math.Max(-1f, Math.Min(1f, sample));
                    writer.Write((short)Math.Round(clamped * short.MaxValue));
                }
            }
        }

        private static string Tag(BinaryReader reader)
        {
            return Encoding.ASCII.GetString(reader.ReadBytes(4));
        }

        private static float[] Decode(string path, byte[] data, ushort format, ushort channels, ushort bits)
        {
            int bytesPerSample = bits / 8;
            bool known = (format == PcmFormat && (bits == 16 || bits == 24 || bits == 32))
                || (format == FloatFormat && bits == 32);
            if (!known)
            {
                throw new InvalidDataException(path + " is a WAV of a kind this can't read (format " + format + ", "
                    + bits + " bits): export it as 16-bit PCM, or record the take as M4A.");
            }

            int frames = data.Length / (bytesPerSample * channels);
            float[] samples = new float[frames];
            for (int frame = 0; frame < frames; frame++)
            {
                float sum = 0f;
                for (int channel = 0; channel < channels; channel++)
                {
                    int offset = (frame * channels + channel) * bytesPerSample;
                    sum += Sample(data, offset, format, bits);
                }

                samples[frame] = sum / channels;
            }

            return samples;
        }

        private static float Sample(byte[] data, int offset, ushort format, ushort bits)
        {
            if (format == FloatFormat)
            {
                return BitConverter.ToSingle(data, offset);
            }

            switch (bits)
            {
                case 16:
                    return BitConverter.ToInt16(data, offset) / 32768f;
                case 24:
                    int value = data[offset] | (data[offset + 1] << 8) | ((sbyte)data[offset + 2] << 16);
                    return value / 8388608f;
                default:
                    return BitConverter.ToInt32(data, offset) / 2147483648f;
            }
        }
    }
}
