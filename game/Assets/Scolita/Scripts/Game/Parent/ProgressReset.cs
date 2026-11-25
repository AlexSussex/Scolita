using System.Collections.Generic;
using Scolita.Game.Lessons;
using Scolita.Game.Map;
using Scolita.Game.Progress;
using Scolita.Game.Rewards;
using Scolita.Game.Saving;

namespace Scolita.Game.Parent
{
    /// <summary>
    /// The parent area's "Șterge progresul": deletes a profile's progress files together (her letters and lessons,
    /// the map's older file, the lesson in progress, and her stars, stickers and trails; with one of them left, the
    /// progress would be made again from it, and the rewards from her lessons done). The learning settings, the play time and every damaged copy stay.
    /// </summary>
    public static class ProgressReset
    {
        /// <summary>The files deleted, in the order they are deleted.</summary>
        public static IReadOnlyList<string> FileNames { get; } = new[]
        {
            ProgressData.FileName,
            MapProgressData.FileName,
            LessonRunData.FileName,
            RewardsData.FileName
        };

        /// <summary>
        /// Deletes <see cref="FileNames"/> in <paramref name="profileFiles"/>; true when every one is gone (each that
        /// could not be deleted is logged by the store). Called by <c>ParentRunner</c> after the second "yes", and the
        /// tests.
        /// </summary>
        public static bool Run(SaveStore profileFiles)
        {
            bool allGone = true;
            foreach (string name in FileNames)
            {
                allGone &= profileFiles.Delete(name);
            }

            return allGone;
        }
    }
}
