using System;
using Scolita.Core.Audio;
using UnityEngine;

namespace Scolita.Game.Audio
{
    /// <summary>One music loop of the <see cref="SoundBank"/>: its <see cref="MusicPiece"/> and its streamed clip.</summary>
    [Serializable]
    public sealed class SoundBankMusic
    {
        [SerializeField] private MusicPiece piece;
        [SerializeField] private AudioClip clip;

        /// <summary>Pairs <paramref name="piece"/> with its clip; called by the Editor's sound maker and the tests.</summary>
        public SoundBankMusic(MusicPiece piece, AudioClip clip)
        {
            this.piece = piece;
            this.clip = clip;
        }

        /// <summary>The piece.</summary>
        public MusicPiece Piece => piece;

        /// <summary>Its loop.</summary>
        public AudioClip Clip => clip;
    }
}
