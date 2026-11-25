using System.Collections.Generic;
using Scolita.Core.Content;
using Scolita.Core.Rewards;
using Scolita.Game.Rewards;
using Scolita.UI.Map;
using UnityEngine;

namespace Scolita.UI.Rewards
{
    /// <summary>
    /// The reward screens of a scene (<c>Prefabs/Rewards/RewardBooks.prefab</c> and the map's bottom bar): the bar, the
    /// sky book, the sticker book and the trail picker, one book open at a time, and the rocket's trail on the map. A
    /// part the scene doesn't have is left empty (the lesson has only the sticker book).
    /// </summary>
    public sealed class RewardScreens : RewardsView
    {
        [SerializeField] private RewardBar bar;
        [SerializeField] private SkyBookView skyBook;
        [SerializeField] private StickerBookView stickerBook;
        [SerializeField] private TrailPickerView trailPicker;
        [SerializeField] private RocketView rocket;

        /// <summary>The map's bottom bar; null in the lesson.</summary>
        public RewardBar Bar => bar;

        /// <summary>The sky book; null in the lesson.</summary>
        public SkyBookView SkyBook => skyBook;

        /// <summary>The sticker book.</summary>
        public StickerBookView StickerBook => stickerBook;

        /// <summary>The trail picker; null in the lesson.</summary>
        public TrailPickerView TrailPicker => trailPicker;

        /// <inheritdoc/>
        public override bool IsBookOpen => IsOpen(skyBook) || IsOpen(stickerBook) || IsOpen(trailPicker);

        /// <inheritdoc/>
        public override void Bind(IRewardsInput input)
        {
            if (bar != null)
            {
                bar.Bind(input);
            }

            foreach (BookPage page in Pages())
            {
                page.Bind(input);
            }
        }

        /// <inheritdoc/>
        public override void ShowBar(int stars, bool hasStickers, bool hasTrails)
        {
            if (bar != null)
            {
                bar.Show(stars, hasStickers && stickerBook != null, hasTrails && trailPicker != null);
            }
        }

        /// <inheritdoc/>
        public override void ShowSkyBook(SkyChart sky)
        {
            if (skyBook != null)
            {
                CloseBooks();
                skyBook.Show(sky);
            }
        }

        /// <inheritdoc/>
        public override void ShowStickerBook(StickerShelf shelf)
        {
            if (stickerBook != null)
            {
                CloseBooks();
                stickerBook.Show(shelf);
            }
        }

        /// <inheritdoc/>
        public override void ShowTrails(IReadOnlyList<TrailChoice> trails, ContentId chosen)
        {
            if (trailPicker != null)
            {
                CloseBooks();
                trailPicker.Show(trails, chosen);
            }
        }

        /// <inheritdoc/>
        public override void ShowRocketTrail(Sprite trail)
        {
            if (rocket != null)
            {
                rocket.ShowTrail(trail);
            }
        }

        /// <inheritdoc/>
        public override void CloseBooks()
        {
            foreach (BookPage page in Pages())
            {
                page.Close();
            }
        }

        private static bool IsOpen(BookPage page)
        {
            return page != null && page.IsOpen;
        }

        private IEnumerable<BookPage> Pages()
        {
            if (skyBook != null)
            {
                yield return skyBook;
            }

            if (stickerBook != null)
            {
                yield return stickerBook;
            }

            if (trailPicker != null)
            {
                yield return trailPicker;
            }
        }

        private void Awake()
        {
            CloseBooks();
        }
    }
}
