using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Rewards;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Rewards
{
    /// <summary>Tests <see cref="RewardContentRules"/> on the test set of <see cref="ContentCsvs"/>.</summary>
    public sealed class RewardContentRulesTests
    {
        /// <summary>Tests that <see cref="RewardContentRules.Check"/> finds nothing wrong in the valid set.</summary>
        [Test]
        public void TestCheck_TheValidSetHasNoProblem()
        {
            IReadOnlyList<ContentProblem> problems = Check(ContentCsvs.Valid());

            Assert.That(problems.Select(problem => problem.ToString()), Is.Empty);
        }

        /// <summary>Tests that <see cref="RewardContentRules.Check"/> wants a constellation for every planet, on a planet that exists.</summary>
        [Test]
        public void TestCheck_EveryPlanetHasOneConstellation()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int row = ContentCsvs.ReplaceRow(csvs, ContentFileNames.Constellations, "unicorn,",
                "unicorn,planet-9,470:56 418:150 356:146,1-2,,");

            ContentProblem[] errors = Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Constellations, row, "planet-9", "units.csv");
            ContentCsvs.AssertError(errors, ContentFileNames.Constellations, 0, "planet-1", "no constellation");
        }

        /// <summary>Tests that <see cref="RewardContentRules.Check"/> wants at least three stars inside the frame, and lines between its own stars.</summary>
        [Test]
        public void TestCheck_AFigureInsideItsFrame()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int row = ContentCsvs.ReplaceRow(csvs, ContentFileNames.Constellations, "unicorn,",
                "unicorn,planet-1,470:56 700:150,1-3 2-2,,");

            ContentProblem[] errors = Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Constellations, row, "2 stars", "at least 3");
            ContentCsvs.AssertError(errors, ContentFileNames.Constellations, row, "star 2", "outside");
            ContentCsvs.AssertError(errors, ContentFileNames.Constellations, row, "1-3", "two different stars");
            ContentCsvs.AssertError(errors, ContentFileNames.Constellations, row, "2-2", "two different stars");
        }

        /// <summary>Tests that <see cref="RewardContentRules.Check"/> wants every line the rewards say, in each language of the lessons.</summary>
        [Test]
        public void TestCheck_TheRewardsLinesInBothLanguages()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            ContentCsvs.ReplaceRow(csvs, ContentFileNames.VoiceLines, "en-line-constellation-unicorn,",
                "en-line-constellation-unicorn,en,line,This is the Unicorn constellation.,,,yes,");
            ContentCsvs.ReplaceRow(csvs, ContentFileNames.VoiceLines, "ro-line-trail-picker,", "ro-line-unused,ro,line,Nimic.,,,");

            ContentProblem[] errors = Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Constellations, 2, "en-line-constellation-unicorn");
            ContentCsvs.AssertError(errors, ContentFileNames.VoiceLines, 0, "ro-line-trail-picker");
        }

        /// <summary>Tests that <see cref="RewardContentRules.Check"/> wants a sticker that isn't retired.</summary>
        [Test]
        public void TestCheck_AStickerToGive()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            ContentCsvs.ReplaceRow(csvs, ContentFileNames.Stickers, "guitar,", "guitar,yes,");
            ContentCsvs.ReplaceRow(csvs, ContentFileNames.Stickers, "whale-rainbow,", "whale-rainbow,yes,");

            ContentProblem[] errors = Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Stickers, 0, "no sticker");
        }

        private static IReadOnlyList<ContentProblem> Check(Dictionary<string, string> csvs)
        {
            ContentProblems problems = new ContentProblems();
            Curriculum curriculum = ContentTables.Read(csvs, problems);
            RewardContent rewards = RewardTables.Read(csvs, problems);
            RewardContentRules.Check(rewards, curriculum, problems);
            return problems.All;
        }

        private static ContentProblem[] Errors(Dictionary<string, string> csvs)
        {
            return Check(csvs)
                .Where(problem => problem.IsError)
                .ToArray();
        }
    }
}
