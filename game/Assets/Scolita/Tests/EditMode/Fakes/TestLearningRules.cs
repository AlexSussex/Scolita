using Scolita.Core.Lessons;

namespace Scolita.Tests.EditMode.Fakes
{
    /// <summary>
    /// <see cref="LearningRules"/> with the numbers of the learning rules asset's defaults, for the tests
    /// that only care about one of them. Used by the activity and lesson tests.
    /// </summary>
    public static class TestLearningRules
    {
        /// <summary>The default rules (2 tries, praise not repeated within 3, 10 stars with 4 right); called by the tests.</summary>
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

        private static LearningRules Make(int tries)
        {
            return new LearningRules(tries, 3, new LetterHuntTuning(10, 4, 60f));
        }
    }
}
