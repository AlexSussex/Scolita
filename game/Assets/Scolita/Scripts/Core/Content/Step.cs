using System;
using System.Collections.Generic;
using Scolita.Core.Languages;

namespace Scolita.Core.Content
{
    /// <summary>
    /// One step of a lesson: an activity kind, the languages it is in, the letters and words it
    /// uses (in their order: each kind's brief says what the order means) and its options. Built by
    /// <see cref="CurriculumBuilder"/> from one row of <c>steps.csv</c>.
    /// </summary>
    public sealed class Step
    {
        /// <summary>Creates a step; called by <see cref="CurriculumBuilder"/> and tests.</summary>
        public Step(ContentId lessonId, int number, ActivityKind kind, LanguageMix languages,
            IReadOnlyList<ContentId> letters, IReadOnlyList<ContentId> words, IReadOnlyList<StepOption> options,
            ContentSource source)
        {
            LessonId = lessonId;
            Number = number;
            Kind = kind;
            Languages = languages;
            Letters = letters ?? new ContentId[0];
            Words = words ?? new ContentId[0];
            Options = options ?? new StepOption[0];
            Source = source;
        }

        /// <summary>The lesson it belongs to.</summary>
        public ContentId LessonId { get; }

        /// <summary>Its place in its lesson, counted from 1.</summary>
        public int Number { get; }

        /// <summary>Its activity kind.</summary>
        public ActivityKind Kind { get; }

        /// <summary>
        /// Its languages: one, or both for a bridge kind (<see cref="ActivityKindRequirements.IsBridge"/>).
        /// A profile whose language mix doesn't hold them all skips the step.
        /// </summary>
        public LanguageMix Languages { get; }

        /// <summary>The letters it uses, in order.</summary>
        public IReadOnlyList<ContentId> Letters { get; }

        /// <summary>The words it uses, in order.</summary>
        public IReadOnlyList<ContentId> Words { get; }

        /// <summary>Its options, in the order written.</summary>
        public IReadOnlyList<StepOption> Options { get; }

        /// <summary>Where it was read from.</summary>
        public ContentSource Source { get; }

        /// <summary>
        /// The value of its option <paramref name="key"/>; false when it has none. Called by the
        /// content rules and, later, the activities' rules.
        /// </summary>
        public bool TryGetOption(string key, out string value)
        {
            foreach (StepOption option in Options)
            {
                if (string.Equals(option.Key, key, StringComparison.Ordinal))
                {
                    value = option.Value;
                    return true;
                }
            }

            value = string.Empty;
            return false;
        }
    }
}
