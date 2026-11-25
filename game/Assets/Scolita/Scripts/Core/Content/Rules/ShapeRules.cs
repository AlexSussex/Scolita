using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Scolita.Core.Languages;
using Scolita.Core.Tracing;

namespace Scolita.Core.Content.Rules
{
    /// <summary>
    /// The rules on the letters' shapes (<c>content/strokes/</c>): each shape belongs to a letter and has
    /// at most <see cref="ActivityLimits.MostStrokes"/> strokes; a draft shape is a warning (the human
    /// checks it against her manual); a print shape has only body runs; a handwritten shape's entry is the first
    /// run and its exit the last run of its line, one of each at most; a handwritten lower-case shape's entry starts
    /// on the baseline, its exit ends between the middle line and the baseline, the strokes of its line continue
    /// each other (so it is written in one line and can be joined), and all of them lean alike; a <c>TraceLetter</c> step's letter has the shape it asks for, and each of its strokes after the
    /// first has its checkpoint line ("Acum mergi la punctul cu numărul doi."); a <c>TraceWord</c> step's word has
    /// at most <see cref="ActivityLimits.MostTracedLetters"/> letters, each with its handwritten lower-case shape,
    /// all but the last with an exit, and a line for each of its dots and bars. Part of <see cref="ContentRules"/>.
    /// </summary>
    public static class ShapeRules
    {
        private const float OnTheLineWithin = 4f;

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

                CheckRuns(shape, problems);
            }

            CheckSlants(curriculum, problems);

            IEnumerable<Step> tracingSteps = curriculum.Domains
                .SelectMany(domain => domain.Units)
                .Where(unit => !unit.Retired)
                .SelectMany(unit => unit.Lessons)
                .Where(lesson => !lesson.Retired)
                .SelectMany(lesson => lesson.Steps)
                .Where(step => step.Kind == ActivityKind.TraceLetter || step.Kind == ActivityKind.TraceWord);
            foreach (Step step in tracingSteps)
            {
                if (step.Kind == ActivityKind.TraceLetter)
                {
                    CheckStep(curriculum, step, problems);
                }
                else
                {
                    CheckWordStep(curriculum, step, problems);
                }
            }
        }

        private static void CheckRuns(LetterShape shape, ContentProblems problems)
        {
            List<StrokeRun> runs = shape.Strokes
                .SelectMany(stroke => stroke.Runs)
                .ToList();
            if (shape.Form == LetterForm.Print)
            {
                if (runs.Any(run => run.Kind != StrokeRunKind.Body))
                {
                    problems.Error(shape.Source, "a print shape has only \"body\" runs: entry and exit runs are for the "
                        + "handwritten forms.");
                }

                return;
            }

            CheckEntry(shape, runs, problems);
            CheckExit(shape, runs, problems);
            if (shape.Case == LetterCase.Lower)
            {
                CheckLine(shape, problems);
            }
        }

        private static void CheckSlants(Curriculum curriculum, ContentProblems problems)
        {
            LetterShape first = curriculum.Shapes.All
                .FirstOrDefault(shape => shape.Form == LetterForm.Handwritten && shape.Case == LetterCase.Lower);
            IEnumerable<LetterShape> others = curriculum.Shapes.All
                .Where(shape => shape.Form == LetterForm.Handwritten && shape.Case == LetterCase.Lower)
                .Where(shape => first != null && shape.Slant != first.Slant);
            foreach (LetterShape shape in others)
            {
                problems.Error(shape.Source, "the handwritten small letters all lean alike, so a word leans as one: this one's "
                    + "slant is " + Number(shape.Slant) + ", " + LetterShapeCodes.FileNameOf(first.Id) + "'s is "
                    + Number(first.Slant) + ".");
            }
        }

        private static void CheckEntry(LetterShape shape, List<StrokeRun> runs, ContentProblems problems)
        {
            StrokeRun first = shape.Strokes[0].Runs[0];
            int entries = runs.Count(run => run.Kind == StrokeRunKind.Entry);
            if (entries > (first.Kind == StrokeRunKind.Entry ? 1 : 0))
            {
                problems.Error(shape.Source, "an entry run is the first run of the first stroke, and there is one at most.");
            }
            else if (shape.Case == LetterCase.Lower && first.Kind == StrokeRunKind.Entry && Math.Abs(first.Points[0].Y - WritingLines.Baseline) > OnTheLineWithin)
            {
                problems.Error(shape.Source, "the entry starts on the baseline (y " + Number(WritingLines.Baseline)
                    + "), not at y " + Number(first.Points[0].Y) + ".");
            }
        }

        private static void CheckExit(LetterShape shape, List<StrokeRun> runs, ContentProblems problems)
        {
            int exits = runs.Count(run => run.Kind == StrokeRunKind.Exit);
            if (exits == 0)
            {
                return;
            }

            if (exits > 1 || !LetterShapeCodes.HasExit(shape))
            {
                problems.Error(shape.Source, "an exit run is the last run of the letter's line, and there is one at most.");
                return;
            }

            IReadOnlyList<StrokeRun> lineRuns = shape.Strokes[TraceFigure.LineEndOf(shape)].Runs;
            IReadOnlyList<ShapePoint> exit = lineRuns[lineRuns.Count - 1].Points;
            float endY = exit[exit.Count - 1].Y;
            if (shape.Case == LetterCase.Lower && endY < WritingLines.Middle - OnTheLineWithin || endY > WritingLines.Baseline + OnTheLineWithin)
            {
                problems.Error(shape.Source, "the exit ends between the middle line and the baseline, not at y "
                    + Number(endY) + ".");
            }
        }

        private static void CheckLine(LetterShape shape, ContentProblems problems)
        {
            TraceFigure figure = TraceFigure.OfLetter(shape);
            int lineEnd = TraceFigure.LineEndOf(shape);
            for (int stroke = 1; stroke <= lineEnd; stroke++)
            {
                if (!figure.Continues(stroke))
                {
                    problems.Error(shape.Source, "stroke " + (stroke + 1) + " doesn't begin where stroke " + stroke + " ends: the "
                        + "letter's line, up to its exit, is written without lifting (its dots and bars come after).");
                }
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
                problems.Error(step.Source, "the letter " + letter.Id + " has no " + LetterShapeCodes.CodeOf(form)
                    + " shape for this step: it needs " + WantedFile(letter, letterCase, form) + ".");
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

        private static void CheckWordStep(Curriculum curriculum, Step step, ContentProblems problems)
        {
            if (step.Words.Count != 1 || !curriculum.Words.TryGet(step.Words[0], out Word word))
            {
                return;
            }

            if (word.Letters.Count > ActivityLimits.MostTracedLetters)
            {
                problems.Error(step.Source, "the word " + word.Id + " has " + word.Letters.Count + " letters; the writing "
                    + "board joins " + ActivityLimits.MostTracedLetters + " at most.");
            }

            for (int index = 0; index < word.Letters.Count; index++)
            {
                if (curriculum.Letters.TryGet(word.Letters[index], out Letter letter))
                {
                    CheckWordLetter(curriculum, step, word, letter, index + 1 < word.Letters.Count, problems);
                }
            }
        }

        private static void CheckWordLetter(Curriculum curriculum, Step step, Word word, Letter letter, bool joinsNext,
            ContentProblems problems)
        {
            if (!curriculum.TryGetShape(letter, LetterCase.Lower, LetterForm.Handwritten, out LetterShape shape))
            {
                problems.Error(step.Source, "the letter " + letter.Id + " of " + word.Id + " has no handwritten shape: it "
                    + "needs " + WantedFile(letter, LetterCase.Lower, LetterForm.Handwritten) + ".");
                return;
            }

            if (joinsNext && !LetterShapeCodes.HasExit(shape))
            {
                problems.Error(step.Source, "the letter " + letter.Id + " of " + word.Id + " has no exit run, so the next "
                    + "letter can't join it (" + LetterShapeCodes.FileNameOf(shape.Id) + ").");
            }

            TraceFigure figure = TraceFigure.OfLetter(shape);
            for (int stroke = 0; stroke < figure.Strokes.Count; stroke++)
            {
                if (!figure.IsMark(stroke))
                {
                    continue;
                }

                string key = (figure.Strokes[stroke].Path.IsDot ? "trace-dot-" : "trace-bar-") + letter.Strokes;
                foreach (Language language in step.Languages.Languages())
                {
                    VoiceLineRules.Line(curriculum, VoiceLineIds.For(language, VoiceLineKind.Line, key), step.Source, problems);
                }
            }
        }

        private static string WantedFile(Letter letter, LetterCase letterCase, LetterForm form)
        {
            return letter.Strokes.IsSet
                ? LetterShapeCodes.FileNameOf(LetterShapeCodes.IdOf(letter.Strokes, letterCase, form))
                : "a strokes id in letters.csv and its shape file";
        }

        private static string Number(float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
