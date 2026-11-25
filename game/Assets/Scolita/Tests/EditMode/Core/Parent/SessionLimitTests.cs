using System;
using NUnit.Framework;
using Scolita.Core.Parent;
using Scolita.Core.Timing;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Parent
{
    /// <summary>Tests <see cref="SessionLimit"/>.</summary>
    public sealed class SessionLimitTests
    {
        /// <summary>Tests that <see cref="SessionLimit.Of"/> keeps one of the rules' choices.</summary>
        [Test]
        public void TestOf_AChoiceIsKept()
        {
            SessionLimit limit = SessionLimit.Of(15, TestParentRules.Default());

            Assert.That(limit.IsOn, Is.True);
            Assert.That(limit.Minutes, Is.EqualTo(15));
        }

        /// <summary>Tests that <see cref="SessionLimit.Of"/> reads 0, a negative and an unknown number as off.</summary>
        [Test]
        public void TestOf_ZeroAndUnknownAreOff()
        {
            ParentRules rules = TestParentRules.Default();

            Assert.That(SessionLimit.Of(0, rules), Is.EqualTo(SessionLimit.Off));
            Assert.That(SessionLimit.Of(-5, rules), Is.EqualTo(SessionLimit.Off));
            Assert.That(SessionLimit.Of(12, rules), Is.EqualTo(SessionLimit.Off));
            Assert.That(SessionLimit.Off.IsOn, Is.False);
        }

        /// <summary>Tests that <see cref="SessionLimit.IsReachedBy"/> is true once today's play reaches the minutes.</summary>
        [Test]
        public void TestIsReachedBy_TodaysPlayReachesIt()
        {
            CalendarDay today = CalendarDay.FromDate(2026, 11, 25);
            SessionLimit limit = SessionLimit.Of(10, TestParentRules.Default());

            Assert.That(limit.IsReachedBy(new DailyPlay(today, TimeSpan.FromSeconds(599)), today), Is.False);
            Assert.That(limit.IsReachedBy(new DailyPlay(today, TimeSpan.FromMinutes(10)), today), Is.True);
            Assert.That(limit.IsReachedBy(new DailyPlay(today, TimeSpan.FromMinutes(40)), today), Is.True);
        }

        /// <summary>Tests that <see cref="SessionLimit.IsReachedBy"/> is false the next day and once woken.</summary>
        [Test]
        public void TestIsReachedBy_NextDayAndWokenAreAwake()
        {
            CalendarDay today = CalendarDay.FromDate(2026, 11, 25);
            SessionLimit limit = SessionLimit.Of(10, TestParentRules.Default());
            DailyPlay play = new DailyPlay(today, TimeSpan.FromMinutes(12));

            Assert.That(limit.IsReachedBy(play, today.AddDays(1)), Is.False);
            Assert.That(limit.IsReachedBy(DailyPlay.Woken(today), today), Is.False);
        }

        /// <summary>Tests that <see cref="SessionLimit.IsReachedBy"/> is never true when the limit is off.</summary>
        [Test]
        public void TestIsReachedBy_OffNever()
        {
            CalendarDay today = CalendarDay.FromDate(2026, 11, 25);

            Assert.That(SessionLimit.Off.IsReachedBy(new DailyPlay(today, TimeSpan.FromHours(5)), today), Is.False);
        }
    }
}
