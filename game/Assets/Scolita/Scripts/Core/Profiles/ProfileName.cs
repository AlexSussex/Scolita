using System;
using System.Text;

namespace Scolita.Core.Profiles
{
    /// <summary>
    /// A profile's name, as the parent typed it: letters (any alphabet's, so ă â î ș ț and every name), with one
    /// space or hyphen between its parts ("Ana-Maria", "Ana Maria"), at most <see cref="ProfileRules.NameMostCharacters"/>
    /// characters. Or <see cref="None"/>: a profile without a name (the app's first profile, until the parent gives it
    /// one), shown by its avatar alone and greeted without a name. A name is only ever said in the nominative
    /// („Bună, {name}!”), so it is never changed to fit a sentence.
    /// </summary>
    public readonly struct ProfileName : IEquatable<ProfileName>
    {
        private const char CedillaS = 'ş';
        private const char CedillaCapitalS = 'Ş';
        private const char CedillaT = 'ţ';
        private const char CedillaCapitalT = 'Ţ';
        private const char CommaS = 'ș';
        private const char CommaCapitalS = 'Ș';
        private const char CommaT = 'ț';
        private const char CommaCapitalT = 'Ț';

        private readonly string text;

        private ProfileName(string text)
        {
            this.text = text;
        }

        /// <summary>No name.</summary>
        public static ProfileName None => default;

        /// <summary>Whether the profile has no name.</summary>
        public bool IsNone => text == null;

        /// <summary>The name, or an empty text for <see cref="None"/>.</summary>
        public string Text => text ?? string.Empty;

        /// <summary>
        /// Reads <paramref name="typed"/> into <paramref name="name"/>: trimmed, its runs of spaces made one, a cedilla
        /// ş ţ (an old keyboard's) made the comma-below ș ț; empty is <see cref="None"/>. Returns why it can't be a name
        /// (and <paramref name="name"/> is <see cref="None"/> then), or <see cref="ProfileNameProblem.None"/>. Called by
        /// the parent area's name field, the profile list's loading and <c>ProfileNameTests</c>.
        /// </summary>
        public static ProfileNameProblem Read(string typed, ProfileRules rules, out ProfileName name)
        {
            if (rules == null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            name = None;
            string tidy = Tidy(typed);
            if (tidy.Length == 0)
            {
                return ProfileNameProblem.None;
            }

            if (!IsLettersWithSeparators(tidy))
            {
                return ProfileNameProblem.NotLetters;
            }

            if (tidy.Length > rules.NameMostCharacters)
            {
                return ProfileNameProblem.TooLong;
            }

            name = new ProfileName(tidy);
            return ProfileNameProblem.None;
        }

        /// <inheritdoc/>
        public bool Equals(ProfileName other)
        {
            return string.Equals(text, other.text, StringComparison.Ordinal);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is ProfileName other && Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return text == null ? 0 : StringComparer.Ordinal.GetHashCode(text);
        }

        /// <summary>The name, or an empty text for <see cref="None"/>.</summary>
        public override string ToString()
        {
            return Text;
        }

        /// <summary>Whether two names are the same.</summary>
        public static bool operator ==(ProfileName left, ProfileName right)
        {
            return left.Equals(right);
        }

        /// <summary>Whether two names are different.</summary>
        public static bool operator !=(ProfileName left, ProfileName right)
        {
            return !left.Equals(right);
        }

        private static string Tidy(string typed)
        {
            if (string.IsNullOrWhiteSpace(typed))
            {
                return string.Empty;
            }

            string composed = typed.Trim().Normalize(NormalizationForm.FormC);
            StringBuilder tidy = new StringBuilder(composed.Length);
            bool lastWasSpace = false;
            foreach (char character in composed)
            {
                bool isSpace = char.IsWhiteSpace(character);
                if (isSpace && lastWasSpace)
                {
                    continue;
                }

                tidy.Append(isSpace ? ' ' : CommaBelow(character));
                lastWasSpace = isSpace;
            }

            return tidy.ToString();
        }

        private static char CommaBelow(char character)
        {
            switch (character)
            {
                case CedillaS:
                    return CommaS;
                case CedillaCapitalS:
                    return CommaCapitalS;
                case CedillaT:
                    return CommaT;
                case CedillaCapitalT:
                    return CommaCapitalT;
                default:
                    return character;
            }
        }

        private static bool IsLettersWithSeparators(string tidy)
        {
            bool lastWasLetter = false;
            foreach (char character in tidy)
            {
                if (char.IsLetter(character))
                {
                    lastWasLetter = true;
                    continue;
                }

                if ((character != ' ' && character != '-') || !lastWasLetter)
                {
                    return false;
                }

                lastWasLetter = false;
            }

            return lastWasLetter;
        }
    }
}
