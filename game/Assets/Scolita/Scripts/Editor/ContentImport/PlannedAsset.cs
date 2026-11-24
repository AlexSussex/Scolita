using UnityEngine;

namespace Scolita.Editor.ContentImport
{
    /// <summary>
    /// A content asset as the importer would write it: its project path and an in-memory asset
    /// holding what it should contain. Made by <see cref="ContentImportPlan"/>.
    /// </summary>
    public sealed class PlannedAsset
    {
        /// <summary>Creates a planned asset; called by <see cref="ContentImportPlan"/>.</summary>
        public PlannedAsset(string path, ScriptableObject desired)
        {
            Path = path;
            Desired = desired;
        }

        /// <summary>Its project path (<c>Assets/Scolita/Content/Words/ro-mama.asset</c>).</summary>
        public string Path { get; }

        /// <summary>What it should contain (not saved; the plan writes it into the asset at <see cref="Path"/>).</summary>
        public ScriptableObject Desired { get; }
    }
}
