using System;
using NUnit.Framework;
using Scolita.Core.Parent;
using Scolita.Core.Timing;

namespace Scolita.Tests.EditMode.Core.Parent
{
    /// <summary>Tests <see cref="DailyPlay"/>.</summary>
    public sealed class DailyPlayTests
    {
        /// <summary>Tests that <see cref="DailyPlay.Plus"/> adds up the time played on the same day.</summary>
        [Test]
        public void TestPlus_SameDayAddsUp()
        {
            CalendarDay today = CalendarDay.FromDate(2026, 11, 25);

            DailyPlay play = DailyPlay.Woken(today).Plus(today, TimeSpan.FromMinutes(4)).Plus(today, TimeSpan.FromSeconds(30));

            Assert.That(play.PlayedOn(today), Is.EqualTo(TimeSpan.FromSeconds(270)));
            Assert.That(play.Day, Is.EqualTo(today));
        }

        /// <summary>Tests that <see cref="DailyPlay.Plus"/> on a new day starts from nothing.</summary>
        [Test]
        public void TestPlus_NewDayStartsFromNothing()
        {
            CalendarDay yesterday = CalendarDay.FromDate(2026, 11, 24);
            CalendarDay today = yesterday.AddDays(1);

            DailyPlay play = new DailyPlay(yesterday, TimeSpan.FromMinutes(25)).Plus(today, TimeSpan.FromMinutes(1));

            Assert.That(play.PlayedOn(today), Is.EqualTo(TimeSpan.FromMinutes(1)));
            Assert.That(play.Day, Is.EqualTo(today));
        }

        /// <summary>Tests that <see cref="DailyPlay.PlayedOn"/> counts another day's time (a clock set back too) as nothing.</summary>
        [Test]
        public void TestPlayedOn_AnotherDayIsNothing()
        {
            CalendarDay today = CalendarDay.FromDate(2026, 11, 25);

            DailyPlay play = new DailyPlay(today, TimeSpan.FromMinutes(25));

            Assert.That(play.PlayedOn(today.AddDays(1)), Is.EqualTo(TimeSpan.Zero));
            Assert.That(play.PlayedOn(today.AddDays(-1)), Is.EqualTo(TimeSpan.Zero));
        }

        /// <summary>Tests that <see cref="DailyPlay(CalendarDay, TimeSpan)"/> and <see cref="DailyPlay.Plus"/> refuse a negative time.</summary>
        [Test]
        public void TestDailyPlay_NegativeThrows()
        {
            CalendarDay today = CalendarDay.FromDate(2026, 11, 25);

            Assert.Throws<ArgumentOutOfRangeException>(() => new DailyPlay(today, TimeSpan.FromSeconds(-1)));
            Assert.Throws<ArgumentOutOfRangeException>(() => DailyPlay.Woken(today).Plus(today, TimeSpan.FromSeconds(-1)));
        }

        /// <summary>Tests that <see cref="DailyPlay.Woken"/> is nothing played today.</summary>
        [Test]
        public void TestWoken_NothingPlayedToday()
        {
            CalendarDay today = CalendarDay.FromDate(2026, 11, 25);

            DailyPlay play = DailyPlay.Woken(today);

            Assert.That(play, Is.EqualTo(new DailyPlay(today, TimeSpan.Zero)));
        }
    }
}
