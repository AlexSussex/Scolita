namespace Scolita.Core.Rewards
{
    /// <summary>A line of a constellation's figure between two of its stars, by their index from 0.</summary>
    public readonly struct StarLine
    {
        /// <summary>Creates a line; called by <see cref="RewardTables"/>, the constellation asset and the tests.</summary>
        public StarLine(int from, int to)
        {
            From = from;
            To = to;
        }

        /// <summary>The index of the star it starts at.</summary>
        public int From { get; }

        /// <summary>The index of the star it ends at.</summary>
        public int To { get; }
    }
}
