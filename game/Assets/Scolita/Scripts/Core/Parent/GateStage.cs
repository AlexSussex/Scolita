namespace Scolita.Core.Parent
{
    /// <summary>Where the parent gate is (<see cref="ParentGate"/>).</summary>
    public enum GateStage
    {
        /// <summary>Shut: the parent button is not held.</summary>
        Shut,
        /// <summary>The parent button is held; the ring fills.</summary>
        Holding,
        /// <summary>The number is said and the numbers are shown; one is to be chosen.</summary>
        Asking,
        /// <summary>The right number was chosen: the parent area opens.</summary>
        Open
    }
}
