using System;

namespace Scolita.Core.Rewards
{
    /// <summary>
    /// The numbers of the rewards, from the reward rules asset (<c>Settings/RewardRules.asset</c>), in one place:
    /// the stars a finished lesson gives (the stars lines in the content say this number), every how many finished
    /// lessons a sticker comes, and how small and how large a sticker can be made on the scene (1 is its size when
    /// placed).
    /// </summary>
    public sealed class RewardRules
    {
        /// <summary>
        /// Creates the rules; throws an <see cref="ArgumentOutOfRangeException"/> for a count under 1, or sizes that
        /// aren't above 0 with the smallest at most the largest. Called by <c>RewardRulesAsset</c> and the tests.
        /// </summary>
        public RewardRules(int starsPerLesson, int lessonsPerSticker, float smallestStickerSize, float largestStickerSize)
        {
            if (starsPerLesson < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(starsPerLesson), "A lesson gives at least one star.");
            }

            if (lessonsPerSticker < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(lessonsPerSticker), "A sticker comes every one lesson or more.");
            }

            if (smallestStickerSize <= 0f || largestStickerSize < smallestStickerSize)
            {
                throw new ArgumentOutOfRangeException(nameof(smallestStickerSize),
                    "A sticker's sizes are above 0, the smallest at most the largest.");
            }

            StarsPerLesson = starsPerLesson;
            LessonsPerSticker = lessonsPerSticker;
            SmallestStickerSize = smallestStickerSize;
            LargestStickerSize = largestStickerSize;
        }

        /// <summary>The stars every finished lesson gives, whatever the hints.</summary>
        public int StarsPerLesson { get; }

        /// <summary>A sticker comes when the count of finished lessons is a multiple of this (and at a planet's end).</summary>
        public int LessonsPerSticker { get; }

        /// <summary>The smallest a sticker can be made on the scene, 1 being its size when placed.</summary>
        public float SmallestStickerSize { get; }

        /// <summary>The largest a sticker can be made on the scene, 1 being its size when placed.</summary>
        public float LargestStickerSize { get; }

        /// <summary>
        /// <paramref name="size"/> kept between <see cref="SmallestStickerSize"/> and <see cref="LargestStickerSize"/>
        /// (1 for a number that isn't one). Called by <see cref="RewardBook"/> and the sticker scene.
        /// </summary>
        public float ClampSize(float size)
        {
            if (float.IsNaN(size) || float.IsInfinity(size))
            {
                return 1f;
            }

            return Math.Min(LargestStickerSize, Math.Max(SmallestStickerSize, size));
        }
    }
}
