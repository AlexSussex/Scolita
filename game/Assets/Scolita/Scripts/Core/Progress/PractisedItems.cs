using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Languages;

namespace Scolita.Core.Progress
{
    /// <summary>
    /// Which items a step practises, by its kind (the one place a new kind's items are named, one line per
    /// kind, as <c>ActivityRulesFactory</c> names its rules): <c>ListenAndPick</c> its answer (the first
    /// letter, or the first word), <c>LetterHunt</c> and <c>TraceLetter</c> their letter, <c>FirstSound</c>
    /// the letter its word starts with, <c>TwoSounds</c> its letter in both languages, the word kinds their
    /// word (<c>WordTwins</c> all its words), <c>MeetLetter</c> its letter (met, not answered).
    /// </summary>
    public static class PractisedItems
    {
        /// <summary>
        /// The items <paramref name="step"/> practises, in <paramref name="curriculum"/>'s content (an item the
        /// content doesn't know is left out). Called by <see cref="LessonProgress"/> when a lesson is built, and
        /// the tests.
        /// </summary>
        public static IReadOnlyList<LearningItem> Of(Step step, Curriculum curriculum)
        {
            switch (step.Kind)
            {
                case ActivityKind.MeetLetter:
                case ActivityKind.LetterHunt:
                case ActivityKind.TraceLetter:
                case ActivityKind.TwoSounds:
                    return LettersOf(step, curriculum, step.Letters.Take(1));
                case ActivityKind.ListenAndPick:
                    return step.Letters.Count > 0
                        ? LettersOf(step, curriculum, step.Letters.Take(1))
                        : WordsOf(curriculum, step.Words.Take(1));
                case ActivityKind.FirstSound:
                    return FirstSoundLetter(step, curriculum);
                case ActivityKind.Syllables:
                case ActivityKind.BuildWord:
                case ActivityKind.ReadAndMatch:
                case ActivityKind.TraceWord:
                    return WordsOf(curriculum, step.Words.Take(1));
                case ActivityKind.WordTwins:
                    return WordsOf(curriculum, step.Words);
                default:
                    throw new ArgumentOutOfRangeException(nameof(step), step.Kind, "No items named for this kind.");
            }
        }

        /// <summary>
        /// Whether a step of <paramref name="kind"/> is answered (a done step is a right answer, or practice);
        /// false for <c>MeetLetter</c>, a first contact with no wrong answers. Called by
        /// <see cref="LessonProgress"/> and the tests.
        /// </summary>
        public static bool IsAnswered(ActivityKind kind)
        {
            return kind != ActivityKind.MeetLetter;
        }

        private static IReadOnlyList<LearningItem> LettersOf(Step step, Curriculum curriculum, IEnumerable<ContentId> letterIds)
        {
            return letterIds
                .Where(curriculum.Letters.Contains)
                .Select(curriculum.Letters.Get)
                .SelectMany(letter => step.Languages.Languages()
                    .Where(letter.HasLanguage)
                    .Select(language => LearningItem.Letter(letter.Id, language)))
                .ToArray();
        }

        private static IReadOnlyList<LearningItem> WordsOf(Curriculum curriculum, IEnumerable<ContentId> wordIds)
        {
            return wordIds
                .Where(curriculum.Words.Contains)
                .Select(curriculum.Words.Get)
                .Select(word => new LearningItem(ItemKind.Word, word.Id, word.Language))
                .ToArray();
        }

        private static IReadOnlyList<LearningItem> FirstSoundLetter(Step step, Curriculum curriculum)
        {
            if (step.Words.Count == 0 || !curriculum.Words.TryGet(step.Words[0], out Word word))
            {
                return new LearningItem[0];
            }

            IEnumerable<Letter> letters = step.Letters.Where(curriculum.Letters.Contains).Select(curriculum.Letters.Get);
            return WordStarts.TryFind(word, letters, out Letter answer) && answer.HasLanguage(word.Language)
                ? new[] { LearningItem.Letter(answer.Id, word.Language) }
                : new LearningItem[0];
        }
    }
}
