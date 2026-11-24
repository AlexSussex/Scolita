using System.Collections.Generic;
using Scolita.Core.Content;

namespace Scolita.Tests.EditMode.Fakes
{
    /// <summary>
    /// An <see cref="IContentFiles"/> for tests: the art and clip files it is given exist, nothing
    /// else does. Used by <c>ContentCsvs.Check</c>.
    /// </summary>
    public sealed class FakeContentFiles : IContentFiles
    {
        private readonly HashSet<string> files;

        /// <summary>Creates the fake with <paramref name="existingFiles"/>; called by <c>ContentCsvs</c> and tests.</summary>
        public FakeContentFiles(params string[] existingFiles)
        {
            files = new HashSet<string>(existingFiles);
        }

        /// <inheritdoc/>
        public bool ArtExists(string fileName)
        {
            return files.Contains(fileName);
        }

        /// <inheritdoc/>
        public bool ClipExists(string fileName)
        {
            return files.Contains(fileName);
        }
    }
}
