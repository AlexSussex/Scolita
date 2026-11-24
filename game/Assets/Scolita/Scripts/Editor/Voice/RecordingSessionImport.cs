using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Scolita.Core.Content;

namespace Scolita.Editor.Voice
{
    /// <summary>
    /// Import recording session (brief item 9): reads a session's take (an M4A from the Windows Voice
    /// Recorder through ffmpeg, or a 44.1 kHz WAV directly), cuts it at its pauses, matches the
    /// pieces to the script in order (<see cref="TakeMatcher"/>), shapes each clip
    /// (<see cref="ClipShaper"/>), checks it (<see cref="ClipChecks"/>), and writes the clips as WAV,
    /// a report page and <c>result.json</c> into the session's folder. Nothing goes into
    /// <c>content/voice/</c> here: that is <see cref="RecordingSessionAccept"/>, when the human says
    /// the session is good.
    /// </summary>
    public static class RecordingSessionImport
    {
        private static readonly string[] TakeExtensions = { ".m4a", ".wav", ".mp3", ".aac", ".ogg" };

        /// <summary>
        /// Imports session <paramref name="sessionId"/> of <paramref name="inbox"/>, decoding with
        /// <paramref name="ffmpeg"/> (null when it isn't there: only a 44.1 kHz WAV take can be read).
        /// Throws an <see cref="IOException"/> saying what to do when the manifest or the take is
        /// missing or can't be read. Called by <see cref="RecordingKitMenu"/>.
        /// </summary>
        public static RecordingResult Import(VoiceInbox inbox, string sessionId, Ffmpeg ffmpeg)
        {
            if (!File.Exists(inbox.Manifest(sessionId)))
            {
                throw new IOException("There is no script for the session " + sessionId + " (" + inbox.Manifest(sessionId)
                    + "): run \u0218coli\u021Ba \u2192 Make recording scripts, and name the take's folder after its session.");
            }

            RecordingSession session = RecordingSession.FromJson(File.ReadAllText(inbox.Manifest(sessionId)));
            MonoAudio take = ReadTake(inbox, sessionId, ffmpeg);
            IReadOnlyList<ImportedClip> clips = Cut(session, take, out string summary);
            string clipsFolder = inbox.Clips(sessionId);
            if (Directory.Exists(clipsFolder))
            {
                Directory.Delete(clipsFolder, true);
            }

            Directory.CreateDirectory(clipsFolder);
            List<RecordingResultItem> items = new List<RecordingResultItem>();
            foreach (ImportedClip clip in clips)
            {
                string wav = string.Empty;
                if (clip.Audio != null)
                {
                    wav = (clip.Item.IsLevelTest ? "level-test" : Path.GetFileNameWithoutExtension(clip.Item.Clip)) + ".wav";
                    WavFile.Write(Path.Combine(clipsFolder, wav), clip.Audio);
                }

                items.Add(new RecordingResultItem(clip.Item, wav, clip.Measurement?.ToString(), clip.Flags));
            }

            RecordingResult result = new RecordingResult(sessionId, summary, items);
            UTF8Encoding utf8 = new UTF8Encoding(false);
            File.WriteAllText(inbox.Result(sessionId), result.ToJson(), utf8);
            File.WriteAllText(inbox.Report(sessionId), RecordingReportPage.Render(session, result), utf8);
            return result;
        }

        /// <summary>
        /// Cuts <paramref name="take"/> into the clips of <paramref name="session"/>'s items, saying in
        /// <paramref name="summary"/> how its pieces matched. Called by <see cref="Import"/> and
        /// <c>RecordingSessionImportTests</c>.
        /// </summary>
        public static IReadOnlyList<ImportedClip> Cut(RecordingSession session, MonoAudio take, out string summary)
        {
            IReadOnlyList<TakeSegment> segments = TakeSplitter.Split(take, out double _);
            TakeMatch match = TakeMatcher.Match(
                session.Items.Select(item => (double)item.ExpectedSeconds).ToArray(),
                segments.Select(segment => (double)segment.Length / take.SampleRate).ToArray());
            summary = match.Summary;
            List<ImportedClip> clips = new List<ImportedClip>();
            for (int index = 0; index < session.Items.Count; index++)
            {
                RecordingItem item = session.Items[index];
                List<ClipFlag> flags = new List<ClipFlag>(match.FlagsOf(index));
                int piece = match.SegmentOf(index);
                if (piece < 0)
                {
                    clips.Add(new ImportedClip(item, null, null, flags));
                    continue;
                }

                int previousEnd = piece > 0 ? segments[piece - 1].End : 0;
                int nextStart = piece + 1 < segments.Count ? segments[piece + 1].Start : take.Samples.Length;
                MonoAudio audio = ClipShaper.Shape(take, segments[piece], previousEnd, nextStart, out double gain);
                VoiceLineKind kind = ContentCodes.TryParseVoiceLineKind(item.Kind, out VoiceLineKind parsed) ? parsed : VoiceLineKind.Line;
                ClipMeasurement measurement = ClipChecks.Measure(audio, kind);
                flags.AddRange(measurement.Flags.Where(flag => !flags.Contains(flag)));
                if (gain >= ClipShaper.MaximumGainDb && !flags.Contains(ClipFlag.TooQuiet))
                {
                    flags.Add(ClipFlag.TooQuiet);
                }

                clips.Add(new ImportedClip(item, audio, measurement, flags));
            }

            return clips;
        }

        private static MonoAudio ReadTake(VoiceInbox inbox, string sessionId, Ffmpeg ffmpeg)
        {
            string folder = inbox.Session(sessionId);
            string[] takes = Directory.Exists(folder)
                ? Directory.GetFiles(folder).Where(file => TakeExtensions.Contains(Path.GetExtension(file).ToLowerInvariant())).ToArray()
                : new string[0];
            if (takes.Length != 1)
            {
                throw new IOException("The folder " + folder + " must hold the session's one recording (an .m4a from "
                    + "Voice Recorder, or a .wav); it holds " + takes.Length + ".");
            }

            string take = takes[0];
            if (ffmpeg == null)
            {
                return ReadWavWithoutFfmpeg(take);
            }

            Directory.CreateDirectory(inbox.Work(sessionId));
            string decoded = Path.Combine(inbox.Work(sessionId), "take.wav");
            ffmpeg.DecodeToWav(take, decoded);
            return WavFile.Read(decoded);
        }

        private static MonoAudio ReadWavWithoutFfmpeg(string take)
        {
            if (!string.Equals(Path.GetExtension(take), ".wav", StringComparison.OrdinalIgnoreCase))
            {
                throw new IOException(Ffmpeg.Missing + " It is needed to read " + Path.GetFileName(take) + ".");
            }

            MonoAudio audio = WavFile.Read(take);
            if (audio.SampleRate != VoiceClipStandard.SampleRate)
            {
                throw new IOException(Path.GetFileName(take) + " is at " + audio.SampleRate + " Hz; without ffmpeg only "
                    + VoiceClipStandard.SampleRate + " Hz is read. " + Ffmpeg.Missing);
            }

            return audio;
        }
    }
}
