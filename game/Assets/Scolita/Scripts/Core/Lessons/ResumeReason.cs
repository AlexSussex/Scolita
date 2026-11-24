namespace Scolita.Core.Lessons
{
    /// <summary>Why <see cref="LessonResume"/> chose what it chose (the Lesson scene logs the unusual ones).</summary>
    public enum ResumeReason
    {
        /// <summary>Nothing saved: the first lesson, from its start.</summary>
        FirstLesson,

        /// <summary>The saved lesson, from its start (it was saved at its first step).</summary>
        FromTheStart,

        /// <summary>The saved lesson, at the step she left.</summary>
        Resumed,

        /// <summary>The saved lesson can't be played (unknown, retired, outside her languages, nothing built): the next one.</summary>
        NextLesson,

        /// <summary>The saved lesson's steps changed since: it starts again.</summary>
        StepsChanged,

        /// <summary>The saved step is past the lesson's last step to play: it starts again.</summary>
        PastTheEnd
    }
}
