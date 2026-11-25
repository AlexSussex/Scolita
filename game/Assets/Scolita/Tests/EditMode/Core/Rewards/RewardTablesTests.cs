using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Rewards;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Rewards
{
    /// <summary>Tests <see cref="RewardTables"/> on the test set of <see cref="ContentCsvs"/>.</summary>
    public sealed class RewardTablesTests
    {
        /// <summary>Tests that <see cref="RewardTables.Read"/> reads the stickers, the constellations, the trails and the avatars.</summary>
        [Test]
        public void TestRead_TheFourFiles()
        {
            ContentProblems problems = new ContentProblems();

            RewardContent rewards = RewardTables.Read(ContentCsvs.Valid(), problems);

            Assert.That(problems.All.Select(problem => problem.ToString()), Is.Empty);
            Assert.That(rewards.Stickers.All.Select(sticker => sticker.Id.ToString()), Is.EqualTo(new[] { "guitar", "whale-rainbow" }));
            Assert.That(rewards.Trails.All.Select(trail => trail.Id.ToString()), Is.EqualTo(new[] { "rainbow", "stars" }));
            Assert.That(rewards.Avatars.All.Select(avatar => avatar.Id.ToString()), Is.EqualTo(new[] { "unicorn", "dog" }));
            Assert.That(rewards.TryGetConstellation(ContentId.Create("planet-1"), out Constellation unicorn), Is.True);
            Assert.That(unicorn.Stars.Count, Is.EqualTo(4));
            Assert.That(unicorn.Stars[0].X, Is.EqualTo(470f));
            Assert.That(unicorn.Stars[0].Y, Is.EqualTo(56f));
            Assert.That(unicorn.Lines.Select(line => line.From), Is.EqualTo(new[] { 0, 1, 2 }));
            Assert.That(unicorn.Lines.Select(line => line.To), Is.EqualTo(new[] { 1, 2, 3 }));
        }

        /// <summary>Tests that <see cref="RewardTables.Read"/> reports a star or a line it can't read, and leaves the row out.</summary>
        [Test]
        public void TestRead_ABrokenStarOrLineIsReported()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int row = ContentCsvs.ReplaceRow(csvs, ContentFileNames.Constellations, "unicorn,",
                "unicorn,planet-1,470:56 418-150 356:146,1-2 2:3,,");
            ContentProblems problems = new ContentProblems();

            RewardContent rewards = RewardTables.Read(csvs, problems);

            ContentProblem[] errors = problems.All.Where(problem => problem.IsError).ToArray();
            ContentCsvs.AssertError(errors, ContentFileNames.Constellations, row, "418-150", "x:y");
            ContentCsvs.AssertError(errors, ContentFileNames.Constellations, row, "2:3", "star numbers");
            Assert.That(rewards.Constellations.All, Is.Empty);
        }

        /// <summary>Tests that <see cref="RewardTables.Read"/> reports a missing file and an id used twice.</summary>
        [Test]
        public void TestRead_AMissingFileAndARepeatedId()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            csvs.Remove(ContentFileNames.Trails);
            int row = ContentCsvs.AddRow(csvs, ContentFileNames.Stickers, "guitar,,");
            int avatarRow = ContentCsvs.AddRow(csvs, ContentFileNames.Avatars, "dog,,");
            ContentProblems problems = new ContentProblems();

            RewardTables.Read(csvs, problems);

            ContentProblem[] errors = problems.All.Where(problem => problem.IsError).ToArray();
            ContentCsvs.AssertError(errors, ContentFileNames.Trails, 0, "missing");
            ContentCsvs.AssertError(errors, ContentFileNames.Stickers, row, "guitar", "already used");
            ContentCsvs.AssertError(errors, ContentFileNames.Avatars, avatarRow, "dog", "already used");
        }
    }
}
