using System.Text.RegularExpressions;

namespace Scolita.Core.Ids
{
    /// <summary>
    /// The one rule for stable ids (a profile, a letter, a word, a lesson, a voice line): lower-case
    /// ASCII letters, digits and <c>-</c> only, so an id is safe as a file or folder name and reads
    /// the same everywhere.
    /// </summary>
    public static class StableIdText
    {
        private static readonly Regex Pattern = new Regex(@"^[a-z0-9-]+\z", RegexOptions.CultureInvariant);

        /// <summary>
        /// Whether <paramref name="text"/> is a valid stable id (<c>[a-z0-9-]+</c>). Called by
        /// <see cref="Profiles.ProfileId"/>, <see cref="Content.ContentId"/> and their tests.
        /// </summary>
        public static bool IsValid(string text)
        {
            return text != null && Pattern.IsMatch(text);
        }
    }
}
