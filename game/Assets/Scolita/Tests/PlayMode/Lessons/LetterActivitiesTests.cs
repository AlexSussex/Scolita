using System.Collections;
using System.Collections.Generic;
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
using UnityEngine.UI;

namespace Scolita.Tests.PlayMode.Lessons
{
    /// <summary>
    /// Tests the letter activities' views in the real Lesson scene, each run to its end with simulated
    /// taps and drags through a hint and the answer shown: <see cref="LetterHuntView"/> and
    /// <see cref="FirstSoundView"/> on <c>ro-m</c>, <see cref="TwoSoundsView"/> on <c>en-a</c>. Each test
    /// keeps the Editor's own <c>child-1</c> lesson and learning settings files as they were.
    /// </summary>
    public sealed class LetterActivitiesTests
    {
        /// <summary>
        /// Tests that <see cref="LetterHuntView"/> keeps its stars under the top bar (never over the home
        /// button or the mascot) and never faster than the rules allow, and that the hunt runs to its end: a
        /// miss twinkles the stars with m, a second shows one, and every star with m pops.
        /// </summary>
        [UnityTest]
        public IEnumerator TestLetterHuntView_RunsToItsEnd()
        {
            string keptRun = LessonScene.KeepFile(LessonRunData.FileName);
            string keptSettings = LessonScene.KeepFile(LearningSettingsData.FileName);
            try
            {
                yield return LessonScene.OpenAtStep("ro-m", ActivityKind.LetterHunt);
                LessonRunner runner = SceneSearch.FindInActiveScene<LessonRunner>();
                LetterHuntView view = (LetterHuntView)runner.ShownView;
                HuntStar[] stars = view.Stars.Where(star => star.gameObject.activeSelf).ToArray();
                ActivityRound round = runner.Run.Current.Round;

                Assert.That(stars.Length, Is.EqualTo(10));
                Assert.That(stars.All(star => star.GetComponentInChildren<LanguageBadge>().Shown == Language.Romanian), Is.True);
                Assert.That(Top(view.Field), Is.LessThanOrEqualTo(Bottom(SceneSearch.FindInActiveScene<LessonScreen>().Guide.transform)));
                Assert.That(Top(view.Field), Is.LessThanOrEqualTo(Bottom(SceneSearch.FindInActiveScene<LessonHud>().GetComponentInChildren<Button>().transform)));
                yield return CheckTheStarsMove(view, stars);

                HuntStar[] right = stars.Where(star => round.Choices[star.Index].Item.ToString() == "m").ToArray();
                HuntStar[] wrong = stars.Except(right).ToArray();
                LessonScene.Tap(wrong[0]);
                Assert.That(right.All(star => star.IsTwinkling), Is.True);
                Assert.That(wrong.Any(star => star.IsTwinkling), Is.False);
                LessonScene.Tap(wrong[1]);
                Assert.That(runner.Run.State, Is.EqualTo(LessonRunState.AnswerShown));
                Assert.That(right.All(star => star.IsGlowing), Is.True, "every star with m glows");
                Assert.That(wrong.Any(star => star.IsGlowing), Is.False);
                HuntStar shown = right[0];
                LessonScene.Tap(shown);
                Assert.That(shown.IsInTheSky, Is.False);
                Assert.That(right.Any(star => star.IsGlowing), Is.False, "the glow ends once one is found");
                foreach (HuntStar star in right.Where(star => star != shown))
                {
                    LessonScene.Tap(star);
                }

                Assert.That(runner.Run.State, Is.EqualTo(LessonRunState.StepDone));
                Assert.That(right.All(star => !star.IsInTheSky), Is.True, "every star with m popped");
                Assert.That(wrong.All(star => star.IsInTheSky), Is.True);
            }
            finally
            {
                LessonScene.RestoreFile(LessonRunData.FileName, keptRun);
                LessonScene.RestoreFile(LearningSettingsData.FileName, keptSettings);
            }
        }

        /// <summary>
        /// Tests that <see cref="FirstSoundView"/> shows the picture without its word and the letters m and a;
        /// a tap on the picture is no answer, a miss fades the letter, the second shows m glowing, and m ends
        /// the lesson.
        /// </summary>
        [UnityTest]
        public IEnumerator TestFirstSoundView_RunsToItsEnd()
        {
            string keptRun = LessonScene.KeepFile(LessonRunData.FileName);
            string keptSettings = LessonScene.KeepFile(LearningSettingsData.FileName);
            try
            {
                yield return LessonScene.OpenAtStep("ro-m", ActivityKind.FirstSound);
                LessonRunner runner = SceneSearch.FindInActiveScene<LessonRunner>();
                FirstSoundView view = (FirstSoundView)runner.ShownView;
                ChoiceTile[] letters = view.Tiles.Where(tile => tile.gameObject.activeSelf).ToArray();
                ChoiceTile m = letters.Single(tile => tile.Item.ToString() == "m");
                ChoiceTile a = letters.Single(tile => tile.Item.ToString() == "a");

                Assert.That(letters.Length, Is.EqualTo(2));
                Assert.That(view.PictureCard.Item.ToString(), Is.EqualTo("ro-mama"));
                Assert.That(view.PictureCard.ShownText, Is.Empty,
                    "the word isn't written, so its first letter isn't shown");
                LessonScene.Tap(view.PictureCard);
                Assert.That(runner.Run.State, Is.EqualTo(LessonRunState.Playing));
                LessonScene.Tap(a);
                Assert.That(a.IsFaded, Is.True);
                LessonScene.Tap(a);
                Assert.That(m.IsGlowing, Is.True);
                LessonScene.Tap(m);
                yield return LessonScene.WaitUntil(() => runner.Run.State == LessonRunState.Finished);

                Assert.That(SceneSearch.FindInActiveScene<LessonScreen>().IsEndShown, Is.True);
            }
            finally
            {
                LessonScene.RestoreFile(LessonRunData.FileName, keptRun);
                LessonScene.RestoreFile(LearningSettingsData.FileName, keptSettings);
            }
        }

        /// <summary>
        /// Tests that <see cref="TwoSoundsView"/> holds the pictures back until both planets are heard (their
        /// ticks), then sorts them by dragging: a picture dropped on the wrong planet goes back, the second miss
        /// makes the right planet glow, and every picture ends in its planet's tray.
        /// </summary>
        [UnityTest]
        public IEnumerator TestTwoSoundsView_RunsToItsEnd()
        {
            string keptRun = LessonScene.KeepFile(LessonRunData.FileName);
            string keptSettings = LessonScene.KeepFile(LearningSettingsData.FileName);
            try
            {
                yield return LessonScene.OpenAtStep("en-a", ActivityKind.TwoSounds);
                LessonRunner runner = SceneSearch.FindInActiveScene<LessonRunner>();
                TwoSoundsView view = (TwoSoundsView)runner.ShownView;
                ActivityRound round = runner.Run.Current.Round;
                SoundPlanet romanian = view.Planets[TwoSoundsRules.RomanianPlanet];
                SoundPlanet english = view.Planets[TwoSoundsRules.EnglishPlanet];

                Assert.That(romanian.GetComponentInChildren<LanguageBadge>().Shown, Is.EqualTo(Language.Romanian));
                Assert.That(english.GetComponentInChildren<LanguageBadge>().Shown, Is.EqualTo(Language.English));
                Assert.That(view.Held.gameObject.activeSelf, Is.False);
                LessonScene.Tap(romanian);
                Assert.That(romanian.IsHeard, Is.True);
                Assert.That(view.Held.gameObject.activeSelf, Is.False, "no picture before both sounds are heard");
                LessonScene.Tap(english);
                Assert.That(english.IsHeard, Is.True);
                Assert.That(view.Held.gameObject.activeSelf, Is.True);
                Assert.That(view.Held.Shown, Is.Not.Null);
                Assert.That(view.Held.GetComponentInChildren<LanguageBadge>(true), Is.Null,
                    "no badge on the picture in her hand: she sorts it by its sound");
                LessonScene.Tap(view.LetterCard);
                LessonScene.Tap(view.Held);
                Assert.That(runner.Run.State, Is.EqualTo(LessonRunState.Playing), "tapping the letter or the picture is no answer");

                SoundPlanet wrongPlanet = view.Planets[1 - round.Answer];
                Drag(view.Held, wrongPlanet);
                Assert.That(runner.Run.State, Is.EqualTo(LessonRunState.Playing));
                Drag(view.Held, wrongPlanet);
                Assert.That(view.Planets[round.Answer].IsGlowing, Is.True);
                for (int picture = 0; picture < 4; picture++)
                {
                    Drag(view.Held, view.Planets[round.Answer]);
                }

                Assert.That(runner.Run.State, Is.EqualTo(LessonRunState.StepDone));
                Assert.That(romanian.PicturesInTray, Is.EqualTo(2));
                Assert.That(english.PicturesInTray, Is.EqualTo(2));
                Assert.That(view.Held.gameObject.activeSelf, Is.False);
            }
            finally
            {
                LessonScene.RestoreFile(LessonRunData.FileName, keptRun);
                LessonScene.RestoreFile(LearningSettingsData.FileName, keptSettings);
            }
        }

        private static IEnumerator CheckTheStarsMove(LetterHuntView view, IReadOnlyList<HuntStar> stars)
        {
            float fieldWidth = view.Field.rect.width;
            Vector2[] before = stars.Select(star => star.Rect.anchoredPosition).ToArray();
            float fastestSeen = 0f;
            for (int frame = 0; frame < 30; frame++)
            {
                yield return null;
                float seconds = Time.unscaledDeltaTime;
                for (int index = 0; index < stars.Count; index++)
                {
                    Vector2 now = stars[index].Rect.anchoredPosition;
                    Vector2 moved = now - before[index];
                    before[index] = now;
                    Assert.That(Mathf.Abs(now.y) + stars[index].Rect.rect.height * 0.5f, Is.LessThanOrEqualTo(view.Field.rect.height * 0.5f),
                        "a star left the field");
                    if (Mathf.Abs(moved.x) < fieldWidth * 0.5f && seconds > 0f)
                    {
                        fastestSeen = Mathf.Max(fastestSeen, moved.magnitude / seconds);
                    }
                }
            }

            Assert.That(fastestSeen, Is.GreaterThan(0f), "the stars move");
            Assert.That(fastestSeen, Is.LessThanOrEqualTo(view.FastestSpeed * 1.01f));
        }

        private static void Drag(HeldPicture held, SoundPlanet planet)
        {
            Vector2 target = RectTransformUtility.WorldToScreenPoint(null, planet.Body.position);
            PointerEventData pointer = new PointerEventData(EventSystem.current) { position = target, pressPosition = target };
            held.OnBeginDrag(pointer);
            held.OnDrag(pointer);
            held.OnEndDrag(pointer);
        }

        private static float Top(Transform rect)
        {
            Vector3[] corners = new Vector3[4];
            ((RectTransform)rect).GetWorldCorners(corners);
            return corners[1].y;
        }

        private static float Bottom(Transform rect)
        {
            Vector3[] corners = new Vector3[4];
            ((RectTransform)rect).GetWorldCorners(corners);
            return corners[0].y;
        }
    }
}
