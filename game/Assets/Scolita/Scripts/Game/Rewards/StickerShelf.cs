using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Rewards;
using UnityEngine;

namespace Scolita.Game.Rewards
{
    /// <summary>
    /// What the sticker book shows when it opens: the stickers she has (each with its picture and how many copies are
    /// not on the scene) and the stickers on the scene, in drawing order, with the sizes she can make them. Made by
    /// <see cref="RewardsSession"/>.
    /// </summary>
    public sealed class StickerShelf
    {
        /// <summary>Creates the shelf; called by <see cref="RewardsSession"/> and the tests.</summary>
        public StickerShelf(IReadOnlyList<ShelfSticker> owned, IReadOnlyList<ShelfSticker> placed, IReadOnlyList<PlacedSticker> placements,
            RewardRules rules)
        {
            Owned = owned;
            Placed = placed;
            Placements = placements;
            Rules = rules;
        }

        /// <summary>The stickers she has, each once, in the order first given.</summary>
        public IReadOnlyList<ShelfSticker> Owned { get; }

        /// <summary>The stickers on the scene, in drawing order (the pictures of <see cref="Placements"/>).</summary>
        public IReadOnlyList<ShelfSticker> Placed { get; }

        /// <summary>Where each sticker on the scene is, in drawing order.</summary>
        public IReadOnlyList<PlacedSticker> Placements { get; }

        /// <summary>The rules: the smallest and the largest a sticker can be made.</summary>
        public RewardRules Rules { get; }

        /// <summary>The shelf of <paramref name="book"/>, with <paramref name="sprites"/>' pictures; called by <see cref="RewardsSession"/>.</summary>
        public static StickerShelf Of(RewardBook book, RewardSprites sprites, RewardRules rules)
        {
            ShelfSticker[] owned = book.OwnedStickers
                .Select(id => new ShelfSticker(id, sprites.Sticker(id), book.CopiesLeft(id)))
                .ToArray();
            ShelfSticker[] placed = book.Placed
                .Select(sticker => new ShelfSticker(sticker.StickerId, sprites.Sticker(sticker.StickerId), 0))
                .ToArray();
            return new StickerShelf(owned, placed, book.Placed.ToArray(), rules);
        }
    }
}
