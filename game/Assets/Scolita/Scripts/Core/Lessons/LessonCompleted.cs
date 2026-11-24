using Scolita.Core.Content;

namespace Scolita.Core.Lessons
{
    /// <summary>Raised by <see cref="LessonRun.LessonCompleted"/> when every step is done. <c>rewards</c> listens.</summary>
    public readonly struct LessonCompleted
    {
        /// <summary>Creates the event; called by <see cref="LessonRun"/>.</summary>
        public LessonCompleted(Lesson lesson)
        {
            Lesson = lesson;
        }

        /// <summary>The lesson finished.</summary>
        public Lesson Lesson { get; }
    }
}
