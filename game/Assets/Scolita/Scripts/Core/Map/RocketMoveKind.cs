namespace Scolita.Core.Map
{
    /// <summary>How the rocket gets to its stop when the map opens (<see cref="RocketMove"/>).</summary>
    public enum RocketMoveKind
    {
        /// <summary>It is simply there: the first time, the same stop as before, or a stop behind the last one shown.</summary>
        Placed,

        /// <summary>It moves along the path to a later stop on the same planet.</summary>
        Hop,

        /// <summary>It flies to a later planet: the trip plays (skippable), then it lands on its stop.</summary>
        Trip
    }
}
