using System;
using System.Collections.Generic;
using System.Globalization;
using Scolita.Core.Content;
using Scolita.Core.Content.Csv;

namespace Scolita.Core.Rewards
{
    /// <summary>
    /// Reads the rewards' three CSVs (<c>stickers.csv</c>, <c>constellations.csv</c>, <c>trails.csv</c>;
    /// <c>content/README.md</c>) into a <see cref="RewardContent"/>, through the same table reader as the
    /// curriculum (<see cref="ContentTables.ReadTable"/>): each file's columns, ids and spelling, and a
    /// constellation's stars (<c>x:y</c>, separated by spaces) and lines (<c>a-b</c>, star numbers from 1). A row
    /// with a broken cell is reported and left out; the rewards' own rules are <see cref="RewardContentRules"/>.
    /// </summary>
    public static class RewardTables
    {
        private const string Id = "id";
        private const string Retired = "retired";
        private const string Notes = "notes";
        private const string StarsColumn = "stars";
        private const string LinesColumn = "lines";

        private static readonly string[] StickerColumns = { Id, Retired, Notes };
        private static readonly string[] ConstellationColumns = { Id, "unit", StarsColumn, LinesColumn, Retired, Notes };
        private static readonly string[] TrailColumns = { Id, Retired, Notes };

        /// <summary>
        /// Reads the three files of <paramref name="textsByFileName"/> (each CSV's name and its text), reporting into
        /// <paramref name="problems"/> every missing file and column and every cell that can't be read. Called by the
        /// importer, the content validator test and <c>RewardTablesTests</c>.
        /// </summary>
        public static RewardContent Read(IReadOnlyDictionary<string, string> textsByFileName, ContentProblems problems)
        {
            List<Sticker> stickers = new List<Sticker>();
            List<Constellation> constellations = new List<Constellation>();
            List<Trail> trails = new List<Trail>();
            ContentTables.ReadTable(textsByFileName, ContentFileNames.Stickers, StickerColumns, problems,
                (table, row) => ReadSticker(table, row, problems, stickers));
            ContentTables.ReadTable(textsByFileName, ContentFileNames.Constellations, ConstellationColumns, problems,
                (table, row) => ReadConstellation(table, row, problems, constellations));
            ContentTables.ReadTable(textsByFileName, ContentFileNames.Trails, TrailColumns, problems,
                (table, row) => ReadTrail(table, row, problems, trails));
            return RewardContent.Build(stickers, constellations, trails, problems);
        }

        private static void ReadSticker(CsvTable table, CsvRow row, ContentProblems problems, List<Sticker> stickers)
        {
            RowReader cells = new RowReader(table, row, problems);
            ContentId id = cells.RequiredId(Id);
            bool retired = cells.YesNo(Retired);
            if (cells.IsGood)
            {
                stickers.Add(new Sticker(id, retired, cells.Source));
            }
        }

        private static void ReadTrail(CsvTable table, CsvRow row, ContentProblems problems, List<Trail> trails)
        {
            RowReader cells = new RowReader(table, row, problems);
            ContentId id = cells.RequiredId(Id);
            bool retired = cells.YesNo(Retired);
            if (cells.IsGood)
            {
                trails.Add(new Trail(id, retired, cells.Source));
            }
        }

        private static void ReadConstellation(CsvTable table, CsvRow row, ContentProblems problems,
            List<Constellation> constellations)
        {
            RowReader cells = new RowReader(table, row, problems);
            ContentId id = cells.RequiredId(Id);
            ContentId unit = cells.RequiredId("unit");
            IReadOnlyList<StarPoint> stars = ReadStars(cells, cells.Required(StarsColumn));
            IReadOnlyList<StarLine> lines = ReadLines(cells, cells.Optional(LinesColumn));
            bool retired = cells.YesNo(Retired);
            if (cells.IsGood)
            {
                constellations.Add(new Constellation(id, unit, stars, lines, retired, cells.Source));
            }
        }

        private static IReadOnlyList<StarPoint> ReadStars(RowReader cells, string cell)
        {
            List<StarPoint> stars = new List<StarPoint>();
            foreach (string part in Parts(cell))
            {
                string[] pair = part.Split(':');
                if (pair.Length == 2 && TryNumber(pair[0], out float x) && TryNumber(pair[1], out float y))
                {
                    stars.Add(new StarPoint(x, y));
                }
                else
                {
                    cells.Error("the star '" + part + "' is not x:y (two numbers, as 470:56).");
                }
            }

            return stars;
        }

        private static IReadOnlyList<StarLine> ReadLines(RowReader cells, string cell)
        {
            List<StarLine> lines = new List<StarLine>();
            foreach (string part in Parts(cell))
            {
                string[] pair = part.Split('-');
                if (pair.Length == 2
                    && int.TryParse(pair[0], NumberStyles.None, CultureInfo.InvariantCulture, out int from)
                    && int.TryParse(pair[1], NumberStyles.None, CultureInfo.InvariantCulture, out int to))
                {
                    lines.Add(new StarLine(from - 1, to - 1));
                }
                else
                {
                    cells.Error("the line '" + part + "' is not two star numbers joined by - (as 1-2).");
                }
            }

            return lines;
        }

        private static string[] Parts(string cell)
        {
            return cell.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static bool TryNumber(string text, out float number)
        {
            return float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out number);
        }
    }
}
