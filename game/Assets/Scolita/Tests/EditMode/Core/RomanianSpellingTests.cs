using System.Collections.Generic;
using NUnit.Framework;
using Scolita.Core.Languages;

namespace Scolita.Tests.EditMode.Core
{
    /// <summary>Tests <see cref="RomanianSpelling"/>, the cedilla check of the content validator.</summary>
    public sealed class RomanianSpellingTests
    {
        /// <summary>Tests that <see cref="RomanianSpelling.FindCedillas"/> passes ș ț Ș Ț with the comma below.</summary>
        [Test]
        public void TestFindCedillas_CommaBelowLettersPass()
        {
            IReadOnlyList<CedillaFinding> findings = RomanianSpelling.FindCedillas(
                "\u0218coli\u021Ba, \u021Bar\u0103, \u0219ase, \u021Aara, \u0103 \u00E2 \u00EE \u0102 \u00C2 \u00CE");

            Assert.That(findings, Is.Empty);
        }

        /// <summary>Tests that <see cref="RomanianSpelling.FindCedillas"/> finds each of ş ţ Ş Ţ with the letter that belongs there.</summary>
        [Test]
        public void TestFindCedillas_FindsEachCedillaLetter()
        {
            IReadOnlyList<CedillaFinding> findings = RomanianSpelling.FindCedillas("\u015F\u0163\u015E\u0162");

            Assert.That(findings, Has.Count.EqualTo(4));
            Assert.That(findings[0].Found, Is.EqualTo('\u015F'));
            Assert.That(findings[0].Correct, Is.EqualTo('\u0219'));
            Assert.That(findings[1].Correct, Is.EqualTo('\u021B'));
            Assert.That(findings[2].Correct, Is.EqualTo('\u0218'));
            Assert.That(findings[3].Correct, Is.EqualTo('\u021A'));
        }

        /// <summary>Tests that <see cref="RomanianSpelling.FindCedillas"/> gives the line and column, with Windows line endings too.</summary>
        [Test]
        public void TestFindCedillas_GivesLineAndColumn()
        {
            IReadOnlyList<CedillaFinding> findings = RomanianSpelling.FindCedillas("id,text\r\nw1,pi\u0163\u0103\nw2,\u015Fase");

            Assert.That(findings, Has.Count.EqualTo(2));
            Assert.That(findings[0].Line, Is.EqualTo(2));
            Assert.That(findings[0].Column, Is.EqualTo(6));
            Assert.That(findings[1].Line, Is.EqualTo(3));
            Assert.That(findings[1].Column, Is.EqualTo(4));
            Assert.That(findings[1].ToString(), Is.EqualTo("line 3, column 4: U+015F \u015F must be U+0219 \u0219"));
        }

        /// <summary>Tests that <see cref="RomanianSpelling.FindCedillas"/> finds nothing in a null or empty text.</summary>
        [TestCase(null)]
        [TestCase("")]
        public void TestFindCedillas_NothingInEmptyText(string text)
        {
            Assert.That(RomanianSpelling.FindCedillas(text), Is.Empty);
        }
    }
}
