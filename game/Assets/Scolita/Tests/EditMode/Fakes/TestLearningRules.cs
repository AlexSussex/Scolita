using Scolita.Core.Lessons;
using Scolita.Core.Tracing;

namespace Scolita.Tests.EditMode.Fakes
{
    /// <summary>
    /// <see cref="LearningRules"/> with the numbers of the learning rules asset's defaults, for the tests
    /// that only care about one of them. Used by the activity and lesson tests.
    /// </summary>
    public static class TestLearningRules
    {
        /// <summary>
        /// The default rules (2 tries, praise not repeated within 3, 10 stars with 4 right, the tracing rounds of
        /// <see cref="Tracing"/>); called by the tests.
        /// </summary>
        public static LearningRules Default()
        {
            return Make(2);
        }

        /// <summary>
        /// The default rules but with <paramref name="triesBeforeAnswerShown"/> tries before the answer is
        /// shown; called by the tests.
        /// </summary>
        public static LearningRules WithTries(int triesBeforeAnswerShown)
        {
            return Make(triesBeforeAnswerShown);
        }

        /// <summary>
        /// The learning rules asset's default tracing rounds: corridor 32, start radius 34, coverage 0.8; then 28,
        /// 30, 0.85; then 24, 28, 0.9. Called by the tests.
        /// </summary>
        public static TracingTuning Tracing()
        {
            return MakeTracing();
        }

        private static LearningRules Make(int tries)
        {
            return new LearningRules(tries, 3, new LetterHuntTuning(10, 4, 60f), MakeTracing());
        }

        private static TracingTuning MakeTracing()
        {
            return new TracingTuning(new StrokeTolerance(32f, 34f, 0.8f), new StrokeTolerance(28f, 30f, 0.85f),
                new StrokeTolerance(24f, 28f, 0.9f));
        }
    }
}
