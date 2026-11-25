using System.Collections.Generic;
using Scolita.Core.Languages;

namespace Scolita.Core.Content
{
    /// <summary>
    /// The lines every lesson says whatever its steps' kinds, in each of its steps' languages: the
    /// praise after a step (<c>&lt;language&gt;-line-praise-1</c>, <c>-2</c> … as many as the content
    /// has, so a new one needs no code), the line at its end (<c>lesson-done</c>) and the line before its
    /// warm-ups (<c>lets-remember</c>).
    /// </summary>
    public static class LessonLines
    {
        /// <summary>The key of the line said when a lesson is finished.</summary>
        public const string LessonDoneKey = "lesson-done";

        /// <summary>The key of the line said before a lesson's warm-ups.</summary>
        public const string LetsRememberKey = "lets-remember";

        private const string PraiseKeyPrefix = "praise-";

        /// <summary>
        /// The line said at the end of a lesson in <paramref name="language"/>. Called by the content
        /// rules, <c>LessonVoiceLines</c> and the lesson run.
        /// </summary>
        public static VoiceLineId LessonDone(Language language)
        {
            return VoiceLineIds.For(language, VoiceLineKind.Line, LessonDoneKey);
        }

        /// <summary>
        /// The line said before a lesson's warm-ups, in <paramref name="language"/> ("Hai să ne amintim!"). Called by
        /// the content rules, <c>LessonVoiceLines</c> and the lesson run.
        /// </summary>
        public static VoiceLineId LetsRemember(Language language)
        {
            return VoiceLineIds.For(language, VoiceLineKind.Line, LetsRememberKey);
        }

        /// <summary>
        /// The praise line number <paramref name="number"/> (from 1) in <paramref name="language"/>.
        /// Called by <see cref="Praise"/> and the content rules.
        /// </summary>
        public static VoiceLineId PraiseLine(Language language, int number)
        {
            return VoiceLineIds.For(language, VoiceLineKind.Line, PraiseKeyPrefix + number);
        }

        /// <summary>
        /// The praise lines of <paramref name="language"/> in <paramref name="curriculum"/>:
        /// <c>praise-1</c>, <c>praise-2</c> … up to the first number missing (or retired). Called by
        /// <c>LessonVoiceLines</c> and <c>PraisePicker</c>, when a lesson is built.
        /// </summary>
        public static IReadOnlyList<VoiceLineId> Praise(Curriculum curriculum, Language language)
        {
            List<VoiceLineId> lines = new List<VoiceLineId>();
            for (int number = 1; ; number++)
            {
                VoiceLineId line = PraiseLine(language, number);
                if (!curriculum.VoiceLines.TryGet(line.Id, out VoiceLine found) || found.Retired)
                {
                    return lines;
                }

                lines.Add(line);
            }
        }
    }
}
