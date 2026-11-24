using System;
using System.Collections.Generic;
using Scolita.Core.Content.Rules;

namespace Scolita.Core.Content
{
    /// <summary>
    /// Which letter a word starts with, by its spelling: the longest of the letters given that matches
    /// (a group such as <c>ch</c> before <c>c</c>). Used by <c>FirstSound</c>'s rules (its answer) and the
    /// voice (a stretched word said from its first sound until it is recorded).
    /// </summary>
    public static class WordStarts
    {
        /// <summary>
        /// Finds the letter among <paramref name="letters"/> that <paramref name="word"/> starts with; false
        /// when none does. Called by <c>FirstSoundRules</c>, <c>VoicePlanner</c> and <c>FirstSoundRulesTests</c>.
        /// </summary>
        public static bool TryFind(Word word, IEnumerable<Letter> letters, out Letter found)
        {
            string spelling = WordRules.Spelling(word.Text);
            found = null;
            foreach (Letter letter in letters)
            {
                bool starts = letter.Lower.Length > 0 && spelling.StartsWith(letter.Lower, StringComparison.Ordinal);
                if (starts && (found == null || letter.Lower.Length > found.Lower.Length))
                {
                    found = letter;
                }
            }

            return found != null;
        }
    }
}
