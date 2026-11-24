using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scolita.UI.Lesson
{
    /// <summary>
    /// <c>TwoSounds</c>' picture in her hand (design screen 4): a tilted card with the picture and a
    /// yellow ring, without its language badge (the badge would tell her which planet it goes to; she
    /// listens instead). She drags it with her finger; let go, it is handed to its
    /// <see cref="TwoSoundsView"/>, and floats back home unless it landed. A tap on it says its word
    /// again. Its float home and its wobble run on plain arithmetic in <c>Update</c>, allocating nothing.
    /// </summary>
    public sealed class HeldPicture : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        [SerializeField] private RectTransform rect;
        [SerializeField] private Image picture;
        [SerializeField] private float homeSeconds = 0.35f;
        [SerializeField] private float wobbleSeconds = 0.5f;
        [SerializeField] private float wobbleDegrees = 10f;
        [SerializeField] private float wobblesPerSecond = 6f;
        [SerializeField] private float restingDegrees = -6f;

        private TwoSoundsView owner;
        private Vector2 home;
        private Vector2 from;
        private float homeLeft;
        private float wobbleLeft;
        private bool dragging;

        /// <summary>Whether she is dragging it.</summary>
        public bool IsDragged => dragging;

        /// <summary>Whether it is at home (not dragged, not floating back).</summary>
        public bool IsHome => !dragging && homeLeft <= 0f;

        /// <summary>The picture shown.</summary>
        public Sprite Shown => picture.sprite;

        /// <summary>
        /// Remembers where home is and which view it belongs to; called by <see cref="TwoSoundsView"/> once,
        /// when it wakes.
        /// </summary>
        public void Bind(TwoSoundsView view)
        {
            owner = view;
            home = rect.anchoredPosition;
        }

        /// <summary>
        /// Shows <paramref name="sprite"/> at home, or hides it when <paramref name="sprite"/> is null; called by
        /// <see cref="TwoSoundsView"/> when the picture in play changes.
        /// </summary>
        public void Hold(Sprite sprite)
        {
            dragging = false;
            homeLeft = 0f;
            wobbleLeft = 0f;
            rect.anchoredPosition = home;
            rect.localRotation = Quaternion.Euler(0f, 0f, restingDegrees);
            picture.sprite = sprite;
            gameObject.SetActive(sprite != null);
        }

        /// <summary>Wobbles as it floats back (a miss); called by <see cref="TwoSoundsView"/>.</summary>
        public void Wobble()
        {
            wobbleLeft = wobbleSeconds;
        }

        /// <summary>Starts a drag; called by the EventSystem.</summary>
        public void OnBeginDrag(PointerEventData eventData)
        {
            dragging = true;
            homeLeft = 0f;
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
            owner.PictureLetGo(eventData.position, eventData.pressEventCamera);
        }

        /// <summary>Asks to hear its word again; called by the EventSystem.</summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!eventData.dragging)
            {
                owner.PictureTapped();
            }
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
            float delta = Time.unscaledDeltaTime;
            if (homeLeft > 0f)
            {
                homeLeft = Mathf.Max(0f, homeLeft - delta);
                float done = 1f - homeLeft / homeSeconds;
                rect.anchoredPosition = Vector2.Lerp(from, home, done * (2f - done));
            }

            if (wobbleLeft > 0f)
            {
                wobbleLeft = Mathf.Max(0f, wobbleLeft - delta);
                float share = wobbleLeft / wobbleSeconds;
                float angle = restingDegrees
                    + wobbleDegrees * share * Mathf.Sin((wobbleSeconds - wobbleLeft) * wobblesPerSecond * 2f * Mathf.PI);
                rect.localRotation = Quaternion.Euler(0f, 0f, angle);
            }
        }
    }
}
