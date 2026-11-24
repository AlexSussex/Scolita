namespace Scolita.Core.Content
{
    /// <summary>
    /// A unit as read (a row of <c>units.csv</c> or a unit asset), before
    /// <see cref="CurriculumBuilder"/> gives it its lessons.
    /// </summary>
    public sealed class UnitEntry
    {
        /// <summary>Creates an entry; called by <see cref="ContentTables"/>, the content assets and tests.</summary>
        public UnitEntry(ContentId id, ContentId domainId, bool retired, ContentSource source)
        {
            Id = id;
            DomainId = domainId;
            Retired = retired;
            Source = source;
        }

        /// <summary>Its stable id.</summary>
        public ContentId Id { get; }

        /// <summary>Its domain.</summary>
        public ContentId DomainId { get; }

        /// <summary>Whether it is retired.</summary>
        public bool Retired { get; }

        /// <summary>Where it was read from.</summary>
        public ContentSource Source { get; }
    }
}
