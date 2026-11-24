using System;
using Scolita.Core.Tracing;

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
        /// its range, and an <see cref="ArgumentNullException"/> for no <paramref name="letterHunt"/> or
        /// <paramref name="tracing"/>.
        /// Called by the learning rules asset and the tests.
        /// </summary>
        public LearningRules(int triesBeforeAnswerShown, int praiseNotRepeatedWithin, LetterHuntTuning letterHunt,
            TracingTuning tracing)
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
            LetterHunt = letterHunt ?? throw new ArgumentNullException(nameof(letterHunt));
            Tracing = tracing ?? throw new ArgumentNullException(nameof(tracing));
        }

        /// <summary>
        /// How many misses of one part show its answer (2: the first miss gets a hint, the second shows
        /// the answer, <c>docs/curriculum.md</c> §4.3).
        /// </summary>
        public int TriesBeforeAnswerShown { get; }

        /// <summary>How many of the last praise lines said are never said again next (praise variety).</summary>
        public int PraiseNotRepeatedWithin { get; }

        /// <summary>The numbers of <c>LetterHunt</c>: its stars and their speed.</summary>
        public LetterHuntTuning LetterHunt { get; }

        /// <summary>The numbers of <c>TraceLetter</c>: how forgiving each of its rounds is.</summary>
        public TracingTuning Tracing { get; }
    }
}
