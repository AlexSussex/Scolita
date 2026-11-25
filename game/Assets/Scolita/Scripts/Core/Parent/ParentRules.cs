using System;
using System.Collections.Generic;
using System.Linq;

namespace Scolita.Core.Parent
{
    /// <summary>
    /// The numbers of the parent gate and the daily limit (filled from the learning rules asset, beside
    /// <c>LearningRules</c>): how long the parent button is held, the range of the number said, how many numbers are
    /// offered and how long the gate rests after a wrong one, and the daily limit's choices in minutes (off is always a
    /// choice too).
    /// </summary>
    public sealed class ParentRules
    {
        /// <summary>The fewest numbers the gate offers.</summary>
        public const int FewestGateChoices = 2;

        /// <summary>The most numbers the gate offers (the panel's room).</summary>
        public const int MostGateChoices = 4;

        private readonly int[] sessionLimitMinutes;

        /// <summary>
        /// Creates the numbers; throws an <see cref="ArgumentOutOfRangeException"/> for one out of its range (a
        /// hold longer than 0 seconds, numbers from 0, room in the range for every choice,
        /// <see cref="FewestGateChoices"/> to <see cref="MostGateChoices"/> choices, a rest of 0 seconds or more, at least
        /// one limit, each of at least a minute, rising) and an <see cref="ArgumentNullException"/> for no limits. Called by the learning
        /// rules asset and the tests.
        /// </summary>
        public ParentRules(float gateHoldSeconds, int gateLowestNumber, int gateHighestNumber, int gateChoices,
            float gateRestSeconds, IReadOnlyList<int> sessionLimitMinutes)
        {
            if (!(gateHoldSeconds > 0f))
            {
                throw new ArgumentOutOfRangeException(nameof(gateHoldSeconds), gateHoldSeconds,
                    "The parent button is held for more than 0 seconds.");
            }

            if (gateLowestNumber < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(gateLowestNumber), gateLowestNumber,
                    "The gate's numbers start at 0 or more.");
            }

            if (gateChoices < FewestGateChoices || gateChoices > MostGateChoices)
            {
                throw new ArgumentOutOfRangeException(nameof(gateChoices), gateChoices,
                    "The gate offers " + FewestGateChoices + " to " + MostGateChoices + " numbers.");
            }

            if (gateHighestNumber - gateLowestNumber + 1 < gateChoices)
            {
                throw new ArgumentOutOfRangeException(nameof(gateHighestNumber), gateHighestNumber,
                    "The gate's range has room for its " + gateChoices + " different numbers.");
            }

            if (!(gateRestSeconds >= 0f))
            {
                throw new ArgumentOutOfRangeException(nameof(gateRestSeconds), gateRestSeconds,
                    "The gate rests for 0 seconds or more after a wrong number.");
            }

            if (sessionLimitMinutes == null)
            {
                throw new ArgumentNullException(nameof(sessionLimitMinutes));
            }

            if (sessionLimitMinutes.Count == 0 || sessionLimitMinutes[0] < 1 || !IsRising(sessionLimitMinutes))
            {
                throw new ArgumentOutOfRangeException(nameof(sessionLimitMinutes), sessionLimitMinutes.Count,
                    "The daily limit has at least one choice, each of at least a minute, rising.");
            }

            GateHoldSeconds = gateHoldSeconds;
            GateLowestNumber = gateLowestNumber;
            GateHighestNumber = gateHighestNumber;
            GateChoices = gateChoices;
            GateRestSeconds = gateRestSeconds;
            this.sessionLimitMinutes = sessionLimitMinutes.ToArray();
        }

        /// <summary>How long the parent button is held before the gate asks (3 seconds).</summary>
        public float GateHoldSeconds { get; }

        /// <summary>The lowest number the gate says (11).</summary>
        public int GateLowestNumber { get; }

        /// <summary>The highest number the gate says (19).</summary>
        public int GateHighestNumber { get; }

        /// <summary>How many numbers the gate offers, the one said among them (3).</summary>
        public int GateChoices { get; }

        /// <summary>
        /// How long the gate rests after a wrong number (30 seconds): the hold doesn't fill meanwhile, so guessing one
        /// number in three again and again takes long (the fresh-eyes review).
        /// </summary>
        public float GateRestSeconds { get; }

        /// <summary>The daily limit's choices beside off, in minutes, rising (10, 15, 20).</summary>
        public IReadOnlyList<int> SessionLimitMinutes => sessionLimitMinutes;

        private static bool IsRising(IReadOnlyList<int> minutes)
        {
            for (int index = 1; index < minutes.Count; index++)
            {
                if (minutes[index] <= minutes[index - 1])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
