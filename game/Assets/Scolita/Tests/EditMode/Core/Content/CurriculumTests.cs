using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Content
{
    /// <summary>Tests <see cref="Curriculum"/>: its tracks, and which letters are taught before a lesson.</summary>
    public sealed class CurriculumTests
    {
        /// <summary>
        /// Tests that <see cref="Curriculum.LettersTaughtBefore"/> counts only the letters introduced
        /// by earlier lessons of the asked language's track, never the lesson's own.
        /// </summary>
        [TestCase("ro-a", "ro", "")]
        [TestCase("ro-m", "ro", "a")]
        [TestCase("en-a", "ro", "a m")]
        [TestCase("en-a", "en", "")]
        [TestCase("en-t", "en", "a m")]
        [TestCase("party", "ro", "a m")]
        [TestCase("party", "en", "a m t")]
        public void TestLettersTaughtBefore_CountsEarlierLessonsOfTheTrack(string lessonId, string languageCode,
            string expected)
        {
            Curriculum curriculum = ContentCsvs.Read(ContentCsvs.Valid());
            LanguageCodes.TryParseLanguage(languageCode, out Language language);
            Assert.That(curriculum.TryGetLesson(ContentId.Create(lessonId), out Lesson lesson), Is.True);

            IReadOnlyList<ContentId> taught = curriculum.LettersTaughtBefore(lesson, language);

            Assert.That(string.Join(" ", taught), Is.EqualTo(expected));
        }

        /// <summary>Tests that <see cref="Curriculum.LettersTaughtBefore"/> skips a retired lesson's letter.</summary>
        [Test]
        public void TestLettersTaughtBefore_SkipsRetiredLessons()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            ContentCsvs.ReplaceRow(csvs, ContentFileNames.Lessons, "en-m,", "en-m,planet-1,en,m,yes,");
            Curriculum curriculum = ContentCsvs.Read(csvs);
            curriculum.TryGetLesson(ContentId.Create("en-t"), out Lesson lesson);

            IReadOnlyList<ContentId> taught = curriculum.LettersTaughtBefore(lesson, Language.English);

            Assert.That(string.Join(" ", taught), Is.EqualTo("a"));
        }

        /// <summary>
        /// Tests that <see cref="Curriculum.Track"/> holds the domain's lessons in that language and the
        /// lessons in both, in teaching order, with the letters they introduce.
        /// </summary>
        [Test]
        public void TestTrack_HoldsTheLanguagesLessonsAndTheMixedOnes()
        {
            Curriculum curriculum = ContentCsvs.Read(ContentCsvs.Valid());

            Track romanian = curriculum.Track(ContentId.Create("reading"), Language.Romanian);
            Track english = curriculum.Track(ContentId.Create("reading"), Language.English);

            Assert.That(romanian.Lessons.Select(lesson => lesson.Id.Value), Is.EqualTo(new[] { "ro-a", "ro-m", "party" }));
            Assert.That(string.Join(" ", romanian.Letters), Is.EqualTo("a m"));
            Assert.That(english.Lessons.Select(lesson => lesson.Id.Value), Is.EqualTo(new[] { "en-a", "en-m", "en-t", "party" }));
            Assert.That(string.Join(" ", english.Letters), Is.EqualTo("a m t"));
        }

        /// <summary>
        /// Tests that the <see cref="Curriculum"/> built by <see cref="CurriculumBuilder"/> keeps the tree
        /// (domain, unit, lessons and numbered steps in order) and settles each step's languages.
        /// </summary>
        [Test]
        public void TestBuild_KeepsTheTreeInOrder()
        {
            Curriculum curriculum = ContentCsvs.Read(ContentCsvs.Valid());

            Domain domain = curriculum.Domains.Single();
            Unit unit = domain.Units.Single();
            Lesson party = unit.Lessons.Last();

            Assert.That(domain.Id.Value, Is.EqualTo("reading"));
            Assert.That(unit.Lessons.Select(lesson => lesson.Id.Value),
                Is.EqualTo(new[] { "ro-a", "ro-m", "en-a", "en-m", "en-t", "party" }));
            Assert.That(party.Languages, Is.EqualTo(LanguageMix.Both));
            Assert.That(party.Steps.Select(step => step.Number), Is.EqualTo(new[] { 1, 2 }));
            Assert.That(party.Steps[0].Languages, Is.EqualTo(LanguageMix.RomanianOnly));
            Assert.That(party.Steps[1].Languages, Is.EqualTo(LanguageMix.Both));
            Assert.That(unit.Lessons[0].Steps[1].TryGetOption("form", out string form), Is.True);
            Assert.That(form, Is.EqualTo("print"));
        }

        /// <summary>Tests that a <see cref="Word"/>'s and a <see cref="Letter"/>'s voice lines follow the naming rule.</summary>
        [Test]
        public void TestLines_FollowTheNamingRule()
        {
            Curriculum curriculum = ContentCsvs.Read(ContentCsvs.Valid());

            Word mama = curriculum.Words.Get(ContentId.Create("ro-mama"));
            Letter t = curriculum.Letters.Get(ContentId.Create("t"));

            Assert.That(mama.Key, Is.EqualTo("mama"));
            Assert.That(mama.WordLine.ToString(), Is.EqualTo("ro-word-mama"));
            Assert.That(mama.SyllablesLine.ToString(), Is.EqualTo("ro-syllables-mama"));
            Assert.That(mama.SlowLine.ToString(), Is.EqualTo("ro-slow-mama"));
            Assert.That(t.SoundLine(Language.English).ToString(), Is.EqualTo("en-sound-t"));
            Assert.That(t.NameLine(Language.Romanian).ToString(), Is.EqualTo("ro-name-t"));
            Assert.That(t.Languages, Is.EqualTo(LanguageMix.Both));
        }
    }
}
