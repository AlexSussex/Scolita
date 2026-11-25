using System;

namespace Scolita.Core.Tracing
{
    /// <summary>
    /// The numbers of <c>TraceLetter</c> she meets (part of <c>LearningRules</c>, filled from the learning
    /// rules asset): how forgiving each of its three rounds is, the first the most and the last the least, and how
    /// near a join a handwritten word's next letter may be begun after she lifted her finger there.
    /// </summary>
    public sealed class TracingTuning
    {
        /// <summary>How many rounds a <c>TraceLetter</c> step has: on dots, on a shadow, from the start dot only.</summary>
        public const int Rounds = 3;

        private readonly StrokeTolerance[] byRound;

        /// <summary>
        /// Creates the numbers of the three rounds and the join radius; throws an
        /// <see cref="ArgumentOutOfRangeException"/> when a later round is more forgiving than an earlier one (a wider
        /// corridor or start radius, a smaller coverage), or when the join radius is smaller than the first round's
        /// start radius. Called by the learning rules asset and the tests.
        /// </summary>
        public TracingTuning(StrokeTolerance dotted, StrokeTolerance faint, StrokeTolerance startDotOnly, float joinRadius)
        {
            CheckOrder(dotted, faint, nameof(faint));
            CheckOrder(faint, startDotOnly, nameof(startDotOnly));
            if (!(joinRadius >= dotted.StartRadius))
            {
                throw new ArgumentOutOfRangeException(nameof(joinRadius), joinRadius,
                    "The join radius is at least the first round's start radius.");
            }

            byRound = new[] { dotted, faint, startDotOnly };
            JoinRadius = joinRadius;
        }

        /// <summary>
        /// How near the join (the end of the letter before) her line may begin the next letter of a handwritten word,
        /// in a shape's units, after she lifted her finger at the join (a lifted join is forgiven).
        /// </summary>
        public float JoinRadius { get; }

        private static void CheckOrder(in StrokeTolerance earlier, in StrokeTolerance later, string name)
        {
            if (later.Corridor > earlier.Corridor || later.StartRadius > earlier.StartRadius || later.Coverage < earlier.Coverage)
            {
                throw new ArgumentOutOfRangeException(name,
                    "A later round is never more forgiving than an earlier one (its corridor, start radius and coverage).");
            }
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
