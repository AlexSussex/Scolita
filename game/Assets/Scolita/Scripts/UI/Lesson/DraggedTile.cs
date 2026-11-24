using UnityEngine;
using UnityEngine.EventSystems;

namespace Scolita.UI.Lesson
{
    /// <summary>
    /// Makes a <see cref="ChoiceTile"/> of <c>BuildWord</c> draggable: it follows her finger and, let go, is handed
    /// to its <see cref="BuildWordView"/> with where it was let go, then floats back home (the view hides it once
    /// it fills a window). A tap without a drag stays the tile's own (its sound). Its float home runs on plain
    /// arithmetic in <c>Update</c>, allocating nothing.
    /// </summary>
    public sealed class DraggedTile : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private RectTransform rect;
        [SerializeField] private ChoiceTile tile;
        [SerializeField] private float homeSeconds = 0.35f;

        private BuildWordView owner;
        private Vector2 home;
        private Vector2 from;
        private float homeLeft;
        private bool dragging;

        /// <summary>The tile it drags.</summary>
        public ChoiceTile Tile => tile;

        /// <summary>Whether it is away from home (dragged, or floating back).</summary>
        public bool IsAway => dragging || homeLeft > 0f;

        /// <summary>
        /// Puts it home at <paramref name="position"/>, for <paramref name="view"/>; called by
        /// <see cref="BuildWordView"/> when a step begins.
        /// </summary>
        public void Home(BuildWordView view, Vector2 position)
        {
            owner = view;
            home = position;
            dragging = false;
            homeLeft = 0f;
            rect.anchoredPosition = home;
        }

        /// <summary>Starts a drag; called by the EventSystem.</summary>
        public void OnBeginDrag(PointerEventData eventData)
        {
            dragging = true;
            homeLeft = 0f;
            rect.SetAsLastSibling();
            Follow(eventData);
        }

        /// <summary>Follows her finger; called by the EventSystem.</summary>
        public void OnDrag(PointerEventData eventData)
        {
            if (dragging)
            {
                Follow(eventData);
            }
        }

        /// <summary>Lets go: floats home, and tells the view where; called by the EventSystem.</summary>
        public void OnEndDrag(PointerEventData eventData)
        {
            if (!dragging)
            {
                return;
            }

            dragging = false;
            from = rect.anchoredPosition;
            homeLeft = homeSeconds;
            owner.TileLetGo(tile.Index, eventData.position, eventData.pressEventCamera);
        }

        private void Follow(PointerEventData eventData)
        {
            RectTransform parent = (RectTransform)rect.parent;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, eventData.position, eventData.pressEventCamera,
                out Vector2 local))
            {
                rect.anchoredPosition = local;
            }
        }

        private void Update()
        {
            if (homeLeft <= 0f)
            {
                return;
            }

            homeLeft = Mathf.Max(0f, homeLeft - Time.unscaledDeltaTime);
            float done = 1f - homeLeft / homeSeconds;
            rect.anchoredPosition = Vector2.Lerp(from, home, done * (2f - done));
        }
    }
}
