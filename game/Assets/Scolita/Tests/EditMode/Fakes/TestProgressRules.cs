using Scolita.Core.Progress;

namespace Scolita.Tests.EditMode.Fakes
{
    /// <summary>
    /// <see cref="ProgressRules"/> with the learning rules asset's defaults (<c>docs/curriculum.md</c> §5): 6 right
    /// answers in each language, across 2 kinds, on 2 days; reviews after 1, 3, 7, 14 and 30 days, a miss one step
    /// back; 2 warm-ups of 3 letters; damaged copies kept until 3 fine days. Used by the progress tests.
    /// </summary>
    public static class TestProgressRules
    {
        /// <summary>The defaults; called by the tests.</summary>
        public static ProgressRules Default()
        {
            return Make(6);
        }

        /// <summary>The defaults, but <paramref name="englishRights"/> right answers for an English item; called by the tests.</summary>
        public static ProgressRules WithEnglishRights(int englishRights)
        {
            return Make(englishRights);
        }

        private static ProgressRules Make(int englishRights)
        {
            return new ProgressRules(6, englishRights, 2, 2, new[] { 1, 3, 7, 14, 30 }, 1, 2, 3, 3);
        }
    }
}
