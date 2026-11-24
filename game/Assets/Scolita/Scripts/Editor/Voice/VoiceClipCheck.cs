using System.Collections.Generic;
using System.IO;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Editor.ContentImport;

namespace Scolita.Editor.Voice
{
    /// <summary>
    /// Check voice clips (brief item 6): measures every clip in <c>content/voice/</c> (decoded through
    /// ffmpeg) against <see cref="VoiceClipStandard"/>: its length, loudness, peak and the silence at
    /// its ends, flagging what is off.
    /// </summary>
    public static class VoiceClipCheck
    {
        /// <summary>
        /// Each clip of <paramref name="content"/> with its measurement, in name order, decoding into
        /// <paramref name="workFolder"/>. Called by <see cref="RecordingKitMenu"/>.
        /// </summary>
        public static IReadOnlyList<KeyValuePair<string, ClipMeasurement>> Check(ContentFolder content, Ffmpeg ffmpeg,
            string workFolder)
        {
            List<KeyValuePair<string, ClipMeasurement>> checks = new List<KeyValuePair<string, ClipMeasurement>>();
            if (!Directory.Exists(content.VoicePath))
            {
                return checks;
            }

            Directory.CreateDirectory(workFolder);
            string decoded = Path.Combine(workFolder, "check.wav");
            foreach (string clip in Directory.GetFiles(content.VoicePath, "*" + VoiceClipNames.Extension).OrderBy(path => path))
            {
                ffmpeg.DecodeToWav(clip, decoded);
                string name = Path.GetFileName(clip);
                checks.Add(new KeyValuePair<string, ClipMeasurement>(name, ClipChecks.Measure(WavFile.Read(decoded), KindOf(name))));
            }

            File.Delete(decoded);
            return checks;
        }

        /// <summary>The kind of the line a clip named <paramref name="clipName"/> belongs to (a line, when unknown); called by the check and tests.</summary>
        public static VoiceLineKind KindOf(string clipName)
        {
            string id = clipName.Split('.')[0];
            return ContentId.TryCreate(id, out ContentId lineId) && VoiceLineIds.TryParse(lineId, out Language _, out VoiceLineKind kind)
                ? kind
                : VoiceLineKind.Line;
        }
    }
}
