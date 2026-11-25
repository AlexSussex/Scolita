using System;
using Scolita.Core.Parent;
using Scolita.Core.Timing;
using Scolita.Game.Saving;
using UnityEngine;

namespace Scolita.Game.Parent
{
    /// <summary>
    /// How long a learner has played today, for the daily limit: saved as <c>play-time.json</c> in the profile's
    /// folder through <see cref="SaveStore.ForProfile"/> (the day as <c>yyyy-MM-dd</c> and the seconds played that
    /// day). Saved when a step ends, when the map or a lesson closes and when the app goes to the background; a
    /// missing or damaged file is nothing played, so the rocket is never put to sleep by a damaged file.
    /// </summary>
    [Serializable]
    public sealed class PlayTimeData : ISaveData<PlayTimeData>
    {
        /// <summary>The save file's name, without its extension.</summary>
        public const string FileName = "play-time";

        private const int LatestVersion = 1;
        private const double SecondsInADay = 24d * 60d * 60d;

        [SerializeField] private int version;
        [SerializeField] private string day;
        [SerializeField] private double seconds;

        /// <summary>The data of <paramref name="play"/>; called by <see cref="PlayClock"/>, the parent area and the tests.</summary>
        public PlayTimeData(DailyPlay play)
            : this(play.Day.ToString(), play.Played.TotalSeconds)
        {
        }

        private PlayTimeData(string day, double seconds)
        {
            version = LatestVersion;
            this.day = day;
            this.seconds = double.IsNaN(seconds) ? 0d : Math.Min(Math.Max(seconds, 0d), SecondsInADay);
        }

        /// <inheritdoc/>
        public int Version => version;

        /// <inheritdoc/>
        public int CurrentVersion => LatestVersion;

        /// <summary>The time played on the day saved; nothing played when the day can't be read.</summary>
        public DailyPlay Play => CalendarDay.TryParse(day, out CalendarDay played)
            ? new DailyPlay(played, TimeSpan.FromSeconds(seconds))
            : default;

        /// <summary>Nothing played yet. Called by the scenes when they read the file, and the tests.</summary>
        public static PlayTimeData CreateDefaults()
        {
            return new PlayTimeData(string.Empty, 0d);
        }

        /// <summary>
        /// Returns this data in the current shape, the seconds clamped to one day (a negative or a non-number is
        /// nothing). Called by <see cref="SaveStore.Load{T}"/>.
        /// </summary>
        public PlayTimeData Upgraded()
        {
            return new PlayTimeData(day, seconds);
        }
    }
}
