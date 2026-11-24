using System;
using System.Collections.Generic;

namespace Scolita.Core.Languages
{
    /// <summary>
    /// What a <see cref="LanguageMix"/> holds: a profile's mix (which languages she learns) and, in
    /// the content, the languages of a letter, a lesson or a step.
    /// </summary>
    public static class LanguageMixExtensions
    {
        private static readonly IReadOnlyList<Language> RomanianAlone = new[] { Language.Romanian };
        private static readonly IReadOnlyList<Language> EnglishAlone = new[] { Language.English };
        private static readonly IReadOnlyList<Language> BothLanguages = new[] { Language.Romanian, Language.English };

        /// <summary>
        /// Whether <paramref name="mix"/> holds <paramref name="language"/>. Called by the content
        /// model (<c>Letter.HasLanguage</c>, the curriculum's tracks), the content rules and
        /// <c>LanguageMixExtensionsTests</c>.
        /// </summary>
        public static bool Includes(this LanguageMix mix, Language language)
        {
            return mix == LanguageMix.Both
                || (mix == LanguageMix.RomanianOnly && language == Language.Romanian)
                || (mix == LanguageMix.EnglishOnly && language == Language.English);
        }

        /// <summary>
        /// The languages of <paramref name="mix"/>, Romanian first. Called by the content rules (a
        /// step's instruction lines in each of its languages) and <c>LanguageMixExtensionsTests</c>.
        /// </summary>
        public static IReadOnlyList<Language> Languages(this LanguageMix mix)
        {
            switch (mix)
            {
                case LanguageMix.Both:
                    return BothLanguages;
                case LanguageMix.RomanianOnly:
                    return RomanianAlone;
                case LanguageMix.EnglishOnly:
                    return EnglishAlone;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mix), mix, "Unknown language mix.");
            }
        }

        /// <summary>
        /// The mix holding only <paramref name="language"/>. Called by the content model
        /// (<c>Word.Languages</c>) and <c>LanguageMixExtensionsTests</c>.
        /// </summary>
        public static LanguageMix Alone(this Language language)
        {
            switch (language)
            {
                case Language.Romanian:
                    return LanguageMix.RomanianOnly;
                case Language.English:
                    return LanguageMix.EnglishOnly;
                default:
                    throw new ArgumentOutOfRangeException(nameof(language), language, "Unknown language.");
            }
        }
    }
}
