using NUnit.Framework;
using Scolita.Editor.ContentImport;

namespace Scolita.Tests.EditMode.Content
{
    /// <summary>
    /// The importer's freshness test: the assets under <c>Assets/Scolita/Content</c> are exactly what
    /// importing <c>content/*.csv</c> makes (<see cref="ContentImportPlan.Differences"/>), so a commit
    /// that changes a CSV without importing it fails.
    /// </summary>
    public sealed class ContentFreshnessTests
    {
        /// <summary>Tests that <see cref="ContentImportPlan.Differences"/> finds nothing to change for the repository's content.</summary>
        [Test]
        public void TestDifferences_TheAssetsMatchTheCsvs()
        {
            ContentImportPlan plan = ContentImportPlan.Make(ContentImporter.RepositoryContent(),
                new ContentAssetPaths(ContentImporter.AssetFolder));

            Assert.That(plan.HasErrors, Is.False, string.Join("\n", plan.Problems));
            Assert.That(plan.Differences(), Is.Empty, "Run \u0218coli\u021Ba \u2192 Import content and commit what it writes.");
        }
    }
}
