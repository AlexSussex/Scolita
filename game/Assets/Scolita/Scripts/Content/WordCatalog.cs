namespace Scolita.Content
{
    /// <summary>Every word asset, both languages, in the order of <c>words.csv</c> (<c>Content/WordCatalog.asset</c>).</summary>
    public sealed class WordCatalog : ContentCatalog<WordAsset>
    {
        /// <inheritdoc/>
        protected override string IdOf(WordAsset asset)
        {
            return asset.Id;
        }
    }
}
