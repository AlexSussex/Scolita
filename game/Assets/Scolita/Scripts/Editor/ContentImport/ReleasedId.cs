namespace Scolita.Editor.ContentImport
{
    /// <summary>
    /// One line of the ledger of released ids (<see cref="ReleasedIdLedger"/>): a stable id, the CSV
    /// that holds it (<c>words.csv</c>), and its row in <c>content/released-ids.txt</c>.
    /// </summary>
    public readonly struct ReleasedId
    {
        /// <summary>Creates a line; called by <see cref="ReleasedIdLedger"/>.</summary>
        public ReleasedId(string file, string id, int row)
        {
            File = file;
            Id = id;
            Row = row;
        }

        /// <summary>The CSV that holds the id, such as <c>words.csv</c>.</summary>
        public string File { get; }

        /// <summary>The id, such as <c>ro-mama</c>.</summary>
        public string Id { get; }

        /// <summary>Its row in the ledger, counted from 1; 0 for an id not written there yet.</summary>
        public int Row { get; }

        /// <summary>The ledger's line for it, <c>words.csv ro-mama</c>; called by <see cref="ReleasedIdLedger"/>.</summary>
        public override string ToString()
        {
            return File + " " + Id;
        }
    }
}
