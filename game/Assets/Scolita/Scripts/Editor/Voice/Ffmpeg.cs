using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Scolita.Editor.Voice
{
    /// <summary>
    /// ffmpeg, the recording kit's one outside tool (agreed by the human on 2026-11-24; an LGPL
    /// Windows build, on this PC only, never in the repo or the APK): it reads a take recorded as M4A
    /// (the Windows Voice Recorder), or any WAV, into 44.1 kHz mono, and writes the accepted clips as
    /// mono OGG Vorbis. Found through the <c>SCOLITA_FFMPEG</c> environment variable.
    /// </summary>
    public sealed class Ffmpeg
    {
        /// <summary>The environment variable that names ffmpeg.exe.</summary>
        public const string Variable = "SCOLITA_FFMPEG";

        /// <summary>What to tell the human when ffmpeg isn't found.</summary>
        public const string Missing = "ffmpeg was not found: set " + Variable + " to the full path of ffmpeg.exe "
            + "(local/todo/04-record-voices.md, \"Set up once\"), then restart Unity Hub and the Editor.";

        private const int TimeoutMilliseconds = 120000;
        private const string VorbisQuality = "6";

        private readonly string executable;

        private Ffmpeg(string executable)
        {
            this.executable = executable;
        }

        /// <summary>
        /// ffmpeg from <see cref="Variable"/>, or false (with <see cref="Missing"/> for the human) when it
        /// isn't set or isn't there. Called by the recording kit.
        /// </summary>
        public static bool TryFind(out Ffmpeg ffmpeg)
        {
            string path = Environment.GetEnvironmentVariable(Variable);
            ffmpeg = !string.IsNullOrEmpty(path) && File.Exists(path) ? new Ffmpeg(path) : null;
            return ffmpeg != null;
        }

        /// <summary>
        /// Reads <paramref name="input"/> (M4A, WAV, OGG …) into a 44.1 kHz mono WAV at
        /// <paramref name="wavOutput"/>. Throws an <see cref="IOException"/> naming ffmpeg's complaint
        /// when it fails. Called by the session import and Check voice clips.
        /// </summary>
        public void DecodeToWav(string input, string wavOutput)
        {
            Run("-y -v error -i \"" + input + "\" -ac 1 -ar " + VoiceClipStandard.SampleRate
                + " -c:a pcm_f32le \"" + wavOutput + "\"");
        }

        /// <summary>
        /// Writes <paramref name="wavInput"/> as mono OGG Vorbis at 44.1 kHz to <paramref name="oggOutput"/>.
        /// Throws an <see cref="IOException"/> naming ffmpeg's complaint when it fails. Called by Accept
        /// recording session.
        /// </summary>
        public void EncodeVorbis(string wavInput, string oggOutput)
        {
            Run("-y -v error -i \"" + wavInput + "\" -ac 1 -ar " + VoiceClipStandard.SampleRate
                + " -c:a libvorbis -q:a " + VorbisQuality + " -map_metadata -1 \"" + oggOutput + "\"");
        }

        private void Run(string arguments)
        {
            ProcessStartInfo start = new ProcessStartInfo(executable, arguments)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };
            using (Process process = Process.Start(start))
            {
                if (process == null)
                {
                    throw new IOException("ffmpeg could not be started from " + executable + ".");
                }

                Task<string> errors = process.StandardError.ReadToEndAsync();
                Task<string> output = process.StandardOutput.ReadToEndAsync();
                if (!process.WaitForExit(TimeoutMilliseconds))
                {
                    process.Kill();
                    throw new IOException("ffmpeg took too long: " + arguments);
                }

                Task.WaitAll(errors, output);
                if (process.ExitCode != 0)
                {
                    throw new IOException("ffmpeg failed (" + process.ExitCode + "): " + errors.Result.Trim());
                }
            }
        }
    }
}
