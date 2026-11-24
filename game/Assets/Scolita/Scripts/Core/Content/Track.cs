using System.Collections.Generic;
using Scolita.Core.Languages;

namespace Scolita.Core.Content
{
    /// <summary>
    /// A domain's track in one language: its lessons in that language and its lessons that mix both
    /// languages, in teaching order, skipping retired ones, and the letters they introduce, in the
    /// order she meets them. Not a second tree: a view of the domain, made by <see cref="Curriculum"/>.
    /// </summary>
    public sealed class Track
    {
        /// <summary>Creates a track; called by <see cref="Curriculum"/>.</summary>
        public Track(ContentId domainId, Language language, IReadOnlyList<Lesson> lessons,
            IReadOnlyList<ContentId> letters)
        {
            DomainId = domainId;
            Language = language;
            Lessons = lessons;
            Letters = letters;
        }

        /// <summary>The domain it is a track of.</summary>
        public ContentId DomainId { get; }

        /// <summary>Its language.</summary>
        public Language Language { get; }

        /// <summary>Its lessons (not retired), in teaching order.</summary>
        public IReadOnlyList<Lesson> Lessons { get; }

        /// <summary>The letters its lessons introduce, in the order she meets them.</summary>
        public IReadOnlyList<ContentId> Letters { get; }
    }
}
