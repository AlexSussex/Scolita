using System;

namespace Scolita.Core.Lessons
{
    /// <summary>
    /// The numbers of how she learns that the child and the parent meet (AGENTS.md → "Quality"): one
    /// place, filled from the learning rules asset on the Unity side, never written in code.
    /// <c>progress-review</c> adds the numbers of <c>docs/curriculum.md</c> §5.
    /// </summary>
    public sealed class LearningRules
    {
        /// <summary>
        /// Creates the rules; throws an <see cref="ArgumentOutOfRangeException"/> for a number out of
        /// its range. Called by the learning rules asset and the tests.
        /// </summary>
        public LearningRules(int triesBeforeAnswerShown, int praiseNotRepeatedWithin)
        {
            if (triesBeforeAnswerShown < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(triesBeforeAnswerShown), triesBeforeAnswerShown,
                    "At least one try comes before the answer is shown.");
            }

            if (praiseNotRepeatedWithin < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(praiseNotRepeatedWithin), praiseNotRepeatedWithin,
                    "The praise variety can't be negative.");
            }

            TriesBeforeAnswerShown = triesBeforeAnswerShown;
            PraiseNotRepeatedWithin = praiseNotRepeatedWithin;
        }

        /// <summary>
        /// How many misses of one part show its answer (2: the first miss gets a hint, the second shows
        /// the answer, <c>docs/curriculum.md</c> §4.3).
        /// </summary>
        public int TriesBeforeAnswerShown { get; }

        /// <summary>How many of the last praise lines said are never said again next (praise variety).</summary>
        public int PraiseNotRepeatedWithin { get; }
    }
}
