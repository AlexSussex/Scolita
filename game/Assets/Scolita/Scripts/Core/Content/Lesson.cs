using System.Collections.Generic;
using Scolita.Core.Languages;

namespace Scolita.Core.Content
{
    /// <summary>
    /// A lesson: its unit, its language (<c>ro</c>, <c>en</c>, or both for a lesson that mixes them,
    /// such as a planet party), the one new letter it introduces, if any, and its steps in order.
    /// Built by <see cref="CurriculumBuilder"/> from a row of <c>lessons.csv</c> and its rows of
    /// <c>steps.csv</c>.
    /// </summary>
    public sealed class Lesson
    {
        /// <summary>Creates a lesson; called by <see cref="CurriculumBuilder"/>.</summary>
        public Lesson(ContentId id, ContentId unitId, LanguageMix languages, ContentId introduces,
            IReadOnlyList<Step> steps, bool retired, ContentSource source)
        {
            Id = id;
            UnitId = unitId;
            Languages = languages;
            Introduces = introduces;
            Steps = steps;
            Retired = retired;
            Source = source;
        }

        /// <summary>Its stable id.</summary>
        public ContentId Id { get; }

        /// <summary>The unit (planet) it belongs to.</summary>
        public ContentId UnitId { get; }

        /// <summary>Its language, or <see cref="LanguageMix.Both"/> for a lesson that mixes both.</summary>
        public LanguageMix Languages { get; }

        /// <summary>The new letter it introduces in its language's track; not set for none.</summary>
        public ContentId Introduces { get; }

        /// <summary>Its steps, in order.</summary>
        public IReadOnlyList<Step> Steps { get; }

        /// <summary>Whether it is retired (kept, never played).</summary>
        public bool Retired { get; }

        /// <summary>Where it was read from.</summary>
        public ContentSource Source { get; }
    }
}
