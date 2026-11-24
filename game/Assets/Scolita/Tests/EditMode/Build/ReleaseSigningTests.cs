using System;
using System.Collections.Generic;
using NUnit.Framework;
using Scolita.Editor;

namespace Scolita.Tests.EditMode.Build
{
    /// <summary>Tests <see cref="ReleaseSigning"/>, which the release build reads from the environment.</summary>
    public sealed class ReleaseSigningTests
    {
        /// <summary>Tests that <see cref="ReleaseSigning.Read"/> gives the four values when all are set.</summary>
        [Test]
        public void TestRead_AllVariablesSetGivesTheirValues()
        {
            Dictionary<string, string> variables = new Dictionary<string, string>
            {
                { ReleaseSigning.KeystorePathVariable, "C:/keys/release.keystore" },
                { ReleaseSigning.KeystorePasswordVariable, "store-secret" },
                { ReleaseSigning.KeyAliasVariable, "scolita" },
                { ReleaseSigning.KeyPasswordVariable, "key-secret" }
            };

            ReleaseSigning signing = ReleaseSigning.Read(name => variables.TryGetValue(name, out string value) ? value : null);

            Assert.That(signing.KeystorePath, Is.EqualTo("C:/keys/release.keystore"));
            Assert.That(signing.KeystorePassword, Is.EqualTo("store-secret"));
            Assert.That(signing.KeyAlias, Is.EqualTo("scolita"));
            Assert.That(signing.KeyPassword, Is.EqualTo("key-secret"));
        }

        /// <summary>Tests that <see cref="ReleaseSigning.Read"/> names every missing variable when none is set.</summary>
        [Test]
        public void TestRead_NoVariableSetNamesAllFour()
        {
            Assert.That(() => ReleaseSigning.Read(name => null),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.Contains(ReleaseSigning.KeystorePathVariable)
                    .And.Message.Contains(ReleaseSigning.KeystorePasswordVariable)
                    .And.Message.Contains(ReleaseSigning.KeyAliasVariable)
                    .And.Message.Contains(ReleaseSigning.KeyPasswordVariable));
        }

        /// <summary>
        /// Tests that <see cref="ReleaseSigning.Read"/> refuses an empty variable and never puts a
        /// password in its message.
        /// </summary>
        [Test]
        public void TestRead_OneEmptyVariableThrowsWithoutShowingValues()
        {
            Dictionary<string, string> variables = new Dictionary<string, string>
            {
                { ReleaseSigning.KeystorePathVariable, "C:/keys/release.keystore" },
                { ReleaseSigning.KeystorePasswordVariable, "store-secret" },
                { ReleaseSigning.KeyAliasVariable, "scolita" },
                { ReleaseSigning.KeyPasswordVariable, " " }
            };

            Assert.That(() => ReleaseSigning.Read(name => variables[name]),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.Contains(ReleaseSigning.KeyPasswordVariable)
                    .And.Message.Not.Contains("store-secret")
                    .And.Message.Not.Contains(ReleaseSigning.KeystorePasswordVariable));
        }

        /// <summary>
        /// Tests that <see cref="ReleaseSigning.Read"/> refuses a value with spaces or a line break
        /// around it, naming the variable and not showing the value.
        /// </summary>
        [Test]
        public void TestRead_PaddedValueThrowsWithoutShowingIt()
        {
            Dictionary<string, string> variables = new Dictionary<string, string>
            {
                { ReleaseSigning.KeystorePathVariable, "C:/keys/release.keystore" },
                { ReleaseSigning.KeystorePasswordVariable, "store-secret\n" },
                { ReleaseSigning.KeyAliasVariable, "scolita" },
                { ReleaseSigning.KeyPasswordVariable, "key-secret" }
            };

            Assert.That(() => ReleaseSigning.Read(name => variables[name]),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.Contains(ReleaseSigning.KeystorePasswordVariable)
                    .And.Message.Not.Contains("store-secret"));
        }

        /// <summary>Tests that <see cref="ReleaseSigning.VariableNames"/> lists the four variables and can't be changed.</summary>
        [Test]
        public void TestVariableNames_ListsTheFourAndIsReadOnly()
        {
            IList<string> names = (IList<string>)ReleaseSigning.VariableNames;

            Assert.That(names, Has.Count.EqualTo(4));
            Assert.That(() => names[0] = "changed", Throws.TypeOf<NotSupportedException>());
        }
    }
}
