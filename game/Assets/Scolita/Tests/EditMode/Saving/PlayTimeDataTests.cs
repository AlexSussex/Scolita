using System;
using NUnit.Framework;
using Scolita.Core.Parent;
using Scolita.Core.Timing;
using Scolita.Game.Parent;
using UnityEngine;

namespace Scolita.Tests.EditMode.Saving
{
    /// <summary>Tests <see cref="PlayTimeData"/>: its shape, defaults and ranges.</summary>
    public sealed class PlayTimeDataTests
    {
        /// <summary>Tests that <see cref="PlayTimeData"/> is written as its day and seconds, and read back the same.</summary>
        [Test]
        public void TestPlayTimeData_RoundTrip()
        {
            CalendarDay today = CalendarDay.FromDate(2026, 11, 25);
            PlayTimeData data = new PlayTimeData(new DailyPlay(today, TimeSpan.FromSeconds(90.5)));

            string json = JsonUtility.ToJson(data);
            PlayTimeData read = JsonUtility.FromJson<PlayTimeData>(json).Upgraded();

            Assert.That(json, Is.EqualTo("{\"version\":1,\"day\":\"2026-11-25\",\"seconds\":90.5}"));
            Assert.That(read.Play, Is.EqualTo(new DailyPlay(today, TimeSpan.FromSeconds(90.5))));
        }

        /// <summary>Tests that <see cref="PlayTimeData.CreateDefaults"/> is nothing played on any day.</summary>
        [Test]
        public void TestCreateDefaults_NothingPlayed()
        {
            CalendarDay today = CalendarDay.FromDate(2026, 11, 25);

            DailyPlay play = PlayTimeData.CreateDefaults().Play;

            Assert.That(play.PlayedOn(today), Is.EqualTo(TimeSpan.Zero));
        }

        /// <summary>Tests that <see cref="PlayTimeData.Upgraded"/> reads a day it can't parse as nothing played.</summary>
        [Test]
        public void TestUpgraded_UnreadableDayIsNothing()
        {
            PlayTimeData read = JsonUtility.FromJson<PlayTimeData>("{\"version\":1,\"day\":\"yesterday\",\"seconds\":900}");

            DailyPlay play = read.Upgraded().Play;

            Assert.That(play.PlayedOn(CalendarDay.FromDate(2026, 11, 25)), Is.EqualTo(TimeSpan.Zero));
        }

        /// <summary>Tests that <see cref="PlayTimeData.Upgraded"/> clamps the seconds to a day, and a negative to nothing.</summary>
        [TestCase(-30.0, 0.0)]
        [TestCase(600.0, 600.0)]
        [TestCase(999999.0, 86400.0)]
        public void TestUpgraded_ClampsTheSeconds(double seconds, double expected)
        {
            CalendarDay today = CalendarDay.FromDate(2026, 11, 25);
            PlayTimeData read = JsonUtility.FromJson<PlayTimeData>(
                "{\"version\":1,\"day\":\"2026-11-25\",\"seconds\":" + seconds.ToString(System.Globalization.CultureInfo.InvariantCulture) + "}");

            DailyPlay play = read.Upgraded().Play;

            Assert.That(play.PlayedOn(today).TotalSeconds, Is.EqualTo(expected).Within(0.001));
        }
    }
}
