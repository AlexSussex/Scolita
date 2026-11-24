using System;

namespace Scolita.Core.Timing
{
    /// <summary>
    /// The only source of time in Core: today's date on the tablet and how long the app has been
    /// open. Implemented by the Unity side's <c>SystemClock</c> and, in tests, by a fake clock
    /// whose day and elapsed time the test sets.
    /// </summary>
    public interface IClock
    {
        /// <summary>Today on the tablet's calendar, in its local time. Read by Core's progress and review rules.</summary>
        CalendarDay Today { get; }

        /// <summary>The time since the app started (the session), never negative. Read by the session limit.</summary>
        TimeSpan SessionElapsed { get; }
    }
}
