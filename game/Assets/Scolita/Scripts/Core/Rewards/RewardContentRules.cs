using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Languages;

namespace Scolita.Core.Rewards
{
    /// <summary>
    /// The content validator's rules on the rewards (<see cref="RewardContent"/>) against the
    /// <see cref="Curriculum"/>: at least one sticker to give; one constellation per planet that has lessons, on a
    /// planet that exists, with at least <see cref="FewestStars"/> stars inside its frame and lines between its own
    /// stars; and every line the rewards say, in each language of the lessons (<see cref="RewardLines"/>). The
    /// pictures of the stickers and trails are checked by the importer, which sees the art.
    /// </summary>
    public static class RewardContentRules
    {
        /// <summary>The fewest stars a constellation's figure has.</summary>
        public const int FewestStars = 3;

        /// <summary>
        /// Checks <paramref name="rewards"/> against <paramref name="curriculum"/> into <paramref name="problems"/>.
        /// Called by the importer, the content validator test and <c>RewardContentRulesTests</c>.
        /// </summary>
        public static void Check(RewardContent rewards, Curriculum curriculum, ContentProblems problems)
        {
            CheckStickers(rewards, problems);
            foreach (Constellation constellation in rewards.Constellations.All)
            {
                CheckFigure(constellation, problems);
                CheckUnit(constellation, curriculum, problems);
            }

            CheckEveryPlanetHasOne(rewards, curriculum, problems);
            CheckCommonLines(curriculum, problems);
        }

        private static void CheckStickers(RewardContent rewards, ContentProblems problems)
        {
            if (rewards.Stickers.All.All(sticker => sticker.Retired))
            {
                problems.Error(new ContentSource(ContentFileNames.Stickers, 0),
                    "there is no sticker to give: add one that isn't retired.");
            }
        }

        private static void CheckFigure(Constellation constellation, ContentProblems problems)
        {
            if (constellation.Stars.Count < FewestStars)
            {
                problems.Error(constellation.Source, "the constellation " + constellation.Id + " has "
                    + constellation.Stars.Count + " stars; a figure needs at least " + FewestStars + ".");
            }

            for (int index = 0; index < constellation.Stars.Count; index++)
            {
                StarPoint star = constellation.Stars[index];
                if (star.X < 0f || star.X > Constellation.FrameWidth || star.Y < 0f || star.Y > Constellation.FrameHeight)
                {
                    problems.Error(constellation.Source, "the star " + (index + 1) + " of " + constellation.Id
                        + " is outside its " + Constellation.FrameWidth + " × " + Constellation.FrameHeight + " frame.");
                }
            }

            foreach (StarLine line in constellation.Lines)
            {
                if (!IsStar(constellation, line.From) || !IsStar(constellation, line.To) || line.From == line.To)
                {
                    problems.Error(constellation.Source, "the line " + (line.From + 1) + "-" + (line.To + 1) + " of "
                        + constellation.Id + " must join two different stars from 1 to " + constellation.Stars.Count + ".");
                }
            }
        }

        private static bool IsStar(Constellation constellation, int index)
        {
            return index >= 0 && index < constellation.Stars.Count;
        }

        private static void CheckUnit(Constellation constellation, Curriculum curriculum, ContentProblems problems)
        {
            if (!curriculum.TryGetUnit(constellation.UnitId, out Unit unit))
            {
                problems.Error(constellation.Source, "the constellation " + constellation.Id + " names the unit '"
                    + constellation.UnitId + "', which is not in units.csv.");
                return;
            }

            if (constellation.Retired)
            {
                return;
            }

            foreach (Language language in LanguagesOf(unit.Lessons))
            {
                RequireLine(curriculum, RewardLines.Stars(constellation, language), constellation.Source, problems);
                RequireLine(curriculum, RewardLines.Name(constellation, language), constellation.Source, problems);
            }
        }

        private static void CheckEveryPlanetHasOne(RewardContent rewards, Curriculum curriculum, ContentProblems problems)
        {
            IEnumerable<Unit> planets = curriculum.Domains
                .SelectMany(domain => domain.Units)
                .Where(unit => !unit.Retired)
                .Where(unit => unit.Lessons.Any(lesson => !lesson.Retired));
            foreach (Unit unit in planets)
            {
                Constellation[] own = rewards.Constellations.All
                    .Where(constellation => !constellation.Retired)
                    .Where(constellation => constellation.UnitId == unit.Id)
                    .ToArray();
                if (own.Length == 0)
                {
                    problems.Error(new ContentSource(ContentFileNames.Constellations, 0), "the unit " + unit.Id
                        + " has no constellation: add one (each planet's lessons light their own).");
                }
                else if (own.Length > 1)
                {
                    problems.Error(own[1].Source, "the unit " + unit.Id + " already has the constellation " + own[0].Id
                        + ": a planet has one (retire the other).");
                }
            }
        }

        private static void CheckCommonLines(Curriculum curriculum, ContentProblems problems)
        {
            ContentSource source = new ContentSource(ContentFileNames.VoiceLines, 0);
            foreach (Language language in LanguagesOf(curriculum.AllLessons))
            {
                foreach (string key in RewardLines.CommonKeys)
                {
                    RequireLine(curriculum, RewardLines.Common(key, language), source, problems);
                }
            }
        }

        private static IEnumerable<Language> LanguagesOf(IEnumerable<Lesson> lessons)
        {
            return lessons
                .Where(lesson => !lesson.Retired)
                .SelectMany(lesson => lesson.Languages.Languages())
                .Distinct()
                .ToArray();
        }

        private static void RequireLine(Curriculum curriculum, VoiceLineId line, ContentSource source,
            ContentProblems problems)
        {
            if (!curriculum.VoiceLines.TryGet(line.Id, out VoiceLine found) || found.Retired)
            {
                problems.Error(source, "the rewards need the voice line " + line + " in voice-lines.csv.");
            }
        }
    }
}
