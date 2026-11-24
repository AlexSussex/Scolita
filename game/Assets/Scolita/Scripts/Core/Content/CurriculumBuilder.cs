using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Languages;

namespace Scolita.Core.Content
{
    /// <summary>
    /// Builds the <see cref="Curriculum"/> from items and entries read from the CSVs
    /// (<see cref="ContentTables"/>) or from the content assets, the same way for both. It keeps
    /// going past a problem (reported, and the broken item left out), so one run reports them all.
    /// </summary>
    public sealed class CurriculumBuilder
    {
        private readonly List<Letter> letters = new List<Letter>();
        private readonly List<Word> words = new List<Word>();
        private readonly List<Picture> pictures = new List<Picture>();
        private readonly List<VoiceLine> voiceLines = new List<VoiceLine>();
        private readonly List<UnitEntry> units = new List<UnitEntry>();
        private readonly List<LessonEntry> lessons = new List<LessonEntry>();
        private readonly List<StepEntry> steps = new List<StepEntry>();

        /// <summary>Adds a letter; called by <see cref="ContentTables"/> and the content assets.</summary>
        public void Add(Letter letter)
        {
            letters.Add(letter);
        }

        /// <summary>Adds a word; called by <see cref="ContentTables"/> and the content assets.</summary>
        public void Add(Word word)
        {
            words.Add(word);
        }

        /// <summary>Adds a picture; called by <see cref="ContentTables"/> and the content assets.</summary>
        public void Add(Picture picture)
        {
            pictures.Add(picture);
        }

        /// <summary>Adds a voice line; called by <see cref="ContentTables"/> and the content assets.</summary>
        public void Add(VoiceLine voiceLine)
        {
            voiceLines.Add(voiceLine);
        }

        /// <summary>Adds a unit, in teaching order; called by <see cref="ContentTables"/> and the content assets.</summary>
        public void Add(UnitEntry unit)
        {
            units.Add(unit);
        }

        /// <summary>Adds a lesson, in teaching order; called by <see cref="ContentTables"/> and the content assets.</summary>
        public void Add(LessonEntry lesson)
        {
            lessons.Add(lesson);
        }

        /// <summary>Adds a step, in teaching order; called by <see cref="ContentTables"/> and the content assets.</summary>
        public void Add(StepEntry step)
        {
            steps.Add(step);
        }

        /// <summary>
        /// Builds the curriculum, reporting into <paramref name="problems"/> every repeated id, every
        /// lesson whose unit and every step whose lesson doesn't exist, and every step whose languages
        /// don't fit its lesson and kind. Called by <see cref="ContentTables.Read"/>, the content
        /// assets' loader and tests.
        /// </summary>
        public Curriculum Build(ContentProblems problems)
        {
            ItemCatalog<Letter> letterCatalog = new ItemCatalog<Letter>(Distinct(letters, "letter", problems));
            ItemCatalog<Word> wordCatalog = new ItemCatalog<Word>(Distinct(words, "word", problems));
            ItemCatalog<Picture> pictureCatalog = new ItemCatalog<Picture>(Distinct(pictures, "picture", problems));
            ItemCatalog<VoiceLine> lineCatalog = new ItemCatalog<VoiceLine>(Distinct(voiceLines, "voice line", problems));
            return new Curriculum(letterCatalog, wordCatalog, pictureCatalog, lineCatalog, BuildDomains(problems));
        }

        private IReadOnlyList<Domain> BuildDomains(ContentProblems problems)
        {
            UnitEntry[] distinctUnits = DistinctEntries(units, unit => unit.Id, unit => unit.Source, "unit", problems);
            HashSet<ContentId> unitIds = new HashSet<ContentId>(distinctUnits.Select(unit => unit.Id));
            LessonEntry[] distinctLessons = DistinctEntries(lessons, lesson => lesson.Id, lesson => lesson.Source,
                "lesson", problems);
            List<LessonEntry> placedLessons = new List<LessonEntry>();
            foreach (LessonEntry lesson in distinctLessons)
            {
                if (unitIds.Contains(lesson.UnitId))
                {
                    placedLessons.Add(lesson);
                }
                else
                {
                    problems.Error(lesson.Source, "the lesson " + lesson.Id + " names the unit '" + lesson.UnitId
                        + "', which is not in units.csv.");
                }
            }

            Dictionary<ContentId, List<Step>> stepsByLesson = BuildSteps(placedLessons, problems);
            List<Domain> domains = new List<Domain>();
            foreach (ContentId domainId in distinctUnits.Select(unit => unit.DomainId).Distinct())
            {
                Unit[] domainUnits = distinctUnits
                    .Where(unit => unit.DomainId == domainId)
                    .Select(unit => BuildUnit(unit, placedLessons, stepsByLesson))
                    .ToArray();
                domains.Add(new Domain(domainId, domainUnits));
            }

            return domains;
        }

        private static Unit BuildUnit(UnitEntry unit, IEnumerable<LessonEntry> placedLessons,
            IReadOnlyDictionary<ContentId, List<Step>> stepsByLesson)
        {
            Lesson[] unitLessons = placedLessons
                .Where(lesson => lesson.UnitId == unit.Id)
                .Select(lesson => new Lesson(lesson.Id, lesson.UnitId, lesson.Languages, lesson.Introduces,
                    stepsByLesson[lesson.Id], lesson.Retired, lesson.Source))
                .ToArray();
            return new Unit(unit.Id, unit.DomainId, unitLessons, unit.Retired, unit.Source);
        }

        private Dictionary<ContentId, List<Step>> BuildSteps(IReadOnlyList<LessonEntry> placedLessons,
            ContentProblems problems)
        {
            Dictionary<ContentId, LessonEntry> lessonsById = placedLessons.ToDictionary(lesson => lesson.Id);
            Dictionary<ContentId, List<Step>> stepsByLesson = placedLessons.ToDictionary(lesson => lesson.Id,
                lesson => new List<Step>());
            foreach (StepEntry step in steps)
            {
                if (!lessonsById.TryGetValue(step.LessonId, out LessonEntry lesson))
                {
                    problems.Error(step.Source, "the step names the lesson '" + step.LessonId
                        + "', which is not in lessons.csv.");
                    continue;
                }

                if (TryResolveLanguages(step, lesson, problems, out LanguageMix languages))
                {
                    List<Step> lessonSteps = stepsByLesson[lesson.Id];
                    lessonSteps.Add(new Step(lesson.Id, lessonSteps.Count + 1, step.Kind, languages, step.Letters,
                        step.Words, step.Options, step.Source));
                }
            }

            return stepsByLesson;
        }

        private static bool TryResolveLanguages(StepEntry step, LessonEntry lesson, ContentProblems problems,
            out LanguageMix languages)
        {
            string kind = ContentCodes.CodeOf(step.Kind);
            if (ActivityKinds.Of(step.Kind).IsBridge)
            {
                languages = LanguageMix.Both;
                if (step.Languages.HasValue && step.Languages.Value != LanguageMix.Both)
                {
                    problems.Error(step.Source, "a " + kind + " step is in both languages: leave its language empty or write both.");
                    return false;
                }

                return true;
            }

            languages = step.Languages ?? lesson.Languages;
            if (languages == LanguageMix.Both)
            {
                problems.Error(step.Source, "a " + kind + " step is in one language: in the lesson " + lesson.Id
                    + ", which mixes both, write ro or en in its language column.");
                return false;
            }

            if (lesson.Languages != LanguageMix.Both && languages != lesson.Languages)
            {
                problems.Error(step.Source, "the step's language " + LanguageCodes.CodeOf(languages)
                    + " is not its lesson's (" + LanguageCodes.CodeOf(lesson.Languages) + "): leave it empty.");
                return false;
            }

            return true;
        }

        private static TItem[] Distinct<TItem>(IEnumerable<TItem> items, string what, ContentProblems problems)
            where TItem : IContentItem
        {
            return DistinctEntries(items, item => item.Id, item => item.Source, what, problems);
        }

        private static TEntry[] DistinctEntries<TEntry>(IEnumerable<TEntry> entries, Func<TEntry, ContentId> idOf,
            Func<TEntry, ContentSource> sourceOf, string what, ContentProblems problems)
        {
            Dictionary<ContentId, TEntry> seen = new Dictionary<ContentId, TEntry>();
            List<TEntry> distinct = new List<TEntry>();
            foreach (TEntry entry in entries)
            {
                ContentId id = idOf(entry);
                if (seen.TryGetValue(id, out TEntry first))
                {
                    problems.Error(sourceOf(entry), "the " + what + " id " + id + " is already used ("
                        + sourceOf(first) + "): an id is used once and never reused.");
                    continue;
                }

                seen.Add(id, entry);
                distinct.Add(entry);
            }

            return distinct.ToArray();
        }
    }
}
