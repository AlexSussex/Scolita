namespace Scolita.Core.Audio
{
    /// <summary>What one answer sounds and feels like: its <see cref="SoundCue"/> and its <see cref="Haptic"/> (<see cref="FeedbackEffects.Of"/>).</summary>
    public readonly struct FeedbackEffect
    {
        /// <summary>Pairs <paramref name="sound"/> with <paramref name="haptic"/>; called by <see cref="FeedbackEffects.Of"/>.</summary>
        public FeedbackEffect(SoundCue sound, Haptic haptic)
        {
            Sound = sound;
            Haptic = haptic;
        }

        /// <summary>The sound played.</summary>
        public SoundCue Sound { get; }

        /// <summary>The vibration, when the switch is on.</summary>
        public Haptic Haptic { get; }
    }
}
