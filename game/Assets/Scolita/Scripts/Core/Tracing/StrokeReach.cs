namespace Scolita.Core.Tracing
{
    /// <summary>
    /// How far along a path her line went (<see cref="StrokeCheck.Reach"/>): from where she first reached it to how
    /// far she had reached before she strayed from it, whether she began at its start, and how many of her points
    /// came before she strayed (all of them when she never did).
    /// </summary>
    public readonly struct StrokeReach
    {
        /// <summary>Creates a reach; called by <see cref="StrokeCheck.Reach"/>.</summary>
        public StrokeReach(float from, float to, bool startedRight, int pointsOnPath)
        {
            From = from;
            To = to;
            StartedRight = startedRight;
            PointsOnPath = pointsOnPath;
        }

        /// <summary>The distance along the path where her first point reached it.</summary>
        public float From { get; }

        /// <summary>The distance along the path she had reached before she strayed from it.</summary>
        public float To { get; }

        /// <summary>Whether her first point was within the start radius of the path's start.</summary>
        public bool StartedRight { get; }

        /// <summary>How many of her points came before she strayed from the path.</summary>
        public int PointsOnPath { get; }
    }
}
