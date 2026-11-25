using System.Collections.Generic;
using Scolita.Core.Audio;
using UnityEngine;

namespace Scolita.Game.Audio
{
    /// <summary>
    /// Every effect's takes and every music loop (<c>Audio/SoundBank.asset</c>, filled by Școlița → Make sounds), read by
    /// each scene's <see cref="SoundPlayer"/>. Looking a cue up doesn't allocate.
    /// </summary>
    [CreateAssetMenu(fileName = "SoundBank", menuName = "Scolita/Sound bank")]
    public sealed class SoundBank : ScriptableObject
    {
        [SerializeField] private SoundBankEffect[] effects = new SoundBankEffect[0];
        [SerializeField] private SoundBankMusic[] music = new SoundBankMusic[0];

        /// <summary>The effects, a cue each.</summary>
        public IReadOnlyList<SoundBankEffect> Effects => effects;

        /// <summary>The music loops, a piece each.</summary>
        public IReadOnlyList<SoundBankMusic> Music => music;

        /// <summary>
        /// The take number <paramref name="take"/> of <paramref name="cue"/> (counting round its takes); null when the bank has no
        /// clip for it. Called by <see cref="SoundPlayer"/> when an effect plays, and the tests.
        /// </summary>
        public AudioClip Take(SoundCue cue, int take)
        {
            for (int index = 0; index < effects.Length; index++)
            {
                IReadOnlyList<AudioClip> clips = effects[index].Clips;
                if (effects[index].Cue == cue && clips.Count > 0)
                {
                    return clips[(take % clips.Count + clips.Count) % clips.Count];
                }
            }

            return null;
        }

        /// <summary>The loop of <paramref name="piece"/>; null for <see cref="MusicPiece.None"/> or a piece the bank lacks. Called by <see cref="SoundPlayer"/> and the tests.</summary>
        public AudioClip LoopOf(MusicPiece piece)
        {
            for (int index = 0; index < music.Length; index++)
            {
                if (music[index].Piece == piece)
                {
                    return music[index].Clip;
                }
            }

            return null;
        }

        /// <summary>Replaces the bank's effects and music; called by the Editor's sound maker.</summary>
        public void Fill(SoundBankEffect[] madeEffects, SoundBankMusic[] madeMusic)
        {
            effects = madeEffects;
            music = madeMusic;
        }
    }
}
