using System;
using System.Collections.Generic;

namespace Scolita.Core.Content.Csv
{
    /// <summary>
    /// A CSV file as read by <see cref="CsvReader"/>: its name, its header's column names and its
    /// rows (blank rows left out, each keeping its own row number).
    /// </summary>
    public sealed class CsvTable
    {
        /// <summary>Creates a table; called by <see cref="CsvReader"/>.</summary>
        public CsvTable(string fileName, IReadOnlyList<string> columns, IReadOnlyList<CsvRow> rows, char separator,
            CsvRow header)
        {
            FileName = fileName;
            Columns = columns;
            Rows = rows;
            Separator = separator;
            Header = header;
        }

        /// <summary>The character between cells (<c>,</c>, or <c>;</c> from a Romanian Excel).</summary>
        public char Separator { get; }

        /// <summary>The header row, with its cells' places in the text; null for a file without one.</summary>
        public CsvRow Header { get; }

        /// <summary>The index of the column named <paramref name="column"/>, or -1; called by <see cref="CsvCellWriter"/>.</summary>
        public int ColumnIndex(string column)
        {
            for (int index = 0; index < Columns.Count; index++)
            {
                if (string.Equals(Columns[index], column, StringComparison.Ordinal))
                {
                    return index;
                }
            }

            return -1;
        }

        /// <summary>The file's name (<c>words.csv</c>).</summary>
        public string FileName { get; }

        /// <summary>The header's column names, in order.</summary>
        public IReadOnlyList<string> Columns { get; }

        /// <summary>The rows after the header, blank rows left out.</summary>
        public IReadOnlyList<CsvRow> Rows { get; }

        /// <summary>
        /// The cell of <paramref name="row"/> in the column named <paramref name="column"/>, or an
        /// empty string when the table has no such column (<see cref="ContentTables"/> reports that).
        /// Called by <see cref="ContentTables"/>.
        /// </summary>
        public string Cell(CsvRow row, string column)
        {
            int index = ColumnIndex(column);
            return index < 0 ? string.Empty : row.Cell(index);
        }

        /// <summary>The source of <paramref name="row"/>, for problems; called by <see cref="ContentTables"/>.</summary>
        public ContentSource SourceOf(CsvRow row)
        {
            return new ContentSource(FileName, row.Number);
        }
    }
}
