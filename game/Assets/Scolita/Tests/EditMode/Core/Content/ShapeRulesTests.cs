using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Content.Rules;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Content
{
    /// <summary>
    /// Tests <see cref="ShapeRules"/> (through <see cref="ContentRules.Check"/>): a tracing step's letter needs
    /// its shape (an error for print, a warning by hand), a shape belongs to a letter, a draft is a warning,
    /// and each later stroke has its checkpoint line.
    /// </summary>
    public sealed class ShapeRulesTests
    {
        /// <summary>Tests that <see cref="ShapeRules.Check"/> passes the valid set, whose print a has its shape.</summary>
        [Test]
        public void TestCheck_TheValidSetPasses()
        {
            Assert.That(ContentCsvs.Errors(ContentCsvs.Valid()).Select(error => error.ToString()), Is.Empty);
        }

        /// <summary>Tests that <see cref="ShapeRules.Check"/> fails a print tracing step whose letter has no shape for its case.</summary>
        [Test]
        public void TestCheck_APrintStepNeedsItsShape()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int row = ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "ro-a,TraceLetter,,a,,form=print;case=upper,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Steps, row, "no print shape", "strokes/a-upper-print.json");
            Assert.That(errors.Length, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that <see cref="ShapeRules.Check"/> fails a handwritten tracing step whose letter has no handwritten shape.
        /// </summary>
        [Test]
        public void TestCheck_AHandwrittenStepNeedsItsShape()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int row = ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "ro-a,TraceLetter,,a,,form=handwritten;case=lower,");

            IReadOnlyList<ContentProblem> problems = ContentCsvs.Check(csvs);

            Assert.That(problems.Where(problem => problem.IsError).Select(problem => problem.ToString()),
                Is.EqualTo(new[] { "steps.csv, row " + row + ": the letter a has no handwritten shape for this step: it needs "
                    + "strokes/a-lower-handwritten.json." }));
        }

        /// <summary>Tests that <see cref="ShapeRules.Check"/> fails a shape no letter has, and warns about a draft.</summary>
        [Test]
        public void TestCheck_AShapeOfNoLetterAndADraft()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            csvs["strokes/z-lower-print.json"] = ContentCsvs.Shape.Replace("\"checked\"", "\"draft\"");

            IReadOnlyList<ContentProblem> problems = ContentCsvs.Check(csvs);

            ContentCsvs.AssertError(problems.Where(problem => problem.IsError).ToArray(), "strokes/z-lower-print.json", 0,
                "no letter has the strokes id z");
            Assert.That(problems.Any(problem => !problem.IsError && problem.Source.File == "strokes/z-lower-print.json"
                && problem.Reason.Contains("draft")), Is.True);
        }

        /// <summary>Tests that <see cref="ShapeRules.Check"/> fails a tracing step whose shape's second stroke has no checkpoint line.</summary>
        [Test]
        public void TestCheck_EachLaterStrokeHasItsLine()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            ContentCsvs.ReplaceRow(csvs, ContentFileNames.VoiceLines, "ro-line-trace-next-2,",
                "ro-line-trace-next-2,ro,line,Foarte bine! Acum mergi la punctul cu num\u0103rul doi.,,,yes,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            Assert.That(errors.Any(error => error.Source.File == ContentFileNames.Steps && error.Reason.Contains("ro-line-trace-next-2")),
                Is.True, string.Join("\n", errors.Select(error => error.ToString())));
        }

        /// <summary>
        /// Tests that <see cref="ShapeRules.Check"/> takes the test set's handwritten a and m, and fails a print shape with
        /// an exit run, an entry off the baseline, an exit that isn't the end of the line, and a line that lifts.
        /// </summary>
        [Test]
        public void TestCheck_EntriesExitsAndLines()
        {
            Dictionary<string, string> valid = ContentCsvs.Valid();
            ContentCsvs.AddTraceWordNeeds(valid);
            Assert.That(ContentCsvs.Errors(valid).Select(error => error.ToString()), Is.Empty);

            Assert.That(ErrorsWith("strokes/a-lower-print.json", ContentCsvs.Shape.Replace("\"kind\": \"body\", \"points\": [[70, 100]", "\"kind\": \"exit\", \"points\": [[70, 100]")),
                Has.Some.Contains("a print shape has only \"body\" runs"));
            Assert.That(ErrorsWith("strokes/a-lower-handwritten.json", ContentCsvs.HandwrittenA.Replace("[[0, 200], [22, 172]", "[[0, 150], [22, 172]")),
                Has.Some.Contains("the entry starts on the baseline (y 200), not at y 150."));
            Assert.That(ErrorsWith("strokes/m-lower-handwritten.json", ContentCsvs.HandwrittenM.Replace("[112, 197], [122, 184]", "[112, 197], [122, 260]")),
                Has.Some.Contains("the exit ends between the middle line and the baseline, not at y 260."));
            Assert.That(ErrorsWith("strokes/a-lower-handwritten.json", ContentCsvs.HandwrittenA.Replace("[[60, 100], [60, 185]]", "[[64, 100], [60, 185]]")),
                Has.Some.Contains("stroke 3 doesn't begin where stroke 2 ends"));
        }

        /// <summary>
        /// Tests that <see cref="ShapeRules.Check"/> fails a <c>TraceWord</c> word whose letter before the last has no exit,
        /// and one longer than the board joins.
        /// </summary>
        [Test]
        public void TestCheck_AWordToTrace()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            ContentCsvs.AddTraceWordNeeds(csvs);
            ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "ro-m,TraceWord,,,ro-mama,,");
            csvs["strokes/m-lower-handwritten.json"] = ContentCsvs.HandwrittenM.Replace("\"kind\": \"exit\"", "\"kind\": \"body\"");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            Assert.That(errors.Select(error => error.Reason), Has.Some.Contains("the letter m of ro-mama has no exit run"));
            Assert.That(ActivityLimits.MostTracedLetters, Is.EqualTo(5));
        }

        private static IEnumerable<string> ErrorsWith(string file, string text)
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            ContentCsvs.AddTraceWordNeeds(csvs);
            csvs[file] = text;
            return ContentCsvs.Errors(csvs).Select(error => error.Reason).ToArray();
        }
    }
}
