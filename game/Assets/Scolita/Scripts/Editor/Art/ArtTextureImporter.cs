using UnityEditor;
using UnityEngine;

namespace Scolita.Editor.Art
{
    /// <summary>
    /// Imports the art's PNGs the same way every time: each one under <see cref="ArtPaths.Root"/> is
    /// a single, full-rect sprite with no mipmaps (packed by its folder's atlas, <see cref="ArtAtlases"/>),
    /// with its 9-slice border from <see cref="SpriteBorderTable"/>; the launcher icons are plain,
    /// uncompressed textures for Player Settings. Unity calls it on every import of a texture.
    /// </summary>
    public sealed class ArtTextureImporter : AssetPostprocessor
    {
        private const int SpritePixelsPerUnit = 100;
        private const int MaxTextureSize = 2048;
        private const int IconMaxTextureSize = 1024;

        /// <summary>
        /// The rules' version: raising it makes Unity import the art again with the new rules.
        /// Called by Unity's asset database.
        /// </summary>
        public override uint GetVersion()
        {
            return 1;
        }

        private void OnPreprocessTexture()
        {
            if (!ArtPaths.IsArt(assetPath))
            {
                return;
            }

            TextureImporter importer = (TextureImporter)assetImporter;
            if (ArtPaths.IsIcon(assetPath))
            {
                ApplyIconSettings(importer);
                return;
            }

            ApplySpriteSettings(importer, SpriteBorderTable.Load(ArtPaths.BordersFile).BorderFor(ArtPaths.ArtName(assetPath)));
        }

        private static void ApplySpriteSettings(TextureImporter importer, Vector4 border)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = SpritePixelsPerUnit;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.sRGBTexture = true;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.filterMode = FilterMode.Bilinear;
            importer.maxTextureSize = MaxTextureSize;
            importer.isReadable = false;

            TextureImporterSettings settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            settings.spriteAlignment = (int)SpriteAlignment.Center;
            settings.spriteBorder = border;
            settings.spriteGenerateFallbackPhysicsShape = false;
            importer.SetTextureSettings(settings);
        }

        private static void ApplyIconSettings(TextureImporter importer)
        {
            importer.textureType = TextureImporterType.Default;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.sRGBTexture = true;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.maxTextureSize = IconMaxTextureSize;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.isReadable = false;
        }
    }
}
