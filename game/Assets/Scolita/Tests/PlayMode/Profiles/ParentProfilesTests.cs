using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;
using Scolita.Core.Profiles;
using Scolita.Game.Lessons;
using Scolita.Game.Parent;
using Scolita.Game.Progress;
using Scolita.Game.Scenes;
using Scolita.Game.Settings;
using Scolita.Tests.PlayMode.Map;
using Scolita.Tests.PlayMode.Parent;
using Scolita.UI.Parent;
using Scolita.UI.Profiles;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Scolita.Tests.PlayMode.Profiles
{
    /// <summary>
    /// Tests the parent area's profiles page (<see cref="ParentRunner"/>, <see cref="ParentProfilesScreen"/>): adding,
    /// changing and deleting a child (two questions, only that folder gone), the chosen profile shown in the area, and
    /// the tester's profile (made and opened only from the area, "TEST", every stop open, a step skipped, its one-tap
    /// reset leaving the children's files). Each test keeps the Editor's own save root and starts from an empty one.
    /// </summary>
    public sealed class ParentProfilesTests
    {
        /// <summary>Tests that a child is added with a name, an avatar and a language mix, and that a name that can't be used isn't saved.</summary>
        [UnityTest]
        public IEnumerator TestAddChild_SavedWithItsSettings()
        {
            return ProfilesScene.Isolated(TestAddChild_SavedWithItsSettingsSteps);
        }

        private static IEnumerator TestAddChild_SavedWithItsSettingsSteps()
        {
            ProfilesScene.SaveChildren(false, "Ana");
            yield return ParentScene.Open();
            ParentProfilesScreen page = SceneSearch.FindInActiveScene<ParentProfilesScreen>();

            ParentScene.Screen().ChosenButton.onClick.Invoke();
            Assert.That(page.IsOpen, Is.True);
            Assert.That(page.Rows.Select(row => row.NameShown), Is.EqualTo(new[] { "Ana" }));
            Assert.That(page.DeleteButton.gameObject.activeSelf, Is.False);
            page.AddChildButton.onClick.Invoke();
            page.Type("Ana1");
            Assert.That(page.NameProblemText, Is.Not.Empty);
            Assert.That(page.SaveButton.interactable, Is.False);
            Assert.That(page.Avatars[0].IsFree, Is.False);
            page.Type("Ștefan");
            page.Avatars.First(avatar => avatar.IsFree && !avatar.IsChosen).Button.onClick.Invoke();
            page.MixPills[2].Button.onClick.Invoke();
            page.SaveButton.onClick.Invoke();
            yield return null;

            ProfileList saved = ProfilesScene.Saved();
            Assert.That(saved.Children.Select(child => child.Name.Text), Is.EqualTo(new[] { "Ana", "Ștefan" }));
            Assert.That(saved.Children[1].Avatar, Is.Not.EqualTo(saved.Children[0].Avatar));
            Assert.That(ProfilesScene.FilesOf("child-2").Load(LearningSettingsData.FileName, LearningSettingsData.CreateDefaults())
                .Data.LanguageMix, Is.EqualTo(LanguageMix.EnglishOnly));
            Assert.That(page.MessageText, Is.EqualTo("Profilul a fost salvat."));
            Assert.That(page.Rows[1].IsChosen, Is.True);
            Assert.That(ParentScene.Runner().Chosen.Id.Value, Is.EqualTo("child-2"));
            Assert.That(ParentScene.Screen().ChosenNameText, Is.EqualTo("Ștefan"));
        }

        /// <summary>Tests that deleting a child asks twice, then removes only its folder, and that "no" keeps it.</summary>
        [UnityTest]
        public IEnumerator TestDelete_AsksTwiceThenRemovesOnlyItsFolder()
        {
            return ProfilesScene.Isolated(TestDelete_AsksTwiceThenRemovesOnlyItsFolderSteps);
        }

        private static IEnumerator TestDelete_AsksTwiceThenRemovesOnlyItsFolderSteps()
        {
            ProfilesScene.SaveChildren(true, "Ana", "Matei");
            foreach (string profile in new[] { "child-1", "child-2", "tester" })
            {
                ProfilesScene.SaveLessonToOpen(profile, "ro-a");
            }

            yield return ParentScene.Open();
            ParentProfilesScreen page = SceneSearch.FindInActiveScene<ParentProfilesScreen>();
            ParentScene.Screen().ChosenButton.onClick.Invoke();
            page.Rows[1].Button.onClick.Invoke();
            page.DeleteButton.onClick.Invoke();
            string first = page.QuestionText;
            page.NoButton.onClick.Invoke();
            bool closedByNo = !page.IsAskingDelete;
            page.DeleteButton.onClick.Invoke();
            page.YesButton.onClick.Invoke();
            string second = page.QuestionText;
            bool stillThere = ProfilesScene.FolderExists("child-2");
            page.YesButton.onClick.Invoke();
            yield return null;

            Assert.That(first, Does.Contain("Ștergi profilul?"));
            Assert.That(closedByNo, Is.True);
            Assert.That(second, Does.Contain("Sigur?"));
            Assert.That(stillThere, Is.True);
            Assert.That(ProfilesScene.FolderExists("child-2"), Is.False);
            Assert.That(ProfilesScene.FolderExists("child-1"), Is.True);
            Assert.That(ProfilesScene.FolderExists("tester"), Is.True);
            Assert.That(ProfilesScene.Saved().All.Select(profile => profile.Id.Value), Is.EqualTo(new[] { "child-1", "tester" }));
            Assert.That(page.MessageText, Is.EqualTo("Profilul a fost șters."));
            Assert.That(page.Rows.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests the tester: never on "Who's playing?"; made and opened from the area, its map shows "TEST" and opens every
        /// stop, its lesson skips a step, and its one-tap reset deletes only its own progress.
        /// </summary>
        [UnityTest]
        public IEnumerator TestTester_FromTheAreaOnly()
        {
            return ProfilesScene.Isolated(TestTester_FromTheAreaOnlySteps);
        }

        private static IEnumerator TestTester_FromTheAreaOnlySteps()
        {
            ProfilesScene.SaveChildren(false, "Ana", "Matei");
            ProfilesScene.SaveLessonToOpen("child-1", "ro-a");
            string anaLesson = File.ReadAllText(Path.Combine(ProfilesScene.FilesOf("child-1").Folder, "lesson-run.json"));
            yield return ParentScene.Open();
            ParentProfilesScreen page = SceneSearch.FindInActiveScene<ParentProfilesScreen>();
            ParentScene.Screen().ChosenButton.onClick.Invoke();
            Assert.That(page.PlayAsTesterButton.gameObject.activeInHierarchy, Is.False);
            page.MakeTesterButton.onClick.Invoke();
            Assert.That(ProfilesScene.Saved().Tester, Is.Not.Null);
            Assert.That(page.Rows.Last().DetailShown, Does.StartWith("Nu se arată copiilor"));
            page.PlayAsTesterButton.onClick.Invoke();
            yield return MapScene.WaitForMap();

            TesterStrip mapStrip = ProfilesScene.Strip();
            Assert.That(ProfilesScene.Saved().Playing, Is.EqualTo(ProfileList.TesterId));
            Assert.That(mapStrip.IsShown, Is.True);
            Assert.That(mapStrip.SkipButton.gameObject.activeSelf, Is.False);
            Assert.That(MapScene.Runner().Route.Stops.All(stop => stop.CanOpen), Is.True);
            Assert.That(MapScene.Screen().AvatarButton.gameObject.activeSelf, Is.True);

            MapScene.Screen().GoButton.onClick.Invoke();
            yield return SceneSearch.WaitForScene(AppScene.Lesson);
            yield return MapScene.WaitFor(() => SceneSearch.FindInActiveScene<LessonRunner>()?.Run != null);
            LessonRunner lesson = SceneSearch.FindInActiveScene<LessonRunner>();
            TesterStrip lessonStrip = ProfilesScene.Strip();
            Assert.That(lessonStrip.IsShown, Is.True);
            Assert.That(lessonStrip.Info, Does.Contain(lesson.Run.Plan.Lesson.Id.Value));
            yield return MapScene.WaitFor(() => lesson.Run.State == LessonRunState.Playing);
            lessonStrip.SkipButton.onClick.Invoke();
            yield return MapScene.WaitFor(() => lesson.Run.StepIndex == 1);
            yield return ParentScene.LeaveTheOpenScene();

            Assert.That(ProfilesScene.FilesOf("tester").Load(LessonRunData.FileName, LessonRunData.CreateDefaults()).Data.StepNumber,
                Is.GreaterThan(1));
            Assert.That(File.ReadAllText(Path.Combine(ProfilesScene.FilesOf("child-1").Folder, "lesson-run.json")), Is.EqualTo(anaLesson));

            yield return ProfilesScene.OpenPicker();
            Assert.That(ProfilesScene.Screen().Cards.Select(card => card.Id.Value), Is.EqualTo(new[] { "child-1", "child-2" }));
            yield return ParentScene.Open();
            page = SceneSearch.FindInActiveScene<ParentProfilesScreen>();
            ParentScene.Screen().ChosenButton.onClick.Invoke();
            page.ResetTesterButton.onClick.Invoke();

            Assert.That(page.MessageText, Is.EqualTo("Progresul de test a fost șters."));
            Assert.That(File.Exists(Path.Combine(ProfilesScene.FilesOf("tester").Folder, "lesson-run.json")), Is.False);
            Assert.That(File.ReadAllText(Path.Combine(ProfilesScene.FilesOf("child-1").Folder, "lesson-run.json")), Is.EqualTo(anaLesson));
        }

        /// <summary>Tests that Back from the area with nobody playing and two children goes to "Who's playing?".</summary>
        [UnityTest]
        public IEnumerator TestBack_NobodyPlayingGoesToThePicker()
        {
            return ProfilesScene.Isolated(TestBack_NobodyPlayingGoesToThePickerSteps);
        }

        private static IEnumerator TestBack_NobodyPlayingGoesToThePickerSteps()
        {
            ProfilesScene.SaveChildren(false, "Ana", "Matei");
            yield return ParentScene.Open();

            ParentScene.Screen().BackButton.onClick.Invoke();
            yield return ProfilesScene.WaitForPicker();

            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(AppScene.Profiles.ToString()));
        }
    }
}
