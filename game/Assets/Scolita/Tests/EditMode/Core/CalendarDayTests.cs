using System;
using NUnit.Framework;
using Scolita.Core.Timing;

namespace Scolita.Tests.EditMode.Core
{
    /// <summary>Tests <see cref="CalendarDay"/>, the day Core counts reviews in.</summary>
    public sealed class CalendarDayTests
    {
        /// <summary>Tests that <see cref="CalendarDay.FromDateTime"/> drops the time of day.</summary>
        [Test]
        public void TestFromDateTime_DropsTheTimeOfDay()
        {
            CalendarDay morning = CalendarDay.FromDateTime(new DateTime(2026, 11, 24, 0, 0, 1));
            CalendarDay evening = CalendarDay.FromDateTime(new DateTime(2026, 11, 24, 23, 59, 59));

            Assert.That(morning, Is.EqualTo(evening));
            Assert.That(morning, Is.EqualTo(CalendarDay.FromDate(2026, 11, 24)));
        }

        /// <summary>Tests that <see cref="CalendarDay.TryParse"/> reads the ISO form <see cref="CalendarDay.ToString"/> writes, and nothing else.</summary>
        [Test]
        public void TestTryParse_TheIsoFormOnly()
        {
            bool read = CalendarDay.TryParse(CalendarDay.FromDate(2028, 2, 29).ToString(), out CalendarDay leapDay);

            Assert.That(read, Is.True);
            Assert.That(leapDay, Is.EqualTo(CalendarDay.FromDate(2028, 2, 29)));
            Assert.That(CalendarDay.TryParse("2026-02-30", out _), Is.False);
            Assert.That(CalendarDay.TryParse("25.11.2026", out _), Is.False);
            Assert.That(CalendarDay.TryParse("", out _), Is.False);
            Assert.That(CalendarDay.TryParse(null, out CalendarDay none), Is.False);
            Assert.That(none, Is.EqualTo(default(CalendarDay)));
        }

        /// <summary>Tests that <see cref="CalendarDay.DaysSince"/> counts one day across midnight, however few minutes pass.</summary>
        [Test]
        public void TestDaysSince_MidnightMakesTheNextDay()
        {
            CalendarDay lateEvening = CalendarDay.FromDateTime(new DateTime(2026, 11, 24, 23, 58, 0));
            CalendarDay earlyMorning = CalendarDay.FromDateTime(new DateTime(2026, 11, 25, 0, 2, 0));

            Assert.That(earlyMorning.DaysSince(lateEvening), Is.EqualTo(1));
            Assert.That(lateEvening.DaysSince(earlyMorning), Is.EqualTo(-1));
            Assert.That(lateEvening.DaysSince(lateEvening), Is.EqualTo(0));
        }

        /// <summary>Tests that <see cref="CalendarDay.DaysSince"/> counts across months, years and a leap day.</summary>
        [Test]
        public void TestDaysSince_AcrossMonthsYearsAndLeapDay()
        {
            Assert.That(CalendarDay.FromDate(2026, 12, 1).DaysSince(CalendarDay.FromDate(2026, 11, 30)), Is.EqualTo(1));
            Assert.That(CalendarDay.FromDate(2027, 1, 1).DaysSince(CalendarDay.FromDate(2026, 12, 31)), Is.EqualTo(1));
            Assert.That(CalendarDay.FromDate(2028, 3, 1).DaysSince(CalendarDay.FromDate(2028, 2, 28)), Is.EqualTo(2));
            Assert.That(CalendarDay.FromDate(2027, 11, 24).DaysSince(CalendarDay.FromDate(2026, 11, 24)), Is.EqualTo(365));
        }

        /// <summary>Tests that <see cref="CalendarDay.AddDays"/> moves forward and back, and agrees with <see cref="CalendarDay.DaysSince"/>.</summary>
        [Test]
        public void TestAddDays_MovesForwardAndBack()
        {
            CalendarDay today = CalendarDay.FromDate(2026, 12, 30);

            Assert.That(today.AddDays(3), Is.EqualTo(CalendarDay.FromDate(2027, 1, 2)));
            Assert.That(today.AddDays(-30), Is.EqualTo(CalendarDay.FromDate(2026, 11, 30)));
            Assert.That(today.AddDays(7).DaysSince(today), Is.EqualTo(7));
        }

        /// <summary>Tests that <see cref="CalendarDay.CompareTo"/> and the operators order and match days.</summary>
        [Test]
        public void TestCompareTo_OrdersDays()
        {
            CalendarDay first = CalendarDay.FromDate(2026, 11, 24);
            CalendarDay second = CalendarDay.FromDate(2026, 11, 25);

            Assert.That(first.CompareTo(second), Is.LessThan(0));
            Assert.That(second.CompareTo(first), Is.GreaterThan(0));
            Assert.That(first == CalendarDay.FromDate(2026, 11, 24), Is.True);
            Assert.That(first != second, Is.True);
            Assert.That(first.GetHashCode(), Is.EqualTo(CalendarDay.FromDate(2026, 11, 24).GetHashCode()));
        }

        /// <summary>Tests that <see cref="CalendarDay.ToString"/> writes the day as ISO 8601.</summary>
        [Test]
        public void TestToString_WritesIsoDate()
        {
            Assert.That(CalendarDay.FromDate(2026, 3, 5).ToString(), Is.EqualTo("2026-03-05"));
        }

        /// <summary>Tests that <see cref="CalendarDay.FromDate"/> refuses a date that doesn't exist.</summary>
        [Test]
        public void TestFromDate_InvalidDateThrows()
        {
            Assert.That(() => CalendarDay.FromDate(2026, 2, 29), Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => CalendarDay.FromDate(2026, 13, 1), Throws.TypeOf<ArgumentOutOfRangeException>());
        }
    }
}
