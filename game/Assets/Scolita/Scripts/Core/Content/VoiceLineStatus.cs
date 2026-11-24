namespace Scolita.Core.Content
{
    /// <summary>
    /// Whether a voice line is recorded: the <c>status</c> cell of <c>voice-lines.csv</c>, written as
    /// its code (<see cref="ContentCodes"/>) and filled by the content importer from the clips found
    /// (<see cref="VoiceLineStatuses"/>), never typed.
    /// </summary>
    public enum VoiceLineStatus
    {
        /// <summary>No clip yet (an empty cell): the system voice says it, and the recording scripts list it.</summary>
        ToRecord,

        /// <summary><c>recorded</c>: every clip it names is in <c>content/voice/</c>.</summary>
        Recorded,

        /// <summary><c>missing</c>: it names a clip that isn't in <c>content/voice/</c>.</summary>
        Missing,

        /// <summary><c>sounds</c>: a slow line with no clip, said as its word's letters' sounds; nothing to record.</summary>
        FromSounds
    }
}
