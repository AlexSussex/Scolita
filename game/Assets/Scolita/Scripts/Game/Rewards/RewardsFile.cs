using Scolita.Core.Lessons;
using Scolita.Core.Map;
using Scolita.Core.Rewards;
using Scolita.Game.Saving;
using UnityEngine;

namespace Scolita.Game.Rewards
{
    /// <summary>
    /// Reads and writes a profile's <c>rewards.json</c> (<see cref="RewardsData"/>) as Core's <see cref="RewardBook"/>,
    /// through the profile's <see cref="SaveStore"/> only.
    /// </summary>
    public static class RewardsFile
    {
        /// <summary>
        /// Her rewards from <paramref name="profileFiles"/>. With no file yet (a profile from before rewards), or a
        /// damaged one (set aside, dated, by the store), the lessons <paramref name="done"/> says she finished get their
        /// stars and her finished planets their trails (<see cref="RewardGiver.CatchUp"/>), saved at once, so her sky
        /// matches her map. A file that can't be read now is kept as it is, and she starts from an empty book that is
        /// never saved over it. Called by <c>RewardsSession</c> and the tests.
        /// </summary>
        public static RewardBook Load(SaveStore profileFiles, RewardGiver giver, IDoneLessons done, LessonOrder order)
        {
            SaveLoadResult<RewardsData> saved = profileFiles.Load(RewardsData.FileName, RewardsData.CreateDefaults());
            if (saved.Outcome != SaveLoadOutcome.Missing && saved.Outcome != SaveLoadOutcome.SetAside)
            {
                return saved.Data.ToBook();
            }

            RewardBook book = giver.CatchUp(done, order);
            WriteTo(profileFiles, book);
            return book;
        }

        /// <summary>
        /// Saves <paramref name="book"/> to <paramref name="profileFiles"/>; false, after a warning, when it could not be
        /// saved. Called by <c>RewardsSession</c> and the tests.
        /// </summary>
        public static bool Save(SaveStore profileFiles, RewardBook book)
        {
            return WriteTo(profileFiles, book);
        }

        private static bool WriteTo(SaveStore profileFiles, RewardBook book)
        {
            if (profileFiles.Save(RewardsData.FileName, new RewardsData(book)))
            {
                return true;
            }

            Debug.LogWarning("Rewards: her rewards could not be saved; they are kept while the app is open.");
            return false;
        }
    }
}
