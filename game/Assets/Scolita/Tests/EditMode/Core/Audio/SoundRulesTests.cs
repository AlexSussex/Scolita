using System;
using NUnit.Framework;
using Scolita.Core.Audio;

namespace Scolita.Tests.EditMode.Core.Audio
{
    /// <summary>Tests <see cref="SoundRules"/>.</summary>
    public sealed class SoundRulesTests
    {
        /// <summary>Tests that the <see cref="SoundRules"/> constructor keeps numbers in their ranges.</summary>
        [Test]
        public void TestSoundRules_KeepsTheNumbers()
        {
            SoundRules rules = new SoundRules(0.3f, 0.2f, 0.4f, 0.9f, 0.6f, 1.5f);

            Assert.That(rules.DuckedLevel, Is.EqualTo(0.3f));
            Assert.That(rules.DuckSeconds, Is.EqualTo(0.2f));
            Assert.That(rules.HoldSeconds, Is.EqualTo(0.4f));
            Assert.That(rules.RecoverSeconds, Is.EqualTo(0.9f));
            Assert.That(rules.EffectsUnderVoice, Is.EqualTo(0.6f));
            Assert.That(rules.MusicFadeInSeconds, Is.EqualTo(1.5f));
        }

        /// <summary>Tests that the <see cref="SoundRules"/> constructor refuses a level outside 0–1 and a negative or missing time.</summary>
        [Test]
        public void TestSoundRules_RefusesNumbersOutOfRange()
        {
            Assert.That(() => new SoundRules(1.2f, 0.2f, 0.4f, 0.9f, 0.6f, 1.5f), Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => new SoundRules(0.3f, -0.1f, 0.4f, 0.9f, 0.6f, 1.5f), Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => new SoundRules(0.3f, 0.2f, float.NaN, 0.9f, 0.6f, 1.5f), Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => new SoundRules(0.3f, 0.2f, 0.4f, 0.9f, -0.6f, 1.5f), Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => new SoundRules(0.3f, 0.2f, 0.4f, 0.9f, 0.6f, -1f), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        /// <summary>Tests that <see cref="MusicDucking"/>'s constructor refuses no rules.</summary>
        [Test]
        public void TestMusicDucking_RefusesNoRules()
        {
            Assert.That(() => new MusicDucking(null), Throws.TypeOf<ArgumentNullException>());
        }
    }
}
