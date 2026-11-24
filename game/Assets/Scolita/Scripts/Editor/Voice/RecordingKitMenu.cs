using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Editor.ContentImport;
using UnityEditor;
using UnityEngine;

namespace Scolita.Editor.Voice
{
    /// <summary>
    /// The recording kit's menu items (Școlița → Make recording scripts, Import recording session,
    /// Accept recording session, Check voice clips) and their batch-mode forms
    /// (<c>-executeMethod Scolita.Editor.Voice.RecordingKitMenu.&lt;Name&gt;FromCommandLine</c>, with
    /// <c>-voiceSession &lt;id&gt;</c> and, optionally, <c>-voiceInbox &lt;folder&gt;</c>), which exit
    /// with 1 on any problem. Every problem is said plainly, never a crash.
    /// </summary>
    public static class RecordingKitMenu
    {
        private const string Menu = "\u0218coli\u021Ba/";
        private const string Title = "\u0218coli\u021Ba";
        private const string SessionArgument = "-voiceSession";
        private const string InboxArgument = "-voiceInbox";

        /// <summary>Writes the recording scripts and opens their folder; Școlița → Make recording scripts.</summary>
        [MenuItem(Menu + "Make recording scripts")]
        public static void MakeScriptsFromMenu()
        {
            string outcome = Guarded(() => MakeScripts(VoiceInbox.OfRepository()), out bool _);
            EditorUtility.DisplayDialog(Title, outcome, "OK");
            EditorUtility.RevealInFinder(VoiceInbox.OfRepository().Scripts);
        }

        /// <summary>Imports a session's take, chosen by its folder; Școlița → Import recording session.</summary>
        [MenuItem(Menu + "Import recording session")]
        public static void ImportSessionFromMenu()
        {
            VoiceInbox inbox = VoiceInbox.OfRepository();
            string session = ChooseSession(inbox, "Choose the session's folder (with its take)");
            if (session == null)
            {
                return;
            }

            string outcome = Guarded(() => ImportSession(inbox, session), out bool good);
            EditorUtility.DisplayDialog(Title, outcome, "OK");
            if (good)
            {
                Application.OpenURL(new Uri(inbox.Report(session)).AbsoluteUri);
            }
        }

        /// <summary>Accepts an imported session's good clips into the content; Școlița → Accept recording session.</summary>
        [MenuItem(Menu + "Accept recording session")]
        public static void AcceptSessionFromMenu()
        {
            VoiceInbox inbox = VoiceInbox.OfRepository();
            string session = ChooseSession(inbox, "Choose the session to accept (it was imported and is good)");
            if (session == null)
            {
                return;
            }

            if (!EditorUtility.DisplayDialog(Title, "Put the good clips of " + session + " into the app's content?", "Accept", "Cancel"))
            {
                return;
            }

            EditorUtility.DisplayDialog(Title, Guarded(() => AcceptSession(inbox, session), out bool _), "OK");
        }

        /// <summary>Checks every clip in content/voice/, logging each one; Școlița → Check voice clips.</summary>
        [MenuItem(Menu + "Check voice clips")]
        public static void CheckClipsFromMenu()
        {
            EditorUtility.DisplayDialog(Title, Guarded(CheckClips, out bool _), "OK");
        }

        /// <summary>Batch-mode Make recording scripts; exits with 1 on a problem.</summary>
        public static void MakeScriptsFromCommandLine()
        {
            Exit(() => MakeScripts(InboxFromArguments()));
        }

        /// <summary>Batch-mode Import recording session (<c>-voiceSession &lt;id&gt;</c>); exits with 1 on a problem.</summary>
        public static void ImportSessionFromCommandLine()
        {
            Exit(() => ImportSession(InboxFromArguments(), Argument(SessionArgument)));
        }

        /// <summary>Batch-mode Accept recording session (<c>-voiceSession &lt;id&gt;</c>); exits with 1 on a problem.</summary>
        public static void AcceptSessionFromCommandLine()
        {
            Exit(() => AcceptSession(InboxFromArguments(), Argument(SessionArgument)));
        }

        /// <summary>Batch-mode Check voice clips; exits with 1 on a problem or a flagged clip.</summary>
        public static void CheckClipsFromCommandLine()
        {
            Exit(CheckClips);
        }

        private static string MakeScripts(VoiceInbox inbox)
        {
            ContentProblems problems = new ContentProblems();
            IReadOnlyList<RecordingSession> sessions = RecordingScripts.Make(ContentImporter.RepositoryContent(), inbox, problems);
            if (problems.HasErrors)
            {
                throw new InvalidOperationException("The content has errors, so no script was made: " + string.Join(" ", problems.All));
            }

            int items = sessions.Sum(session => session.Items.Count - 1);
            return sessions.Count + " sessions, " + items + " lines to record, in " + inbox.Scripts + ": "
                + string.Join(", ", sessions.Select(session => session.Id)) + ".";
        }

        private static string ImportSession(VoiceInbox inbox, string session)
        {
            Ffmpeg.TryFind(out Ffmpeg ffmpeg);
            RecordingResult result = RecordingSessionImport.Import(inbox, session, ffmpeg);
            int lines = result.Items.Count(item => !string.IsNullOrEmpty(item.Line));
            int good = result.Items.Count(item => item.IsGood);
            string outcome = result.Summary + " " + good + " of " + lines + " clips are good; the report is "
                + inbox.Report(session) + ".";
            foreach (RecordingResultItem item in result.Items.Where(item => !item.IsGood && !string.IsNullOrEmpty(item.Line)))
            {
                Debug.LogWarning("Voice session " + session + ", " + item.Number + " (" + item.Line + "): "
                    + string.Join(", ", item.Flags));
            }

            return outcome;
        }

        private static string AcceptSession(VoiceInbox inbox, string session)
        {
            if (!Ffmpeg.TryFind(out Ffmpeg ffmpeg))
            {
                throw new InvalidOperationException(Ffmpeg.Missing);
            }

            ContentProblems problems = new ContentProblems();
            ContentFolder content = ContentImporter.RepositoryContent();
            int accepted = RecordingSessionAccept.Accept(inbox, session, ffmpeg, content, problems);
            ContentImportPlan plan = ContentImporter.Import(content, new ContentAssetPaths(ContentImporter.AssetFolder));
            IEnumerable<ContentProblem> errors = problems.All.Concat(plan.Problems).Where(problem => problem.IsError);
            if (errors.Any())
            {
                throw new InvalidOperationException(accepted + " clips were written, but the content import failed: "
                    + string.Join(" ", errors));
            }

            return accepted + " clips of " + session + " are in content/voice/ and imported. Commit content/voice/, "
                + "content/voice-lines.csv and Assets/Scolita/Content/Voice/ together.";
        }

        private static string CheckClips()
        {
            if (!Ffmpeg.TryFind(out Ffmpeg ffmpeg))
            {
                throw new InvalidOperationException(Ffmpeg.Missing);
            }

            string work = Path.Combine(Path.GetTempPath(), "scolita-voice-check");
            IReadOnlyList<KeyValuePair<string, ClipMeasurement>> checks =
                VoiceClipCheck.Check(ContentImporter.RepositoryContent(), ffmpeg, work);
            int flagged = 0;
            foreach (KeyValuePair<string, ClipMeasurement> check in checks)
            {
                string line = "Voice clip " + check.Key + ": " + check.Value;
                if (check.Value.Flags.Count == 0)
                {
                    Debug.Log(line + ", good.");
                    continue;
                }

                flagged++;
                Debug.LogWarning(line + ", " + string.Join(", ", check.Value.Flags) + ".");
            }

            string outcome = checks.Count + " clips checked, " + flagged + " flagged (the Console lists them).";
            if (flagged > 0)
            {
                throw new InvalidOperationException(outcome);
            }

            return outcome;
        }

        private static string ChooseSession(VoiceInbox inbox, string title)
        {
            Directory.CreateDirectory(inbox.Root);
            string folder = EditorUtility.OpenFolderPanel(title, inbox.Root, string.Empty);
            return string.IsNullOrEmpty(folder) ? null : Path.GetFileName(folder.TrimEnd('/', '\\'));
        }

        private static string Guarded(Func<string> action, out bool good)
        {
            try
            {
                string outcome = action();
                Debug.Log("Voice kit: " + outcome);
                good = true;
                return outcome;
            }
            catch (Exception exception) when (exception is IOException || exception is InvalidOperationException
                || exception is ArgumentException || exception is InvalidDataException)
            {
                Debug.LogError("Voice kit: " + exception.Message);
                good = false;
                return exception.Message;
            }
        }

        private static void Exit(Func<string> action)
        {
            Guarded(action, out bool good);
            EditorApplication.Exit(good ? 0 : 1);
        }

        private static VoiceInbox InboxFromArguments()
        {
            string inbox = Argument(InboxArgument, false);
            return inbox == null ? VoiceInbox.OfRepository() : new VoiceInbox(inbox);
        }

        private static string Argument(string name, bool required = true)
        {
            string[] arguments = Environment.GetCommandLineArgs();
            int index = Array.IndexOf(arguments, name);
            if (index >= 0 && index + 1 < arguments.Length)
            {
                return arguments[index + 1];
            }

            if (required)
            {
                throw new ArgumentException("Give " + name + " <value> on the command line.");
            }

            return null;
        }
    }
}
