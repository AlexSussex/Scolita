using System.Collections.Generic;
using Scolita.Core.Content;

namespace Scolita.Core.Lessons
{
    /// <summary>
    /// The one thing a lesson asks of her progress: the review steps (warm-ups) to play before its own steps.
    /// Answered by <c>Scolita.Core.Progress.WarmUps</c>; <see cref="LessonResume"/> asks it for the lesson it
    /// opens.
    /// </summary>
    public interface IWarmUps
    {
        /// <summary>The warm-up steps before <paramref name="lesson"/>, in order; none when nothing is due. Read by <see cref="LessonResume"/>.</summary>
        IReadOnlyList<Step> For(Lesson lesson);
    }
}
