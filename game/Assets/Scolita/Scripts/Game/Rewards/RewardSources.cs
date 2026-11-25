using Scolita.Content;
using Scolita.Core.Rewards;

namespace Scolita.Game.Rewards
{
    /// <summary>
    /// What the rewards are made of in a scene: the content (stickers, constellations, trails), the rules and the
    /// pictures. Built once by a scene's composition root from its serialized catalogs and rules asset.
    /// </summary>
    public sealed class RewardSources
    {
        private RewardSources(RewardContent content, RewardRules rules, RewardSprites sprites)
        {
            Content = content;
            Rules = rules;
            Sprites = sprites;
        }

        /// <summary>The stickers, the constellations and the trails.</summary>
        public RewardContent Content { get; }

        /// <summary>The numbers of the rewards.</summary>
        public RewardRules Rules { get; }

        /// <summary>The pictures of the stickers and the trails.</summary>
        public RewardSprites Sprites { get; }

        /// <summary>
        /// The sources of the three catalogs and the rules asset; throws an <see cref="System.InvalidOperationException"/>
        /// for assets that can't be read (an impossible state: they were validated at import). Called by
        /// <c>LessonRunner</c>, <c>MapRunner</c> and the tests.
        /// </summary>
        public static RewardSources Build(StickerCatalog stickers, ConstellationCatalog constellations, TrailCatalog trails,
            RewardRulesAsset rules)
        {
            return new RewardSources(RewardContentAssets.Build(stickers, constellations, trails), rules.ToRules(),
                new RewardSprites(stickers, trails));
        }
    }
}
