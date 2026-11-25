using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Map;
using Scolita.Game.Saving;
using UnityEngine;

namespace Scolita.Game.Map
{
    /// <summary>
    /// Where she was on the galaxy map in 0.10.0, saved as <c>map-progress.json</c> in the profile's folder: the
    /// lessons she had finished and the stop where the map last showed the rocket. Read into Core's
    /// <see cref="MapProgress"/> only to make her <c>progress</c> file from it (<c>ProgressFile</c>); never written
    /// by the app any more.
    /// </summary>
    [Serializable]
    public sealed class MapProgressData : ISaveData<MapProgressData>
    {
        /// <summary>The save file's name, without its extension.</summary>
        public const string FileName = "map-progress";

        private const int LatestVersion = 1;

        [SerializeField] private int version;
        [SerializeField] private string[] doneLessons;
        [SerializeField] private string rocketStop;

        /// <summary>The data of <paramref name="progress"/>; called by the tests (writing a 0.10.0 file).</summary>
        public MapProgressData(MapProgress progress)
            : this(progress.DoneLessons.Select(id => id.Value), progress.RocketStop.IsSet ? progress.RocketStop.Value : string.Empty)
        {
        }

        private MapProgressData(IEnumerable<string> done, string rocket)
        {
            version = LatestVersion;
            doneLessons = (done ?? Enumerable.Empty<string>())
                .Where(id => ContentId.TryCreate(id, out ContentId _))
                .Distinct(StringComparer.Ordinal)
                .ToArray();
            rocketStop = ContentId.TryCreate(rocket, out ContentId _) ? rocket : string.Empty;
        }

        /// <inheritdoc/>
        public int Version => version;

        /// <inheritdoc/>
        public int CurrentVersion => LatestVersion;

        /// <summary>The finished lessons' ids, as saved.</summary>
        public IReadOnlyList<string> DoneLessons => doneLessons;

        /// <summary>
        /// Nothing done yet and the map never shown: the rocket starts on the first stop. Called by
        /// <c>ProgressFile</c> and the tests.
        /// </summary>
        public static MapProgressData CreateDefaults()
        {
            return new MapProgressData(Enumerable.Empty<string>(), string.Empty);
        }

        /// <summary>Core's progress from this data; called by <c>ProgressFile</c> and the tests.</summary>
        public MapProgress ToProgress()
        {
            ContentId shownAt = ContentId.TryCreate(rocketStop, out ContentId stop) ? stop : default;
            return new MapProgress(doneLessons.Select(ContentId.Create), shownAt);
        }

        /// <summary>
        /// Returns this data in the current shape: a missing list becomes empty, duplicates are merged, a
        /// text that can't be a lesson id is dropped, and an id the content doesn't know is kept (a later
        /// version may know it; the map ignores it). Called by <see cref="SaveStore.Load{T}"/>.
        /// </summary>
        public MapProgressData Upgraded()
        {
            return new MapProgressData(doneLessons, rocketStop);
        }
    }
}
