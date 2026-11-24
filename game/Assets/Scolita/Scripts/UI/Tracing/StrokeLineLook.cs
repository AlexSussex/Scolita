namespace Scolita.UI.Tracing
{
    /// <summary>How a <see cref="StrokeLineGraphic"/> draws its lines.</summary>
    public enum StrokeLineLook
    {
        /// <summary>An even line in the graphic's colour (the faint shadow, the arrow, the stroke drawing itself).</summary>
        Solid,

        /// <summary>A line whose colour runs through the rainbow along it (her trail and the strokes she finished).</summary>
        Rainbow,

        /// <summary>Round dots along the line (the dotted guide).</summary>
        Dotted
    }
}
