using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Map;
using Scolita.Core.Progress;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Progress
{
    /// <summary>Tests <see cref="ProgressRebuild"/>: her progress made from the saves that came before the <c>progress</c> file.</summary>
    public sealed class ProgressRebuildTests
    {
        /// <summary>
        /// Tests that <see cref="ProgressRebuild.From"/> joins 0.10.0's map progress and the lessons before the one in progress,
        /// keeps the rocket's stop and a lesson the content doesn't know, and meets the letters the done lessons introduced
        /// without inventing a right answer.
        /// </summary>
        [Test]
        public void TestFrom_JoinsTheEarlierSaves()
        {
            Curriculum curriculum = RealContent.Read();
            MapProgress map = new MapProgress(new[] { ContentId.Create("ro-a"), ContentId.Create("en-a"), ContentId.Create("lesson-from-later") },
                ContentId.Create("ro-m"));
            MapProgress before = new MapProgress(new[] { ContentId.Create("ro-a"), ContentId.Create("en-a"), ContentId.Create("ro-m") }, default);

            LearnerProgress progress = ProgressRebuild.From(map, before, curriculum);

            Assert.That(progress.Lessons.DoneLessons.Select(id => id.ToString()),
                Is.EqualTo(new[] { "ro-a", "en-a", "lesson-from-later", "ro-m" }));
            Assert.That(progress.Lessons.RocketStop.ToString(), Is.EqualTo("ro-m"));
            Assert.That(progress.Items.Select(item => item.Item.ToString()),
                Is.EqualTo(new[] { "letter a (ro)", "letter a (en)", "letter m (ro)" }));
            Assert.That(progress.Items.All(item => item.State == ItemState.Learning && item.TotalRights == 0), Is.True);
            Assert.That(progress.DaysPlayed, Is.Empty);
        }

        /// <summary>
        /// Tests that <see cref="ProgressRebuild.From"/> with no map progress (0.9.0 and earlier) takes the lessons before the
        /// one in progress, with no rocket stop, and that a planet's party (in both languages) meets no letter.
        /// </summary>
        [Test]
        public void TestFrom_BeforeTheMap()
        {
            Curriculum curriculum = RealContent.Read();
            MapProgress before = new MapProgress(new[] { ContentId.Create("capitals-1"), ContentId.Create("party-1") }, default);

            LearnerProgress progress = ProgressRebuild.From(null, before, curriculum);

            Assert.That(progress.IsDone(ContentId.Create("party-1")), Is.True);
            Assert.That(progress.Lessons.RocketStop.IsSet, Is.False);
            Assert.That(progress.Items, Is.Empty);
            Assert.That(progress.StateOf(LearningItem.Letter(ContentId.Create("a"), Language.Romanian)), Is.EqualTo(ItemState.New));
        }
    }
}
