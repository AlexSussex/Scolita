using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Languages;

namespace Scolita.Core.Activities
{
    /// <summary>
    /// Makes each step's <see cref="IActivityRules"/> by its kind: the one place a new kind's rules
    /// are added (one line here, beside its rules class). A kind not listed in <see cref="Kinds"/>
    /// has no rules yet and can't be played.
    /// </summary>
    public static class ActivityRulesFactory
    {
        private static readonly ActivityKind[] KindsWithRules = { ActivityKind.MeetLetter, ActivityKind.ListenAndPick };

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
        /// The rules of <paramref name="step"/>, its items taken from <paramref name="curriculum"/>.
        /// Throws an <see cref="ArgumentException"/> for a kind with no rules or a step in both
        /// languages, and a <see cref="KeyNotFoundException"/> for an item that isn't in the content
        /// (the validator makes both impossible). Called by the lesson run when a lesson is built.
        /// </summary>
        public static IActivityRules Create(Step step, Curriculum curriculum)
        {
            Language language = LanguageOf(step);
            switch (step.Kind)
            {
                case ActivityKind.MeetLetter:
                    return new MeetLetterRules(curriculum.Letters.Get(step.Letters[0]), curriculum.Words.Get(step.Words[0]),
                        language);
                case ActivityKind.ListenAndPick:
                    return step.Letters.Count > 0
                        ? ListenAndPickRules.OfLetters(step.Letters.Select(curriculum.Letters.Get).ToArray(), language)
                        : ListenAndPickRules.OfWords(step.Words.Select(curriculum.Words.Get).ToArray(), language);
                default:
                    throw new ArgumentException("The kind " + step.Kind + " has no rules yet.", nameof(step));
            }
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
    }
}
