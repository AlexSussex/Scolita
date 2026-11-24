namespace Scolita.Core.Content
{
    /// <summary>How a kind of step shows its words, which decides what the content rules ask of them.</summary>
    public enum ActivityWordUse
    {
        /// <summary>The kind takes no words, or only says them.</summary>
        Heard,

        /// <summary>The kind shows each word's picture: every word needs one; listen-only words are fine.</summary>
        Pictured,

        /// <summary>The kind shows its words to read: no listen-only word.</summary>
        Read,

        /// <summary>The kind shows its words to read and their pictures: both rules.</summary>
        ReadAndPictured
    }
}
