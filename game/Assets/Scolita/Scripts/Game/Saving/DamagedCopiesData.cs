using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Saving;
using UnityEngine;

namespace Scolita.Game.Saving
{
    /// <summary>
    /// The ledger of a save folder's damaged copies, <c>broken-copies.json</c>, kept by <see cref="SaveStore"/> beside the
    /// files it watches: per file with dated copies, the app version that set the latest one aside and the days a newer
    /// version read and saved the file fine since. Read into Core's <see cref="DamagedCopies"/>, which decides when the
    /// copies go.
    /// </summary>
    [Serializable]
    public sealed class DamagedCopiesData : ISaveData<DamagedCopiesData>
    {
        /// <summary>The ledger's file name, without its extension.</summary>
        public const string FileName = "broken-copies";

        private const int LatestVersion = 1;

        [SerializeField] private int version;
        [SerializeField] private DamagedCopyData[] files;

        /// <summary>The data of <paramref name="copies"/>; called by <see cref="SaveStore"/> and the tests.</summary>
        public DamagedCopiesData(DamagedCopies copies)
        {
            version = LatestVersion;
            files = copies.Watches.Select(watch => new DamagedCopyData(watch)).ToArray();
        }

        /// <inheritdoc/>
        public int Version => version;

        /// <inheritdoc/>
        public int CurrentVersion => LatestVersion;

        /// <summary>Nothing watched; called by <see cref="SaveStore"/> and the tests.</summary>
        public static DamagedCopiesData CreateDefaults()
        {
            return new DamagedCopiesData(new DamagedCopies(null));
        }

        /// <summary>Core's ledger from this data, a watch without a file name dropped; called by <see cref="SaveStore"/> and the tests.</summary>
        public DamagedCopies ToCopies()
        {
            return Read();
        }

        /// <summary>Returns this data in the current shape, made safe as <see cref="ToCopies"/> reads it. Called by <see cref="SaveStore.Load{T}"/>.</summary>
        public DamagedCopiesData Upgraded()
        {
            return new DamagedCopiesData(Read());
        }

        private DamagedCopies Read()
        {
            List<DamagedCopyWatch> watches = new List<DamagedCopyWatch>();
            foreach (DamagedCopyData file in files ?? new DamagedCopyData[0])
            {
                if (file != null && file.TryToWatch(out DamagedCopyWatch watch))
                {
                    watches.Add(watch);
                }
            }

            return new DamagedCopies(watches);
        }
    }
}
