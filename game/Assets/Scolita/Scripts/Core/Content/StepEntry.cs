using System.Collections.Generic;
using Scolita.Core.Languages;

namespace Scolita.Core.Content
{
    /// <summary>
    /// A step as read (a row of <c>steps.csv</c> or a step in a lesson asset), before
    /// <see cref="CurriculumBuilder"/> numbers it and settles its languages.
    /// </summary>
    public sealed class StepEntry
    {
        /// <summary>Creates an entry; called by <see cref="ContentTables"/>, the content assets and tests.</summary>
        public StepEntry(ContentId lessonId, ActivityKind kind, LanguageMix? languages, IReadOnlyList<ContentId> letters,
            IReadOnlyList<ContentId> words, IReadOnlyList<StepOption> options, ContentSource source)
        {
            LessonId = lessonId;
            Kind = kind;
            Languages = languages;
            Letters = letters;
            Words = words;
            Options = options;
            Source = source;
        }

        /// <summary>Its lesson.</summary>
        public ContentId LessonId { get; }

        /// <summary>Its kind.</summary>
        public ActivityKind Kind { get; }

        /// <summary>The languages written for it; null when none is written (the lesson's, or both for a bridge kind).</summary>
        public LanguageMix? Languages { get; }

        /// <summary>Its letters, in order.</summary>
        public IReadOnlyList<ContentId> Letters { get; }

        /// <summary>Its words, in order.</summary>
        public IReadOnlyList<ContentId> Words { get; }

        /// <summary>Its options.</summary>
        public IReadOnlyList<StepOption> Options { get; }

        /// <summary>Where it was read from.</summary>
        public ContentSource Source { get; }
    }
}
