using System;
using Scolita.Core.Ids;

namespace Scolita.Core.Profiles
{
    /// <summary>
    /// The stable id of a learner's profile (<c>child-1</c>): lower-case letters, digits and
    /// <c>-</c> only, so it is safe as the name of the profile's save folder
    /// (<c>profiles/&lt;id&gt;/</c>). Never shown to the child, never reused.
    /// </summary>
    public readonly struct ProfileId : IEquatable<ProfileId>
    {
        private readonly string value;

        private ProfileId(string value)
        {
            this.value = value;
        }

        /// <summary>
        /// The id's text. Throws an <see cref="InvalidOperationException"/> for the default value,
        /// which is no profile's id.
        /// </summary>
        public string Value => value ?? throw new InvalidOperationException("This ProfileId was never set.");

        /// <summary>Whether this is a real id (false for the default value, which stands for "nobody").</summary>
        public bool IsSet => value != null;

        /// <summary>
        /// The id <paramref name="text"/>; throws an <see cref="ArgumentException"/> when it isn't
        /// <c>[a-z0-9-]+</c>. Called by the Unity side for its built-in profile and by
        /// <c>ProfileIdTests</c>.
        /// </summary>
        public static ProfileId Create(string text)
        {
            if (!IsValidText(text))
            {
                throw new ArgumentException("A profile id is lower-case letters, digits and '-' only: '"
                    + text + "'.", nameof(text));
            }

            return new ProfileId(text);
        }

        /// <summary>
        /// Reads an id from <paramref name="text"/> (from a save file); false when it isn't
        /// <c>[a-z0-9-]+</c>. Called by the profile list's loading and by <c>ProfileIdTests</c>.
        /// </summary>
        public static bool TryCreate(string text, out ProfileId id)
        {
            id = IsValidText(text) ? new ProfileId(text) : default;
            return id.value != null;
        }

        /// <inheritdoc/>
        public bool Equals(ProfileId other)
        {
            return string.Equals(value, other.value, StringComparison.Ordinal);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is ProfileId other && Equals(other);
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
        public static bool operator ==(ProfileId left, ProfileId right)
        {
            return left.Equals(right);
        }

        /// <summary>Whether two ids are different.</summary>
        public static bool operator !=(ProfileId left, ProfileId right)
        {
            return !left.Equals(right);
        }

        private static bool IsValidText(string text)
        {
            return StableIdText.IsValid(text);
        }
    }
}
