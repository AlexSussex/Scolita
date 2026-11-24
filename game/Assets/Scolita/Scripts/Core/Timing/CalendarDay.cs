using System;
using System.Globalization;

namespace Scolita.Core.Timing
{
    /// <summary>
    /// A day on the tablet's calendar, without a time or a time zone. Core counts days with it, so a
    /// review "after 1 day" means the next calendar day on the tablet, whatever the hour.
    /// Given by <see cref="IClock.Today"/>.
    /// </summary>
    public readonly struct CalendarDay : IEquatable<CalendarDay>, IComparable<CalendarDay>
    {
        private const string IsoFormat = "yyyy-MM-dd";

        private CalendarDay(int dayNumber)
        {
            DayNumber = dayNumber;
        }

        /// <summary>The number of whole days since 1 January of year 1, the day's identity.</summary>
        public int DayNumber { get; }

        /// <summary>
        /// The day of <paramref name="year"/>, <paramref name="month"/> and <paramref name="day"/>;
        /// throws an <see cref="ArgumentOutOfRangeException"/> for a date that doesn't exist.
        /// Called by <c>CalendarDayTests</c> and the fake clocks of the tests.
        /// </summary>
        public static CalendarDay FromDate(int year, int month, int day)
        {
            return FromDateTimeValue(new DateTime(year, month, day));
        }

        /// <summary>
        /// The calendar day of <paramref name="localTime"/>, its time of day dropped. Called by the
        /// Unity side's <c>SystemClock</c> with the tablet's local time, and by <c>CalendarDayTests</c>.
        /// </summary>
        public static CalendarDay FromDateTime(DateTime localTime)
        {
            return FromDateTimeValue(localTime);
        }

        /// <summary>
        /// The number of calendar days from <paramref name="earlier"/> to this day: 1 for the next
        /// day, negative when <paramref name="earlier"/> is later. Called by Core's review rules and
        /// by <c>CalendarDayTests</c>.
        /// </summary>
        public int DaysSince(CalendarDay earlier)
        {
            return DayNumber - earlier.DayNumber;
        }

        /// <summary>
        /// The day <paramref name="days"/> calendar days after this one (before it when negative).
        /// Called by Core's review rules and by <c>CalendarDayTests</c>.
        /// </summary>
        public CalendarDay AddDays(int days)
        {
            return new CalendarDay(checked(DayNumber + days));
        }

        /// <inheritdoc/>
        public bool Equals(CalendarDay other)
        {
            return DayNumber == other.DayNumber;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is CalendarDay other && Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return DayNumber;
        }

        /// <inheritdoc/>
        public int CompareTo(CalendarDay other)
        {
            return DayNumber.CompareTo(other.DayNumber);
        }

        /// <summary>The day as <c>yyyy-MM-dd</c> (ISO 8601), for save files and logs.</summary>
        public override string ToString()
        {
            return new DateTime(DayNumber * TimeSpan.TicksPerDay).ToString(IsoFormat, CultureInfo.InvariantCulture);
        }

        /// <summary>Whether two days are the same day.</summary>
        public static bool operator ==(CalendarDay left, CalendarDay right)
        {
            return left.Equals(right);
        }

        /// <summary>Whether two days are different days.</summary>
        public static bool operator !=(CalendarDay left, CalendarDay right)
        {
            return !left.Equals(right);
        }

        private static CalendarDay FromDateTimeValue(DateTime time)
        {
            return new CalendarDay((int)(time.Date.Ticks / TimeSpan.TicksPerDay));
        }
    }
}
