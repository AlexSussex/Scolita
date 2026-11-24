using System;
using System.Collections.Generic;
using Scolita.Core.Languages;

namespace Scolita.Core.Content
{
    /// <summary>
    /// Something the app says, in one language: its text (what the system voice reads, and what the
    /// bubble shows), its recorded clips (one per spoken segment, <c>VoiceTemplate</c>), or none yet,
    /// and whether they are recorded. One row of <c>voice-lines.csv</c>.
    /// </summary>
    public sealed class VoiceLine : IContentItem
    {
        private static readonly char[] ClipSeparators = { ' ', '\t' };

        /// <summary>Creates a voice line; called by <see cref="ContentTables"/>, the voice line asset and tests.</summary>
        public VoiceLine(ContentId id, Language language, VoiceLineKind kind, string text, string clip,
            VoiceLineStatus status, bool retired, ContentSource source)
        {
            Id = id;
            Language = language;
            Kind = kind;
            Text = text ?? string.Empty;
            Clip = clip ?? string.Empty;
            Clips = Clip.Split(ClipSeparators, StringSplitOptions.RemoveEmptyEntries);
            Status = status;
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

        /// <summary>
        /// The <c>clip</c> cell as written: its clips' file names in <c>content/voice/</c>, separated by
        /// spaces; empty for none (the system voice says it).
        /// </summary>
        public string Clip { get; }

        /// <summary>Its clips' file names, one per spoken segment of its text, in order.</summary>
        public IReadOnlyList<string> Clips { get; }

        /// <summary>Whether a clip is named.</summary>
        public bool HasClip => Clips.Count > 0;

        /// <summary>The status as written in the <c>status</c> cell.</summary>
        public VoiceLineStatus Status { get; }

        /// <inheritdoc/>
        public bool Retired { get; }

        /// <inheritdoc/>
        public ContentSource Source { get; }
    }
}
