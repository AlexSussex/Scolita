namespace Scolita.Core.Profiles
{
    /// <summary>What a <see cref="Profile"/> is for: a child who plays, or the parent testing the app.</summary>
    public enum ProfileKind
    {
        /// <summary>A child: shown on "Who's playing?", plays the lessons in their order.</summary>
        Child,

        /// <summary>
        /// The parent as a tester (the human): opened only from the parent area, never shown to the children; every
        /// lesson open, steps can be skipped, the test information shown.
        /// </summary>
        Tester
    }
}
