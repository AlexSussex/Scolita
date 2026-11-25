namespace Scolita.Core.Rewards
{
    /// <summary>
    /// A point of a constellation in its <see cref="Constellation.FrameWidth"/> × <see cref="Constellation.FrameHeight"/>
    /// frame, x to the right and y down from the top left.
    /// </summary>
    public readonly struct StarPoint
    {
        /// <summary>Creates a point; called by <see cref="RewardTables"/>, <see cref="ConstellationPlaces"/> and the tests.</summary>
        public StarPoint(float x, float y)
        {
            X = x;
            Y = y;
        }

        /// <summary>From the frame's left edge.</summary>
        public float X { get; }

        /// <summary>From the frame's top edge.</summary>
        public float Y { get; }
    }
}
