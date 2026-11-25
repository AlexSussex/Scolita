using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Content.Json;
using Scolita.Core.Tracing;
using Scolita.Tests.EditMode.Fakes;
using UnityEngine;

namespace Scolita.Tests.EditMode.Tracing
{
    /// <summary>
    /// Tests <see cref="LineCheck"/> on the real handwritten shapes of planets 1–3 (a m t p, s i n o d e) and six joined
    /// words (mama, tata, pom, dinte, dot, stea) with the lines in <c>Tests/EditMode/Tracing/Strokes/handwritten/</c>: a good line
    /// through the letter or the word, one sloppy but fine, one backwards, one without its entry, one off during its
    /// first stroke (and for the a, one off after it: the entry kept), and for the words a line up to a join and the next
    /// one begun near it after a lift, each with the verdict and the strokes done expected in each round. An o drawn
    /// backwards from its high exit, and an e joined in a word drawn backwards, end near their round's start, so they are
    /// taken for a line without its entry (all rounds for the e, which starts at a join; the first round for the o).
    /// </summary>
    public sealed class HandwrittenStrokeTests
    {
        /// <summary>
        /// Tests that <see cref="LineCheck.Check"/> gives every recorded line of <paramref name="figureName"/> its
        /// expected verdict and strokes done in each round, a stroke begun at a join with the join radius.
        /// </summary>
        [TestCaseSource(nameof(Figures))]
        public void TestCheck_RecordedHandwrittenLines(string figureName)
        {
            TraceFigure figure = FigureOf(RealContent.Read(), figureName);
            JsonValue recorded = ReadRecorded(figureName);
            TracingTuning tuning = TestLearningRules.Tracing();
            List<string> wrong = new List<string>();
            Assert.That(recorded.TryGet("cases", out JsonValue cases), Is.True);

            foreach (JsonValue drawn in cases.Items)
            {
                drawn.TryGet("from", out JsonValue fromValue);
                drawn.TryGet("drawn", out JsonValue name);
                drawn.TryGet("expect", out JsonValue expected);
                drawn.TryGet("done", out JsonValue done);
                drawn.TryGet("points", out JsonValue pointList);
                int from = (int)fromValue.Number;
                TracePoints points = new TracePoints();
                foreach (JsonValue point in pointList.Items)
                {
                    points.Add((float)point.Items[0].Number, (float)point.Items[1].Number);
                }

                for (int round = 0; round < TracingTuning.Rounds; round++)
                {
                    StrokeTolerance tolerance = tuning.ForRound(round);
                    if (figure.BeginsAtJoin(from))
                    {
                        tolerance = tolerance.WithStartRadiusAtLeast(tuning.JoinRadius);
                    }

                    LineResult result = LineCheck.Check(figure, from, points, tolerance);
                    if (result.Verdict.ToString() != expected.Items[round].Text || result.StrokesDone != (int)done.Items[round].Number)
                    {
                        wrong.Add("from " + from + ", " + name.Text + ", round " + (round + 1) + ": " + result.Verdict + " ("
                            + result.StrokesDone + " done), expected " + expected.Items[round].Text + " ("
                            + done.Items[round].Number + " done)");
                    }
                }
            }

            Assert.That(wrong, Is.Empty);
        }

        private static IEnumerable<string> Figures()
        {
            return new[]
            {
                "a-lower-handwritten", "m-lower-handwritten", "t-lower-handwritten", "p-lower-handwritten", "s-lower-handwritten",
                "i-lower-handwritten", "n-lower-handwritten", "o-lower-handwritten", "d-lower-handwritten", "e-lower-handwritten",
                "ro-mama", "ro-tata", "ro-pom", "ro-dinte", "en-dot", "ro-stea"
            };
        }

        private static TraceFigure FigureOf(Curriculum curriculum, string figureName)
        {
            if (curriculum.Shapes.TryGet(ContentId.Create(figureName), out LetterShape shape))
            {
                return TraceFigure.OfLetter(shape);
            }

            Word word = curriculum.Words.Get(ContentId.Create(figureName));
            LetterShape[] shapes = word.Letters
                .Select(curriculum.Letters.Get)
                .Select(letter => HandwrittenOf(curriculum, letter))
                .ToArray();
            return JoinedWord.Of(shapes);
        }

        private static LetterShape HandwrittenOf(Curriculum curriculum, Letter letter)
        {
            Assert.That(curriculum.TryGetShape(letter, LetterCase.Lower, LetterForm.Handwritten, out LetterShape shape), Is.True);
            return shape;
        }

        private static JsonValue ReadRecorded(string figureName)
        {
            string path = Path.Combine(Application.dataPath, "Scolita", "Tests", "EditMode", "Tracing", "Strokes", "handwritten",
                figureName + ".json");
            ContentProblems problems = new ContentProblems();
            JsonValue recorded = JsonReader.Read(figureName + ".json", File.ReadAllText(path), problems);
            Assert.That(problems.All, Is.Empty);
            return recorded;
        }
    }
}
