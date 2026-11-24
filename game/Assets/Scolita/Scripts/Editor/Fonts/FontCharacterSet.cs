namespace Scolita.Editor.Fonts
{
    /// <summary>
    /// The characters baked into every font atlas by <see cref="FontAssetBuilder"/>: printable
    /// ASCII, the Romanian letters ă â î ș ț and their capitals (ș ț with the comma below, never the
    /// cedilla), and the quotation marks and dashes of Romanian and English text. A text needing
    /// another character adds it here and rebuilds the fonts (Școlița → Rebuild font assets).
    /// </summary>
    public static class FontCharacterSet
    {
        /// <summary>The ten Romanian letters beyond ASCII: ă â î ș ț Ă Â Î Ș Ț.</summary>
        public const string RomanianLetters = "\u0103\u00E2\u00EE\u0219\u021B\u0102\u00C2\u00CE\u0218\u021A";

        /// <summary>„ ” “ ‘ ’ – — … • ·: Romanian and English quotation marks, dashes, the ellipsis and dots.</summary>
        public const string Punctuation = "\u201E\u201D\u201C\u2018\u2019\u2013\u2014\u2026\u2022\u00B7";

        private const char FirstPrintableAscii = ' ';
        private const char LastPrintableAscii = '~';

        /// <summary>
        /// Every character of the set, in one string. Called by <see cref="FontAssetBuilder"/>.
        /// </summary>
        public static string All()
        {
            char[] ascii = new char[LastPrintableAscii - FirstPrintableAscii + 1];
            for (int i = 0; i < ascii.Length; i++)
            {
                ascii[i] = (char)(FirstPrintableAscii + i);
            }

            return new string(ascii) + RomanianLetters + Punctuation;
        }
    }
}
