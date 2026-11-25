namespace Scolita.Core.Map
{
    /// <summary>A letter on a planet's tiles.</summary>
    public enum LetterState
    {
        /// <summary>Introduced by a stop before the next one: shown in full.</summary>
        Met,

        /// <summary>The next stop introduces it: ringed in yellow.</summary>
        Current,

        /// <summary>Still to come: faint.</summary>
        Ahead
    }
}
