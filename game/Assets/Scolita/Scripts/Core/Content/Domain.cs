using System.Collections.Generic;

namespace Scolita.Core.Content
{
    /// <summary>
    /// A domain of learning (<c>reading</c> now; numbers or music later): its units in order. It is
    /// named by its units' <c>domain</c> column, so a new domain is new rows, not code. Built by
    /// <see cref="CurriculumBuilder"/>.
    /// </summary>
    public sealed class Domain
    {
        /// <summary>Creates a domain; called by <see cref="CurriculumBuilder"/>.</summary>
        public Domain(ContentId id, IReadOnlyList<Unit> units)
        {
            Id = id;
            Units = units;
        }

        /// <summary>Its stable id.</summary>
        public ContentId Id { get; }

        /// <summary>Its units, in teaching order.</summary>
        public IReadOnlyList<Unit> Units { get; }
    }
}
