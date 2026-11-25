using Scolita.Core.Tracing;
using UnityEngine;

namespace Scolita.UI.Tracing
{
    /// <summary>
    /// How a <see cref="TraceFigure"/> sits in a rectangle: its writing lines (top to the line below) over a share of
    /// the rectangle's height, its width within a share of its width, centred. One place for the board
    /// (<see cref="WritingBoardView"/>) and <see cref="HandwritingView"/>, so a letter by hand is placed alike in both.
    /// </summary>
    public readonly struct FigureFit
    {
        /// <summary>The narrowest width a figure is fitted as, in a shape's units (a one-stroke letter isn't blown up).</summary>
        public const float NarrowestWidth = 100f;

        private const float LinesSpan = WritingLines.Below - WritingLines.Top;

        /// <summary>The middle of the writing lines, in a shape's units: the rectangle's centre.</summary>
        public const float LinesMiddle = (WritingLines.Top + WritingLines.Below) * 0.5f;

        private FigureFit(float scale, float centreX)
        {
            Scale = scale;
            CentreX = centreX;
        }

        /// <summary>Pixels per shape unit.</summary>
        public float Scale { get; }

        /// <summary>The figure's middle x, in a shape's units: the rectangle's centre.</summary>
        public float CentreX { get; }

        /// <summary>
        /// The fit of <paramref name="figure"/> in <paramref name="area"/>: the lines over <paramref name="heightShare"/> of
        /// its height, the figure within <paramref name="widthShare"/> of its width. Called by the board and the
        /// handwriting view when they show a figure.
        /// </summary>
        public static FigureFit Of(TraceFigure figure, Rect area, float heightShare, float widthShare)
        {
            float width = Mathf.Max(figure.MaxX - figure.MinX, NarrowestWidth);
            float scale = Mathf.Min(area.height * heightShare / LinesSpan, area.width * widthShare / width);
            return new FigureFit(scale, (figure.MinX + figure.MaxX) * 0.5f);
        }

        /// <summary>Where <paramref name="point"/> (in a shape's units) is, in the rectangle's local space; called by the views.</summary>
        public Vector2 Local(ShapePoint point)
        {
            return new Vector2((point.X - CentreX) * Scale, (LinesMiddle - point.Y) * Scale);
        }

        /// <summary>The shape point at <paramref name="local"/> (in the rectangle's local space); called by the board for her finger.</summary>
        public ShapePoint ShapeAt(Vector2 local)
        {
            return new ShapePoint(local.x / Scale + CentreX, LinesMiddle - local.y / Scale);
        }
    }
}
