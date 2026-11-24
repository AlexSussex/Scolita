using System.Linq;
using Scolita.Core.Languages;

namespace Scolita.Core.Content.Rules
{
    /// <summary>
    /// The rules on units and lessons: no empty unit or lesson, no lesson in use in a retired unit,
    /// and the one new letter a lesson introduces is in its language (a lesson mixing both languages
    /// introduces none). Part of <see cref="ContentRules"/>.
    /// </summary>
    public static class LessonRules
    {
        /// <summary>Checks every unit and lesson; called by <see cref="ContentRules.Check"/>.</summary>
        public static void Check(Curriculum curriculum, ContentProblems problems)
        {
            foreach (Unit unit in curriculum.Domains.SelectMany(domain => domain.Units))
            {
                if (!unit.Retired && unit.Lessons.All(lesson => lesson.Retired))
                {
                    problems.Error(unit.Source, "the unit " + unit.Id + " has no lesson in lessons.csv.");
                }

                foreach (Lesson lesson in unit.Lessons.Where(lesson => !lesson.Retired))
                {
                    CheckLesson(curriculum, unit, lesson, problems);
                }
            }
        }

        private static void CheckLesson(Curriculum curriculum, Unit unit, Lesson lesson, ContentProblems problems)
        {
            if (unit.Retired)
            {
                problems.Error(lesson.Source, "the lesson's unit " + unit.Id
                    + " is retired: retire the lesson too, or move it to another unit.");
            }

            if (lesson.Steps.Count == 0)
            {
                problems.Error(lesson.Source, "the lesson " + lesson.Id + " has no step in steps.csv.");
            }

            if (!lesson.Introduces.IsSet)
            {
                return;
            }

            if (lesson.Languages == LanguageMix.Both)
            {
                problems.Error(lesson.Source, "a lesson in both languages introduces no letter: a new letter comes in its own ro or en lesson.");
                return;
            }

            Letter letter = References.InUse(curriculum.Letters, lesson.Introduces, lesson.Source, "letter",
                ContentFileNames.Letters, problems);
            if (letter != null && !letter.Languages.Includes(lesson.Languages.Languages()[0]))
            {
                problems.Error(lesson.Source, "the letter " + letter.Id + " is not in the "
                    + LanguageCodes.CodeOf(lesson.Languages) + " alphabet, so this lesson can't introduce it.");
            }
        }
    }
}
