using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Saving;
using Scolita.Core.Timing;
using UnityEngine;

namespace Scolita.Game.Saving
{
    /// <summary>One file's watch in the damaged copies' ledger (<see cref="DamagedCopiesData"/>), read into Core's <see cref="DamagedCopyWatch"/>.</summary>
    [Serializable]
    public sealed class DamagedCopyData
    {
        [SerializeField] private string file;
        [SerializeField] private string setAsideBy;
        [SerializeField] private string[] fineDays;

        /// <summary>The data of <paramref name="watch"/>; called by <see cref="DamagedCopiesData"/>.</summary>
        public DamagedCopyData(DamagedCopyWatch watch)
        {
            file = watch.FileName;
            setAsideBy = watch.SetAsideBy.ToString();
            fineDays = watch.FineDays.Select(day => day.ToString()).ToArray();
        }

        /// <summary>
        /// Core's watch; false for one without a file name. A version that can't be read is the unknown, older one, and a
        /// day that isn't one is dropped. Called by <see cref="DamagedCopiesData"/>.
        /// </summary>
        public bool TryToWatch(out DamagedCopyWatch watch)
        {
            if (string.IsNullOrEmpty(file))
            {
                watch = null;
                return false;
            }

            Version version = Version.TryParse(setAsideBy, out Version read) ? read : DamagedCopies.UnknownVersion;
            watch = new DamagedCopyWatch(file, version, Days());
            return true;
        }

        private IEnumerable<CalendarDay> Days()
        {
            foreach (string text in fineDays ?? new string[0])
            {
                if (CalendarDay.TryParse(text, out CalendarDay day))
                {
                    yield return day;
                }
            }
        }
    }
}
