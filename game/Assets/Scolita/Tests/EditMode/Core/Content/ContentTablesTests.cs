using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Content
{
    /// <summary>
    /// Tests <see cref="ContentTables"/> and <see cref="CurriculumBuilder"/>: every problem reading the
    /// CSVs names its file, its row and the reason.
    /// </summary>
    public sealed class ContentTablesTests
    {
        /// <summary>Tests that <see cref="ContentTables.Read"/> finds no error in the valid set.</summary>
        [Test]
        public void TestRead_ValidSetHasNoError()
        {
            ContentProblem[] errors = ContentCsvs.Errors(ContentCsvs.Valid());

            Assert.That(errors.Select(error => error.ToString()), Is.Empty);
        }

        /// <summary>Tests that <see cref="ContentTables.Read"/> reports a missing file.</summary>
        [Test]
        public void TestRead_MissingFile()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            csvs.Remove(ContentFileNames.Pictures);

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Pictures, 0, "missing");
        }

        /// <summary>Tests that <see cref="ContentTables.Read"/> reports a missing and an unknown column on the header row.</summary>
        [Test]
        public void TestRead_MissingAndUnknownColumns()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            csvs[ContentFileNames.Units] = "id,domian,retired,notes\nplanet-1,reading,,\n";

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Units, 1, "no column domain");
            ContentCsvs.AssertError(errors, ContentFileNames.Units, 1, "unknown column domian");
        }

        /// <summary>Tests that <see cref="ContentTables.Read"/> reports a cedilla in a cell, naming the column and both letters.</summary>
        [Test]
        public void TestRead_Cedilla()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int row = ContentCsvs.ReplaceRow(csvs, ContentFileNames.VoiceLines, "ro-line-trace-start,",
                "ro-line-trace-start,ro,line,\u00CEncepe de la punct \u015Fi urmeaz\u0103 s\u0103geata.,,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.VoiceLines, row, "cell text", "U+015F", "U+0219");
        }

        /// <summary>Tests that <see cref="ContentTables.Read"/> reports an id that isn't <c>[a-z0-9-]+</c>.</summary>
        [Test]
        public void TestRead_MalformedId()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int row = ContentCsvs.AddRow(csvs, ContentFileNames.Pictures, "Big Moon,,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Pictures, row, "'Big Moon'", "not an id");
        }

        /// <summary>Tests that <see cref="CurriculumBuilder.Build"/> reports a repeated id at its second row, naming the first.</summary>
        [Test]
        public void TestBuild_DuplicateId()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int row = ContentCsvs.AddRow(csvs, ContentFileNames.Words, "ro-mama,ro,mama,m a m a,ma-ma,mother,,,,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Words, row, "ro-mama is already used", "words.csv, row 2",
                "never reused");
        }

        /// <summary>Tests that <see cref="ContentTables.Read"/> reports unknown codes: a group, a kind, a language, a yes/no.</summary>
        [Test]
        public void TestRead_UnknownCodes()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int letterRow = ContentCsvs.AddRow(csvs, ContentFileNames.Letters, "n,n,N,shared,,,");
            int stepRow = ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "ro-a,Hunt,,a,,,");
            int wordRow = ContentCsvs.AddRow(csvs, ContentFileNames.Words, "fr-chat,fr,chat,,chat,mat,yes,,,,");
            int lineRow = ContentCsvs.AddRow(csvs, ContentFileNames.VoiceLines, "ro-word-x,ro,word,x,,,no,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Letters, letterRow, "group 'shared'");
            ContentCsvs.AssertError(errors, ContentFileNames.Steps, stepRow, "kind 'Hunt'", "MeetLetter");
            ContentCsvs.AssertError(errors, ContentFileNames.Words, wordRow, "language 'fr'");
            ContentCsvs.AssertError(errors, ContentFileNames.VoiceLines, lineRow, "retired says 'no'");
        }

        /// <summary>Tests that <see cref="CurriculumBuilder.Build"/> reports a lesson whose unit and a step whose lesson don't exist.</summary>
        [Test]
        public void TestBuild_UnknownParents()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int lessonRow = ContentCsvs.AddRow(csvs, ContentFileNames.Lessons, "ro-x,planet-9,ro,,,");
            int stepRow = ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "ro-y,LetterHunt,,a,,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Lessons, lessonRow, "planet-9", "not in units.csv");
            ContentCsvs.AssertError(errors, ContentFileNames.Steps, stepRow, "ro-y", "not in lessons.csv");
        }

        /// <summary>
        /// Tests that <see cref="CurriculumBuilder.Build"/> reports a step whose language doesn't fit:
        /// none given in a lesson in both languages, another than its lesson's, or one language for a
        /// bridge kind.
        /// </summary>
        [Test]
        public void TestBuild_StepLanguages()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int noLanguage = ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "party,LetterHunt,,a,,,");
            int otherLanguage = ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "ro-a,LetterHunt,en,a,,,");
            int bridgeInOne = ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "party,WordTwins,ro,,ro-mama en-mat,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Steps, noLanguage, "mixes both", "write ro or en");
            ContentCsvs.AssertError(errors, ContentFileNames.Steps, otherLanguage, "not its lesson's");
            ContentCsvs.AssertError(errors, ContentFileNames.Steps, bridgeInOne, "WordTwins step is in both languages");
        }

        /// <summary>Tests that <see cref="ContentTables.Read"/> reports an option that isn't key=value and an empty syllable.</summary>
        [Test]
        public void TestRead_BadOptionAndSyllables()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int stepRow = ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "ro-a,TraceLetter,,a,,form,");
            int wordRow = ContentCsvs.AddRow(csvs, ContentFileNames.Words, "ro-tata,ro,tata,t a t a,ta--ta,,,,,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Steps, stepRow, "'form' is not key=value");
            ContentCsvs.AssertError(errors, ContentFileNames.Words, wordRow, "empty syllable");
        }
    }
}
