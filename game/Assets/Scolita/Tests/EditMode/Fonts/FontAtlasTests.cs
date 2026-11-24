using NUnit.Framework;
using TMPro;
using UnityEditor;

namespace Scolita.Tests.EditMode.Fonts
{
    /// <summary>
    /// Tests the app's two TextMeshPro font assets (<see cref="TMP_FontAsset"/>), Andika for reading
    /// and Baloo 2 for the app's words: both atlases hold the ten Romanian letters, ș ț with the
    /// comma below.
    /// </summary>
    public sealed class FontAtlasTests
    {
        /// <summary>
        /// Tests that <see cref="TMP_FontAsset.characterLookupTable"/> holds ă â î ș ț Ă Â Î Ș Ț, each
        /// with a glyph drawn in the atlas.
        /// </summary>
        [TestCase("Assets/Scolita/Fonts/Andika-Regular SDF.asset")]
        [TestCase("Assets/Scolita/Fonts/Baloo2 SDF.asset")]
        public void TestCharacterLookupTable_HoldsTheTenRomanianLetters(string path)
        {
            const string romanianLetters = "\u0103\u00E2\u00EE\u0219\u021B\u0102\u00C2\u00CE\u0218\u021A";
            TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);

            Assert.That(font, Is.Not.Null, path + " must exist.");
            Assert.That(font.atlasPopulationMode, Is.EqualTo(AtlasPopulationMode.Static));
            foreach (char letter in romanianLetters)
            {
                bool found = font.characterLookupTable.TryGetValue(letter, out TMP_Character character);
                Assert.That(found, Is.True, path + " lacks U+" + ((int)letter).ToString("X4") + " " + letter);
                Assert.That(character.glyph.glyphRect.width, Is.GreaterThan(0), "U+" + ((int)letter).ToString("X4"));
                Assert.That(character.glyph.glyphRect.height, Is.GreaterThan(0), "U+" + ((int)letter).ToString("X4"));
            }
        }

        /// <summary>
        /// Tests that <see cref="TMP_FontAsset.characterLookupTable"/> holds the comma-below ș ț Ș Ț
        /// and not the cedilla ş ţ Ş Ţ, so a cedilla that slips past the validator shows as missing.
        /// </summary>
        [TestCase("Assets/Scolita/Fonts/Andika-Regular SDF.asset")]
        [TestCase("Assets/Scolita/Fonts/Baloo2 SDF.asset")]
        public void TestCharacterLookupTable_CommaBelowNotCedilla(string path)
        {
            TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);

            Assert.That(font.characterLookupTable.ContainsKey(0x0219), Is.True);
            Assert.That(font.characterLookupTable.ContainsKey(0x021B), Is.True);
            Assert.That(font.characterLookupTable.ContainsKey(0x0218), Is.True);
            Assert.That(font.characterLookupTable.ContainsKey(0x021A), Is.True);
            Assert.That(font.characterLookupTable.ContainsKey(0x015F), Is.False);
            Assert.That(font.characterLookupTable.ContainsKey(0x0163), Is.False);
            Assert.That(font.characterLookupTable.ContainsKey(0x015E), Is.False);
            Assert.That(font.characterLookupTable.ContainsKey(0x0162), Is.False);
        }
    }
}
