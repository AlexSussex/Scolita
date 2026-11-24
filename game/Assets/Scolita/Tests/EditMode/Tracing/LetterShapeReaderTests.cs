using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Tracing;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Tracing
{
    /// <summary>
    /// Tests <see cref="LetterShapeReader"/> (and <see cref="ContentTables.Read"/> reading the shape files):
    /// a valid shape, and each thing it refuses with its file and line.
    /// </summary>
    public sealed class LetterShapeReaderTests
    {
        /// <summary>Tests that <see cref="LetterShapeReader.Read"/> reads the valid set's print a: its id, its two strokes, its status.</summary>
        [Test]
        public void TestRead_TheValidShape()
        {
            ContentProblems problems = new ContentProblems();

            LetterShape shape = LetterShapeReader.Read(ContentCsvs.ShapeFileName, ContentCsvs.Shape, problems);

            Assert.That(problems.All, Is.Empty);
            Assert.That(shape.Id.ToString(), Is.EqualTo("a-lower-print"));
            Assert.That(shape.StrokesId.ToString(), Is.EqualTo("a"));
            Assert.That(shape.Case, Is.EqualTo(LetterCase.Lower));
            Assert.That(shape.Form, Is.EqualTo(LetterForm.Print));
            Assert.That(shape.Status, Is.EqualTo(ShapeStatus.Checked));
            Assert.That(shape.Strokes.Count, Is.EqualTo(2));
            Assert.That(shape.Strokes[1].Path.Start.X, Is.EqualTo(70f).Within(0.01f));
            Assert.That(shape.MinX, Is.LessThan(10f));
            Assert.That(shape.MaxX, Is.EqualTo(70f).Within(0.5f));
        }

        /// <summary>Tests that <see cref="ContentTables.Read"/> reads the shape files among the content's texts into the curriculum's shapes.</summary>
        [Test]
        public void TestRead_TheCurriculumHasTheShape()
        {
            Curriculum curriculum = ContentCsvs.Read(ContentCsvs.Valid());
            Letter a = curriculum.Letters.Get(ContentId.Create("a"));

            Assert.That(curriculum.Shapes.All.Count, Is.EqualTo(1));
            Assert.That(curriculum.TryGetShape(a, LetterCase.Lower, LetterForm.Print, out LetterShape shape), Is.True);
            Assert.That(shape.Strokes.Count, Is.EqualTo(2));
            Assert.That(curriculum.TryGetShape(a, LetterCase.Upper, LetterForm.Print, out _), Is.False);
            Assert.That(curriculum.TryGetShape(curriculum.Letters.Get(ContentId.Create("t")), LetterCase.Lower, LetterForm.Print,
                out _), Is.False);
        }

        /// <summary>Tests that <see cref="LetterShapeReader.Read"/> refuses a file whose name isn't strokes id, case and form.</summary>
        [TestCase("strokes/a-print.json")]
        [TestCase("strokes/a-small-print.json")]
        [TestCase("strokes/a-lower-typed.json")]
        [TestCase("strokes/A-lower-print.json")]
        public void TestRead_RefusesABadName(string fileName)
        {
            ContentProblems problems = new ContentProblems();

            Assert.That(LetterShapeReader.Read(fileName, ContentCsvs.Shape, problems), Is.Null);

            Assert.That(problems.All.Single().Reason, Does.Contain("<strokes id>-<lower or upper>-<print or handwritten>.json"));
        }

        /// <summary>Tests that <see cref="LetterShapeReader.Read"/> refuses what the format doesn't allow, naming the line.</summary>
        [TestCase("{ \"status\": \"draft\", \"strokes\": [], \"colour\": 1 }", 1, "\"colour\" is not a member")]
        [TestCase("{\n\"strokes\": [ { \"runs\": [ { \"kind\": \"body\", \"points\": [[0, 0], [0, 10]] } ] } ] }", 1, "no \"status\"")]
        [TestCase("{ \"status\": \"done\",\n\"strokes\": [ { \"runs\": [ { \"kind\": \"body\", \"points\": [[0, 0], [0, 10]] } ] } ] }", 1, "\"status\" is")]
        [TestCase("{ \"status\": \"draft\",\n\"strokes\": [] }", 2, "at least one stroke")]
        [TestCase("{ \"status\": \"draft\", \"strokes\": [ { \"runs\": [\n{ \"kind\": \"body\", \"points\": [[0, 0], [0, 10]] },\n{ \"kind\": \"body\", \"points\": [[5, 10], [9, 20]] } ] } ] }", 3, "starts where the run before it ends")]
        [TestCase("{ \"status\": \"draft\", \"strokes\": [ { \"runs\": [ { \"kind\": \"body\", \"points\": [\n[0, 900]] } ] } ] }", 2, "far off the writing lines")]
        [TestCase("{ \"status\": \"draft\", \"strokes\": [ { \"runs\": [ { \"kind\": \"stem\", \"points\": [[0, 0]] } ] } ] }", 1, "\"kind\" is")]
        [TestCase("{ \"status\": \"draft\", \"strokes\": [ { \"runs\": [ { \"kind\": \"body\", \"points\": [[0, 0, 1]] } ] } ] }", 1, "[x, y]")]
        [TestCase("{ \"status\": \"draft\", \"slant\": 2, \"strokes\": [ { \"runs\": [ { \"kind\": \"body\", \"points\": [[0, 0]] } ] } ] }", 1, "\"slant\"")]
        [TestCase("{ \"status\": \"draft\", \"strokes\": [ { \"runs\": [ { \"kind\": \"body\", \"points\": [[0, 0], [0, 9]] },\n{ \"kind\": \"body\", \"points\": [[0, 9]] } ] } ] }", 1, "a run of one point is a dot")]
        [TestCase("[1]", 1, "the file is an object")]
        [TestCase("{ \"status\": ", 1, "this is not JSON")]
        public void TestRead_RefusesWhatTheFormatDoesNotAllow(string text, int line, string reason)
        {
            ContentProblems problems = new ContentProblems();

            LetterShape shape = LetterShapeReader.Read("strokes/a-lower-print.json", text, problems);

            Assert.That(shape, Is.Null);
            ContentCsvs.AssertError(problems.All, "strokes/a-lower-print.json", line, reason);
        }

        /// <summary>Tests that <see cref="ContentTables.Read"/> refuses a cedilla in a shape file, as in every content file.</summary>
        [Test]
        public void TestRead_RefusesACedilla()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            csvs[ContentCsvs.ShapeFileName] = csvs[ContentCsvs.ShapeFileName].Replace("\"status\": \"checked\",",
                "\"status\": \"checked\", \"source\": \"ş\",");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentCsvs.ShapeFileName, 0, "U+015F", "comma below");
        }
    }
}
