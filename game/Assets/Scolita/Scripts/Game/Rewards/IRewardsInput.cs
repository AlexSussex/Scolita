using Scolita.Core.Content;
using UnityEngine;

namespace Scolita.Game.Rewards
{
    /// <summary>
    /// Where the reward screens send her taps (the map's bottom bar, the sky book, the sticker book, the trail picker,
    /// the lesson's end); implemented by <see cref="RewardsSession"/>.
    /// </summary>
    public interface IRewardsInput
    {
        /// <summary>She tapped "Cerul meu": the sky book opens. Called by the bottom bar.</summary>
        void OpenSkyBook();

        /// <summary>She tapped "Abțibilduri" or "În cartea mea": the sticker book opens. Called by the bar and the lesson's end.</summary>
        void OpenStickerBook();

        /// <summary>She tapped "Dâre": the trail picker opens. Called by the bottom bar.</summary>
        void OpenTrails();

        /// <summary>She tapped a book's back button: it closes. Called by the books.</summary>
        void CloseBook();

        /// <summary>She tapped the constellation number <paramref name="index"/> of the sky book: its name is said. Called by the sky book.</summary>
        void TapConstellation(int index);

        /// <summary>
        /// She dropped a copy of <paramref name="sticker"/> on the scene at <paramref name="at"/> (0 to 1 across the
        /// scene, from its bottom left) and <paramref name="size"/>: true when it is placed (on top), false when no copy
        /// is left. Called by the sticker book.
        /// </summary>
        bool PlaceSticker(ContentId sticker, Vector2 at, float size);

        /// <summary>
        /// She let go of the placed sticker number <paramref name="index"/> at <paramref name="at"/> and
        /// <paramref name="size"/>: it stays there, on top. Called by the sticker book.
        /// </summary>
        void MoveSticker(int index, Vector2 at, float size);

        /// <summary>She dropped the placed sticker number <paramref name="index"/> back on the book: it leaves the scene. Called by the sticker book.</summary>
        void TakeOffSticker(int index);

        /// <summary>She tapped the trail <paramref name="trail"/>: the rocket wears it. Called by the trail picker.</summary>
        void ChooseTrail(ContentId trail);

        /// <summary>A star of the lesson's end landed in its constellation: its sound. Called by the lesson's end.</summary>
        void StarLanded();

        /// <summary>The gift box of the lesson's end opens on its sticker: its sound. Called by the lesson's end.</summary>
        void GiftOpened();
    }
}
