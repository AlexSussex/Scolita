namespace Scolita.Core.Progress
{
    /// <summary>
    /// What kind of content item her progress is kept for. A later kind (a letter's name, a number, a
    /// note) is added here, with its line in <see cref="PractisedItems"/>, and raises the
    /// <c>progress</c> file's version.
    /// </summary>
    public enum ItemKind
    {
        /// <summary>A letter of the letters catalog, in one language (its sound).</summary>
        Letter,

        /// <summary>A word of the words catalog (a word has one language).</summary>
        Word
    }
}
