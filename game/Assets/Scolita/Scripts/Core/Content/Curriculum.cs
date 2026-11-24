using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Languages;
using Scolita.Core.Tracing;

namespace Scolita.Core.Content
{
    /// <summary>
    /// All the content, built from the CSVs (by the importer and the validator) or from the content
    /// assets (in the app): the catalogs of letters, words, pictures, voice lines and the letters' shapes, the domains with
    /// their units, lessons and steps, each domain's track per language, and which letters are taught
    /// before a lesson. Made by <see cref="CurriculumBuilder"/>.
    /// </summary>
    public sealed class Curriculum
    {
        private readonly Dictionary<ContentId, Lesson> lessons = new Dictionary<ContentId, Lesson>();
        private readonly Dictionary<ContentId, Unit> units = new Dictionary<ContentId, Unit>();
        private readonly Dictionary<ContentId, int> lessonPositions = new Dictionary<ContentId, int>();
        private readonly Dictionary<ContentId, Track> romanianTracks = new Dictionary<ContentId, Track>();
        private readonly Dictionary<ContentId, Track> englishTracks = new Dictionary<ContentId, Track>();

        /// <summary>Creates the curriculum; called by <see cref="CurriculumBuilder"/>.</summary>
        public Curriculum(ItemCatalog<Letter> letters, ItemCatalog<Word> words, ItemCatalog<Picture> pictures,
            ItemCatalog<VoiceLine> voiceLines, ItemCatalog<LetterShape> shapes, IReadOnlyList<Domain> domains)
        {
            Shapes = shapes;
            Letters = letters;
            Words = words;
            Pictures = pictures;
            VoiceLines = voiceLines;
            Domains = domains;
            foreach (Domain domain in domains)
            {
                IndexDomain(domain);
                romanianTracks.Add(domain.Id, MakeTrack(domain, Language.Romanian));
                englishTracks.Add(domain.Id, MakeTrack(domain, Language.English));
            }
        }

        /// <summary>Every letter.</summary>
        public ItemCatalog<Letter> Letters { get; }

        /// <summary>Every word, in both languages.</summary>
        public ItemCatalog<Word> Words { get; }

        /// <summary>Every picture.</summary>
        public ItemCatalog<Picture> Pictures { get; }

        /// <summary>Every voice line, in both languages.</summary>
        public ItemCatalog<VoiceLine> VoiceLines { get; }

        /// <summary>Every letter's shapes (how it is written, per case and form).</summary>
        public ItemCatalog<LetterShape> Shapes { get; }

        /// <summary>The domains, in the order their first unit appears.</summary>
        public IReadOnlyList<Domain> Domains { get; }

        /// <summary>Every lesson of every domain, in teaching order.</summary>
        public IEnumerable<Lesson> AllLessons => Domains
            .SelectMany(domain => domain.Units)
            .SelectMany(unit => unit.Lessons);

        /// <summary>Finds the lesson <paramref name="id"/>; called by the rules, and later the lesson engine and the map.</summary>
        public bool TryGetLesson(ContentId id, out Lesson lesson)
        {
            return lessons.TryGetValue(id, out lesson);
        }

        /// <summary>
        /// The shape of <paramref name="letter"/> in <paramref name="letterCase"/> and <paramref name="form"/>;
        /// false when it has none (no stroke data yet, or not for that case and form). Called by the
        /// activities, the playable steps, the content rules and the tests.
        /// </summary>
        public bool TryGetShape(Letter letter, LetterCase letterCase, LetterForm form, out LetterShape shape)
        {
            if (!letter.Strokes.IsSet)
            {
                shape = null;
                return false;
            }

            return Shapes.TryGet(LetterShapeCodes.IdOf(letter.Strokes, letterCase, form), out shape);
        }

        /// <summary>Finds the unit <paramref name="id"/>; called by the rules, and later the map.</summary>
        public bool TryGetUnit(ContentId id, out Unit unit)
        {
            return units.TryGetValue(id, out unit);
        }

        /// <summary>
        /// The track of <paramref name="domainId"/> in <paramref name="language"/>; throws a
        /// <see cref="KeyNotFoundException"/> for a domain that doesn't exist. Called by
        /// <see cref="LettersTaughtBefore"/>, and later by the map and the progress.
        /// </summary>
        public Track Track(ContentId domainId, Language language)
        {
            return TrackIn(domainId, language);
        }

        /// <summary>
        /// The letters taught in <paramref name="language"/>'s track before <paramref name="lesson"/>
        /// (introduced by the track's earlier lessons, retired ones skipped), in the order she met
        /// them. The letter <paramref name="lesson"/> itself introduces is not among them. Throws an
        /// <see cref="ArgumentException"/> for a lesson that isn't in this curriculum. Called by the
        /// taught-letters rule and <c>CurriculumTests</c>.
        /// </summary>
        public IReadOnlyList<ContentId> LettersTaughtBefore(Lesson lesson, Language language)
        {
            if (!lessonPositions.TryGetValue(lesson.Id, out int position) || !units.TryGetValue(lesson.UnitId, out Unit unit))
            {
                throw new ArgumentException("The lesson " + lesson.Id + " is not in this curriculum.", nameof(lesson));
            }

            return TrackIn(unit.DomainId, language).Lessons
                .Where(earlier => lessonPositions[earlier.Id] < position)
                .Where(IntroducesInItsTrack)
                .Select(earlier => earlier.Introduces)
                .ToArray();
        }

        private Track TrackIn(ContentId domainId, Language language)
        {
            Dictionary<ContentId, Track> tracks = language == Language.Romanian ? romanianTracks : englishTracks;
            if (!tracks.TryGetValue(domainId, out Track track))
            {
                throw new KeyNotFoundException("No domain '" + domainId + "'.");
            }

            return track;
        }

        private static bool IntroducesInItsTrack(Lesson lesson)
        {
            return lesson.Introduces.IsSet && lesson.Languages != LanguageMix.Both;
        }

        private void IndexDomain(Domain domain)
        {
            foreach (Unit unit in domain.Units)
            {
                units.Add(unit.Id, unit);
                foreach (Lesson lesson in unit.Lessons)
                {
                    lessons.Add(lesson.Id, lesson);
                    lessonPositions.Add(lesson.Id, lessonPositions.Count);
                }
            }
        }

        private static Track MakeTrack(Domain domain, Language language)
        {
            Lesson[] trackLessons = domain.Units
                .Where(unit => !unit.Retired)
                .SelectMany(unit => unit.Lessons)
                .Where(lesson => !lesson.Retired)
                .Where(lesson => lesson.Languages.Includes(language))
                .ToArray();
            ContentId[] letters = trackLessons
                .Where(IntroducesInItsTrack)
                .Select(lesson => lesson.Introduces)
                .ToArray();
            return new Track(domain.Id, language, trackLessons, letters);
        }
    }
}
