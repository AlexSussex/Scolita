namespace Scolita.Content
{
    /// <summary>Every constellation asset, in the order of <c>constellations.csv</c> (<c>Content/ConstellationCatalog.asset</c>).</summary>
    public sealed class ConstellationCatalog : ContentCatalog<ConstellationAsset>
    {
        /// <inheritdoc/>
        protected override string IdOf(ConstellationAsset asset)
        {
            return asset.Id;
        }
    }
}
