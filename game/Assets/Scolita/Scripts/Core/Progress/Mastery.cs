namespace Scolita.Core.Progress
{
    /// <summary>
    /// When an item is learned (<c>AGENTS.md</c> → "Learning": seen often, on different days), the one
    /// owner of this rule: its right answers reach its language's number, across at least
    /// <see cref="ProgressRules.KindsForLearned"/> activity kinds, on at least
    /// <see cref="ProgressRules.DaysForLearned"/> different days.
    /// </summary>
    public static class Mastery
    {
        /// <summary>
        /// Whether <paramref name="item"/>'s answers meet <paramref name="rules"/> for "learned". Called by
        /// <see cref="LearnerProgress"/> after each right answer, and by the tests.
        /// </summary>
        public static bool IsReached(ItemProgress item, ProgressRules rules)
        {
            return item.TotalRights >= rules.RightsForLearned(item.Item.Language)
                && item.KindsRight >= rules.KindsForLearned
                && item.RightDays.Count >= rules.DaysForLearned;
        }
    }
}
