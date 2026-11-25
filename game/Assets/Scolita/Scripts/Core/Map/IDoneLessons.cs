using Scolita.Core.Content;

namespace Scolita.Core.Map
{
    /// <summary>
    /// The one thing the galaxy map asks of her progress: whether a lesson is done. Answered by
    /// <see cref="MapProgress"/> (the <c>map-progress</c> save) until <c>progress-review</c> puts its
    /// progress behind the same interface.
    /// </summary>
    public interface IDoneLessons
    {
        /// <summary>Whether she has finished the lesson <paramref name="lessonId"/> at least once. Read by <see cref="MapRoute"/>.</summary>
        bool IsDone(ContentId lessonId);
    }
}
