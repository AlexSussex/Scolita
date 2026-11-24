using System;
using NUnit.Framework;
using Scolita.Core.Lessons;

namespace Scolita.Tests.EditMode.Core.Lessons
{
    /// <summary>Tests <see cref="LearningRules"/>.</summary>
    public sealed class LearningRulesTests
    {
        /// <summary>Tests that the <see cref="LearningRules"/> constructor keeps its numbers and refuses ones out of range.</summary>
        [Test]
        public void TestLearningRules_Ranges()
        {
            LearningRules rules = new LearningRules(2, 3);

            Assert.That(rules.TriesBeforeAnswerShown, Is.EqualTo(2));
            Assert.That(rules.PraiseNotRepeatedWithin, Is.EqualTo(3));
            Assert.That(() => new LearningRules(0, 3), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new LearningRules(2, -1), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }
    }
}
