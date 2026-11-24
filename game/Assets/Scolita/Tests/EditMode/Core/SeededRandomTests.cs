using System;
using NUnit.Framework;
using Scolita.Core.Randomness;

namespace Scolita.Tests.EditMode.Core
{
    /// <summary>Tests <see cref="SeededRandom"/>.</summary>
    public sealed class SeededRandomTests
    {
        /// <summary>Tests that <see cref="SeededRandom.NextInt"/> gives the same sequence for the same seed.</summary>
        [Test]
        public void TestNextInt_SameSeedGivesSameSequence()
        {
            SeededRandom first = new SeededRandom(12345UL);
            SeededRandom second = new SeededRandom(12345UL);

            for (int i = 0; i < 10000; i++)
            {
                Assert.That(second.NextInt(0, 1000), Is.EqualTo(first.NextInt(0, 1000)));
            }
        }

        /// <summary>Tests that <see cref="SeededRandom.NextInt"/> gives different sequences for different seeds.</summary>
        [Test]
        public void TestNextInt_DifferentSeedsGiveDifferentSequences()
        {
            SeededRandom first = new SeededRandom(1UL);
            SeededRandom second = new SeededRandom(2UL);
            int differences = 0;

            for (int i = 0; i < 100; i++)
            {
                if (first.NextInt(0, 1000000) != second.NextInt(0, 1000000))
                {
                    differences++;
                }
            }

            Assert.That(differences, Is.GreaterThan(90));
        }

        /// <summary>
        /// Tests that <see cref="SeededRandom.NextInt"/> keeps its known sequence (SplitMix64's, the
        /// same as SeliSnake's), so a change to the generator, which would change every seeded
        /// lesson, is caught.
        /// </summary>
        [Test]
        public void TestNextInt_KnownSeedGivesKnownSequence()
        {
            SeededRandom random = new SeededRandom(42UL);
            int[] expected = { 741, 159, 278, 344, 38 };

            int[] actual = new int[expected.Length];
            for (int i = 0; i < actual.Length; i++)
            {
                actual[i] = random.NextInt(0, 1000);
            }

            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>Tests that <see cref="SeededRandom.NextInt"/> stays within its bounds and reaches both ends.</summary>
        [Test]
        public void TestNextInt_StaysWithinBoundsAndReachesBothEnds()
        {
            SeededRandom random = new SeededRandom(7UL);
            bool sawMin = false;
            bool sawMax = false;

            for (int i = 0; i < 10000; i++)
            {
                int value = random.NextInt(-3, 4);
                Assert.That(value, Is.InRange(-3, 3));
                sawMin |= value == -3;
                sawMax |= value == 3;
            }

            Assert.That(sawMin, Is.True);
            Assert.That(sawMax, Is.True);
        }

        /// <summary>Tests that <see cref="SeededRandom.NextInt"/> spreads its values evenly over a few answer cards.</summary>
        [Test]
        public void TestNextInt_SpreadsEvenlyOverAnswerCards()
        {
            SeededRandom random = new SeededRandom(2026UL);
            int[] counts = new int[4];

            for (int i = 0; i < 40000; i++)
            {
                counts[random.NextInt(0, counts.Length)]++;
            }

            foreach (int count in counts)
            {
                Assert.That(count, Is.InRange(9500, 10500));
            }
        }

        /// <summary>Tests that <see cref="SeededRandom.NextInt"/> with a single value always gives it, even from seed 0.</summary>
        [Test]
        public void TestNextInt_SingleValueRangeFromSeedZero()
        {
            SeededRandom random = new SeededRandom(0UL);

            for (int i = 0; i < 100; i++)
            {
                Assert.That(random.NextInt(5, 6), Is.EqualTo(5));
            }
        }

        /// <summary>Tests that <see cref="SeededRandom.NextInt"/> refuses an empty or reversed range.</summary>
        [Test]
        public void TestNextInt_EmptyOrReversedRangeThrows()
        {
            SeededRandom random = new SeededRandom(1UL);

            Assert.That(() => random.NextInt(3, 3), Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => random.NextInt(4, 2), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        /// <summary>Tests that <see cref="SeededRandom.NextInt"/> handles the widest range without overflowing.</summary>
        [Test]
        public void TestNextInt_WidestRangeStaysInBounds()
        {
            SeededRandom random = new SeededRandom(99UL);

            for (int i = 0; i < 1000; i++)
            {
                Assert.That(random.NextInt(int.MinValue, int.MaxValue), Is.LessThan(int.MaxValue));
            }
        }
    }
}
