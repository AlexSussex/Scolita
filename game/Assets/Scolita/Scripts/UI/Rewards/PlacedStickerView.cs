using Scolita.Core.Content;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scolita.UI.Rewards
{
    /// <summary>
    /// A sticker on the scene: one finger moves it, two fingers make it bigger or smaller (and move it by their middle),
    /// and letting go tells <see cref="StickerBookView"/>, which saves it or, over the book, takes it off. Its fingers
    /// are kept in fields, so a drag or a pinch allocates nothing.
    /// </summary>
    public sealed class PlacedStickerView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IInitializePotentialDragHandler,
        IBeginDragHandler, IDragHandler
    {
        private const int NoFinger = int.MinValue;

        [SerializeField] private RectTransform body;
        [SerializeField] private Image picture;

        private StickerBookView book;
        private ContentId sticker;
        private Vector2 place;
        private float size = 1f;
        private int firstFinger = NoFinger;
        private int secondFinger = NoFinger;
        private Vector2 firstPoint;
        private Vector2 secondPoint;
        private Vector2 startPlace;
        private Vector2 startMiddle;
        private float startSize;
        private float startDistance;

        /// <summary>The sticker shown.</summary>
        public ContentId Sticker => sticker;

        /// <summary>Its centre, in the scene's units.</summary>
        public Vector2 Place => place;

        /// <summary>Its size, 1 as placed.</summary>
        public float Size => size;

        /// <summary>Whether two fingers hold it.</summary>
        public bool IsPinched => firstFinger != NoFinger && secondFinger != NoFinger;

        /// <summary>Shows <paramref name="stickerId"/>'s <paramref name="sprite"/> for <paramref name="owner"/>; called by <see cref="StickerBookView"/>.</summary>
        public void Show(ContentId stickerId, Sprite sprite, StickerBookView owner)
        {
            book = owner;
            sticker = stickerId;
            picture.sprite = sprite;
            picture.enabled = sprite != null;
            firstFinger = NoFinger;
            secondFinger = NoFinger;
        }

        /// <summary>Puts it at <paramref name="at"/> (the scene's units) and <paramref name="newSize"/>; called by <see cref="StickerBookView"/>.</summary>
        public void SetPlace(Vector2 at, float newSize)
        {
            place = at;
            size = newSize;
            body.anchoredPosition = at;
            float side = book.BaseSide * newSize;
            body.sizeDelta = new Vector2(side, side);
        }

        /// <inheritdoc/>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (firstFinger == NoFinger)
            {
                firstFinger = eventData.pointerId;
                firstPoint = book.LocalPoint(eventData);
                StartMove();
            }
            else if (secondFinger == NoFinger && eventData.pointerId != firstFinger)
            {
                secondFinger = eventData.pointerId;
                secondPoint = book.LocalPoint(eventData);
                StartMove();
            }
        }

        /// <inheritdoc/>
        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }

        /// <inheritdoc/>
        public void OnBeginDrag(PointerEventData eventData)
        {
            transform.SetAsLastSibling();
        }

        /// <inheritdoc/>
        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerId == firstFinger)
            {
                firstPoint = book.LocalPoint(eventData);
            }
            else if (eventData.pointerId == secondFinger)
            {
                secondPoint = book.LocalPoint(eventData);
            }
            else
            {
                return;
            }

            if (IsPinched)
            {
                float distance = Vector2.Distance(firstPoint, secondPoint);
                float pinched = startDistance > 1f ? book.Clamp(startSize * distance / startDistance) : startSize;
                SetPlace(startPlace + (Middle() - startMiddle), pinched);
                return;
            }

            SetPlace(startPlace + (firstPoint - startMiddle), size);
        }

        /// <inheritdoc/>
        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId == secondFinger)
            {
                secondFinger = NoFinger;
            }
            else if (eventData.pointerId == firstFinger)
            {
                firstFinger = secondFinger;
                firstPoint = secondPoint;
                secondFinger = NoFinger;
            }
            else
            {
                return;
            }

            if (firstFinger == NoFinger)
            {
                book.LetGo(this, eventData);
                return;
            }

            StartMove();
        }

        private void StartMove()
        {
            startPlace = place;
            startSize = size;
            startMiddle = IsPinched ? Middle() : firstPoint;
            startDistance = IsPinched ? Vector2.Distance(firstPoint, secondPoint) : 0f;
        }

        private Vector2 Middle()
        {
            return (firstPoint + secondPoint) * 0.5f;
        }
    }
}
