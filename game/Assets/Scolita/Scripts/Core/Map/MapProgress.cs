using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Lessons;

namespace Scolita.Core.Map
{
    /// <summary>
    /// Where she is on the galaxy map, part of her <c>LearnerProgress</c> (and all that 0.10.0's
    /// <c>map-progress</c> save held, read once to migrate it): the lessons she has finished (ids the content no longer knows are kept, so a later version can still use
    /// them) and the stop where the map last showed the rocket (to know when to play the trip).
    /// </summary>
    public sealed class MapProgress : IDoneLessons
    {
        private readonly List<ContentId> doneInOrder = new List<ContentId>();
        private readonly HashSet<ContentId> done = new HashSet<ContentId>();

        /// <summary>
        /// Starts from <paramref name="doneLessons"/> (duplicates merged, unset ids left out) and the
        /// stop <paramref name="rocketStop"/> (not set when the map was never shown). Called by
        /// <c>MapProgressData</c>, the <c>progress</c> file's reader, <c>ProgressRebuild</c> and the tests.
        /// </summary>
        public MapProgress(IEnumerable<ContentId> doneLessons, ContentId rocketStop)
        {
            foreach (ContentId lessonId in doneLessons)
            {
                Add(lessonId);
            }

            RocketStop = rocketStop;
        }

        /// <summary>
        /// Her progress from before the map (0.9.0 and earlier), when Go opened the lessons one after another in
        /// teaching order: every lesson <paramref name="order"/> lets her play before <paramref name="savedLesson"/>
        /// (the lesson saved as in progress, or as the next one) is done, and the map was never shown. Nothing is
        /// done when no lesson is saved or the one saved can't be played. Called by the Unity side's progress file
        /// when a profile has no <c>progress</c> file yet (or a damaged one), and by the tests.
        /// </summary>
        public static MapProgress Before(LessonOrder order, ContentId savedLesson)
        {
            if (!savedLesson.IsSet || !order.TryFind(savedLesson, out Lesson _))
            {
                return new MapProgress(new ContentId[0], default);
            }

            IEnumerable<ContentId> done = order.Playable
                .TakeWhile(lesson => lesson.Id != savedLesson)
                .Select(lesson => lesson.Id);
            return new MapProgress(done, default);
        }

        /// <summary>The finished lessons, in the order she first finished them.</summary>
        public IReadOnlyList<ContentId> DoneLessons => doneInOrder;

        /// <summary>The stop (a lesson id) where the map last showed the rocket; not set before the first time.</summary>
        public ContentId RocketStop { get; private set; }

        /// <inheritdoc/>
        public bool IsDone(ContentId lessonId)
        {
            return done.Contains(lessonId);
        }

        /// <summary>
        /// Records <paramref name="lessonId"/> as finished (once, however often she plays it). Called by
        /// <c>LessonProgress</c> when a lesson ends, and by the tests.
        /// </summary>
        public void MarkDone(ContentId lessonId)
        {
            Add(lessonId);
        }

        /// <summary>
        /// Records that the map now shows the rocket at <paramref name="lessonId"/>. Called by
        /// <c>MapRunner</c> once the rocket has arrived (after the trip, or at once), and by the tests.
        /// </summary>
        public void ShowRocketAt(ContentId lessonId)
        {
            RocketStop = lessonId;
        }

        private void Add(ContentId lessonId)
        {
            if (lessonId.IsSet && done.Add(lessonId))
            {
                doneInOrder.Add(lessonId);
            }
        }
    }
}
