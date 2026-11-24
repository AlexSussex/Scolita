using System.Collections.Generic;

namespace Scolita.Core.Content
{
    /// <summary>
    /// A unit of a domain (a planet of the galaxy map): its lessons in both languages, in teaching
    /// order. Built by <see cref="CurriculumBuilder"/> from a row of <c>units.csv</c>.
    /// </summary>
    public sealed class Unit
    {
        /// <summary>Creates a unit; called by <see cref="CurriculumBuilder"/>.</summary>
        public Unit(ContentId id, ContentId domainId, IReadOnlyList<Lesson> lessons, bool retired, ContentSource source)
        {
            Id = id;
            DomainId = domainId;
            Lessons = lessons;
            Retired = retired;
            Source = source;
        }

        /// <summary>Its stable id.</summary>
        public ContentId Id { get; }

        /// <summary>The domain it belongs to (<c>reading</c>).</summary>
        public ContentId DomainId { get; }

        /// <summary>Its lessons, in teaching order.</summary>
        public IReadOnlyList<Lesson> Lessons { get; }

        /// <summary>Whether it is retired (kept, never played).</summary>
        public bool Retired { get; }

        /// <summary>Where it was read from.</summary>
        public ContentSource Source { get; }
    }
}
