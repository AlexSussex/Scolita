using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scolita.Core.Content.Csv
{
    /// <summary>
    /// Reads the text of a CSV file as a spreadsheet writes it: a header row, quoted fields (with
    /// commas, line breaks and <c>""</c> inside), CRLF or LF line endings, a byte order mark or none,
    /// and <c>,</c> or <c>;</c> between fields (Excel uses <c>;</c> in a Romanian Windows), told
    /// from the header. Cells are trimmed; blank rows are left out.
    /// </summary>
    public static class CsvReader
    {
        private const char ByteOrderMark = '\uFEFF';
        private const char Quote = '"';

        /// <summary>
        /// Reads <paramref name="text"/>, the contents of <paramref name="fileName"/>, reporting into
        /// <paramref name="problems"/> an empty file, a quote never closed, a repeated or empty column
        /// name and a row with more cells than the header (that row is left out). Called by
        /// <see cref="ContentTables.Read"/> and <c>CsvReaderTests</c>.
        /// </summary>
        public static CsvTable Read(string fileName, string text, ContentProblems problems)
        {
            string body = text ?? string.Empty;
            if (body.Length > 0 && body[0] == ByteOrderMark)
            {
                body = body.Substring(1);
            }

            char separator = SeparatorOf(body);
            List<List<string>> records = Records(fileName, body, separator, problems);
            if (records.Count == 0 || records[0].All(cell => cell.Length == 0))
            {
                problems.Error(new ContentSource(fileName, 0), "the file has no header row.");
                return new CsvTable(fileName, new string[0], new CsvRow[0]);
            }

            string[] columns = records[0].ToArray();
            CheckColumns(fileName, columns, problems);
            return new CsvTable(fileName, columns, Rows(fileName, columns.Length, records, problems));
        }

        private static IReadOnlyList<CsvRow> Rows(string fileName, int columnCount, List<List<string>> records,
            ContentProblems problems)
        {
            List<CsvRow> rows = new List<CsvRow>();
            for (int index = 1; index < records.Count; index++)
            {
                List<string> cells = records[index];
                int number = index + 1;
                if (cells.All(cell => cell.Length == 0))
                {
                    continue;
                }

                if (cells.Count > columnCount && cells.Skip(columnCount).Any(cell => cell.Length > 0))
                {
                    problems.Error(new ContentSource(fileName, number), "the row has " + cells.Count
                        + " cells but the header has " + columnCount
                        + " columns: put a cell holding a comma or a semicolon in quotes.");
                    continue;
                }

                rows.Add(new CsvRow(number, cells));
            }

            return rows;
        }

        private static void CheckColumns(string fileName, IReadOnlyList<string> columns, ContentProblems problems)
        {
            ContentSource header = new ContentSource(fileName, 1);
            HashSet<string> seen = new HashSet<string>();
            foreach (string column in columns)
            {
                if (column.Length == 0)
                {
                    problems.Error(header, "a column of the header has no name.");
                }
                else if (!seen.Add(column))
                {
                    problems.Error(header, "the column " + column + " appears twice in the header.");
                }
            }
        }

        private static char SeparatorOf(string body)
        {
            bool quoted = false;
            int commas = 0;
            int semicolons = 0;
            foreach (char character in body)
            {
                if (character == Quote)
                {
                    quoted = !quoted;
                }
                else if (!quoted && (character == '\n' || character == '\r'))
                {
                    break;
                }
                else if (!quoted && character == ',')
                {
                    commas++;
                }
                else if (!quoted && character == ';')
                {
                    semicolons++;
                }
            }

            return semicolons > commas ? ';' : ',';
        }

        private static List<List<string>> Records(string fileName, string body, char separator,
            ContentProblems problems)
        {
            List<List<string>> records = new List<List<string>>();
            List<string> record = new List<string>();
            StringBuilder cell = new StringBuilder();
            bool quoted = false;
            int index = 0;
            while (index < body.Length)
            {
                char character = body[index];
                if (quoted)
                {
                    if (character == Quote && index + 1 < body.Length && body[index + 1] == Quote)
                    {
                        cell.Append(Quote);
                        index += 2;
                        continue;
                    }

                    if (character == Quote)
                    {
                        quoted = false;
                    }
                    else
                    {
                        cell.Append(character);
                    }

                    index++;
                    continue;
                }

                if (character == Quote)
                {
                    quoted = true;
                }
                else if (character == separator)
                {
                    record.Add(cell.ToString().Trim());
                    cell.Clear();
                }
                else if (character == '\r' || character == '\n')
                {
                    record.Add(cell.ToString().Trim());
                    cell.Clear();
                    records.Add(record);
                    record = new List<string>();
                    if (character == '\r' && index + 1 < body.Length && body[index + 1] == '\n')
                    {
                        index++;
                    }
                }
                else
                {
                    cell.Append(character);
                }

                index++;
            }

            if (quoted)
            {
                problems.Error(new ContentSource(fileName, records.Count + 1),
                    "a quote opened in this row is never closed.");
            }

            if (cell.Length > 0 || record.Count > 0)
            {
                record.Add(cell.ToString().Trim());
                records.Add(record);
            }

            return records;
        }
    }
}
