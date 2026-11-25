using System.Collections.Generic;
using Scolita.Core.Content;
using Scolita.Core.Rewards;
using Scolita.Game.Rewards;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scolita.UI.Rewards
{
    /// <summary>
    /// The sticker book and its scene: the Earth on the purple sky, where she drags her stickers from the book (the strip
    /// along the bottom), moves them, makes them bigger or smaller with two fingers, and drags them back to the book to
    /// take them off. Every change is sent to <see cref="IRewardsInput"/> when she lets go (and saved there). The stickers
    /// on the scene keep the book's drawing order (the last moved on top). Views are made when it opens and reused; a
    /// drag or a pinch moves cached transforms, allocating nothing.
    /// </summary>
    public sealed class StickerBookView : BookPage
    {
        [SerializeField] private RectTransform scene;
        [SerializeField] private RectTransform tray;
        [SerializeField] private RectTransform placedLayer;
        [SerializeField] private StickerTrayItem trayTemplate;
        [SerializeField] private PlacedStickerView placedTemplate;
        [SerializeField, Range(0.05f, 0.5f)] private float stickerShareOfScene = 0.2f;

        private readonly List<StickerTrayItem> trayItems = new List<StickerTrayItem>();
        private readonly List<PlacedStickerView> placed = new List<PlacedStickerView>();
        private readonly List<PlacedStickerView> spare = new List<PlacedStickerView>();
        private RewardRules rules;
        private PlacedStickerView fromTray;
        private StickerTrayItem fromTrayItem;

        /// <summary>The book's stickers, in the order first given.</summary>
        public IReadOnlyList<StickerTrayItem> TrayItems => trayItems;

        /// <summary>The stickers on the scene, in drawing order.</summary>
        public IReadOnlyList<PlacedStickerView> Placed => placed;

        /// <summary>The scene.</summary>
        public RectTransform Scene => scene;

        /// <summary>The book's strip.</summary>
        public RectTransform Tray => tray;

        /// <summary>A sticker's side on the scene at size 1, in the scene's units.</summary>
        public float BaseSide => scene.rect.height * stickerShareOfScene;

        /// <summary>Opens the book on <paramref name="shelf"/>; called by <see cref="RewardScreens"/>.</summary>
        public void Show(StickerShelf shelf)
        {
            Open();
            rules = shelf.Rules;
            for (int index = 0; index < shelf.Owned.Count; index++)
            {
                if (index >= trayItems.Count)
                {
                    trayItems.Add(Instantiate(trayTemplate, trayTemplate.transform.parent));
                }

                ShelfSticker sticker = shelf.Owned[index];
                trayItems[index].gameObject.SetActive(sticker.Picture != null);
                trayItems[index].Show(sticker, this);
            }

            for (int index = shelf.Owned.Count; index < trayItems.Count; index++)
            {
                trayItems[index].gameObject.SetActive(false);
            }

            while (placed.Count > 0)
            {
                Release(placed[placed.Count - 1]);
                placed.RemoveAt(placed.Count - 1);
            }

            for (int index = 0; index < shelf.Placements.Count; index++)
            {
                PlacedSticker placement = shelf.Placements[index];
                PlacedStickerView view = Take();
                view.Show(shelf.Placed[index].Id, shelf.Placed[index].Picture, this);
                view.SetPlace(PointIn(placement.X, placement.Y), placement.Size);
                placed.Add(view);
            }
        }

        /// <summary>
        /// She starts dragging <paramref name="item"/> out of the book: a copy follows her finger. Called by
        /// <see cref="StickerTrayItem"/>.
        /// </summary>
        public void BeginFromTray(StickerTrayItem item, PointerEventData eventData)
        {
            if (fromTray != null)
            {
                return;
            }

            fromTrayItem = item;
            fromTray = Take();
            fromTray.Show(item.Sticker, item.Picture, this);
            fromTray.SetPlace(LocalPoint(eventData), 1f);
        }

        /// <summary>The copy from the book follows her finger; called by <see cref="StickerTrayItem"/>.</summary>
        public void DragFromTray(PointerEventData eventData)
        {
            if (fromTray != null)
            {
                fromTray.SetPlace(LocalPoint(eventData), 1f);
            }
        }

        /// <summary>
        /// She lets go of the copy from the book: on the scene it is placed (if a copy is left), anywhere else it goes
        /// back. Called by <see cref="StickerTrayItem"/>.
        /// </summary>
        public void EndFromTray(PointerEventData eventData)
        {
            if (fromTray == null)
            {
                return;
            }

            PlacedStickerView view = fromTray;
            StickerTrayItem item = fromTrayItem;
            fromTray = null;
            fromTrayItem = null;
            Vector2 at = ShareOf(view.Place);
            if (IsOverScene(eventData) && Input != null && Input.PlaceSticker(item.Sticker, at, 1f))
            {
                placed.Add(view);
                item.CopyUsed();
                return;
            }

            Release(view);
        }

        /// <summary>
        /// She let go of the placed <paramref name="view"/>: over the book it leaves the scene, elsewhere it stays where
        /// it is and comes on top. Called by <see cref="PlacedStickerView"/>.
        /// </summary>
        public void LetGo(PlacedStickerView view, PointerEventData eventData)
        {
            int index = placed.IndexOf(view);
            if (index < 0 || Input == null)
            {
                return;
            }

            placed.RemoveAt(index);
            if (IsOverTray(eventData))
            {
                Input.TakeOffSticker(index);
                CopyBack(view.Sticker);
                Release(view);
                return;
            }

            Vector2 at = ShareOf(view.Place);
            float size = rules == null ? view.Size : rules.ClampSize(view.Size);
            Input.MoveSticker(index, at, size);
            view.SetPlace(PointIn(Mathf.Clamp01(at.x), Mathf.Clamp01(at.y)), size);
            view.transform.SetAsLastSibling();
            placed.Add(view);
        }

        /// <summary>A size kept within the rules (while she pinches); called by <see cref="PlacedStickerView"/>.</summary>
        public float Clamp(float size)
        {
            return rules == null ? size : rules.ClampSize(size);
        }

        /// <summary>The point of <paramref name="eventData"/> in the scene's units; called by <see cref="PlacedStickerView"/>.</summary>
        public Vector2 LocalPoint(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(scene, eventData.position, eventData.pressEventCamera, out Vector2 point);
            return point;
        }

        private void CopyBack(ContentId sticker)
        {
            foreach (StickerTrayItem item in trayItems)
            {
                if (item.Sticker == sticker)
                {
                    item.CopyBack();
                    return;
                }
            }
        }

        private bool IsOverScene(PointerEventData eventData)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(scene, eventData.position, eventData.pressEventCamera)
                && !IsOverTray(eventData);
        }

        private bool IsOverTray(PointerEventData eventData)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(tray, eventData.position, eventData.pressEventCamera);
        }

        private Vector2 ShareOf(Vector2 point)
        {
            Rect rect = scene.rect;
            return new Vector2((point.x - rect.xMin) / rect.width, (point.y - rect.yMin) / rect.height);
        }

        private Vector2 PointIn(float x, float y)
        {
            Rect rect = scene.rect;
            return new Vector2(rect.xMin + rect.width * x, rect.yMin + rect.height * y);
        }

        private PlacedStickerView Take()
        {
            PlacedStickerView view;
            if (spare.Count > 0)
            {
                view = spare[spare.Count - 1];
                spare.RemoveAt(spare.Count - 1);
            }
            else
            {
                view = Instantiate(placedTemplate, placedLayer);
            }

            view.gameObject.SetActive(true);
            view.transform.SetAsLastSibling();
            return view;
        }

        private void Release(PlacedStickerView view)
        {
            view.gameObject.SetActive(false);
            spare.Add(view);
        }

        private void Awake()
        {
            trayTemplate.gameObject.SetActive(false);
            placedTemplate.gameObject.SetActive(false);
        }
    }
}
