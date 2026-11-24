using System;

namespace Scolita.Core.Lessons
{
    /// <summary>
    /// The numbers of <c>LetterHunt</c> that she meets (part of <see cref="LearningRules"/>, filled from
    /// the learning rules asset): how many stars float by, how many carry the letter, and how fast they
    /// may move, so a star is never too fast to tap.
    /// </summary>
    public sealed class LetterHuntTuning
    {
        /// <summary>The fewest stars a hunt may have (the brief: 8 to 12).</summary>
        public const int FewestStars = 8;

        /// <summary>The most stars a hunt may have: the stars its view holds, made with its prefab.</summary>
        public const int MostStars = 12;

        /// <summary>
        /// Creates the numbers; throws an <see cref="ArgumentOutOfRangeException"/> for a number out of its
        /// range (<see cref="FewestStars"/> to <see cref="MostStars"/> stars, at least one with the letter and one without, a speed
        /// above 0). Called by the learning rules asset and the tests.
        /// </summary>
        public LetterHuntTuning(int stars, int rightStars, float fastestSpeed)
        {
            if (stars < FewestStars || stars > MostStars)
            {
                throw new ArgumentOutOfRangeException(nameof(stars), stars,
                    "A hunt has " + FewestStars + " to " + MostStars + " stars.");
            }

            if (rightStars < 1 || rightStars >= stars)
            {
                throw new ArgumentOutOfRangeException(nameof(rightStars), rightStars,
                    "At least one star has the letter, and at least one doesn't.");
            }

            if (!(fastestSpeed > 0f))
            {
                throw new ArgumentOutOfRangeException(nameof(fastestSpeed), fastestSpeed, "The stars must move.");
            }

            Stars = stars;
            RightStars = rightStars;
            FastestSpeed = fastestSpeed;
        }

        /// <summary>How many stars float by.</summary>
        public int Stars { get; }

        /// <summary>How many of them carry the letter to find, half of those as its capital (4).</summary>
        public int RightStars { get; }

        /// <summary>
        /// The fastest a star may drift, in the lesson screen's reference pixels (its 1280 × 800 canvas)
        /// per second, so the same share of the screen on every size.
        /// </summary>
        public float FastestSpeed { get; }
    }
}
