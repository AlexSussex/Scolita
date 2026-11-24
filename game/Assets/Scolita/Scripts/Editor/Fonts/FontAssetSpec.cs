namespace Scolita.Editor.Fonts
{
    /// <summary>
    /// One TextMeshPro font asset that <see cref="FontAssetBuilder"/> makes: its source font file,
    /// where the asset goes, and how its SDF atlas is sampled.
    /// </summary>
    public readonly struct FontAssetSpec
    {
        /// <summary>Creates a spec; called by <see cref="FontAssetBuilder"/>.</summary>
        public FontAssetSpec(string sourceFontPath, string assetPath, int samplingPointSize, int atlasPadding, int atlasSize)
        {
            SourceFontPath = sourceFontPath;
            AssetPath = assetPath;
            SamplingPointSize = samplingPointSize;
            AtlasPadding = atlasPadding;
            AtlasSize = atlasSize;
        }

        /// <summary>The font file (<c>.ttf</c>) under <c>Assets/ThirdParty</c>.</summary>
        public string SourceFontPath { get; }

        /// <summary>The font asset's path under <c>Assets/Scolita/Fonts</c>.</summary>
        public string AssetPath { get; }

        /// <summary>The point size the glyphs are rendered at into the atlas.</summary>
        public int SamplingPointSize { get; }

        /// <summary>The padding around each glyph in the atlas, in pixels: the SDF's spread.</summary>
        public int AtlasPadding { get; }

        /// <summary>The atlas texture's width and height, in pixels.</summary>
        public int AtlasSize { get; }
    }
}
