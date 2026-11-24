using System;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace Scolita.Editor.Art
{
    /// <summary>
    /// Makes one sprite atlas (V2) per art folder in <see cref="ArtPaths.AtlasFolders"/>, each packing
    /// its whole folder, so a new sprite dropped into a folder is packed with no change here. An
    /// existing atlas is rewritten in place, keeping its GUID.
    /// </summary>
    public static class ArtAtlases
    {
        private const int MaxAtlasSize = 2048;
        private const int Padding = 4;

        /// <summary>
        /// Rebuilds every atlas. Called from the Editor's menu (Școlița → Rebuild sprite atlases) and
        /// in batch mode with <c>-executeMethod Scolita.Editor.Art.ArtAtlases.RebuildAll</c>.
        /// </summary>
        [MenuItem("Școlița/Rebuild sprite atlases")]
        public static void RebuildAll()
        {
            if (!AssetDatabase.IsValidFolder(ArtPaths.AtlasFolder))
            {
                AssetDatabase.CreateFolder(ArtPaths.Root.TrimEnd('/'), "Atlases");
            }

            foreach (string folder in ArtPaths.AtlasFolders)
            {
                Build(folder);
            }

            AssetDatabase.SaveAssets();
            Debug.Log("Sprite atlases rebuilt: " + string.Join(", ", ArtPaths.AtlasFolders) + ".");
        }

        private static void Build(string folder)
        {
            string folderPath = ArtPaths.Root + folder;
            DefaultAsset folderAsset = AssetDatabase.LoadAssetAtPath<DefaultAsset>(folderPath);
            if (folderAsset == null)
            {
                throw new InvalidOperationException("No art folder " + folderPath + ": run design/art/build.mjs first.");
            }

            string atlasPath = ArtPaths.AtlasPath(folder);
            SpriteAtlasAsset atlas = new SpriteAtlasAsset();
            atlas.Add(new UnityEngine.Object[] { folderAsset });
            SpriteAtlasAsset.Save(atlas, atlasPath);
            AssetDatabase.ImportAsset(atlasPath, ImportAssetOptions.ForceSynchronousImport);

            SpriteAtlasImporter importer = (SpriteAtlasImporter)AssetImporter.GetAtPath(atlasPath);
            importer.includeInBuild = true;
            importer.packingSettings = new SpriteAtlasPackingSettings
            {
                enableRotation = false,
                enableTightPacking = false,
                enableAlphaDilation = true,
                padding = Padding
            };
            importer.textureSettings = new SpriteAtlasTextureSettings
            {
                readable = false,
                generateMipMaps = false,
                sRGB = true,
                filterMode = FilterMode.Bilinear
            };
            TextureImporterPlatformSettings platform = importer.GetPlatformSettings("DefaultTexturePlatform");
            platform.maxTextureSize = MaxAtlasSize;
            platform.textureCompression = TextureImporterCompression.CompressedHQ;
            importer.SetPlatformSettings(platform);
            importer.SaveAndReimport();
        }
    }
}
