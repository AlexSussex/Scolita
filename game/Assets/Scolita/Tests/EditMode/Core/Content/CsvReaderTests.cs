using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Content.Csv;

namespace Scolita.Tests.EditMode.Core.Content
{
    /// <summary>Tests <see cref="CsvReader"/>, which reads the content CSVs as spreadsheets write them.</summary>
    public sealed class CsvReaderTests
    {
        /// <summary>Tests that <see cref="CsvReader.Read"/> reads a header and rows, with their spreadsheet row numbers.</summary>
        [Test]
        public void TestRead_HeaderAndRows()
        {
            ContentProblems problems = new ContentProblems();

            CsvTable table = CsvReader.Read("words.csv", "id,text\nro-mama,mama\nro-tata,tata\n", problems);

            Assert.That(problems.All, Is.Empty);
            Assert.That(table.Columns, Is.EqualTo(new[] { "id", "text" }));
            Assert.That(table.Rows.Count, Is.EqualTo(2));
            Assert.That(table.Rows[0].Number, Is.EqualTo(2));
            Assert.That(table.Cell(table.Rows[1], "text"), Is.EqualTo("tata"));
            Assert.That(table.SourceOf(table.Rows[1]).ToString(), Is.EqualTo("words.csv, row 3"));
        }

        /// <summary>Tests that <see cref="CsvReader.Read"/> keeps commas, doubled quotes and line breaks inside a quoted field.</summary>
        [Test]
        public void TestRead_QuotedFields()
        {
            ContentProblems problems = new ContentProblems();
            string text = "id,text\nl1,\"Bravo, ai reu\u0219it!\"\nl2,\"Say \"\"hi\"\"\"\nl3,\"two\nlines\"\nl4,last\n";

            CsvTable table = CsvReader.Read("voice-lines.csv", text, problems);

            Assert.That(problems.All, Is.Empty);
            Assert.That(table.Cell(table.Rows[0], "text"), Is.EqualTo("Bravo, ai reu\u0219it!"));
            Assert.That(table.Cell(table.Rows[1], "text"), Is.EqualTo("Say \"hi\""));
            Assert.That(table.Cell(table.Rows[2], "text"), Is.EqualTo("two\nlines"));
            Assert.That(table.Rows[3].Number, Is.EqualTo(5));
        }

        /// <summary>Tests that <see cref="CsvReader.Read"/> reads the same with and without a byte order mark.</summary>
        [Test]
        public void TestRead_WithAndWithoutByteOrderMark()
        {
            ContentProblems problems = new ContentProblems();

            CsvTable withMark = CsvReader.Read("units.csv", "\uFEFFid,domain\nplanet-1,reading\n", problems);
            CsvTable withoutMark = CsvReader.Read("units.csv", "id,domain\nplanet-1,reading\n", problems);

            Assert.That(problems.All, Is.Empty);
            Assert.That(withMark.Columns, Is.EqualTo(withoutMark.Columns));
            Assert.That(withMark.Cell(withMark.Rows[0], "id"), Is.EqualTo("planet-1"));
        }

        /// <summary>
        /// Tests that <see cref="CsvReader.Read"/> reads Excel's "CSV UTF-8" as a Romanian Windows saves
        /// it: a byte order mark, CRLF line endings, <c>;</c> between fields, quotes around a field with a
        /// <c>;</c>, and empty trailing cells.
        /// </summary>
        [Test]
        public void TestRead_ExcelCsvUtf8InARomanianLocale()
        {
            ContentProblems problems = new ContentProblems();
            string text = "\uFEFFid;language;text;notes\r\nro-line-a;ro;\"Atinge; ascult\u0103\";\r\nro-line-b;ro;Bravo!;\r\n";

            CsvTable table = CsvReader.Read("voice-lines.csv", text, problems);

            Assert.That(problems.All, Is.Empty);
            Assert.That(table.Columns, Is.EqualTo(new[] { "id", "language", "text", "notes" }));
            Assert.That(table.Cell(table.Rows[0], "text"), Is.EqualTo("Atinge; ascult\u0103"));
            Assert.That(table.Cell(table.Rows[1], "notes"), Is.Empty);
            Assert.That(table.Rows[1].Number, Is.EqualTo(3));
        }

        /// <summary>Tests that <see cref="CsvReader.Read"/> leaves out blank rows but keeps the next rows' numbers.</summary>
        [Test]
        public void TestRead_BlankRowsLeftOutNumbersKept()
        {
            ContentProblems problems = new ContentProblems();

            CsvTable table = CsvReader.Read("units.csv", "id,domain\n\nplanet-1,reading\n,\n\n", problems);

            Assert.That(problems.All, Is.Empty);
            Assert.That(table.Rows.Count, Is.EqualTo(1));
            Assert.That(table.Rows[0].Number, Is.EqualTo(3));
        }

        /// <summary>Tests that <see cref="CsvReader.Read"/> reads a short row's missing cells as empty.</summary>
        [Test]
        public void TestRead_ShortRowReadsEmptyCells()
        {
            ContentProblems problems = new ContentProblems();

            CsvTable table = CsvReader.Read("units.csv", "id,domain,notes\nplanet-1,reading\n", problems);

            Assert.That(problems.All, Is.Empty);
            Assert.That(table.Cell(table.Rows[0], "notes"), Is.Empty);
        }

        /// <summary>Tests that <see cref="CsvReader.Read"/> reports and leaves out a row with more cells than the header.</summary>
        [Test]
        public void TestRead_LongRowIsReported()
        {
            ContentProblems problems = new ContentProblems();

            CsvTable table = CsvReader.Read("units.csv", "id,domain\nplanet-1,reading\nplanet-2,reading,extra\n", problems);

            Assert.That(table.Rows.Count, Is.EqualTo(1));
            Assert.That(problems.All.Single().ToString(), Does.StartWith("units.csv, row 3: the row has 3 cells"));
        }

        /// <summary>Tests that <see cref="CsvReader.Read"/> reports a quote that is never closed.</summary>
        [Test]
        public void TestRead_UnclosedQuoteIsReported()
        {
            ContentProblems problems = new ContentProblems();

            CsvReader.Read("voice-lines.csv", "id,text\nl1,ok\nl2,\"never closed\n", problems);

            Assert.That(problems.All.Single().ToString(), Does.Contain("row 3").And.Contain("never closed"));
        }

        /// <summary>Tests that <see cref="CsvReader.Read"/> reports an empty file and a repeated column.</summary>
        [Test]
        public void TestRead_EmptyFileAndRepeatedColumnAreReported()
        {
            ContentProblems empty = new ContentProblems();
            ContentProblems repeated = new ContentProblems();

            CsvReader.Read("units.csv", string.Empty, empty);
            CsvReader.Read("units.csv", "id,id\nx,y\n", repeated);

            Assert.That(empty.All.Single().ToString(), Is.EqualTo("units.csv: the file has no header row."));
            Assert.That(repeated.All.Single().ToString(), Does.Contain("row 1").And.Contain("appears twice"));
        }
    }
}
