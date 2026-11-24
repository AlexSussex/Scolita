using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace Scolita.Editor.Fonts
{
    /// <summary>
    /// Makes the app's TextMeshPro font assets from the font files in <c>Assets/ThirdParty</c>:
    /// Andika for the letters and words she reads, Baloo 2 for the app's own words. Each is a static
    /// SDF atlas holding <see cref="FontCharacterSet.All"/>, so a missing letter is found here, never
    /// on the tablet. An existing asset is rebuilt in place, keeping its GUID and the references to it.
    /// </summary>
    public static class FontAssetBuilder
    {
        private static readonly IReadOnlyList<FontAssetSpec> Specs = new[]
        {
            new FontAssetSpec("Assets/ThirdParty/Andika/Andika-Regular.ttf", "Assets/Scolita/Fonts/Andika-Regular SDF.asset",
                72, 8, 1024),
            new FontAssetSpec("Assets/ThirdParty/Baloo2/Baloo2-Variable.ttf", "Assets/Scolita/Fonts/Baloo2 SDF.asset",
                72, 8, 1024)
        };

        /// <summary>
        /// Rebuilds every font asset; throws an <see cref="InvalidOperationException"/> naming any
        /// character a font lacks or that didn't fit its atlas. Called from the Editor's menu
        /// (Școlița → Rebuild font assets) and in batch mode with <c>-executeMethod</c>.
        /// </summary>
        [MenuItem("\u0218coli\u021Ba/Rebuild font assets")]
        public static void RebuildAll()
        {
            string characters = FontCharacterSet.All();
            foreach (FontAssetSpec spec in Specs)
            {
                Build(spec, characters);
            }

            AssetDatabase.SaveAssets();
        }

        private static void Build(FontAssetSpec spec, string characters)
        {
            Font font = AssetDatabase.LoadAssetAtPath<Font>(spec.SourceFontPath);
            if (font == null)
            {
                throw new InvalidOperationException("No font at " + spec.SourceFontPath + ".");
            }

            TMP_FontAsset existing = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(spec.AssetPath);
            TMP_FontAsset asset = existing != null ? existing : Create(font, spec);
            asset.atlasPopulationMode = AtlasPopulationMode.Dynamic;
            asset.ClearFontAssetData();
            if (!asset.TryAddCharacters(characters, out string missing))
            {
                throw new InvalidOperationException(spec.SourceFontPath + " can't hold these characters in its "
                    + spec.AtlasSize + " px atlas, or lacks them: '" + missing + "'.");
            }

            asset.atlasPopulationMode = AtlasPopulationMode.Static;
            EditorUtility.SetDirty(asset);
            EditorUtility.SetDirty(asset.atlasTexture);
        }

        private static TMP_FontAsset Create(Font font, FontAssetSpec spec)
        {
            TMP_FontAsset asset = TMP_FontAsset.CreateFontAsset(font, spec.SamplingPointSize, spec.AtlasPadding,
                GlyphRenderMode.SDFAA, spec.AtlasSize, spec.AtlasSize, AtlasPopulationMode.Dynamic, false);
            if (asset == null)
            {
                throw new InvalidOperationException("TextMeshPro could not read " + spec.SourceFontPath + ".");
            }

            AssetDatabase.CreateAsset(asset, spec.AssetPath);
            AssetDatabase.AddObjectToAsset(asset.atlasTexture, asset);
            AssetDatabase.AddObjectToAsset(asset.material, asset);
            return asset;
        }
    }
}
