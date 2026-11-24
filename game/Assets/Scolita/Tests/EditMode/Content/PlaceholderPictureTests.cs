using NUnit.Framework;
using Scolita.Editor.ContentImport;
using UnityEngine;

namespace Scolita.Tests.EditMode.Content
{
    /// <summary>Tests <see cref="PlaceholderPicture"/>, which draws a picture with no art yet.</summary>
    public sealed class PlaceholderPictureTests
    {
        /// <summary>
        /// Tests that <see cref="PlaceholderPicture.Render"/> draws the same PNG every time: a plain
        /// circle in one colour, with clear corners and no letter (no ink) at its centre.
        /// </summary>
        [Test]
        public void TestRender_SamePlainCircleEveryTime()
        {
            byte[] first = PlaceholderPicture.Render("mother");
            byte[] second = PlaceholderPicture.Render("mother");

            Texture2D image = new Texture2D(2, 2);
            try
            {
                Assert.That(image.LoadImage(first), Is.True);
                Color32 corner = image.GetPixel(0, 0);
                Color32 centre = image.GetPixel(PlaceholderPicture.Size / 2, PlaceholderPicture.Size / 2);
                Color32 edge = image.GetPixel(PlaceholderPicture.Size / 2, 20);

                Assert.That(second, Is.EqualTo(first));
                Assert.That(image.width, Is.EqualTo(PlaceholderPicture.Size));
                Assert.That(corner.a, Is.EqualTo(0));
                Assert.That(edge.a, Is.EqualTo(255));
                Assert.That(centre, Is.EqualTo(edge));
                Assert.That(CountOtherOpaqueColours(image, edge), Is.EqualTo(0));
            }
            finally
            {
                Object.DestroyImmediate(image);
            }
        }

        private static int CountOtherOpaqueColours(Texture2D image, Color32 circle)
        {
            int count = 0;
            foreach (Color32 pixel in image.GetPixels32())
            {
                bool sameColour = pixel.r == circle.r && pixel.g == circle.g && pixel.b == circle.b;
                if (pixel.a > 0 && !sameColour)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
