using Scolita.Core.Content;
using Scolita.Core.Lessons;
using Scolita.Core.Map;
using Scolita.Game.Lessons;
using Scolita.Game.Saving;
using UnityEngine;

namespace Scolita.Game.Map
{
    /// <summary>
    /// Reads and writes a profile's <c>map-progress.json</c> (<see cref="MapProgressData"/>) as Core's
    /// <see cref="MapProgress"/>, through the profile's <see cref="SaveStore"/> only.
    /// </summary>
    public static class MapProgressFile
    {
        /// <summary>
        /// Her map progress from <paramref name="profileFiles"/>. With no file yet (a profile from before the map),
        /// or a damaged one (set aside by the store), the lessons <paramref name="order"/> puts before the one in
        /// <c>lesson-run.json</c> are done (<see cref="MapProgress.Before"/>): that file names the next stop or one
        /// already done, so this never claims more than she did, and an update or a damaged file keeps where she
        /// is. Called by <c>MapRunner</c> and the tests.
        /// </summary>
        public static MapProgress Load(SaveStore profileFiles, LessonOrder order)
        {
            return ReadFrom(profileFiles, order);
        }

        /// <summary>
        /// Saves <paramref name="progress"/> to <paramref name="profileFiles"/>; false, after a warning, when it
        /// could not be saved. Called by <c>MapRunner</c> and the tests.
        /// </summary>
        public static bool Save(SaveStore profileFiles, MapProgress progress)
        {
            return WriteTo(profileFiles, progress);
        }

        /// <summary>
        /// Records the lesson <paramref name="lessonId"/> as finished in <paramref name="profileFiles"/>'s map
        /// progress (read as <see cref="Load"/> reads it, with <paramref name="order"/>). Called by
        /// <c>LessonRunner</c> when a lesson ends, and by the tests.
        /// </summary>
        public static bool RecordDone(SaveStore profileFiles, LessonOrder order, ContentId lessonId)
        {
            MapProgress progress = ReadFrom(profileFiles, order);
            progress.MarkDone(lessonId);
            return WriteTo(profileFiles, progress);
        }

        private static MapProgress ReadFrom(SaveStore profileFiles, LessonOrder order)
        {
            SaveLoadResult<MapProgressData> saved = profileFiles.Load(MapProgressData.FileName, MapProgressData.CreateDefaults());
            if (saved.Outcome != SaveLoadOutcome.Missing && saved.Outcome != SaveLoadOutcome.SetAside)
            {
                return saved.Data.ToProgress();
            }

            LessonRunData run = profileFiles.Load(LessonRunData.FileName, LessonRunData.CreateDefaults()).Data;
            run.TryGetLessonId(out ContentId lessonInProgress);
            return MapProgress.Before(order, lessonInProgress);
        }

        private static bool WriteTo(SaveStore profileFiles, MapProgress progress)
        {
            if (profileFiles.Save(MapProgressData.FileName, new MapProgressData(progress)))
            {
                return true;
            }

            Debug.LogWarning("Map: where she is on the map could not be saved; the map shows what was saved before.");
            return false;
        }
    }
}
