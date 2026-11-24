namespace Scolita.Core.Activities
{
    /// <summary>
    /// Where one choice of a round stands (<see cref="ActivityRound.StateOf"/>), set by the step's
    /// rules so a view draws Core's state and keeps none of its own.
    /// </summary>
    public enum ChoiceState
    {
        /// <summary>Nothing has happened to it yet.</summary>
        Open,

        /// <summary>She has heard it (<c>TwoSounds</c>' planets, before the sorting starts).</summary>
        Heard,

        /// <summary>It is done with: a star popped, a picture sorted.</summary>
        Done
    }
}
