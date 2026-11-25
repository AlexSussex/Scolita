using Scolita.Core.Content;

namespace Scolita.Core.Map
{
    /// <summary>
    /// The one thing the galaxy map asks of her progress: whether a lesson is done. Answered by her
    /// <c>LearnerProgress</c> (the <c>progress</c> save), and by <see cref="MapProgress"/> alone in the tests.
    /// </summary>
    public interface IDoneLessons
    {
        /// <summary>Whether she has finished the lesson <paramref name="lessonId"/> at least once. Read by <see cref="MapRoute"/>.</summary>
        bool IsDone(ContentId lessonId);
    }
}
