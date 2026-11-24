using System.Collections.Generic;
using System.IO;
using System.Text;
using Scolita.Core.Content;

namespace Scolita.Editor.ContentImport
{
    /// <summary>
    /// The content's source folder (<c>content/</c> at the repository's root, or a test's copy): reads
    /// its CSVs as strict UTF-8, and says which pictures' art and voice clips are there.
    /// </summary>
    public sealed class ContentFolder : IContentFiles
    {
        private const string PicturesFolder = "pictures";
        private const string VoiceFolder = "voice";

        /// <summary>Creates the folder at the full path <paramref name="root"/>; called by <see cref="ContentImporter"/> and tests.</summary>
        public ContentFolder(string root)
        {
            Root = root;
        }

        /// <summary>The folder's full path.</summary>
        public string Root { get; }

        /// <summary>
        /// Reads every content CSV (<see cref="ContentFileNames.All"/>) that is there, by file name.
        /// A missing file is left out (<see cref="ContentTables.Read"/> reports it); a file that isn't
        /// UTF-8 (saved as ANSI) is reported into <paramref name="problems"/> and left out, never misread.
        /// Called by <see cref="ContentImporter"/> and the content validator test.
        /// </summary>
        public Dictionary<string, string> ReadCsvs(ContentProblems problems)
        {
            Dictionary<string, string> texts = new Dictionary<string, string>();
            UTF8Encoding strictUtf8 = new UTF8Encoding(false, true);
            foreach (string fileName in ContentFileNames.All)
            {
                string path = Path.Combine(Root, fileName);
                if (!File.Exists(path))
                {
                    continue;
                }

                try
                {
                    texts.Add(fileName, strictUtf8.GetString(File.ReadAllBytes(path)));
                }
                catch (DecoderFallbackException)
                {
                    problems.Error(new ContentSource(fileName, 0),
                        "the file is not UTF-8: save it as \"CSV UTF-8\" (Excel) or UTF-8 (a text editor).");
                }
            }

            return texts;
        }

        /// <summary>The full path of the art <paramref name="fileName"/>; called by <see cref="ContentImportPlan"/>.</summary>
        public string ArtPath(string fileName)
        {
            return FullArtPath(fileName);
        }

        /// <inheritdoc/>
        public bool ArtExists(string fileName)
        {
            return File.Exists(FullArtPath(fileName));
        }

        /// <inheritdoc/>
        public bool ClipExists(string fileName)
        {
            return File.Exists(ClipPath(fileName));
        }

        /// <summary>The full path of the clip <paramref name="fileName"/> in <c>content/voice/</c>; called by the importer and the recording kit.</summary>
        public string ClipPath(string fileName)
        {
            return Path.Combine(VoicePath, fileName);
        }

        /// <summary>The full path of <c>content/voice/</c>; called by the recording kit.</summary>
        public string VoicePath => Path.Combine(Root, VoiceFolder);

        /// <summary>
        /// Writes <paramref name="text"/> as the CSV <paramref name="fileName"/>, in UTF-8 (a byte order
        /// mark in the text is kept). Called by the importer (the <c>status</c> column) and the recording kit.
        /// </summary>
        public void WriteCsv(string fileName, string text)
        {
            File.WriteAllText(Path.Combine(Root, fileName), text, new UTF8Encoding(false));
        }

        private string FullArtPath(string fileName)
        {
            return Path.Combine(Root, PicturesFolder, fileName);
        }
    }
}
