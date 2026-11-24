using NUnit.Framework;
using Scolita.Editor;

namespace Scolita.Tests.EditMode.Build
{
    /// <summary>Tests <see cref="ApkPermissions"/>, which the build script uses to refuse an APK asking for the internet.</summary>
    public sealed class ApkPermissionsTests
    {
        /// <summary>Tests that <see cref="ApkPermissions.Parse"/> returns only the uses-permission names.</summary>
        [Test]
        public void TestParse_ReturnsUsesPermissionNames()
        {
            string output = "package: com.softshack.scolita\n"
                + "uses-permission: name='android.permission.INTERNET'\n"
                + "permission: com.softshack.scolita.DYNAMIC_RECEIVER_NOT_EXPORTED_PERMISSION\n"
                + "uses-permission: name='com.softshack.scolita.DYNAMIC_RECEIVER_NOT_EXPORTED_PERMISSION'\n";

            Assert.That(ApkPermissions.Parse(output), Is.EqualTo(new[]
            {
                ApkPermissions.Internet,
                "com.softshack.scolita.DYNAMIC_RECEIVER_NOT_EXPORTED_PERMISSION"
            }));
        }

        /// <summary>Tests that <see cref="ApkPermissions.Parse"/> of an APK without permissions is empty.</summary>
        [Test]
        public void TestParse_NoPermissionsIsEmpty()
        {
            Assert.That(ApkPermissions.Parse("package: com.softshack.scolita\r\n"), Is.Empty);
        }
    }
}
