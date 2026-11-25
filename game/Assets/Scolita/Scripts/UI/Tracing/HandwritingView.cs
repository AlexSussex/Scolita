using Scolita.Core.Tracing;
using UnityEngine;

namespace Scolita.UI.Tracing
{
    /// <summary>
    /// Draws a letter or a word by hand from its stroke data (<see cref="TraceFigure"/>), never from a cursive font
    /// (decision D3): each stroke's sampled path, the same samples the check follows, as an even pen line in ink
    /// <c>#2A2140</c> (the <see cref="StrokeLineGraphic"/> beside it), fitted into its rectangle on the writing lines. It can also
    /// write the figure itself at a pen's pace (<c>MeetLetter</c>'s "de mână" button). Drawing allocates nothing.
    /// </summary>
    [RequireComponent(typeof(StrokeLineGraphic))]
    public sealed class HandwritingView : MonoBehaviour
    {
        [SerializeField, Min(10f)] private float penSpeed = 220f;
        [SerializeField, Range(0.3f, 1f)] private float heightShare = 0.9f;

        private StrokeLineGraphic pen;
        private TraceFigure figure;
        private FigureFit fit;
        private float totalLength;
        private float written;
        private bool writing;

        /// <summary>Whether it is writing the figure itself.</summary>
        public bool IsWriting => writing;

        /// <summary>The pen line it draws; read by the PlayMode tests to compare its points with the stroke data.</summary>
        public StrokeLineGraphic Pen => pen;

        private void Awake()
        {
            pen = GetComponent<StrokeLineGraphic>();
        }

        /// <summary>
        /// Draws <paramref name="shown"/> whole, fitted into the rectangle; called by the activity views and the tests.
        /// </summary>
        public void Show(TraceFigure shown)
        {
            Fit(shown);
            writing = false;
            DrawUpTo(totalLength);
        }

        /// <summary>
        /// Writes <paramref name="shown"/> itself, from its first stroke, at the pen's pace; called by the activity views
        /// and the tests.
        /// </summary>
        public void Write(TraceFigure shown)
        {
            Fit(shown);
            written = 0f;
            writing = true;
            DrawUpTo(0f);
        }

        /// <summary>Clears the line; called by the activity views when they hide.</summary>
        public void Clear()
        {
            writing = false;
            figure = null;
            pen.Clear();
        }

        /// <summary>
        /// Where <paramref name="point"/> (in the shape's units) is drawn, in the pen's local space; used by the PlayMode
        /// tests to compare the line with the stroke data.
        /// </summary>
        public Vector2 Local(ShapePoint point)
        {
            return fit.Local(point);
        }

        private void Update()
        {
            if (!writing)
            {
                return;
            }

            written += penSpeed * Time.unscaledDeltaTime;
            if (written >= totalLength)
            {
                written = totalLength;
                writing = false;
            }

            DrawUpTo(written);
        }

        private void Fit(TraceFigure shown)
        {
            figure = shown;
            fit = FigureFit.Of(shown, pen.rectTransform.rect, heightShare, heightShare);
            totalLength = 0f;
            for (int stroke = 0; stroke < shown.Strokes.Count; stroke++)
            {
                totalLength += shown.Strokes[stroke].Path.Length;
            }
        }

        private void DrawUpTo(float length)
        {
            pen.Clear();
            if (figure == null)
            {
                return;
            }

            float before = 0f;
            for (int stroke = 0; stroke < figure.Strokes.Count && before <= length; stroke++)
            {
                StrokePath path = figure.Strokes[stroke].Path;
                pen.BeginLine();
                for (int sample = 0; sample < path.Count && before + path.DistanceAt(sample) <= length; sample++)
                {
                    pen.Add(Local(new ShapePoint(path.X(sample), path.Y(sample))));
                }

                if (path.IsDot)
                {
                    pen.Add(Local(path.Start) + Vector2.right * 0.5f);
                }

                before += path.Length;
            }
        }
    }
}
