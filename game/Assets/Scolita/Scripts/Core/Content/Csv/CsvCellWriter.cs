using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scolita.Core.Content.Csv
{
    /// <summary>
    /// Writes one column's cells into a CSV file's text, leaving every other character as it was (the
    /// quotes, the separator, the line endings, the other cells), so a spreadsheet's file changes only
    /// where the column changes. Adds the column after another one if the header doesn't have it.
    /// Used by the content importer to fill <c>voice-lines.csv</c>'s <c>status</c> column.
    /// </summary>
    public static class CsvCellWriter
    {
        /// <summary>
        /// The text of <paramref name="fileName"/> with the cell of <paramref name="column"/> set, in
        /// each row whose <paramref name="idColumn"/> is a key of <paramref name="values"/>, to that
        /// value (other rows keep theirs). A missing column is added right after
        /// <paramref name="afterColumn"/>. Throws an <see cref="ArgumentException"/> for a value that
        /// would need quotes, or when <paramref name="idColumn"/> or <paramref name="afterColumn"/>
        /// isn't in the header. Problems reading the file go into <paramref name="problems"/>. Called
        /// by the content importer and <c>CsvCellWriterTests</c>.
        /// </summary>
        public static string Write(string fileName, string text, string idColumn, string column, string afterColumn,
            IReadOnlyDictionary<string, string> values, ContentProblems problems)
        {
            CsvTable table = CsvReader.Read(fileName, text, problems);
            if (table.Header == null)
            {
                return text;
            }

            CheckValues(values.Values, table.Separator);
            int idIndex = RequiredIndex(table, idColumn);
            int columnIndex = table.ColumnIndex(column);
            List<KeyValuePair<int, string>> inserts = new List<KeyValuePair<int, string>>();
            List<(int Start, int Length, string Text)> replacements = new List<(int Start, int Length, string Text)>();
            if (columnIndex < 0)
            {
                int afterIndex = RequiredIndex(table, afterColumn);
                AddColumn(table, afterIndex, column, idIndex, values, inserts);
            }
            else
            {
                SetCells(table, columnIndex, idIndex, values, inserts, replacements);
            }

            foreach (KeyValuePair<int, string> insert in inserts)
            {
                replacements.Add((insert.Key, 0, insert.Value));
            }

            StringBuilder result = new StringBuilder(text ?? string.Empty);
            foreach ((int Start, int Length, string Text) replacement in replacements.OrderByDescending(replacement => replacement.Start))
            {
                result.Remove(replacement.Start, replacement.Length);
                result.Insert(replacement.Start, replacement.Text);
            }

            return result.ToString();
        }

        private static void CheckValues(IEnumerable<string> values, char separator)
        {
            foreach (string value in values)
            {
                if (value.IndexOf(separator) >= 0 || value.IndexOf('"') >= 0 || value.IndexOf('\n') >= 0
                    || value.IndexOf('\r') >= 0)
                {
                    throw new ArgumentException("The value '" + value + "' would need quotes in a CSV cell.",
                        nameof(values));
                }
            }
        }

        private static int RequiredIndex(CsvTable table, string column)
        {
            int index = table.ColumnIndex(column);
            if (index < 0)
            {
                throw new ArgumentException(table.FileName + " has no column " + column + ".", nameof(column));
            }

            return index;
        }

        private static void SetCells(CsvTable table, int columnIndex, int idIndex, IReadOnlyDictionary<string, string> values,
            List<KeyValuePair<int, string>> inserts, List<(int Start, int Length, string Text)> replacements)
        {
            foreach (CsvRow row in table.Rows)
            {
                if (!values.TryGetValue(row.Cell(idIndex), out string value) || row.Cell(columnIndex) == value)
                {
                    continue;
                }

                if (row.TryGetSpan(columnIndex, out CsvSpan span))
                {
                    replacements.Add((span.Start, span.Length, value));
                }
                else
                {
                    inserts.Add(new KeyValuePair<int, string>(row.End,
                        new string(table.Separator, columnIndex - row.CellCount + 1) + value));
                }
            }
        }

        private static void AddColumn(CsvTable table, int afterIndex, string column, int idIndex,
            IReadOnlyDictionary<string, string> values, List<KeyValuePair<int, string>> inserts)
        {
            inserts.Add(new KeyValuePair<int, string>(EndOfCell(table.Header, afterIndex), table.Separator + column));
            foreach (CsvRow row in table.Rows)
            {
                string value = values.TryGetValue(row.Cell(idIndex), out string found) ? found : string.Empty;
                if (row.TryGetSpan(afterIndex, out CsvSpan _))
                {
                    inserts.Add(new KeyValuePair<int, string>(EndOfCell(row, afterIndex), table.Separator + value));
                }
                else if (value.Length > 0)
                {
                    inserts.Add(new KeyValuePair<int, string>(row.End,
                        new string(table.Separator, afterIndex - row.CellCount + 2) + value));
                }
            }
        }

        private static int EndOfCell(CsvRow row, int index)
        {
            row.TryGetSpan(index, out CsvSpan span);
            return span.Start + span.Length;
        }
    }
}
