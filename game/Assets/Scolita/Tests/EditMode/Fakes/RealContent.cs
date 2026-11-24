using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Editor.ContentImport;

namespace Scolita.Tests.EditMode.Fakes
{
    /// <summary>
    /// The repository's real content (<c>content/*.csv</c>), so the activities' rules and the lesson
    /// run are tested with planet 1's real items (AGENTS.md → "C# tests"). Used by the activity and
    /// lesson tests.
    /// </summary>
    public static class RealContent
    {
        /// <summary>Reads the content, asserting it reads without an error; called by the activity and lesson tests.</summary>
        public static Curriculum Read()
        {
            ContentFolder folder = ContentImporter.RepositoryContent();
            ContentProblems problems = new ContentProblems();
            Curriculum curriculum = ContentTables.Read(folder.ReadTexts(problems), problems);
            Assert.That(problems.All.Where(problem => problem.IsError).Select(problem => problem.ToString()), Is.Empty);
            return curriculum;
        }

        /// <summary>The lesson <paramref name="id"/>, asserting it exists; called by the activity and lesson tests.</summary>
        public static Lesson Lesson(Curriculum curriculum, string id)
        {
            Assert.That(curriculum.TryGetLesson(ContentId.Create(id), out Lesson lesson), Is.True, "no lesson " + id);
            return lesson;
        }

        /// <summary>
        /// The first step of <paramref name="kind"/> in the lesson <paramref name="lessonId"/>, asserting
        /// it exists; called by the activity and lesson tests.
        /// </summary>
        public static Step FirstStep(Curriculum curriculum, string lessonId, ActivityKind kind)
        {
            Step step = Lesson(curriculum, lessonId).Steps.FirstOrDefault(candidate => candidate.Kind == kind);
            Assert.That(step, Is.Not.Null, "no " + kind + " step in " + lessonId);
            return step;
        }
    }
}
