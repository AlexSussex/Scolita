using System.Collections.Generic;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using UnityEngine;

namespace Scolita.Content
{
    /// <summary>
    /// A <see cref="VoiceLine"/> as an asset (<c>Content/Voice/&lt;id&gt;.asset</c>), written by the
    /// content importer from <c>voice-lines.csv</c> and never edited by hand. Its clip is named; the
    /// <c>voice</c> brief links the recorded clips themselves.
    /// </summary>
    public sealed class VoiceLineAsset : ScriptableObject, IValidatableContent
    {
        [SerializeField] private string id;
        [SerializeField] private string language;
        [SerializeField] private string kind;
        [SerializeField] private string text;
        [SerializeField] private string clip;
        [SerializeField] private bool retired;

        /// <summary>The line's id.</summary>
        public string Id => id;

        /// <summary>Writes <paramref name="line"/> into this asset; called by the content importer.</summary>
        public void Fill(VoiceLine line)
        {
            id = line.Id.Value;
            language = LanguageCodes.CodeOf(line.Language);
            kind = ContentCodes.CodeOf(line.Kind);
            text = line.Text;
            clip = line.Clip;
            retired = line.Retired;
        }

        /// <summary>
        /// The line for Core; throws an <see cref="System.InvalidOperationException"/> if the asset
        /// can't be read. Called by <see cref="CurriculumAssets.Build"/>.
        /// </summary>
        public VoiceLine ToVoiceLine()
        {
            AssetFields fields = new AssetFields(name);
            VoiceLine line = Read(fields);
            fields.ThrowIfAny();
            return line;
        }

        /// <inheritdoc/>
        public IReadOnlyList<string> Validate()
        {
            AssetFields fields = new AssetFields(name);
            Read(fields);
            return fields.Problems;
        }

        private VoiceLine Read(AssetFields fields)
        {
            if (string.IsNullOrEmpty(text))
            {
                fields.Problem("It has no text.");
            }

            return new VoiceLine(fields.OwnId(id), fields.OneLanguage(language), fields.LineKind(kind), text, clip,
                retired, fields.Source);
        }
    }
}
