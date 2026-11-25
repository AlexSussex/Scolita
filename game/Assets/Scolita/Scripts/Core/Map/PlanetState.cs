namespace Scolita.Core.Map
{
    /// <summary>How a planet looks on the galaxy map (design/README.md → "Each planet is a character").</summary>
    public enum PlanetState
    {
        /// <summary>Ahead: dimmed, eyes closed, "z z z"; tapping it says "not yet".</summary>
        Sleepy,

        /// <summary>The planet of the next stop: eyes open, a ring, its letters standing below it.</summary>
        Awake,

        /// <summary>Every stop done: a happy face and a sparkle.</summary>
        Beaming
    }
}
