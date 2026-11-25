using System;
using Scolita.Core.Content;
using Scolita.Core.Lessons;

namespace Scolita.Core.Audio
{
    /// <summary>
    /// The one owner of what an answer sounds and feels like (<c>docs/prd.md</c> → "Feedback": a happy sound and a
    /// sparkle for a right answer, a soft sound for a miss), for every activity kind, so the activity views don't
    /// choose sounds: the lesson's composition root plays what this says after each answer.
    /// </summary>
    public static class FeedbackEffects
    {
        /// <summary>
        /// The effect of an answer of <paramref name="kind"/> with <paramref name="outcome"/>: right → <see cref="SoundCue.Sparkle"/>
        /// and a tick; the step done → <see cref="SoundCue.StepDone"/> and a tick; a miss → <see cref="SoundCue.Boing"/>; the answer
        /// shown → <see cref="SoundCue.Glow"/>; a tap that isn't an answer → <see cref="SoundCue.Tap"/>, or the
        /// <see cref="SoundCue.Drum"/> on <see cref="ActivityKind.Syllables"/> (each beat is such a tap). Throws an
        /// <see cref="ArgumentOutOfRangeException"/> for an outcome it doesn't know. Called by <c>LessonRunner.Answer</c> and
        /// <c>FeedbackEffectsTests</c>.
        /// </summary>
        public static FeedbackEffect Of(ActivityKind kind, FeedbackOutcome outcome)
        {
            switch (outcome)
            {
                case FeedbackOutcome.Neutral:
                    return new FeedbackEffect(kind == ActivityKind.Syllables ? SoundCue.Drum : SoundCue.Tap, Haptic.None);
                case FeedbackOutcome.Right:
                    return new FeedbackEffect(SoundCue.Sparkle, Haptic.Tick);
                case FeedbackOutcome.TryAgain:
                    return new FeedbackEffect(SoundCue.Boing, Haptic.None);
                case FeedbackOutcome.ShowAnswer:
                    return new FeedbackEffect(SoundCue.Glow, Haptic.None);
                case FeedbackOutcome.Done:
                    return new FeedbackEffect(SoundCue.StepDone, Haptic.Tick);
                default:
                    throw new ArgumentOutOfRangeException(nameof(outcome), outcome, "An answer's outcome without an effect.");
            }
        }
    }
}
