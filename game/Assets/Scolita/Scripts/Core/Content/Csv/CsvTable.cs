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
        public CsvTable(string fileName, IReadOnlyList<string> columns, IReadOnlyList<CsvRow> rows)
        {
            FileName = fileName;
            Columns = columns;
            Rows = rows;
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
            for (int index = 0; index < Columns.Count; index++)
            {
                if (string.Equals(Columns[index], column, StringComparison.Ordinal))
                {
                    return row.Cell(index);
                }
            }

            return string.Empty;
        }

        /// <summary>The source of <paramref name="row"/>, for problems; called by <see cref="ContentTables"/>.</summary>
        public ContentSource SourceOf(CsvRow row)
        {
            return new ContentSource(FileName, row.Number);
        }
    }
}
