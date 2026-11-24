namespace Scolita.Core.Content.Csv
{
    /// <summary>Where a cell is in its file's text, quotes and spaces included, so it can be rewritten in place.</summary>
    public readonly struct CsvSpan
    {
        /// <summary>Creates a span; called by <see cref="CsvReader"/>.</summary>
        public CsvSpan(int start, int length)
        {
            Start = start;
            Length = length;
        }

        /// <summary>The index of the cell's first character in the file's text.</summary>
        public int Start { get; }

        /// <summary>The cell's length in the text.</summary>
        public int Length { get; }
    }
}
