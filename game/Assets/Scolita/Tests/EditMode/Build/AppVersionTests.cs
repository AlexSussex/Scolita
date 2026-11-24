using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using Scolita.Editor;

namespace Scolita.Tests.EditMode.Build
{
    /// <summary>Tests <see cref="AppVersion"/>, read by the build script from the <c>VERSION</c> file.</summary>
    public sealed class AppVersionTests
    {
        /// <summary>Tests that <see cref="AppVersion.Parse"/> reads each part of a valid version.</summary>
        [TestCase("0.0.0", 0, 0, 0)]
        [TestCase("0.4.2", 0, 4, 2)]
        [TestCase("1.10.99", 1, 10, 99)]
        [TestCase("99.99.99", 99, 99, 99)]
        public void TestParse_ValidVersionGivesItsParts(string text, int major, int minor, int patch)
        {
            AppVersion version = AppVersion.Parse(text);

            Assert.That(version.Major, Is.EqualTo(major));
            Assert.That(version.Minor, Is.EqualTo(minor));
            Assert.That(version.Patch, Is.EqualTo(patch));
            Assert.That(version.ToString(), Is.EqualTo(text));
        }

        /// <summary>Tests that <see cref="AppVersion.Parse"/> ignores the line ending and spaces around the version.</summary>
        [TestCase("0.1.0\n")]
        [TestCase("0.1.0\r\n")]
        [TestCase("  0.1.0  ")]
        public void TestParse_IgnoresSurroundingWhitespace(string text)
        {
            Assert.That(AppVersion.Parse(text).ToString(), Is.EqualTo("0.1.0"));
        }

        /// <summary>Tests that <see cref="AppVersion.Parse"/> refuses malformed versions with a clear message.</summary>
        [TestCase("1.2")]
        [TestCase("1.2.3.4")]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("a.b.c")]
        [TestCase("01.2.3")]
        [TestCase("1.2.100")]
        [TestCase("-1.2.3")]
        [TestCase("+1.2.3")]
        [TestCase("1. 2.3")]
        [TestCase("1.2.3-beta")]
        [TestCase("v1.2.3")]
        [TestCase("1.2.٣")]
        public void TestParse_MalformedVersionThrows(string text)
        {
            Assert.That(() => AppVersion.Parse(text),
                Throws.TypeOf<FormatException>().With.Message.Contains("VERSION must be MAJOR.MINOR.PATCH"));
        }

        /// <summary>Tests <see cref="AppVersion.VersionCode"/>: major * 10000 + minor * 100 + patch.</summary>
        [TestCase("0.0.1", 1)]
        [TestCase("0.4.2", 402)]
        [TestCase("0.10.0", 1000)]
        [TestCase("1.0.0", 10000)]
        [TestCase("12.34.56", 123456)]
        [TestCase("99.99.99", 999999)]
        public void TestVersionCode_FollowsTheFormula(string text, int expectedCode)
        {
            Assert.That(AppVersion.Parse(text).VersionCode, Is.EqualTo(expectedCode));
        }

        /// <summary>
        /// Tests that <see cref="AppVersion.VersionCode"/> of 0.0.0 is 1, as Android refuses 0, so
        /// 0.0.0 and 0.0.1 share code 1 (neither is ever released).
        /// </summary>
        [Test]
        public void TestVersionCode_ZeroAndFirstPatchShareCodeOne()
        {
            Assert.That(AppVersion.Parse("0.0.0").VersionCode, Is.EqualTo(1));
            Assert.That(AppVersion.Parse("0.0.1").VersionCode, Is.EqualTo(AppVersion.Parse("0.0.0").VersionCode));
        }

        /// <summary>Tests that <see cref="AppVersion.ReadFile"/> reads a file with a byte order mark and a line ending.</summary>
        [Test]
        public void TestReadFile_IgnoresByteOrderMarkAndLineEnding()
        {
            string path = Path.Combine(Path.GetTempPath(), "scolita-version-" + Guid.NewGuid().ToString("N"));
            try
            {
                File.WriteAllText(path, "0.4.2\r\n", new UTF8Encoding(true));

                Assert.That(AppVersion.ReadFile(path).ToString(), Is.EqualTo("0.4.2"));
            }
            finally
            {
                File.Delete(path);
            }
        }

        /// <summary>Tests that <see cref="AppVersion.ReadFile"/> says where it looked for a missing file.</summary>
        [Test]
        public void TestReadFile_MissingFileThrows()
        {
            string path = Path.Combine(Path.GetTempPath(), "scolita-no-version-" + Guid.NewGuid().ToString("N"));

            Assert.That(() => AppVersion.ReadFile(path),
                Throws.TypeOf<FileNotFoundException>().With.Message.Contains(path));
        }
    }
}
