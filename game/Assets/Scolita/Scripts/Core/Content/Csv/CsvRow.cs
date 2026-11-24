using System.Collections.Generic;

namespace Scolita.Core.Content.Csv
{
    /// <summary>One row of a <see cref="CsvTable"/>: its row number as a spreadsheet shows it, and its cells, trimmed.</summary>
    public sealed class CsvRow
    {
        private readonly IReadOnlyList<string> cells;

        /// <summary>Creates a row; called by <see cref="CsvReader"/>.</summary>
        public CsvRow(int number, IReadOnlyList<string> cells)
        {
            Number = number;
            this.cells = cells;
        }

        /// <summary>The row's number, as a spreadsheet shows it (the header is row 1).</summary>
        public int Number { get; }

        /// <summary>
        /// The cell in column <paramref name="index"/>, or an empty string when the row stops before
        /// it. Called by <see cref="CsvTable.Cell"/>.
        /// </summary>
        public string Cell(int index)
        {
            return index >= 0 && index < cells.Count ? cells[index] : string.Empty;
        }
    }
}
