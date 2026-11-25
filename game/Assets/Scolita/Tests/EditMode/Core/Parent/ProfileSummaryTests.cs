using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Parent;
using Scolita.Core.Progress;
using Scolita.Core.Timing;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Parent
{
    /// <summary>Tests <see cref="ProfileSummary"/>, a profile's line in the parent area's list, on the real content.</summary>
    public sealed class ProfileSummaryTests
    {
        /// <summary>Tests that <see cref="ProfileSummary.Of"/> counts the letters learned per language and finds the last day played.</summary>
        [Test]
        public void TestOf_LearnedLettersAndTheLastDay()
        {
            Curriculum curriculum = RealContent.Read();
            CalendarDay today = CalendarDay.FromDate(2026, 11, 25);
            LearnerProgress progress = Progresses.Learned(today, Language.Romanian, "a", "m");
            progress.Meet(LearningItem.Letter(ContentId.Create("t"), Language.English));
            progress.RecordDayPlayed(today.AddDays(-3));
            progress.RecordDayPlayed(today);
            progress.RecordDayPlayed(today.AddDays(-1));
            ParentReport report = new ParentReport(curriculum, curriculum.Domains[0],
                MapRoutes.Of(curriculum, LanguageMix.Both, progress), progress, today);

            ProfileSummary summary = ProfileSummary.Of(report, progress);

            Assert.That(summary.LearnedRomanian, Is.EqualTo(2));
            Assert.That(summary.LearnedEnglish, Is.EqualTo(0));
            Assert.That(summary.HasPlayed, Is.True);
            Assert.That(summary.LastPlayed, Is.EqualTo(today));
        }

        /// <summary>Tests that <see cref="ProfileSummary.Of"/> of a profile that never played has nothing learned and no day.</summary>
        [Test]
        public void TestOf_NeverPlayed()
        {
            Curriculum curriculum = RealContent.Read();
            LearnerProgress progress = Progresses.Empty();
            ParentReport report = new ParentReport(curriculum, curriculum.Domains[0],
                MapRoutes.Of(curriculum, LanguageMix.Both, progress), progress, CalendarDay.FromDate(2026, 11, 25));

            ProfileSummary summary = ProfileSummary.Of(report, progress);

            Assert.That(summary.LearnedRomanian, Is.EqualTo(0));
            Assert.That(summary.LearnedEnglish, Is.EqualTo(0));
            Assert.That(summary.HasPlayed, Is.False);
        }
    }
}
