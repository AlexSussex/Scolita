using System;

namespace Scolita.Core.Tracing
{
    /// <summary>
    /// The numbers of <c>TraceLetter</c> she meets (part of <c>LearningRules</c>, filled from the learning
    /// rules asset): how forgiving each of its three rounds is, the first the most and the last the least.
    /// </summary>
    public sealed class TracingTuning
    {
        /// <summary>How many rounds a <c>TraceLetter</c> step has: on dots, on a shadow, from the start dot only.</summary>
        public const int Rounds = 3;

        private readonly StrokeTolerance[] byRound;

        /// <summary>
        /// Creates the numbers of the three rounds; throws an <see cref="ArgumentOutOfRangeException"/> when a
        /// later round is more forgiving than an earlier one. Called by the learning rules asset and the tests.
        /// </summary>
        public TracingTuning(StrokeTolerance dotted, StrokeTolerance faint, StrokeTolerance startDotOnly)
        {
            if (faint.Corridor > dotted.Corridor || startDotOnly.Corridor > faint.Corridor)
            {
                throw new ArgumentOutOfRangeException(nameof(faint), "A later round's corridor is never wider than an earlier one's.");
            }

            if (faint.Coverage < dotted.Coverage || startDotOnly.Coverage < faint.Coverage)
            {
                throw new ArgumentOutOfRangeException(nameof(faint), "A later round never asks for less of the stroke.");
            }

            byRound = new[] { dotted, faint, startDotOnly };
        }

        /// <summary>
        /// The tolerance of round <paramref name="round"/> (from 0); throws an
        /// <see cref="ArgumentOutOfRangeException"/> outside the rounds. Called by <c>TraceLetterRules</c> and the tests.
        /// </summary>
        public StrokeTolerance ForRound(int round)
        {
            if (round < 0 || round >= Rounds)
            {
                throw new ArgumentOutOfRangeException(nameof(round), round, "A TraceLetter step has " + Rounds + " rounds.");
            }

            return byRound[round];
        }
    }
}
