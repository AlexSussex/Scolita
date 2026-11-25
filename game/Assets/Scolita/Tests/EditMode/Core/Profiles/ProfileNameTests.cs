using NUnit.Framework;
using Scolita.Core.Profiles;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Profiles
{
    /// <summary>Tests <see cref="ProfileName"/>, the name the parent types for a child.</summary>
    public sealed class ProfileNameTests
    {
        /// <summary>Tests that <see cref="ProfileName.Read"/> takes letters of any name, Romanian ones too, with one space or hyphen between parts.</summary>
        [TestCase("Ana")]
        [TestCase("Matei")]
        [TestCase("Ștefan")]
        [TestCase("Ioana-Maria")]
        [TestCase("Ana Maria")]
        [TestCase("Ăâîșț")]
        [TestCase("Zoë")]
        public void TestRead_TakesLettersWithOneSeparator(string typed)
        {
            ProfileNameProblem problem = ProfileName.Read(typed, TestProfileRules.Default(), out ProfileName name);

            Assert.That(problem, Is.EqualTo(ProfileNameProblem.None));
            Assert.That(name.Text, Is.EqualTo(typed));
            Assert.That(name.IsNone, Is.False);
        }

        /// <summary>Tests that <see cref="ProfileName.Read"/> trims the name and makes a run of spaces one.</summary>
        [Test]
        public void TestRead_TrimsAndJoinsSpaces()
        {
            ProfileName.Read("  Ana   Maria \t", TestProfileRules.Default(), out ProfileName name);

            Assert.That(name.Text, Is.EqualTo("Ana Maria"));
        }

        /// <summary>Tests that <see cref="ProfileName.Read"/> turns the cedilla ş ţ (an old keyboard's) into the comma-below ș ț.</summary>
        [Test]
        public void TestRead_TurnsTheCedillaIntoTheComma()
        {
            ProfileName.Read("Ştefan Ţiţi", TestProfileRules.Default(), out ProfileName name);

            Assert.That(name.Text, Is.EqualTo("Ștefan Țiți"));
        }

        /// <summary>Tests that <see cref="ProfileName.Read"/> joins a letter and its separate accent into one letter.</summary>
        [Test]
        public void TestRead_ComposesAccents()
        {
            ProfileName.Read("Steșă", TestProfileRules.Default(), out ProfileName name);

            Assert.That(name.Text, Is.EqualTo("Steșă"));
        }

        /// <summary>Tests that <see cref="ProfileName.Read"/> reads nothing typed as no name, which is allowed.</summary>
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void TestRead_NothingIsNoName(string typed)
        {
            ProfileNameProblem problem = ProfileName.Read(typed, TestProfileRules.Default(), out ProfileName name);

            Assert.That(problem, Is.EqualTo(ProfileNameProblem.None));
            Assert.That(name.IsNone, Is.True);
            Assert.That(name, Is.EqualTo(ProfileName.None));
            Assert.That(name.Text, Is.Empty);
        }

        /// <summary>Tests that <see cref="ProfileName.Read"/> refuses digits, signs, a separator at an end and two separators together.</summary>
        [TestCase("Ana1")]
        [TestCase("Ana!")]
        [TestCase("-Ana")]
        [TestCase("Ana-")]
        [TestCase("Ana--Maria")]
        [TestCase("Ana - Maria")]
        [TestCase("Ana.Maria")]
        [TestCase("../Ana")]
        public void TestRead_RefusesWhatIsNotLetters(string typed)
        {
            ProfileNameProblem problem = ProfileName.Read(typed, TestProfileRules.Default(), out ProfileName name);

            Assert.That(problem, Is.EqualTo(ProfileNameProblem.NotLetters));
            Assert.That(name.IsNone, Is.True);
        }

        /// <summary>Tests that <see cref="ProfileName.Read"/> takes a name as long as the rules allow, and refuses one longer.</summary>
        [Test]
        public void TestRead_AtMostTheRulesCharacters()
        {
            ProfileNameProblem longest = ProfileName.Read("Alexandra-Ioana Elen", TestProfileRules.Default(), out ProfileName kept);
            ProfileNameProblem tooLong = ProfileName.Read("Alexandra-Ioana Elena", TestProfileRules.Default(), out ProfileName refused);

            Assert.That(longest, Is.EqualTo(ProfileNameProblem.None));
            Assert.That(kept.Text.Length, Is.EqualTo(20));
            Assert.That(tooLong, Is.EqualTo(ProfileNameProblem.TooLong));
            Assert.That(refused.IsNone, Is.True);
        }

        /// <summary>Tests that <see cref="ProfileName.ReadSaved"/> keeps a saved name of any length, and still refuses what isn't letters.</summary>
        [Test]
        public void TestReadSaved_NoLengthLimit()
        {
            ProfileNameProblem saved = ProfileName.ReadSaved("Alexandra-Ioana Elena Maria", out ProfileName name);
            ProfileNameProblem notLetters = ProfileName.ReadSaved("Ana1", out ProfileName _);

            Assert.That(saved, Is.EqualTo(ProfileNameProblem.None));
            Assert.That(name.Text, Is.EqualTo("Alexandra-Ioana Elena Maria"));
            Assert.That(notLetters, Is.EqualTo(ProfileNameProblem.NotLetters));
        }
    }
}
