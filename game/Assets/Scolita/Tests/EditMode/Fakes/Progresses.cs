using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Map;
using Scolita.Core.Progress;
using Scolita.Core.Timing;

namespace Scolita.Tests.EditMode.Fakes
{
    /// <summary>Ready-made <see cref="LearnerProgress"/> for the progress tests.</summary>
    public static class Progresses
    {
        /// <summary>Nothing met, nothing done; called by the tests.</summary>
        public static LearnerProgress Empty()
        {
            return new LearnerProgress(new MapProgress(new ContentId[0], default), null, null);
        }

        /// <summary>
        /// A progress where each of <paramref name="letters"/> is learned in <paramref name="language"/> on
        /// <paramref name="learnedOn"/> (6 right answers across two kinds, on that day and the one before), so its
        /// first review is due the next day. Called by the tests.
        /// </summary>
        public static LearnerProgress Learned(CalendarDay learnedOn, Language language, params string[] letters)
        {
            LearnerProgress progress = Empty();
            ProgressRules rules = TestProgressRules.Default();
            foreach (string letter in letters)
            {
                LearningItem item = LearningItem.Letter(ContentId.Create(letter), language);
                for (int answer = 0; answer < 3; answer++)
                {
                    progress.RecordRight(item, ActivityKind.ListenAndPick, learnedOn.AddDays(-1), rules);
                    progress.RecordRight(item, ActivityKind.FirstSound, learnedOn, rules);
                }
            }

            return progress;
        }
    }
}
