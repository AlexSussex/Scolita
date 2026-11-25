using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Scolita.Editor.Art;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.U2D;

namespace Scolita.Tests.EditMode.Art
{
    /// <summary>
    /// Tests the app's art as imported by <see cref="ArtTextureImporter"/> and packed by
    /// <see cref="ArtAtlases"/>: every sprite is in exactly one atlas, the
    /// stretchable pieces have their 9-slice borders, every asset reference in the project
    /// resolves, and the launcher icon is set.
    /// </summary>
    public sealed class ArtTests
    {
        private static readonly string[] ReferringFolders =
        {
            "Assets/Scolita/Prefabs", "Assets/Scolita/Scenes", "Assets/Scolita/Content", ArtPaths.Root
        };
        private static readonly Regex GuidReference = new Regex("guid: ([0-9a-f]{32})", RegexOptions.Compiled);

        /// <summary>
        /// Tests that <see cref="ArtTextureImporter"/> makes every PNG under <see cref="ArtPaths.Root"/>
        /// (but the launcher icons) a single sprite.
        /// </summary>
        [Test]
        public void TestOnPreprocessTexture_EveryArtPngIsASprite()
        {
            List<string> paths = SpritePaths().ToList();

            Assert.That(paths, Is.Not.Empty);
            foreach (string path in paths)
            {
                TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
                Assert.That(importer.textureType, Is.EqualTo(TextureImporterType.Sprite), path);
                Assert.That(importer.spriteImportMode, Is.EqualTo(SpriteImportMode.Single), path);
                Assert.That(importer.mipmapEnabled, Is.False, path);
            }
        }

        /// <summary>
        /// Tests that every art folder holding sprites has its atlas from <see cref="ArtAtlases.RebuildAll"/>,
        /// and is one of <see cref="ArtPaths.AtlasFolders"/>, so a rebuild keeps it.
        /// </summary>
        [Test]
        public void TestRebuildAll_EachArtFolderHasItsAtlas()
        {
            List<string> folders = SpriteFolders().ToList();

            Assert.That(folders, Is.Not.Empty);
            foreach (string folder in folders)
            {
                Assert.That(ArtPaths.AtlasFolders, Does.Contain(folder), folder + " must be listed in ArtPaths.AtlasFolders.");
                Assert.That(AssetDatabase.LoadAssetAtPath<SpriteAtlas>(ArtPaths.AtlasPath(folder)), Is.Not.Null,
                    folder + " has no atlas; run Școlița → Rebuild sprite atlases.");
            }
        }

        /// <summary>
        /// Tests that the atlases built by <see cref="ArtAtlases.RebuildAll"/> hold every sprite exactly
        /// once (<see cref="SpriteAtlas.CanBindTo"/>), so nothing draws outside an atlas.
        /// </summary>
        [Test]
        public void TestRebuildAll_EverySpriteIsInExactlyOneAtlas()
        {
            List<SpriteAtlas> atlases = AssetDatabase.FindAssets("t:SpriteAtlas", new[] { ArtPaths.AtlasFolder })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<SpriteAtlas>)
                .ToList();

            Assert.That(atlases, Is.Not.Empty);
            foreach (string path in SpritePaths())
            {
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                int count = atlases.Count(atlas => atlas.CanBindTo(sprite));
                Assert.That(count, Is.EqualTo(1), path + " must be in exactly one atlas.");
            }
        }

        /// <summary>
        /// Tests that <see cref="ArtTextureImporter"/> gives each stretchable piece the border listed
        /// in <see cref="SpriteBorderTable"/>.
        /// </summary>
        [Test]
        public void TestOnPreprocessTexture_StretchablePiecesHaveTheirBorders()
        {
            SpriteBorderTable table = SpriteBorderTable.Load(ArtPaths.BordersFile);

            Assert.That(table.Names, Is.Not.Empty);
            foreach (string name in table.Names)
            {
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPaths.Root + name + ".png");
                Assert.That(sprite, Is.Not.Null, name);
                Assert.That(sprite.border, Is.EqualTo(table.BorderFor(name)), name);
            }
        }

        /// <summary>
        /// Tests that every asset the scenes, prefabs, content assets and atlases refer to by GUID
        /// still exists (<see cref="AssetDatabase.GUIDToAssetPath(string)"/>), so removing a sprite
        /// that something still shows fails here, not on the tablet.
        /// </summary>
        [Test]
        public void TestGUIDToAssetPath_EveryReferenceResolves()
        {
            string[] files = ReferringFolders
                .SelectMany(folder => Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories))
                .Where(f => f.EndsWith(".prefab") || f.EndsWith(".unity") || f.EndsWith(".asset") || f.EndsWith(".spriteatlasv2"))
                .ToArray();

            Assert.That(files, Is.Not.Empty);
            foreach (string file in files)
            {
                foreach (Match match in GuidReference.Matches(File.ReadAllText(file)))
                {
                    string guid = match.Groups[1].Value;
                    if (guid.StartsWith("0000000000000000"))
                    {
                        continue;
                    }

                    Assert.That(AssetDatabase.GUIDToAssetPath(guid), Is.Not.Empty, file + " refers to a missing asset " + guid + ".");
                }
            }
        }

        /// <summary>
        /// Tests that the Android adaptive launcher icon (<see cref="PlayerSettings.GetPlatformIcons"/>)
        /// has its two layers from <see cref="ArtPaths.IconFolder"/> at every size.
        /// </summary>
        [Test]
        public void TestGetPlatformIcons_AdaptiveIconHasBothLayers()
        {
            PlatformIcon[] icons = PlayerSettings.GetPlatformIcons(NamedBuildTarget.Android, AndroidPlatformIconKind.Adaptive);

            Assert.That(icons, Is.Not.Empty);
            foreach (PlatformIcon icon in icons)
            {
                Texture2D[] layers = icon.GetTextures();
                Assert.That(layers.Length, Is.EqualTo(2));
                foreach (Texture2D layer in layers)
                {
                    Assert.That(layer, Is.Not.Null, icon.width + " px");
                    Assert.That(AssetDatabase.GetAssetPath(layer), Does.StartWith(ArtPaths.IconFolder));
                }
            }
        }

        private static IEnumerable<string> SpritePaths()
        {
            return Directory.GetFiles(ArtPaths.Root, "*.png", SearchOption.AllDirectories)
                .Select(p => p.Replace('\\', '/'))
                .Where(p => !ArtPaths.IsIcon(p))
                .OrderBy(p => p, System.StringComparer.Ordinal);
        }

        private static IEnumerable<string> SpriteFolders()
        {
            return SpritePaths()
                .Select(FolderOf)
                .Distinct();
        }

        private static string FolderOf(string spritePath)
        {
            string relative = spritePath.Substring(ArtPaths.Root.Length);
            return relative.Substring(0, relative.IndexOf('/'));
        }
    }
}
