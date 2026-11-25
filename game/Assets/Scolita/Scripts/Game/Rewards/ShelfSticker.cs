using Scolita.Core.Content;
using UnityEngine;

namespace Scolita.Game.Rewards
{
    /// <summary>A sticker on the shelf: its id, its picture (null when the content no longer has it) and the copies left.</summary>
    public readonly struct ShelfSticker
    {
        /// <summary>Creates the sticker's entry; called by <see cref="StickerShelf"/>.</summary>
        public ShelfSticker(ContentId id, Sprite picture, int copiesLeft)
        {
            Id = id;
            Picture = picture;
            CopiesLeft = copiesLeft;
        }

        /// <summary>The sticker's id.</summary>
        public ContentId Id { get; }

        /// <summary>Its picture; null for a sticker the content no longer has.</summary>
        public Sprite Picture { get; }

        /// <summary>The copies not on the scene.</summary>
        public int CopiesLeft { get; }
    }
}
