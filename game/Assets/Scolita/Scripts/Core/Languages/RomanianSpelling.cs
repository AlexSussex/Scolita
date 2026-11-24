using System.Collections.Generic;

namespace Scolita.Core.Languages
{
    /// <summary>
    /// Romanian spelling rules that can be checked by a program. Romanian writes ș and ț with the
    /// comma below (U+0219, U+021B and the capitals U+0218, U+021A), never the look-alike cedilla
    /// letters ş ţ Ş Ţ (U+015F, U+0163, U+015E, U+0162). Used by the content validator on the
    /// string tables and the content CSVs.
    /// </summary>
    public static class RomanianSpelling
    {
        private const char SmallSCedilla = '\u015F';
        private const char SmallTCedilla = '\u0163';
        private const char CapitalSCedilla = '\u015E';
        private const char CapitalTCedilla = '\u0162';
        private const char SmallSCommaBelow = '\u0219';
        private const char SmallTCommaBelow = '\u021B';
        private const char CapitalSCommaBelow = '\u0218';
        private const char CapitalTCommaBelow = '\u021A';

        /// <summary>
        /// Finds every cedilla letter in <paramref name="text"/>, in order, with its line and column
        /// (a line ends at <c>\n</c>; a <c>\r</c> before it is not counted). Empty when there is none
        /// or the text is null. Called by the content validator (<c>CedillaCheckTests</c>) and by
        /// <c>RomanianSpellingTests</c>.
        /// </summary>
        public static IReadOnlyList<CedillaFinding> FindCedillas(string text)
        {
            List<CedillaFinding> findings = new List<CedillaFinding>();
            if (string.IsNullOrEmpty(text))
            {
                return findings;
            }

            int line = 1;
            int column = 0;
            foreach (char character in text)
            {
                if (character == '\n')
                {
                    line++;
                    column = 0;
                    continue;
                }

                if (character != '\r')
                {
                    column++;
                }

                if (TryGetCommaBelow(character, out char correct))
                {
                    findings.Add(new CedillaFinding(line, column, character, correct));
                }
            }

            return findings;
        }

        private static bool TryGetCommaBelow(char character, out char correct)
        {
            switch (character)
            {
                case SmallSCedilla:
                    correct = SmallSCommaBelow;
                    return true;
                case SmallTCedilla:
                    correct = SmallTCommaBelow;
                    return true;
                case CapitalSCedilla:
                    correct = CapitalSCommaBelow;
                    return true;
                case CapitalTCedilla:
                    correct = CapitalTCommaBelow;
                    return true;
                default:
                    correct = default;
                    return false;
            }
        }
    }
}
