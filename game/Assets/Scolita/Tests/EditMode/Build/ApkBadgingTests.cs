using NUnit.Framework;
using Scolita.Editor;

namespace Scolita.Tests.EditMode.Build
{
    /// <summary>Tests <see cref="ApkBadging"/>, which the release build uses to refuse a debuggable APK.</summary>
    public sealed class ApkBadgingTests
    {
        /// <summary>Tests that <see cref="ApkBadging.IsDebuggable"/> finds the application-debuggable line.</summary>
        [Test]
        public void TestIsDebuggable_FindsTheDebuggableLine()
        {
            string output = "package: name='com.softshack.scolita' versionCode='1' versionName='0.0.0'\r\n"
                + "sdkVersion:'26'\r\n"
                + "application-debuggable\r\n"
                + "native-code: 'arm64-v8a'\r\n";

            Assert.That(ApkBadging.IsDebuggable(output), Is.True);
        }

        /// <summary>Tests that <see cref="ApkBadging.IsDebuggable"/> is false for a release APK's output, or none.</summary>
        [TestCase(null)]
        [TestCase("")]
        [TestCase("package: name='com.softshack.scolita'\napplication-label:'debuggable app'\nnative-code: 'arm64-v8a'\n")]
        public void TestIsDebuggable_FalseWithoutTheLine(string output)
        {
            Assert.That(ApkBadging.IsDebuggable(output), Is.False);
        }
    }
}
