using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Scolita.Editor.Art;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace Scolita.Tests.EditMode.Art
{
    /// <summary>
    /// Tests the app's art as imported by <see cref="ArtTextureImporter"/> and packed by
    /// <see cref="ArtAtlases"/>: every sprite is in exactly one atlas, the stretchable pieces have
    /// their 9-slice borders, every sprite a prefab shows resolves, and the launcher icon is set.
    /// </summary>
    public sealed class ArtTests
    {
        private const string PrefabFolder = "Assets/Scolita/Prefabs";

        /// <summary>
        /// Tests that <see cref="ArtTextureImporter"/> makes every PNG under <see cref="ArtPaths.Root"/>
        /// (but the launcher icons) a single sprite.
        /// </summary>
        [Test]
        public void TestOnPreprocessTexture_EveryArtPngIsASprite()
        {
            List<string> paths = ArtPngPaths().Where(p => !ArtPaths.IsIcon(p)).ToList();

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
        /// Tests that the atlases built by <see cref="ArtAtlases.RebuildAll"/> hold every sprite exactly
        /// once (<see cref="SpriteAtlas.CanBindTo"/>), so nothing draws outside an atlas.
        /// </summary>
        [Test]
        public void TestRebuildAll_EverySpriteIsInExactlyOneAtlas()
        {
            List<SpriteAtlas> atlases = ArtPaths.AtlasFolders
                .Select(folder => AssetDatabase.LoadAssetAtPath<SpriteAtlas>(ArtPaths.AtlasPath(folder)))
                .ToList();

            Assert.That(atlases, Has.None.Null, "Every art folder has its atlas; run Școlița → Rebuild sprite atlases.");
            foreach (string path in ArtPngPaths().Where(p => !ArtPaths.IsIcon(p)))
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
        /// Tests that every <see cref="Image"/> and <see cref="SpriteRenderer"/> in the app's prefabs
        /// refers to a sprite that exists (none missing).
        /// </summary>
        [Test]
        public void TestPrefabs_EverySpriteReferenceResolves()
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { PrefabFolder });

            Assert.That(guids, Is.Not.Empty);
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                foreach (Component component in prefab.GetComponentsInChildren<Component>(true))
                {
                    if (component is Image || component is SpriteRenderer)
                    {
                        SerializedProperty sprite = new SerializedObject(component).FindProperty("m_Sprite");
                        bool missing = sprite.objectReferenceValue == null && sprite.objectReferenceInstanceIDValue != 0;
                        Assert.That(missing, Is.False, path + ": " + component.name + " shows a missing sprite.");
                    }
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

        private static IEnumerable<string> ArtPngPaths()
        {
            return Directory.GetFiles(ArtPaths.Root, "*.png", SearchOption.AllDirectories)
                .Select(p => p.Replace('\\', '/'))
                .OrderBy(p => p, System.StringComparer.Ordinal);
        }
    }
}
