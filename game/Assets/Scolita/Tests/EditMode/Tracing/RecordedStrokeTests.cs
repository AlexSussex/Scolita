using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Content.Json;
using Scolita.Core.Tracing;
using Scolita.Tests.EditMode.Fakes;
using UnityEngine;

namespace Scolita.Tests.EditMode.Tracing
{
    /// <summary>
    /// Tests <see cref="StrokeCheck"/> on planet 1's real print shapes (<c>content/strokes/</c>) with the lines
    /// in <c>Tests/EditMode/Tracing/Strokes/</c>: for every stroke of a m t p and A M T P, a good line, one
    /// sloppy but fine, one wobbly (good in round 1 only), one backwards, one off the path, one too short and
    /// one begun elsewhere, each with the verdict expected in each of the three rounds.
    /// </summary>
    public sealed class RecordedStrokeTests
    {
        /// <summary>
        /// Tests that <see cref="StrokeCheck.Check"/> gives every recorded line of <paramref name="shapeId"/> the
        /// verdict expected for it in each round.
        /// </summary>
        [TestCaseSource(nameof(Shapes))]
        public void TestCheck_RecordedStrokes(string shapeId)
        {
            Curriculum curriculum = RealContent.Read();
            Assert.That(curriculum.Shapes.TryGet(ContentId.Create(shapeId), out LetterShape shape), Is.True, "no shape " + shapeId);
            JsonValue recorded = ReadRecorded(shapeId);
            TracingTuning tuning = TestLearningRules.Tracing();
            List<string> wrong = new List<string>();
            Assert.That(recorded.TryGet("cases", out JsonValue cases), Is.True);

            foreach (JsonValue drawn in cases.Items)
            {
                drawn.TryGet("stroke", out JsonValue strokeValue);
                drawn.TryGet("drawn", out JsonValue name);
                drawn.TryGet("expect", out JsonValue expected);
                drawn.TryGet("points", out JsonValue pointList);
                StrokePath path = shape.Strokes[(int)strokeValue.Number].Path;
                TracePoints points = new TracePoints();
                foreach (JsonValue point in pointList.Items)
                {
                    points.Add((float)point.Items[0].Number, (float)point.Items[1].Number);
                }

                for (int round = 0; round < TracingTuning.Rounds; round++)
                {
                    StrokeResult result = StrokeCheck.Check(path, points, tuning.ForRound(round));
                    if (result.Verdict.ToString() != expected.Items[round].Text)
                    {
                        wrong.Add("stroke " + strokeValue.Number + ", " + name.Text + ", round " + (round + 1) + ": "
                            + result.Verdict + " (covered " + result.Covered.ToString("0.00") + "), expected "
                            + expected.Items[round].Text);
                    }
                }
            }

            Assert.That(wrong, Is.Empty);
        }

        private static IEnumerable<string> Shapes()
        {
            return new[]
            {
                "a-lower-print", "m-lower-print", "t-lower-print", "p-lower-print",
                "a-upper-print", "m-upper-print", "t-upper-print", "p-upper-print"
            };
        }

        private static JsonValue ReadRecorded(string shapeId)
        {
            string path = Path.Combine(Application.dataPath, "Scolita", "Tests", "EditMode", "Tracing", "Strokes", shapeId + ".json");
            ContentProblems problems = new ContentProblems();
            JsonValue recorded = JsonReader.Read(shapeId + ".json", File.ReadAllText(path), problems);
            Assert.That(problems.All, Is.Empty);
            return recorded;
        }
    }
}
