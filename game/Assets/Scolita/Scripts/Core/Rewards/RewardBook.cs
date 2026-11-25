using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;

namespace Scolita.Core.Rewards
{
    /// <summary>
    /// Her rewards, one book per profile (the <c>rewards</c> save): the stars earned (a total that only grows), the
    /// lessons whose stars are lit (in the order first lit), how many lessons she has finished, the stickers given (in
    /// order: one comes again only in a later round), the planets whose end was rewarded, the trails earned and the one
    /// chosen, and the stickers she put on the scene. Only <see cref="RewardGiver"/> adds to it, and nothing in it can be
    /// taken away. The sticker scene's rules are here: a sticker can be placed as many times as it was given.
    /// </summary>
    public sealed class RewardBook
    {
        private readonly List<ContentId> litLessons = new List<ContentId>();
        private readonly HashSet<ContentId> lit = new HashSet<ContentId>();
        private readonly List<ContentId> stickers = new List<ContentId>();
        private readonly List<ContentId> rewardedUnits = new List<ContentId>();
        private readonly List<ContentId> trails = new List<ContentId>();
        private readonly List<PlacedSticker> placed = new List<PlacedSticker>();

        /// <summary>
        /// A book with what was saved, brought into its rules: counts under 0 become 0, a lesson, planet or trail
        /// written twice counts once, the chosen trail must be one earned, a placement is kept inside the scene and
        /// only as many times as its sticker was given (the extra ones are dropped). Ids the content doesn't know are
        /// kept, and ignored. Called by the save (<c>RewardsData</c>), <see cref="RewardGiver.CatchUp"/> and the tests.
        /// </summary>
        public RewardBook(int starsEarned, IEnumerable<ContentId> litLessons, int lessonsFinished,
            IEnumerable<ContentId> stickers, IEnumerable<ContentId> rewardedUnits, IEnumerable<ContentId> trails,
            ContentId chosenTrail, IEnumerable<PlacedSticker> placed)
        {
            StarsEarned = Math.Max(0, starsEarned);
            LessonsFinished = Math.Max(0, lessonsFinished);
            foreach (ContentId lesson in litLessons ?? Enumerable.Empty<ContentId>())
            {
                AddLit(lesson);
            }

            this.stickers.AddRange((stickers ?? Enumerable.Empty<ContentId>()).Where(id => id.IsSet));
            this.rewardedUnits.AddRange((rewardedUnits ?? Enumerable.Empty<ContentId>()).Where(id => id.IsSet).Distinct());
            this.trails.AddRange((trails ?? Enumerable.Empty<ContentId>()).Where(id => id.IsSet).Distinct());
            ChosenTrail = chosenTrail.IsSet && this.trails.Contains(chosenTrail) ? chosenTrail : default;
            foreach (PlacedSticker sticker in placed ?? Enumerable.Empty<PlacedSticker>())
            {
                if (sticker.StickerId.IsSet && CopiesLeftOf(sticker.StickerId) > 0)
                {
                    this.placed.Add(Inside(sticker.StickerId, sticker.X, sticker.Y, sticker.Size));
                }
            }
        }

        /// <summary>Every star she has earned, a replayed lesson's too.</summary>
        public int StarsEarned { get; private set; }

        /// <summary>How many lessons she has finished, replays too.</summary>
        public int LessonsFinished { get; private set; }

        /// <summary>The lessons whose stars are lit, in the order first lit.</summary>
        public IReadOnlyList<ContentId> LitLessons => litLessons;

        /// <summary>The stickers given, in order (a sticker given in two rounds is here twice).</summary>
        public IReadOnlyList<ContentId> GivenStickers => stickers;

        /// <summary>The planets whose end was rewarded, in order.</summary>
        public IReadOnlyList<ContentId> RewardedUnits => rewardedUnits;

        /// <summary>The trails earned, in order.</summary>
        public IReadOnlyList<ContentId> Trails => trails;

        /// <summary>The trail the rocket wears; unset for none.</summary>
        public ContentId ChosenTrail { get; private set; }

        /// <summary>The stickers on the scene, in drawing order (the last on top).</summary>
        public IReadOnlyList<PlacedSticker> Placed => placed;

        /// <summary>The stickers she has, each once, in the order first given. Read by the sticker book when it opens.</summary>
        public IReadOnlyList<ContentId> OwnedStickers => stickers.Distinct().ToArray();

        /// <summary>An empty book: a new profile. Called by the save's defaults and the tests.</summary>
        public static RewardBook Empty()
        {
            return new RewardBook(0, null, 0, null, null, null, default, null);
        }

        /// <summary>
        /// Whether the lesson <paramref name="lessonId"/> has its stars lit; called by <see cref="RewardGiver"/>,
        /// <see cref="SkyChart"/> and the tests.
        /// </summary>
        public bool IsLit(ContentId lessonId)
        {
            return lit.Contains(lessonId);
        }

        /// <summary>How many copies of <paramref name="stickerId"/> are not on the scene; called by the sticker book.</summary>
        public int CopiesLeft(ContentId stickerId)
        {
            return CopiesLeftOf(stickerId);
        }

        /// <summary>
        /// Puts a copy of <paramref name="stickerId"/> on the scene at <paramref name="x"/>, <paramref name="y"/> (kept
        /// inside 0..1) and <paramref name="size"/> (kept within <paramref name="rules"/>), on top; false, with nothing
        /// changed, when no copy is left. Called by <c>RewardsSession</c> when she drops a sticker from the book.
        /// </summary>
        public bool TryPlace(ContentId stickerId, float x, float y, float size, RewardRules rules)
        {
            if (!stickerId.IsSet || CopiesLeftOf(stickerId) <= 0)
            {
                return false;
            }

            placed.Add(Inside(stickerId, x, y, rules.ClampSize(size)));
            return true;
        }

        /// <summary>
        /// Moves the placed sticker number <paramref name="index"/> to <paramref name="x"/>, <paramref name="y"/> at
        /// <paramref name="size"/> (kept inside the scene and within <paramref name="rules"/>), and brings it on top.
        /// Throws an <see cref="ArgumentOutOfRangeException"/> for an index that isn't placed. Called by
        /// <c>RewardsSession</c> when she lets go of a sticker she moved or resized.
        /// </summary>
        public void Move(int index, float x, float y, float size, RewardRules rules)
        {
            PlacedSticker moved = PlacedAt(index);
            placed.RemoveAt(index);
            placed.Add(Inside(moved.StickerId, x, y, rules.ClampSize(size)));
        }

        /// <summary>
        /// Takes the placed sticker number <paramref name="index"/> off the scene; it stays in the book. Throws an
        /// <see cref="ArgumentOutOfRangeException"/> for an index that isn't placed. Called by <c>RewardsSession</c>
        /// when she drops a sticker back on the book.
        /// </summary>
        public void TakeOff(int index)
        {
            PlacedAt(index);
            placed.RemoveAt(index);
        }

        /// <summary>
        /// Brings every placed sticker's size within <paramref name="rules"/> (a saved file knows no rules). Called by
        /// <c>RewardsSession</c> once her book is read.
        /// </summary>
        public void FitSizes(RewardRules rules)
        {
            for (int index = 0; index < placed.Count; index++)
            {
                PlacedSticker sticker = placed[index];
                placed[index] = new PlacedSticker(sticker.StickerId, sticker.X, sticker.Y, rules.ClampSize(sticker.Size));
            }
        }

        /// <summary>
        /// The rocket wears <paramref name="trailId"/>; false, with nothing changed, for a trail not earned. Called
        /// by <c>RewardsSession</c> from the trail picker.
        /// </summary>
        public bool ChooseTrail(ContentId trailId)
        {
            if (!trailId.IsSet || !trails.Contains(trailId))
            {
                return false;
            }

            ChosenTrail = trailId;
            return true;
        }

        /// <summary>Counts a finished lesson and adds its <paramref name="stars"/>; called by <see cref="RewardGiver"/>.</summary>
        internal void Finish(int stars)
        {
            LessonsFinished++;
            StarsEarned += stars;
        }

        /// <summary>Lights the lesson's stars; false when they were lit before. Called by <see cref="RewardGiver"/>.</summary>
        internal bool Light(ContentId lessonId)
        {
            return AddLit(lessonId);
        }

        /// <summary>Adds a sticker given; called by <see cref="RewardGiver"/>.</summary>
        internal void Give(ContentId stickerId)
        {
            stickers.Add(stickerId);
        }

        /// <summary>Records a planet's end as rewarded; called by <see cref="RewardGiver"/>.</summary>
        internal void RewardUnit(ContentId unitId)
        {
            rewardedUnits.Add(unitId);
        }

        /// <summary>Adds an earned trail, chosen at once when it is the first; called by <see cref="RewardGiver"/>.</summary>
        internal void Earn(ContentId trailId)
        {
            trails.Add(trailId);
            if (!ChosenTrail.IsSet)
            {
                ChosenTrail = trailId;
            }
        }

        private static PlacedSticker Inside(ContentId stickerId, float x, float y, float size)
        {
            bool sizeUsable = !float.IsNaN(size) && !float.IsInfinity(size) && size > 0f;
            return new PlacedSticker(stickerId, Fraction(x), Fraction(y), sizeUsable ? size : 1f);
        }

        private static float Fraction(float value)
        {
            return float.IsNaN(value) ? 0.5f : Math.Min(1f, Math.Max(0f, value));
        }

        private bool AddLit(ContentId lessonId)
        {
            if (!lessonId.IsSet || !lit.Add(lessonId))
            {
                return false;
            }

            litLessons.Add(lessonId);
            return true;
        }

        private int CopiesLeftOf(ContentId stickerId)
        {
            int left = 0;
            foreach (ContentId sticker in stickers)
            {
                if (sticker == stickerId)
                {
                    left++;
                }
            }

            foreach (PlacedSticker sticker in placed)
            {
                if (sticker.StickerId == stickerId)
                {
                    left--;
                }
            }

            return left;
        }

        private PlacedSticker PlacedAt(int index)
        {
            if (index < 0 || index >= placed.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "No sticker number " + index + " on the scene.");
            }

            return placed[index];
        }
    }
}
