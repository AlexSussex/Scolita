using System;
using Scolita.Core.Ids;

namespace Scolita.Core.Content
{
    /// <summary>
    /// The stable id of a content item (a letter <c>m</c>, a word <c>ro-mama</c>, a picture, a
    /// unit, a lesson): <c>[a-z0-9-]+</c> (<see cref="StableIdText"/>). Never renamed and never
    /// reused; a retired item keeps its id (AGENTS.md → "Progress survives updates").
    /// </summary>
    public readonly struct ContentId : IEquatable<ContentId>
    {
        private readonly string value;

        private ContentId(string value)
        {
            this.value = value;
        }

        /// <summary>
        /// The id's text. Throws an <see cref="InvalidOperationException"/> for the default value,
        /// which is no item's id.
        /// </summary>
        public string Value => value ?? throw new InvalidOperationException("This ContentId was never set.");

        /// <summary>Whether this is a real id (false for the default value, which stands for "none").</summary>
        public bool IsSet => value != null;

        /// <summary>
        /// The id <paramref name="text"/>; throws an <see cref="ArgumentException"/> when it isn't
        /// <c>[a-z0-9-]+</c>. Called by <see cref="VoiceLineIds"/>, the content assets and tests.
        /// </summary>
        public static ContentId Create(string text)
        {
            if (!StableIdText.IsValid(text))
            {
                throw new ArgumentException("A content id is lower-case letters, digits and '-' only: '"
                    + text + "'.", nameof(text));
            }

            return new ContentId(text);
        }

        /// <summary>
        /// Reads an id from <paramref name="text"/>; false when it isn't <c>[a-z0-9-]+</c>. Called by
        /// <see cref="ContentTables"/> for every id cell, by <see cref="ContentCodes"/> and by
        /// <c>ContentIdTests</c>.
        /// </summary>
        public static bool TryCreate(string text, out ContentId id)
        {
            id = StableIdText.IsValid(text) ? new ContentId(text) : default;
            return id.value != null;
        }

        /// <inheritdoc/>
        public bool Equals(ContentId other)
        {
            return string.Equals(value, other.value, StringComparison.Ordinal);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is ContentId other && Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return value == null ? 0 : StringComparer.Ordinal.GetHashCode(value);
        }

        /// <summary>The id's text, or an empty string for the default value.</summary>
        public override string ToString()
        {
            return value ?? string.Empty;
        }

        /// <summary>Whether two ids are the same.</summary>
        public static bool operator ==(ContentId left, ContentId right)
        {
            return left.Equals(right);
        }

        /// <summary>Whether two ids are different.</summary>
        public static bool operator !=(ContentId left, ContentId right)
        {
            return !left.Equals(right);
        }
    }
}
