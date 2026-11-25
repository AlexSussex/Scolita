namespace Scolita.Core.Profiles
{
    /// <summary>Why a typed name can't be a <see cref="ProfileName"/>; the parent area says it under the field.</summary>
    public enum ProfileNameProblem
    {
        /// <summary>The name can be used.</summary>
        None,

        /// <summary>It is longer than <see cref="ProfileRules.NameMostCharacters"/>.</summary>
        TooLong,

        /// <summary>It has something other than letters, with one space or hyphen between them.</summary>
        NotLetters
    }
}
