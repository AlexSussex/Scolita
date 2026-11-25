using System;
using System.Collections.Generic;
using Scolita.Core.Audio;
using UnityEngine;

namespace Scolita.Game.Audio
{
    /// <summary>One effect of the <see cref="SoundBank"/>: its <see cref="SoundCue"/> and its takes, played in turn.</summary>
    [Serializable]
    public sealed class SoundBankEffect
    {
        [SerializeField] private SoundCue cue;
        [SerializeField] private AudioClip[] clips;

        /// <summary>Pairs <paramref name="cue"/> with its takes; called by the Editor's sound maker and the tests.</summary>
        public SoundBankEffect(SoundCue cue, AudioClip[] clips)
        {
            this.cue = cue;
            this.clips = clips ?? new AudioClip[0];
        }

        /// <summary>The cue.</summary>
        public SoundCue Cue => cue;

        /// <summary>Its takes (one or more).</summary>
        public IReadOnlyList<AudioClip> Clips => clips;
    }
}
