using System.Collections.Generic;
using Scolita.Content;
using Scolita.Core.Content;
using UnityEngine;

namespace Scolita.Game.Rewards
{
    /// <summary>The pictures of the stickers and the trails, by id, from their catalogs; made once per scene by <see cref="RewardSources"/>.</summary>
    public sealed class RewardSprites
    {
        private readonly Dictionary<ContentId, Sprite> stickers = new Dictionary<ContentId, Sprite>();
        private readonly Dictionary<ContentId, Sprite> trails = new Dictionary<ContentId, Sprite>();

        /// <summary>The pictures of <paramref name="stickerCatalog"/> and <paramref name="trailCatalog"/>; called by <see cref="RewardSources"/>.</summary>
        public RewardSprites(StickerCatalog stickerCatalog, TrailCatalog trailCatalog)
        {
            foreach (StickerAsset sticker in stickerCatalog.Items)
            {
                stickers[ContentId.Create(sticker.Id)] = sticker.Sprite;
            }

            foreach (TrailAsset trail in trailCatalog.Items)
            {
                trails[ContentId.Create(trail.Id)] = trail.Sprite;
            }
        }

        /// <summary>The sticker's picture; null for one the content doesn't have (it isn't shown). Called by the sticker book and the lesson's end.</summary>
        public Sprite Sticker(ContentId id)
        {
            return stickers.TryGetValue(id, out Sprite sprite) ? sprite : null;
        }

        /// <summary>The trail's picture; null for one the content doesn't have (the rocket wears none). Called by the map and the trail picker.</summary>
        public Sprite Trail(ContentId id)
        {
            return id.IsSet && trails.TryGetValue(id, out Sprite sprite) ? sprite : null;
        }
    }
}
