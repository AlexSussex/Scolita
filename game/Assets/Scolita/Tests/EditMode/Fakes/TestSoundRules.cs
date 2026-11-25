using Scolita.Core.Audio;

namespace Scolita.Tests.EditMode.Fakes
{
    /// <summary>Sound rules with round numbers, for the tests of <see cref="MusicDucking"/>, <see cref="SoundLevels"/> and <c>SoundPlayer</c>.</summary>
    public static class TestSoundRules
    {
        /// <summary>
        /// Ducked to 0.25 in 0.25 s, held 0.5 s, back in 1 s; effects at 0.5 under the voice; music fading in over 2 s.
        /// Called by <c>MusicDuckingTests</c>, <c>SoundLevelsTests</c> and <c>SoundPlayerTests</c>.
        /// </summary>
        public static SoundRules Default()
        {
            return new SoundRules(0.25f, 0.25f, 0.5f, 1f, 0.5f, 2f);
        }
    }
}
