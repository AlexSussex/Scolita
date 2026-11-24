namespace Scolita.Core.Content
{
    /// <summary>
    /// Where a content item or a problem comes from: a file (<c>words.csv</c>, or an asset's name)
    /// and its row as a spreadsheet numbers it (the header is row 1, so the first item is row 2);
    /// row 0 for the file as a whole.
    /// </summary>
    public readonly struct ContentSource
    {
        /// <summary>Creates a source; called by <see cref="ContentTables"/>, the content assets and tests.</summary>
        public ContentSource(string file, int row)
        {
            File = file ?? string.Empty;
            Row = row;
        }

        /// <summary>The file's name, or the asset's name.</summary>
        public string File { get; }

        /// <summary>The row, counted as a spreadsheet does; 0 for the whole file.</summary>
        public int Row { get; }

        /// <summary><c>words.csv, row 7</c>, or the file's name alone for row 0.</summary>
        public override string ToString()
        {
            return Row > 0 ? File + ", row " + Row : File;
        }
    }
}
