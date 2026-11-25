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
        /// Tests that <see cref="ShapeRules.Check"/> only warns about a handwritten tracing step whose letter has no
        /// shape yet (it is skipped until <c>handwriting</c> draws it).
        /// </summary>
        [Test]
        public void TestCheck_AHandwrittenStepWithoutItsShapeIsAWarning()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int row = ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "ro-a,TraceLetter,,a,,form=handwritten;case=lower,");

            IReadOnlyList<ContentProblem> problems = ContentCsvs.Check(csvs);

            Assert.That(problems.Where(problem => problem.IsError).Select(problem => problem.ToString()), Is.Empty);
            Assert.That(problems.Any(problem => problem.Source.Row == row && problem.Reason.Contains("is skipped until")), Is.True);
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
    }
}
