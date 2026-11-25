using System;
using NUnit.Framework;
using Scolita.Core.Activities;
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
                Assert.That(FeedbackEffects.Of(kind, ActivityAnswer.Tap(1), FeedbackOutcome.Right).HasSound, Is.True, kind.ToString());
                Assert.That(FeedbackEffects.Of(kind, ActivityAnswer.Tap(1), FeedbackOutcome.Right).Sound, Is.EqualTo(SoundCue.Sparkle), kind.ToString());
                Assert.That(FeedbackEffects.Of(kind, ActivityAnswer.Tap(1), FeedbackOutcome.Right).Haptic, Is.EqualTo(Haptic.Tick), kind.ToString());
                Assert.That(FeedbackEffects.Of(kind, ActivityAnswer.Tap(1), FeedbackOutcome.Done).Sound, Is.EqualTo(SoundCue.StepDone), kind.ToString());
                Assert.That(FeedbackEffects.Of(kind, ActivityAnswer.Tap(1), FeedbackOutcome.Done).Haptic, Is.EqualTo(Haptic.Tick), kind.ToString());
                Assert.That(FeedbackEffects.Of(kind, ActivityAnswer.Tap(1), FeedbackOutcome.TryAgain).Sound, Is.EqualTo(SoundCue.Boing), kind.ToString());
                Assert.That(FeedbackEffects.Of(kind, ActivityAnswer.Tap(1), FeedbackOutcome.TryAgain).Haptic, Is.EqualTo(Haptic.None), kind.ToString());
                Assert.That(FeedbackEffects.Of(kind, ActivityAnswer.Tap(1), FeedbackOutcome.ShowAnswer).Sound, Is.EqualTo(SoundCue.Glow), kind.ToString());
                Assert.That(FeedbackEffects.Of(kind, ActivityAnswer.Tap(1), FeedbackOutcome.ShowAnswer).Haptic, Is.EqualTo(Haptic.None), kind.ToString());
                Assert.That(FeedbackEffects.Of(kind, ActivityAnswer.Tap(1), FeedbackOutcome.Neutral).Haptic, Is.EqualTo(Haptic.None), kind.ToString());
            }
        }

        /// <summary>Tests that <see cref="FeedbackEffects.Of"/> makes a tap on <c>Syllables</c>' drum the drum, and every other tap that isn't an answer silent (the tapped thing is heard).</summary>
        [Test]
        public void TestOf_ANeutralTapIsTheDrumOnSyllablesElseSilent()
        {
            Assert.That(FeedbackEffects.Of(ActivityKind.Syllables, ActivityAnswer.Tap(SyllablesRules.Drum), FeedbackOutcome.Neutral).HasSound, Is.True);
            Assert.That(FeedbackEffects.Of(ActivityKind.Syllables, ActivityAnswer.Tap(SyllablesRules.Drum), FeedbackOutcome.Neutral).Sound, Is.EqualTo(SoundCue.Drum));
            Assert.That(FeedbackEffects.Of(ActivityKind.Syllables, ActivityAnswer.Tap(SyllablesRules.Tick), FeedbackOutcome.Neutral).HasSound,
                Is.False);
            Assert.That(FeedbackEffects.Of(ActivityKind.ListenAndPick, ActivityAnswer.Tap(0), FeedbackOutcome.Neutral).HasSound, Is.False);
            Assert.That(FeedbackEffects.Of(ActivityKind.MeetLetter, ActivityAnswer.Tap(0), FeedbackOutcome.Neutral).HasSound, Is.False);
            Assert.That(FeedbackEffects.Of(ActivityKind.TraceLetter, ActivityAnswer.Tap(0), FeedbackOutcome.Neutral).HasSound, Is.False);
        }

        /// <summary>Tests that <see cref="FeedbackEffects.Of"/> refuses an outcome it doesn't know.</summary>
        [Test]
        public void TestOf_AnUnknownOutcomeThrows()
        {
            Assert.That(() => FeedbackEffects.Of(ActivityKind.MeetLetter, ActivityAnswer.Tap(0), (FeedbackOutcome)99),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }
    }
}
