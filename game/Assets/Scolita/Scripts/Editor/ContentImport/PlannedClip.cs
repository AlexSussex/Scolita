namespace Scolita.Editor.ContentImport
{
    /// <summary>
    /// A voice clip's copy as the importer would write it: its project path and the clip it copies
    /// from <c>content/voice/</c>. Made by <see cref="ContentImportPlan"/>.
    /// </summary>
    public sealed class PlannedClip
    {
        /// <summary>Creates a planned clip; called by <see cref="ContentImportPlan"/>.</summary>
        public PlannedClip(string path, string sourcePath)
        {
            Path = path;
            SourcePath = sourcePath;
        }

        /// <summary>Its project path (<c>Assets/Scolita/Content/Voice/Clips/ro-sound-m.ogg</c>).</summary>
        public string Path { get; }

        /// <summary>The full path of the clip it copies.</summary>
        public string SourcePath { get; }
    }
}
