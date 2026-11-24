using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content.Csv;
using Scolita.Core.Languages;
using Scolita.Core.Tracing;

namespace Scolita.Core.Content
{
    /// <summary>
    /// Reads the seven content CSVs (<see cref="ContentFileNames"/>) and the letters' shape files
    /// (<see cref="LetterShapeReader"/>) into a <see cref="Curriculum"/>:
    /// checks each file's columns, each cell's code, id and spelling (no cedilla), and hands the rows
    /// to <see cref="CurriculumBuilder"/>. A row with a broken cell is reported and left out; the
    /// curriculum's own rules are <see cref="Rules.ContentRules"/>.
    /// </summary>
    public static class ContentTables
    {
        private const string Id = "id";
        private const string Notes = "notes";
        private const string Retired = "retired";
        private const string LanguageColumn = "language";
        private const string LettersColumn = "letters";
        private const string WordsColumn = "words";

        /// <summary>The <c>status</c> column of <c>voice-lines.csv</c>, filled by the importer; called by the importer.</summary>
        public const string StatusColumn = "status";

        /// <summary>The <c>clip</c> column of <c>voice-lines.csv</c>; called by the importer.</summary>
        public const string ClipColumn = "clip";

        /// <summary>The <c>id</c> column of every file but <c>steps.csv</c>; called by the importer.</summary>
        public const string IdColumn = Id;

        private static readonly IReadOnlyDictionary<string, string[]> ColumnsByFile = new Dictionary<string, string[]>
        {
            { ContentFileNames.Letters, new[] { Id, "lower", "upper", "group", "strokes", Retired, Notes } },
            {
                ContentFileNames.Words,
                new[]
                {
                    Id, LanguageColumn, "text", LettersColumn, "syllables", "picture", "listen_only", "twin", "false_friend",
                    Retired, Notes
                }
            },
            { ContentFileNames.Pictures, new[] { Id, "art", Retired, Notes } },
            { ContentFileNames.VoiceLines, new[] { Id, LanguageColumn, "kind", "text", ClipColumn, StatusColumn, Retired, Notes } },
            { ContentFileNames.Units, new[] { Id, "domain", Retired, Notes } },
            { ContentFileNames.Lessons, new[] { Id, "unit", LanguageColumn, "introduces", Retired, Notes } },
            { ContentFileNames.Steps, new[] { "lesson", "kind", LanguageColumn, LettersColumn, WordsColumn, "options", Notes } }
        };

        /// <summary>
        /// Reads the files in <paramref name="textsByFileName"/> (each CSV's name and its text, and each
        /// shape file of <c>content/strokes/</c> as <c>strokes/a-lower-print.json</c>) and builds the curriculum, reporting into <paramref name="problems"/> every missing file and
        /// column, every unknown column, and every cell that can't be read. Called by the importer, the
        /// content validator test and <c>ContentTablesTests</c>.
        /// </summary>
        public static Curriculum Read(IReadOnlyDictionary<string, string> textsByFileName, ContentProblems problems)
        {
            CurriculumBuilder builder = new CurriculumBuilder();
            ReadTable(textsByFileName, ContentFileNames.Letters, problems, (table, row) => ReadLetter(table, row, problems, builder));
            ReadTable(textsByFileName, ContentFileNames.Words, problems, (table, row) => ReadWord(table, row, problems, builder));
            ReadTable(textsByFileName, ContentFileNames.Pictures, problems, (table, row) => ReadPicture(table, row, problems, builder));
            ReadTable(textsByFileName, ContentFileNames.VoiceLines, problems, (table, row) => ReadVoiceLine(table, row, problems, builder));
            ReadTable(textsByFileName, ContentFileNames.Units, problems, (table, row) => ReadUnit(table, row, problems, builder));
            ReadTable(textsByFileName, ContentFileNames.Lessons, problems, (table, row) => ReadLesson(table, row, problems, builder));
            ReadTable(textsByFileName, ContentFileNames.Steps, problems, (table, row) => ReadStep(table, row, problems, builder));
            ReadShapes(textsByFileName, problems, builder);
            return builder.Build(problems);
        }

        private static void ReadShapes(IReadOnlyDictionary<string, string> textsByFileName, ContentProblems problems,
            CurriculumBuilder builder)
        {
            IEnumerable<KeyValuePair<string, string>> files = textsByFileName
                .Where(file => LetterShapeCodes.IsShapeFile(file.Key))
                .OrderBy(file => file.Key, StringComparer.Ordinal);
            foreach (KeyValuePair<string, string> file in files)
            {
                foreach (CedillaFinding finding in RomanianSpelling.FindCedillas(file.Value))
                {
                    problems.Error(new ContentSource(file.Key, 0), "it has U+" + ((int)finding.Found).ToString("X4") + " "
                        + finding.Found + " with a cedilla; Romanian writes U+" + ((int)finding.Correct).ToString("X4") + " "
                        + finding.Correct + " with the comma below.");
                }

                LetterShape shape = LetterShapeReader.Read(file.Key, file.Value, problems);
                if (shape != null)
                {
                    builder.Add(shape);
                }
            }
        }

        private static void ReadTable(IReadOnlyDictionary<string, string> textsByFileName, string fileName,
            ContentProblems problems, Action<CsvTable, CsvRow> readRow)
        {
            if (!textsByFileName.TryGetValue(fileName, out string text))
            {
                problems.Error(new ContentSource(fileName, 0), "the file is missing from content/.");
                return;
            }

            CsvTable table = CsvReader.Read(fileName, text, problems);
            CheckColumns(table, ColumnsByFile[fileName], problems);
            foreach (CsvRow row in table.Rows)
            {
                CheckSpelling(table, row, problems);
                readRow(table, row);
            }
        }

        private static void CheckColumns(CsvTable table, IReadOnlyList<string> expected, ContentProblems problems)
        {
            if (table.Columns.Count == 0)
            {
                return;
            }

            ContentSource header = new ContentSource(table.FileName, 1);
            foreach (string missing in expected.Where(column => !table.Columns.Contains(column)))
            {
                if (missing == StatusColumn)
                {
                    problems.Warning(header, "the header has no column " + missing
                        + ": run \u0218coli\u021Ba \u2192 Import content, which adds and fills it.");
                }
                else
                {
                    problems.Error(header, "the header has no column " + missing + ".");
                }
            }

            foreach (string unknown in table.Columns.Where(column => column.Length > 0 && !expected.Contains(column)))
            {
                problems.Error(header, "the header has an unknown column " + unknown + " (the columns are "
                    + string.Join(", ", expected) + ").");
            }
        }

        private static void CheckSpelling(CsvTable table, CsvRow row, ContentProblems problems)
        {
            foreach (string column in table.Columns)
            {
                foreach (CedillaFinding finding in RomanianSpelling.FindCedillas(table.Cell(row, column)))
                {
                    problems.Error(table.SourceOf(row), "the cell " + column + " has U+"
                        + ((int)finding.Found).ToString("X4") + " " + finding.Found + " with a cedilla; Romanian writes U+"
                        + ((int)finding.Correct).ToString("X4") + " " + finding.Correct + " with the comma below.");
                }
            }
        }

        private static void ReadLetter(CsvTable table, CsvRow row, ContentProblems problems, CurriculumBuilder builder)
        {
            RowReader cells = new RowReader(table, row, problems);
            ContentId id = cells.RequiredId(Id);
            string lower = cells.Required("lower");
            string upper = cells.Required("upper");
            string groupCode = cells.Required("group");
            ContentId strokes = cells.OptionalId("strokes");
            bool retired = cells.YesNo(Retired);
            bool groupKnown = ContentCodes.TryParseGroup(groupCode, out LetterGroup group);
            if (!groupKnown && groupCode.Length > 0)
            {
                cells.Error("the group '" + groupCode + "' is not one of " + ContentCodes.AllGroupCodes() + ".");
            }

            if (cells.IsGood && groupKnown)
            {
                builder.Add(new Letter(id, lower, upper, group, strokes, retired, cells.Source));
            }
        }

        private static void ReadWord(CsvTable table, CsvRow row, ContentProblems problems, CurriculumBuilder builder)
        {
            RowReader cells = new RowReader(table, row, problems);
            ContentId id = cells.RequiredId(Id);
            bool hasLanguage = cells.TryLanguage(LanguageColumn, out Language language);
            string text = cells.Required("text");
            IReadOnlyList<ContentId> letters = cells.IdList(LettersColumn);
            IReadOnlyList<string> syllables = cells.Syllables("syllables");
            ContentId picture = cells.OptionalId("picture");
            bool listenOnly = cells.YesNo("listen_only");
            ContentId twin = cells.OptionalId("twin");
            ContentId falseFriend = cells.OptionalId("false_friend");
            bool retired = cells.YesNo(Retired);
            if (cells.IsGood && hasLanguage)
            {
                builder.Add(new Word(id, language, text, letters, syllables, picture, listenOnly, twin, falseFriend,
                    retired, cells.Source));
            }
        }

        private static void ReadPicture(CsvTable table, CsvRow row, ContentProblems problems, CurriculumBuilder builder)
        {
            RowReader cells = new RowReader(table, row, problems);
            ContentId id = cells.RequiredId(Id);
            string art = cells.Optional("art");
            bool retired = cells.YesNo(Retired);
            if (cells.IsGood)
            {
                builder.Add(new Picture(id, art, retired, cells.Source));
            }
        }

        private static void ReadVoiceLine(CsvTable table, CsvRow row, ContentProblems problems,
            CurriculumBuilder builder)
        {
            RowReader cells = new RowReader(table, row, problems);
            ContentId id = cells.RequiredId(Id);
            bool hasLanguage = cells.TryLanguage(LanguageColumn, out Language language);
            string kindCode = cells.Required("kind");
            string text = cells.Required("text");
            string clip = cells.Optional(ClipColumn);
            string statusCode = cells.Optional(StatusColumn);
            bool retired = cells.YesNo(Retired);
            bool kindKnown = ContentCodes.TryParseVoiceLineKind(kindCode, out VoiceLineKind kind);
            if (!kindKnown && kindCode.Length > 0)
            {
                cells.Error("the kind '" + kindCode + "' is not one of " + ContentCodes.AllVoiceLineKindCodes() + ".");
            }

            if (!ContentCodes.TryParseStatus(statusCode, out VoiceLineStatus status))
            {
                cells.Error("the status '" + statusCode + "' is not one of " + ContentCodes.AllStatusCodes()
                    + " (the importer fills it: leave it to Import content).");
            }

            if (cells.IsGood && hasLanguage && kindKnown)
            {
                builder.Add(new VoiceLine(id, language, kind, text, clip, status, retired, cells.Source));
            }
        }

        private static void ReadUnit(CsvTable table, CsvRow row, ContentProblems problems, CurriculumBuilder builder)
        {
            RowReader cells = new RowReader(table, row, problems);
            ContentId id = cells.RequiredId(Id);
            ContentId domain = cells.RequiredId("domain");
            bool retired = cells.YesNo(Retired);
            if (cells.IsGood)
            {
                builder.Add(new UnitEntry(id, domain, retired, cells.Source));
            }
        }

        private static void ReadLesson(CsvTable table, CsvRow row, ContentProblems problems, CurriculumBuilder builder)
        {
            RowReader cells = new RowReader(table, row, problems);
            ContentId id = cells.RequiredId(Id);
            ContentId unit = cells.RequiredId("unit");
            LanguageMix? languages = cells.LanguageMixOrNone(LanguageColumn);
            ContentId introduces = cells.OptionalId("introduces");
            bool retired = cells.YesNo(Retired);
            if (!languages.HasValue)
            {
                cells.Error("the language is empty: write ro, en, or both for a lesson that mixes them.");
            }

            if (cells.IsGood && languages.HasValue)
            {
                builder.Add(new LessonEntry(id, unit, languages.Value, introduces, retired, cells.Source));
            }
        }

        private static void ReadStep(CsvTable table, CsvRow row, ContentProblems problems, CurriculumBuilder builder)
        {
            RowReader cells = new RowReader(table, row, problems);
            ContentId lesson = cells.RequiredId("lesson");
            string kindCode = cells.Required("kind");
            LanguageMix? languages = cells.LanguageMixOrNone(LanguageColumn);
            IReadOnlyList<ContentId> letters = cells.IdList(LettersColumn);
            IReadOnlyList<ContentId> words = cells.IdList(WordsColumn);
            IReadOnlyList<StepOption> options = cells.Options("options");
            bool kindKnown = ContentCodes.TryParseKind(kindCode, out ActivityKind kind);
            if (!kindKnown && kindCode.Length > 0)
            {
                cells.Error("the kind '" + kindCode + "' is not one of " + ContentCodes.AllKindCodes() + ".");
            }

            if (cells.IsGood && kindKnown)
            {
                builder.Add(new StepEntry(lesson, kind, languages, letters, words, options, cells.Source));
            }
        }
    }
}
