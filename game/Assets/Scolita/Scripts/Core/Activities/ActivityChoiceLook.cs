namespace Scolita.Core.Activities
{
    /// <summary>How a choice of an activity is drawn (<see cref="ActivityChoice.Look"/>).</summary>
    public enum ActivityChoiceLook
    {
        /// <summary>A letter, in the reading typeface.</summary>
        Letter,

        /// <summary>A picture, with its word under it where the kind shows words.</summary>
        Picture,

        /// <summary>
        /// A word written to read, in the reading typeface; its picture is shown only when the round reveals
        /// it (<c>ReadAndMatch</c>'s card, a readable <c>WordTwins</c> word).
        /// </summary>
        Word,

        /// <summary>A part of the activity the view draws by its index (<c>Syllables</c>' drum and tick).</summary>
        Control
    }
}
