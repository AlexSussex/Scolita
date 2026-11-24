using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Languages;

namespace Scolita.Core.Content.Rules
{
    /// <summary>
    /// The rules on each step of a lesson in use: the items its kind needs
    /// (<see cref="ActivityKinds"/>), in its languages; pictures where its kind shows them; no
    /// listen-only word where words are read; only letters taught in the word's track by its lesson,
    /// and only taught letters as choices; its options; its instruction and hint lines; and the
    /// lines every lesson says (<see cref="LessonLines"/>) in each language a step is in. Part of
    /// <see cref="ContentRules"/>.
    /// </summary>
    public static class StepRules
    {
        /// <summary>Checks every step of every lesson in use; called by <see cref="ContentRules.Check"/>.</summary>
        public static void Check(Curriculum curriculum, ContentProblems problems)
        {
            IEnumerable<Lesson> lessonsInUse = curriculum.Domains
                .SelectMany(domain => domain.Units)
                .Where(unit => !unit.Retired)
                .SelectMany(unit => unit.Lessons)
                .Where(lesson => !lesson.Retired);
            Dictionary<Language, ContentSource> firstStepIn = new Dictionary<Language, ContentSource>();
            foreach (Lesson lesson in lessonsInUse)
            {
                foreach (Step step in lesson.Steps)
                {
                    CheckStep(curriculum, lesson, step, problems);
                    foreach (Language language in step.Languages.Languages().Where(language => !firstStepIn.ContainsKey(language)))
                    {
                        firstStepIn.Add(language, step.Source);
                    }
                }
            }

            foreach (KeyValuePair<Language, ContentSource> language in firstStepIn)
            {
                VoiceLineRules.Line(curriculum, LessonLines.LessonDone(language.Key), language.Value, problems);
                VoiceLineRules.Line(curriculum, LessonLines.PraiseLine(language.Key, 1), language.Value, problems);
            }
        }

        private static void CheckStep(Curriculum curriculum, Lesson lesson, Step step, ContentProblems problems)
        {
            ActivityKindRequirements needs = ActivityKinds.Of(step.Kind);
            CheckCounts(step, needs, problems);
            CheckOptions(step, needs, problems);
            List<Letter> letters = new List<Letter>();
            foreach (ContentId letterId in step.Letters)
            {
                Letter letter = CheckLetter(curriculum, step, letterId, problems);
                if (letter != null)
                {
                    letters.Add(letter);
                }
            }

            if (needs.LettersAreChoices)
            {
                CheckChoicesTaught(curriculum, lesson, step, problems);
                CheckChoicesOnce(step, problems);
            }

            List<Word> words = new List<Word>();
            foreach (ContentId wordId in step.Words)
            {
                Word word = References.InUse(curriculum.Words, wordId, step.Source, "word", ContentFileNames.Words,
                    problems);
                if (word != null)
                {
                    CheckWord(curriculum, lesson, step, needs, word, problems);
                    CheckWordStart(step, needs, word, letters, problems);
                    words.Add(word);
                }
            }

            if (needs.ItemRules == ActivityItemRules.SortsByTheLettersTwoSounds)
            {
                CheckTwoSounds(step, letters, words, problems);
            }

            foreach (Language language in step.Languages.Languages())
            {
                foreach (string key in needs.InstructionLines.Concat(needs.HintLines))
                {
                    VoiceLineRules.Line(curriculum, VoiceLineIds.For(language, VoiceLineKind.Line, key), step.Source,
                        problems);
                }
            }
        }

        private static void CheckCounts(Step step, ActivityKindRequirements needs, ContentProblems problems)
        {
            string kind = ContentCodes.CodeOf(step.Kind);
            if (!needs.Letters.Holds(step.Letters.Count))
            {
                problems.Error(step.Source, "a " + kind + " step takes " + needs.Letters + " letters, not "
                    + step.Letters.Count + ".");
            }

            if (!needs.Words.Holds(step.Words.Count))
            {
                problems.Error(step.Source, "a " + kind + " step takes " + needs.Words + " words, not "
                    + step.Words.Count + ".");
            }

            if (needs.ItemRules == ActivityItemRules.LettersOrWords && step.Letters.Count > 0 && step.Words.Count > 0)
            {
                problems.Error(step.Source, "a " + kind + " step takes letters or words (shown as pictures), not both.");
            }

            int items = step.Letters.Count + step.Words.Count;
            if (!needs.Items.Holds(items))
            {
                problems.Error(step.Source, "a " + kind + " step takes " + needs.Items
                    + " letters and words together, not " + items + ".");
            }
        }

        private static void CheckWordStart(Step step, ActivityKindRequirements needs, Word word,
            IReadOnlyList<Letter> letters, ContentProblems problems)
        {
            bool needsItsStart = needs.ItemRules == ActivityItemRules.WordStartsWithALetter
                || needs.ItemRules == ActivityItemRules.SortsByTheLettersTwoSounds;
            if (!needsItsStart || letters.Count != step.Letters.Count)
            {
                return;
            }

            string spelling = WordRules.Spelling(word.Text);
            if (!letters.Any(letter => spelling.StartsWith(letter.Lower, StringComparison.Ordinal)))
            {
                problems.Error(step.Source, "the word " + word.Id + " doesn't start with "
                    + string.Join(" or ", step.Letters) + ", so the " + ContentCodes.CodeOf(step.Kind)
                    + " step has no right answer.");
            }
        }

        private static void CheckTwoSounds(Step step, IReadOnlyList<Letter> letters, IReadOnlyList<Word> words,
            ContentProblems problems)
        {
            string kind = ContentCodes.CodeOf(step.Kind);
            foreach (Letter letter in letters.Where(letter => !SoundsDiffer(letter.Group)))
            {
                problems.Error(step.Source, "a " + kind + " step needs a letter whose sound differs between Romanian and "
                    + "English (" + ContentCodes.CodeOf(LetterGroup.VowelDiffers) + " or "
                    + ContentCodes.CodeOf(LetterGroup.SoundDiffers) + "); " + letter.Id + " is "
                    + ContentCodes.CodeOf(letter.Group) + ".");
            }

            foreach (Language language in LanguageMix.Both.Languages().Where(language => words.Count > 0
                && words.All(word => word.Language != language)))
            {
                problems.Error(step.Source, "a " + kind + " step sorts pictures of both languages, and none of its words is "
                    + LanguageCodes.CodeOf(language) + ".");
            }
        }

        private static bool SoundsDiffer(LetterGroup group)
        {
            return group == LetterGroup.VowelDiffers || group == LetterGroup.SoundDiffers;
        }

        private static void CheckOptions(Step step, ActivityKindRequirements needs, ContentProblems problems)
        {
            string kind = ContentCodes.CodeOf(step.Kind);
            foreach (StepOption option in step.Options)
            {
                ActivityOption known = needs.Options.FirstOrDefault(candidate => candidate.Key == option.Key);
                if (known == null)
                {
                    string keys = needs.Options.Count == 0 ? "none" : string.Join(", ", needs.Options.Select(candidate => candidate.Key));
                    problems.Error(step.Source, "a " + kind + " step has no option " + option.Key + " (its options: " + keys + ").");
                }
                else if (!known.Values.Contains(option.Value))
                {
                    problems.Error(step.Source, "the option " + option.Key + " is " + string.Join(" or ", known.Values)
                        + ", not " + option.Value + ".");
                }
            }

            foreach (ActivityOption required in needs.Options.Where(candidate => candidate.Required))
            {
                if (step.Options.Count(option => option.Key == required.Key) != 1)
                {
                    problems.Error(step.Source, "a " + kind + " step needs its option " + required.Key + " once ("
                        + string.Join(" or ", required.Values) + ").");
                }
            }
        }

        private static Letter CheckLetter(Curriculum curriculum, Step step, ContentId letterId, ContentProblems problems)
        {
            Letter letter = References.InUse(curriculum.Letters, letterId, step.Source, "letter",
                ContentFileNames.Letters, problems);
            if (letter == null)
            {
                return null;
            }

            foreach (Language language in step.Languages.Languages().Where(language => !letter.HasLanguage(language)))
            {
                problems.Error(step.Source, "the letter " + letterId + " is not in the "
                    + LanguageCodes.CodeOf(language) + " alphabet, and a " + ContentCodes.CodeOf(step.Kind)
                    + " step here needs it in " + LanguageCodes.CodeOf(step.Languages) + ".");
            }

            return letter;
        }

        private static void CheckWord(Curriculum curriculum, Lesson lesson, Step step, ActivityKindRequirements needs,
            Word word, ContentProblems problems)
        {
            if (!step.Languages.Includes(word.Language))
            {
                problems.Error(step.Source, "the word " + word.Id + " is " + LanguageCodes.CodeOf(word.Language)
                    + ", but the step is " + LanguageCodes.CodeOf(step.Languages) + ": a step's words are in its language.");
            }

            bool pictured = needs.WordUse == ActivityWordUse.Pictured || needs.WordUse == ActivityWordUse.ReadAndPictured;
            bool read = needs.WordUse == ActivityWordUse.Read || needs.WordUse == ActivityWordUse.ReadAndPictured;
            if (pictured && !word.Picture.IsSet)
            {
                problems.Error(step.Source, "a " + ContentCodes.CodeOf(step.Kind) + " step shows its words' pictures, and the word "
                    + word.Id + " has none.");
            }

            if (read && word.ListenOnly)
            {
                problems.Error(step.Source, "a " + ContentCodes.CodeOf(step.Kind) + " step shows its words to read, and the word "
                    + word.Id + " is listen-only.");
            }

            if (!word.ListenOnly)
            {
                CheckTaughtLetters(curriculum, lesson, step, word, problems);
            }

            foreach (VoiceLineKind kind in needs.WordLines)
            {
                VoiceLineRules.Line(curriculum, VoiceLineIds.For(word.Language, kind, word.Key), step.Source, problems);
            }
        }

        private static void CheckChoicesTaught(Curriculum curriculum, Lesson lesson, Step step, ContentProblems problems)
        {
            foreach (Language language in step.Languages.Languages())
            {
                List<ContentId> taught = TaughtBy(curriculum, lesson, language);
                ContentId[] untaught = step.Letters
                    .Where(letter => !taught.Contains(letter))
                    .Distinct()
                    .ToArray();
                if (untaught.Length > 0)
                {
                    string known = taught.Count == 0 ? "none yet" : string.Join(" ", taught);
                    problems.Error(step.Source, "a " + ContentCodes.CodeOf(step.Kind) + " step's letters are its choices, and "
                        + "some are not taught yet in the " + LanguageCodes.CodeOf(language) + " track by the lesson "
                        + lesson.Id + ": " + string.Join(" ", untaught) + " (taught: " + known
                        + "); a wrong choice is never a letter still to come.");
                }
            }
        }

        private static void CheckChoicesOnce(Step step, ContentProblems problems)
        {
            ContentId[] repeated = step.Letters
                .Concat(step.Words)
                .GroupBy(item => item)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToArray();
            if (repeated.Length > 0)
            {
                problems.Error(step.Source, "a " + ContentCodes.CodeOf(step.Kind) + " step's items are its choices, each once; "
                    + string.Join(" ", repeated) + " is listed more than once.");
            }
        }

        private static List<ContentId> TaughtBy(Curriculum curriculum, Lesson lesson, Language language)
        {
            List<ContentId> taught = curriculum.LettersTaughtBefore(lesson, language).ToList();
            if (lesson.Introduces.IsSet && lesson.Languages == language.Alone())
            {
                taught.Add(lesson.Introduces);
            }

            return taught;
        }

        private static void CheckTaughtLetters(Curriculum curriculum, Lesson lesson, Step step, Word word,
            ContentProblems problems)
        {
            List<ContentId> taught = TaughtBy(curriculum, lesson, word.Language);

            ContentId[] untaught = word.Letters
                .Where(letter => !taught.Contains(letter))
                .Distinct()
                .ToArray();
            if (untaught.Length > 0)
            {
                string known = taught.Count == 0 ? "none yet" : string.Join(" ", taught);
                problems.Error(step.Source, "the word " + word.Id + " uses " + string.Join(" ", untaught)
                    + ", not taught yet in the " + LanguageCodes.CodeOf(word.Language) + " track by the lesson "
                    + lesson.Id + " (taught: " + known + "); mark the word listen_only if it is only heard here.");
            }
        }
    }
}
