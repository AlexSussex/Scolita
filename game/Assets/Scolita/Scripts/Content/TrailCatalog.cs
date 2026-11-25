namespace Scolita.Content
{
    /// <summary>Every trail asset, in the order of <c>trails.csv</c> (<c>Content/TrailCatalog.asset</c>).</summary>
    public sealed class TrailCatalog : ContentCatalog<TrailAsset>
    {
        /// <inheritdoc/>
        protected override string IdOf(TrailAsset asset)
        {
            return asset.Id;
        }
    }
}
