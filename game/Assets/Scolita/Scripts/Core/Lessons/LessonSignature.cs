using System.Globalization;
using System.Text;
using Scolita.Core.Content;
using Scolita.Core.Languages;

namespace Scolita.Core.Lessons
{
    /// <summary>
    /// A short text that changes whenever a lesson's steps change (their number, order, kinds,
    /// languages, items or options): steps have no ids, so a lesson in progress is resumed by step
    /// number only while its signature is the same (the <c>content-model</c> follow-up). The same
    /// steps give the same signature on every device: an FNV-1a hash of their written form, as 16
    /// hexadecimal digits.
    /// </summary>
    public static class LessonSignature
    {
        private const ulong OffsetBasis = 14695981039346656037UL;
        private const ulong Prime = 1099511628211UL;

        /// <summary>
        /// The signature of <paramref name="lesson"/>'s steps. Called by the Lesson scene's composition
        /// root (saving and resuming) and the tests.
        /// </summary>
        public static string Of(Lesson lesson)
        {
            StringBuilder text = new StringBuilder();
            foreach (Step step in lesson.Steps)
            {
                text.Append(step.Number.ToString(CultureInfo.InvariantCulture)).Append('|')
                    .Append(ContentCodes.CodeOf(step.Kind)).Append('|')
                    .Append(LanguageCodes.CodeOf(step.Languages)).Append('|')
                    .Append(string.Join(" ", step.Letters)).Append('|')
                    .Append(string.Join(" ", step.Words)).Append('|')
                    .Append(string.Join(";", step.Options)).Append('\n');
            }

            ulong hash = OffsetBasis;
            foreach (char character in text.ToString())
            {
                hash = (hash ^ character) * Prime;
            }

            return hash.ToString("x16", CultureInfo.InvariantCulture);
        }
    }
}
