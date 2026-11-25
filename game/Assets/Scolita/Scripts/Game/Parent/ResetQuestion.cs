namespace Scolita.Game.Parent
{
    /// <summary>Which of the reset's two questions is asked (<see cref="ParentRunner"/>).</summary>
    public enum ResetQuestion
    {
        /// <summary>No question: the parent area as it is.</summary>
        None,
        /// <summary>"Ștergi progresul?", saying what will be lost and that the settings stay.</summary>
        First,
        /// <summary>"Sigur?": the progress can't be brought back.</summary>
        Second
    }
}
