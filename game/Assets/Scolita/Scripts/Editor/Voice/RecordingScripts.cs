using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Scolita.Core.Content;
using Scolita.Editor.ContentImport;

namespace Scolita.Editor.Voice
{
    /// <summary>
    /// Make recording scripts (brief item 8): writes each planned session's printable page and its
    /// manifest into <c>local/voice-inbox/scripts/</c>, replacing the scripts made before.
    /// </summary>
    public static class RecordingScripts
    {
        /// <summary>
        /// Plans the sessions of the content in <paramref name="content"/> and writes them into
        /// <paramref name="inbox"/>; returns them, or none (with the reason in <paramref name="problems"/>)
        /// when the content has errors. Called by <see cref="RecordingKitMenu"/> and tests.
        /// </summary>
        public static IReadOnlyList<RecordingSession> Make(ContentFolder content, VoiceInbox inbox, ContentProblems problems)
        {
            Curriculum curriculum = ContentTables.Read(content.ReadTexts(problems), problems);
            if (problems.HasErrors)
            {
                return new RecordingSession[0];
            }

            IReadOnlyList<RecordingSession> sessions = RecordingSessionPlanner.Plan(curriculum, content);
            Directory.CreateDirectory(inbox.Scripts);
            foreach (string old in Directory.GetFiles(inbox.Scripts).Where(IsScriptFile))
            {
                File.Delete(old);
            }

            UTF8Encoding utf8 = new UTF8Encoding(false);
            foreach (RecordingSession session in sessions)
            {
                File.WriteAllText(inbox.ScriptPage(session.Id), RecordingScriptPage.Render(session), utf8);
                File.WriteAllText(inbox.Manifest(session.Id), session.ToJson(), utf8);
            }

            return sessions;
        }

        private static bool IsScriptFile(string path)
        {
            string extension = Path.GetExtension(path);
            return extension == ".html" || extension == ".json";
        }
    }
}
