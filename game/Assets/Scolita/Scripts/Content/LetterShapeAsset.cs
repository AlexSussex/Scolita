using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Tracing;
using UnityEngine;

namespace Scolita.Content
{
    /// <summary>
    /// A <see cref="LetterShape"/> as an asset (<c>Content/Strokes/&lt;id&gt;.asset</c>), written by the
    /// content importer from its file in <c>content/strokes/</c> and never edited by hand.
    /// </summary>
    public sealed class LetterShapeAsset : ScriptableObject, IValidatableContent
    {
        [SerializeField] private string id;
        [SerializeField] private string strokesId;
        [SerializeField] private string letterCase;
        [SerializeField] private string form;
        [SerializeField] private string status;
        [SerializeField] private float slant;
        [SerializeField] private StrokeData[] strokes;

        /// <summary>The shape's id.</summary>
        public string Id => id;

        /// <summary>Writes <paramref name="shape"/> into this asset; called by the content importer.</summary>
        public void Fill(LetterShape shape)
        {
            id = shape.Id.Value;
            strokesId = shape.StrokesId.Value;
            letterCase = LetterShapeCodes.CodeOf(shape.Case);
            form = LetterShapeCodes.CodeOf(shape.Form);
            status = LetterShapeCodes.CodeOf(shape.Status);
            slant = shape.Slant;
            strokes = shape.Strokes.Select(stroke => new StrokeData(stroke)).ToArray();
        }

        /// <summary>
        /// The shape for Core; throws an <see cref="System.InvalidOperationException"/> if the asset can't be
        /// read. Called by <see cref="CurriculumAssets.Build"/>.
        /// </summary>
        public LetterShape ToShape()
        {
            AssetFields fields = new AssetFields(name);
            LetterShape shape = Read(fields);
            fields.ThrowIfAny();
            return shape;
        }

        /// <inheritdoc/>
        public IReadOnlyList<string> Validate()
        {
            AssetFields fields = new AssetFields(name);
            Read(fields);
            return fields.Problems;
        }

        private LetterShape Read(AssetFields fields)
        {
            ContentId readStrokesId = fields.Id(strokesId, "strokes id");
            if (!LetterShapeCodes.TryParseCase(letterCase, out LetterCase readCase))
            {
                fields.Problem("Its case '" + letterCase + "' is not lower or upper.");
            }

            if (!LetterShapeCodes.TryParseForm(form, out LetterForm readForm))
            {
                fields.Problem("Its form '" + form + "' is not print or handwritten.");
            }

            if (!LetterShapeCodes.TryParseStatus(status, out ShapeStatus readStatus))
            {
                fields.Problem("Its status '" + status + "' is not draft or checked.");
            }

            List<Stroke> read = (strokes ?? new StrokeData[0])
                .Select(stroke => stroke?.ToStroke(slant, fields))
                .ToList();
            if (read.Count == 0 || read.Any(stroke => stroke == null))
            {
                fields.Problem("A stroke of it is missing or can't be read.");
            }

            if (fields.Problems.Count > 0)
            {
                return null;
            }

            LetterShape shape = new LetterShape(readStrokesId, readCase, readForm, readStatus, slant, read, fields.Source);
            if (shape.Id.Value != id || id != name)
            {
                fields.Problem("Its id " + id + " is not its strokes id, case and form, or not its name " + name + ".");
            }

            return shape;
        }
    }
}
