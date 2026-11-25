using Scolita.Core.Content;

namespace Scolita.Core.Rewards
{
    /// <summary>
    /// A sticker she put on the scene: which one, where (x and y from 0 to 1 across the scene, from its bottom
    /// left), and its size (1 as placed). Kept in <see cref="RewardBook.Placed"/>, in drawing order.
    /// </summary>
    public readonly struct PlacedSticker
    {
        /// <summary>Creates a placed sticker; called by <see cref="RewardBook"/>, the save and the tests.</summary>
        public PlacedSticker(ContentId stickerId, float x, float y, float size)
        {
            StickerId = stickerId;
            X = x;
            Y = y;
            Size = size;
        }

        /// <summary>The sticker's id.</summary>
        public ContentId StickerId { get; }

        /// <summary>From the scene's left (0) to its right (1).</summary>
        public float X { get; }

        /// <summary>From the scene's bottom (0) to its top (1).</summary>
        public float Y { get; }

        /// <summary>Its size, 1 as placed.</summary>
        public float Size { get; }
    }
}
