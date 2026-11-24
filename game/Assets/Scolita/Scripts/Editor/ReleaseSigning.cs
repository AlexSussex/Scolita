using System;
using System.Collections.Generic;

namespace Scolita.Editor
{
    /// <summary>
    /// The release key's location and passwords, read from four environment variables for one
    /// build (README → "Build a release APK"). Never logged, and never given to Unity's settings:
    /// <see cref="BuildScript"/> hands it to Gradle only through <see cref="GradleSigningFile"/>.
    /// </summary>
    public sealed class ReleaseSigning
    {
        /// <summary>The variable naming the keystore file.</summary>
        public const string KeystorePathVariable = "SCOLITA_KEYSTORE_PATH";

        /// <summary>The variable holding the keystore's password.</summary>
        public const string KeystorePasswordVariable = "SCOLITA_KEYSTORE_PASS";

        /// <summary>The variable naming the key's alias in the keystore.</summary>
        public const string KeyAliasVariable = "SCOLITA_KEY_ALIAS";

        /// <summary>The variable holding the key's password.</summary>
        public const string KeyPasswordVariable = "SCOLITA_KEY_PASS";

        /// <summary>The four variables, in the order they are read.</summary>
        public static readonly IReadOnlyList<string> VariableNames = Array.AsReadOnly(new[]
        {
            KeystorePathVariable, KeystorePasswordVariable, KeyAliasVariable, KeyPasswordVariable
        });

        private ReleaseSigning(string keystorePath, string keystorePassword, string keyAlias, string keyPassword)
        {
            KeystorePath = keystorePath;
            KeystorePassword = keystorePassword;
            KeyAlias = keyAlias;
            KeyPassword = keyPassword;
        }

        /// <summary>The keystore file's path.</summary>
        public string KeystorePath { get; }

        /// <summary>The keystore's password.</summary>
        public string KeystorePassword { get; }

        /// <summary>The key's alias in the keystore.</summary>
        public string KeyAlias { get; }

        /// <summary>The key's password.</summary>
        public string KeyPassword { get; }

        /// <summary>
        /// Reads the four variables through <paramref name="readVariable"/> (the process's environment
        /// in a build). Throws an <see cref="InvalidOperationException"/> naming every variable that is
        /// missing or empty, or has spaces or a line break at its start or end (often a copy-paste
        /// slip, never trimmed silently as it could be part of a password), never their values.
        /// Called by <see cref="BuildScript"/> and by <c>ReleaseSigningTests</c>.
        /// </summary>
        public static ReleaseSigning Read(Func<string, string> readVariable)
        {
            if (readVariable == null)
            {
                throw new ArgumentNullException(nameof(readVariable));
            }

            List<string> missing = new List<string>();
            List<string> padded = new List<string>();
            string keystorePath = ReadRequired(readVariable, KeystorePathVariable, missing, padded);
            string keystorePassword = ReadRequired(readVariable, KeystorePasswordVariable, missing, padded);
            string keyAlias = ReadRequired(readVariable, KeyAliasVariable, missing, padded);
            string keyPassword = ReadRequired(readVariable, KeyPasswordVariable, missing, padded);
            if (missing.Count > 0)
            {
                throw new InvalidOperationException("The release build needs the release key's environment "
                    + "variables (README → \"Build a release APK\"); missing: " + string.Join(", ", missing) + ".");
            }

            if (padded.Count > 0)
            {
                throw new InvalidOperationException("The release key's environment variables must not start or "
                    + "end with spaces or a line break; check: " + string.Join(", ", padded) + ".");
            }

            return new ReleaseSigning(keystorePath, keystorePassword, keyAlias, keyPassword);
        }

        private static string ReadRequired(Func<string, string> readVariable, string name, List<string> missing,
            List<string> padded)
        {
            string value = readVariable(name);
            if (string.IsNullOrWhiteSpace(value))
            {
                missing.Add(name);
            }
            else if (value.Trim().Length != value.Length)
            {
                padded.Add(name);
            }

            return value;
        }
    }
}
