namespace Scolita.Editor.Voice
{
    /// <summary>Why a clip, or a session's item, needs a listen or another take.</summary>
    public enum ClipFlag
    {
        /// <summary>Quieter than −16 LUFS by more than the tolerance (or too quiet in the take to level).</summary>
        TooQuiet,

        /// <summary>Louder than −16 LUFS by more than the tolerance, or a peak over −1 dBFS.</summary>
        TooLoud,

        /// <summary>Samples at full scale in a row: the recording clipped.</summary>
        Clipped,

        /// <summary>Too much silence at the start.</summary>
        LongSilenceAtStart,

        /// <summary>Too much silence at the end.</summary>
        LongSilenceAtEnd,

        /// <summary>Too little silence at the start: the first sound may be cut.</summary>
        ShortSilenceAtStart,

        /// <summary>Too little silence at the end: the last sound may be cut.</summary>
        ShortSilenceAtEnd,

        /// <summary>Hardly any sound.</summary>
        TooShort,

        /// <summary>Longer than its kind of line should be.</summary>
        TooLong,

        /// <summary>The take seems to have skipped this line.</summary>
        Missed,

        /// <summary>The take seems to have a piece too many near this line (a repeat, or a noise).</summary>
        Repeated,

        /// <summary>The take's pieces don't match the script near here, so this clip may be another line's.</summary>
        Uncertain
    }
}
