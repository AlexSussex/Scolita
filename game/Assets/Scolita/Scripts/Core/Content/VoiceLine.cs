using Scolita.Core.Languages;

namespace Scolita.Core.Content
{
    /// <summary>
    /// Something the app says, in one language: its text (what the system voice reads, and what the
    /// bubble shows) and its recorded clip, or none yet. One row of <c>voice-lines.csv</c>.
    /// </summary>
    public sealed class VoiceLine : IContentItem
    {
        /// <summary>Creates a voice line; called by <see cref="ContentTables"/>, the voice line asset and tests.</summary>
        public VoiceLine(ContentId id, Language language, VoiceLineKind kind, string text, string clip, bool retired,
            ContentSource source)
        {
            Id = id;
            Language = language;
            Kind = kind;
            Text = text ?? string.Empty;
            Clip = clip ?? string.Empty;
            Retired = retired;
            Source = source;
        }

        /// <inheritdoc/>
        public ContentId Id { get; }

        /// <summary>The id as a voice line's id, for <c>IVoice</c>.</summary>
        public VoiceLineId LineId => new VoiceLineId(Id);

        /// <summary>The line's one language.</summary>
        public Language Language { get; }

        /// <summary>What kind of line it is.</summary>
        public VoiceLineKind Kind { get; }

        /// <summary>What it says.</summary>
        public string Text { get; }

        /// <summary>Its clip's file name in <c>content/voice/</c>; empty for none (the system voice says it).</summary>
        public string Clip { get; }

        /// <summary>Whether a clip is named.</summary>
        public bool HasClip => Clip.Length > 0;

        /// <inheritdoc/>
        public bool Retired { get; }

        /// <inheritdoc/>
        public ContentSource Source { get; }
    }
}
