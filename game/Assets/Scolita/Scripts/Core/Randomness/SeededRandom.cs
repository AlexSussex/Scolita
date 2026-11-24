using System;

namespace Scolita.Core.Randomness
{
    /// <summary>
    /// A small deterministic random generator (SplitMix64, the xorshift-multiply generator
    /// SeliSnake uses): the same seed gives the same sequence on every runtime and device, unlike
    /// <see cref="System.Random"/>. Created by the Unity side when it builds a lesson, and by
    /// <c>SeededRandomTests</c>.
    /// </summary>
    public sealed class SeededRandom : IRandom
    {
        private const ulong GoldenGamma = 0x9E3779B97F4A7C15UL;
        private const ulong MixMultiplierA = 0xBF58476D1CE4E5B9UL;
        private const ulong MixMultiplierB = 0x94D049BB133111EBUL;

        private ulong state;

        /// <summary>
        /// Starts the sequence from <paramref name="seed"/>; every seed, including 0, is valid.
        /// Called when a lesson is built and by <c>SeededRandomTests</c>.
        /// </summary>
        public SeededRandom(ulong seed)
        {
            state = seed;
        }

        /// <inheritdoc/>
        public int NextInt(int minInclusive, int maxExclusive)
        {
            if (maxExclusive <= minInclusive)
            {
                throw new ArgumentOutOfRangeException(nameof(maxExclusive), maxExclusive,
                    "The upper bound must be greater than the lower bound " + minInclusive + ".");
            }

            ulong range = (ulong)((long)maxExclusive - minInclusive);
            ulong high32 = NextUInt64() >> 32;
            return (int)(minInclusive + (long)((high32 * range) >> 32));
        }

        private ulong NextUInt64()
        {
            state += GoldenGamma;
            ulong mixed = state;
            mixed = (mixed ^ (mixed >> 30)) * MixMultiplierA;
            mixed = (mixed ^ (mixed >> 27)) * MixMultiplierB;
            return mixed ^ (mixed >> 31);
        }
    }
}
