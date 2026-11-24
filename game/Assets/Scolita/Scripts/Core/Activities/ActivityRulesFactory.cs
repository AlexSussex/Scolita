using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;

namespace Scolita.Core.Activities
{
    /// <summary>
    /// Makes each step's <see cref="IActivityRules"/> by its kind: the one place a new kind's rules
    /// are added (one line here, beside its rules class). A kind not listed in <see cref="Kinds"/>
    /// has no rules yet and can't be played.
    /// </summary>
    public static class ActivityRulesFactory
    {
        private static readonly ActivityKind[] KindsWithRules =
        {
            ActivityKind.MeetLetter, ActivityKind.ListenAndPick, ActivityKind.LetterHunt, ActivityKind.FirstSound,
            ActivityKind.TwoSounds, ActivityKind.Syllables, ActivityKind.BuildWord, ActivityKind.ReadAndMatch,
            ActivityKind.WordTwins
        };

        /// <summary>The kinds that have rules.</summary>
        public static IReadOnlyList<ActivityKind> Kinds => KindsWithRules;

        /// <summary>
        /// Whether <paramref name="kind"/> has rules; called by the Lesson scene's composition root (the
        /// kinds that can be played), the registry's validator test and the tests.
        /// </summary>
        public static bool Has(ActivityKind kind)
        {
            return Array.IndexOf(KindsWithRules, kind) >= 0;
        }

        /// <summary>
        /// The rules of <paramref name="step"/>, its items taken from <paramref name="curriculum"/> and its
        /// numbers from <paramref name="rules"/>. Throws an <see cref="ArgumentException"/> for a kind with
        /// no rules or a step in both languages that isn't a bridge kind, and a
        /// <see cref="KeyNotFoundException"/> for an item or a lesson that isn't in the content (the
        /// validator makes both impossible). Called by the lesson run when a lesson is built.
        /// </summary>
        public static IActivityRules Create(Step step, Curriculum curriculum, LearningRules rules)
        {
            switch (step.Kind)
            {
                case ActivityKind.MeetLetter:
                    return new MeetLetterRules(curriculum.Letters.Get(step.Letters[0]), curriculum.Words.Get(step.Words[0]),
                        LanguageOf(step));
                case ActivityKind.ListenAndPick:
                    return step.Letters.Count > 0
                        ? ListenAndPickRules.OfLetters(LettersOf(step, curriculum), LanguageOf(step))
                        : ListenAndPickRules.OfWords(step.Words.Select(curriculum.Words.Get).ToArray(), LanguageOf(step));
                case ActivityKind.LetterHunt:
                    return new LetterHuntRules(LettersOf(step, curriculum), LanguageOf(step), rules.LetterHunt);
                case ActivityKind.FirstSound:
                    return FirstSound(step, curriculum);
                case ActivityKind.TwoSounds:
                    return new TwoSoundsRules(curriculum.Letters.Get(step.Letters[0]),
                        step.Words.Select(curriculum.Words.Get).ToArray(), LessonLanguageOf(step, curriculum));
                case ActivityKind.Syllables:
                    return new SyllablesRules(curriculum.Words.Get(step.Words[0]), LanguageOf(step));
                case ActivityKind.BuildWord:
                    return BuildWord(step, curriculum);
                case ActivityKind.ReadAndMatch:
                    return ReadAndMatch(step, curriculum);
                case ActivityKind.WordTwins:
                    return new WordTwinsRules(step.Words.Select(curriculum.Words.Get).ToArray(),
                        LessonLanguageOf(step, curriculum));
                default:
                    throw new ArgumentException("The kind " + step.Kind + " has no rules yet.", nameof(step));
            }
        }

        private static FirstSoundRules FirstSound(Step step, Curriculum curriculum)
        {
            Word word = curriculum.Words.Get(step.Words[0]);
            VoiceLine stretched = curriculum.VoiceLines.Get(word.StretchedLine.Id);
            return new FirstSoundRules(LettersOf(step, curriculum), word, LanguageOf(step), stretched.Text);
        }

        private static BuildWordRules BuildWord(Step step, Curriculum curriculum)
        {
            Word word = curriculum.Words.Get(step.Words[0]);
            Letter[] spelling = word.Letters.Select(curriculum.Letters.Get).ToArray();
            return new BuildWordRules(word, spelling, LettersOf(step, curriculum), LanguageOf(step), SlowTextOf(word, curriculum));
        }

        private static ReadAndMatchRules ReadAndMatch(Step step, Curriculum curriculum)
        {
            Word[] words = step.Words.Select(curriculum.Words.Get).ToArray();
            return new ReadAndMatchRules(words, LanguageOf(step), SlowTextOf(words[0], curriculum));
        }

        private static string SlowTextOf(Word word, Curriculum curriculum)
        {
            return curriculum.VoiceLines.Get(word.SlowLine.Id).Text;
        }

        private static Letter[] LettersOf(Step step, Curriculum curriculum)
        {
            return step.Letters.Select(curriculum.Letters.Get).ToArray();
        }

        private static Language LanguageOf(Step step)
        {
            switch (step.Languages)
            {
                case LanguageMix.RomanianOnly:
                    return Language.Romanian;
                case LanguageMix.EnglishOnly:
                    return Language.English;
                default:
                    throw new ArgumentException("The " + step.Kind + " step " + step.Number + " of " + step.LessonId
                        + " is in both languages; only a bridge kind may be.", nameof(step));
            }
        }

        private static Language LessonLanguageOf(Step step, Curriculum curriculum)
        {
            if (!curriculum.TryGetLesson(step.LessonId, out Lesson lesson))
            {
                throw new KeyNotFoundException("The step " + step.Number + " names the lesson " + step.LessonId
                    + ", which isn't in the content.");
            }

            return lesson.Languages == LanguageMix.EnglishOnly ? Language.English : Language.Romanian;
        }
    }
}
