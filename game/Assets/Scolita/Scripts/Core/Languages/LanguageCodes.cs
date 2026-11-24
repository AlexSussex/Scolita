using System;

namespace Scolita.Core.Languages
{
    /// <summary>
    /// The stable codes of <see cref="Language"/> and <see cref="LanguageMix"/> in save files and
    /// content (<c>ro</c>, <c>en</c>, <c>both</c>), so renaming an enum value never changes a file.
    /// </summary>
    public static class LanguageCodes
    {
        /// <summary>The code of <see cref="Language.Romanian"/>.</summary>
        public const string Romanian = "ro";

        /// <summary>The code of <see cref="Language.English"/>.</summary>
        public const string English = "en";

        /// <summary>The code of <see cref="LanguageMix.Both"/>.</summary>
        public const string Both = "both";

        /// <summary>
        /// The code of <paramref name="language"/>. Called by the save files and by
        /// <c>LanguageCodesTests</c>.
        /// </summary>
        public static string CodeOf(Language language)
        {
            switch (language)
            {
                case Language.Romanian:
                    return Romanian;
                case Language.English:
                    return English;
                default:
                    throw new ArgumentOutOfRangeException(nameof(language), language, "Unknown language.");
            }
        }

        /// <summary>
        /// The code of <paramref name="mix"/>: <c>both</c>, or the code of its only language.
        /// Called by the save files and by <c>LanguageCodesTests</c>.
        /// </summary>
        public static string CodeOf(LanguageMix mix)
        {
            switch (mix)
            {
                case LanguageMix.Both:
                    return Both;
                case LanguageMix.RomanianOnly:
                    return Romanian;
                case LanguageMix.EnglishOnly:
                    return English;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mix), mix, "Unknown language mix.");
            }
        }

        /// <summary>
        /// Reads a language from its exact code; false for anything else (an unknown code is never
        /// guessed). Called by the save files and by <c>LanguageCodesTests</c>.
        /// </summary>
        public static bool TryParseLanguage(string code, out Language language)
        {
            switch (code)
            {
                case Romanian:
                    language = Language.Romanian;
                    return true;
                case English:
                    language = Language.English;
                    return true;
                default:
                    language = default;
                    return false;
            }
        }

        /// <summary>
        /// Reads a language mix from its exact code; false for anything else. Called by the save
        /// files and by <c>LanguageCodesTests</c>.
        /// </summary>
        public static bool TryParseMix(string code, out LanguageMix mix)
        {
            switch (code)
            {
                case Both:
                    mix = LanguageMix.Both;
                    return true;
                case Romanian:
                    mix = LanguageMix.RomanianOnly;
                    return true;
                case English:
                    mix = LanguageMix.EnglishOnly;
                    return true;
                default:
                    mix = default;
                    return false;
            }
        }
    }
}
