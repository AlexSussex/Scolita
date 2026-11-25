using System;
using Scolita.Core.Content;
using Scolita.Core.Rewards;
using UnityEngine;

namespace Scolita.Game.Rewards
{
    /// <summary>A sticker on the scene as saved in <see cref="RewardsData"/>: its id, where it is and its size.</summary>
    [Serializable]
    public sealed class PlacedStickerData
    {
        [SerializeField] private string sticker;
        [SerializeField] private float x;
        [SerializeField] private float y;
        [SerializeField] private float size;

        /// <summary>An empty placement, for Unity's serialization.</summary>
        public PlacedStickerData()
        {
        }

        /// <summary>The stored form of <paramref name="placed"/>; called by <see cref="RewardsData"/>.</summary>
        public PlacedStickerData(PlacedSticker placed)
        {
            sticker = placed.StickerId.Value;
            x = placed.X;
            y = placed.Y;
            size = placed.Size;
        }

        /// <summary>The placement for Core; its id is unset when the text can't be one. Called by <see cref="RewardsData.ToBook"/>.</summary>
        public PlacedSticker ToPlaced()
        {
            ContentId id = ContentId.TryCreate(sticker, out ContentId read) ? read : default;
            return new PlacedSticker(id, x, y, size);
        }
    }
}
