using Scolita.Core.Audio;

namespace Scolita.Game.Audio
{
    /// <summary>
    /// What a scene's parts play and make her feel, through the scene's <see cref="SoundPlayer"/>, with the tablet's sounds
    /// volume and vibration switch. Tests use a fake.
    /// </summary>
    public interface ISoundEffects
    {
        /// <summary>Plays <paramref name="cue"/> (nothing when the sounds slider is at 0).</summary>
        void Play(SoundCue cue);

        /// <summary>Makes the tablet give <paramref name="haptic"/> (nothing when the vibration switch is off).</summary>
        void Vibrate(Haptic haptic);
    }
}
