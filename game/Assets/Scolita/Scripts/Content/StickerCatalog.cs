namespace Scolita.Content
{
    /// <summary>Every sticker asset, in the order of <c>stickers.csv</c> (<c>Content/StickerCatalog.asset</c>).</summary>
    public sealed class StickerCatalog : ContentCatalog<StickerAsset>
    {
        /// <inheritdoc/>
        protected override string IdOf(StickerAsset asset)
        {
            return asset.Id;
        }
    }
}
