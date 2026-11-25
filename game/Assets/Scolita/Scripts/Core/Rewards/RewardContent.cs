using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Profiles;

namespace Scolita.Core.Rewards
{
    /// <summary>
    /// The rewards' content, beside the <see cref="Curriculum"/>: the stickers, the planets' constellations and
    /// the rocket's trails, read from <c>stickers.csv</c>, <c>constellations.csv</c> and <c>trails.csv</c>
    /// (<see cref="RewardTables"/>) or from their assets in the app; and, read with them, the profile pictures
    /// (<c>avatars.csv</c>), which the app reads from their own catalog.
    /// </summary>
    public sealed class RewardContent
    {
        private readonly Dictionary<ContentId, Constellation> constellationsByUnit = new Dictionary<ContentId, Constellation>();

        private RewardContent(ItemCatalog<Sticker> stickers, ItemCatalog<Constellation> constellations, ItemCatalog<Trail> trails,
            ItemCatalog<ProfileAvatar> avatars)
        {
            Avatars = avatars;
            Stickers = stickers;
            Constellations = constellations;
            Trails = trails;
            foreach (Constellation constellation in constellations.All.Where(constellation => !constellation.Retired))
            {
                if (!constellationsByUnit.ContainsKey(constellation.UnitId))
                {
                    constellationsByUnit.Add(constellation.UnitId, constellation);
                }
            }
        }

        /// <summary>Every sticker, in the order read.</summary>
        public ItemCatalog<Sticker> Stickers { get; }

        /// <summary>Every constellation, in the order read.</summary>
        public ItemCatalog<Constellation> Constellations { get; }

        /// <summary>Every trail, in the order they are earned.</summary>
        public ItemCatalog<Trail> Trails { get; }

        /// <summary>Every profile picture, in the order offered (none in the app's rewards, which don't use them).</summary>
        public ItemCatalog<ProfileAvatar> Avatars { get; }

        /// <summary>
        /// The content of <paramref name="stickers"/>, <paramref name="constellations"/>, <paramref name="trails"/> and
        /// <paramref name="avatars"/>, reporting into <paramref name="problems"/> every id used twice in one of them (the
        /// second one is left out). Called by <see cref="RewardTables"/>, the reward assets' loader and the tests.
        /// </summary>
        public static RewardContent Build(IEnumerable<Sticker> stickers, IEnumerable<Constellation> constellations,
            IEnumerable<Trail> trails, IEnumerable<ProfileAvatar> avatars, ContentProblems problems)
        {
            return new RewardContent(
                new ItemCatalog<Sticker>(Distinct(stickers, "sticker", problems)),
                new ItemCatalog<Constellation>(Distinct(constellations, "constellation", problems)),
                new ItemCatalog<Trail>(Distinct(trails, "trail", problems)),
                new ItemCatalog<ProfileAvatar>(Distinct(avatars, "avatar", problems)));
        }

        /// <summary>
        /// The constellation of the unit <paramref name="unitId"/> that isn't retired (the first one, when the
        /// content wrongly has two); false when it has none. Called by <see cref="SkyChart"/>, <see cref="RewardGiver"/>
        /// and the content rules.
        /// </summary>
        public bool TryGetConstellation(ContentId unitId, out Constellation constellation)
        {
            return constellationsByUnit.TryGetValue(unitId, out constellation);
        }

        private static TItem[] Distinct<TItem>(IEnumerable<TItem> items, string what, ContentProblems problems)
            where TItem : IContentItem
        {
            Dictionary<ContentId, TItem> seen = new Dictionary<ContentId, TItem>();
            List<TItem> distinct = new List<TItem>();
            foreach (TItem item in items)
            {
                if (seen.TryGetValue(item.Id, out TItem first))
                {
                    problems.Error(item.Source, "the " + what + " id " + item.Id + " is already used (" + first.Source
                        + "): an id is used once and never reused.");
                    continue;
                }

                seen.Add(item.Id, item);
                distinct.Add(item);
            }

            return distinct.ToArray();
        }
    }
}
