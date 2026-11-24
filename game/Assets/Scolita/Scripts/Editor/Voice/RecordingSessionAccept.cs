using System.Collections.Generic;
using System.IO;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Content.Csv;
using Scolita.Core.Voice;
using Scolita.Editor.ContentImport;

namespace Scolita.Editor.Voice
{
    /// <summary>
    /// Accept recording session (brief item 9), run when the human says a session is good: writes each
    /// good clip of the session as mono OGG Vorbis into <c>content/voice/</c> (through ffmpeg), names
    /// the clips in <c>voice-lines.csv</c>'s <c>clip</c> cells (a line with several parts only once
    /// all of them are there), and runs the content import, which copies and links them and fills the
    /// <c>status</c> column. Flagged clips are left out, so the next scripts list their lines again.
    /// </summary>
    public static class RecordingSessionAccept
    {
        /// <summary>
        /// Accepts session <paramref name="sessionId"/> of <paramref name="inbox"/> into
        /// <paramref name="content"/>, and returns how many clips went in. Throws an
        /// <see cref="IOException"/> when the session wasn't imported. Called by <see cref="RecordingKitMenu"/>.
        /// </summary>
        public static int Accept(VoiceInbox inbox, string sessionId, Ffmpeg ffmpeg, ContentFolder content,
            ContentProblems problems)
        {
            if (!File.Exists(inbox.Result(sessionId)))
            {
                throw new IOException("The session " + sessionId + " has not been imported yet: run "
                    + "\u0218coli\u021Ba \u2192 Import recording session first.");
            }

            RecordingResult result = RecordingResult.FromJson(File.ReadAllText(inbox.Result(sessionId)));
            RecordingResultItem[] good = result.Items.Where(item => item.IsGood).ToArray();
            Directory.CreateDirectory(content.VoicePath);
            foreach (RecordingResultItem item in good)
            {
                ffmpeg.EncodeVorbis(Path.Combine(inbox.Clips(sessionId), item.Wav), content.ClipPath(item.Clip));
            }

            NameClips(content, problems);
            return good.Length;
        }

        private static void NameClips(ContentFolder content, ContentProblems problems)
        {
            Dictionary<string, string> csvs = content.ReadCsvs(problems);
            Curriculum curriculum = ContentTables.Read(csvs, problems);
            if (problems.HasErrors)
            {
                return;
            }

            Dictionary<string, string> names = new Dictionary<string, string>();
            foreach (VoiceLine line in curriculum.VoiceLines.All.Where(line => !line.HasClip))
            {
                int segments = VoiceTemplate.Parse(line.Text).SpokenSegments;
                string[] clips = Enumerable.Range(0, segments)
                    .Select(segment => VoiceClipNames.For(line.LineId, segment, segments))
                    .ToArray();
                if (segments > 0 && clips.All(content.ClipExists))
                {
                    names.Add(line.Id.Value, string.Join(" ", clips));
                }
            }

            string text = csvs[ContentFileNames.VoiceLines];
            string named = CsvCellWriter.Write(ContentFileNames.VoiceLines, text, ContentTables.IdColumn,
                ContentTables.ClipColumn, ContentTables.IdColumn, names, problems);
            if (named != text)
            {
                content.WriteCsv(ContentFileNames.VoiceLines, named);
            }
        }
    }
}
