using System.Net;

namespace Scolita.Editor.Voice
{
    /// <summary>The kit's pages' shared parts: escaping text, and the head with its style.</summary>
    public static class Html
    {
        /// <summary><paramref name="text"/>, safe inside HTML; called by the kit's pages.</summary>
        public static string Text(string text)
        {
            return WebUtility.HtmlEncode(text ?? string.Empty);
        }

        /// <summary>A page's start, titled <paramref name="title"/>, with the kit's print-friendly style; called by the kit's pages.</summary>
        public static string Head(string title)
        {
            return "<!DOCTYPE html>\n<html lang=\"en\">\n<head>\n<meta charset=\"utf-8\">\n<title>" + Text(title)
                + "</title>\n<style>\n"
                + "body { font-family: Segoe UI, Arial, sans-serif; margin: 2em; color: #222; }\n"
                + "h1 { font-size: 1.6em; } h2 { font-size: 1.2em; margin-top: 1.5em; }\n"
                + "table { border-collapse: collapse; width: 100%; }\n"
                + "td, th { border-bottom: 1px solid #ccc; padding: 0.5em; text-align: left; vertical-align: middle; }\n"
                + "td.number { font-size: 1.4em; width: 3em; color: #666; }\n"
                + "td.say { font-size: 1.8em; }\n"
                + ".note { color: #666; font-size: 0.9em; }\n"
                + ".flag { color: #a33; font-weight: bold; }\n"
                + ".good { color: #272; }\n"
                + "@media print { tr { page-break-inside: avoid; } }\n"
                + "</style>\n</head>\n<body>\n";
        }

        /// <summary>A page's end; called by the kit's pages.</summary>
        public static string Foot()
        {
            return "</body>\n</html>\n";
        }
    }
}
