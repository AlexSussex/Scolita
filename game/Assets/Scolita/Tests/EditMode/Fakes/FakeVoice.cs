using System;
using System.Collections.Generic;
using Scolita.Core.Content;
using Scolita.Core.Voice;

namespace Scolita.Tests.EditMode.Fakes
{
    /// <summary>An <see cref="IVoice"/> for the EditMode tests (the PlayMode tests have their own: the two test assemblies share no code): it records what it is asked to say, and starts and finishes when told.</summary>
    public sealed class FakeVoice : IVoice
    {
        private readonly List<string> spoken = new List<string>();

        /// <inheritdoc/>
        public event Action<VoiceLineId> Started;

        /// <inheritdoc/>
        public event Action<VoiceLineId> Finished;

        /// <summary>The lines it was asked to say, in order.</summary>
        public IReadOnlyList<string> Spoken => spoken;

        /// <inheritdoc/>
        public bool IsSpeaking { get; private set; }

        /// <inheritdoc/>
        public VoiceLineId Current { get; private set; }

        /// <inheritdoc/>
        public void Speak(VoiceLineId line)
        {
            spoken.Add(line.ToString());
            Begin(line);
        }

        /// <inheritdoc/>
        public void SpeakSequence(VoiceSequence sequence)
        {
            spoken.Add(sequence.Lead.ToString());
            Begin(sequence.Lead);
        }

        /// <inheritdoc/>
        public void Stop()
        {
            End();
        }

        /// <summary>Ends the line being said, as its end would; called by the EditMode tests.</summary>
        public void FinishSpeaking()
        {
            End();
        }

        private void Begin(VoiceLineId line)
        {
            End();
            Current = line;
            IsSpeaking = true;
            Started?.Invoke(line);
        }

        private void End()
        {
            if (!IsSpeaking)
            {
                return;
            }

            IsSpeaking = false;
            Finished?.Invoke(Current);
        }
    }
}
