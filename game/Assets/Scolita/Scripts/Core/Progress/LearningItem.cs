using System;
using Scolita.Core.Content;
using Scolita.Core.Languages;

namespace Scolita.Core.Progress
{
    /// <summary>
    /// One thing she learns, in one language: the letter <c>m</c> in Romanian and the letter <c>m</c> in
    /// English are two items (their sounds differ), a word is an item in its own language.
    /// </summary>
    public readonly struct LearningItem : IEquatable<LearningItem>
    {
        /// <summary>Creates an item; called by <see cref="PractisedItems"/>, the save's reader and the tests.</summary>
        public LearningItem(ItemKind kind, ContentId id, Language language)
        {
            Kind = kind;
            Id = id;
            Language = language;
        }

        /// <summary>Its kind.</summary>
        public ItemKind Kind { get; }

        /// <summary>Its content id (a letter's or a word's).</summary>
        public ContentId Id { get; }

        /// <summary>Its language.</summary>
        public Language Language { get; }

        /// <summary>The letter <paramref name="id"/> in <paramref name="language"/>; called by Core's progress and the tests.</summary>
        public static LearningItem Letter(ContentId id, Language language)
        {
            return new LearningItem(ItemKind.Letter, id, language);
        }

        /// <inheritdoc/>
        public bool Equals(LearningItem other)
        {
            return Kind == other.Kind && Id == other.Id && Language == other.Language;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is LearningItem other && Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return ((Id.GetHashCode() * 31) + (int)Kind) * 31 + (int)Language;
        }

        /// <summary><c>letter m (ro)</c>, for logs and test messages.</summary>
        public override string ToString()
        {
            return ProgressCodes.CodeOf(Kind) + " " + Id + " (" + LanguageCodes.CodeOf(Language) + ")";
        }

        /// <summary>Whether two items are the same.</summary>
        public static bool operator ==(LearningItem left, LearningItem right)
        {
            return left.Equals(right);
        }

        /// <summary>Whether two items are different.</summary>
        public static bool operator !=(LearningItem left, LearningItem right)
        {
            return !left.Equals(right);
        }
    }
}
