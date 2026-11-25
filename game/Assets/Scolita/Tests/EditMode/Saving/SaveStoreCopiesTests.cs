using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Scolita.Core.Profiles;
using Scolita.Core.Saving;
using Scolita.Core.Timing;
using Scolita.Game.Saving;
using Scolita.Game.Settings;
using Scolita.Tests.EditMode.Fakes;
using UnityEngine;
using UnityEngine.TestTools;

namespace Scolita.Tests.EditMode.Saving
{
    /// <summary>
    /// Tests <see cref="SaveStore"/>'s damaged copies' ledger (<c>broken-copies.json</c>) and
    /// <see cref="SaveStore.RemoveSettledCopies"/>, with a fake clock's days and set app versions.
    /// </summary>
    public sealed class SaveStoreCopiesTests
    {
        /// <summary>
        /// Tests that <see cref="SaveStore.RemoveSettledCopies"/> removes a file's dated copies once a newer version read and
        /// saved it fine on 3 different days, and not before: the version that set the copy aside, or the same day twice,
        /// doesn't count.
        /// </summary>
        [Test]
        public void TestRemoveSettledCopies_ANewerVersionOnThreeDays()
        {
            string root = CreateTemporaryFolder();
            try
            {
                string folder = Path.Combine(root, "profiles", "child-1");
                Directory.CreateDirectory(folder);
                File.WriteAllText(Path.Combine(folder, "learning-settings.json"), "{ not json");
                FakeClock clock = new FakeClock(CalendarDay.FromDate(2026, 11, 25));
                LogAssert.Expect(LogType.Warning, new Regex(@"learning-settings\.json could not be read"));
                ReadAndSave(root, clock, "0.11.0");
                ReadAndSave(root, clock, "0.11.0");
                int beforeANewerVersion = RemoveSettled(root, clock, "0.11.0");

                ReadAndSave(root, clock, "0.12.0");
                ReadAndSave(root, clock, "0.12.0");
                clock.PassDays(1);
                ReadAndSave(root, clock, "0.12.0");
                int afterTwoDays = RemoveSettled(root, clock, "0.12.0");
                clock.PassDays(3);
                ReadAndSave(root, clock, "0.12.1");
                LogAssert.Expect(LogType.Log, new Regex(@"damaged copies of learning-settings\.json were removed \(1\)"));
                int afterThreeDays = RemoveSettled(root, clock, "0.12.1");

                Assert.That(beforeANewerVersion, Is.EqualTo(0));
                Assert.That(afterTwoDays, Is.EqualTo(0));
                Assert.That(afterThreeDays, Is.EqualTo(1));
                Assert.That(Directory.GetFiles(folder, "learning-settings.broken-*.json"), Is.Empty);
                Assert.That(File.Exists(Path.Combine(folder, "learning-settings.json")), Is.True);
                Assert.That(Ledger(root, clock).Watches, Is.Empty);
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>
        /// Tests that a new damaged copy of the same file starts its count again (the ledger notes the version that set the
        /// latest copy aside), and every copy stays.
        /// </summary>
        [Test]
        public void TestLoad_ANewCopyStartsTheCountAgain()
        {
            string root = CreateTemporaryFolder();
            try
            {
                string folder = Path.Combine(root, "profiles", "child-1");
                Directory.CreateDirectory(folder);
                string path = Path.Combine(folder, "learning-settings.json");
                FakeClock clock = new FakeClock(CalendarDay.FromDate(2026, 11, 25));
                LogAssert.Expect(LogType.Warning, new Regex(@"learning-settings\.json could not be read"));
                LogAssert.Expect(LogType.Warning, new Regex(@"learning-settings\.json could not be read"));
                File.WriteAllText(path, "first");
                ReadAndSave(root, clock, "0.11.0");
                ReadAndSave(root, clock, "0.12.0");
                clock.PassDays(1);

                File.WriteAllText(path, "second");
                ReadAndSave(root, clock, "0.12.0");
                ReadAndSave(root, clock, "0.12.0");

                DamagedCopyWatch watch = Ledger(root, clock).Watches.Single();
                Assert.That(watch.SetAsideBy, Is.EqualTo(new Version(0, 12, 0)));
                Assert.That(watch.FineDays, Is.Empty);
                Assert.That(Directory.GetFiles(folder, "learning-settings.broken-*.json"), Has.Length.EqualTo(2));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>
        /// Tests that <see cref="SaveStore.RemoveSettledCopies"/> watches copies found without a watch (0.10.0's, or after a
        /// damaged ledger) as set aside by the version running, so that version never removes them, only a later one; and
        /// forgets a watch whose copies were removed by hand.
        /// </summary>
        [Test]
        public void TestRemoveSettledCopies_CopiesFromBeforeTheLedger()
        {
            string root = CreateTemporaryFolder();
            try
            {
                string folder = Path.Combine(root, "profiles", "child-1");
                Directory.CreateDirectory(folder);
                string copy = Path.Combine(folder, "map-progress.broken-20261125-120000.json");
                File.WriteAllText(copy, "{ not json");
                FakeClock clock = new FakeClock(CalendarDay.FromDate(2026, 11, 25));

                int removed = RemoveSettled(root, clock, "0.11.0");
                DamagedCopyWatch watch = Ledger(root, clock).Watches.Single();
                for (int day = 0; day < 3; day++)
                {
                    clock.PassDays(1);
                    ReadAndSave(root, clock, "0.11.0");
                }

                int bySameVersion = RemoveSettled(root, clock, "0.11.0");
                File.Delete(copy);
                RemoveSettled(root, clock, "0.11.0");

                Assert.That(removed, Is.EqualTo(0));
                Assert.That(bySameVersion, Is.EqualTo(0));
                Assert.That(watch.FileName, Is.EqualTo("map-progress"));
                Assert.That(watch.SetAsideBy, Is.EqualTo(new Version(0, 11, 0)));
                Assert.That(Ledger(root, clock).Watches, Is.Empty);
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>
        /// Tests that <see cref="SaveStore.RemoveSettledCopies"/> removes nothing and never writes over a ledger a newer
        /// version of the app wrote, even when its watch says the copies may go.
        /// </summary>
        [Test]
        public void TestRemoveSettledCopies_NeverOverANewerLedger()
        {
            string root = CreateTemporaryFolder();
            try
            {
                string folder = Path.Combine(root, "profiles", "child-1");
                Directory.CreateDirectory(folder);
                string copy = Path.Combine(folder, "progress.broken-20261125-120000.json");
                File.WriteAllText(copy, "{ not json");
                string ledger = Path.Combine(folder, DamagedCopiesData.FileName + ".json");
                string newer = "{\"version\":99,\"files\":[{\"file\":\"progress\",\"setAsideBy\":\"0.1.0\","
                    + "\"fineDays\":[\"2026-11-20\",\"2026-11-21\",\"2026-11-22\"]}]}";
                File.WriteAllText(ledger, newer);
                LogAssert.Expect(LogType.Error, new Regex(@"broken-copies\.json could not be read"));

                int removed = RemoveSettled(root, new FakeClock(CalendarDay.FromDate(2026, 11, 25)), "0.11.0");

                Assert.That(removed, Is.EqualTo(0));
                Assert.That(File.Exists(copy), Is.True);
                Assert.That(File.ReadAllText(ledger), Is.EqualTo(newer));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>Tests that <see cref="SaveStore.RemoveSettledCopies"/> in a folder with no copy writes no ledger.</summary>
        [Test]
        public void TestRemoveSettledCopies_NothingToDo()
        {
            string root = CreateTemporaryFolder();
            try
            {
                SaveStore store = SaveStore.ForTablet(root, new FakeClock(CalendarDay.FromDate(2026, 11, 25)), "0.11.0");
                store.Save(TabletSettingsData.FileName, TabletSettingsData.CreateDefaults());

                int removed = store.RemoveSettledCopies(3);

                Assert.That(removed, Is.EqualTo(0));
                Assert.That(File.Exists(Path.Combine(root, DamagedCopiesData.FileName + ".json")), Is.False);
                Assert.That(SaveStore.ForTablet(Path.Combine(root, "missing")).RemoveSettledCopies(3), Is.EqualTo(0));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>Tests that <see cref="SaveStore.ProfilesIn"/> lists the profiles' folders, leaving out a name that can't be an id.</summary>
        [Test]
        public void TestProfilesIn_TheProfilesFolders()
        {
            string root = CreateTemporaryFolder();
            try
            {
                string[] none = SaveStore.ProfilesIn(root).Select(profile => profile.Value).ToArray();
                Directory.CreateDirectory(Path.Combine(root, "profiles", "child-1"));
                Directory.CreateDirectory(Path.Combine(root, "profiles", "tester"));
                Directory.CreateDirectory(Path.Combine(root, "profiles", "Not A Profile"));

                string[] found = SaveStore.ProfilesIn(root).Select(profile => profile.Value).ToArray();

                Assert.That(none, Is.Empty);
                Assert.That(found, Is.EquivalentTo(new[] { "child-1", "tester" }));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        private static void ReadAndSave(string root, FakeClock clock, string version)
        {
            SaveStore store = SaveStore.ForProfile(root, ProfileId.Create("child-1"), clock, version);
            LearningSettingsData data = store.Load(LearningSettingsData.FileName, LearningSettingsData.CreateDefaults()).Data;
            store.Save(LearningSettingsData.FileName, data);
        }

        private static int RemoveSettled(string root, FakeClock clock, string version)
        {
            return SaveStore.ForProfile(root, ProfileId.Create("child-1"), clock, version).RemoveSettledCopies(3);
        }

        private static DamagedCopies Ledger(string root, FakeClock clock)
        {
            SaveStore store = SaveStore.ForProfile(root, ProfileId.Create("child-1"), clock, "0.0.0");
            return store.Load(DamagedCopiesData.FileName, DamagedCopiesData.CreateDefaults()).Data.ToCopies();
        }

        private static string CreateTemporaryFolder()
        {
            string folder = Path.Combine(Path.GetTempPath(), "scolita-copies-tests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
            return folder;
        }
    }
}
