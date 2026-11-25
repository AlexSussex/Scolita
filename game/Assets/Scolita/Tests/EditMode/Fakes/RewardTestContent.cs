using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Activities;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;
using Scolita.Core.Rewards;

namespace Scolita.Tests.EditMode.Fakes
{
    /// <summary>
    /// Rewards for the reward tests, beside the real curriculum: stickers <c>s1</c> … <c>sN</c>, a triangle
    /// constellation per real planet (<c>planet-1</c> to <c>planet-3</c>) and the three trails, with the rules of the
    /// rules asset. Used by <c>RewardGiverTests</c>, <c>SkyChartTests</c> and <c>RewardBookTests</c>.
    /// </summary>
    public static class RewardTestContent
    {
        /// <summary>The trails, in the order they are earned.</summary>
        public static readonly string[] TrailIds = { "rainbow", "stars", "bubbles" };

        /// <summary>
        /// The rewards with <paramref name="stickerCount"/> stickers, <paramref name="retiredStickers"/> of them retired;
        /// used by the reward tests.
        /// </summary>
        public static RewardContent Of(int stickerCount, params string[] retiredStickers)
        {
            ContentSource source = new ContentSource("test", 0);
            IEnumerable<Sticker> stickers = Enumerable.Range(1, stickerCount)
                .Select(number => "s" + number)
                .Select(id => new Sticker(ContentId.Create(id), retiredStickers.Contains(id), source));
            IEnumerable<Constellation> constellations = new[] { "planet-1", "planet-2", "planet-3" }
                .Select(unit => new Constellation(ContentId.Create("sky-" + unit), ContentId.Create(unit), Triangle(),
                    new[] { new StarLine(0, 1), new StarLine(1, 2), new StarLine(2, 0) }, false, source));
            IEnumerable<Trail> trails = TrailIds.Select(id => new Trail(ContentId.Create(id), false, source));
            return RewardContent.Build(stickers, constellations, trails, new ContentProblems());
        }

        /// <summary>The rules: 3 stars a lesson, a sticker every second lesson, sizes from 0.5 to 3; used by the reward tests.</summary>
        public static RewardRules Rules()
        {
            return new RewardRules(3, 2, 0.5f, 3f);
        }

        /// <summary>The real content's lessons in both languages, every kind playable; used by the reward tests.</summary>
        public static LessonOrder Order(Curriculum curriculum)
        {
            return new LessonOrder(curriculum.Domains[0], LanguageMix.Both, new PlayableSteps(ActivityRulesFactory.Kinds, curriculum));
        }

        /// <summary>The lessons of <paramref name="unit"/> in <paramref name="order"/>; used by the reward tests.</summary>
        public static Lesson[] LessonsOf(LessonOrder order, string unit)
        {
            return order.Playable
                .Where(lesson => lesson.UnitId == ContentId.Create(unit))
                .ToArray();
        }

        private static StarPoint[] Triangle()
        {
            return new[] { new StarPoint(100f, 100f), new StarPoint(400f, 100f), new StarPoint(100f, 400f) };
        }
    }
}
