using System;
using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Timing;

namespace Scolita.Core.Saving
{
    /// <summary>
    /// What is known about one save file's damaged copies (<see cref="DamagedCopies"/>): the app version that set
    /// the latest copy aside, and the days on which a newer version read and saved the file fine since.
    /// </summary>
    public sealed class DamagedCopyWatch
    {
        /// <summary>
        /// Creates the watch of <paramref name="fileName"/> (without its extension), set aside by
        /// <paramref name="setAsideBy"/>, with <paramref name="fineDays"/> (each once, in order). Throws an
        /// <see cref="ArgumentException"/> for no file name and an <see cref="ArgumentNullException"/> for no
        /// version. Called by <see cref="DamagedCopies"/>, the ledger's reader and the tests.
        /// </summary>
        public DamagedCopyWatch(string fileName, Version setAsideBy, IEnumerable<CalendarDay> fineDays)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("A watch names its file.", nameof(fileName));
            }

            FileName = fileName;
            SetAsideBy = setAsideBy ?? throw new ArgumentNullException(nameof(setAsideBy));
            FineDays = (fineDays ?? Enumerable.Empty<CalendarDay>()).Distinct().OrderBy(day => day).ToArray();
        }

        /// <summary>The save file's name, without its extension (<c>progress</c>).</summary>
        public string FileName { get; }

        /// <summary>The app version that set its latest copy aside (<see cref="DamagedCopies.UnknownVersion"/> when not known).</summary>
        public Version SetAsideBy { get; }

        /// <summary>The days a newer version read and saved the file fine, in order, each once.</summary>
        public IReadOnlyList<CalendarDay> FineDays { get; }
    }
}
