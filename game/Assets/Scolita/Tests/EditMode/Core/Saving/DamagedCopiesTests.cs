using System;
using NUnit.Framework;
using Scolita.Core.Saving;
using Scolita.Core.Timing;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Saving
{
    /// <summary>Tests <see cref="DamagedCopies"/>, the rule for removing a damaged save's dated copies, with a fake clock's days.</summary>
    public sealed class DamagedCopiesTests
    {
        /// <summary>
        /// Tests that <see cref="DamagedCopies.Settled"/> lets a file's copies go only after a newer version read and saved it
        /// fine on 3 different days: the same version, or the same day twice, doesn't count.
        /// </summary>
        [Test]
        public void TestSettled_ANewerVersionOnThreeDifferentDays()
        {
            FakeClock clock = new FakeClock(CalendarDay.FromDate(2026, 11, 25));
            DamagedCopies copies = new DamagedCopies(null);
            copies.SetAside("progress", new Version(0, 11, 0));

            bool sameVersion = copies.ReadAndSavedFine("progress", new Version(0, 11, 0), clock.Today);
            bool newer = copies.ReadAndSavedFine("progress", new Version(0, 11, 1), clock.Today);
            bool sameDay = copies.ReadAndSavedFine("progress", new Version(0, 12, 0), clock.Today);
            clock.PassDays(1);
            copies.ReadAndSavedFine("progress", new Version(0, 11, 1), clock.Today);
            bool afterTwoDays = copies.Settled(3).Count == 0;
            clock.PassDays(5);
            copies.ReadAndSavedFine("progress", new Version(0, 11, 1), clock.Today);

            Assert.That(sameVersion, Is.False);
            Assert.That(newer, Is.True);
            Assert.That(sameDay, Is.False);
            Assert.That(afterTwoDays, Is.True);
            Assert.That(copies.Settled(3), Is.EqualTo(new[] { "progress" }));
        }

        /// <summary>Tests that <see cref="DamagedCopies.SetAside"/> starts a file's count again when a new copy is set aside.</summary>
        [Test]
        public void TestSetAside_ANewCopyStartsAgain()
        {
            FakeClock clock = new FakeClock(CalendarDay.FromDate(2026, 11, 25));
            DamagedCopies copies = new DamagedCopies(new[]
            {
                new DamagedCopyWatch("progress", new Version(0, 10, 0), new[] { clock.Today.AddDays(-2), clock.Today.AddDays(-1) })
            });

            copies.SetAside("progress", new Version(0, 11, 0));
            bool sameVersion = copies.ReadAndSavedFine("progress", new Version(0, 11, 0), clock.Today);

            Assert.That(copies.Watches[0].FineDays, Is.Empty);
            Assert.That(copies.Watches[0].SetAsideBy, Is.EqualTo(new Version(0, 11, 0)));
            Assert.That(sameVersion, Is.False);
        }

        /// <summary>
        /// Tests that <see cref="DamagedCopies.Found"/> watches copies found without a watch as set aside by the version
        /// running now (only a later one counts), and changes nothing for a file watched already.
        /// </summary>
        [Test]
        public void TestFound_CopiesFromBeforeTheLedger()
        {
            CalendarDay today = CalendarDay.FromDate(2026, 11, 25);
            DamagedCopies copies = new DamagedCopies(null);
            copies.SetAside("rewards", new Version(0, 12, 0));

            bool found = copies.Found("map-progress", new Version(0, 11, 0));
            bool again = copies.Found("rewards", new Version(0, 11, 0));
            bool sameVersion = copies.ReadAndSavedFine("map-progress", new Version(0, 11, 0), today);
            bool later = copies.ReadAndSavedFine("map-progress", new Version(0, 12, 0), today);
            bool notWatched = copies.ReadAndSavedFine("lesson-run", new Version(0, 11, 0), today);

            Assert.That(found, Is.True);
            Assert.That(again, Is.False);
            Assert.That(copies.Watches[1].SetAsideBy, Is.EqualTo(new Version(0, 11, 0)));
            Assert.That(copies.Watches[0].SetAsideBy, Is.EqualTo(new Version(0, 12, 0)));
            Assert.That(sameVersion, Is.False);
            Assert.That(later, Is.True);
            Assert.That(notWatched, Is.False);
        }

        /// <summary>Tests that <see cref="DamagedCopies.Forget"/> stops watching a file, once.</summary>
        [Test]
        public void TestForget_StopsWatching()
        {
            DamagedCopies copies = new DamagedCopies(null);
            copies.Found("progress", new Version(0, 11, 0));

            bool first = copies.Forget("progress");
            bool second = copies.Forget("progress");

            Assert.That(first, Is.True);
            Assert.That(second, Is.False);
            Assert.That(copies.Watches, Is.Empty);
        }
    }
}
