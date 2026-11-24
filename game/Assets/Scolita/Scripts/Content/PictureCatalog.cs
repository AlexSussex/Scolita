namespace Scolita.Content
{
    /// <summary>Every picture asset, in the order of <c>pictures.csv</c> (<c>Content/PictureCatalog.asset</c>).</summary>
    public sealed class PictureCatalog : ContentCatalog<PictureAsset>
    {
        /// <inheritdoc/>
        protected override string IdOf(PictureAsset asset)
        {
            return asset.Id;
        }
    }
}
