using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;
using Scolita.Core.Randomness;
using Scolita.Core.Timing;

namespace Scolita.Core.Progress
{
    /// <summary>
    /// The warm-ups at a lesson's start (<c>docs/curriculum.md</c> §4.1 and §5): up to
    /// <see cref="ProgressRules.WarmUpsAtStart"/> of her learned letters due for review today, most overdue
    /// first, then in teaching order; only letters in the lesson's language (either, for a lesson in both) and
    /// in her mix, taught before the lesson in their track, and not the letter the lesson introduces. Each is
    /// one step: a <c>ListenAndPick</c> on the letter among other letters taught before the lesson (only taught
    /// letters as wrong choices), or a <c>FirstSound</c> on a word from the content's own <c>FirstSound</c>
    /// steps for that letter up to this lesson (so its picture and stretched line exist, and it isn't from a later
    /// planet), chosen at random among the kinds that can be played. A letter with no possible step (no other
    /// letter taught yet, no kind that can be played) waits for a later lesson.
    /// </summary>
    public sealed class WarmUps : IWarmUps
    {
        /// <summary>The source a warm-up step names, for logs (it comes from no row of the content).</summary>
        public const string SourceName = "warm-up";

        private readonly LearnerProgress progress;
        private readonly Curriculum curriculum;
        private readonly LanguageMix mix;
        private readonly PlayableSteps playable;
        private readonly ProgressRules rules;
        private readonly CalendarDay today;
        private readonly IRandom random;

        /// <summary>
        /// Creates the warm-ups of <paramref name="learner"/> on <paramref name="day"/>, for a learner of
        /// <paramref name="languageMix"/>, from <paramref name="content"/>, only of steps <paramref name="playableSteps"/>
        /// plays, by <paramref name="progressRules"/>, choosing with <paramref name="randomness"/>. Called by the Lesson
        /// scene's composition root and the tests.
        /// </summary>
        public WarmUps(LearnerProgress learner, Curriculum content, LanguageMix languageMix, PlayableSteps playableSteps,
            ProgressRules progressRules, CalendarDay day, IRandom randomness)
        {
            playable = playableSteps ?? throw new ArgumentNullException(nameof(playableSteps));
            progress = learner ?? throw new ArgumentNullException(nameof(learner));
            curriculum = content ?? throw new ArgumentNullException(nameof(content));
            mix = languageMix;
            rules = progressRules ?? throw new ArgumentNullException(nameof(progressRules));
            today = day;
            random = randomness ?? throw new ArgumentNullException(nameof(randomness));
        }

        /// <inheritdoc/>
        public IReadOnlyList<Step> For(Lesson lesson)
        {
            List<Step> steps = new List<Step>();
            if (rules.WarmUpsAtStart == 0 || !curriculum.TryGetLesson(lesson.Id, out Lesson _))
            {
                return steps;
            }

            Dictionary<Language, IReadOnlyList<ContentId>> taught = lesson.Languages.Languages()
                .Where(language => mix.Includes(language))
                .ToDictionary(language => language, language => curriculum.LettersTaughtBefore(lesson, language));
            IEnumerable<ItemProgress> due = progress.DueOn(today)
                .Where(item => item.Item.Kind == ItemKind.Letter)
                .Where(item => taught.ContainsKey(item.Item.Language))
                .Where(item => taught[item.Item.Language].Contains(item.Item.Id))
                .Where(item => !IsIntroducedBy(lesson, item.Item))
                .OrderBy(item => item.NextReview.HasValue ? item.NextReview.Value.DayNumber : int.MinValue)
                .ThenBy(item => IndexOf(taught[item.Item.Language], item.Item.Id))
                .ThenBy(item => item.Item.Language);
            foreach (ItemProgress item in due)
            {
                if (steps.Count >= rules.WarmUpsAtStart)
                {
                    break;
                }

                if (TryMake(lesson, item.Item, taught[item.Item.Language], out Step step))
                {
                    steps.Add(step);
                }
            }

            return steps;
        }

        private static bool IsIntroducedBy(Lesson lesson, LearningItem item)
        {
            return lesson.Introduces == item.Id && lesson.Languages == item.Language.Alone();
        }

        private static int IndexOf(IReadOnlyList<ContentId> letters, ContentId id)
        {
            for (int index = 0; index < letters.Count; index++)
            {
                if (letters[index] == id)
                {
                    return index;
                }
            }

            return letters.Count;
        }

        private bool TryMake(Lesson lesson, LearningItem item, IReadOnlyList<ContentId> taught, out Step step)
        {
            step = null;
            if (!curriculum.Letters.TryGet(item.Id, out Letter letter) || letter.Retired || !letter.HasLanguage(item.Language))
            {
                return false;
            }

            List<Letter> others = taught
                .Where(id => id != item.Id)
                .Distinct()
                .Where(curriculum.Letters.Contains)
                .Select(curriculum.Letters.Get)
                .Where(other => !other.Retired && other.HasLanguage(item.Language))
                .ToList();
            if (others.Count == 0)
            {
                return false;
            }

            List<Letter> choices = new List<Letter> { letter };
            choices.AddRange(Pick(others, rules.WarmUpChoices - 1));
            ContentId[] letterIds = choices.Select(choice => choice.Id).ToArray();
            Step pick = Make(lesson, item, ActivityKind.ListenAndPick, letterIds, new ContentId[0]);
            IReadOnlyList<Word> words = FirstSoundWords(lesson, item, choices);
            Step firstSound = words.Count > 0
                ? Make(lesson, item, ActivityKind.FirstSound, letterIds, new[] { words[random.NextInt(0, words.Count)].Id })
                : null;
            bool pickPlays = playable.Includes(pick);
            bool firstSoundPlays = firstSound != null && playable.Includes(firstSound);
            if (pickPlays && firstSoundPlays)
            {
                step = random.NextInt(0, 2) == 1 ? firstSound : pick;
            }
            else
            {
                step = firstSoundPlays ? firstSound : pickPlays ? pick : null;
            }

            return step != null;
        }

        private static Step Make(Lesson lesson, LearningItem item, ActivityKind kind, ContentId[] letters, ContentId[] words)
        {
            return new Step(lesson.Id, 0, kind, item.Language.Alone(), letters, words, new StepOption[0],
                new ContentSource(SourceName, 0));
        }

        private IEnumerable<Letter> Pick(List<Letter> from, int count)
        {
            List<Letter> pool = new List<Letter>(from);
            int taken = Math.Min(count, pool.Count);
            for (int index = 0; index < taken; index++)
            {
                int swap = random.NextInt(index, pool.Count);
                Letter held = pool[index];
                pool[index] = pool[swap];
                pool[swap] = held;
            }

            return pool.Take(taken);
        }

        private IReadOnlyList<Word> FirstSoundWords(Lesson upTo, LearningItem item, IReadOnlyList<Letter> choices)
        {
            IEnumerable<Lesson> before = curriculum.AllLessons.TakeWhile(lesson => lesson.Id != upTo.Id);
            return before
                .Concat(new[] { upTo })
                .Where(lesson => !lesson.Retired)
                .SelectMany(lesson => lesson.Steps)
                .Where(step => step.Kind == ActivityKind.FirstSound && step.Languages == item.Language.Alone())
                .Where(step => step.Words.Count > 0 && curriculum.Words.Contains(step.Words[0]))
                .Select(step => curriculum.Words.Get(step.Words[0]))
                .Where(word => !word.Retired && word.Language == item.Language)
                .Where(word => WordStarts.TryFind(word, choices, out Letter starts) && starts.Id == item.Id)
                .GroupBy(word => word.Id)
                .Select(group => group.First())
                .ToArray();
        }
    }
}
