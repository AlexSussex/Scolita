using System.Linq;
using System.Text;

namespace Scolita.Editor.Voice
{
    /// <summary>
    /// The report page of an imported session: how the take matched its script, and each clip with a
    /// player beside its text, its numbers and its flags, so only the flagged lines are recorded again.
    /// </summary>
    public static class RecordingReportPage
    {
        /// <summary>The page of <paramref name="result"/>, the import of <paramref name="session"/>; called by <see cref="RecordingSessionImport"/>.</summary>
        public static string Render(RecordingSession session, RecordingResult result)
        {
            int lines = result.Items.Count(item => !string.IsNullOrEmpty(item.Line));
            int good = result.Items.Count(item => item.IsGood);
            StringBuilder page = new StringBuilder();
            page.Append(Html.Head("\u0218coli\u021Ba session " + session.Id + ": report"));
            page.Append("<h1>Session ").Append(Html.Text(session.Id)).Append("</h1>\n");
            page.Append("<p>").Append(Html.Text(result.Summary)).Append("</p>\n");
            page.Append("<p><b>").Append(good).Append(" of ").Append(lines).Append(" clips are good.</b> ")
                .Append("Listen to each one beside its text. When the session is good, run <b>\u0218coli\u021Ba \u2192 Accept recording session</b>: ")
                .Append("the good clips go into the app, and the flagged ones come back in the next scripts.</p>\n");
            page.Append("<table>\n<tr><th>#</th><th>Text</th><th>Listen</th><th>Numbers</th><th>Flags</th></tr>\n");
            foreach (RecordingResultItem item in result.Items)
            {
                page.Append("<tr><td class=\"number\">").Append(item.Number).Append("</td><td class=\"say\">")
                    .Append(Html.Text(item.Text)).Append("</td><td>");
                if (item.Wav.Length > 0)
                {
                    page.Append("<audio controls preload=\"none\" src=\"clips/").Append(Html.Text(item.Wav)).Append("\"></audio>");
                }

                page.Append("</td><td class=\"note\">").Append(Html.Text(item.Measurement)).Append("</td><td>");
                if (string.IsNullOrEmpty(item.Line))
                {
                    page.Append("<span class=\"note\">level test, not kept</span>");
                }
                else if (item.Flags.Count == 0)
                {
                    page.Append("<span class=\"good\">good</span>");
                }
                else
                {
                    page.Append("<span class=\"flag\">").Append(Html.Text(string.Join(", ", item.Flags.Select(Explain)))).Append("</span>");
                }

                page.Append("</td></tr>\n");
            }

            page.Append("</table>\n").Append(Html.Foot());
            return page.ToString();
        }

        private static string Explain(string flag)
        {
            switch (flag)
            {
                case nameof(ClipFlag.Missed):
                    return "seems missed";
                case nameof(ClipFlag.Repeated):
                    return "a piece too many near here";
                case nameof(ClipFlag.Uncertain):
                    return "may be another line's: listen";
                case nameof(ClipFlag.TooQuiet):
                    return "too quiet";
                case nameof(ClipFlag.TooLoud):
                    return "too loud";
                case nameof(ClipFlag.Clipped):
                    return "clipped (too close to the microphone)";
                case nameof(ClipFlag.TooLong):
                    return "too long";
                case nameof(ClipFlag.TooShort):
                    return "too short";
                default:
                    return "silence at an end";
            }
        }
    }
}
