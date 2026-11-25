using System;
using System.Linq;
using Scolita.Core.Languages;
using Scolita.Core.Progress;
using Scolita.Core.Timing;

namespace Scolita.Core.Parent
{
    /// <summary>
    /// A profile's line in the parent area's list (design screen 10): the letters learned in each language, as the
    /// grid shows them (<see cref="ParentReport"/>), and the last day played.
    /// </summary>
    public sealed class ProfileSummary
    {
        private ProfileSummary(int learnedRomanian, int learnedEnglish, bool hasPlayed, CalendarDay lastPlayed)
        {
            LearnedRomanian = learnedRomanian;
            LearnedEnglish = learnedEnglish;
            HasPlayed = hasPlayed;
            LastPlayed = lastPlayed;
        }

        /// <summary>How many Romanian letters are learned.</summary>
        public int LearnedRomanian { get; }

        /// <summary>How many English letters are learned.</summary>
        public int LearnedEnglish { get; }

        /// <summary>Whether the profile has played on any day.</summary>
        public bool HasPlayed { get; }

        /// <summary>The last day played, when <see cref="HasPlayed"/>.</summary>
        public CalendarDay LastPlayed { get; }

        /// <summary>
        /// The summary of <paramref name="report"/> (its learned letters) and <paramref name="progress"/> (its days
        /// played). Called by <c>ParentRunner</c> for each profile's row and the tests.
        /// </summary>
        public static ProfileSummary Of(ParentReport report, LearnerProgress progress)
        {
            if (report == null)
            {
                throw new ArgumentNullException(nameof(report));
            }

            if (progress == null)
            {
                throw new ArgumentNullException(nameof(progress));
            }

            bool played = progress.DaysPlayed.Count > 0;
            return new ProfileSummary(LearnedIn(report, Language.Romanian), LearnedIn(report, Language.English), played,
                played ? progress.DaysPlayed.Max() : default);
        }

        private static int LearnedIn(ParentReport report, Language language)
        {
            return report.Rows
                .Where(row => row.Language == language)
                .SelectMany(row => row.Cells)
                .Count(cell => cell.State == ItemState.Learned);
        }
    }
}
