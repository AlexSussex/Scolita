using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Languages;

namespace Scolita.Tests.EditMode.Core.Content
{
    /// <summary>Tests <see cref="ContentId"/> and <see cref="VoiceLineIds"/>, the content's stable ids.</summary>
    public sealed class ContentIdTests
    {
        /// <summary>Tests that <see cref="ContentId.TryCreate"/> accepts <c>[a-z0-9-]+</c>.</summary>
        [TestCase("a")]
        [TestCase("ro-mama")]
        [TestCase("planet-1")]
        public void TestTryCreate_AcceptsStableIds(string text)
        {
            Assert.That(ContentId.TryCreate(text, out ContentId id), Is.True);
            Assert.That(id.Value, Is.EqualTo(text));
            Assert.That(id.IsSet, Is.True);
        }

        /// <summary>Tests that <see cref="ContentId.TryCreate"/> refuses capitals, spaces, diacritics and empty text.</summary>
        [TestCase(null)]
        [TestCase("")]
        [TestCase("Mama")]
        [TestCase("ro mama")]
        [TestCase("ro_mama")]
        [TestCase("\u0219")]
        public void TestTryCreate_RefusesOtherText(string text)
        {
            Assert.That(ContentId.TryCreate(text, out ContentId id), Is.False);
            Assert.That(id.IsSet, Is.False);
            Assert.That(() => ContentId.Create(text), Throws.ArgumentException);
        }

        /// <summary>Tests that the default <see cref="ContentId"/> is not set and has no <see cref="ContentId.Value"/>.</summary>
        [Test]
        public void TestValue_DefaultIsNotSet()
        {
            ContentId none = default;

            Assert.That(none.IsSet, Is.False);
            Assert.That(none.ToString(), Is.Empty);
            Assert.That(() => none.Value, Throws.InvalidOperationException);
        }

        /// <summary>Tests that <see cref="VoiceLineIds.For"/> names a line language, kind, key.</summary>
        [Test]
        public void TestFor_NamesLanguageKindKey()
        {
            Assert.That(VoiceLineIds.For(Language.Romanian, VoiceLineKind.Sound, "m").ToString(), Is.EqualTo("ro-sound-m"));
            Assert.That(VoiceLineIds.For(Language.English, VoiceLineKind.Line, "try-again").ToString(),
                Is.EqualTo("en-line-try-again"));
        }

        /// <summary>Tests that <see cref="VoiceLineIds.TryParse"/> reads a line's language and kind, and refuses other ids.</summary>
        [Test]
        public void TestTryParse_ReadsLanguageAndKind()
        {
            bool parsed = VoiceLineIds.TryParse(ContentId.Create("en-syllables-piano"), out Language language,
                out VoiceLineKind kind);

            Assert.That(parsed, Is.True);
            Assert.That(language, Is.EqualTo(Language.English));
            Assert.That(kind, Is.EqualTo(VoiceLineKind.Syllables));
            Assert.That(VoiceLineIds.TryParse(ContentId.Create("fr-word-chat"), out _, out _), Is.False);
            Assert.That(VoiceLineIds.TryParse(ContentId.Create("ro-noise-m"), out _, out _), Is.False);
            Assert.That(VoiceLineIds.TryParse(ContentId.Create("ro-word"), out _, out _), Is.False);
        }

        /// <summary>Tests that <see cref="LanguageMixExtensions.Includes"/> and <see cref="LanguageMixExtensions.Languages"/> agree.</summary>
        [Test]
        public void TestIncludes_MatchesLanguages()
        {
            Assert.That(LanguageMix.Both.Includes(Language.English), Is.True);
            Assert.That(LanguageMix.RomanianOnly.Includes(Language.English), Is.False);
            Assert.That(LanguageMix.Both.Languages(), Is.EqualTo(new[] { Language.Romanian, Language.English }));
            Assert.That(LanguageMix.EnglishOnly.Languages(), Is.EqualTo(new[] { Language.English }));
            Assert.That(Language.Romanian.Alone(), Is.EqualTo(LanguageMix.RomanianOnly));
        }
    }
}
