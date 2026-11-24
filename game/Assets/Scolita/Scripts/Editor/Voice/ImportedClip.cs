using System.Collections.Generic;

namespace Scolita.Editor.Voice
{
    /// <summary>One item of a session as cut from its take by <see cref="RecordingSessionImport.Cut"/>: its clip (none when missed), measurement and flags.</summary>
    public sealed class ImportedClip
    {
        /// <summary>Creates one; called by <see cref="RecordingSessionImport"/>.</summary>
        public ImportedClip(RecordingItem item, MonoAudio audio, ClipMeasurement measurement, IReadOnlyList<ClipFlag> flags)
        {
            Item = item;
            Audio = audio;
            Measurement = measurement;
            Flags = flags;
        }

        /// <summary>The script's item.</summary>
        public RecordingItem Item { get; }

        /// <summary>Its shaped clip; null when the take seems to have missed it.</summary>
        public MonoAudio Audio { get; }

        /// <summary>Its clip's numbers; null when missed.</summary>
        public ClipMeasurement Measurement { get; }

        /// <summary>What is wrong, if anything.</summary>
        public IReadOnlyList<ClipFlag> Flags { get; }
    }
}
