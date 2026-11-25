using System;
using NUnit.Framework;
using Scolita.Core.Parent;

namespace Scolita.Tests.EditMode.Core.Parent
{
    /// <summary>Tests <see cref="ParentRules"/>.</summary>
    public sealed class ParentRulesTests
    {
        /// <summary>Tests that <see cref="ParentRules(float, int, int, int, float, System.Collections.Generic.IReadOnlyList{int})"/> keeps its numbers.</summary>
        [Test]
        public void TestParentRules_KeepsItsNumbers()
        {
            ParentRules rules = new ParentRules(3f, 11, 19, 3, 30f, new[] { 10, 15, 20 });

            Assert.That(rules.GateHoldSeconds, Is.EqualTo(3f));
            Assert.That(rules.GateLowestNumber, Is.EqualTo(11));
            Assert.That(rules.GateHighestNumber, Is.EqualTo(19));
            Assert.That(rules.GateChoices, Is.EqualTo(3));
            Assert.That(rules.GateRestSeconds, Is.EqualTo(30f));
            Assert.That(rules.SessionLimitMinutes, Is.EqualTo(new[] { 10, 15, 20 }));
        }

        /// <summary>Tests that <see cref="ParentRules(float, int, int, int, float, System.Collections.Generic.IReadOnlyList{int})"/> refuses numbers out of range.</summary>
        [Test]
        public void TestParentRules_RefusesNumbersOutOfRange()
        {
            int[] limits = { 10, 15, 20 };

            Assert.Throws<ArgumentOutOfRangeException>(() => new ParentRules(0f, 11, 19, 3, 30f, limits));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ParentRules(float.NaN, 11, 19, 3, 30f, limits));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ParentRules(3f, -1, 19, 3, 30f, limits));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ParentRules(3f, 11, 12, 3, 30f, limits));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ParentRules(3f, 11, 19, 1, 30f, limits));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ParentRules(3f, 11, 19, 5, 30f, limits));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ParentRules(3f, 11, 19, 3, 30f, new int[0]));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ParentRules(3f, 11, 19, 3, 30f, new[] { 0, 10 }));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ParentRules(3f, 11, 19, 3, 30f, new[] { 15, 10 }));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ParentRules(3f, 11, 19, 3, -1f, limits));
            Assert.Throws<ArgumentNullException>(() => new ParentRules(3f, 11, 19, 3, 30f, null));
        }
    }
}
