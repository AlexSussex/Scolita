using System;
using NUnit.Framework;
using Scolita.Core.Audio;
using Scolita.Core.Content;
using Scolita.Core.Lessons;

namespace Scolita.Tests.EditMode.Core.Audio
{
    /// <summary>Tests <see cref="FeedbackEffects"/>.</summary>
    public sealed class FeedbackEffectsTests
    {
        /// <summary>Tests that <see cref="FeedbackEffects.Of"/> gives every outcome of every kind its sound, and a tick only to the right ones.</summary>
        [Test]
        public void TestOf_EveryKindAndOutcome()
        {
            foreach (ActivityKind kind in (ActivityKind[])Enum.GetValues(typeof(ActivityKind)))
            {
                Assert.That(FeedbackEffects.Of(kind, FeedbackOutcome.Right).Sound, Is.EqualTo(SoundCue.Sparkle), kind.ToString());
                Assert.That(FeedbackEffects.Of(kind, FeedbackOutcome.Right).Haptic, Is.EqualTo(Haptic.Tick), kind.ToString());
                Assert.That(FeedbackEffects.Of(kind, FeedbackOutcome.Done).Sound, Is.EqualTo(SoundCue.StepDone), kind.ToString());
                Assert.That(FeedbackEffects.Of(kind, FeedbackOutcome.Done).Haptic, Is.EqualTo(Haptic.Tick), kind.ToString());
                Assert.That(FeedbackEffects.Of(kind, FeedbackOutcome.TryAgain).Sound, Is.EqualTo(SoundCue.Boing), kind.ToString());
                Assert.That(FeedbackEffects.Of(kind, FeedbackOutcome.TryAgain).Haptic, Is.EqualTo(Haptic.None), kind.ToString());
                Assert.That(FeedbackEffects.Of(kind, FeedbackOutcome.ShowAnswer).Sound, Is.EqualTo(SoundCue.Glow), kind.ToString());
                Assert.That(FeedbackEffects.Of(kind, FeedbackOutcome.ShowAnswer).Haptic, Is.EqualTo(Haptic.None), kind.ToString());
                Assert.That(FeedbackEffects.Of(kind, FeedbackOutcome.Neutral).Haptic, Is.EqualTo(Haptic.None), kind.ToString());
            }
        }

        /// <summary>Tests that <see cref="FeedbackEffects.Of"/> makes a tap that isn't an answer the drum on <c>Syllables</c> and a soft tap elsewhere.</summary>
        [Test]
        public void TestOf_ANeutralTapIsTheDrumOnSyllables()
        {
            Assert.That(FeedbackEffects.Of(ActivityKind.Syllables, FeedbackOutcome.Neutral).Sound, Is.EqualTo(SoundCue.Drum));
            Assert.That(FeedbackEffects.Of(ActivityKind.ListenAndPick, FeedbackOutcome.Neutral).Sound, Is.EqualTo(SoundCue.Tap));
            Assert.That(FeedbackEffects.Of(ActivityKind.TraceLetter, FeedbackOutcome.Neutral).Sound, Is.EqualTo(SoundCue.Tap));
        }

        /// <summary>Tests that <see cref="FeedbackEffects.Of"/> refuses an outcome it doesn't know.</summary>
        [Test]
        public void TestOf_AnUnknownOutcomeThrows()
        {
            Assert.That(() => FeedbackEffects.Of(ActivityKind.MeetLetter, (FeedbackOutcome)99),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }
    }
}
