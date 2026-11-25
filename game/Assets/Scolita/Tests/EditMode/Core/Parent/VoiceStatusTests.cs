using System.Collections.Generic;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Parent;

namespace Scolita.Tests.EditMode.Core.Parent
{
    /// <summary>Tests <see cref="VoiceStatus"/>.</summary>
    public sealed class VoiceStatusTests
    {
        /// <summary>Tests that <see cref="VoiceStatus.Of"/> counts the recorded lines of each language out of those to record.</summary>
        [Test]
        public void TestOf_RecordedOutOfToRecord()
        {
            ItemCatalog<VoiceLine> lines = new ItemCatalog<VoiceLine>(new List<VoiceLine>
            {
                Line("ro-line-a", Language.Romanian, VoiceLineStatus.Recorded, false),
                Line("ro-line-b", Language.Romanian, VoiceLineStatus.ToRecord, false),
                Line("ro-line-c", Language.Romanian, VoiceLineStatus.Missing, false),
                Line("ro-line-d", Language.Romanian, VoiceLineStatus.FromSounds, false),
                Line("ro-line-e", Language.Romanian, VoiceLineStatus.Recorded, true),
                Line("en-line-a", Language.English, VoiceLineStatus.Recorded, false),
                Line("en-line-b", Language.English, VoiceLineStatus.Recorded, false)
            });

            IReadOnlyList<LanguageVoiceStatus> status = VoiceStatus.Of(lines);

            Assert.That(status[0].Language, Is.EqualTo(Language.Romanian));
            Assert.That(status[0].Recorded, Is.EqualTo(1));
            Assert.That(status[0].ToRecord, Is.EqualTo(3));
            Assert.That(status[1].Language, Is.EqualTo(Language.English));
            Assert.That(status[1].Recorded, Is.EqualTo(2));
            Assert.That(status[1].ToRecord, Is.EqualTo(2));
        }

        private static VoiceLine Line(string id, Language language, VoiceLineStatus status, bool retired)
        {
            string clip = status == VoiceLineStatus.Recorded ? id + ".ogg" : string.Empty;
            return new VoiceLine(ContentId.Create(id), language, VoiceLineKind.Line, "Text.", clip, status, retired,
                new ContentSource("voice-lines.csv", 2));
        }
    }
}
