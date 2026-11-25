namespace Scolita.Core.Audio
{
    /// <summary>
    /// The volume routing (the tablet's settings, set in the parent area): the music plays at the music slider × the
    /// ducking, an effect at the sounds slider (lower while the voice speaks, <see cref="SoundRules.EffectsUnderVoice"/>),
    /// and a slider at 0 plays nothing at all. The voice has its own slider and isn't routed here. No allocation.
    /// </summary>
    public readonly struct SoundLevels
    {
        /// <summary>
        /// Creates the levels from the sliders, each clamped to 0–1 (0 when not a number). Called by <c>SoundPlayer</c> and
        /// <c>SoundLevelsTests</c>.
        /// </summary>
        public SoundLevels(float musicVolume, float soundsVolume)
        {
            MusicVolume = Clamped(musicVolume);
            SoundsVolume = Clamped(soundsVolume);
        }

        /// <summary>The music slider, 0–1.</summary>
        public float MusicVolume { get; }

        /// <summary>The sounds slider, 0–1.</summary>
        public float SoundsVolume { get; }

        /// <summary>Whether any music plays (its slider above 0).</summary>
        public bool PlaysMusic => MusicVolume > 0f;

        /// <summary>Whether any effect plays (its slider above 0).</summary>
        public bool PlaysEffects => SoundsVolume > 0f;

        /// <summary>
        /// The music's volume at the ducking's <paramref name="duckingLevel"/> (<see cref="MusicDucking.Level"/>). Called by
        /// <c>SoundPlayer</c> every frame, and <c>SoundLevelsTests</c>.
        /// </summary>
        public float MusicAt(float duckingLevel)
        {
            return MusicVolume * Clamped(duckingLevel);
        }

        /// <summary>
        /// An effect's volume: the sounds slider, × <see cref="SoundRules.EffectsUnderVoice"/> while the voice speaks.
        /// Called by <c>SoundPlayer</c> when an effect plays, and <c>SoundLevelsTests</c>.
        /// </summary>
        public float EffectAt(bool voiceSpeaking, SoundRules rules)
        {
            return voiceSpeaking ? SoundsVolume * rules.EffectsUnderVoice : SoundsVolume;
        }

        private static float Clamped(float volume)
        {
            if (float.IsNaN(volume) || volume <= 0f)
            {
                return 0f;
            }

            return volume >= 1f ? 1f : volume;
        }
    }
}
