using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Editor.ContentImport;
using Scolita.Editor.Voice;

namespace Scolita.Tests.EditMode.Voice
{
    /// <summary>Tests <see cref="RecordingSessionPlanner"/> and <see cref="RecordingScriptPage"/> on the real content (<c>content/*.csv</c>).</summary>
    public sealed class RecordingSessionPlannerTests
    {
        /// <summary>
        /// Tests that <see cref="RecordingSessionPlanner.Plan"/> makes sessions of one language and one
        /// planet, at most 50 items after the level test, sounds before lines, with no slow word (said
        /// from its sounds) and no line with a spliced name (the system voice's).
        /// </summary>
        [Test]
        public void TestPlan_TheRealContent()
        {
            ContentFolder folder = ContentImporter.RepositoryContent();
            ContentProblems problems = new ContentProblems();
            Curriculum curriculum = ContentTables.Read(folder.ReadTexts(problems), problems);

            IReadOnlyList<RecordingSession> sessions = RecordingSessionPlanner.Plan(curriculum, folder);
            RecordingItem[] items = sessions.SelectMany(session => session.Items.Skip(1)).ToArray();

            Assert.That(sessions.First().Id, Is.EqualTo("ro-planet-1-1"));
            Assert.That(sessions.All(session => session.Items[0].IsLevelTest && session.Items.Count <= RecordingSessionPlanner.MostItems + 1), Is.True);
            Assert.That(sessions.Select(session => session.Id), Is.Unique);
            Assert.That(items.Select(item => item.Clip), Is.Unique);
            Assert.That(items.Any(item => item.Kind == "slow"), Is.False);
            Assert.That(items.Any(item => item.Line.EndsWith("hello-name", System.StringComparison.Ordinal)), Is.False);
            Assert.That(items.First().Line, Is.EqualTo("ro-sound-a"));
            Assert.That(items.Where(item => item.Line == "ro-line-word-join").Select(item => item.Clip),
                Is.EqualTo(new[] { "ro-line-word-join.1.ogg", "ro-line-word-join.2.ogg", "ro-line-word-join.3.ogg" }));
            Assert.That(sessions.All(session => session.Items.Skip(1).All(item => item.Line.StartsWith(session.Language + "-", System.StringComparison.Ordinal))), Is.True);
        }

        /// <summary>Tests that a session's manifest reads back the same, and that its page lists every item with the level test first.</summary>
        [Test]
        public void TestPlan_ManifestAndPage()
        {
            ContentFolder folder = ContentImporter.RepositoryContent();
            ContentProblems problems = new ContentProblems();
            Curriculum curriculum = ContentTables.Read(folder.ReadTexts(problems), problems);
            RecordingSession session = RecordingSessionPlanner.Plan(curriculum, folder).First();

            RecordingSession read = RecordingSession.FromJson(session.ToJson());
            string page = RecordingScriptPage.Render(session);

            Assert.That(read.Items.Select(item => item.Clip), Is.EqualTo(session.Items.Select(item => item.Clip)));
            Assert.That(read.Items.Select(item => item.ExpectedSeconds), Is.EqualTo(session.Items.Select(item => item.ExpectedSeconds)));
            Assert.That(page, Does.Contain("Unu, doi, trei, patru, cinci.").And.Contain("Voice Recorder"));
            Assert.That(page.IndexOf("Unu, doi", System.StringComparison.Ordinal),
                Is.LessThan(page.IndexOf("/a/", System.StringComparison.Ordinal)));
        }
    }
}
