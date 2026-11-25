using System;
using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Parent;
using Scolita.Core.Profiles;
using Scolita.Core.Randomness;
using Scolita.Core.Timing;
using Scolita.Game.Lessons;
using Scolita.Game.Map;
using Scolita.Game.Parent;
using Scolita.Game.Progress;
using Scolita.Game.Rewards;
using Scolita.Game.Scenes;
using Scolita.Game.Settings;
using Scolita.Tests.PlayMode.Lessons;
using Scolita.Tests.PlayMode.Map;
using Scolita.Tests.PlayMode.Parent;
using Scolita.UI.Map;
using Scolita.UI.Parent;
using Scolita.UI.Profiles;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Scolita.Tests.PlayMode.Profiles
{
    /// <summary>
    /// Tests the profiles' scene flow: Boot opens the map with one child and "Who's playing?" with two
    /// (<see cref="ProfilesRunner"/>), a card says the name and opens her map, the map's avatar button goes back to the
    /// picker, two children each keep their own progress, rewards and settings, and random taps never open the parent
    /// area. Each test keeps the Editor's own save root and starts from an empty one.
    /// </summary>
    public sealed class ProfilesFlowTests
    {
        /// <summary>Tests that with one child (and the tester) Boot opens her map straight away, with no avatar button and no strip.</summary>
        [UnityTest]
        public IEnumerator TestBoot_OneChildOpensHerMap()
        {
            return ProfilesScene.Isolated(TestBoot_OneChildOpensHerMapSteps);
        }

        private static IEnumerator TestBoot_OneChildOpensHerMapSteps()
        {
            ProfilesScene.SaveChildren(true, "Ana");

            yield return ProfilesScene.Boot(AppScene.Map);
            yield return MapScene.WaitForMap();

            Assert.That(ProfilesScene.Saved().Playing, Is.EqualTo(ProfileList.FirstChildId));
            Assert.That(MapScene.Screen().AvatarButton.gameObject.activeSelf, Is.False);
            Assert.That(ProfilesScene.Strip().IsShown, Is.False);
            Assert.That(MapScene.Runner().Route.Stops.Skip(1).Any(stop => stop.CanOpen), Is.False);
        }

        /// <summary>
        /// Tests that with two children Boot opens "Who's playing?" with their two cards (never the tester's), the last
        /// child ringed, and that tapping a card says her name and then opens her map.
        /// </summary>
        [UnityTest]
        public IEnumerator TestPicker_ACardSaysTheNameAndOpensHerMap()
        {
            return ProfilesScene.Isolated(TestPicker_ACardSaysTheNameAndOpensHerMapSteps);
        }

        private static IEnumerator TestPicker_ACardSaysTheNameAndOpensHerMapSteps()
        {
            ProfilesScene.SaveChildren(true, "Ana", "Ștefan");

            yield return ProfilesScene.Boot(AppScene.Profiles);
            yield return ProfilesScene.WaitForPicker();
            ProfilesScreen screen = ProfilesScene.Screen();

            Assert.That(ProfilesScene.Saved().Playing.IsSet, Is.False);
            Assert.That(screen.Cards.Select(card => card.NameShown), Is.EqualTo(new[] { "Ana", "Ștefan" }));
            Assert.That(screen.Cards.Select(card => card.Id.Value), Is.EqualTo(new[] { "child-1", "child-2" }));
            Assert.That(screen.Cards[0].IsRinged, Is.True);
            Assert.That(ProfilesScene.BubbleWords(), Is.EqualTo("Cine se joacă? Atinge poza ta."));

            screen.Cards[1].Button.onClick.Invoke();
            yield return null;

            Assert.That(ProfilesScene.BubbleWords(), Is.EqualTo("Bună, Ștefan! Hai să zburăm!"));
            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(AppScene.Profiles.ToString()));
            yield return SceneSearch.WaitForScene(AppScene.Map);
            yield return MapScene.WaitForMap();
            ProfileList saved = ProfilesScene.Saved();
            Assert.That(saved.Playing.Value, Is.EqualTo("child-2"));
            Assert.That(saved.LastChild.Value, Is.EqualTo("child-2"));
            Assert.That(MapScene.Screen().AvatarButton.gameObject.activeSelf, Is.True);
            Assert.That(ProfilesScene.FolderExists("child-2"), Is.True);
        }

        /// <summary>Tests that a child without a name is greeted without one, in the only language she learns.</summary>
        [UnityTest]
        public IEnumerator TestPicker_NoNameGreetsWithoutOne()
        {
            return ProfilesScene.Isolated(TestPicker_NoNameGreetsWithoutOneSteps);
        }

        private static IEnumerator TestPicker_NoNameGreetsWithoutOneSteps()
        {
            ProfilesScene.SaveChildren(false, "Ana", string.Empty);
            Assert.That(ProfilesScene.FilesOf("child-2").Save(LearningSettingsData.FileName,
                new LearningSettingsData(LanguageMix.EnglishOnly)), Is.True);

            yield return ProfilesScene.OpenPicker();
            ProfilesScene.Screen().Cards[1].Button.onClick.Invoke();
            yield return null;

            Assert.That(ProfilesScene.Screen().Cards[1].NameShown, Is.Empty);
            Assert.That(ProfilesScene.BubbleWords(), Is.EqualTo("Hello! Let's fly!"));
            yield return SceneSearch.WaitForScene(AppScene.Map);
        }

        /// <summary>Tests that the map's avatar button opens "Who's playing?" again, without the gate.</summary>
        [UnityTest]
        public IEnumerator TestMap_TheAvatarButtonSwitchesChild()
        {
            return ProfilesScene.Isolated(TestMap_TheAvatarButtonSwitchesChildSteps);
        }

        private static IEnumerator TestMap_TheAvatarButtonSwitchesChildSteps()
        {
            ProfilesScene.SaveChildren(false, "Ana", "Matei");
            ProfilesScene.Play("child-1");
            yield return MapScene.Open();

            MapScene.Screen().AvatarButton.onClick.Invoke();
            yield return ProfilesScene.WaitForPicker();

            Assert.That(ProfilesScene.Screen().Cards.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests the acceptance criterion: with two children, each plays a lesson as herself and keeps her own progress,
        /// stars and settings; playing as one never changes the other's files, and only the list sits in the root.
        /// </summary>
        [UnityTest]
        public IEnumerator TestTwoChildren_EachKeepsHerOwnProgress()
        {
            return ProfilesScene.Isolated(TestTwoChildren_EachKeepsHerOwnProgressSteps);
        }

        private static IEnumerator TestTwoChildren_EachKeepsHerOwnProgressSteps()
        {
            ProfilesScene.SaveChildren(false, "Ana", "Matei");
            Assert.That(ProfilesScene.FilesOf("child-2").Save(LearningSettingsData.FileName,
                new LearningSettingsData(LanguageMix.EnglishOnly)), Is.True);
            ProfilesScene.Play("child-1");
            ProfilesScene.SaveLessonToOpen("child-1", "ro-a");
            yield return LessonScene.Open();
            yield return LessonScene.FinishLesson(SceneSearch.FindInActiveScene<LessonRunner>());
            yield return ParentScene.LeaveTheOpenScene();
            int anaStars = Stars("child-1");
            string anaProgress = File.ReadAllText(Path.Combine(ProfilesScene.FilesOf("child-1").Folder, "progress.json"));

            Assert.That(anaStars, Is.GreaterThan(0));
            Assert.That(anaProgress, Does.Contain("\"ro-a\""));
            Assert.That(File.Exists(Path.Combine(ProfilesScene.FilesOf("child-2").Folder, "progress.json")), Is.False);

            ProfilesScene.Play("child-2");
            ProfilesScene.SaveLessonToOpen("child-2", "en-a");
            yield return LessonScene.Open();
            yield return LessonScene.FinishLesson(SceneSearch.FindInActiveScene<LessonRunner>());
            yield return ParentScene.LeaveTheOpenScene();
            yield return MapScene.Open();
            bool mateiHasRomanian = MapScene.Runner().Route.Stops.Any(stop => stop.Lesson.Languages == LanguageMix.RomanianOnly);
            yield return ParentScene.LeaveTheOpenScene();

            Assert.That(Stars("child-2"), Is.GreaterThan(0));
            Assert.That(File.ReadAllText(Path.Combine(ProfilesScene.FilesOf("child-2").Folder, "progress.json")), Does.Contain("\"en-a\""));
            Assert.That(mateiHasRomanian, Is.False);
            Assert.That(Stars("child-1"), Is.EqualTo(anaStars));
            Assert.That(File.ReadAllText(Path.Combine(ProfilesScene.FilesOf("child-1").Folder, "progress.json")), Is.EqualTo(anaProgress));
            Assert.That(ProfilesScene.FilesOf("child-1").Load(LearningSettingsData.FileName, LearningSettingsData.CreateDefaults())
                .Data.LanguageMix, Is.EqualTo(LanguageMix.Both));
            Assert.That(Directory.GetFiles(ProfilesScene.Root, "*.json").Select(Path.GetFileName),
                Is.SubsetOf(new[] { "profiles.json", "settings.json" }));
        }

        /// <summary>
        /// Tests the acceptance criterion: half a minute of random taps on "Who's playing?" and the maps it opens (each
        /// held 1 to 40 frames, the parent button among the places hit; the rockets resting so no lesson opens) never opens
        /// the parent area and never deletes anything.
        /// </summary>
        [UnityTest]
        public IEnumerator TestMonkey_RandomTapsNeverOpenTheAreaOrDelete()
        {
            return ProfilesScene.Isolated(TestMonkey_RandomTapsNeverOpenTheAreaOrDeleteSteps);
        }

        private static IEnumerator TestMonkey_RandomTapsNeverOpenTheAreaOrDeleteSteps()
        {
            ProfilesScene.SaveChildren(true, "Ana", "Matei");
            foreach (string child in new[] { "child-1", "child-2" })
            {
                Assert.That(ProfilesScene.FilesOf(child).Save(LearningSettingsData.FileName,
                    new LearningSettingsData(LanguageMix.Both, SessionLimit.Of(10, ParentScene.Rules))), Is.True);
                Assert.That(ProfilesScene.FilesOf(child).Save(PlayTimeData.FileName,
                    new PlayTimeData(new DailyPlay(CalendarDay.FromDateTime(DateTime.Now), TimeSpan.FromMinutes(10)))), Is.True);
            }

            yield return ProfilesScene.Boot(AppScene.Profiles);
            yield return ProfilesScene.WaitForPicker();
            SeededRandom random = new SeededRandom(2026);
            float until = Time.realtimeSinceStartup + 30f;
            int taps = 0;
            while (Time.realtimeSinceStartup < until)
            {
                ParentGatePanel panel = SceneSearch.FindInActiveScene<ParentGatePanel>();
                Vector2 position = taps % 4 == 0 && panel != null
                    ? RectTransformUtility.WorldToScreenPoint(null, ((RectTransform)panel.HoldButton.transform).position)
                    : new Vector2(random.NextInt(0, Screen.width), random.NextInt(0, Screen.height));
                yield return ParentScene.Tap(position, random.NextInt(1, 41));
                taps++;
                Assert.That(SceneManager.GetActiveScene().name, Is.Not.EqualTo(AppScene.Parent.ToString()));
                Assert.That(panel == null || !panel.IsAsking, Is.True);
            }

            Assert.That(taps, Is.GreaterThan(50));
            Assert.That(ProfilesScene.Saved().All.Count, Is.EqualTo(3));
            Assert.That(ProfilesScene.FolderExists("child-1") && ProfilesScene.FolderExists("child-2"), Is.True);
        }

        private static int Stars(string child)
        {
            return ProfilesScene.FilesOf(child).Load(RewardsData.FileName, RewardsData.CreateDefaults()).Data.ToBook().StarsEarned;
        }
    }
}
