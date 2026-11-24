namespace Scolita.Content
{
    /// <summary>Every voice line asset, both languages, in the order of <c>voice-lines.csv</c> (<c>Content/VoiceLineCatalog.asset</c>).</summary>
    public sealed class VoiceLineCatalog : ContentCatalog<VoiceLineAsset>
    {
        /// <inheritdoc/>
        protected override string IdOf(VoiceLineAsset asset)
        {
            return asset.Id;
        }
    }
}
