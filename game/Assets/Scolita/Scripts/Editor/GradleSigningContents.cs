using System;
using System.IO;
using UnityEngine;

namespace Scolita.Editor
{
    /// <summary>
    /// What <see cref="GradleSigningFile"/> writes for Gradle, as JSON: the four values of a
    /// <see cref="ReleaseSigning"/>, under the names the launcher's Gradle template reads
    /// (<c>keystorePath</c>, <c>keystorePassword</c>, <c>keyAlias</c>, <c>keyPassword</c>).
    /// </summary>
    [Serializable]
    public sealed class GradleSigningContents
    {
        [SerializeField] private string keystorePath;
        [SerializeField] private string keystorePassword;
        [SerializeField] private string keyAlias;
        [SerializeField] private string keyPassword;

        /// <summary>
        /// Copies <paramref name="signing"/>'s values, the keystore's path made full (Gradle would
        /// resolve a relative one from its own folder). Called by <see cref="GradleSigningFile.Write"/>.
        /// </summary>
        public GradleSigningContents(ReleaseSigning signing)
        {
            keystorePath = Path.GetFullPath(signing.KeystorePath);
            keystorePassword = signing.KeystorePassword;
            keyAlias = signing.KeyAlias;
            keyPassword = signing.KeyPassword;
        }

        /// <summary>The keystore file's path.</summary>
        public string KeystorePath => keystorePath;

        /// <summary>The keystore's password.</summary>
        public string KeystorePassword => keystorePassword;

        /// <summary>The key's alias in the keystore.</summary>
        public string KeyAlias => keyAlias;

        /// <summary>The key's password.</summary>
        public string KeyPassword => keyPassword;
    }
}
