using System;
using Scolita.Core.Timing;
using UnityEngine;

namespace Scolita.Game.Timing
{
    /// <summary>
    /// The tablet's clock for Core: today's date in the tablet's local time, and the real time
    /// since this clock was made at the start of the session. Created by the scenes' composition
    /// roots and by <c>SystemClockTests</c>.
    /// </summary>
    public sealed class SystemClock : IClock
    {
        private readonly double sessionStart;

        /// <summary>Starts the session now. Called by the composition roots and by <c>SystemClockTests</c>.</summary>
        public SystemClock()
        {
            sessionStart = Time.realtimeSinceStartupAsDouble;
        }

        /// <inheritdoc/>
        public CalendarDay Today => CalendarDay.FromDateTime(DateTime.Now);

        /// <inheritdoc/>
        public TimeSpan SessionElapsed =>
            TimeSpan.FromSeconds(Math.Max(0d, Time.realtimeSinceStartupAsDouble - sessionStart));
    }
}
