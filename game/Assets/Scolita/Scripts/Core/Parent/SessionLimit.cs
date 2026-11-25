using System;
using Scolita.Core.Timing;

namespace Scolita.Core.Parent
{
    /// <summary>
    /// The daily limit a parent sets (PRD §3.7, decision D8: off by default): off, or one of the rules'
    /// choices in minutes. When today's play reaches it, the rocket rests: the lesson begun always plays to
    /// its end, then the map sleeps until the next day or until a parent passes the gate.
    /// </summary>
    public readonly struct SessionLimit : IEquatable<SessionLimit>
    {
        private SessionLimit(int minutes)
        {
            Minutes = minutes;
        }

        /// <summary>No limit.</summary>
        public static SessionLimit Off => default;

        /// <summary>Whether there is a limit.</summary>
        public bool IsOn => Minutes > 0;

        /// <summary>The limit in minutes; 0 when it is off.</summary>
        public int Minutes { get; }

        /// <summary>
        /// The limit of <paramref name="minutes"/> when it is one of <paramref name="rules"/>' choices, else off
        /// (0, and a number from another version of the app, read as off). Called by the learning settings and
        /// the tests.
        /// </summary>
        public static SessionLimit Of(int minutes, ParentRules rules)
        {
            if (rules == null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            for (int index = 0; index < rules.SessionLimitMinutes.Count; index++)
            {
                if (rules.SessionLimitMinutes[index] == minutes)
                {
                    return new SessionLimit(minutes);
                }
            }

            return Off;
        }

        /// <summary>
        /// Whether <paramref name="play"/> has reached this limit on <paramref name="today"/>; never when it is
        /// off. Called by the map (when it opens and while it is shown) and the tests.
        /// </summary>
        public bool IsReachedBy(DailyPlay play, CalendarDay today)
        {
            return IsOn && play.PlayedOn(today) >= TimeSpan.FromMinutes(Minutes);
        }

        /// <inheritdoc/>
        public bool Equals(SessionLimit other)
        {
            return Minutes == other.Minutes;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is SessionLimit other && Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Minutes;
        }

        /// <summary>The minutes, or "off", for test messages.</summary>
        public override string ToString()
        {
            return IsOn ? Minutes + " min" : "off";
        }
    }
}
