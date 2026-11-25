using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Lessons;
using Scolita.Core.Map;
using Scolita.Core.Randomness;

namespace Scolita.Core.Rewards
{
    /// <summary>
    /// The one system that gives rewards (PRD §3.6; AGENTS.md → "no pressure and no tricks"): every finished lesson
    /// gives its stars whatever the hints; a sticker comes every <see cref="RewardRules.LessonsPerSticker"/> finished
    /// lessons and at a planet's end, never one she already has until she has them all; a planet's end gives the next
    /// trail. Nothing is ever taken away and nothing can be bought. Raises <see cref="RewardEarned"/> for each reward.
    /// </summary>
    public sealed class RewardGiver
    {
        private readonly RewardContent content;
        private readonly RewardRules rules;
        private readonly IRandom random;

        /// <summary>Creates the rewards' rules; called by <c>RewardsSession</c> and the tests.</summary>
        public RewardGiver(RewardContent content, RewardRules rules, IRandom random)
        {
            this.content = content ?? throw new ArgumentNullException(nameof(content));
            this.rules = rules ?? throw new ArgumentNullException(nameof(rules));
            this.random = random ?? throw new ArgumentNullException(nameof(random));
        }

        /// <summary>Raised for each reward given: the stars, then a trail, then a sticker. <c>sound-feel</c> listens.</summary>
        public event Action<RewardEarned> RewardEarned;

        /// <summary>
        /// Gives the rewards of <paramref name="lesson"/>, just finished, into <paramref name="book"/>: its stars (a
        /// replay's too; its places light once); when this finishes its planet for the first time (every lesson of the
        /// planet in <paramref name="order"/> has its stars), the next trail; and a sticker when the count of finished
        /// lessons is a multiple of <see cref="RewardRules.LessonsPerSticker"/> or the planet is finished. Called by
        /// <c>RewardsSession</c> at the lesson's end and the tests.
        /// </summary>
        public LessonRewards LessonFinished(RewardBook book, Lesson lesson, LessonOrder order)
        {
            book.Finish(rules.StarsPerLesson);
            book.Light(lesson.Id);
            Raise(new RewardEarned(RewardKind.Stars, lesson.Id, rules.StarsPerLesson, default));
            bool finishedPlanet = !book.RewardedUnits.Contains(lesson.UnitId) && IsPlanetLit(book, lesson.UnitId, order);
            Trail trail = null;
            if (finishedPlanet)
            {
                book.RewardUnit(lesson.UnitId);
                trail = NextTrail(book);
                if (trail != null)
                {
                    book.Earn(trail.Id);
                    Raise(new RewardEarned(RewardKind.Trail, lesson.Id, 0, trail.Id));
                }
            }

            Sticker sticker = null;
            if (finishedPlanet || book.LessonsFinished % rules.LessonsPerSticker == 0)
            {
                sticker = NextSticker(book);
                if (sticker != null)
                {
                    book.Give(sticker.Id);
                    Raise(new RewardEarned(RewardKind.Sticker, lesson.Id, 0, sticker.Id));
                }
            }

            return new LessonRewards(lesson, rules.StarsPerLesson, sticker, trail, finishedPlanet);
        }

        /// <summary>
        /// The book of a profile from before rewards: the lessons of <paramref name="order"/> that
        /// <paramref name="done"/> says she finished get their stars, and her finished planets their trails (no
        /// stickers for the past), so her sky matches her map. Raises nothing. Called by <c>RewardsFile</c> when a
        /// profile has no <c>rewards</c> file yet, and the tests.
        /// </summary>
        public RewardBook CatchUp(IDoneLessons done, LessonOrder order)
        {
            RewardBook book = RewardBook.Empty();
            foreach (Lesson lesson in order.Playable.Where(lesson => done.IsDone(lesson.Id)))
            {
                book.Finish(rules.StarsPerLesson);
                book.Light(lesson.Id);
            }

            foreach (ContentId unit in order.Playable.Select(lesson => lesson.UnitId).Distinct().ToArray())
            {
                if (!IsPlanetLit(book, unit, order))
                {
                    continue;
                }

                book.RewardUnit(unit);
                Trail trail = NextTrail(book);
                if (trail != null)
                {
                    book.Earn(trail.Id);
                }
            }

            return book;
        }

        private static bool IsPlanetLit(RewardBook book, ContentId unitId, LessonOrder order)
        {
            IEnumerable<Lesson> planetLessons = order.Playable.Where(lesson => lesson.UnitId == unitId);
            return planetLessons.Any() && planetLessons.All(lesson => book.IsLit(lesson.Id));
        }

        private Trail NextTrail(RewardBook book)
        {
            return content.Trails.All.FirstOrDefault(trail => !trail.Retired && !book.Trails.Contains(trail.Id));
        }

        private Sticker NextSticker(RewardBook book)
        {
            Sticker[] active = content.Stickers.All
                .Where(sticker => !sticker.Retired)
                .ToArray();
            if (active.Length == 0)
            {
                return null;
            }

            HashSet<ContentId> activeIds = new HashSet<ContentId>(active.Select(sticker => sticker.Id));
            HashSet<ContentId> givenThisRound = new HashSet<ContentId>();
            foreach (ContentId given in book.GivenStickers.Where(activeIds.Contains))
            {
                givenThisRound.Add(given);
                if (givenThisRound.Count == activeIds.Count)
                {
                    givenThisRound.Clear();
                }
            }

            Sticker[] left = active
                .Where(sticker => !givenThisRound.Contains(sticker.Id))
                .ToArray();
            return left[random.NextInt(0, left.Length)];
        }

        private void Raise(RewardEarned earned)
        {
            RewardEarned?.Invoke(earned);
        }
    }
}
