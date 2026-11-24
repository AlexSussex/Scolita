using System;
using System.Collections.Generic;
using Scolita.Core.Content;

namespace Scolita.Core.Tracing
{
    /// <summary>
    /// How one letter is written in one case and form: its strokes in order, on the writing lines
    /// (<see cref="WritingLines"/>), leaned by its slant. One file of <c>content/strokes/</c>
    /// (<c>a-lower-print.json</c>), its id the file's name; a letter finds its shapes by its
    /// <see cref="Letter.Strokes"/> id. Shapes are what <c>TraceLetter</c> traces, and what the
    /// handwritten letters, the joined words and the digits of later domains are drawn from.
    /// </summary>
    public sealed class LetterShape : IContentItem
    {
        /// <summary>
        /// Creates a shape; throws an <see cref="ArgumentException"/> for no stroke. Called by
        /// <see cref="LetterShapeReader"/>, the shape asset and the tests.
        /// </summary>
        public LetterShape(ContentId strokesId, LetterCase letterCase, LetterForm form, ShapeStatus status, float slant,
            IReadOnlyList<Stroke> strokes, ContentSource source)
        {
            if (strokes == null || strokes.Count == 0)
            {
                throw new ArgumentException("A shape has at least one stroke.", nameof(strokes));
            }

            StrokesId = strokesId;
            Case = letterCase;
            Form = form;
            Id = LetterShapeCodes.IdOf(strokesId, letterCase, form);
            Status = status;
            Slant = slant;
            Strokes = strokes;
            Source = source;
            MinX = float.MaxValue;
            MaxX = float.MinValue;
            foreach (Stroke stroke in strokes)
            {
                for (int index = 0; index < stroke.Path.Count; index++)
                {
                    MinX = Math.Min(MinX, stroke.Path.X(index));
                    MaxX = Math.Max(MaxX, stroke.Path.X(index));
                }
            }
        }

        /// <summary>Its id: <c>&lt;strokes id&gt;-&lt;case&gt;-&lt;form&gt;</c> (<c>a-lower-print</c>).</summary>
        public ContentId Id { get; }

        /// <summary>The stroke data id of the letters it belongs to (<see cref="Letter.Strokes"/>).</summary>
        public ContentId StrokesId { get; }

        /// <summary>Its case.</summary>
        public LetterCase Case { get; }

        /// <summary>Its form.</summary>
        public LetterForm Form { get; }

        /// <summary>Whether it has been checked against her manual.</summary>
        public ShapeStatus Status { get; }

        /// <summary>How much it leans: x += (baseline - y) * slant (0 for print, 0.2 for handwritten letters).</summary>
        public float Slant { get; }

        /// <summary>Its strokes, in the order she draws them.</summary>
        public IReadOnlyList<Stroke> Strokes { get; }

        /// <summary>Its leftmost x, slanted (the view centres the letter on the board with it).</summary>
        public float MinX { get; }

        /// <summary>Its rightmost x, slanted.</summary>
        public float MaxX { get; }

        /// <summary>A shape is never retired: her progress never names it, so it is replaced instead.</summary>
        public bool Retired => false;

        /// <inheritdoc/>
        public ContentSource Source { get; }
    }
}
