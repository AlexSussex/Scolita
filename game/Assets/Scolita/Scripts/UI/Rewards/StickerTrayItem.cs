using Scolita.Core.Content;
using Scolita.Game.Rewards;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scolita.UI.Rewards
{
    /// <summary>
    /// A sticker in the book's strip: its picture and, with more than one copy left, how many. Dragging it up takes a
    /// copy out onto the scene (<see cref="StickerBookView"/>); dragging it sideways scrolls the strip. With no copy
    /// left it is shown faded and stays put.
    /// </summary>
    public sealed class StickerTrayItem : MonoBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image picture;
        [SerializeField] private TMP_Text copies;
        [SerializeField] private CanvasGroup fade;
        [SerializeField] private ScrollRect strip;
        [SerializeField, Range(0f, 1f)] private float fadedAlpha = 0.35f;

        private StickerBookView book;
        private ContentId sticker;
        private int copiesLeft;
        private bool takingOut;
        private bool scrolling;

        /// <summary>The sticker shown.</summary>
        public ContentId Sticker => sticker;

        /// <summary>Its picture.</summary>
        public Sprite Picture => picture.sprite;

        /// <summary>The copies not on the scene.</summary>
        public int CopiesLeft => copiesLeft;

        /// <summary>Shows <paramref name="shelfSticker"/> for <paramref name="owner"/>; called by <see cref="StickerBookView"/>.</summary>
        public void Show(ShelfSticker shelfSticker, StickerBookView owner)
        {
            book = owner;
            sticker = shelfSticker.Id;
            picture.sprite = shelfSticker.Picture;
            SetCopies(shelfSticker.CopiesLeft);
        }

        /// <summary>A copy went onto the scene; called by <see cref="StickerBookView"/>.</summary>
        public void CopyUsed()
        {
            SetCopies(copiesLeft - 1);
        }

        /// <summary>A copy came back from the scene; called by <see cref="StickerBookView"/>.</summary>
        public void CopyBack()
        {
            SetCopies(copiesLeft + 1);
        }

        /// <inheritdoc/>
        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            strip.OnInitializePotentialDrag(eventData);
        }

        /// <inheritdoc/>
        public void OnBeginDrag(PointerEventData eventData)
        {
            takingOut = copiesLeft > 0 && Mathf.Abs(eventData.delta.y) >= Mathf.Abs(eventData.delta.x);
            scrolling = !takingOut;
            if (takingOut)
            {
                book.BeginFromTray(this, eventData);
            }
            else
            {
                strip.OnBeginDrag(eventData);
            }
        }

        /// <inheritdoc/>
        public void OnDrag(PointerEventData eventData)
        {
            if (takingOut)
            {
                book.DragFromTray(eventData);
            }
            else if (scrolling)
            {
                strip.OnDrag(eventData);
            }
        }

        /// <inheritdoc/>
        public void OnEndDrag(PointerEventData eventData)
        {
            if (takingOut)
            {
                book.EndFromTray(eventData);
            }
            else if (scrolling)
            {
                strip.OnEndDrag(eventData);
            }

            takingOut = false;
            scrolling = false;
        }

        private void SetCopies(int left)
        {
            copiesLeft = Mathf.Max(0, left);
            copies.gameObject.SetActive(copiesLeft > 1);
            if (copiesLeft > 1)
            {
                copies.SetText("{0}", copiesLeft);
            }

            fade.alpha = copiesLeft > 0 ? 1f : fadedAlpha;
        }
    }
}
