using System;
using Scolita.Core.Languages;

namespace Scolita.Core.Content
{
    /// <summary>
    /// A letter (later also a group of letters such as <c>ch</c>, or a digit): its two cases, how it
    /// stands between the alphabets (which gives its languages), its stroke data and, by the naming
    /// rule, its sound and name lines in each of its languages. One row of <c>letters.csv</c>.
    /// </summary>
    public sealed class Letter : IContentItem
    {
        /// <summary>Creates a letter; called by <see cref="ContentTables"/>, the letter asset and tests.</summary>
        public Letter(ContentId id, string lower, string upper, LetterGroup group, ContentId strokes, bool retired,
            ContentSource source)
        {
            Id = id;
            Lower = lower ?? string.Empty;
            Upper = upper ?? string.Empty;
            Group = group;
            Strokes = strokes;
            Retired = retired;
            Source = source;
        }

        /// <inheritdoc/>
        public ContentId Id { get; }

        /// <summary>The letter in lower case (<c>m</c>, <c>ă</c>, <c>ch</c>).</summary>
        public string Lower { get; }

        /// <summary>The letter in upper case (<c>M</c>, <c>Ă</c>, <c>Ch</c>).</summary>
        public string Upper { get; }

        /// <summary>How it stands between the two alphabets.</summary>
        public LetterGroup Group { get; }

        /// <summary>The id of its stroke data (its format is <c>tracing</c>'s); not set when it has none yet.</summary>
        public ContentId Strokes { get; }

        /// <inheritdoc/>
        public bool Retired { get; }

        /// <inheritdoc/>
        public ContentSource Source { get; }

        /// <summary>The languages the letter belongs to, from its <see cref="Group"/>.</summary>
        public LanguageMix Languages
        {
            get
            {
                switch (Group)
                {
                    case LetterGroup.RomanianOnly:
                        return LanguageMix.RomanianOnly;
                    case LetterGroup.EnglishOnly:
                        return LanguageMix.EnglishOnly;
                    default:
                        return LanguageMix.Both;
                }
            }
        }

        /// <summary>Whether the letter belongs to <paramref name="language"/>; called by the content rules.</summary>
        public bool HasLanguage(Language language)
        {
            return Languages.Includes(language);
        }

        /// <summary>
        /// The line of its sound in <paramref name="language"/> (<c>ro-sound-m</c>); throws an
        /// <see cref="ArgumentException"/> for a language the letter doesn't belong to. Called by the
        /// content rules and, later, the activities.
        /// </summary>
        public VoiceLineId SoundLine(Language language)
        {
            return LineIn(language, VoiceLineKind.Sound);
        }

        /// <summary>
        /// The line of its name in <paramref name="language"/> (<c>ro-name-m</c>); throws an
        /// <see cref="ArgumentException"/> for a language the letter doesn't belong to. Called by the
        /// content rules and, later, the activities.
        /// </summary>
        public VoiceLineId NameLine(Language language)
        {
            return LineIn(language, VoiceLineKind.Name);
        }

        private VoiceLineId LineIn(Language language, VoiceLineKind kind)
        {
            if (!HasLanguage(language))
            {
                throw new ArgumentException("The letter " + Id + " has no " + LanguageCodes.CodeOf(language)
                    + " sound or name.", nameof(language));
            }

            return VoiceLineIds.For(language, kind, Id.Value);
        }
    }
}
