using System.Collections.Generic;
using System.IO;
using System.Text;
using Scolita.Core.Content;
using Scolita.Core.Tracing;

namespace Scolita.Editor.ContentImport
{
    /// <summary>
    /// The content's source folder (<c>content/</c> at the repository's root, or a test's copy): reads
    /// its CSVs and shape files as strict UTF-8 and its ledger of released ids, and says which pictures' art and voice
    /// clips are there.
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
        /// Reads every content CSV (<see cref="ContentFileNames.All"/>) that is there, by file name, and every
        /// shape file of <c>strokes/</c>, named <c>strokes/a-lower-print.json</c>. A missing CSV is left out
        /// (<see cref="ContentTables.Read"/> reports it); a file that isn't UTF-8 (saved as ANSI) is reported
        /// into <paramref name="problems"/> and left out, never misread. Called by <see cref="ContentImporter"/>,
        /// the recording kit and the content tests.
        /// </summary>
        public Dictionary<string, string> ReadTexts(ContentProblems problems)
        {
            Dictionary<string, string> texts = new Dictionary<string, string>();
            foreach (string fileName in ContentFileNames.All)
            {
                string path = Path.Combine(Root, fileName);
                if (!File.Exists(path))
                {
                    continue;
                }

                string text = ReadStrictUtf8(fileName, problems);
                if (text != null)
                {
                    texts.Add(fileName, text);
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

        /// <summary>
        /// The text of the ledger of released ids (<see cref="ReleasedIdLedger.FileName"/>) read as strict
        /// UTF-8; <c>null</c> when it is missing, and empty (reported into <paramref name="problems"/>)
        /// when it isn't UTF-8. Called by <see cref="ContentImportPlan"/> and tests.
        /// </summary>
        public string ReleasedIdsText(ContentProblems problems)
        {
            if (!File.Exists(Path.Combine(Root, ReleasedIdLedger.FileName)))
            {
                return null;
            }

            return ReadStrictUtf8(ReleasedIdLedger.FileName, problems) ?? string.Empty;
        }

        /// <summary>Writes <paramref name="text"/> as the ledger of released ids, in UTF-8; called by <see cref="ReleasedIdsRecorder"/>.</summary>
        public void WriteReleasedIds(string text)
        {
            File.WriteAllText(Path.Combine(Root, ReleasedIdLedger.FileName), text, new UTF8Encoding(false));
        }

        private string ReadStrictUtf8(string fileName, ContentProblems problems)
        {
            try
            {
                return new UTF8Encoding(false, true).GetString(File.ReadAllBytes(Path.Combine(Root, fileName)));
            }
            catch (DecoderFallbackException)
            {
                problems.Error(new ContentSource(fileName, 0),
                    "the file is not UTF-8: save it as \"CSV UTF-8\" (Excel) or UTF-8 (a text editor).");
                return null;
            }
        }

        private string FullArtPath(string fileName)
        {
            return Path.Combine(Root, PicturesFolder, fileName);
        }
    }
}
