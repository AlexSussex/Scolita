using System.Collections.Generic;

namespace Scolita.Editor.ContentImport
{
    /// <summary>
    /// Where the importer puts each content asset under its asset folder (<c>Assets/Scolita/Content</c>,
    /// or a test's): one asset per stable id, named after it, so its path (and its GUID) never changes.
    /// </summary>
    public sealed class ContentAssetPaths
    {
        private const string Letters = "Letters";
        private const string Words = "Words";
        private const string Pictures = "Pictures";
        private const string Voice = "Voice";
        private const string VoiceClips = "Voice/Clips";
        private const string Domains = "Domains";
        private const string Units = "Domains/Units";
        private const string Lessons = "Domains/Lessons";
        private const string Strokes = "Strokes";

        /// <summary>Creates the paths under <paramref name="folder"/> (a project path, <c>Assets/…</c>); called by <see cref="ContentImportPlan"/>.</summary>
        public ContentAssetPaths(string folder)
        {
            Folder = folder;
        }

        /// <summary>The asset folder.</summary>
        public string Folder { get; }

        /// <summary>
        /// The folders below <see cref="Folder"/> that the importer owns, each holding only what it
        /// writes (the catalogs sit in <see cref="Folder"/> itself); called by <see cref="ContentImportPlan"/>.
        /// </summary>
        public IReadOnlyList<string> OwnedFolders => new[]
        {
            Under(Letters), Under(Words), Under(Pictures), Under(Voice), Under(Domains), Under(Units),
            Under(Lessons), Under(Strokes)
        };

        /// <summary>A letter's asset; called by <see cref="ContentImportPlan"/>.</summary>
        public string Letter(string id)
        {
            return Under(Letters) + "/" + id + ".asset";
        }

        /// <summary>A word's asset; called by <see cref="ContentImportPlan"/>.</summary>
        public string Word(string id)
        {
            return Under(Words) + "/" + id + ".asset";
        }

        /// <summary>A picture's asset; called by <see cref="ContentImportPlan"/>.</summary>
        public string Picture(string id)
        {
            return Under(Pictures) + "/" + id + ".asset";
        }

        /// <summary>A picture's image (its art, or its placeholder); called by <see cref="ContentImportPlan"/>.</summary>
        public string PictureImage(string id)
        {
            return Under(Pictures) + "/" + id + ".png";
        }

        /// <summary>A voice line's asset; called by <see cref="ContentImportPlan"/>.</summary>
        public string VoiceLine(string id)
        {
            return Under(Voice) + "/" + id + ".asset";
        }

        /// <summary>The folder of the voice clips' copies, made only once there is a clip; called by <see cref="ContentImportPlan"/>.</summary>
        public string VoiceClipFolder => Under(VoiceClips);

        /// <summary>A voice clip's copy, by its file name in <c>content/voice/</c>; called by <see cref="ContentImportPlan"/>.</summary>
        public string VoiceClip(string clipName)
        {
            return Under(VoiceClips) + "/" + clipName;
        }

        /// <summary>A domain's asset; called by <see cref="ContentImportPlan"/>.</summary>
        public string Domain(string id)
        {
            return Under(Domains) + "/" + id + ".asset";
        }

        /// <summary>A unit's asset; called by <see cref="ContentImportPlan"/>.</summary>
        public string Unit(string id)
        {
            return Under(Units) + "/" + id + ".asset";
        }

        /// <summary>A lesson's asset; called by <see cref="ContentImportPlan"/>.</summary>
        public string Lesson(string id)
        {
            return Under(Lessons) + "/" + id + ".asset";
        }

        /// <summary>
        /// The folder of the letter shapes' assets, whose stale assets the importer removes (a shape is replaced, never
        /// retired); called by <see cref="ContentImportPlan"/>.
        /// </summary>
        public string ShapeFolder => Under(Strokes);

        /// <summary>A letter shape's asset; called by <see cref="ContentImportPlan"/>.</summary>
        public string Shape(string id)
        {
            return Under(Strokes) + "/" + id + ".asset";
        }

        /// <summary>A catalog's asset (<c>LetterCatalog</c>), at the folder's top.</summary>
        public string Catalog(string catalogName)
        {
            return Folder + "/" + catalogName + ".asset";
        }

        private string Under(string subfolder)
        {
            return Folder + "/" + subfolder;
        }
    }
}
