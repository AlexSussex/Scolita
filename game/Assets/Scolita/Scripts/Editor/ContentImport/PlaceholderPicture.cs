using UnityEngine;
using Object = UnityEngine.Object;

namespace Scolita.Editor.ContentImport
{
    /// <summary>
    /// Draws the placeholder of a picture with no art yet: a plain circle in one of the rainbow path's
    /// colours (design/README.md → "Palette"), picked from the picture's id so it never changes. It
    /// shows no letter, so it never gives a <c>FirstSound</c> answer away (the human, 2026-11-24).
    /// </summary>
    public static class PlaceholderPicture
    {
        /// <summary>The placeholder's width and height, in pixels.</summary>
        public const int Size = 256;

        private const float CircleRadius = 120f;

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
        /// The PNG of the placeholder for the picture <paramref name="pictureId"/>. Called by
        /// <see cref="ContentImportPlan"/> and <c>PlaceholderPictureTests</c>.
        /// </summary>
        public static byte[] Render(string pictureId)
        {
            Texture2D image = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
            try
            {
                image.SetPixels32(Circle(CircleColour(pictureId)));
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
    }
}
