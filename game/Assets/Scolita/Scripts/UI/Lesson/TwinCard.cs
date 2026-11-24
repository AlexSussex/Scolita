using Scolita.Core.Activities;
using Scolita.Game.Lessons;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scolita.UI.Lesson
{
    /// <summary>
    /// One word card of <c>WordTwins</c>, on the same object as its <see cref="ChoiceTile"/> (which takes her taps):
    /// a word she can read is written, its picture beside it once revealed; a listen-only word is its picture. A
    /// yellow ring shows the word in play. A line dragged from it is handed to its <see cref="WordTwinsView"/>,
    /// which joins it to the card it ends on.
    /// </summary>
    public sealed class TwinCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private RectTransform rect;
        [SerializeField] private ChoiceTile tile;
        [SerializeField] private RectTransform label;
        [SerializeField] private RectTransform picture;
        [SerializeField] private Image ring;
        [SerializeField] private float pictureBesideTheWord = 96f;
        [SerializeField] private float pictureAlone = 104f;

        private WordTwinsView owner;
        private bool written;
        private bool revealed;

        /// <summary>The card's tile (its taps, its feedback).</summary>
        public ChoiceTile Tile => tile;

        /// <summary>The card's place on the board.</summary>
        public RectTransform Rect => rect;

        /// <summary>Whether the word is in play (its ring shown).</summary>
        public bool IsSelected => ring.enabled;

        /// <summary>
        /// Shows <paramref name="choice"/> as choice <paramref name="index"/> at <paramref name="position"/>, its
        /// picture <paramref name="sprite"/>, sending taps to <paramref name="input"/> and lines to
        /// <paramref name="view"/>; called by <see cref="WordTwinsView"/> when a step begins.
        /// </summary>
        public void Show(WordTwinsView view, IActivityInput input, int index, ActivityChoice choice, Sprite sprite, Vector2 position)
        {
            owner = view;
            written = choice.Look == ActivityChoiceLook.Word;
            revealed = false;
            rect.anchoredPosition = position;
            tile.Show(index, choice, written ? choice.Text : string.Empty, sprite, input);
            ring.enabled = false;
            Arrange();
        }

        /// <summary>
        /// Shows whether the word is in play, and its picture once <paramref name="reveal"/> is true (it stays
        /// revealed); called by <see cref="WordTwinsView"/> after every answer.
        /// </summary>
        public void Refresh(bool selected, bool reveal)
        {
            ring.enabled = selected;
            if (reveal && !revealed)
            {
                revealed = true;
                Arrange();
            }
        }

        /// <summary>Starts a line from it; called by the EventSystem.</summary>
        public void OnBeginDrag(PointerEventData eventData)
        {
            owner.LineStarted(tile.Index, eventData.position, eventData.pressEventCamera);
        }

        /// <summary>Draws the line to her finger; called by the EventSystem.</summary>
        public void OnDrag(PointerEventData eventData)
        {
            owner.LineMoved(tile.Index, eventData.position, eventData.pressEventCamera);
        }

        /// <summary>Lets the line go; called by the EventSystem.</summary>
        public void OnEndDrag(PointerEventData eventData)
        {
            owner.LineLetGo(tile.Index, eventData.position, eventData.pressEventCamera);
        }

        private void Arrange()
        {
            bool pictureShown = !written || revealed;
            tile.Restyle(tile.ShownText, pictureShown);
            if (!written)
            {
                picture.anchoredPosition = Vector2.zero;
                picture.sizeDelta = new Vector2(pictureAlone, pictureAlone);
                return;
            }

            float half = rect.rect.width / 4f;
            picture.anchoredPosition = new Vector2(-half, 0f);
            picture.sizeDelta = new Vector2(pictureBesideTheWord, pictureBesideTheWord);
            label.anchoredPosition = new Vector2(revealed ? half / 2f : 0f, label.anchoredPosition.y);
        }
    }
}
