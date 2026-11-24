using System.Collections.Generic;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Content
{
    /// <summary>Tests <see cref="VoiceLineStatuses"/> and <see cref="VoiceClipNames"/>.</summary>
    public sealed class VoiceLineStatusesTests
    {
        /// <summary>
        /// Tests that <see cref="VoiceLineStatuses.Of"/> finds a line recorded when its clips are there,
        /// missing when one isn't, said from its sounds for a word's slow line, and to record otherwise.
        /// </summary>
        [Test]
        public void TestOf_EachStatus()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            ContentCsvs.ReplaceRow(csvs, ContentFileNames.VoiceLines, "ro-sound-m,", "ro-sound-m,ro,sound,/m/,ro-sound-m.ogg,,,");
            ContentCsvs.ReplaceRow(csvs, ContentFileNames.VoiceLines, "ro-sound-a,", "ro-sound-a,ro,sound,/a/,ro-sound-a.ogg,,,");
            ContentCsvs.AddRow(csvs, ContentFileNames.VoiceLines, "ro-slow-mama,ro,slow,m-a-m-a,,,,");
            ContentCsvs.AddRow(csvs, ContentFileNames.VoiceLines, "ro-slow-tata,ro,slow,t-a-t-a,,,,");
            Curriculum curriculum = ContentCsvs.Read(csvs);

            IReadOnlyDictionary<ContentId, VoiceLineStatus> statuses =
                VoiceLineStatuses.Of(curriculum, new FakeContentFiles("ro-sound-m.ogg"));

            Assert.That(statuses[ContentId.Create("ro-sound-m")], Is.EqualTo(VoiceLineStatus.Recorded));
            Assert.That(statuses[ContentId.Create("ro-sound-a")], Is.EqualTo(VoiceLineStatus.Missing));
            Assert.That(statuses[ContentId.Create("ro-slow-mama")], Is.EqualTo(VoiceLineStatus.FromSounds));
            Assert.That(statuses[ContentId.Create("ro-slow-tata")], Is.EqualTo(VoiceLineStatus.ToRecord));
            Assert.That(statuses[ContentId.Create("ro-name-m")], Is.EqualTo(VoiceLineStatus.ToRecord));
        }

        /// <summary>Tests that <see cref="VoiceClipNames.For"/> names a line's one clip by its id, and several by their place.</summary>
        [Test]
        public void TestFor_OneClipOrSeveral()
        {
            VoiceLineId sound = new VoiceLineId(ContentId.Create("ro-sound-m"));
            VoiceLineId join = new VoiceLineId(ContentId.Create("ro-line-word-join"));

            Assert.That(VoiceClipNames.For(sound, 0, 1), Is.EqualTo("ro-sound-m.ogg"));
            Assert.That(VoiceClipNames.For(join, 0, 3), Is.EqualTo("ro-line-word-join.1.ogg"));
            Assert.That(VoiceClipNames.For(join, 2, 3), Is.EqualTo("ro-line-word-join.3.ogg"));
        }
    }
}
