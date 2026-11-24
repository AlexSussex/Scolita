namespace Scolita.Core.Content
{
    /// <summary>
    /// What the content rules need to know about the files beside the CSVs, without Core touching a
    /// file: whether a picture's art and a voice line's clip are there. The importer's implementation
    /// looks in <c>content/pictures/</c> and <c>content/voice/</c>; tests use a fake.
    /// </summary>
    public interface IContentFiles
    {
        /// <summary>Whether the art <paramref name="fileName"/> is in <c>content/pictures/</c>; called by the picture rule.</summary>
        bool ArtExists(string fileName);

        /// <summary>Whether the clip <paramref name="fileName"/> is in <c>content/voice/</c>; called by the voice line rule.</summary>
        bool ClipExists(string fileName);
    }
}
