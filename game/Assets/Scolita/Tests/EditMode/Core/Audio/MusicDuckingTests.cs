using NUnit.Framework;
using Scolita.Core.Audio;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Audio
{
    /// <summary>Tests <see cref="MusicDucking"/>.</summary>
    public sealed class MusicDuckingTests
    {
        private const float Tolerance = 0.0001f;

        /// <summary>Tests that <see cref="MusicDucking.Tick"/> leaves the music at full level while nobody speaks.</summary>
        [Test]
        public void TestTick_FullWhileNobodySpeaks()
        {
            MusicDucking ducking = new MusicDucking(TestSoundRules.Default());

            ducking.Tick(1f, false);

            Assert.That(ducking.Level, Is.EqualTo(1f));
            Assert.That(ducking.IsDucked, Is.False);
        }

        /// <summary>Tests that <see cref="MusicDucking.Tick"/> brings the music down to the ducked level over the duck time.</summary>
        [Test]
        public void TestTick_GoesDownWhileTheVoiceSpeaks()
        {
            MusicDucking ducking = new MusicDucking(TestSoundRules.Default());

            ducking.Tick(0.125f, true);
            float halfway = ducking.Level;
            ducking.Tick(0.125f, true);
            float down = ducking.Level;
            ducking.Tick(1f, true);

            Assert.That(halfway, Is.EqualTo(0.625f).Within(Tolerance));
            Assert.That(down, Is.EqualTo(0.25f).Within(Tolerance));
            Assert.That(ducking.Level, Is.EqualTo(0.25f).Within(Tolerance));
            Assert.That(ducking.IsDucked, Is.True);
        }

        /// <summary>Tests that <see cref="MusicDucking.Tick"/> keeps the music down for the hold time after the voice ends, then brings it back.</summary>
        [Test]
        public void TestTick_HoldsThenComesBack()
        {
            MusicDucking ducking = new MusicDucking(TestSoundRules.Default());
            ducking.Tick(1f, true);

            ducking.Tick(0.4f, false);
            float holding = ducking.Level;
            ducking.Tick(0.6f, false);
            float recovering = ducking.Level;
            ducking.Tick(1f, false);

            Assert.That(holding, Is.EqualTo(0.25f).Within(Tolerance));
            Assert.That(recovering, Is.EqualTo(0.625f).Within(Tolerance));
            Assert.That(ducking.Level, Is.EqualTo(1f));
            Assert.That(ducking.IsDucked, Is.False);
        }

        /// <summary>Tests that <see cref="MusicDucking.Tick"/> keeps the music down when a new line starts during the hold.</summary>
        [Test]
        public void TestTick_ANewLineDuringTheHoldKeepsItDown()
        {
            MusicDucking ducking = new MusicDucking(TestSoundRules.Default());
            ducking.Tick(1f, true);
            ducking.Tick(0.4f, false);

            ducking.Tick(0.2f, true);
            ducking.Tick(0.4f, false);

            Assert.That(ducking.Level, Is.EqualTo(0.25f).Within(Tolerance));
        }

        /// <summary>Tests that <see cref="MusicDucking.Tick"/> counts a negative time as none.</summary>
        [Test]
        public void TestTick_ANegativeTimeChangesNothing()
        {
            MusicDucking ducking = new MusicDucking(TestSoundRules.Default());

            ducking.Tick(-1f, true);

            Assert.That(ducking.Level, Is.EqualTo(1f));
        }

        /// <summary>Tests that <see cref="MusicDucking.FadeIn"/> starts the music from silence and raises it over the fade-in time, ducked on top.</summary>
        [Test]
        public void TestFadeIn_RisesFromSilenceUnderTheDucking()
        {
            MusicDucking ducking = new MusicDucking(TestSoundRules.Default());

            ducking.FadeIn();
            float silent = ducking.Level;
            ducking.Tick(1f, false);
            float half = ducking.Level;
            ducking.Tick(0.5f, true);

            Assert.That(silent, Is.EqualTo(0f));
            Assert.That(half, Is.EqualTo(0.5f).Within(Tolerance));
            Assert.That(ducking.Level, Is.EqualTo(0.75f * 0.25f).Within(Tolerance));
        }

        /// <summary>Tests that <see cref="MusicDucking.Tick"/> jumps at once when the ramps are 0 seconds long.</summary>
        [Test]
        public void TestTick_ZeroRampsJump()
        {
            MusicDucking ducking = new MusicDucking(new SoundRules(0.3f, 0f, 0f, 0f, 1f, 0f));

            ducking.FadeIn();
            ducking.Tick(0.01f, true);
            float down = ducking.Level;
            ducking.Tick(0.01f, false);

            Assert.That(down, Is.EqualTo(0.3f).Within(Tolerance));
            Assert.That(ducking.Level, Is.EqualTo(1f));
        }
    }
}
