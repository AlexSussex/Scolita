using System;

namespace Scolita.Core.Tracing
{
    /// <summary>
    /// The numbers of <c>TraceLetter</c> she meets (part of <c>LearningRules</c>, filled from the learning
    /// rules asset): how forgiving each of its three rounds is, the first the most and the last the least, how
    /// near a join a handwritten word's next letter may be begun after she lifted her finger there, and how much of
    /// the corridor is left for her fingertip around the guide she sees.
    /// </summary>
    public sealed class TracingTuning
    {
        /// <summary>How many rounds a <c>TraceLetter</c> step has: on dots, on a shadow, from the start dot only.</summary>
        public const int Rounds = 3;

        private readonly StrokeTolerance[] byRound;

        /// <summary>
        /// Creates the numbers of the three rounds, the join radius and the finger allowance; throws an
        /// <see cref="ArgumentOutOfRangeException"/> when a later round is more forgiving than an earlier one (a wider
        /// corridor or start radius, a smaller coverage), when the join radius is smaller than the first round's
        /// start radius, or when the finger allowance is negative or leaves no guide in the last round. Called by the
        /// learning rules asset and the tests.
        /// </summary>
        public TracingTuning(StrokeTolerance dotted, StrokeTolerance faint, StrokeTolerance startDotOnly, float joinRadius,
            float fingerAllowance)
        {
            CheckOrder(dotted, faint, nameof(faint));
            CheckOrder(faint, startDotOnly, nameof(startDotOnly));
            if (!(joinRadius >= dotted.StartRadius))
            {
                throw new ArgumentOutOfRangeException(nameof(joinRadius), joinRadius,
                    "The join radius is at least the first round's start radius.");
            }

            if (!(fingerAllowance >= 0f) || fingerAllowance >= startDotOnly.Corridor)
            {
                throw new ArgumentOutOfRangeException(nameof(fingerAllowance), fingerAllowance,
                    "The finger allowance is at least 0 and less than the last round's corridor.");
            }

            byRound = new[] { dotted, faint, startDotOnly };
            JoinRadius = joinRadius;
            FingerAllowance = fingerAllowance;
        }

        /// <summary>
        /// How near the join (the end of the letter before) her line may begin the next letter of a handwritten word,
        /// in a shape's units, after she lifted her finger at the join (a lifted join is forgiven).
        /// </summary>
        public float JoinRadius { get; }

        /// <summary>
        /// How much of each side of the corridor is for her fingertip, in a shape's units: the guide she follows is drawn
        /// as wide as the corridor less this, so a fingertip touching the guide's edge (its middle, which the screen
        /// reports, a little further out) is still on the stroke. Read by <see cref="TraceState.GuideHalfWidth"/>.
        /// </summary>
        public float FingerAllowance { get; }

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
