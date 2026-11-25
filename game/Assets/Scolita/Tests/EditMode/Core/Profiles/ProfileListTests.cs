using System;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Profiles;
using Scolita.Core.Timing;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Profiles
{
    /// <summary>Tests <see cref="ProfileList"/>: the children and the tester, their ids, avatars, who plays and who opens.</summary>
    public sealed class ProfileListTests
    {
        /// <summary>Tests that <see cref="ProfileList.AddChild"/> numbers the children, never reusing a deleted child's id.</summary>
        [Test]
        public void TestAddChild_IdsAreNeverReused()
        {
            ProfileList list = NewInstall();
            CalendarDay day = CalendarDay.FromDate(2026, 11, 25);

            Profile second = list.AddChild(Name("Ana"), Avatar("cat"), day);
            list.Delete(second.Id);
            Profile third = list.AddChild(Name("Matei"), Avatar("cat"), day);

            Assert.That(second.Id.Value, Is.EqualTo("child-2"));
            Assert.That(third.Id.Value, Is.EqualTo("child-3"));
            Assert.That(list.NextChildNumber, Is.EqualTo(4));
            Assert.That(third.Name.Text, Is.EqualTo("Matei"));
            Assert.That(third.Kind, Is.EqualTo(ProfileKind.Child));
            Assert.That(third.Created, Is.EqualTo(day));
            Assert.That(list.All.Select(profile => profile.Id.Value), Is.EqualTo(new[] { "child-1", "child-3" }));
        }

        /// <summary>Tests that <see cref="ProfileList.AddChild"/> stops at the rules' most children.</summary>
        [Test]
        public void TestAddChild_AtMostTheRulesChildren()
        {
            ProfileList list = NewInstall();
            CalendarDay day = CalendarDay.FromDate(2026, 11, 25);
            for (int index = 1; index < 6; index++)
            {
                list.AddChild(ProfileName.None, Avatar(AvatarIds()[index]), day);
            }

            Assert.That(list.Children.Count, Is.EqualTo(6));
            Assert.That(list.CanAddChild, Is.False);
            Assert.That(() => list.AddChild(ProfileName.None, Avatar("planet"), day), Throws.InvalidOperationException);
        }

        /// <summary>Tests that each child has an avatar no other child has, while the tester may share one.</summary>
        [Test]
        public void TestAvatars_OnePerChild()
        {
            ProfileList list = NewInstall();
            CalendarDay day = CalendarDay.FromDate(2026, 11, 25);
            Profile ana = list.AddChild(Name("Ana"), Avatar("cat"), day);
            Profile tester = list.AddTester(Avatar("kitty"), day);

            Assert.That(list.IsAvatarFree(Avatar("kitty"), ana.Id), Is.False);
            Assert.That(list.IsAvatarFree(Avatar("cat"), ana.Id), Is.True);
            Assert.That(list.IsAvatarFree(Avatar("cat"), default), Is.False);
            Assert.That(list.IsAvatarFree(Avatar("cat"), tester.Id), Is.True);
            Assert.That(() => list.AddChild(Name("Matei"), Avatar("cat"), day), Throws.InvalidOperationException);
            Assert.That(() => list.Change(ana.Id, Name("Ana"), Avatar("kitty")), Throws.InvalidOperationException);
            Assert.That(list.FirstFreeAvatar(AvatarIds().Select(Avatar).ToArray()), Is.EqualTo(Avatar("unicorn")));
        }

        /// <summary>Tests that <see cref="ProfileList.Change"/> gives a profile a new name and avatar, keeping its id, kind and day.</summary>
        [Test]
        public void TestChange_NameAndAvatar()
        {
            ProfileList list = NewInstall();

            Profile changed = list.Change(ProfileList.FirstChildId, Name("Ștefan"), Avatar("dog"));

            Assert.That(changed.Id, Is.EqualTo(ProfileList.FirstChildId));
            Assert.That(changed.Name.Text, Is.EqualTo("Ștefan"));
            Assert.That(changed.Avatar, Is.EqualTo(Avatar("dog")));
            Assert.That(list.All.Single(), Is.SameAs(changed));
            Assert.That(() => list.Change(ProfileId.Create("child-9"), Name("Ana"), Avatar("cat")), Throws.InvalidOperationException);
        }

        /// <summary>Tests that the last child and the tester can't be deleted.</summary>
        [Test]
        public void TestDelete_NeverTheLastChildNorTheTester()
        {
            ProfileList list = NewInstall();
            Profile tester = list.AddTester(Avatar("rocket"), CalendarDay.FromDate(2026, 11, 25));

            Assert.That(list.CanDelete(ProfileList.FirstChildId), Is.False);
            Assert.That(list.CanDelete(tester.Id), Is.False);
            Assert.That(() => list.Delete(ProfileList.FirstChildId), Throws.InvalidOperationException);
            Assert.That(() => list.Delete(tester.Id), Throws.InvalidOperationException);
            Assert.That(list.All.Count, Is.EqualTo(2));
        }

        /// <summary>Tests that deleting the child who played last, and is playing, leaves the first child last and nobody playing.</summary>
        [Test]
        public void TestDelete_TheLastChildMovesToTheFirst()
        {
            ProfileList list = NewInstall();
            Profile ana = list.AddChild(Name("Ana"), Avatar("cat"), CalendarDay.FromDate(2026, 11, 25));
            list.Play(ana.Id);

            list.Delete(ana.Id);

            Assert.That(list.LastChild, Is.EqualTo(ProfileList.FirstChildId));
            Assert.That(list.Playing.IsSet, Is.False);
            Assert.That(list.ProfileToOpen.Id, Is.EqualTo(ProfileList.FirstChildId));
            Assert.That(list.TryGet(ana.Id, out Profile _), Is.False);
        }

        /// <summary>Tests that <see cref="ProfileList.AddTester"/> makes one tester, never shown as a child.</summary>
        [Test]
        public void TestAddTester_Once()
        {
            ProfileList list = NewInstall();

            Profile tester = list.AddTester(Avatar("rocket"), CalendarDay.FromDate(2026, 11, 25));

            Assert.That(tester.Id, Is.EqualTo(ProfileList.TesterId));
            Assert.That(tester.Kind, Is.EqualTo(ProfileKind.Tester));
            Assert.That(list.Tester, Is.SameAs(tester));
            Assert.That(list.Children.Select(profile => profile.Id), Has.No.Member(tester.Id));
            Assert.That(list.StartsWithPicker, Is.False);
            Assert.That(() => list.AddTester(Avatar("star"), CalendarDay.FromDate(2026, 11, 26)), Throws.InvalidOperationException);
        }

        /// <summary>Tests that only the tester's profile opens every lesson, skips steps and shows the test information.</summary>
        [Test]
        public void TestTester_UnlocksOnlyForTheTester()
        {
            ProfileList list = NewInstall();
            Profile tester = list.AddTester(Avatar("rocket"), CalendarDay.FromDate(2026, 11, 25));
            Profile child = list.Children[0];

            Assert.That(new[] { tester.OpensEveryLesson, tester.CanSkipSteps, tester.ShowsTestInfo }, Is.All.True);
            Assert.That(new[] { child.OpensEveryLesson, child.CanSkipSteps, child.ShowsTestInfo }, Is.All.False);
        }

        /// <summary>Tests that <see cref="ProfileList.Play"/> opens a profile, a child also becoming the last child, the tester not.</summary>
        [Test]
        public void TestPlay_AChildIsTheLastChildTheTesterIsNot()
        {
            ProfileList list = NewInstall();
            CalendarDay day = CalendarDay.FromDate(2026, 11, 25);
            Profile ana = list.AddChild(Name("Ana"), Avatar("cat"), day);
            Profile tester = list.AddTester(Avatar("rocket"), day);

            list.Play(ana.Id);
            ProfileId lastAfterAna = list.LastChild;
            list.Play(tester.Id);

            Assert.That(lastAfterAna, Is.EqualTo(ana.Id));
            Assert.That(list.LastChild, Is.EqualTo(ana.Id));
            Assert.That(list.Playing, Is.EqualTo(tester.Id));
            Assert.That(list.ProfileToOpen, Is.SameAs(tester));
            Assert.That(() => list.Play(ProfileId.Create("child-9")), Throws.InvalidOperationException);
        }

        /// <summary>Tests that <see cref="ProfileList.ProfileToOpen"/> is the one playing, else the last child, else the first.</summary>
        [Test]
        public void TestProfileToOpen_PlayingElseLastElseFirst()
        {
            ProfileList list = NewInstall();
            Profile ana = list.AddChild(Name("Ana"), Avatar("cat"), CalendarDay.FromDate(2026, 11, 25));

            Profile first = list.ProfileToOpen;
            list.Play(ana.Id);
            Profile playing = list.ProfileToOpen;
            list.StopPlaying();
            Profile last = list.ProfileToOpen;

            Assert.That(first.Id, Is.EqualTo(ProfileList.FirstChildId));
            Assert.That(playing, Is.SameAs(ana));
            Assert.That(list.Playing.IsSet, Is.False);
            Assert.That(last, Is.SameAs(ana));
        }

        /// <summary>Tests that the app starts on "Who's playing?" with two or more children only.</summary>
        [Test]
        public void TestStartsWithPicker_TwoOrMoreChildren()
        {
            ProfileList list = NewInstall();
            bool withOne = list.StartsWithPicker;

            list.AddChild(Name("Ana"), Avatar("cat"), CalendarDay.FromDate(2026, 11, 25));

            Assert.That(withOne, Is.False);
            Assert.That(list.StartsWithPicker, Is.True);
        }

        /// <summary>
        /// Tests that <see cref="ProfileList.FromFolders"/> makes the list from the folders: <c>child-1</c> first, the
        /// children by number, then the tester; other folders left out; a different avatar each; nobody playing.
        /// </summary>
        [Test]
        public void TestFromFolders_TheChildrenByNumberThenTheTester()
        {
            ProfileId[] folders = new[] { "child-10", "tester", "child-1", "guest", "child-02", "child-3" }
                .Select(ProfileId.Create)
                .ToArray();
            CalendarDay day = CalendarDay.FromDate(2026, 11, 25);

            ProfileList list = ProfileList.FromFolders(folders, AvatarIds().Select(Avatar).ToArray(), day, TestProfileRules.Default());

            Assert.That(list.All.Select(profile => profile.Id.Value), Is.EqualTo(new[] { "child-1", "child-3", "child-10", "tester" }));
            Assert.That(list.Children.Select(profile => profile.Avatar.Value), Is.EqualTo(new[] { "kitty", "cat", "unicorn" }));
            Assert.That(list.Tester.Avatar.Value, Is.EqualTo("rocket"));
            Assert.That(list.All.Select(profile => profile.Name), Is.All.EqualTo(ProfileName.None));
            Assert.That(list.All.Select(profile => profile.Created), Is.All.EqualTo(day));
            Assert.That(list.LastChild, Is.EqualTo(ProfileList.FirstChildId));
            Assert.That(list.Playing.IsSet, Is.False);
            Assert.That(list.NextChildNumber, Is.EqualTo(11));
        }

        /// <summary>Tests that <see cref="ProfileList.FromFolders"/> with no folder (a new install) is <c>child-1</c> alone, with the first avatar.</summary>
        [Test]
        public void TestFromFolders_ANewInstallIsTheFirstChild()
        {
            ProfileList list = NewInstall();

            Assert.That(list.All.Single().Id, Is.EqualTo(ProfileList.FirstChildId));
            Assert.That(list.All.Single().Avatar.Value, Is.EqualTo("kitty"));
            Assert.That(list.All.Single().Kind, Is.EqualTo(ProfileKind.Child));
            Assert.That(list.NextChildNumber, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests that the constructor reads a last child or a player that is no profile (or the tester as the last child)
        /// as nobody, raises the next number above every child's, and refuses a list it can't keep.
        /// </summary>
        [Test]
        public void TestConstructor_ReadsWhatItCanAndRefusesTheRest()
        {
            CalendarDay day = CalendarDay.FromDate(2026, 11, 25);
            Profile child = new Profile(ProfileId.Create("child-4"), ProfileName.None, Avatar("cat"), ProfileKind.Child, day);
            Profile tester = new Profile(ProfileList.TesterId, ProfileName.None, Avatar("cat"), ProfileKind.Tester, day);

            ProfileList list = new ProfileList(new[] { child, tester }, tester.Id, ProfileId.Create("child-9"), 2,
                TestProfileRules.Default());

            Assert.That(list.LastChild.IsSet, Is.False);
            Assert.That(list.Playing.IsSet, Is.False);
            Assert.That(list.NextChildNumber, Is.EqualTo(5));
            Assert.That(() => new ProfileList(new[] { child, child }, default, default, 1, TestProfileRules.Default()),
                Throws.ArgumentException);
            Assert.That(() => new ProfileList(new[] { tester }, default, default, 1, TestProfileRules.Default()),
                Throws.ArgumentException);
            Profile secondTester = new Profile(ProfileId.Create("tester-2"), ProfileName.None, Avatar("cat"), ProfileKind.Tester, day);
            Assert.That(() => new ProfileList(new[] { child, tester, secondTester }, default, default, 1, TestProfileRules.Default()),
                Throws.ArgumentException);
            Profile childAsTester = new Profile(ProfileList.TesterId, ProfileName.None, Avatar("cat"), ProfileKind.Child, day);
            Assert.That(() => new ProfileList(new[] { child, childAsTester }, default, default, 1, TestProfileRules.Default()),
                Throws.ArgumentException);
            Assert.That(ProfileList.IsIdOfKind(ProfileId.Create("child-02"), ProfileKind.Child), Is.False);
            Assert.That(ProfileList.IsIdOfKind(ProfileId.Create("child-12"), ProfileKind.Child), Is.True);
            Assert.That(ProfileList.IsIdOfKind(ProfileList.TesterId, ProfileKind.Tester), Is.True);
        }

        /// <summary>
        /// Tests that a next number too high to be an id reads as the highest child's plus one, and that
        /// <see cref="ProfileList.KeepIdsAbove"/> raises it above every child folder.
        /// </summary>
        [Test]
        public void TestNextChildNumber_AlwaysAnIdAndAboveTheFolders()
        {
            CalendarDay day = CalendarDay.FromDate(2026, 11, 25);
            Profile child = new Profile(ProfileId.Create("child-4"), ProfileName.None, Avatar("cat"), ProfileKind.Child, day);

            ProfileList list = new ProfileList(new[] { child }, default, default, int.MaxValue, TestProfileRules.Default());
            int afterTheList = list.NextChildNumber;
            list.KeepIdsAbove(new[] { "child-1", "child-9", "tester", "child-09" }.Select(ProfileId.Create));

            Assert.That(afterTheList, Is.EqualTo(5));
            Assert.That(list.NextChildNumber, Is.EqualTo(10));
            Assert.That(list.AddChild(ProfileName.None, Avatar("dog"), day).Id.Value, Is.EqualTo("child-10"));
        }

        /// <summary>Tests that <see cref="ProfileRules"/> refuses numbers out of their range.</summary>
        [Test]
        public void TestRules_RefuseNumbersOutOfRange()
        {
            Assert.That(() => new ProfileRules(1, 20), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new ProfileRules(7, 20), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new ProfileRules(6, 19), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new ProfileRules(6, 41), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        private static ProfileList NewInstall()
        {
            return ProfileList.FromFolders(new ProfileId[0], AvatarIds().Select(Avatar).ToArray(), CalendarDay.FromDate(2026, 11, 24),
                TestProfileRules.Default());
        }

        private static string[] AvatarIds()
        {
            return new[] { "kitty", "cat", "unicorn", "rocket", "star", "moon", "planet" };
        }

        private static ProfileName Name(string text)
        {
            ProfileName.Read(text, TestProfileRules.Default(), out ProfileName name);
            return name;
        }

        private static ContentId Avatar(string id)
        {
            return ContentId.Create(id);
        }
    }
}
