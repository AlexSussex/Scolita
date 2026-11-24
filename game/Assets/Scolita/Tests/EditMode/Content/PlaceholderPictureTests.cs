using NUnit.Framework;
using Scolita.Editor.ContentImport;
using UnityEngine;

namespace Scolita.Tests.EditMode.Content
{
    /// <summary>Tests <see cref="PlaceholderPicture"/>, which draws a picture with no art yet.</summary>
    public sealed class PlaceholderPictureTests
    {
        /// <summary>
        /// Tests that <see cref="PlaceholderPicture.Render"/> draws the same PNG every time: a coloured
        /// circle, clear corners and the letter in the ink at its centre.
        /// </summary>
        [Test]
        public void TestRender_SameCircleAndLetterEveryTime()
        {
            byte[] first = PlaceholderPicture.Render("mother", "m");
            byte[] second = PlaceholderPicture.Render("mother", "m");
            byte[] other = PlaceholderPicture.Render("mother", "a");

            Texture2D image = new Texture2D(2, 2);
            try
            {
                Assert.That(image.LoadImage(first), Is.True);
                Color32 corner = image.GetPixel(0, 0);
                int inkPixels = CountInk(image);
                Color32 edge = image.GetPixel(PlaceholderPicture.Size / 2, 20);

                Assert.That(second, Is.EqualTo(first));
                Assert.That(other, Is.Not.EqualTo(first));
                Assert.That(image.width, Is.EqualTo(PlaceholderPicture.Size));
                Assert.That(corner.a, Is.EqualTo(0));
                Assert.That(edge.a, Is.EqualTo(255));
                Assert.That(inkPixels, Is.GreaterThan(200));
            }
            finally
            {
                Object.DestroyImmediate(image);
            }
        }

        private static int CountInk(Texture2D image)
        {
            int count = 0;
            foreach (Color32 pixel in image.GetPixels32())
            {
                if (pixel.a == 255 && pixel.r < 0x40 && pixel.g < 0x40 && pixel.b < 0x60)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
