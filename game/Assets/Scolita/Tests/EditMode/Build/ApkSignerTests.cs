using NUnit.Framework;
using Scolita.Editor;

namespace Scolita.Tests.EditMode.Build
{
    /// <summary>Tests <see cref="ApkSigner"/>, which the release build uses to refuse an APK signed with the debug key.</summary>
    public sealed class ApkSignerTests
    {
        /// <summary>Tests that <see cref="ApkSigner.Parse"/> returns each signer's certificate subject.</summary>
        [Test]
        public void TestParse_ReturnsSignerCertificateNames()
        {
            string output = "Verifies\r\n"
                + "Verified using v1 scheme (JAR signing): false\r\n"
                + "Verified using v2 scheme (APK Signature Scheme v2): true\r\n"
                + "Signer #1 certificate DN: CN=Throwaway Test\r\n"
                + "Signer #1 certificate SHA-256 digest: 0123abcd\r\n"
                + "Signer #2 certificate DN: CN=Android Debug, O=Android, C=US\n";

            Assert.That(ApkSigner.Parse(output), Is.EqualTo(new[] { "CN=Throwaway Test", "CN=Android Debug, O=Android, C=US" }));
        }

        /// <summary>Tests that <see cref="ApkSigner.Parse"/> of output without signers is empty.</summary>
        [TestCase(null)]
        [TestCase("")]
        [TestCase("DOES NOT VERIFY\nERROR: Missing META-INF/MANIFEST.MF\n")]
        public void TestParse_NoSignerIsEmpty(string output)
        {
            Assert.That(ApkSigner.Parse(output), Is.Empty);
        }
    }
}
