namespace Scolita.Core.Audio
{
    /// <summary>What one answer sounds and feels like: its <see cref="SoundCue"/> (or none) and its <see cref="Haptic"/> (<see cref="FeedbackEffects.Of"/>).</summary>
    public readonly struct FeedbackEffect
    {
        /// <summary>Pairs <paramref name="sound"/> with <paramref name="haptic"/>; called by <see cref="FeedbackEffects.Of"/>.</summary>
        public FeedbackEffect(SoundCue sound, Haptic haptic)
        {
            HasSound = true;
            Sound = sound;
            Haptic = haptic;
        }

        /// <summary>No sound and no vibration; called by <see cref="FeedbackEffects.Of"/>.</summary>
        public static FeedbackEffect Nothing => default;

        /// <summary>Whether a sound is played (<see cref="Sound"/>).</summary>
        public bool HasSound { get; }

        /// <summary>The sound played, when <see cref="HasSound"/>.</summary>
        public SoundCue Sound { get; }

        /// <summary>The vibration, when the switch is on.</summary>
        public Haptic Haptic { get; }
    }
}
