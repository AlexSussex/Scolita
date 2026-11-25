using System;
using NUnit.Framework;
using Scolita.Core.Lessons;
using Scolita.Core.Tracing;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Lessons
{
    /// <summary>Tests <see cref="LearningRules"/>, <see cref="LetterHuntTuning"/> and <see cref="TracingTuning"/>.</summary>
    public sealed class LearningRulesTests
    {
        /// <summary>Tests that the <see cref="LearningRules"/> constructor keeps its numbers and refuses ones out of range.</summary>
        [Test]
        public void TestLearningRules_Ranges()
        {
            LetterHuntTuning hunt = new LetterHuntTuning(10, 4, 60f);
            TracingTuning tracing = TestLearningRules.Tracing();

            LearningRules rules = new LearningRules(2, 3, hunt, tracing);

            Assert.That(rules.TriesBeforeAnswerShown, Is.EqualTo(2));
            Assert.That(rules.PraiseNotRepeatedWithin, Is.EqualTo(3));
            Assert.That(rules.LetterHunt, Is.SameAs(hunt));
            Assert.That(rules.Tracing, Is.SameAs(tracing));
            Assert.That(() => new LearningRules(0, 3, hunt, tracing), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new LearningRules(2, -1, hunt, tracing), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new LearningRules(2, 3, null, tracing), Throws.InstanceOf<ArgumentNullException>());
            Assert.That(() => new LearningRules(2, 3, hunt, null), Throws.InstanceOf<ArgumentNullException>());
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
            Assert.That(() => new LetterHuntTuning(LetterHuntTuning.FewestStars - 1, 4, 60f), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new LetterHuntTuning(10, 0, 60f), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new LetterHuntTuning(10, 10, 60f), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new LetterHuntTuning(10, 4, 0f), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new LetterHuntTuning(10, 4, float.NaN), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        /// <summary>
        /// Tests that <see cref="TracingTuning.ForRound"/> gives each round its tolerance, and that the constructor
        /// refuses a later round more forgiving than an earlier one, and a tolerance out of range.
        /// </summary>
        [Test]
        public void TestForRound_EachRoundAndItsOrder()
        {
            StrokeTolerance wide = new StrokeTolerance(32f, 34f, 0.8f);
            StrokeTolerance middle = new StrokeTolerance(28f, 30f, 0.85f);
            StrokeTolerance tight = new StrokeTolerance(24f, 28f, 0.9f);

            TracingTuning tuning = new TracingTuning(wide, middle, tight, 44f);

            Assert.That(tuning.ForRound(0).Corridor, Is.EqualTo(32f).Within(0.001f));
            Assert.That(tuning.ForRound(1).StartRadius, Is.EqualTo(30f).Within(0.001f));
            Assert.That(tuning.ForRound(2).Coverage, Is.EqualTo(0.9f).Within(0.001f));
            Assert.That(() => tuning.ForRound(3), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new TracingTuning(tight, middle, wide, 44f), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new TracingTuning(wide, middle, tight, 20f), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(tuning.JoinRadius, Is.EqualTo(44f).Within(0.001f));
            Assert.That(() => new StrokeTolerance(0f, 30f, 0.8f), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new StrokeTolerance(30f, 0f, 0.8f), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new StrokeTolerance(30f, 30f, 1.2f), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }
    }
}
