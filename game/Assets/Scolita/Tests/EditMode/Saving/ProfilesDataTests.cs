using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Profiles;
using Scolita.Core.Timing;
using Scolita.Game.Profiles;
using Scolita.Tests.EditMode.Fakes;
using UnityEngine;

namespace Scolita.Tests.EditMode.Saving
{
    /// <summary>Tests <see cref="ProfilesData"/>, <c>profiles.json</c>: reading every entry it can, and leaving out the rest.</summary>
    public sealed class ProfilesDataTests
    {
        /// <summary>
        /// Tests that <see cref="ProfilesData.ToList"/> leaves out an entry with a bad id, a repeated id, an unknown kind, a
        /// second tester and an id that doesn't fit its kind, and reads a bad name as none, a bad avatar as the first and a
        /// bad day as today, keeping an avatar id the catalog doesn't know.
        /// </summary>
        [Test]
        public void TestToList_ReadsWhatItCan()
        {
            string json = "{\"version\":1,\"profiles\":["
                + "{\"id\":\"child-1\",\"name\":\"Ana\",\"avatar\":\"kitty\",\"kind\":\"child\",\"created\":\"2026-11-20\"},"
                + "{\"id\":\"Child 2\",\"name\":\"\",\"avatar\":\"cat\",\"kind\":\"child\",\"created\":\"2026-11-20\"},"
                + "{\"id\":\"child-1\",\"name\":\"\",\"avatar\":\"cat\",\"kind\":\"child\",\"created\":\"2026-11-20\"},"
                + "{\"id\":\"child-3\",\"name\":\"\",\"avatar\":\"cat\",\"kind\":\"guest\",\"created\":\"2026-11-20\"},"
                + "{\"id\":\"tester\",\"name\":\"\",\"avatar\":\"cat\",\"kind\":\"child\",\"created\":\"2026-11-20\"},"
                + "{\"id\":\"child-4\",\"name\":\"Ana1\",\"avatar\":\"Not An Id\",\"kind\":\"child\",\"created\":\"yesterday\"},"
                + "{\"id\":\"child-5\",\"name\":\"Matei\",\"avatar\":\"dragon\",\"kind\":\"child\",\"created\":\"2026-11-21\"},"
                + "{\"id\":\"tester\",\"name\":\"\",\"avatar\":\"rocket\",\"kind\":\"tester\",\"created\":\"2026-11-21\"},"
                + "{\"id\":\"tester\",\"name\":\"\",\"avatar\":\"star\",\"kind\":\"tester\",\"created\":\"2026-11-21\"}"
                + "],\"lastChild\":\"child-5\",\"playing\":\"nobody here\",\"nextChild\":2}";
            ProfilesData data = JsonUtility.FromJson<ProfilesData>(json).Upgraded();
            CalendarDay today = CalendarDay.FromDate(2026, 11, 25);

            ProfileList list = data.ToList(TestRules(), Avatars(), today, out int leftOut);

            Assert.That(list.All.Select(profile => profile.Id.Value), Is.EqualTo(new[] { "child-1", "child-4", "child-5", "tester" }));
            Assert.That(list.All[0].Name.Text, Is.EqualTo("Ana"));
            Assert.That(list.All[1].Name.IsNone, Is.True);
            Assert.That(list.All[1].Avatar.Value, Is.EqualTo("kitty"));
            Assert.That(list.All[1].Created, Is.EqualTo(today));
            Assert.That(list.All[2].Avatar.Value, Is.EqualTo("dragon"));
            Assert.That(list.Tester.Avatar.Value, Is.EqualTo("rocket"));
            Assert.That(list.LastChild.Value, Is.EqualTo("child-5"));
            Assert.That(list.Playing.IsSet, Is.False);
            Assert.That(list.NextChildNumber, Is.EqualTo(6));
            Assert.That(leftOut, Is.EqualTo(5));
        }

        /// <summary>Tests that <see cref="ProfilesData.ToList"/> is null when no child can be read (the list is made from the folders then).</summary>
        [Test]
        public void TestToList_NoChildIsNull()
        {
            string json = "{\"version\":1,\"profiles\":["
                + "{\"id\":\"tester\",\"name\":\"\",\"avatar\":\"rocket\",\"kind\":\"tester\",\"created\":\"2026-11-21\"}"
                + "],\"lastChild\":\"\",\"playing\":\"\",\"nextChild\":1}";

            ProfileList fromTesterOnly = JsonUtility.FromJson<ProfilesData>(json).Upgraded()
                .ToList(TestRules(), Avatars(), CalendarDay.FromDate(2026, 11, 25), out int _);
            ProfileList fromDefaults = ProfilesData.CreateDefaults().ToList(TestRules(), Avatars(), CalendarDay.FromDate(2026, 11, 25),
                out int _);

            Assert.That(fromTesterOnly, Is.Null);
            Assert.That(fromDefaults, Is.Null);
        }

        /// <summary>Tests that <see cref="ProfilesData.ToList"/> keeps a saved name longer than today's limit.</summary>
        [Test]
        public void TestToList_KeepsALongSavedName()
        {
            string json = "{\"version\":1,\"profiles\":["
                + "{\"id\":\"child-1\",\"name\":\"Alexandra-Ioana Elena Maria\",\"avatar\":\"kitty\",\"kind\":\"child\",\"created\":\"2026-11-20\"}"
                + "],\"lastChild\":\"child-1\",\"playing\":\"\",\"nextChild\":2}";

            ProfileList list = JsonUtility.FromJson<ProfilesData>(json).Upgraded()
                .ToList(TestRules(), Avatars(), CalendarDay.FromDate(2026, 11, 25), out int leftOut);

            Assert.That(list.All[0].Name.Text, Is.EqualTo("Alexandra-Ioana Elena Maria"));
            Assert.That(leftOut, Is.EqualTo(0));
        }

        private static ProfileRules TestRules()
        {
            return TestProfileRules.Default();
        }

        private static ContentId[] Avatars()
        {
            return new[] { "kitty", "cat", "rocket", "star" }.Select(ContentId.Create).ToArray();
        }
    }
}
