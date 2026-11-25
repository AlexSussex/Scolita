using System;

namespace Scolita.Core.Audio
{
    /// <summary>
    /// The ducking rule (AGENTS.md → "Audio": the music ducks while the voice speaks): the level the music plays at, of
    /// its own volume. While the voice speaks it goes down to <see cref="SoundRules.DuckedLevel"/> in
    /// <see cref="SoundRules.DuckSeconds"/>; once the voice ends it stays down for <see cref="SoundRules.HoldSeconds"/>
    /// (a new line in that time keeps it down), then comes back to 1 in <see cref="SoundRules.RecoverSeconds"/>. It
    /// also fades a scene's music in from silence (<see cref="SoundRules.MusicFadeInSeconds"/>). No allocation.
    /// </summary>
    public sealed class MusicDucking
    {
        private readonly SoundRules rules;
        private float duck = 1f;
        private float fadeIn = 1f;
        private float quietFor;

        /// <summary>
        /// Starts at full level with no fade-in (<see cref="FadeIn"/> starts one) and no hold. Called by <c>SoundPlayer</c>
        /// and <c>MusicDuckingTests</c>.
        /// </summary>
        public MusicDucking(SoundRules rules)
        {
            this.rules = rules ?? throw new ArgumentNullException(nameof(rules));
            quietFor = rules.HoldSeconds;
        }

        /// <summary>The music's level now, from 0 to 1, of its own volume: the ducking times the fade-in.</summary>
        public float Level => duck * fadeIn;

        /// <summary>Whether the music is down (or still coming back up) for the voice.</summary>
        public bool IsDucked => duck < 1f;

        /// <summary>
        /// Starts the music from silence, rising to its level over <see cref="SoundRules.MusicFadeInSeconds"/>. Called by
        /// <c>SoundPlayer</c> when a piece starts, and <c>MusicDuckingTests</c>.
        /// </summary>
        public void FadeIn()
        {
            fadeIn = rules.MusicFadeInSeconds > 0f ? 0f : 1f;
        }

        /// <summary>
        /// Moves the level on by <paramref name="seconds"/> (a negative time counts as none), with the voice speaking or
        /// not. Called by <c>SoundPlayer</c> every frame, and <c>MusicDuckingTests</c>.
        /// </summary>
        public void Tick(float seconds, bool voiceSpeaking)
        {
            float step = MathF.Max(0f, seconds);
            float span = 1f - rules.DuckedLevel;
            fadeIn = Toward(fadeIn, 1f, step, rules.MusicFadeInSeconds, 1f);
            if (voiceSpeaking)
            {
                quietFor = 0f;
                duck = Toward(duck, rules.DuckedLevel, step, rules.DuckSeconds, span);
                return;
            }

            float holdLeft = MathF.Max(0f, rules.HoldSeconds - quietFor);
            quietFor = MathF.Min(quietFor + step, rules.HoldSeconds);
            duck = Toward(duck, 1f, MathF.Max(0f, step - holdLeft), rules.RecoverSeconds, span);
        }

        private static float Toward(float level, float target, float seconds, float rampSeconds, float span)
        {
            if (rampSeconds <= 0f || span <= 0f)
            {
                return target;
            }

            float change = span * seconds / rampSeconds;
            return level < target ? MathF.Min(target, level + change) : MathF.Max(target, level - change);
        }
    }
}
