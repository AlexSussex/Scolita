using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Editor.ContentImport;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Content
{
    /// <summary>Tests <see cref="ReleasedIdLedger"/>, the ledger of released ids in <c>content/released-ids.txt</c>.</summary>
    public sealed class ReleasedIdLedgerTests
    {
        /// <summary>Tests that <see cref="ReleasedIdLedger.Parse"/> reads each line's file, id and row, skipping comments and blank lines.</summary>
        [Test]
        public void TestParse_ReadsLinesAndSkipsComments()
        {
            ContentProblems problems = new ContentProblems();

            ReleasedIdLedger ledger = ReleasedIdLedger.Parse(
                "\uFEFF# the ledger\r\n\r\n# 0.2.0\r\nletters.csv a\r\nwords.csv  ro-mama \n", problems);

            Assert.That(problems.All, Is.Empty);
            Assert.That(ledger.Ids.Select(id => id.ToString()), Is.EqualTo(new[] { "letters.csv a", "words.csv ro-mama" }));
            Assert.That(ledger.Ids.Select(id => id.Row), Is.EqualTo(new[] { 4, 5 }));
        }

        /// <summary>
        /// Tests that <see cref="ReleasedIdLedger.Parse"/> refuses, at its row, a line without a file, an
        /// unknown file, an id that isn't a stable id, a line in three parts and a repeated line.
        /// </summary>
        [Test]
        public void TestParse_RefusesBadAndRepeatedLines()
        {
            ContentProblems problems = new ContentProblems();

            ReleasedIdLedger ledger = ReleasedIdLedger.Parse(
                "ro-mama\nsteps.csv ro-a\nwords.csv Ro-Mama\nwords.csv ro-mama extra\nletters.csv a\nletters.csv a\n", problems);

            ContentCsvs.AssertError(problems.All, ReleasedIdLedger.FileName, 1, "not a ledger line");
            ContentCsvs.AssertError(problems.All, ReleasedIdLedger.FileName, 2, "not a ledger line");
            ContentCsvs.AssertError(problems.All, ReleasedIdLedger.FileName, 3, "not a ledger line");
            ContentCsvs.AssertError(problems.All, ReleasedIdLedger.FileName, 4, "not a ledger line");
            ContentCsvs.AssertError(problems.All, ReleasedIdLedger.FileName, 6, "twice");
            Assert.That(problems.All.Count, Is.EqualTo(5));
            Assert.That(ledger.Ids.Select(id => id.ToString()), Is.EqualTo(new[] { "letters.csv a" }));
        }

        /// <summary>
        /// Tests that <see cref="ReleasedIdLedger.Parse"/> reads the highest of its <c># &lt;version&gt;</c>
        /// lines as the latest release (other comments aside), and none for a ledger without one.
        /// </summary>
        [Test]
        public void TestParse_ReadsTheLatestRelease()
        {
            ContentProblems problems = new ContentProblems();

            ReleasedIdLedger ledger = ReleasedIdLedger.Parse("# the ledger, 1.0.0 one day\n# 0.2.0\nletters.csv a\n"
                + "# 0.10.0\nletters.csv m\n# 0.3.0\n", problems);
            ReleasedIdLedger empty = ReleasedIdLedger.Parse("# the ledger\n", problems);

            Assert.That(problems.All, Is.Empty);
            Assert.That(ledger.LatestRelease.HasValue, Is.True);
            Assert.That(ledger.LatestRelease.Value.ToString(), Is.EqualTo("0.10.0"));
            Assert.That(empty.LatestRelease.HasValue, Is.False);
        }

        /// <summary>Tests that <see cref="ReleasedIdLedger.WithIds"/> ends a last line that has no line end before adding.</summary>
        [Test]
        public void TestWithIds_EndsAnUnfinishedLastLine()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            ReleasedIdLedger ledger = ReleasedIdLedger.Parse("# 0.2.0\nletters.csv a", new ContentProblems());

            string text = ledger.WithIds("0.3.1", new[] { new ReleasedId(ContentFileNames.Letters, "m", 0) });

            Assert.That(ledger.NewIds(ContentCsvs.Read(csvs)).First().ToString(), Is.EqualTo("letters.csv m"));
            Assert.That(text, Is.EqualTo("# 0.2.0\nletters.csv a\n\n# 0.3.1\nletters.csv m\n"));
        }

        /// <summary>Tests that <see cref="ReleasedIdLedger.Parse"/> refuses a missing ledger (no text).</summary>
        [Test]
        public void TestParse_RefusesAMissingLedger()
        {
            ContentProblems problems = new ContentProblems();

            ReleasedIdLedger ledger = ReleasedIdLedger.Parse(null, problems);

            ContentCsvs.AssertError(problems.All, ReleasedIdLedger.FileName, 0, "missing", "restore");
            Assert.That(ledger.Ids, Is.Empty);
        }

        /// <summary>
        /// Tests that <see cref="ReleasedIdLedger.CheckNoneDeleted"/> refuses a released id whose row is
        /// gone, naming its CSV and the id.
        /// </summary>
        [Test]
        public void TestCheckNoneDeleted_RefusesADeletedId()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            RemoveRow(csvs, ContentFileNames.Words, "ro-am,");
            RemoveRow(csvs, ContentFileNames.VoiceLines, "ro-word-am,");
            ContentProblems problems = new ContentProblems();
            ReleasedIdLedger ledger = ReleasedIdLedger.Parse("words.csv ro-mama\nwords.csv ro-am\nvoice-lines.csv ro-word-am\n", problems);

            ledger.CheckNoneDeleted(ContentCsvs.Read(csvs), problems);

            ContentCsvs.AssertError(problems.All, ContentFileNames.Words, 0, "ro-am", "released", "row 2", "retired");
            ContentCsvs.AssertError(problems.All, ContentFileNames.VoiceLines, 0, "ro-word-am", "released", "row 3");
            Assert.That(problems.All.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests that <see cref="ReleasedIdLedger.CheckNoneDeleted"/> accepts a released id that is
        /// retired, and looks for each id in its own CSV (the lesson ro-a is there, a letter ro-a isn't).
        /// </summary>
        [Test]
        public void TestCheckNoneDeleted_AcceptsARetiredId()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            ContentCsvs.ReplaceRow(csvs, ContentFileNames.Words, "ro-am,", "ro-am,ro,am,a m,am,,,,,yes,reading only");
            ContentProblems problems = new ContentProblems();
            ReleasedIdLedger ledger = ReleasedIdLedger.Parse("words.csv ro-am\nlessons.csv ro-a\nletters.csv ro-a\n", problems);

            ledger.CheckNoneDeleted(ContentCsvs.Read(csvs), problems);

            Assert.That(problems.All.Select(problem => problem.ToString()), Is.EqualTo(new[]
            {
                "letters.csv: the id ro-a was released (released-ids.txt, row 3), but its row is gone: ids are never "
                + "deleted (her progress names them), so put its row back and mark it retired."
            }));
        }

        /// <summary>
        /// Tests that <see cref="ReleasedIdLedger.NewIds"/> lists, by file and in CSV order, the ids not in
        /// the ledger, and that <see cref="ReleasedIdLedger.WithIds"/> adds them under the version.
        /// </summary>
        [Test]
        public void TestNewIds_ListsIdsNotReleasedYet()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            ContentCsvs.AddRow(csvs, ContentFileNames.Pictures, "moon,,,");
            Curriculum curriculum = ContentCsvs.Read(csvs);
            string released = string.Join("\n", AllIdLines(ContentCsvs.Valid())) + "\n";
            ReleasedIdLedger ledger = ReleasedIdLedger.Parse("# ledger\n" + released, new ContentProblems());

            IReadOnlyList<ReleasedId> newIds = ledger.NewIds(curriculum);
            string text = ledger.WithIds("0.3.1", newIds);

            Assert.That(newIds.Select(id => id.ToString()), Is.EqualTo(new[] { "pictures.csv moon" }));
            Assert.That(text, Is.EqualTo("# ledger\n" + released + "\n# 0.3.1\npictures.csv moon\n"));
            Assert.That(ledger.WithIds("0.3.1", new ReleasedId[0]), Is.EqualTo("# ledger\n" + released));
        }

        /// <summary>
        /// Tests that <see cref="ReleasedIdLedger.NewIds"/> lists every id of a curriculum for an empty
        /// ledger, letters first and lessons last.
        /// </summary>
        [Test]
        public void TestNewIds_AnEmptyLedgerListsEveryId()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            ReleasedIdLedger ledger = ReleasedIdLedger.Parse(string.Empty, new ContentProblems());

            IReadOnlyList<ReleasedId> newIds = ledger.NewIds(ContentCsvs.Read(csvs));

            Assert.That(newIds.Select(id => id.ToString()), Is.EqualTo(AllIdLines(csvs)));
            Assert.That(ledger.WithIds("0.2.0", newIds), Does.StartWith("# 0.2.0\nletters.csv a\n"));
        }

        /// <summary>
        /// Tests that the repository's <c>content/released-ids.txt</c> reads without a problem, holds ids
        /// of every kind (seeded with planet 1's sample, released in 0.2.0), and that none of its ids is
        /// missing from <c>content/*.csv</c> (<see cref="ReleasedIdLedger.CheckNoneDeleted"/>).
        /// </summary>
        [Test]
        public void TestCheckNoneDeleted_TheRepositorysLedgerMatchesItsContent()
        {
            ContentFolder folder = ContentImporter.RepositoryContent();
            ContentProblems problems = new ContentProblems();
            Curriculum curriculum = ContentTables.Read(folder.ReadCsvs(problems), problems);

            ReleasedIdLedger ledger = ReleasedIdLedger.Parse(folder.ReleasedIdsText(problems), problems);
            ledger.CheckNoneDeleted(curriculum, problems);

            Assert.That(problems.All.Where(problem => problem.IsError).Select(problem => problem.ToString()), Is.Empty);
            Assert.That(ledger.Ids.Select(id => id.File).Distinct(), Is.EquivalentTo(new[]
            {
                ContentFileNames.Letters, ContentFileNames.Words, ContentFileNames.Pictures,
                ContentFileNames.VoiceLines, ContentFileNames.Units, ContentFileNames.Lessons
            }));
            Assert.That(ledger.Ids.Select(id => id.ToString()), Does.Contain("words.csv ro-mama").And.Contain("lessons.csv party-1"));
        }

        private static void RemoveRow(Dictionary<string, string> csvs, string file, string rowStart)
        {
            csvs[file] = string.Join("\n", csvs[file].Split('\n')
                .Where(row => !row.StartsWith(rowStart, StringComparison.Ordinal)));
        }

        private static string[] AllIdLines(Dictionary<string, string> csvs)
        {
            string[] files =
            {
                ContentFileNames.Letters, ContentFileNames.Words, ContentFileNames.Pictures,
                ContentFileNames.VoiceLines, ContentFileNames.Units, ContentFileNames.Lessons
            };
            return files
                .SelectMany(file => csvs[file].Split('\n').Skip(1)
                    .Where(row => row.Length > 0)
                    .Select(row => file + " " + row.Substring(0, row.IndexOf(','))))
                .ToArray();
        }
    }
}
