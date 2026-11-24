namespace Scolita.Core.Content
{
    /// <summary>
    /// A picture, in no language and named by what it shows (<c>bed</c>, <c>moon</c>), shared by
    /// twin words. Its art is a file in <c>content/pictures/</c>; with none yet, the importer draws a
    /// placeholder. One row of <c>pictures.csv</c>.
    /// </summary>
    public sealed class Picture : IContentItem
    {
        /// <summary>Creates a picture; called by <see cref="ContentTables"/>, the picture asset and tests.</summary>
        public Picture(ContentId id, string art, bool retired, ContentSource source)
        {
            Id = id;
            Art = art ?? string.Empty;
            Retired = retired;
            Source = source;
        }

        /// <inheritdoc/>
        public ContentId Id { get; }

        /// <summary>The art's file name in <c>content/pictures/</c> (<c>bed.png</c>); empty for none yet.</summary>
        public string Art { get; }

        /// <summary>Whether it has art (otherwise it shows its placeholder).</summary>
        public bool HasArt => Art.Length > 0;

        /// <inheritdoc/>
        public bool Retired { get; }

        /// <inheritdoc/>
        public ContentSource Source { get; }
    }
}
