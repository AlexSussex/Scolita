using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore;
using Object = UnityEngine.Object;

namespace Scolita.Editor.ContentImport
{
    /// <summary>
    /// Draws the placeholder of a picture with no art yet: a circle in one of the rainbow path's
    /// colours (picked from the picture's id, so it never changes) with a letter in Andika, in the
    /// ink <c>#2A2140</c> (design/README.md → "Palette"). The letter is read on the CPU from the
    /// reading font's distance-field atlas, so it works in batch mode without a graphics device.
    /// </summary>
    public static class PlaceholderPicture
    {
        /// <summary>The placeholder's width and height, in pixels.</summary>
        public const int Size = 256;

        private const string FontPath = "Assets/Scolita/Fonts/Andika-Regular SDF.asset";
        private const float CircleRadius = 120f;
        private const float LetterScale = 2.2f;
        private const float EdgeSoftness = 0.03f;
        private const float EdgeValue = 0.5f;

        private static readonly Color32 Ink = new Color32(0x2A, 0x21, 0x40, 0xFF);

        private static readonly Color32[] Circles =
        {
            new Color32(0xFF, 0x5C, 0x8A, 0xFF),
            new Color32(0xFF, 0x9F, 0x43, 0xFF),
            new Color32(0xFF, 0xD9, 0x3D, 0xFF),
            new Color32(0x4C, 0xD9, 0x7B, 0xFF),
            new Color32(0x3B, 0xA7, 0xF0, 0xFF),
            new Color32(0x9B, 0x6C, 0xFF, 0xFF)
        };

        /// <summary>
        /// The PNG of the placeholder for the picture <paramref name="pictureId"/>, showing the first
        /// character of <paramref name="letter"/> (nothing for an empty string). Throws an
        /// <see cref="InvalidOperationException"/> for a character the reading font's atlas lacks.
        /// Called by <see cref="ContentImportPlan"/> and <c>PlaceholderPictureTests</c>.
        /// </summary>
        public static byte[] Render(string pictureId, string letter)
        {
            Color32[] pixels = Circle(CircleColour(pictureId));
            if (!string.IsNullOrEmpty(letter))
            {
                DrawLetter(pixels, letter[0]);
            }

            Texture2D image = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
            try
            {
                image.SetPixels32(pixels);
                image.Apply(false);
                return image.EncodeToPNG();
            }
            finally
            {
                Object.DestroyImmediate(image);
            }
        }

        private static Color32 CircleColour(string pictureId)
        {
            int hash = 0;
            foreach (char character in pictureId)
            {
                hash = unchecked(hash * 31 + character);
            }

            return Circles[(hash & int.MaxValue) % Circles.Length];
        }

        private static Color32[] Circle(Color32 colour)
        {
            Color32[] pixels = new Color32[Size * Size];
            float centre = (Size - 1) / 2f;
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    float distance = Mathf.Sqrt((x - centre) * (x - centre) + (y - centre) * (y - centre));
                    float coverage = Mathf.Clamp01(CircleRadius - distance + 0.5f);
                    pixels[y * Size + x] = new Color32(colour.r, colour.g, colour.b, (byte)Mathf.RoundToInt(coverage * 255f));
                }
            }

            return pixels;
        }

        private static void DrawLetter(Color32[] pixels, char letter)
        {
            TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
            if (font == null || !font.characterLookupTable.TryGetValue(letter, out TMP_Character character))
            {
                throw new InvalidOperationException("The reading font " + FontPath + " has no '" + letter
                    + "' for the placeholder pictures (add it to FontCharacterSet and rebuild the fonts).");
            }

            Texture2D atlas = font.atlasTexture;
            int padding = font.atlasPadding;
            GlyphRect rect = character.glyph.glyphRect;
            float sourceWidth = rect.width + 2 * padding;
            float sourceHeight = rect.height + 2 * padding;
            int width = Mathf.RoundToInt(sourceWidth * LetterScale);
            int height = Mathf.RoundToInt(sourceHeight * LetterScale);
            int left = (Size - width) / 2;
            int bottom = (Size - height) / 2;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float u = (rect.x - padding + (x + 0.5f) / LetterScale) / atlas.width;
                    float v = (rect.y - padding + (y + 0.5f) / LetterScale) / atlas.height;
                    float distance = atlas.GetPixelBilinear(u, v).a;
                    float coverage = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(EdgeValue - EdgeSoftness, EdgeValue + EdgeSoftness, distance));
                    int index = (bottom + y) * Size + left + x;
                    pixels[index] = Color32.Lerp(pixels[index], Ink, coverage);
                }
            }
        }
    }
}
