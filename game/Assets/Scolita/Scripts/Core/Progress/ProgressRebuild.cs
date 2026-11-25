using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Map;

namespace Scolita.Core.Progress
{
    /// <summary>
    /// Her progress made from the saves that came before the <c>progress</c> file (0.10.0's map progress, and
    /// the lesson in progress of 0.9.0 and earlier), for a profile with no <c>progress</c> file yet or a damaged
    /// one: the lessons either says are done (neither ever claims more than she did), the rocket's stop the map
    /// last showed, and the letters the done lessons introduced as met (no right answer is invented).
    /// </summary>
    public static class ProgressRebuild
    {
        /// <summary>
        /// Joins <paramref name="mapProgress"/> (0.10.0's <c>map-progress</c>; null when there is none) and
        /// <paramref name="beforeTheMap"/> (<see cref="MapProgress.Before"/> from the lesson in progress), the
        /// letters met found in <paramref name="curriculum"/>. Called by the Unity side's progress file and the tests.
        /// </summary>
        public static LearnerProgress From(MapProgress mapProgress, MapProgress beforeTheMap, Curriculum curriculum)
        {
            IEnumerable<ContentId> saved = mapProgress?.DoneLessons ?? Enumerable.Empty<ContentId>();
            ContentId[] done = saved.Concat(beforeTheMap.DoneLessons).Distinct().ToArray();
            ContentId rocketStop = mapProgress?.RocketStop ?? default;
            LearnerProgress progress = new LearnerProgress(new MapProgress(done, rocketStop), null, null);
            foreach (ContentId lessonId in done)
            {
                if (curriculum.TryGetLesson(lessonId, out Lesson lesson) && lesson.Introduces.IsSet
                    && lesson.Languages != LanguageMix.Both && curriculum.Letters.Contains(lesson.Introduces))
                {
                    Language language = lesson.Languages == LanguageMix.EnglishOnly ? Language.English : Language.Romanian;
                    progress.Meet(LearningItem.Letter(lesson.Introduces, language));
                }
            }

            return progress;
        }
    }
}
