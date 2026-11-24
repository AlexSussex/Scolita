using System.Collections;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Activities;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;
using Scolita.Game.Lessons;
using Scolita.Game.Settings;
using Scolita.UI.Lesson;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;

namespace Scolita.Tests.PlayMode.Lessons
{
    /// <summary>
    /// Tests the word activities' views in the real Lesson scene, each run to its end with simulated taps and
    /// drags through a hint and the answer shown: <see cref="SyllablesView"/>, <see cref="BuildWordView"/> and
    /// <see cref="ReadAndMatchView"/> on <c>ro-words-1</c>, <see cref="WordTwinsView"/> on <c>party-1</c>. Each test
    /// keeps the Editor's own <c>child-1</c> lesson and learning settings files as they were.
    /// </summary>
    public sealed class WordActivitiesTests
    {
        /// <summary>
        /// Tests that <see cref="SyllablesView"/> lights a dot per beat, clears them on a wrong count, shows one
        /// empty circle per syllable once the answer is shown, and that the right count then finishes the step.
        /// </summary>
        [UnityTest]
        public IEnumerator TestSyllablesView_RunsToItsEnd()
        {
            string keptRun = LessonScene.KeepFile(LessonRunData.FileName);
            string keptSettings = LessonScene.KeepFile(LearningSettingsData.FileName);
            try
            {
                yield return LessonScene.OpenAtStep("ro-words-1", ActivityKind.Syllables);
                LessonRunner runner = SceneSearch.FindInActiveScene<LessonRunner>();
                SyllablesView view = (SyllablesView)runner.ShownView;

                Assert.That(view.PictureCard.ShownText, Is.EqualTo("mama"));
                Assert.That(view.PictureCard.GetComponentInChildren<LanguageBadge>().Shown, Is.EqualTo(Language.Romanian));
                Assert.That(view.BeatsLit, Is.EqualTo(0));
                Beat(view);
                Assert.That(view.BeatsLit, Is.EqualTo(1));
                Assert.That(view.Drum.IsBouncing, Is.True);
                view.Tick.onClick.Invoke();
                Assert.That(runner.Run.State, Is.EqualTo(LessonRunState.Playing));
                Assert.That(view.BeatsLit, Is.EqualTo(0), "a wrong count starts again");
                Assert.That(view.PlacesShown, Is.EqualTo(0), "the circles would give the count away");
                Beat(view);
                Beat(view);
                Beat(view);
                view.Tick.onClick.Invoke();
                Assert.That(runner.Run.State, Is.EqualTo(LessonRunState.AnswerShown));
                Assert.That(view.PlacesShown, Is.EqualTo(2), "one circle per syllable: ma-ma");
                Beat(view);
                Beat(view);
                Assert.That(view.BeatsLit, Is.EqualTo(2));
                Assert.That(view.PlacesShown, Is.EqualTo(0));
                view.Tick.onClick.Invoke();
                Assert.That(runner.Run.State, Is.EqualTo(LessonRunState.StepDone));
            }
            finally
            {
                LessonScene.RestoreFile(LessonRunData.FileName, keptRun);
                LessonScene.RestoreFile(LearningSettingsData.FileName, keptSettings);
            }
        }

        /// <summary>
        /// Tests that <see cref="BuildWordView"/> shows one window per letter and the tiles, that a tile let go off
        /// the rocket is no answer, a wrong tile on it is a miss and the second makes the next letter glow, and that
        /// the tiles dragged in order fill the windows until the rocket takes off.
        /// </summary>
        [UnityTest]
        public IEnumerator TestBuildWordView_RunsToItsEnd()
        {
            string keptRun = LessonScene.KeepFile(LessonRunData.FileName);
            string keptSettings = LessonScene.KeepFile(LearningSettingsData.FileName);
            try
            {
                yield return LessonScene.OpenAtStep("ro-words-1", ActivityKind.BuildWord);
                LessonRunner runner = SceneSearch.FindInActiveScene<LessonRunner>();
                BuildWordView view = (BuildWordView)runner.ShownView;
                ActivityRound round = runner.Run.Current.Round;
                DraggedTile[] tiles = view.Tiles.Where(tile => tile.gameObject.activeSelf).ToArray();
                RocketWindow[] windows = view.Windows.Where(window => window.gameObject.activeSelf).ToArray();

                Assert.That(tiles.Select(tile => tile.Tile.ShownText), Is.EquivalentTo(new[] { "t", "a", "t", "a", "m" }));
                Assert.That(windows.Length, Is.EqualTo(4));
                Assert.That(windows.All(window => window.Shown.Length == 0), Is.True);
                Assert.That(windows[0].IsNext, Is.True);
                Assert.That(view.PictureCard.ShownText, Is.Empty, "the word isn't written: she builds it");

                DraggedTile extra = tiles.First(tile => tile.Tile.ShownText == "m");
                int misses = 0;
                runner.Run.HintGiven += hint => misses++;
                LessonScene.Tap(extra.Tile);
                DragTo(extra, new Vector2(2f, 2f));
                Assert.That(misses, Is.EqualTo(0), "a tap and a drop off the rocket are no answers");
                DragTo(extra, Centre(view.Rocket));
                Assert.That(misses, Is.EqualTo(1), "a wrong tile on the rocket is a miss");
                Assert.That(runner.Run.State, Is.EqualTo(LessonRunState.Playing));
                Assert.That(extra.gameObject.activeSelf, Is.True, "a wrong tile slides back");
                DragTo(tiles.First(tile => tile.Tile.ShownText == "a"), Centre(view.Rocket));
                Assert.That(runner.Run.State, Is.EqualTo(LessonRunState.AnswerShown));
                Assert.That(tiles[round.Answer].Tile.IsGlowing, Is.True, "the next letter glows");
                for (int window = 0; window < windows.Length; window++)
                {
                    DragTo(tiles[round.Answer], Centre(view.Rocket));
                }

                Assert.That(windows.Select(window => window.Shown), Is.EqualTo(new[] { "t", "a", "t", "a" }));
                Assert.That(tiles.Count(tile => tile.gameObject.activeSelf), Is.EqualTo(1));
                Assert.That(view.IsTakingOff, Is.True);
                Assert.That(runner.Run.State, Is.EqualTo(LessonRunState.StepDone));
            }
            finally
            {
                LessonScene.RestoreFile(LessonRunData.FileName, keptRun);
                LessonScene.RestoreFile(LearningSettingsData.FileName, keptSettings);
            }
        }

        /// <summary>
        /// Tests that <see cref="ReadAndMatchView"/> shows the word written and the pictures without words, that the
        /// word card asks for the word only after her first try, that the first miss lights the word's letters one
        /// by one, and that the second glows the right picture.
        /// </summary>
        [UnityTest]
        public IEnumerator TestReadAndMatchView_RunsToItsEnd()
        {
            string keptRun = LessonScene.KeepFile(LessonRunData.FileName);
            string keptSettings = LessonScene.KeepFile(LearningSettingsData.FileName);
            try
            {
                yield return LessonScene.OpenAtStep("ro-words-1", ActivityKind.ReadAndMatch);
                LessonRunner runner = SceneSearch.FindInActiveScene<LessonRunner>();
                ReadAndMatchView view = (ReadAndMatchView)runner.ShownView;
                ActivityRound round = runner.Run.Current.Round;
                ChoiceTile[] pictures = view.Pictures.Where(picture => picture.gameObject.activeSelf).ToArray();

                Assert.That(view.WordCard.ShownText, Is.EqualTo("mama"));
                Assert.That(pictures.Length, Is.EqualTo(3));
                Assert.That(pictures.All(picture => picture.ShownText.Length == 0 && picture.IsPictureShown), Is.True);
                LessonScene.Tap(view.WordCard);
                Assert.That(runner.Run.Current.Prompt.Lead.ToString(), Is.EqualTo("ro-line-read"), "never the word before her first try");

                ChoiceTile[] wrong = pictures.Where(picture => picture.Index != round.Answer).ToArray();
                LessonScene.Tap(wrong[0]);
                Assert.That(wrong[0].IsFaded, Is.True);
                Assert.That(view.IsSpellingOut, Is.True);
                Assert.That(runner.Run.Current.Prompt.Lead.ToString(), Is.EqualTo("ro-word-mama"));
                yield return LessonScene.WaitUntil(() => view.WordCard.ShownText.Contains("<color"));
                yield return LessonScene.WaitUntil(() => !view.IsSpellingOut);
                Assert.That(view.WordCard.ShownText, Is.EqualTo("mama"));
                LessonScene.Tap(wrong[1]);
                Assert.That(runner.Run.State, Is.EqualTo(LessonRunState.AnswerShown));
                Assert.That(pictures[round.Answer].IsGlowing, Is.True);
                LessonScene.Tap(pictures[round.Answer]);
                Assert.That(runner.Run.State, Is.EqualTo(LessonRunState.StepDone));
            }
            finally
            {
                LessonScene.RestoreFile(LessonRunData.FileName, keptRun);
                LessonScene.RestoreFile(LearningSettingsData.FileName, keptSettings);
            }
        }

        /// <summary>
        /// Tests that <see cref="WordTwinsView"/> puts the Romanian words on the left and the English on the right (a
        /// readable word written, a listen-only one as its picture), that a line drawn to a wrong word reveals the
        /// pair's pictures and the second glows the partner, and that the pairs joined by a tap and by a line stay
        /// joined with their lines until the step is done.
        /// </summary>
        [UnityTest]
        public IEnumerator TestWordTwinsView_RunsToItsEnd()
        {
            string keptRun = LessonScene.KeepFile(LessonRunData.FileName);
            string keptSettings = LessonScene.KeepFile(LearningSettingsData.FileName);
            try
            {
                yield return LessonScene.OpenAtStep("party-1", ActivityKind.WordTwins);
                LessonRunner runner = SceneSearch.FindInActiveScene<LessonRunner>();
                WordTwinsView view = (WordTwinsView)runner.ShownView;
                TwinCard romanianPat = Card(view, "ro-pat");
                TwinCard englishPat = Card(view, "en-pat");
                TwinCard planeta = Card(view, "ro-planeta");
                TwinCard planet = Card(view, "en-planet");

                Assert.That(view.Cards.Count(card => card.gameObject.activeSelf), Is.EqualTo(4));
                Assert.That(romanianPat.Rect.anchoredPosition.x, Is.LessThan(0f));
                Assert.That(englishPat.Rect.anchoredPosition.x, Is.GreaterThan(0f));
                Assert.That(romanianPat.GetComponentInChildren<LanguageBadge>().Shown, Is.EqualTo(Language.Romanian));
                Assert.That(englishPat.GetComponentInChildren<LanguageBadge>().Shown, Is.EqualTo(Language.English));
                Assert.That(romanianPat.Tile.ShownText, Is.EqualTo("pat"));
                Assert.That(romanianPat.Tile.IsPictureShown, Is.False);
                Assert.That(planeta.Tile.ShownText, Is.Empty, "a listen-only word is its picture");
                Assert.That(planeta.Tile.IsPictureShown, Is.True);

                LessonScene.Tap(romanianPat.Tile);
                Assert.That(romanianPat.IsSelected, Is.True);
                DrawLineTo(romanianPat, new Vector2(2f, 2f));
                DrawLine(romanianPat, planeta, false);
                Assert.That(runner.Run.State, Is.EqualTo(LessonRunState.Playing));
                Assert.That(romanianPat.Tile.IsPictureShown, Is.False, "a line on nothing or within a column is no answer");
                DrawLine(romanianPat, planet, true);
                Assert.That(runner.Run.State, Is.EqualTo(LessonRunState.Playing));
                Assert.That(view.Dragged.gameObject.activeSelf, Is.False);
                Assert.That(romanianPat.Tile.IsPictureShown && englishPat.Tile.IsPictureShown, Is.True, "the pair's pictures appear");
                DrawLine(romanianPat, planet, false);
                Assert.That(runner.Run.State, Is.EqualTo(LessonRunState.AnswerShown));
                Assert.That(englishPat.Tile.IsGlowing, Is.True);
                LessonScene.Tap(englishPat.Tile);
                Assert.That(view.Joined.Count(line => line.gameObject.activeSelf), Is.EqualTo(1));
                DrawLine(englishPat, planet, false);
                Assert.That(view.Joined.Count(line => line.gameObject.activeSelf), Is.EqualTo(1), "a line from a joined word is no answer");
                DrawLine(planet, planeta, false);
                Assert.That(view.Joined.Count(line => line.gameObject.activeSelf), Is.EqualTo(2));
                Assert.That(planet.Tile.IsPictureShown && planeta.Tile.IsPictureShown, Is.True);
                Assert.That(runner.Run.State, Is.EqualTo(LessonRunState.StepDone));
            }
            finally
            {
                LessonScene.RestoreFile(LessonRunData.FileName, keptRun);
                LessonScene.RestoreFile(LearningSettingsData.FileName, keptSettings);
            }
        }

        private static void Beat(SyllablesView view)
        {
            view.Drum.OnPointerDown(new PointerEventData(EventSystem.current));
        }

        private static Vector2 Centre(Transform target)
        {
            return RectTransformUtility.WorldToScreenPoint(null, target.position);
        }

        private static void DragTo(DraggedTile tile, Vector2 screenPoint)
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current) { position = screenPoint, pressPosition = screenPoint };
            tile.OnBeginDrag(pointer);
            tile.OnDrag(pointer);
            tile.OnEndDrag(pointer);
        }

        private static void DrawLine(TwinCard from, TwinCard to, bool checkTheLine)
        {
            Vector2 target = Centre(to.transform);
            PointerEventData pointer = new PointerEventData(EventSystem.current) { position = target, pressPosition = Centre(from.transform) };
            from.OnBeginDrag(pointer);
            from.OnDrag(pointer);
            if (checkTheLine)
            {
                Assert.That(SceneSearch.FindInActiveScene<WordTwinsView>().Dragged.gameObject.activeSelf, Is.True, "the line follows her finger");
            }

            from.OnEndDrag(pointer);
        }

        private static void DrawLineTo(TwinCard from, Vector2 screenPoint)
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current) { position = screenPoint, pressPosition = Centre(from.transform) };
            from.OnBeginDrag(pointer);
            from.OnDrag(pointer);
            from.OnEndDrag(pointer);
        }

        private static TwinCard Card(WordTwinsView view, string item)
        {
            return view.Cards.First(card => card.gameObject.activeSelf && card.Tile.Item.ToString() == item);
        }
    }
}
