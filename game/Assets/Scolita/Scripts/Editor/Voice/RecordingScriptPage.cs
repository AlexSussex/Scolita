using System.Linq;
using System.Text;
using Scolita.Core.Content;

namespace Scolita.Editor.Voice
{
    /// <summary>
    /// A session's printable script: how to record it with the Windows Voice Recorder, how to say
    /// each kind of line in it (from <c>local/todo/04-record-voices.md</c>), and its numbered items,
    /// the level test first.
    /// </summary>
    public static class RecordingScriptPage
    {
        /// <summary>The page of <paramref name="session"/>; called by Make recording scripts and tests.</summary>
        public static string Render(RecordingSession session)
        {
            StringBuilder page = new StringBuilder();
            string language = session.Language == "ro" ? "Romanian" : "English";
            page.Append(Html.Head("\u0218coli\u021Ba recording session " + session.Id));
            page.Append("<h1>Recording session ").Append(Html.Text(session.Id)).Append("</h1>\n");
            page.Append("<p>").Append(language).Append(", ").Append(Html.Text(session.Place)).Append(", ")
                .Append(session.Items.Count - 1).Append(" lines after the level test.</p>\n");
            page.Append("<h2>How to record it</h2>\n<ol>\n")
                .Append("<li>Open <b>Voice Recorder</b> (Windows), in a quiet room, 20 to 30 cm from the microphone.</li>\n")
                .Append("<li>Press record, wait two seconds, then read the lines in order, starting with number 0 (the level test).</li>\n")
                .Append("<li><b>After each line, stay quiet for about two seconds</b> (count to two in your head): the kit cuts the take at these pauses. Inside a line, never pause that long.</li>\n")
                .Append("<li>Read each line once. If one goes wrong, just go on: the kit flags it, and the next scripts list it again.</li>\n")
                .Append("<li>Stop, and copy the recording (Documents\\Sound Recordings, an .m4a file) into <code>local\\voice-inbox\\")
                .Append(Html.Text(session.Id)).Append("\\</code>. Then run <b>\u0218coli\u021Ba \u2192 Import recording session</b>.</li>\n")
                .Append("</ol>\n");
            page.Append("<h2>How to say them</h2>\n<ul>\n");
            foreach (string kind in session.Items.Skip(1).Select(item => item.Kind).Distinct())
            {
                page.Append("<li>").Append(HowToSay(kind)).Append("</li>\n");
            }

            page.Append("</ul>\n<table>\n<tr><th>#</th><th>Say</th><th>Note</th></tr>\n");
            foreach (RecordingItem item in session.Items)
            {
                page.Append("<tr><td class=\"number\">").Append(item.Number).Append("</td><td class=\"say\">")
                    .Append(Html.Text(item.Text)).Append("</td><td class=\"note\">").Append(Note(item)).Append("</td></tr>\n");
            }

            page.Append("</table>\n").Append(Html.Foot());
            return page.ToString();
        }

        private static string Note(RecordingItem item)
        {
            if (item.IsLevelTest)
            {
                return "the level test, at your normal voice; it is not kept";
            }

            string note = Html.Text(item.Kind);
            if (item.ContinuesWith.Length > 0)
            {
                note += ": then stop, the app adds the " + (item.ContinuesWith == "X" || item.ContinuesWith == "Y" ? "letter" : "word");
            }

            return item.Segments > 1 ? note + " (part " + (item.Segment + 1) + " of " + item.Segments + ")" : note;
        }

        private static string HowToSay(string kind)
        {
            if (kind == ContentCodes.CodeOf(VoiceLineKind.Sound))
            {
                return "<b>Sounds are pure:</b> /mmm/, /sss/ held for about a second; /t/, /p/ short and crisp, with no vowel after (not \"tuh\", \"puh\"). The slashes are not said.";
            }

            if (kind == ContentCodes.CodeOf(VoiceLineKind.Name))
            {
                return "<b>Names</b> are the alphabet's names, as the teacher says them: Romanian \"a, be, ce, me, pe, te\"; English \"ay, bee, em, pee, tee\".";
            }

            if (kind == ContentCodes.CodeOf(VoiceLineKind.Word))
            {
                return "<b>Words:</b> said once, warmly, at a normal speed.";
            }

            if (kind == ContentCodes.CodeOf(VoiceLineKind.Syllables))
            {
                return "<b>Syllables:</b> the word in beats, with a short pause at each dash (\"ma \u2026 ma\"), shorter than the pause between lines.";
            }

            if (kind == ContentCodes.CodeOf(VoiceLineKind.Slow))
            {
                return "<b>Slow words:</b> stretched sound by sound (\"mmm-aaa-mmm-aaa\"), without stopping.";
            }

            if (kind == ContentCodes.CodeOf(VoiceLineKind.Stretched))
            {
                return "<b>Stretched words:</b> the first sound held for about a second, then the rest of the word at once (\"mmm-ama\", \"aaa-vion\"); a sound that can't be held (t, p) is said three times, short and pure (\"t-t-tata\").";
            }

            return "<b>Instructions and praise:</b> friendly and bright, as you would say them to her.";
        }
    }
}
