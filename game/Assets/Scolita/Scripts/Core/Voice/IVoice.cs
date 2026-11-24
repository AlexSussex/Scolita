using System;
using Scolita.Core.Content;

namespace Scolita.Core.Voice
{
    /// <summary>
    /// Everything the app says goes through this (AGENTS.md, seam 4): a lesson names the line, never
    /// how it is said. <see cref="VoiceRouter"/> is the app's voice; tests use a fake.
    /// </summary>
    public interface IVoice
    {
        /// <summary>Raised when a line (or a sequence, by its lead line) starts; the music ducks.</summary>
        event Action<VoiceLineId> Started;

        /// <summary>Raised when it ends, is stopped or is replaced by a newer line; the music comes back.</summary>
        event Action<VoiceLineId> Finished;

        /// <summary>Whether a line is being said.</summary>
        bool IsSpeaking { get; }

        /// <summary>The line being said (its lead line for a sequence), when <see cref="IsSpeaking"/>.</summary>
        VoiceLineId Current { get; }

        /// <summary>Says <paramref name="line"/>, stopping whatever was being said.</summary>
        void Speak(VoiceLineId line);

        /// <summary>Says <paramref name="sequence"/>, stopping whatever was being said.</summary>
        void SpeakSequence(VoiceSequence sequence);

        /// <summary>Stops at once.</summary>
        void Stop();
    }
}
