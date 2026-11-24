using System;

namespace Scolita.Core.Content
{
    /// <summary>
    /// The id of a voice line (<c>ro-sound-m</c>, <c>en-word-pat</c>, <c>ro-line-try-again</c>):
    /// what everything the app says is named by (<c>IVoice.Speak</c>). Its text is
    /// <c>&lt;language&gt;-&lt;kind&gt;-&lt;key&gt;</c>, made and read by <see cref="VoiceLineIds"/>.
    /// </summary>
    public readonly struct VoiceLineId : IEquatable<VoiceLineId>
    {
        /// <summary>Wraps <paramref name="id"/>; called by <see cref="VoiceLineIds"/> and <see cref="ContentTables"/>.</summary>
        public VoiceLineId(ContentId id)
        {
            Id = id;
        }

        /// <summary>The line's stable id.</summary>
        public ContentId Id { get; }

        /// <inheritdoc/>
        public bool Equals(VoiceLineId other)
        {
            return Id.Equals(other.Id);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is VoiceLineId other && Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>The id's text.</summary>
        public override string ToString()
        {
            return Id.ToString();
        }

        /// <summary>Whether two ids are the same.</summary>
        public static bool operator ==(VoiceLineId left, VoiceLineId right)
        {
            return left.Equals(right);
        }

        /// <summary>Whether two ids are different.</summary>
        public static bool operator !=(VoiceLineId left, VoiceLineId right)
        {
            return !left.Equals(right);
        }
    }
}
