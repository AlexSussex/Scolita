using System.Linq;
using NUnit.Framework;
using Scolita.Core.Voice;

namespace Scolita.Tests.EditMode.Core.Voice
{
    /// <summary>Tests <see cref="VoiceTemplate"/>: splitting a voice line's text at its slots.</summary>
    public sealed class VoiceTemplateTests
    {
        /// <summary>Tests that <see cref="VoiceTemplate.Parse"/> keeps a line without slots as one spoken segment.</summary>
        [Test]
        public void TestParse_NoSlots()
        {
            VoiceTemplate template = VoiceTemplate.Parse("Mai \u00EEncearc\u0103 o dat\u0103!");

            Assert.That(template.HasSlots, Is.False);
            Assert.That(template.SpokenSegments, Is.EqualTo(1));
            Assert.That(template.Pieces.Single().Text, Is.EqualTo("Mai \u00EEncearc\u0103 o dat\u0103!"));
        }

        /// <summary>Tests that <see cref="VoiceTemplate.Parse"/> splits "Atinge litera {x}." into a spoken segment, the slot, and punctuation that isn't said.</summary>
        [Test]
        public void TestParse_SlotAtTheEnd()
        {
            VoiceTemplate template = VoiceTemplate.Parse("Atinge litera {x}.");

            Assert.That(template.HasSlots, Is.True);
            Assert.That(template.SpokenSegments, Is.EqualTo(1));
            Assert.That(template.Pieces.Select(piece => piece.IsSlot ? "{" + piece.Slot + "}" : piece.Text + "|" + piece.SpokenIndex),
                Is.EqualTo(new[] { "Atinge litera |0", "{X}", ".|-1" }));
        }

        /// <summary>Tests that <see cref="VoiceTemplate.Parse"/> counts each spoken segment between slots.</summary>
        [Test]
        public void TestParse_ThreeSpokenSegments()
        {
            VoiceTemplate template = VoiceTemplate.Parse(
                "Bravo, ai f\u0103cut litera {x}! Acum leag-o de litera {y}, f\u0103r\u0103 s\u0103 ridici degetul.");

            Assert.That(template.SpokenSegments, Is.EqualTo(3));
            Assert.That(template.Pieces.Where(piece => piece.IsSlot).Select(piece => piece.Slot),
                Is.EqualTo(new[] { VoiceSlot.X, VoiceSlot.Y }));
        }

        /// <summary>Tests that <see cref="VoiceTemplate.Parse"/> marks an ellipsis between slots as a pause.</summary>
        [Test]
        public void TestParse_EllipsisIsAPause()
        {
            VoiceTemplate template = VoiceTemplate.Parse("Ascult\u0103: {x}\u2026 {x}\u2026 {word}!");

            Assert.That(template.SpokenSegments, Is.EqualTo(1));
            Assert.That(template.Pieces.Count(piece => piece.IsPause), Is.EqualTo(2));
            Assert.That(template.Pieces.Last().IsPause, Is.False);
        }

        /// <summary>Tests that <see cref="VoiceTemplate.Parse"/> lists a name in braces that isn't a slot, keeping it as text.</summary>
        [Test]
        public void TestParse_UnknownSlot()
        {
            VoiceTemplate template = VoiceTemplate.Parse("Atinge {litera} {x}.");

            Assert.That(template.UnknownSlots, Is.EqualTo(new[] { "litera" }));
            Assert.That(template.Pieces.First().Text, Is.EqualTo("Atinge {litera} "));
        }
    }
}
