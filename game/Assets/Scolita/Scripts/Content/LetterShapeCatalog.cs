namespace Scolita.Content
{
    /// <summary>Every letter shape asset, in the order of their file names (<c>Content/LetterShapeCatalog.asset</c>).</summary>
    public sealed class LetterShapeCatalog : ContentCatalog<LetterShapeAsset>
    {
        /// <inheritdoc/>
        protected override string IdOf(LetterShapeAsset asset)
        {
            return asset.Id;
        }
    }
}
