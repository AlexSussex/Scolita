namespace Scolita.Content
{
    /// <summary>Every letter asset, in the order of <c>letters.csv</c> (<c>Content/LetterCatalog.asset</c>).</summary>
    public sealed class LetterCatalog : ContentCatalog<LetterAsset>
    {
        /// <inheritdoc/>
        protected override string IdOf(LetterAsset asset)
        {
            return asset.Id;
        }
    }
}
