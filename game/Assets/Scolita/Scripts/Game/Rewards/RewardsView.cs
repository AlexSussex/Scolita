using System.Collections.Generic;
using Scolita.Core.Content;
using Scolita.Core.Rewards;
using UnityEngine;

namespace Scolita.Game.Rewards
{
    /// <summary>
    /// The reward screens as <see cref="RewardsSession"/> sees them: the map's bottom bar, the sky book, the sticker
    /// book and scene, the trail picker and the rocket's trail. Drawn by <c>Scolita.UI.Rewards.RewardScreens</c>
    /// (<c>Prefabs/Rewards/RewardBooks.prefab</c>, in the map's and the lesson's screens; a part a scene doesn't have
    /// is left out).
    /// </summary>
    public abstract class RewardsView : MonoBehaviour
    {
        /// <summary>Whether a book (the sky, the stickers, the trails) is open.</summary>
        public abstract bool IsBookOpen { get; }

        /// <summary>Sends the screens' taps to <paramref name="input"/>; called once by <see cref="RewardsSession"/>.</summary>
        public abstract void Bind(IRewardsInput input);

        /// <summary>
        /// Shows the map's bottom bar: the sky book's button with <paramref name="stars"/>, the sticker book's once she
        /// <paramref name="hasStickers"/>, the trails' once she <paramref name="hasTrails"/>. Called by <see cref="RewardsSession"/>.
        /// </summary>
        public abstract void ShowBar(int stars, bool hasStickers, bool hasTrails);

        /// <summary>Opens the sky book on <paramref name="sky"/>; called by <see cref="RewardsSession"/>.</summary>
        public abstract void ShowSkyBook(SkyChart sky);

        /// <summary>Opens the sticker book on <paramref name="shelf"/>; called by <see cref="RewardsSession"/>.</summary>
        public abstract void ShowStickerBook(StickerShelf shelf);

        /// <summary>Opens the trail picker on <paramref name="trails"/>, <paramref name="chosen"/> ringed; called by <see cref="RewardsSession"/>.</summary>
        public abstract void ShowTrails(IReadOnlyList<TrailChoice> trails, ContentId chosen);

        /// <summary>Puts <paramref name="trail"/> behind the map's rocket (null for none); called by <see cref="RewardsSession"/>.</summary>
        public abstract void ShowRocketTrail(Sprite trail);

        /// <summary>Closes whichever book is open; called by <see cref="RewardsSession"/>.</summary>
        public abstract void CloseBooks();
    }
}
