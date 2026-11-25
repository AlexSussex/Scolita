using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Timing;

namespace Scolita.Core.Saving
{
    /// <summary>
    /// When a damaged save's dated copies may go (the human, 2026-11-24; <c>AGENTS.md</c> → "Progress survives
    /// updates"): each copy is kept until a newer version of the app than the one that set the latest copy aside
    /// has read and saved the file fine on several different days. One ledger per save folder, a
    /// <see cref="DamagedCopyWatch"/> per file with copies; a new copy of the same file starts its count again.
    /// </summary>
    public sealed class DamagedCopies
    {
        /// <summary>The version of a ledger entry whose version can't be read: older than any release.</summary>
        public static readonly Version UnknownVersion = new Version(0, 0, 0);

        private readonly List<DamagedCopyWatch> watches = new List<DamagedCopyWatch>();

        /// <summary>
        /// Starts from <paramref name="saved"/> (a file watched twice keeps its first watch). Called by the
        /// ledger's reader and the tests.
        /// </summary>
        public DamagedCopies(IEnumerable<DamagedCopyWatch> saved)
        {
            foreach (DamagedCopyWatch watch in saved ?? Enumerable.Empty<DamagedCopyWatch>())
            {
                if (IndexOf(watch.FileName) < 0)
                {
                    watches.Add(watch);
                }
            }
        }

        /// <summary>The files watched, in the order they were first watched.</summary>
        public IReadOnlyList<DamagedCopyWatch> Watches => watches;

        /// <summary>
        /// Records that <paramref name="version"/> set a copy of <paramref name="fileName"/> aside: its count
        /// starts again. Called by <c>SaveStore</c> when it sets a damaged file aside, and the tests.
        /// </summary>
        public void SetAside(string fileName, Version version)
        {
            Put(new DamagedCopyWatch(fileName, version, null));
        }

        /// <summary>
        /// Records copies of <paramref name="fileName"/> found with no watch (made before the ledger, or with a
        /// ledger that could not be saved or was damaged) as set aside by <paramref name="version"/>, the version
        /// running now: they go only after a later one read and saved the file fine, never by the version that may
        /// have made them. False, changing nothing, when the file is watched already. Called by <c>SaveStore</c>'s
        /// tidying at start, and the tests.
        /// </summary>
        public bool Found(string fileName, Version version)
        {
            if (IndexOf(fileName) >= 0)
            {
                return false;
            }

            watches.Add(new DamagedCopyWatch(fileName, version ?? UnknownVersion, null));
            return true;
        }

        /// <summary>
        /// Records that <paramref name="version"/> read and saved <paramref name="fileName"/> fine on
        /// <paramref name="today"/>. It counts only for a watched file, a version newer than the one that set its
        /// copy aside, and a day not counted yet; false when it doesn't count. Called by <c>SaveStore</c> after a
        /// file it read fine is saved fine, and the tests.
        /// </summary>
        public bool ReadAndSavedFine(string fileName, Version version, CalendarDay today)
        {
            int at = IndexOf(fileName);
            if (at < 0 || version == null || version.CompareTo(watches[at].SetAsideBy) <= 0
                || watches[at].FineDays.Contains(today))
            {
                return false;
            }

            DamagedCopyWatch watch = watches[at];
            watches[at] = new DamagedCopyWatch(fileName, watch.SetAsideBy, watch.FineDays.Concat(new[] { today }));
            return true;
        }

        /// <summary>
        /// The files whose copies may go: read and saved fine by a newer version on at least
        /// <paramref name="fineDays"/> different days. Called by <c>SaveStore</c>'s tidying at start, and the tests.
        /// </summary>
        public IReadOnlyList<string> Settled(int fineDays)
        {
            return watches
                .Where(watch => watch.FineDays.Count >= fineDays)
                .Select(watch => watch.FileName)
                .ToArray();
        }

        /// <summary>
        /// Stops watching <paramref name="fileName"/> (its copies are gone); false when it wasn't watched. Called by
        /// <c>SaveStore</c>'s tidying, and the tests.
        /// </summary>
        public bool Forget(string fileName)
        {
            int at = IndexOf(fileName);
            if (at < 0)
            {
                return false;
            }

            watches.RemoveAt(at);
            return true;
        }

        private void Put(DamagedCopyWatch watch)
        {
            int at = IndexOf(watch.FileName);
            if (at < 0)
            {
                watches.Add(watch);
            }
            else
            {
                watches[at] = watch;
            }
        }

        private int IndexOf(string fileName)
        {
            for (int index = 0; index < watches.Count; index++)
            {
                if (string.Equals(watches[index].FileName, fileName, StringComparison.Ordinal))
                {
                    return index;
                }
            }

            return -1;
        }
    }
}
