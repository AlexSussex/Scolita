using System;

namespace Scolita.Core.Profiles
{
    /// <summary>
    /// The profiles' numbers the parent meets (filled from the learning rules asset, beside <c>ParentRules</c>): how many
    /// children the tablet can have, and how long a name can be (from 20 to 40 characters, so the parent area's „cel mult
    /// {0} de caractere” is always correct Romanian: a number from 20 on takes "de").
    /// </summary>
    public sealed class ProfileRules
    {
        /// <summary>
        /// Creates the numbers; throws an <see cref="ArgumentOutOfRangeException"/> for fewer than 2 children (the
        /// picker is for two or more) or more than 6 (the picker's room), or a name limit outside 20 to 40 characters.
        /// Called by the learning rules asset and the tests.
        /// </summary>
        public ProfileRules(int mostChildren, int nameMostCharacters)
        {
            if (mostChildren < 2 || mostChildren > 6)
            {
                throw new ArgumentOutOfRangeException(nameof(mostChildren), mostChildren,
                    "A tablet has room for 2 to 6 children's profiles.");
            }

            if (nameMostCharacters < 20 || nameMostCharacters > 40)
            {
                throw new ArgumentOutOfRangeException(nameof(nameMostCharacters), nameMostCharacters,
                    "A name's limit is 20 to 40 characters.");
            }

            MostChildren = mostChildren;
            NameMostCharacters = nameMostCharacters;
        }

        /// <summary>The most children's profiles on the tablet (6: "Who's playing?" shows them in two rows of three).</summary>
        public int MostChildren { get; }

        /// <summary>The most characters in a name (20), the spaces and hyphens between its parts included.</summary>
        public int NameMostCharacters { get; }
    }
}
