using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Profiles;
using Scolita.Core.Timing;
using Scolita.Core.Voice;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Profiles
{
    /// <summary>Tests <see cref="ProfileLines"/>, what the mascot says on "Who's playing?", on the real content.</summary>
    public sealed class ProfileLinesTests
    {
        /// <summary>Tests that <see cref="ProfileLines.Hello"/> splices a child's name, as typed, into the greeting.</summary>
        [TestCase("Ana")]
        [TestCase("Matei")]
        [TestCase("Ștefan")]
        [TestCase("Ioana-Maria")]
        public void TestHello_TheNameAsTyped(string typed)
        {
            ProfileName.Read(typed, TestProfileRules.Default(), out ProfileName name);
            Profile child = new Profile(ProfileList.FirstChildId, name, ContentId.Create("cat"), ProfileKind.Child,
                CalendarDay.FromDate(2026, 11, 25));

            VoiceSequence hello = ProfileLines.Hello(child, Language.Romanian);

            Assert.That(SequenceLines.Of(hello), Is.EqualTo(new[] { "ro-line-hello-name" }));
            Assert.That(hello.Items[0].TryGetFill(VoiceSlot.Name, out VoiceFill fill), Is.True);
            Assert.That(fill.HasLine, Is.False);
            Assert.That(fill.Text, Is.EqualTo(typed));
        }

        /// <summary>Tests that <see cref="ProfileLines.Hello"/> of a profile without a name is the greeting without one.</summary>
        [Test]
        public void TestHello_NoNameNoSlot()
        {
            Profile child = new Profile(ProfileList.FirstChildId, ProfileName.None, ContentId.Create("cat"), ProfileKind.Child,
                CalendarDay.FromDate(2026, 11, 25));

            VoiceSequence hello = ProfileLines.Hello(child, Language.English);

            Assert.That(SequenceLines.Of(hello), Is.EqualTo(new[] { "en-line-hello" }));
            Assert.That(hello.Items[0].TryGetFill(VoiceSlot.Name, out VoiceFill _), Is.False);
        }

        /// <summary>Tests that <see cref="ProfileLines.LanguageFor"/> greets in the only language she learns, else the app's.</summary>
        [Test]
        public void TestLanguageFor_HerOnlyLanguageElseTheApps()
        {
            Assert.That(ProfileLines.LanguageFor(LanguageMix.EnglishOnly, Language.Romanian), Is.EqualTo(Language.English));
            Assert.That(ProfileLines.LanguageFor(LanguageMix.RomanianOnly, Language.English), Is.EqualTo(Language.Romanian));
            Assert.That(ProfileLines.LanguageFor(LanguageMix.Both, Language.English), Is.EqualTo(Language.English));
        }

        /// <summary>
        /// Tests that every line of <see cref="ProfileLines.LineIds"/> is in the content in both languages, and that the
        /// greeting with a name puts it only after „Bună,” / "Hi," (the nominative), with nothing that agrees with it.
        /// </summary>
        [Test]
        public void TestLineIds_InTheContentWithTheNameInTheNominative()
        {
            Curriculum curriculum = RealContent.Read();

            VoiceLineId[] all = ProfileLines.LineIds(Language.Romanian).Concat(ProfileLines.LineIds(Language.English)).ToArray();

            Assert.That(all.Where(line => !curriculum.VoiceLines.Contains(line.Id)).Select(line => line.ToString()), Is.Empty);
            Assert.That(curriculum.VoiceLines.Get(ContentId.Create("ro-line-hello-name")).Text,
                Is.EqualTo("Bună, {name}! Hai să zburăm!"));
            Assert.That(curriculum.VoiceLines.Get(ContentId.Create("en-line-hello-name")).Text, Is.EqualTo("Hi, {name}! Let's fly!"));
            Assert.That(curriculum.VoiceLines.Get(ContentId.Create("ro-line-who-plays")).Text,
                Is.EqualTo("Cine se joacă? Atinge poza ta."));
        }
    }
}
