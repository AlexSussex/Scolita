namespace Scolita.Core.Languages
{
    /// <summary>
    /// A cedilla letter (ş ţ Ş Ţ) found in a text by <see cref="RomanianSpelling.FindCedillas"/>:
    /// where it is and the comma-below letter that belongs there instead.
    /// </summary>
    public readonly struct CedillaFinding
    {
        /// <summary>Creates a finding; called by <see cref="RomanianSpelling.FindCedillas"/>.</summary>
        public CedillaFinding(int line, int column, char found, char correct)
        {
            Line = line;
            Column = column;
            Found = found;
            Correct = correct;
        }

        /// <summary>The line of the text it is on, counted from 1.</summary>
        public int Line { get; }

        /// <summary>Its column on that line, counted from 1.</summary>
        public int Column { get; }

        /// <summary>The cedilla letter found.</summary>
        public char Found { get; }

        /// <summary>The comma-below letter Romanian spells there.</summary>
        public char Correct { get; }

        /// <summary>
        /// Says where the letter is and what to write instead, with both letters' code points so the
        /// look-alike letters can be told apart: <c>line 3, column 7: U+015F ş must be U+0219 ș</c>.
        /// </summary>
        public override string ToString()
        {
            return "line " + Line + ", column " + Column + ": U+" + ((int)Found).ToString("X4") + " " + Found
                + " must be U+" + ((int)Correct).ToString("X4") + " " + Correct;
        }
    }
}
