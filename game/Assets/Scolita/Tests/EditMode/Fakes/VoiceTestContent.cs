using System.Collections.Generic;
using Scolita.Core.Content;
using Scolita.Core.Voice;

namespace Scolita.Tests.EditMode.Fakes
{
    /// <summary>
    /// The voice tests' content: <see cref="ContentCsvs.Valid"/> with the sounds of m and a recorded,
    /// and a few lines with slots and a slow word added; and a way to let a voice say everything.
    /// Used by <c>VoiceRouterTests</c>.
    /// </summary>
    public static class VoiceTestContent
    {
        /// <summary>The clip of the Romanian sound /m/.</summary>
        public const string SoundM = "ro-sound-m.ogg";

        /// <summary>The clip of the Romanian sound /a/.</summary>
        public const string SoundA = "ro-sound-a.ogg";

        /// <summary>The timings the voice tests use: a 0.3 s pause, and the bubble for at least 1.5 s.</summary>
        public static VoiceTiming Timing => new VoiceTiming(0.3f, 0.08f, 1.5f, 3f);

        /// <summary>The content's CSVs, by file name; called by the voice tests, which may change them before <see cref="Read"/>.</summary>
        public static Dictionary<string, string> Csvs()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            ContentCsvs.ReplaceRow(csvs, ContentFileNames.VoiceLines, "ro-sound-m,", "ro-sound-m,ro,sound,/m/," + SoundM + ",recorded,,");
            ContentCsvs.ReplaceRow(csvs, ContentFileNames.VoiceLines, "ro-sound-a,", "ro-sound-a,ro,sound,/a/," + SoundA + ",recorded,,");
            ContentCsvs.AddRow(csvs, ContentFileNames.VoiceLines, "ro-slow-mama,ro,slow,m-a-m-a,,sounds,,");
            ContentCsvs.AddRow(csvs, ContentFileNames.VoiceLines, "ro-line-hello-name,ro,line,\"Bun\u0103, {name}! Hai s\u0103 zbur\u0103m!\",,,,");
            return csvs;
        }

        /// <summary>The curriculum of <paramref name="csvs"/>; called by the voice tests.</summary>
        public static Curriculum Read(Dictionary<string, string> csvs)
        {
            return ContentCsvs.Read(csvs);
        }

        /// <summary>The id of the voice line <paramref name="id"/>; called by the voice tests.</summary>
        public static VoiceLineId Line(string id)
        {
            return new VoiceLineId(ContentId.Create(id));
        }

        /// <summary>
        /// Lets <paramref name="router"/> say everything planned: each clip and text ends at once, and
        /// each pause and bubble time passes. Called by the voice tests.
        /// </summary>
        public static void Drain(VoiceRouter router, FakeClipVoice clips, FakeSystemVoice system)
        {
            for (int turn = 0; turn < 100 && router.IsSpeaking; turn++)
            {
                clips.FinishPlaying();
                system.FinishSaying();
                router.Tick(1f);
            }
        }
    }
}
