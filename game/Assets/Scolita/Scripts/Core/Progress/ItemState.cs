namespace Scolita.Core.Progress
{
    /// <summary>How far she is with an item (<c>docs/curriculum.md</c> §5), as the parent area's grid shows it.</summary>
    public enum ItemState
    {
        /// <summary>Never met: no progress is kept for it.</summary>
        New,

        /// <summary>Met or practised, not learned yet.</summary>
        Learning,

        /// <summary>Learned (<see cref="Mastery"/>); it stays learned and comes back for reviews.</summary>
        Learned
    }
}
