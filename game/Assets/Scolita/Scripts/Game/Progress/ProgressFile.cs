using Scolita.Core.Content;
using Scolita.Core.Lessons;
using Scolita.Core.Map;
using Scolita.Core.Progress;
using Scolita.Game.Lessons;
using Scolita.Game.Map;
using Scolita.Game.Saving;
using UnityEngine;

namespace Scolita.Game.Progress
{
    /// <summary>
    /// Reads and writes a profile's <c>progress.json</c> (<see cref="ProgressData"/>) as Core's
    /// <see cref="LearnerProgress"/>, through the profile's <see cref="SaveStore"/> only, and makes it from the earlier
    /// saves when it is missing (an update from 0.10.0 or before) or damaged (<see cref="ProgressRebuild"/>).
    /// </summary>
    public static class ProgressFile
    {
        /// <summary>
        /// Her progress from <paramref name="profileFiles"/>. With no <c>progress.json</c> yet, or a damaged one (set aside
        /// dated by the store, with a warning), it is made from 0.10.0's <c>map-progress.json</c> (an update from 0.10.0:
        /// that file alone, which was current), else, and after damage, also from the lessons <paramref name="order"/>
        /// puts before the one in <c>lesson-run.json</c> (0.9.0 and earlier; the place after damage), the letters met found in
        /// <paramref name="curriculum"/>, and saved at once; a file kept as it is (a newer version's, or one that can't be
        /// opened) gives the same, unsaved. Called by <c>MapRunner</c>, <c>LessonRunner</c> and the tests.
        /// </summary>
        public static LearnerProgress Load(SaveStore profileFiles, Curriculum curriculum, LessonOrder order)
        {
            SaveLoadResult<ProgressData> saved = profileFiles.Load(ProgressData.FileName, ProgressData.CreateDefaults());
            if (saved.Outcome == SaveLoadOutcome.Loaded)
            {
                return saved.Data.ToProgress();
            }

            LearnerProgress rebuilt = Rebuild(profileFiles, curriculum, order, saved.Outcome == SaveLoadOutcome.SetAside);
            if (saved.Outcome == SaveLoadOutcome.SetAside)
            {
                Debug.LogWarning("Progress: progress.json was damaged and set aside; her place is made again from "
                    + "map-progress.json and lesson-run.json (" + rebuilt.Lessons.DoneLessons.Count + " lessons done).");
            }
            else if (saved.Outcome == SaveLoadOutcome.Missing)
            {
                Debug.Log("Progress: progress.json is made from the earlier saves (" + rebuilt.Lessons.DoneLessons.Count
                    + " lessons done).");
            }

            if (saved.Outcome != SaveLoadOutcome.Unreadable)
            {
                WriteTo(profileFiles, rebuilt);
            }

            return rebuilt;
        }

        /// <summary>
        /// Saves <paramref name="progress"/> to <paramref name="profileFiles"/>; false, after a warning, when it could not be
        /// saved. Called by <c>MapRunner</c>, <c>LessonRunner</c> and the tests.
        /// </summary>
        public static bool Save(SaveStore profileFiles, LearnerProgress progress)
        {
            return WriteTo(profileFiles, progress);
        }

        private static LearnerProgress Rebuild(SaveStore profileFiles, Curriculum curriculum, LessonOrder order, bool damaged)
        {
            SaveLoadResult<MapProgressData> map = profileFiles.Load(MapProgressData.FileName, MapProgressData.CreateDefaults());
            MapProgress mapProgress = map.Outcome == SaveLoadOutcome.Loaded ? map.Data.ToProgress() : null;
            if (mapProgress != null && !damaged)
            {
                return ProgressRebuild.From(mapProgress, new MapProgress(new ContentId[0], default), curriculum);
            }

            LessonRunData run = profileFiles.Load(LessonRunData.FileName, LessonRunData.CreateDefaults()).Data;
            run.TryGetLessonId(out ContentId lessonInProgress);
            return ProgressRebuild.From(mapProgress, MapProgress.Before(order, lessonInProgress), curriculum);
        }

        private static bool WriteTo(SaveStore profileFiles, LearnerProgress progress)
        {
            if (profileFiles.Save(ProgressData.FileName, new ProgressData(progress)))
            {
                return true;
            }

            Debug.LogWarning("Progress: her progress could not be saved; what was saved before is kept.");
            return false;
        }
    }
}
