using System.Collections.Generic;
using System.Globalization;

namespace Scolita.Editor.Voice
{
    /// <summary>A clip's numbers and what is wrong with them, as <see cref="ClipChecks"/> measures them.</summary>
    public sealed class ClipMeasurement
    {
        /// <summary>Creates a measurement; called by <see cref="ClipChecks"/>.</summary>
        public ClipMeasurement(double seconds, double lufs, double peakDbfs, double leadingSilence,
            double trailingSilence, IReadOnlyList<ClipFlag> flags)
        {
            Seconds = seconds;
            Lufs = lufs;
            PeakDbfs = peakDbfs;
            LeadingSilence = leadingSilence;
            TrailingSilence = trailingSilence;
            Flags = flags;
        }

        /// <summary>Its length.</summary>
        public double Seconds { get; }

        /// <summary>Its integrated loudness.</summary>
        public double Lufs { get; }

        /// <summary>Its sample peak.</summary>
        public double PeakDbfs { get; }

        /// <summary>The silence at its start, in seconds.</summary>
        public double LeadingSilence { get; }

        /// <summary>The silence at its end, in seconds.</summary>
        public double TrailingSilence { get; }

        /// <summary>What is wrong, if anything.</summary>
        public IReadOnlyList<ClipFlag> Flags { get; }

        /// <summary>The numbers as one line, for the Console and the report.</summary>
        public override string ToString()
        {
            return Seconds.ToString("0.00", CultureInfo.InvariantCulture) + " s, " + Lufs.ToString("0.0", CultureInfo.InvariantCulture) + " LUFS, peak " + PeakDbfs.ToString("0.0", CultureInfo.InvariantCulture)
                + " dBFS, silence " + (LeadingSilence * 1000).ToString("0", CultureInfo.InvariantCulture) + " / " + (TrailingSilence * 1000).ToString("0", CultureInfo.InvariantCulture) + " ms";
        }
    }
}
