using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using UnityEngine;

namespace Scolita.Content
{
    /// <summary>
    /// A <see cref="VoiceLine"/> as an asset (<c>Content/Voice/&lt;id&gt;.asset</c>), written by the
    /// content importer from <c>voice-lines.csv</c> and never edited by hand: its text, its clips'
    /// names and status, and the clips themselves (copied from <c>content/voice/</c> into
    /// <c>Content/Voice/Clips/</c>), one per name, in order.
    /// </summary>
    public sealed class VoiceLineAsset : ScriptableObject, IValidatableContent
    {
        [SerializeField] private string id;
        [SerializeField] private string language;
        [SerializeField] private string kind;
        [SerializeField] private string text;
        [SerializeField] private string clip;
        [SerializeField] private string status;
        [SerializeField] private bool retired;
        [SerializeField] private AudioClip[] audioClips = new AudioClip[0];

        /// <summary>The line's id.</summary>
        public string Id => id;

        /// <summary>
        /// Its recorded clips, one per name in its <c>clip</c> cell, in order; none until every one of
        /// them is recorded.
        /// </summary>
        public IReadOnlyList<AudioClip> AudioClips => audioClips;

        /// <summary>Writes <paramref name="line"/> into this asset; called by the content importer.</summary>
        public void Fill(VoiceLine line)
        {
            id = line.Id.Value;
            language = LanguageCodes.CodeOf(line.Language);
            kind = ContentCodes.CodeOf(line.Kind);
            text = line.Text;
            clip = line.Clip;
            status = ContentCodes.CodeOf(line.Status);
            retired = line.Retired;
        }

        /// <summary>Links the line's recorded clips, one per name, in order; called by the content importer.</summary>
        public void LinkClips(IEnumerable<AudioClip> clips)
        {
            audioClips = clips.ToArray();
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

            VoiceLine line = new VoiceLine(fields.OwnId(id), fields.OneLanguage(language), fields.LineKind(kind), text,
                clip, fields.LineStatus(status), retired, fields.Source);
            if (audioClips == null
                || (audioClips.Length != 0 && audioClips.Length != line.Clips.Count)
                || audioClips.Any(audio => audio == null))
            {
                fields.Problem("Its clips don't match its clip names (run the importer again).");
            }

            return line;
        }
    }
}
