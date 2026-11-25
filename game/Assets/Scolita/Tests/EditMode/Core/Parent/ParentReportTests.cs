using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Map;
using Scolita.Core.Parent;
using Scolita.Core.Progress;
using Scolita.Core.Timing;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Parent
{
    /// <summary>Tests <see cref="ParentReport"/> on the real content.</summary>
    public sealed class ParentReportTests
    {
        /// <summary>Tests that <see cref="ParentReport.Rows"/> are Romanian's then English's track letters in teaching order, all new at first.</summary>
        [Test]
        public void TestRows_TheTracksLettersAllNew()
        {
            Curriculum curriculum = RealContent.Read();
            Domain domain = curriculum.Domains[0];
            CalendarDay today = CalendarDay.FromDate(2026, 11, 25);

            ParentReport report = new ParentReport(curriculum, domain, MapRoutes.Of(curriculum, LanguageMix.Both),
                Progresses.Empty(), today);

            Assert.That(report.Rows.Select(row => row.Language), Is.EqualTo(new[] { Language.Romanian, Language.English }));
            Assert.That(report.Rows[0].Cells.Select(cell => cell.Letter.Lower).Take(4), Is.EqualTo(new[] { "a", "m", "t", "p" }));
            Assert.That(report.Rows[0].Cells.Select(cell => cell.Letter.Id),
                Is.EqualTo(curriculum.Track(domain.Id, Language.Romanian).Letters));
            Assert.That(report.Rows[1].Cells.Select(cell => cell.Letter.Id),
                Is.EqualTo(curriculum.Track(domain.Id, Language.English).Letters));
            Assert.That(report.Rows.SelectMany(row => row.Cells).Select(cell => cell.State), Is.All.EqualTo(ItemState.New));
        }

        /// <summary>Tests that <see cref="ParentReport.Rows"/> show each letter's state in its own language.</summary>
        [Test]
        public void TestRows_EachLanguagesOwnStates()
        {
            Curriculum curriculum = RealContent.Read();
            CalendarDay today = CalendarDay.FromDate(2026, 11, 25);
            LearnerProgress progress = Progresses.Learned(today, Language.Romanian, "a", "m");
            progress.Meet(LearningItem.Letter(ContentId.Create("t"), Language.Romanian));
            progress.Meet(LearningItem.Letter(ContentId.Create("a"), Language.English));

            ParentReport report = new ParentReport(curriculum, curriculum.Domains[0],
                MapRoutes.Of(curriculum, LanguageMix.Both), progress, today);

            Assert.That(report.Rows[0].Cells.Select(cell => cell.State).Take(5), Is.EqualTo(new[]
            {
                ItemState.Learned, ItemState.Learned, ItemState.Learning, ItemState.New, ItemState.New
            }));
            Assert.That(report.Rows[1].Cells.Select(cell => cell.State).Take(2), Is.EqualTo(new[]
            {
                ItemState.Learning, ItemState.New
            }));
        }

        /// <summary>Tests that <see cref="ParentReport.Rows"/> keep the other language's row when the mix hides it.</summary>
        [Test]
        public void TestRows_OtherLanguageShownWhenTheMixHidesIt()
        {
            Curriculum curriculum = RealContent.Read();
            CalendarDay today = CalendarDay.FromDate(2026, 11, 25);
            LearnerProgress progress = Progresses.Learned(today, Language.Romanian, "a");

            ParentReport report = new ParentReport(curriculum, curriculum.Domains[0],
                MapRoutes.Of(curriculum, LanguageMix.EnglishOnly, progress), progress, today);

            Assert.That(report.Rows[0].Cells[0].State, Is.EqualTo(ItemState.Learned));
        }

        /// <summary>Tests that <see cref="ParentReport.DaysPlayed"/> are the last 14 days, the oldest first and today last.</summary>
        [Test]
        public void TestDaysPlayed_TheLastTwoWeeks()
        {
            Curriculum curriculum = RealContent.Read();
            CalendarDay today = CalendarDay.FromDate(2026, 11, 25);
            LearnerProgress progress = Progresses.Empty();
            progress.RecordDayPlayed(today.AddDays(-14));
            progress.RecordDayPlayed(today.AddDays(-13));
            progress.RecordDayPlayed(today.AddDays(-2));
            progress.RecordDayPlayed(today);

            ParentReport report = new ParentReport(curriculum, curriculum.Domains[0],
                MapRoutes.Of(curriculum, LanguageMix.Both), progress, today);

            bool[] expected = new bool[ParentReport.DaysShown];
            expected[0] = true;
            expected[11] = true;
            expected[13] = true;
            Assert.That(report.DaysPlayed, Is.EqualTo(expected));
        }

        /// <summary>Tests that <see cref="ParentReport.NextLessons"/> are the next three lessons from the rocket's, with their planet and letter.</summary>
        [Test]
        public void TestNextLessons_TheNextThreeFromTheRocket()
        {
            Curriculum curriculum = RealContent.Read();
            CalendarDay today = CalendarDay.FromDate(2026, 11, 25);

            ParentReport report = new ParentReport(curriculum, curriculum.Domains[0],
                MapRoutes.Before(curriculum, "capitals-1"), Progresses.Empty(), today);

            Assert.That(report.NextLessons.Select(next => next.Lesson.Id.ToString()),
                Is.EqualTo(new[] { "capitals-1", "party-1", "ro-s" }));
            Assert.That(report.NextLessons.Select(next => next.PlanetNumber), Is.EqualTo(new[] { 1, 1, 2 }));
            Assert.That(report.NextLessons.Select(next => next.Letter?.Lower), Is.EqualTo(new[] { null, null, "s" }));
        }

        /// <summary>Tests that <see cref="ParentReport.NextLessons"/> is empty once every lesson is done.</summary>
        [Test]
        public void TestNextLessons_NoneWhenAllDone()
        {
            Curriculum curriculum = RealContent.Read();
            CalendarDay today = CalendarDay.FromDate(2026, 11, 25);

            ParentReport report = new ParentReport(curriculum, curriculum.Domains[0], MapRoutes.AllDone(curriculum),
                Progresses.Empty(), today);

            Assert.That(report.NextLessons, Is.Empty);
        }

        /// <summary>Tests that <see cref="ParentReport.Voices"/> counts the real content's lines per language.</summary>
        [Test]
        public void TestVoices_TheContentsLines()
        {
            Curriculum curriculum = RealContent.Read();
            CalendarDay today = CalendarDay.FromDate(2026, 11, 25);

            ParentReport report = new ParentReport(curriculum, curriculum.Domains[0],
                MapRoutes.Of(curriculum, LanguageMix.Both), Progresses.Empty(), today);

            int romanianToRecord = curriculum.VoiceLines.All
                .Where(line => !line.Retired && line.Language == Language.Romanian)
                .Count(line => line.Status != VoiceLineStatus.FromSounds);
            Assert.That(report.Voices.Select(status => status.Language), Is.EqualTo(new[] { Language.Romanian, Language.English }));
            Assert.That(report.Voices[0].ToRecord, Is.EqualTo(romanianToRecord));
            Assert.That(report.Voices[1].ToRecord, Is.GreaterThan(0));
        }
    }
}
