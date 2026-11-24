using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Scolita.Editor.Voice;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Voice
{
    /// <summary>
    /// Tests <see cref="RecordingSessionImport.Cut"/> (with <see cref="TakeSplitter"/>,
    /// <see cref="TakeMatcher"/> and <see cref="ClipShaper"/>) on takes made in code: the level test
    /// and ten lines as tones of different lengths, two seconds apart (the brief's acceptance).
    /// </summary>
    public sealed class RecordingSessionImportTests
    {
        /// <summary>Tests that <see cref="RecordingSessionImport.Cut"/> makes ten trimmed, levelled clips from a good ten-line take.</summary>
        [Test]
        public void TestCut_TenLines()
        {
            RecordingSession session = Session();
            MonoAudio take = TestSignals.Take(session.Items.Select(item => (double)item.ExpectedSeconds));

            IReadOnlyList<ImportedClip> clips = RecordingSessionImport.Cut(session, take, out string summary);
            ImportedClip[] lines = clips.Where(clip => !clip.Item.IsLevelTest).ToArray();

            Assert.That(summary, Does.Contain("11 pieces for 11 lines"));
            Assert.That(lines.Length, Is.EqualTo(10));
            foreach (ImportedClip clip in lines)
            {
                Assert.That(clip.Flags, Is.Empty, clip.Item.Clip + ": " + clip.Measurement);
                Assert.That(clip.Measurement.Lufs, Is.EqualTo(VoiceClipStandard.TargetLufs).Within(VoiceClipStandard.LufsTolerance));
                Assert.That(clip.Measurement.LeadingSilence, Is.EqualTo(VoiceClipStandard.TargetSilenceSeconds).Within(0.03));
                Assert.That(clip.Audio.Seconds, Is.EqualTo(clip.Item.ExpectedSeconds + 0.2).Within(0.05));
            }
        }

        /// <summary>Tests that <see cref="RecordingSessionImport.Cut"/> flags a take with a line missed, and that no clip is silently given to the wrong line.</summary>
        [Test]
        public void TestCut_ALineMissed()
        {
            RecordingSession session = Session();
            const int missed = 6;
            MonoAudio take = TestSignals.Take(session.Items
                .Where(item => item.Number != missed)
                .Select(item => (double)item.ExpectedSeconds));

            IReadOnlyList<ImportedClip> clips = RecordingSessionImport.Cut(session, take, out string summary);

            Assert.That(summary, Does.Contain("10 pieces for 11 lines"));
            Assert.That(clips[missed].Flags, Does.Contain(ClipFlag.Missed));
            Assert.That(clips[missed].Audio, Is.Null);
            foreach (ImportedClip clip in clips.Where(clip => clip.Audio != null && clip.Flags.Count == 0))
            {
                Assert.That(clip.Audio.Seconds, Is.EqualTo(clip.Item.ExpectedSeconds + 0.2).Within(0.05),
                    "item " + clip.Item.Number + " is unflagged but got another line's piece");
            }
        }

        private static RecordingSession Session()
        {
            float[] seconds = { 2.5f, 0.7f, 0.5f, 1.2f, 0.9f, 1.6f, 0.4f, 2.2f, 1.0f, 0.6f, 1.8f };
            IEnumerable<RecordingItem> items = seconds.Select((length, number) => new RecordingItem(number,
                number == 0 ? string.Empty : "ro-word-test" + number, 0, 1,
                number == 0 ? string.Empty : "ro-word-test" + number + ".ogg", "line", "test " + number, string.Empty, length));
            return new RecordingSession("ro-planet-1-1", "ro", "planet-1", items);
        }
    }
}
