using System;
using Scolita.Core.Timing;

namespace Scolita.Core.Parent
{
    /// <summary>
    /// How long a learner has played on one calendar day (the map and the lessons on screen, the app in
    /// front), for the daily limit (<see cref="SessionLimit"/>). Another day's time counts as nothing today.
    /// A value: adding time makes a new one, so counting allocates nothing.
    /// </summary>
    public readonly struct DailyPlay : IEquatable<DailyPlay>
    {
        /// <summary>
        /// <paramref name="played"/> on <paramref name="day"/>; throws an <see cref="ArgumentOutOfRangeException"/>
        /// for a negative time. Called by the play time's save and the tests.
        /// </summary>
        public DailyPlay(CalendarDay day, TimeSpan played)
        {
            if (played < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(played), played, "Play time is never negative.");
            }

            Day = day;
            Played = played;
        }

        /// <summary>The day counted.</summary>
        public CalendarDay Day { get; }

        /// <summary>How long was played on <see cref="Day"/>.</summary>
        public TimeSpan Played { get; }

        /// <summary>
        /// How long was played on <paramref name="today"/>: nothing when the time counted is another day's.
        /// Called by <see cref="SessionLimit"/> and the tests.
        /// </summary>
        public TimeSpan PlayedOn(CalendarDay today)
        {
            return Day == today ? Played : TimeSpan.Zero;
        }

        /// <summary>
        /// This play with <paramref name="more"/> played on <paramref name="today"/>; a new day starts from
        /// nothing. Throws an <see cref="ArgumentOutOfRangeException"/> for a negative time. Called every frame
        /// by the runners' play clock, and the tests.
        /// </summary>
        public DailyPlay Plus(CalendarDay today, TimeSpan more)
        {
            if (more < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(more), more, "Play time is never negative.");
            }

            return new DailyPlay(today, PlayedOn(today) + more);
        }

        /// <summary>
        /// Nothing played yet on <paramref name="today"/>: a parent who passes the gate wakes the rocket. Called by
        /// the parent area when it opens, and the tests.
        /// </summary>
        public static DailyPlay Woken(CalendarDay today)
        {
            return new DailyPlay(today, TimeSpan.Zero);
        }

        /// <inheritdoc/>
        public bool Equals(DailyPlay other)
        {
            return Day == other.Day && Played == other.Played;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is DailyPlay other && Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return (Day.GetHashCode() * 397) ^ Played.GetHashCode();
        }

        /// <summary>The day and the time, for test messages.</summary>
        public override string ToString()
        {
            return Day + " " + Played;
        }
    }
}
