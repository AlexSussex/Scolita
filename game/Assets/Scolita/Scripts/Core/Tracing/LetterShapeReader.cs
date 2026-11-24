using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Content.Json;

namespace Scolita.Core.Tracing
{
    /// <summary>
    /// Reads a shape file of <c>content/strokes/</c> (<c>a-lower-print.json</c>; the format is in
    /// <c>content/README.md</c>) into a <see cref="LetterShape"/>, reporting everything it refuses with
    /// the file and the line: a name not of the pattern, JSON that doesn't parse, a member it doesn't
    /// know or that is missing, a stroke with no point, runs that don't meet, a point far off the
    /// writing lines.
    /// </summary>
    public static class LetterShapeReader
    {
        private const string StatusKey = "status";
        private const string SourceKey = "source";
        private const string SlantKey = "slant";
        private const string StrokesKey = "strokes";
        private const string RunsKey = "runs";
        private const string KindKey = "kind";
        private const string PointsKey = "points";
        private const float SteepestSlant = 0.5f;
        private const float FarthestOff = 100f;
        private const float RunsMeetWithin = 0.5f;

        private static readonly string[] ShapeKeys = { StatusKey, SourceKey, SlantKey, StrokesKey };
        private static readonly string[] StrokeKeys = { RunsKey };
        private static readonly string[] RunKeys = { KindKey, PointsKey };

        /// <summary>
        /// Reads <paramref name="text"/>, the shape file <paramref name="fileName"/> (as the content's texts
        /// name it, <c>strokes/a-lower-print.json</c>); null when it can't be read, every reason reported
        /// into <paramref name="problems"/>. Called by <see cref="ContentTables.Read"/> and the tests.
        /// </summary>
        public static LetterShape Read(string fileName, string text, ContentProblems problems)
        {
            int errorsBefore = ErrorCount(problems);
            if (!LetterShapeCodes.TryParseFileName(fileName, out ContentId strokesId, out LetterCase letterCase,
                    out LetterForm form))
            {
                problems.Error(new ContentSource(fileName, 0), "a shape file is named <strokes id>-<lower or upper>-"
                    + "<print or handwritten>.json (a-lower-print.json), where the strokes id is a letter's strokes "
                    + "cell in letters.csv.");
                return null;
            }

            JsonValue root = JsonReader.Read(fileName, text, problems);
            if (root == null)
            {
                return null;
            }

            if (!IsObject(root, "the file", fileName, problems))
            {
                return null;
            }

            CheckKeys(root, ShapeKeys, fileName, problems);
            ShapeStatus status = ReadStatus(root, fileName, problems);
            float slant = ReadSlant(root, fileName, problems);
            CheckSource(root, fileName, problems);
            List<Stroke> strokes = ReadStrokes(root, slant, fileName, problems);
            if (ErrorCount(problems) > errorsBefore || strokes.Count == 0)
            {
                return null;
            }

            return new LetterShape(strokesId, letterCase, form, status, slant, strokes, new ContentSource(fileName, 0));
        }

        private static ShapeStatus ReadStatus(JsonValue root, string fileName, ContentProblems problems)
        {
            if (!root.TryGet(StatusKey, out JsonValue status))
            {
                problems.Error(new ContentSource(fileName, root.Line), "it has no \"status\" (\"draft\" or \"checked\").");
                return ShapeStatus.Draft;
            }

            if (status.Kind != JsonKind.String || !LetterShapeCodes.TryParseStatus(status.Text, out ShapeStatus read))
            {
                problems.Error(new ContentSource(fileName, status.Line), "its \"status\" is \"draft\" or \"checked\".");
                return ShapeStatus.Draft;
            }

            return read;
        }

        private static float ReadSlant(JsonValue root, string fileName, ContentProblems problems)
        {
            if (!root.TryGet(SlantKey, out JsonValue slant))
            {
                return 0f;
            }

            if (slant.Kind != JsonKind.Number || Math.Abs(slant.Number) > SteepestSlant)
            {
                problems.Error(new ContentSource(fileName, slant.Line), "its \"slant\" is a number from -" + SteepestSlant
                    + " to " + SteepestSlant + " (0 for print, 0.2 for handwritten letters).");
                return 0f;
            }

            return (float)slant.Number;
        }

        private static void CheckSource(JsonValue root, string fileName, ContentProblems problems)
        {
            if (root.TryGet(SourceKey, out JsonValue source) && source.Kind != JsonKind.String)
            {
                problems.Error(new ContentSource(fileName, source.Line), "its \"source\" is a text, for people.");
            }
        }

        private static List<Stroke> ReadStrokes(JsonValue root, float slant, string fileName, ContentProblems problems)
        {
            List<Stroke> strokes = new List<Stroke>();
            if (!root.TryGet(StrokesKey, out JsonValue list) || list.Kind != JsonKind.Array || list.Items.Count == 0)
            {
                problems.Error(new ContentSource(fileName, list?.Line ?? root.Line),
                    "its \"strokes\" is a list of at least one stroke, in the order she draws them.");
                return strokes;
            }

            foreach (JsonValue strokeValue in list.Items)
            {
                IReadOnlyList<StrokeRun> runs = ReadRuns(strokeValue, fileName, problems);
                if (runs != null)
                {
                    strokes.Add(new Stroke(runs, slant));
                }
            }

            return strokes;
        }

        private static IReadOnlyList<StrokeRun> ReadRuns(JsonValue strokeValue, string fileName, ContentProblems problems)
        {
            if (!IsObject(strokeValue, "a stroke", fileName, problems))
            {
                return null;
            }

            CheckKeys(strokeValue, StrokeKeys, fileName, problems);
            if (!strokeValue.TryGet(RunsKey, out JsonValue list) || list.Kind != JsonKind.Array || list.Items.Count == 0)
            {
                problems.Error(new ContentSource(fileName, strokeValue.Line),
                    "a stroke's \"runs\" is a list of at least one run.");
                return null;
            }

            List<StrokeRun> runs = new List<StrokeRun>();
            foreach (JsonValue runValue in list.Items)
            {
                StrokeRun run = ReadRun(runValue, fileName, problems);
                if (run == null)
                {
                    return null;
                }

                if (runs.Count > 0 && !Meet(runs[runs.Count - 1], run))
                {
                    problems.Error(new ContentSource(fileName, runValue.Line),
                        "a run starts where the run before it ends (a stroke is drawn without lifting).");
                    return null;
                }

                runs.Add(run);
            }

            if (runs.Count > 1 && runs.Any(run => run.Points.Count == 1))
            {
                problems.Error(new ContentSource(fileName, strokeValue.Line),
                    "a run of one point is a dot, a stroke of its own (the dot of i).");
                return null;
            }

            return runs;
        }

        private static StrokeRun ReadRun(JsonValue runValue, string fileName, ContentProblems problems)
        {
            if (!IsObject(runValue, "a run", fileName, problems))
            {
                return null;
            }

            CheckKeys(runValue, RunKeys, fileName, problems);
            if (!runValue.TryGet(KindKey, out JsonValue kindValue) || kindValue.Kind != JsonKind.String
                || !LetterShapeCodes.TryParseRunKind(kindValue.Text, out StrokeRunKind kind))
            {
                problems.Error(new ContentSource(fileName, kindValue?.Line ?? runValue.Line),
                    "a run's \"kind\" is \"body\", \"entry\" or \"exit\".");
                return null;
            }

            if (!runValue.TryGet(PointsKey, out JsonValue pointsValue) || pointsValue.Kind != JsonKind.Array
                || pointsValue.Items.Count == 0)
            {
                problems.Error(new ContentSource(fileName, pointsValue?.Line ?? runValue.Line),
                    "a run's \"points\" is a list of at least one [x, y].");
                return null;
            }

            List<ShapePoint> points = new List<ShapePoint>();
            foreach (JsonValue pointValue in pointsValue.Items)
            {
                if (!TryReadPoint(pointValue, out ShapePoint point))
                {
                    problems.Error(new ContentSource(fileName, pointValue.Line), "a point is [x, y], two numbers.");
                    return null;
                }

                if (IsFarOff(point))
                {
                    problems.Error(new ContentSource(fileName, pointValue.Line), "the point [" + point.X + ", " + point.Y
                        + "] is far off the writing lines (the top line is y " + WritingLines.Top + ", the line below is y "
                        + WritingLines.Below + ").");
                    return null;
                }

                points.Add(point);
            }

            return new StrokeRun(kind, points);
        }

        private static bool TryReadPoint(JsonValue value, out ShapePoint point)
        {
            point = default;
            if (value.Kind != JsonKind.Array || value.Items.Count != 2 || value.Items[0].Kind != JsonKind.Number
                || value.Items[1].Kind != JsonKind.Number)
            {
                return false;
            }

            point = new ShapePoint((float)value.Items[0].Number, (float)value.Items[1].Number);
            return true;
        }

        private static bool IsFarOff(ShapePoint point)
        {
            return point.X < -FarthestOff || point.X > WritingLines.Below + FarthestOff
                || point.Y < WritingLines.Top - FarthestOff || point.Y > WritingLines.Below + FarthestOff;
        }

        private static bool Meet(StrokeRun before, StrokeRun after)
        {
            ShapePoint end = before.Points[before.Points.Count - 1];
            ShapePoint start = after.Points[0];
            return Math.Abs(end.X - start.X) <= RunsMeetWithin && Math.Abs(end.Y - start.Y) <= RunsMeetWithin;
        }

        private static bool IsObject(JsonValue value, string what, string fileName, ContentProblems problems)
        {
            if (value.Kind == JsonKind.Object)
            {
                return true;
            }

            problems.Error(new ContentSource(fileName, value.Line), what + " is an object, in { }.");
            return false;
        }

        private static void CheckKeys(JsonValue value, IReadOnlyList<string> known, string fileName, ContentProblems problems)
        {
            foreach (KeyValuePair<string, JsonValue> member in value.Members.Where(member => !known.Contains(member.Key)))
            {
                problems.Error(new ContentSource(fileName, member.Value.Line), "\"" + member.Key + "\" is not a member it knows ("
                    + string.Join(", ", known) + ").");
            }
        }

        private static int ErrorCount(ContentProblems problems)
        {
            return problems.All.Count(problem => problem.IsError);
        }
    }
}
