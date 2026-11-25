using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Map;
using Scolita.Core.Progress;
using Scolita.Core.Timing;
using Scolita.Game.Saving;
using UnityEngine;

namespace Scolita.Game.Progress
{
    /// <summary>
    /// Her progress, saved as <c>progress.json</c> in the profile's folder through <see cref="SaveStore.ForProfile"/>
    /// (AGENTS.md, seam 5): the lessons done and the rocket's stop (0.10.0's <c>map-progress</c>, which it replaces), the
    /// days she played, and each item's progress (<see cref="ProgressItemData"/>). Read into Core's
    /// <see cref="LearnerProgress"/>. A later version that adds an item kind or an activity kind raises
    /// <see cref="CurrentVersion"/> and adds its migration to <see cref="Upgraded"/>.
    /// </summary>
    [Serializable]
    public sealed class ProgressData : ISaveData<ProgressData>
    {
        /// <summary>The save file's name, without its extension.</summary>
        public const string FileName = "progress";

        private const int LatestVersion = 1;

        [SerializeField] private int version;
        [SerializeField] private string[] doneLessons;
        [SerializeField] private string rocketStop;
        [SerializeField] private string[] daysPlayed;
        [SerializeField] private ProgressItemData[] items;

        /// <summary>The data of <paramref name="progress"/>; called by <see cref="ProgressFile"/> and the tests.</summary>
        public ProgressData(LearnerProgress progress)
        {
            version = LatestVersion;
            doneLessons = progress.Lessons.DoneLessons.Select(id => id.Value).ToArray();
            rocketStop = progress.Lessons.RocketStop.IsSet ? progress.Lessons.RocketStop.Value : string.Empty;
            daysPlayed = progress.DaysPlayed.Select(day => day.ToString()).ToArray();
            items = progress.Items.Select(item => new ProgressItemData(item)).ToArray();
        }

        /// <inheritdoc/>
        public int Version => version;

        /// <inheritdoc/>
        public int CurrentVersion => LatestVersion;

        /// <summary>
        /// Nothing done, nothing met, the map never shown. Called by <see cref="ProgressFile"/> and the tests.
        /// </summary>
        public static ProgressData CreateDefaults()
        {
            return new ProgressData(new LearnerProgress(new MapProgress(new ContentId[0], default), null, null));
        }

        /// <summary>
        /// Core's progress from this data: a text that can't be a lesson id or a day is dropped, as is an item whose kind,
        /// id, language or state can't be read; an id the content doesn't know is kept (a later version may know it). Called
        /// by <see cref="ProgressFile"/> and the tests.
        /// </summary>
        public LearnerProgress ToProgress()
        {
            return Read();
        }

        /// <summary>
        /// Returns this data in the current shape, made safe as <see cref="ToProgress"/> reads it (missing lists empty,
        /// duplicates merged, counts not below 0). Called by <see cref="SaveStore.Load{T}"/>.
        /// </summary>
        public ProgressData Upgraded()
        {
            return new ProgressData(Read());
        }

        private static IEnumerable<CalendarDay> Days(IEnumerable<string> texts)
        {
            foreach (string text in texts ?? Enumerable.Empty<string>())
            {
                if (CalendarDay.TryParse(text, out CalendarDay day))
                {
                    yield return day;
                }
            }
        }

        private LearnerProgress Read()
        {
            IEnumerable<ContentId> done = (doneLessons ?? new string[0])
                .Where(text => ContentId.TryCreate(text, out ContentId _))
                .Select(ContentId.Create);
            ContentId shownAt = ContentId.TryCreate(rocketStop, out ContentId stop) ? stop : default;
            List<ItemProgress> read = new List<ItemProgress>();
            foreach (ProgressItemData item in items ?? new ProgressItemData[0])
            {
                if (item != null && item.TryToItem(out ItemProgress progress))
                {
                    read.Add(progress);
                }
            }

            return new LearnerProgress(new MapProgress(done, shownAt), read, Days(daysPlayed));
        }
    }
}
