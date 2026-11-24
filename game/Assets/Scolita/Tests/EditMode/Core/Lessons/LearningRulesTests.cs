using System;
using NUnit.Framework;
using Scolita.Core.Lessons;

namespace Scolita.Tests.EditMode.Core.Lessons
{
    /// <summary>Tests <see cref="LearningRules"/> and <see cref="LetterHuntTuning"/>.</summary>
    public sealed class LearningRulesTests
    {
        /// <summary>Tests that the <see cref="LearningRules"/> constructor keeps its numbers and refuses ones out of range.</summary>
        [Test]
        public void TestLearningRules_Ranges()
        {
            LetterHuntTuning hunt = new LetterHuntTuning(10, 4, 60f);

            LearningRules rules = new LearningRules(2, 3, hunt);

            Assert.That(rules.TriesBeforeAnswerShown, Is.EqualTo(2));
            Assert.That(rules.PraiseNotRepeatedWithin, Is.EqualTo(3));
            Assert.That(rules.LetterHunt, Is.SameAs(hunt));
            Assert.That(() => new LearningRules(0, 3, hunt), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new LearningRules(2, -1, hunt), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new LearningRules(2, 3, null), Throws.InstanceOf<ArgumentNullException>());
        }

        /// <summary>
        /// Tests that the <see cref="LetterHuntTuning"/> constructor keeps its numbers and refuses a hunt
        /// with too many stars, no right or no wrong star, or stars that don't move.
        /// </summary>
        [Test]
        public void TestLetterHuntTuning_Ranges()
        {
            LetterHuntTuning hunt = new LetterHuntTuning(12, 4, 60f);

            Assert.That(hunt.Stars, Is.EqualTo(12));
            Assert.That(hunt.RightStars, Is.EqualTo(4));
            Assert.That(hunt.FastestSpeed, Is.EqualTo(60f).Within(0.001f));
            Assert.That(() => new LetterHuntTuning(LetterHuntTuning.MostStars + 1, 4, 60f), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new LetterHuntTuning(10, 0, 60f), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new LetterHuntTuning(10, 10, 60f), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new LetterHuntTuning(10, 4, 0f), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new LetterHuntTuning(10, 4, float.NaN), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }
    }
}
