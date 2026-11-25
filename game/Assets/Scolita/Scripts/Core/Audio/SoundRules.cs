using System;

namespace Scolita.Core.Audio
{
    /// <summary>
    /// The numbers of the music's ducking and of the effects under the voice (filled from the learning rules asset, as
    /// every tuning number): how low the music goes while the voice speaks and how fast, how long it waits after the voice
    /// before it comes back, how quiet an effect is while the voice speaks, and how slowly a scene's music starts.
    /// </summary>
    public sealed class SoundRules
    {
        /// <summary>
        /// Creates the numbers; throws an <see cref="ArgumentOutOfRangeException"/> for a level outside 0–1 or a time below
        /// 0 (or not a number). Called by the learning rules asset and the tests.
        /// </summary>
        public SoundRules(float duckedLevel, float duckSeconds, float holdSeconds, float recoverSeconds,
            float effectsUnderVoice, float musicFadeInSeconds)
        {
            RequireLevel(duckedLevel, nameof(duckedLevel));
            RequireTime(duckSeconds, nameof(duckSeconds));
            RequireTime(holdSeconds, nameof(holdSeconds));
            RequireTime(recoverSeconds, nameof(recoverSeconds));
            RequireLevel(effectsUnderVoice, nameof(effectsUnderVoice));
            RequireTime(musicFadeInSeconds, nameof(musicFadeInSeconds));
            DuckedLevel = duckedLevel;
            DuckSeconds = duckSeconds;
            HoldSeconds = holdSeconds;
            RecoverSeconds = recoverSeconds;
            EffectsUnderVoice = effectsUnderVoice;
            MusicFadeInSeconds = musicFadeInSeconds;
        }

        /// <summary>The music's level while the voice speaks, of its own volume (0–1).</summary>
        public float DuckedLevel { get; }

        /// <summary>How long the music takes to go down to <see cref="DuckedLevel"/> when the voice starts.</summary>
        public float DuckSeconds { get; }

        /// <summary>How long the music stays down after the voice ends, so it doesn't pump between two lines.</summary>
        public float HoldSeconds { get; }

        /// <summary>How long the music takes to come back to its volume after <see cref="HoldSeconds"/>.</summary>
        public float RecoverSeconds { get; }

        /// <summary>An effect's level while the voice speaks, of the sounds volume (0–1).</summary>
        public float EffectsUnderVoice { get; }

        /// <summary>How long a scene's music takes to rise from silence to its volume.</summary>
        public float MusicFadeInSeconds { get; }

        private static void RequireLevel(float level, string name)
        {
            if (float.IsNaN(level) || level < 0f || level > 1f)
            {
                throw new ArgumentOutOfRangeException(name, level, "A level is from 0 to 1.");
            }
        }

        private static void RequireTime(float seconds, string name)
        {
            if (float.IsNaN(seconds) || seconds < 0f)
            {
                throw new ArgumentOutOfRangeException(name, seconds, "A time is 0 seconds or more.");
            }
        }
    }
}
