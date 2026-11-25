using System;
using NUnit.Framework;
using Scolita.Core.Languages;
using Scolita.Core.Progress;

namespace Scolita.Tests.EditMode.Core.Progress
{
    /// <summary>Tests <see cref="ProgressRules"/>, the numbers of "learned" and of the reviews.</summary>
    public sealed class ProgressRulesTests
    {
        /// <summary>Tests that <see cref="ProgressRules.RightsForLearned"/> gives each language its own number.</summary>
        [Test]
        public void TestRightsForLearned_PerLanguage()
        {
            ProgressRules rules = new ProgressRules(6, 8, 2, 2, new[] { 1, 3 }, 1, 2, 3, 3);

            Assert.That(rules.RightsForLearned(Language.Romanian), Is.EqualTo(6));
            Assert.That(rules.RightsForLearned(Language.English), Is.EqualTo(8));
            Assert.That(rules.ReviewIntervalDays, Is.EqualTo(new[] { 1, 3 }));
        }

        /// <summary>Tests that the <see cref="ProgressRules"/> constructor refuses a number out of its range.</summary>
        [Test]
        public void TestConstructor_RefusesNumbersOutOfRange()
        {
            int[] days = { 1, 3, 7 };

            Assert.That(() => new ProgressRules(0, 6, 2, 2, days, 1, 2, 3, 3), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new ProgressRules(6, 6, 0, 2, days, 1, 2, 3, 3), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new ProgressRules(6, 6, 2, 31, days, 1, 2, 3, 3), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new ProgressRules(6, 6, 2, 2, new int[0], 1, 2, 3, 3), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new ProgressRules(6, 6, 2, 2, new[] { 1, 0 }, 1, 2, 3, 3), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new ProgressRules(6, 6, 2, 2, null, 1, 2, 3, 3), Throws.InstanceOf<ArgumentNullException>());
            Assert.That(() => new ProgressRules(6, 6, 2, 2, days, -1, 2, 3, 3), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new ProgressRules(6, 6, 2, 2, days, 1, -1, 3, 3), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new ProgressRules(6, 6, 2, 2, days, 1, 2, 5, 3), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new ProgressRules(6, 6, 2, 2, days, 1, 2, 3, 0), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }
    }
}
