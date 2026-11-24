using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Voice;
using Scolita.Editor.ContentImport;

namespace Scolita.Tests.EditMode.Core.Voice
{
    /// <summary>Tests <see cref="LessonVoiceLines"/> on the real content (<c>content/*.csv</c>).</summary>
    public sealed class LessonVoiceLinesTests
    {
        /// <summary>
        /// Tests that <see cref="LessonVoiceLines.Of"/> lists the Romanian m lesson's letters' sounds and
        /// names, its words' lines, its kinds' instructions, each once, and nothing in English.
        /// </summary>
        [Test]
        public void TestOf_TheRomanianMLesson()
        {
            ContentFolder folder = ContentImporter.RepositoryContent();
            ContentProblems problems = new ContentProblems();
            Curriculum curriculum = ContentTables.Read(folder.ReadCsvs(problems), problems);
            curriculum.TryGetLesson(ContentId.Create("ro-m"), out Lesson lesson);

            IReadOnlyList<VoiceLineId> lines = LessonVoiceLines.Of(curriculum, lesson);
            string[] ids = lines.Select(line => line.ToString()).ToArray();

            Assert.That(ids, Does.Contain("ro-sound-m").And.Contain("ro-name-m").And.Contain("ro-sound-t"));
            Assert.That(ids, Does.Contain("ro-word-mama").And.Contain("ro-slow-mama").And.Contain("ro-word-muzica"));
            Assert.That(ids, Does.Contain("ro-line-meet-new").And.Contain("ro-line-pick-letter"));
            Assert.That(ids, Is.Unique);
            Assert.That(ids.Where(id => !id.StartsWith("ro-", System.StringComparison.Ordinal)), Is.Empty);
            Assert.That(lines.All(line => curriculum.VoiceLines.Contains(line.Id)), Is.True);
        }
    }
}
