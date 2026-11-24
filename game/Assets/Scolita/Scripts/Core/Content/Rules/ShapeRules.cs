using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Languages;
using Scolita.Core.Tracing;

namespace Scolita.Core.Content.Rules
{
    /// <summary>
    /// The rules on the letters' shapes (<c>content/strokes/</c>): each shape belongs to a letter and has
    /// at most <see cref="ActivityLimits.MostStrokes"/> strokes; a draft shape is a warning (the human
    /// checks it against her manual); a <c>TraceLetter</c> step's letter has the shape it asks for (an error for print; a warning for a handwritten one, which is
    /// skipped until <c>handwriting</c> draws it); and each of its strokes after the first has its
    /// checkpoint line ("Acum mergi la punctul cu numărul doi."). Part of <see cref="ContentRules"/>.
    /// </summary>
    public static class ShapeRules
    {
        /// <summary>Checks the shapes and the tracing steps; called by <see cref="ContentRules.Check"/>.</summary>
        public static void Check(Curriculum curriculum, ContentProblems problems)
        {
            HashSet<ContentId> strokesIds = new HashSet<ContentId>(curriculum.Letters.All
                .Where(letter => letter.Strokes.IsSet)
                .Select(letter => letter.Strokes));
            foreach (LetterShape shape in curriculum.Shapes.All)
            {
                if (!strokesIds.Contains(shape.StrokesId))
                {
                    problems.Error(shape.Source, "no letter has the strokes id " + shape.StrokesId
                        + " (the strokes cell of letters.csv).");
                }

                if (shape.Strokes.Count > ActivityLimits.MostStrokes)
                {
                    problems.Error(shape.Source, "the shape has " + shape.Strokes.Count + " strokes; the writing board holds "
                        + ActivityLimits.MostStrokes + ".");
                }

                if (shape.Status == ShapeStatus.Draft)
                {
                    problems.Warning(shape.Source, "the shape is a draft: once it is checked against her school's "
                        + "manual, write \"status\": \"checked\".");
                }
            }

            IEnumerable<Step> tracingSteps = curriculum.Domains
                .SelectMany(domain => domain.Units)
                .Where(unit => !unit.Retired)
                .SelectMany(unit => unit.Lessons)
                .Where(lesson => !lesson.Retired)
                .SelectMany(lesson => lesson.Steps)
                .Where(step => step.Kind == ActivityKind.TraceLetter);
            foreach (Step step in tracingSteps)
            {
                CheckStep(curriculum, step, problems);
            }
        }

        private static void CheckStep(Curriculum curriculum, Step step, ContentProblems problems)
        {
            if (step.Letters.Count != 1 || !curriculum.Letters.TryGet(step.Letters[0], out Letter letter)
                || !LetterShapeCodes.TryGetStepShape(step, out LetterCase letterCase, out LetterForm form))
            {
                return;
            }

            if (!curriculum.TryGetShape(letter, letterCase, form, out LetterShape shape))
            {
                string wanted = letter.Strokes.IsSet
                    ? LetterShapeCodes.FileNameOf(LetterShapeCodes.IdOf(letter.Strokes, letterCase, form))
                    : "a strokes id in letters.csv and its shape file";
                if (form == LetterForm.Print)
                {
                    problems.Error(step.Source, "the letter " + letter.Id + " has no print shape for this step: it needs "
                        + wanted + ".");
                }
                else
                {
                    problems.Warning(step.Source, "the letter " + letter.Id + " has no " + LetterShapeCodes.CodeOf(form)
                        + " shape yet (" + wanted + "): the step is skipped until it has one.");
                }

                return;
            }

            foreach (Language language in step.Languages.Languages())
            {
                for (int stroke = 2; stroke <= shape.Strokes.Count; stroke++)
                {
                    VoiceLineRules.Line(curriculum, VoiceLineIds.For(language, VoiceLineKind.Line, "trace-next-" + stroke),
                        step.Source, problems);
                }
            }
        }
    }
}
