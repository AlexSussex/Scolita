using Scolita.Core.Languages;

namespace Scolita.Core.Content
{
    /// <summary>
    /// A lesson as read (a row of <c>lessons.csv</c> or a lesson asset), before
    /// <see cref="CurriculumBuilder"/> gives it its steps.
    /// </summary>
    public sealed class LessonEntry
    {
        /// <summary>Creates an entry; called by <see cref="ContentTables"/>, the content assets and tests.</summary>
        public LessonEntry(ContentId id, ContentId unitId, LanguageMix languages, ContentId introduces, bool retired,
            ContentSource source)
        {
            Id = id;
            UnitId = unitId;
            Languages = languages;
            Introduces = introduces;
            Retired = retired;
            Source = source;
        }

        /// <summary>Its stable id.</summary>
        public ContentId Id { get; }

        /// <summary>Its unit.</summary>
        public ContentId UnitId { get; }

        /// <summary>Its language, or both.</summary>
        public LanguageMix Languages { get; }

        /// <summary>The letter it introduces; not set for none.</summary>
        public ContentId Introduces { get; }

        /// <summary>Whether it is retired.</summary>
        public bool Retired { get; }

        /// <summary>Where it was read from.</summary>
        public ContentSource Source { get; }
    }
}
