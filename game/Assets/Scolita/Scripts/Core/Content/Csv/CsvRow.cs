using System.Collections.Generic;

namespace Scolita.Core.Content.Csv
{
    /// <summary>
    /// One row of a <see cref="CsvTable"/>: its row number as a spreadsheet shows it, its cells, trimmed,
    /// and where each cell is in the file's text (for <see cref="CsvCellWriter"/>).
    /// </summary>
    public sealed class CsvRow
    {
        private readonly IReadOnlyList<string> cells;
        private readonly IReadOnlyList<CsvSpan> spans;

        /// <summary>Creates a row; called by <see cref="CsvReader"/> and tests.</summary>
        public CsvRow(int number, IReadOnlyList<string> cells, IReadOnlyList<CsvSpan> spans, int end)
        {
            Number = number;
            this.cells = cells;
            this.spans = spans;
            End = end;
        }

        /// <summary>The row's number, as a spreadsheet shows it (the header is row 1).</summary>
        public int Number { get; }

        /// <summary>How many cells the row has (it may stop before the header's last column).</summary>
        public int CellCount => cells.Count;

        /// <summary>The index in the file's text just after the row's last cell.</summary>
        public int End { get; }

        /// <summary>
        /// The cell in column <paramref name="index"/>, or an empty string when the row stops before
        /// it. Called by <see cref="CsvTable.Cell"/>.
        /// </summary>
        public string Cell(int index)
        {
            return index >= 0 && index < cells.Count ? cells[index] : string.Empty;
        }

        /// <summary>
        /// Where the cell in column <paramref name="index"/> is in the text; false when the row stops
        /// before it. Called by <see cref="CsvCellWriter"/>.
        /// </summary>
        public bool TryGetSpan(int index, out CsvSpan span)
        {
            bool has = index >= 0 && index < spans.Count;
            span = has ? spans[index] : default;
            return has;
        }
    }
}
