using System;
using NUnit.Framework;
using Scolita.Core.Profiles;

namespace Scolita.Tests.EditMode.Core
{
    /// <summary>Tests <see cref="ProfileId"/>, which names a profile's save folder.</summary>
    public sealed class ProfileIdTests
    {
        /// <summary>Tests that <see cref="ProfileId.Create"/> accepts lower-case letters, digits and '-'.</summary>
        [Test]
        public void TestCreate_AcceptsLettersDigitsAndHyphens()
        {
            ProfileId id = ProfileId.Create("child-1");

            Assert.That(id.Value, Is.EqualTo("child-1"));
            Assert.That(id.ToString(), Is.EqualTo("child-1"));
            Assert.That(ProfileId.Create("tester").Value, Is.EqualTo("tester"));
        }

        /// <summary>
        /// Tests that <see cref="ProfileId.Create"/> refuses anything that could leave the profiles
        /// folder or differ between file systems: capitals, spaces, separators, dots, '_', diacritics.
        /// </summary>
        [TestCase(null)]
        [TestCase("")]
        [TestCase("Child-1")]
        [TestCase("child 1")]
        [TestCase("child_1")]
        [TestCase("../child-1")]
        [TestCase("child/1")]
        [TestCase("child\\1")]
        [TestCase("child.1")]
        [TestCase("\u0219coala")]
        [TestCase("child-1\n")]
        public void TestCreate_RefusesUnsafeText(string text)
        {
            Assert.That(() => ProfileId.Create(text), Throws.ArgumentException);
            Assert.That(ProfileId.TryCreate(text, out ProfileId id), Is.False);
            Assert.That(id, Is.EqualTo(default(ProfileId)));
        }

        /// <summary>Tests that <see cref="ProfileId.TryCreate"/> reads a valid id from a save file.</summary>
        [Test]
        public void TestTryCreate_ReadsValidText()
        {
            bool read = ProfileId.TryCreate("child-2", out ProfileId id);

            Assert.That(read, Is.True);
            Assert.That(id, Is.EqualTo(ProfileId.Create("child-2")));
        }

        /// <summary>Tests that <see cref="ProfileId.Equals(ProfileId)"/> and the operators compare the text exactly.</summary>
        [Test]
        public void TestEquals_ComparesTheText()
        {
            ProfileId first = ProfileId.Create("child-1");

            Assert.That(first == ProfileId.Create("child-1"), Is.True);
            Assert.That(first != ProfileId.Create("child-2"), Is.True);
            Assert.That(first.GetHashCode(), Is.EqualTo(ProfileId.Create("child-1").GetHashCode()));
        }

        /// <summary>Tests that <see cref="ProfileId.Value"/> of the default value fails loudly, as it names no profile.</summary>
        [Test]
        public void TestValue_DefaultThrows()
        {
            ProfileId unset = default;

            Assert.That(() => unset.Value, Throws.TypeOf<InvalidOperationException>());
            Assert.That(unset.ToString(), Is.Empty);
        }
    }
}
