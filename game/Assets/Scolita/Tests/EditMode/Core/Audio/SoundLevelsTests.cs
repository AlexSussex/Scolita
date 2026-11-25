using NUnit.Framework;
using Scolita.Core.Audio;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Audio
{
    /// <summary>Tests <see cref="SoundLevels"/>.</summary>
    public sealed class SoundLevelsTests
    {
        private const float Tolerance = 0.0001f;

        /// <summary>Tests that <see cref="SoundLevels.MusicAt"/> is the music slider times the ducking's level.</summary>
        [Test]
        public void TestMusicAt_TheSliderTimesTheDucking()
        {
            SoundLevels levels = new SoundLevels(0.8f, 0.4f);

            Assert.That(levels.MusicAt(1f), Is.EqualTo(0.8f).Within(Tolerance));
            Assert.That(levels.MusicAt(0.25f), Is.EqualTo(0.2f).Within(Tolerance));
            Assert.That(levels.MusicAt(2f), Is.EqualTo(0.8f).Within(Tolerance));
        }

        /// <summary>Tests that <see cref="SoundLevels.EffectAt"/> is the sounds slider, lower while the voice speaks, and never the music's.</summary>
        [Test]
        public void TestEffectAt_TheSoundsSliderLowerUnderTheVoice()
        {
            SoundLevels levels = new SoundLevels(0.8f, 0.4f);
            SoundRules rules = TestSoundRules.Default();

            Assert.That(levels.EffectAt(false, rules), Is.EqualTo(0.4f).Within(Tolerance));
            Assert.That(levels.EffectAt(true, rules), Is.EqualTo(0.2f).Within(Tolerance));
        }

        /// <summary>Tests that a slider at 0 plays nothing (<see cref="SoundLevels.PlaysMusic"/>, <see cref="SoundLevels.PlaysEffects"/>) and the other still plays.</summary>
        [Test]
        public void TestPlays_ASliderAtZeroPlaysNothing()
        {
            SoundLevels noMusic = new SoundLevels(0f, 1f);
            SoundLevels noSounds = new SoundLevels(1f, 0f);

            Assert.That(noMusic.PlaysMusic, Is.False);
            Assert.That(noMusic.PlaysEffects, Is.True);
            Assert.That(noSounds.PlaysMusic, Is.True);
            Assert.That(noSounds.PlaysEffects, Is.False);
            Assert.That(noSounds.EffectAt(false, TestSoundRules.Default()), Is.EqualTo(0f));
        }

        /// <summary>Tests that the <see cref="SoundLevels"/> constructor clamps the sliders to 0–1 and reads not a number as 0.</summary>
        [Test]
        public void TestSoundLevels_SlidersAreClamped()
        {
            SoundLevels levels = new SoundLevels(1.5f, float.NaN);
            SoundLevels negative = new SoundLevels(-1f, 0.5f);

            Assert.That(levels.MusicVolume, Is.EqualTo(1f));
            Assert.That(levels.SoundsVolume, Is.EqualTo(0f));
            Assert.That(negative.MusicVolume, Is.EqualTo(0f));
            Assert.That(negative.SoundsVolume, Is.EqualTo(0.5f));
        }
    }
}
