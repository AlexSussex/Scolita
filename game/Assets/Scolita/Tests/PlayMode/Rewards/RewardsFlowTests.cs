using System.Collections;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Profiles;
using Scolita.Core.Rewards;
using Scolita.Game.Lessons;
using Scolita.Game.Map;
using Scolita.Game.Progress;
using Scolita.Game.Rewards;
using Scolita.Game.Saving;
using Scolita.Game.Settings;
using Scolita.Tests.PlayMode.Lessons;
using Scolita.Tests.PlayMode.Map;
using Scolita.UI.Lesson;
using Scolita.UI.Map;
using Scolita.UI.Rewards;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;

namespace Scolita.Tests.PlayMode.Rewards
{
    /// <summary>
    /// The rewards in the scenes: a finished lesson lights its stars in the constellation and gives its sticker; the
    /// map's bottom bar opens the sky book, the sticker book and the trail picker; a sticker placed, moved and resized on
    /// the scene is saved and still there when the map opens again. Each test keeps the Editor's own <c>child-1</c>
    /// files as they were.
    /// </summary>
    public sealed class RewardsFlowTests
    {
        private static readonly string[] Files =
        {
            LessonRunData.FileName, MapProgressData.FileName, ProgressData.FileName, LearningSettingsData.FileName, RewardsData.FileName
        };

        /// <summary>
        /// Tests that finishing a lesson (the second one: a sticker comes) lights its three stars in the planet's
        /// constellation as they land, saves its stars and its sticker, and "În cartea mea" opens the sticker book.
        /// </summary>
        [UnityTest]
        public IEnumerator TestLessonEnd_LightsItsStarsAndGivesASticker()
        {
            string[] kept = Files.Select(LessonScene.KeepFile).ToArray();
            try
            {
                LessonScene.SaveStartingFiles("ro-t");
                SaveRewards(new RewardBook(3, new[] { ContentId.Create("ro-a") }, 1, null, null, null, default, null));
                yield return LessonScene.Open();
                LessonRunner runner = SceneSearch.FindInActiveScene<LessonRunner>();
                LessonScreen screen = SceneSearch.FindInActiveScene<LessonScreen>();

                yield return LessonScene.FinishLesson(runner);
                LessonEndScreen end = (LessonEndScreen)screen.End;
                yield return LessonScene.WaitUntil(() => end.AllLanded);

                RewardBook saved = SavedRewards();
                Assert.That(screen.IsEndShown, Is.True);
                Assert.That(saved.IsLit(ContentId.Create("ro-t")), Is.True);
                Assert.That(saved.StarsEarned, Is.EqualTo(6));
                Assert.That(saved.GivenStickers.Count, Is.EqualTo(1));
                Assert.That(Enumerable.Range(0, end.Constellation.PlaceCount).Count(end.Constellation.IsLit), Is.EqualTo(6));
                Assert.That(end.BookButton.gameObject.activeSelf, Is.True);
                end.BookButton.onClick.Invoke();
                RewardScreens books = (RewardScreens)screen.Rewards;
                Assert.That(books.StickerBook.IsOpen, Is.True);
                Assert.That(books.StickerBook.TrayItems.Count(item => item.gameObject.activeSelf), Is.EqualTo(1));
            }
            finally
            {
                Restore(kept);
            }
        }

        /// <summary>
        /// Tests that the map's bar shows her stars and opens the sky book (a constellation per planet, her lessons'
        /// places lit) and the trail picker (her trail ringed, worn by the rocket); each closes with its back button.
        /// </summary>
        [UnityTest]
        public IEnumerator TestMapBar_OpensTheSkyBookAndTheTrails()
        {
            string[] kept = Files.Select(LessonScene.KeepFile).ToArray();
            try
            {
                MapScene.SaveProgress(null, "ro-a");
                ContentId rainbow = ContentId.Create("rainbow");
                SaveRewards(new RewardBook(3, new[] { ContentId.Create("ro-a") }, 1, null, null, new[] { rainbow }, rainbow, null));
                yield return MapScene.Open();
                MapScreen map = MapScene.Screen();
                RewardScreens books = (RewardScreens)map.Rewards;

                Assert.That(books.Bar.StarCount, Is.EqualTo("3"));
                Assert.That(books.Bar.StickerButton.gameObject.activeSelf, Is.False, "no sticker yet");
                Assert.That(books.Bar.TrailButton.gameObject.activeSelf, Is.True);
                Assert.That(map.Rocket.Trail, Is.Not.Null);
                books.Bar.SkyButton.onClick.Invoke();
                yield return null;
                SkyCard[] cards = books.SkyBook.Cards.Where(card => card.gameObject.activeSelf).ToArray();
                Assert.That(books.SkyBook.IsOpen, Is.True);
                Assert.That(cards.Length, Is.EqualTo(3));
                Assert.That(cards[0].Constellation.IsLit(2), Is.True);
                Assert.That(cards[0].Constellation.IsLit(3), Is.False);
                Assert.That(cards[1].Constellation.IsLit(0), Is.False);
                LessonScene.Tap(cards[0]);
                Assert.That(MapScene.Runner().MapVoice.IsBusy, Is.True, "the constellation's name is said");
                books.SkyBook.BackButton.onClick.Invoke();
                Assert.That(books.IsBookOpen, Is.False);

                books.Bar.TrailButton.onClick.Invoke();
                TrailCard[] trails = books.TrailPicker.Cards.Where(card => card.gameObject.activeSelf).ToArray();
                Assert.That(trails.Length, Is.EqualTo(1));
                Assert.That(trails[0].IsChosen, Is.True);
                LessonScene.Tap(trails[0]);
                Assert.That(books.IsBookOpen, Is.False);
                Assert.That(SavedRewards().ChosenTrail, Is.EqualTo(rainbow));
            }
            finally
            {
                Restore(kept);
            }
        }

        /// <summary>
        /// Tests that a sticker dragged from the book onto the scene is placed and saved, that moving it and making it
        /// bigger with two fingers is saved when she lets go, that a drag cut short by closing the book leaves nothing
        /// behind, and that the sticker is still there when the map opens again.
        /// </summary>
        [UnityTest]
        public IEnumerator TestStickerBook_APlacedStickerStays()
        {
            string[] kept = Files.Select(LessonScene.KeepFile).ToArray();
            try
            {
                MapScene.SaveProgress(null, "ro-a");
                ContentId guitar = ContentId.Create("guitar");
                SaveRewards(new RewardBook(3, new[] { ContentId.Create("ro-a") }, 2, new[] { guitar, guitar }, null, null, default, null));
                yield return MapScene.Open();
                RewardScreens books = (RewardScreens)MapScene.Screen().Rewards;
                books.Bar.StickerButton.onClick.Invoke();
                yield return null;
                StickerBookView book = books.StickerBook;
                StickerTrayItem item = book.TrayItems.First(candidate => candidate.gameObject.activeSelf);

                Vector2 middle = ScreenPoint(book.Scene, book.Scene.rect.center);
                PointerEventData drag = Pointer(0, middle);
                drag.delta = new Vector2(0f, 30f);
                item.OnBeginDrag(drag);
                item.OnDrag(drag);
                item.OnEndDrag(drag);

                Assert.That(book.Placed.Count, Is.EqualTo(1));
                Assert.That(item.CopiesLeft, Is.EqualTo(1), "two copies given, one placed");
                PlacedSticker placed = SavedRewards().Placed.Single();
                Assert.That(placed.StickerId, Is.EqualTo(guitar));
                Assert.That(placed.X, Is.EqualTo(0.5f).Within(0.05f));
                Assert.That(placed.Y, Is.EqualTo(0.5f).Within(0.05f));

                PlacedStickerView view = book.Placed[0];
                Vector2 right = ScreenPoint(book.Scene, book.Scene.rect.center + new Vector2(100f, 0f));
                PointerEventData first = Pointer(1, middle);
                PointerEventData second = Pointer(2, right);
                view.OnPointerDown(first);
                view.OnPointerDown(second);
                second.position = ScreenPoint(book.Scene, book.Scene.rect.center + new Vector2(200f, 0f));
                view.OnDrag(second);
                Assert.That(view.IsPinched, Is.True);
                view.OnPointerUp(second);
                view.OnPointerUp(first);

                PlacedSticker pinched = SavedRewards().Placed.Single();
                Assert.That(pinched.Size, Is.EqualTo(2f).Within(0.05f));
                Assert.That(pinched.X, Is.GreaterThan(0.5f));

                PointerEventData loose = Pointer(3, middle);
                loose.delta = new Vector2(0f, 30f);
                item.OnBeginDrag(loose);
                book.BackButton.onClick.Invoke();
                books.Bar.StickerButton.onClick.Invoke();
                Assert.That(book.Placed.Count, Is.EqualTo(1), "a sticker dragged out when the book closed goes back");
                item.OnEndDrag(loose);
                Assert.That(SavedRewards().Placed.Count, Is.EqualTo(1));

                yield return MapScene.Open();
                RewardScreens again = (RewardScreens)MapScene.Screen().Rewards;
                again.Bar.StickerButton.onClick.Invoke();
                Assert.That(again.StickerBook.Placed.Count, Is.EqualTo(1));
                Assert.That(again.StickerBook.Placed[0].Size, Is.EqualTo(2f).Within(0.05f));
            }
            finally
            {
                Restore(kept);
            }
        }

        private static PointerEventData Pointer(int id, Vector2 position)
        {
            return new PointerEventData(EventSystem.current) { pointerId = id, position = position };
        }

        private static Vector2 ScreenPoint(RectTransform rect, Vector2 local)
        {
            return RectTransformUtility.WorldToScreenPoint(null, rect.TransformPoint(local));
        }

        private static void SaveRewards(RewardBook book)
        {
            Assert.That(Store().Save(RewardsData.FileName, new RewardsData(book)), Is.True);
        }

        private static RewardBook SavedRewards()
        {
            return Store().Load(RewardsData.FileName, RewardsData.CreateDefaults()).Data.ToBook();
        }

        private static void Restore(string[] kept)
        {
            for (int index = 0; index < Files.Length; index++)
            {
                LessonScene.RestoreFile(Files[index], kept[index]);
            }
        }

        private static SaveStore Store()
        {
            return SaveStore.ForProfile(Application.persistentDataPath, ProfileList.FirstChildId);
        }
    }
}
