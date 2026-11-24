namespace Scolita.Editor.ContentImport
{
    /// <summary>
    /// A picture's image as the importer would write it: its project path and its PNG bytes (the art
    /// copied from <c>content/pictures/</c>, or the placeholder). Made by <see cref="ContentImportPlan"/>.
    /// </summary>
    public sealed class PlannedImage
    {
        /// <summary>Creates a planned image; called by <see cref="ContentImportPlan"/>.</summary>
        public PlannedImage(string path, byte[] png)
        {
            Path = path;
            Png = png;
        }

        /// <summary>Its project path (<c>Assets/Scolita/Content/Pictures/bed.png</c>).</summary>
        public string Path { get; }

        /// <summary>Its PNG file's bytes.</summary>
        public byte[] Png { get; }
    }
}
