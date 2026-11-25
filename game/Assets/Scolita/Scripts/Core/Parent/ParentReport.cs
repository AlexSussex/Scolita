using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Map;
using Scolita.Core.Progress;
using Scolita.Core.Timing;

namespace Scolita.Core.Parent
{
    /// <summary>
    /// What the parent area's "Ce a învățat" shows (PRD §3.7, design screen 7): a row per language with its
    /// track's letters and their states (new, learning, learned; the other language's progress shows even
    /// when the mix hides it), the days played in the last two weeks, the next lessons on her map, and the
    /// voice status. Made by <c>ParentRunner</c> when the area opens or her progress is reset, and the tests.
    /// </summary>
    public sealed class ParentReport
    {
        /// <summary>How many days the report shows, ending today ("the last two weeks", in its heading).</summary>
        public const int DaysShown = 14;

        /// <summary>How many of the next lessons it lists.</summary>
        public const int NextLessonsShown = 3;

        /// <summary>
        /// The report of <paramref name="progress"/> in <paramref name="domain"/>, with the next lessons of
        /// <paramref name="route"/> (her map, in her mix) and the days up to <paramref name="today"/>. Called by
        /// <c>ParentRunner</c> and the tests.
        /// </summary>
        public ParentReport(Curriculum curriculum, Domain domain, MapRoute route, LearnerProgress progress,
            CalendarDay today)
        {
            if (curriculum == null)
            {
                throw new ArgumentNullException(nameof(curriculum));
            }

            if (domain == null)
            {
                throw new ArgumentNullException(nameof(domain));
            }

            if (route == null)
            {
                throw new ArgumentNullException(nameof(route));
            }

            if (progress == null)
            {
                throw new ArgumentNullException(nameof(progress));
            }

            Rows = new[]
            {
                RowOf(curriculum, domain, progress, Language.Romanian),
                RowOf(curriculum, domain, progress, Language.English)
            };
            DaysPlayed = DaysOf(progress, today);
            NextLessons = NextOf(curriculum, route);
            Voices = VoiceStatus.Of(curriculum.VoiceLines);
        }

        /// <summary>Romanian's row, then English's.</summary>
        public IReadOnlyList<LetterRow> Rows { get; }

        /// <summary>Whether she played on each of the last <see cref="DaysShown"/> days, the oldest first, today last.</summary>
        public IReadOnlyList<bool> DaysPlayed { get; }

        /// <summary>Up to <see cref="NextLessonsShown"/> lessons not done yet, from the rocket's on; empty when all are done.</summary>
        public IReadOnlyList<NextLesson> NextLessons { get; }

        /// <summary>The voice lines recorded per language, Romanian then English.</summary>
        public IReadOnlyList<LanguageVoiceStatus> Voices { get; }

        private static LetterRow RowOf(Curriculum curriculum, Domain domain, LearnerProgress progress, Language language)
        {
            LetterCell[] cells = curriculum.Track(domain.Id, language).Letters
                .Select(id => curriculum.Letters.Get(id))
                .Select(letter => new LetterCell(letter, progress.StateOf(LearningItem.Letter(letter.Id, language))))
                .ToArray();
            return new LetterRow(language, cells);
        }

        private static bool[] DaysOf(LearnerProgress progress, CalendarDay today)
        {
            bool[] played = new bool[DaysShown];
            foreach (CalendarDay day in progress.DaysPlayed)
            {
                int daysAgo = today.DaysSince(day);
                if (daysAgo >= 0 && daysAgo < DaysShown)
                {
                    played[DaysShown - 1 - daysAgo] = true;
                }
            }

            return played;
        }

        private static NextLesson[] NextOf(Curriculum curriculum, MapRoute route)
        {
            if (route.Next == null)
            {
                return new NextLesson[0];
            }

            return route.Stops
                .Skip(route.Next.Index)
                .Where(stop => stop.State != StopState.Done)
                .Take(NextLessonsShown)
                .Select(stop => new NextLesson(stop.Lesson, stop.PlanetIndex + 1, LetterOf(curriculum, stop.Lesson)))
                .ToArray();
        }

        private static Letter LetterOf(Curriculum curriculum, Lesson lesson)
        {
            return lesson.Introduces.IsSet && curriculum.Letters.TryGet(lesson.Introduces, out Letter letter)
                ? letter
                : null;
        }
    }
}
