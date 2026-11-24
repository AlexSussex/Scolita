using System;
using System.Collections.Generic;
using Scolita.Core.Content;

namespace Scolita.Core.Tracing
{
    /// <summary>
    /// The stable codes of the letters' shapes: the case (<c>lower</c>, <c>upper</c>) and the form
    /// (<c>print</c>, <c>handwritten</c>), as a <c>TraceLetter</c> step's options and a shape file's name
    /// write them, the status (<c>draft</c>, <c>checked</c>) and the run kinds (<c>body</c>, <c>entry</c>,
    /// <c>exit</c>); a shape's id and the files' folder, <c>content/strokes/</c>.
    /// </summary>
    public static class LetterShapeCodes
    {
        /// <summary>The folder of the shape files in <c>content/</c>, and the prefix of their names in the content's texts.</summary>
        public const string Folder = "strokes";

        /// <summary>The ending of a shape file's name.</summary>
        public const string Extension = ".json";

        /// <summary>The <c>TraceLetter</c> step option that names the case.</summary>
        public const string CaseOption = "case";

        /// <summary>The <c>TraceLetter</c> step option that names the form.</summary>
        public const string FormOption = "form";

        private static readonly IReadOnlyList<KeyValuePair<string, LetterCase>> Cases = new[]
        {
            new KeyValuePair<string, LetterCase>("lower", LetterCase.Lower),
            new KeyValuePair<string, LetterCase>("upper", LetterCase.Upper)
        };

        private static readonly IReadOnlyList<KeyValuePair<string, LetterForm>> Forms = new[]
        {
            new KeyValuePair<string, LetterForm>("print", LetterForm.Print),
            new KeyValuePair<string, LetterForm>("handwritten", LetterForm.Handwritten)
        };

        private static readonly IReadOnlyList<KeyValuePair<string, ShapeStatus>> Statuses = new[]
        {
            new KeyValuePair<string, ShapeStatus>("draft", ShapeStatus.Draft),
            new KeyValuePair<string, ShapeStatus>("checked", ShapeStatus.Checked)
        };

        private static readonly IReadOnlyList<KeyValuePair<string, StrokeRunKind>> RunKinds = new[]
        {
            new KeyValuePair<string, StrokeRunKind>("body", StrokeRunKind.Body),
            new KeyValuePair<string, StrokeRunKind>("entry", StrokeRunKind.Entry),
            new KeyValuePair<string, StrokeRunKind>("exit", StrokeRunKind.Exit)
        };

        /// <summary>
        /// The id of the shape of <paramref name="strokesId"/> in <paramref name="letterCase"/> and
        /// <paramref name="form"/> (<c>a-lower-print</c>). Called by <see cref="LetterShape"/>,
        /// <c>Curriculum.TryGetShape</c> and the tests.
        /// </summary>
        public static ContentId IdOf(ContentId strokesId, LetterCase letterCase, LetterForm form)
        {
            return ContentId.Create(strokesId.Value + "-" + Code(Cases, letterCase) + "-" + Code(Forms, form));
        }

        /// <summary>
        /// The name of the shape <paramref name="shapeId"/>'s file in the content's texts
        /// (<c>strokes/a-lower-print.json</c>). Called by the importer and the tests.
        /// </summary>
        public static string FileNameOf(ContentId shapeId)
        {
            return Folder + "/" + shapeId.Value + Extension;
        }

        /// <summary>
        /// Whether <paramref name="fileName"/> (a name in the content's texts) is a shape file. Called by
        /// <see cref="ContentTables"/> and the importer.
        /// </summary>
        public static bool IsShapeFile(string fileName)
        {
            return IsShape(fileName);
        }

        private static bool IsShape(string fileName)
        {
            return fileName != null
                && fileName.StartsWith(Folder + "/", StringComparison.Ordinal)
                && fileName.EndsWith(Extension, StringComparison.Ordinal);
        }

        /// <summary>
        /// Reads a shape file's name (<c>strokes/a-lower-print.json</c>) into its letter's strokes id, its
        /// case and its form; false when it isn't of that pattern. Called by <see cref="LetterShapeReader"/>.
        /// </summary>
        public static bool TryParseFileName(string fileName, out ContentId strokesId, out LetterCase letterCase,
            out LetterForm form)
        {
            strokesId = default;
            letterCase = LetterCase.Lower;
            form = LetterForm.Print;
            if (!IsShape(fileName))
            {
                return false;
            }

            string id = fileName.Substring(Folder.Length + 1, fileName.Length - Folder.Length - 1 - Extension.Length);
            int formDash = id.LastIndexOf('-');
            if (formDash <= 0 || !TryParse(Forms, id.Substring(formDash + 1), out form))
            {
                return false;
            }

            int caseDash = id.LastIndexOf('-', formDash - 1);
            if (caseDash <= 0 || !TryParse(Cases, id.Substring(caseDash + 1, formDash - caseDash - 1), out letterCase))
            {
                return false;
            }

            return ContentId.TryCreate(id.Substring(0, caseDash), out strokesId);
        }

        /// <summary>The case written <paramref name="code"/>; called by the rules and the shape asset.</summary>
        public static bool TryParseCase(string code, out LetterCase letterCase)
        {
            return TryParse(Cases, code, out letterCase);
        }

        /// <summary>The form written <paramref name="code"/>; called by the rules and the shape asset.</summary>
        public static bool TryParseForm(string code, out LetterForm form)
        {
            return TryParse(Forms, code, out form);
        }

        /// <summary>The status written <paramref name="code"/>; called by <see cref="LetterShapeReader"/> and the shape asset.</summary>
        public static bool TryParseStatus(string code, out ShapeStatus status)
        {
            return TryParse(Statuses, code, out status);
        }

        /// <summary>The run kind written <paramref name="code"/>; called by <see cref="LetterShapeReader"/> and the shape asset.</summary>
        public static bool TryParseRunKind(string code, out StrokeRunKind kind)
        {
            return TryParse(RunKinds, code, out kind);
        }

        /// <summary>The code of <paramref name="letterCase"/>; called by the importer and the shape asset.</summary>
        public static string CodeOf(LetterCase letterCase)
        {
            return Code(Cases, letterCase);
        }

        /// <summary>The code of <paramref name="form"/>; called by the importer, the rules and the shape asset.</summary>
        public static string CodeOf(LetterForm form)
        {
            return Code(Forms, form);
        }

        /// <summary>The code of <paramref name="status"/>; called by the shape asset.</summary>
        public static string CodeOf(ShapeStatus status)
        {
            return Code(Statuses, status);
        }

        /// <summary>The code of <paramref name="kind"/>; called by the shape asset.</summary>
        public static string CodeOf(StrokeRunKind kind)
        {
            return Code(RunKinds, kind);
        }

        /// <summary>
        /// The case and form a <c>TraceLetter</c> <paramref name="step"/> asks for; false when an option is
        /// missing or unknown (the content rules report it). Called by the rules and the activities.
        /// </summary>
        public static bool TryGetStepShape(Step step, out LetterCase letterCase, out LetterForm form)
        {
            form = LetterForm.Print;
            letterCase = LetterCase.Lower;
            return step.TryGetOption(CaseOption, out string caseCode) && TryParse(Cases, caseCode, out letterCase)
                && step.TryGetOption(FormOption, out string formCode) && TryParse(Forms, formCode, out form);
        }

        private static bool TryParse<T>(IReadOnlyList<KeyValuePair<string, T>> codes, string code, out T value)
        {
            foreach (KeyValuePair<string, T> pair in codes)
            {
                if (pair.Key == code)
                {
                    value = pair.Value;
                    return true;
                }
            }

            value = default;
            return false;
        }

        private static string Code<T>(IReadOnlyList<KeyValuePair<string, T>> codes, T value)
        {
            foreach (KeyValuePair<string, T> pair in codes)
            {
                if (EqualityComparer<T>.Default.Equals(pair.Value, value))
                {
                    return pair.Key;
                }
            }

            throw new ArgumentOutOfRangeException(nameof(value), value, "No code for this value.");
        }
    }
}
