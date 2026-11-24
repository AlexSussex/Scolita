using NUnit.Framework;
using Scolita.Core.Activities;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Voice;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Voice
{
    /// <summary>Tests <see cref="VoiceSequenceText"/> on the real content's lines.</summary>
    public sealed class VoiceSequenceTextTests
    {
        /// <summary>
        /// Tests that <see cref="VoiceSequenceText.Of"/> joins the lines' words with their slots filled:
        /// <c>MeetLetter</c>'s instruction for a, with avion.
        /// </summary>
        [Test]
        public void TestOf_FillsTheSlots()
        {
            Curriculum curriculum = RealContent.Read();
            IActivityRules rules = ActivityRulesFactory.Create(RealContent.FirstStep(curriculum, "ro-a", ActivityKind.MeetLetter), curriculum);

            string text = VoiceSequenceText.Of(curriculum, rules.Instruction);

            Assert.That(text, Is.EqualTo("Hai s\u0103 cunoa\u0219tem o liter\u0103 nou\u0103! Ascult\u0103: a\u2026 a\u2026 avion! "
                + "Atinge litera ca s-o auzi din nou."));
            Assert.That(VoiceSequenceText.TryGetLanguage(curriculum, rules.Instruction, out Language language), Is.True);
            Assert.That(language, Is.EqualTo(Language.Romanian));
        }

        /// <summary>Tests that <see cref="VoiceSequenceText.Of"/> shows a letter's sound as the letter, held, and the English lines in English.</summary>
        [Test]
        public void TestOf_ASoundIsShownAsItsLetter()
        {
            Curriculum curriculum = RealContent.Read();
            VoiceSequence hint = VoiceSequence.Of(new VoiceLineId(ContentId.Create("en-line-listen-again")))
                .Then(new VoiceLineId(ContentId.Create("en-sound-t")));

            Assert.That(VoiceSequenceText.Of(curriculum, hint), Is.EqualTo("Listen again. t\u2026"));
            Assert.That(VoiceSequenceText.TryGetLanguage(curriculum, hint, out Language language), Is.True);
            Assert.That(language, Is.EqualTo(Language.English));
        }
    }
}
