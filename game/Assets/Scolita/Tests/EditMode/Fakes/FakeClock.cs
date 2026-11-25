using System;
using Scolita.Core.Timing;

namespace Scolita.Tests.EditMode.Fakes
{
    /// <summary>
    /// An <see cref="IClock"/> whose day the test sets and moves on, so days pass without waiting. Used by the
    /// progress, review and saving tests.
    /// </summary>
    public sealed class FakeClock : IClock
    {
        /// <summary>Starts on <paramref name="today"/>; called by the tests.</summary>
        public FakeClock(CalendarDay today)
        {
            Today = today;
        }

        /// <inheritdoc/>
        public CalendarDay Today { get; private set; }

        /// <inheritdoc/>
        public TimeSpan SessionElapsed => TimeSpan.Zero;

        /// <summary>Moves the day <paramref name="days"/> on; called by the tests.</summary>
        public void PassDays(int days)
        {
            Today = Today.AddDays(days);
        }
    }
}
