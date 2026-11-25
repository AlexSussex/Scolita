namespace Scolita.Game.Parent
{
    /// <summary>What the profiles page says after a change, under its buttons.</summary>
    public enum ProfilesMessage
    {
        /// <summary>Nothing.</summary>
        None,

        /// <summary>The profile was saved.</summary>
        Saved,

        /// <summary>The profile could not be saved; it stays as it was.</summary>
        NotSaved,

        /// <summary>The profile and its folder were deleted.</summary>
        Deleted,

        /// <summary>The profile is off the list, but some of its files could not be deleted.</summary>
        FilesLeft,

        /// <summary>The tester's progress was deleted.</summary>
        TesterReset,

        /// <summary>The tester's progress could not all be deleted.</summary>
        TesterNotReset
    }
}
