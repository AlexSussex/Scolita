namespace Scolita.Core.Tracing
{
    /// <summary>
    /// A point of a letter's shape, in the writing lines' units (<see cref="WritingLines"/>: x from the
    /// letter's left, y down from the top line).
    /// </summary>
    public readonly struct ShapePoint
    {
        /// <summary>Creates a point; called by <see cref="LetterShapeReader"/>, the shape asset and the tests.</summary>
        public ShapePoint(float x, float y)
        {
            X = x;
            Y = y;
        }

        /// <summary>Across, from the letter's left.</summary>
        public float X { get; }

        /// <summary>Down, from the top line.</summary>
        public float Y { get; }
    }
}
