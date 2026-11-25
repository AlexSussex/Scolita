using System;
using System.IO;
using NUnit.Framework;
using Scolita.Core.Parent;
using Scolita.Core.Profiles;
using Scolita.Core.Timing;
using Scolita.Game.Parent;
using Scolita.Game.Saving;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Game
{
    /// <summary>Tests <see cref="PlayClock"/> with a fake clock, in a fresh temporary folder per test.</summary>
    public sealed class PlayClockTests
    {
        /// <summary>Tests that <see cref="PlayClock.Tick"/> adds up the frames, a long one counted as a second.</summary>
        [Test]
        public void TestTick_AddsTheFramesALongOneAsASecond()
        {
            string root = CreateTemporaryFolder();
            try
            {
                PlayClock clock = new PlayClock(Store(root), new FakeClock(CalendarDay.FromDate(2026, 11, 25)));

                clock.Tick(0.5f);
                clock.Tick(0.25f);
                clock.Tick(30f);
                clock.Tick(-1f);

                Assert.That(clock.PlayedToday.TotalSeconds, Is.EqualTo(1.75).Within(0.0001));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>Tests that <see cref="PlayClock.Save"/> writes today's time, read back by the next clock.</summary>
        [Test]
        public void TestSave_ReadByTheNextClock()
        {
            string root = CreateTemporaryFolder();
            try
            {
                FakeClock day = new FakeClock(CalendarDay.FromDate(2026, 11, 25));
                PlayClock first = new PlayClock(Store(root), day);
                for (int frame = 0; frame < 120; frame++)
                {
                    first.Tick(0.5f);
                }

                bool saved = first.Save();
                PlayClock second = new PlayClock(Store(root), day);

                Assert.That(saved, Is.True);
                Assert.That(second.PlayedToday, Is.EqualTo(TimeSpan.FromMinutes(1)));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>Tests that <see cref="PlayClock.HasReached"/> is true at the limit and false the next day.</summary>
        [Test]
        public void TestHasReached_AtTheLimitUntilTheNextDay()
        {
            string root = CreateTemporaryFolder();
            try
            {
                FakeClock day = new FakeClock(CalendarDay.FromDate(2026, 11, 25));
                SessionLimit limit = SessionLimit.Of(10, TestParentRules.Default());
                PlayClock today = new PlayClock(Store(root), day);
                for (int second = 0; second < 600; second++)
                {
                    today.Tick(1f);
                }

                bool reached = today.HasReached(limit);
                today.Save();
                day.PassDays(1);
                PlayClock tomorrow = new PlayClock(Store(root), day);

                Assert.That(reached, Is.True);
                Assert.That(today.HasReached(SessionLimit.Off), Is.False);
                Assert.That(tomorrow.HasReached(limit), Is.False);
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>Tests that <see cref="PlayClock.Wake"/> starts today again from nothing, and saves it.</summary>
        [Test]
        public void TestWake_StartsTodayAgain()
        {
            string root = CreateTemporaryFolder();
            try
            {
                FakeClock day = new FakeClock(CalendarDay.FromDate(2026, 11, 25));
                PlayClock tired = new PlayClock(Store(root), day);
                tired.Tick(1f);
                tired.Save();

                bool woken = new PlayClock(Store(root), day).Wake();
                PlayClock after = new PlayClock(Store(root), day);

                Assert.That(woken, Is.True);
                Assert.That(after.PlayedToday, Is.EqualTo(TimeSpan.Zero));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        private static SaveStore Store(string root)
        {
            return SaveStore.ForProfile(root, ProfileId.Create("child-1"));
        }

        private static string CreateTemporaryFolder()
        {
            string root = Path.Combine(Path.GetTempPath(), "scolita-play-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(root);
            return root;
        }
    }
}
