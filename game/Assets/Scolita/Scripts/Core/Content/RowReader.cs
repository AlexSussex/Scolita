using System.Collections.Generic;
using Scolita.Core.Content.Csv;
using Scolita.Core.Languages;

namespace Scolita.Core.Content
{
    /// <summary>
    /// Reads the cells of one CSV row as the content's types (ids, codes, yes/no, lists), reporting
    /// each cell it can't read against the row, and remembering whether the row had any such cell.
    /// Used by <see cref="ContentTables"/> for every row.
    /// </summary>
    public sealed class RowReader
    {
        private readonly CsvTable table;
        private readonly CsvRow row;
        private readonly ContentProblems problems;

        /// <summary>Creates a reader of <paramref name="row"/>; called by <see cref="ContentTables"/>.</summary>
        public RowReader(CsvTable table, CsvRow row, ContentProblems problems)
        {
            this.table = table;
            this.row = row;
            this.problems = problems;
            IsGood = true;
        }

        /// <summary>Whether every cell read so far could be read.</summary>
        public bool IsGood { get; private set; }

        /// <summary>The row's file and number.</summary>
        public ContentSource Source => table.SourceOf(row);

        /// <summary>Reports a problem with the row, which is then left out; called by <see cref="ContentTables"/>.</summary>
        public void Error(string reason)
        {
            Report(reason);
        }

        /// <summary>The cell <paramref name="column"/>, which must not be empty; called by <see cref="ContentTables"/>.</summary>
        public string Required(string column)
        {
            return RequiredCell(column);
        }

        /// <summary>The cell <paramref name="column"/>, which may be empty; called by <see cref="ContentTables"/>.</summary>
        public string Optional(string column)
        {
            return table.Cell(row, column);
        }

        /// <summary>The id in the cell <paramref name="column"/>, which must be there; called by <see cref="ContentTables"/>.</summary>
        public ContentId RequiredId(string column)
        {
            string cell = RequiredCell(column);
            return cell.Length == 0 ? default : IdIn(column, cell);
        }

        /// <summary>
        /// The id in the cell <paramref name="column"/>, or the unset id when it is empty; called by
        /// <see cref="ContentTables"/>.
        /// </summary>
        public ContentId OptionalId(string column)
        {
            string cell = table.Cell(row, column);
            return cell.Length == 0 ? default : IdIn(column, cell);
        }

        /// <summary>
        /// The ids in the cell <paramref name="column"/>, separated by spaces (<c>m a m a</c>); empty
        /// for an empty cell. Called by <see cref="ContentTables"/>.
        /// </summary>
        public IReadOnlyList<ContentId> IdList(string column)
        {
            List<ContentId> ids = new List<ContentId>();
            foreach (string part in table.Cell(row, column).Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries))
            {
                ContentId id = IdIn(column, part);
                if (id.IsSet)
                {
                    ids.Add(id);
                }
            }

            return ids;
        }

        /// <summary>
        /// The syllables in the cell <paramref name="column"/>, split by <c>-</c> (<c>ma-ma</c>); the
        /// cell must not be empty, nor any syllable. Called by <see cref="ContentTables"/>.
        /// </summary>
        public IReadOnlyList<string> Syllables(string column)
        {
            string cell = RequiredCell(column);
            if (cell.Length == 0)
            {
                return new string[0];
            }

            string[] syllables = cell.Split('-');
            foreach (string syllable in syllables)
            {
                if (syllable.Trim().Length == 0)
                {
                    Report("the cell " + column + " has an empty syllable in '" + cell + "' (split them with one -).");
                }
            }

            return syllables;
        }

        /// <summary>
        /// The options in the cell <paramref name="column"/>: <c>key=value</c> pairs separated by
        /// <c>;</c>. Called by <see cref="ContentTables"/>.
        /// </summary>
        public IReadOnlyList<StepOption> Options(string column)
        {
            List<StepOption> options = new List<StepOption>();
            foreach (string part in table.Cell(row, column).Split(new[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries))
            {
                string[] pair = part.Split('=');
                if (pair.Length != 2 || pair[0].Trim().Length == 0 || pair[1].Trim().Length == 0)
                {
                    Report("the option '" + part.Trim() + "' is not key=value.");
                    continue;
                }

                options.Add(new StepOption(pair[0].Trim(), pair[1].Trim()));
            }

            return options;
        }

        /// <summary>
        /// Whether the cell <paramref name="column"/> says yes (<c>yes</c>) or no (empty); anything
        /// else is reported and read as no. Called by <see cref="ContentTables"/>.
        /// </summary>
        public bool YesNo(string column)
        {
            string cell = table.Cell(row, column);
            if (cell.Length == 0)
            {
                return false;
            }

            if (cell == ContentCodes.Yes)
            {
                return true;
            }

            Report("the cell " + column + " says '" + cell + "': write yes, or leave it empty for no.");
            return false;
        }

        /// <summary>
        /// The language (<c>ro</c> or <c>en</c>) in the cell <paramref name="column"/>, which must be
        /// there; called by <see cref="ContentTables"/>.
        /// </summary>
        public bool TryLanguage(string column, out Language language)
        {
            string cell = RequiredCell(column);
            if (LanguageCodes.TryParseLanguage(cell, out language))
            {
                return true;
            }

            if (cell.Length > 0)
            {
                Report("the language '" + cell + "' is not ro or en.");
            }

            return false;
        }

        /// <summary>
        /// The languages (<c>ro</c>, <c>en</c> or <c>both</c>) in the cell <paramref name="column"/>,
        /// or null when it is empty; called by <see cref="ContentTables"/>.
        /// </summary>
        public LanguageMix? LanguageMixOrNone(string column)
        {
            string cell = table.Cell(row, column);
            if (cell.Length == 0)
            {
                return null;
            }

            if (LanguageCodes.TryParseMix(cell, out LanguageMix mix))
            {
                return mix;
            }

            Report("the language '" + cell + "' is not ro, en or both.");
            return null;
        }

        private string RequiredCell(string column)
        {
            string cell = table.Cell(row, column);
            if (cell.Length == 0)
            {
                Report("the cell " + column + " is empty.");
            }

            return cell;
        }

        private void Report(string reason)
        {
            problems.Error(Source, reason);
            IsGood = false;
        }

        private ContentId IdIn(string column, string text)
        {
            if (ContentId.TryCreate(text, out ContentId id))
            {
                return id;
            }

            Report("the cell " + column + " has '" + text
                + "', which is not an id (lower-case letters a to z, digits and - only).");
            return default;
        }
    }
}
