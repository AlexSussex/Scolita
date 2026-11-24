namespace Scolita.Core.Voice
{
    /// <summary>
    /// The recorded clips, as <see cref="VoiceRouter"/> needs them: by their file name (the
    /// <c>clip</c> cell of <c>voice-lines.csv</c>). The app's is <c>RecordedVoice</c>; tests use a fake.
    /// </summary>
    public interface IClipVoice
    {
        /// <summary>Whether the clip <paramref name="clip"/> is there to play.</summary>
        bool Has(string clip);

        /// <summary>Plays <paramref name="clip"/>, stopping any other; false if it can't.</summary>
        bool Play(string clip);

        /// <summary>Whether a clip is playing.</summary>
        bool IsPlaying { get; }

        /// <summary>Stops the clip playing, if any.</summary>
        void Stop();
    }
}
