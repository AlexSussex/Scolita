using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;
using Scolita.Core.Tracing;

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
            ActivityKind.WordTwins, ActivityKind.TraceLetter, ActivityKind.TraceWord
        };

        /// <summary>The kinds that have rules.</summary>
        public static IReadOnlyList<ActivityKind> Kinds => KindsWithRules;

        /// <summary>
        /// Whether <paramref name="kind"/> has rules; called by the Lesson scene's composition root (the
        /// kinds that can be played), the registry's validator test and the tests.
        /// </summary>
        public static bool Has(ActivityKind kind)
        {
            return HasRules(kind);
        }

        /// <summary>
        /// Whether <paramref name="step"/> can be played with <paramref name="curriculum"/>'s content: its kind
        /// has rules and, for a <see cref="ActivityKind.TraceLetter"/> step, its letter has the shape it asks
        /// for (so the handwritten steps wait for their shapes). Called by the playable steps and the tests.
        /// </summary>
        public static bool CanCreate(Step step, Curriculum curriculum)
        {
            switch (step.Kind)
            {
                case ActivityKind.TraceLetter:
                    return TryGetShape(step, curriculum, out _, out _);
                case ActivityKind.TraceWord:
                    return TryGetWordShapes(step, curriculum, out _, out _, out _);
                default:
                    return HasRules(step.Kind);
            }
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
                    return MeetLetter(step, curriculum);
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
                case ActivityKind.TraceLetter:
                    return TraceLetter(step, curriculum, rules);
                case ActivityKind.TraceWord:
                    return TraceWord(step, curriculum, rules);
                default:
                    throw new ArgumentException("The kind " + step.Kind + " has no rules yet.", nameof(step));
            }
        }

        private static TraceLetterRules TraceLetter(Step step, Curriculum curriculum, LearningRules rules)
        {
            if (!TryGetShape(step, curriculum, out Letter letter, out LetterShape shape))
            {
                throw new KeyNotFoundException("The TraceLetter step " + step.Number + " of " + step.LessonId
                    + " has no shape for its letter, case and form.");
            }

            return new TraceLetterRules(letter, shape, LanguageOf(step), rules.Tracing, RoundsOf(step));
        }

        private static TraceWordRules TraceWord(Step step, Curriculum curriculum, LearningRules rules)
        {
            if (!TryGetWordShapes(step, curriculum, out Word word, out Letter[] letters, out LetterShape[] shapes))
            {
                throw new KeyNotFoundException("The TraceWord step " + step.Number + " of " + step.LessonId
                    + " has no handwritten shape for every letter of its word, or a letter before the last without an exit.");
            }

            return new TraceWordRules(word, letters, shapes, LanguageOf(step), rules.Tracing, RoundsOf(step));
        }

        private static bool TryGetWordShapes(Step step, Curriculum curriculum, out Word word, out Letter[] letters,
            out LetterShape[] shapes)
        {
            letters = null;
            shapes = null;
            if (step.Words.Count != 1 || !curriculum.Words.TryGet(step.Words[0], out word))
            {
                word = null;
                return false;
            }

            letters = new Letter[word.Letters.Count];
            shapes = new LetterShape[word.Letters.Count];
            for (int index = 0; index < letters.Length; index++)
            {
                if (!curriculum.Letters.TryGet(word.Letters[index], out letters[index])
                    || !curriculum.TryGetShape(letters[index], LetterCase.Lower, LetterForm.Handwritten, out shapes[index])
                    || (index + 1 < letters.Length && !LetterShapeCodes.HasExit(shapes[index])))
                {
                    return false;
                }
            }

            return letters.Length > 0 && letters.Length <= ActivityLimits.MostTracedLetters;
        }

        private static MeetLetterRules MeetLetter(Step step, Curriculum curriculum)
        {
            Letter letter = curriculum.Letters.Get(step.Letters[0]);
            curriculum.TryGetShape(letter, LetterCase.Lower, LetterForm.Handwritten, out LetterShape handwritten);
            return new MeetLetterRules(letter, curriculum.Words.Get(step.Words[0]), LanguageOf(step), handwritten);
        }

        private static int RoundsOf(Step step)
        {
            return step.TryGetOption(LetterShapeCodes.RoundsOption, out string value)
                && int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out int rounds)
                ? rounds
                : TracingTuning.Rounds;
        }

        private static bool TryGetShape(Step step, Curriculum curriculum, out Letter letter, out LetterShape shape)
        {
            shape = null;
            letter = null;
            return step.Letters.Count == 1
                && curriculum.Letters.TryGet(step.Letters[0], out letter)
                && LetterShapeCodes.TryGetStepShape(step, out LetterCase letterCase, out LetterForm form)
                && curriculum.TryGetShape(letter, letterCase, form, out shape);
        }

        private static bool HasRules(ActivityKind kind)
        {
            return Array.IndexOf(KindsWithRules, kind) >= 0;
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
