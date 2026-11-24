using System;
using NUnit.Framework;
using Scolita.Core.Timing;
using Scolita.Game.Timing;

namespace Scolita.Tests.EditMode.Game
{
    /// <summary>Tests <see cref="SystemClock"/>, the tablet's clock for Core.</summary>
    public sealed class SystemClockTests
    {
        /// <summary>Tests that <see cref="SystemClock.Today"/> is the local calendar day.</summary>
        [Test]
        public void TestToday_IsTheLocalCalendarDay()
        {
            SystemClock clock = new SystemClock();
            CalendarDay before = CalendarDay.FromDateTime(DateTime.Now);

            CalendarDay today = clock.Today;

            CalendarDay after = CalendarDay.FromDateTime(DateTime.Now);
            Assert.That(today.DayNumber, Is.InRange(before.DayNumber, after.DayNumber));
        }

        /// <summary>Tests that <see cref="SystemClock.SessionElapsed"/> starts near zero and never goes back.</summary>
        [Test]
        public void TestSessionElapsed_StartsNearZeroAndGrows()
        {
            SystemClock clock = new SystemClock();

            TimeSpan first = clock.SessionElapsed;
            TimeSpan second = clock.SessionElapsed;

            Assert.That(first, Is.GreaterThanOrEqualTo(TimeSpan.Zero));
            Assert.That(first, Is.LessThan(TimeSpan.FromSeconds(5)));
            Assert.That(second, Is.GreaterThanOrEqualTo(first));
        }
    }
}
