namespace Scolita.Content
{
    /// <summary>Every domain asset, in the order their first unit appears in <c>units.csv</c> (<c>Content/DomainCatalog.asset</c>).</summary>
    public sealed class DomainCatalog : ContentCatalog<DomainAsset>
    {
        /// <inheritdoc/>
        protected override string IdOf(DomainAsset asset)
        {
            return asset.Id;
        }
    }
}
