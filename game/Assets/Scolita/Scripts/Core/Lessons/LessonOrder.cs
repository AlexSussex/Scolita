using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Languages;

namespace Scolita.Core.Lessons
{
    /// <summary>
    /// Which lesson Go opens (the human, 2026-11-24, until <c>galaxy-map</c> and
    /// <c>progress-review</c> replace it with the map's stops and her progress): a domain's lessons
    /// in teaching order, without retired ones, those outside her language mix and those with
    /// nothing to play yet; after a lesson comes the next of these, and after the last the first.
    /// </summary>
    public sealed class LessonOrder
    {
        private readonly IReadOnlyList<Lesson> all;
        private readonly HashSet<ContentId> playableIds;

        /// <summary>
        /// Orders <paramref name="domain"/>'s lessons for a learner of <paramref name="mix"/>, playing
        /// only <paramref name="playable"/>'s steps. Called by the Lesson scene's composition root and the tests.
        /// </summary>
        public LessonOrder(Domain domain, LanguageMix mix, PlayableSteps playable)
        {
            Mix = mix;
            PlayableSteps = playable;
            all = domain.Units
                .Where(unit => !unit.Retired)
                .SelectMany(unit => unit.Lessons)
                .Where(lesson => !lesson.Retired)
                .ToArray();
            Playable = all
                .Where(lesson => LessonPlan.Of(lesson, mix, playable, new Step[0], 1).Steps.Count > 0)
                .ToArray();
            playableIds = new HashSet<ContentId>(Playable.Select(lesson => lesson.Id));
        }

        /// <summary>The learner's language mix.</summary>
        public LanguageMix Mix { get; }

        /// <summary>The steps that can be played (rules, a view and the content they need).</summary>
        public PlayableSteps PlayableSteps { get; }

        /// <summary>The lessons Go can open, in teaching order.</summary>
        public IReadOnlyList<Lesson> Playable { get; }

        /// <summary>
        /// Finds the playable lesson <paramref name="id"/>; false when there is none (unknown, retired,
        /// outside the mix, or nothing to play yet). Called by the Lesson scene's composition root.
        /// </summary>
        public bool TryFind(ContentId id, out Lesson lesson)
        {
            lesson = playableIds.Contains(id) ? Playable.First(candidate => candidate.Id == id) : null;
            return lesson != null;
        }

        /// <summary>
        /// The playable lesson that comes after <paramref name="id"/> in teaching order, the first after
        /// the last, and the first for an id that isn't in the domain; null when nothing can be played.
        /// Called by the Lesson scene's composition root (Go, and the end of a lesson) and the tests.
        /// </summary>
        public Lesson After(ContentId id)
        {
            if (Playable.Count == 0)
            {
                return null;
            }

            int position = IndexOf(id);
            for (int later = position + 1; later < all.Count; later++)
            {
                if (playableIds.Contains(all[later].Id))
                {
                    return all[later];
                }
            }

            return Playable[0];
        }

        private int IndexOf(ContentId id)
        {
            for (int index = 0; index < all.Count; index++)
            {
                if (all[index].Id == id)
                {
                    return index;
                }
            }

            return all.Count;
        }
    }
}
