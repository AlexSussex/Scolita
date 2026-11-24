using System.Collections.Generic;

namespace Scolita.Core.Content.Csv
{
    /// <summary>A record being read by <see cref="CsvReader"/>: its cells, where each is in the text, and where it ends.</summary>
    internal sealed class CsvRecord
    {
        /// <summary>The cells, trimmed.</summary>
        public List<string> Cells { get; } = new List<string>();

        /// <summary>Where each cell is in the text.</summary>
        public List<CsvSpan> Spans { get; } = new List<CsvSpan>();

        /// <summary>The index in the text just after the record's last cell (before its line break).</summary>
        public int End { get; set; }

        /// <summary>Adds a cell; called by <see cref="CsvReader"/>.</summary>
        public void Add(string cell, CsvSpan span)
        {
            Cells.Add(cell);
            Spans.Add(span);
        }
    }
}
