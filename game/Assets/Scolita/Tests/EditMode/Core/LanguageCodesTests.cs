using NUnit.Framework;
using Scolita.Core.Languages;

namespace Scolita.Tests.EditMode.Core
{
    /// <summary>Tests <see cref="LanguageCodes"/>, the stable codes in save files and content.</summary>
    public sealed class LanguageCodesTests
    {
        /// <summary>Tests that <see cref="LanguageCodes.CodeOf(Language)"/> gives ro and en, and reads back.</summary>
        [TestCase(Language.Romanian, "ro")]
        [TestCase(Language.English, "en")]
        public void TestCodeOf_LanguageRoundTrips(Language language, string code)
        {
            bool read = LanguageCodes.TryParseLanguage(code, out Language parsed);

            Assert.That(LanguageCodes.CodeOf(language), Is.EqualTo(code));
            Assert.That(read, Is.True);
            Assert.That(parsed, Is.EqualTo(language));
        }

        /// <summary>Tests that <see cref="LanguageCodes.CodeOf(LanguageMix)"/> gives both, ro and en, and reads back.</summary>
        [TestCase(LanguageMix.Both, "both")]
        [TestCase(LanguageMix.RomanianOnly, "ro")]
        [TestCase(LanguageMix.EnglishOnly, "en")]
        public void TestCodeOf_LanguageMixRoundTrips(LanguageMix mix, string code)
        {
            bool read = LanguageCodes.TryParseMix(code, out LanguageMix parsed);

            Assert.That(LanguageCodes.CodeOf(mix), Is.EqualTo(code));
            Assert.That(read, Is.True);
            Assert.That(parsed, Is.EqualTo(mix));
        }

        /// <summary>Tests that <see cref="LanguageCodes.TryParseLanguage"/> and <see cref="LanguageCodes.TryParseMix"/> never guess an unknown code.</summary>
        [TestCase(null)]
        [TestCase("")]
        [TestCase("RO")]
        [TestCase("ro-RO")]
        [TestCase("fr")]
        public void TestTryParseLanguage_UnknownCodeFails(string code)
        {
            Assert.That(LanguageCodes.TryParseLanguage(code, out Language _), Is.False);
            Assert.That(LanguageCodes.TryParseMix(code, out LanguageMix _), Is.False);
        }
    }
}
