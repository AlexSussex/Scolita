using System;
using Scolita.Core.Activities;
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
        /// The effect of <paramref name="answer"/> in a step of <paramref name="kind"/>, judged <paramref name="outcome"/>: right → <see cref="SoundCue.Sparkle"/>
        /// and a tick; the step done → <see cref="SoundCue.StepDone"/> and a tick; a miss → <see cref="SoundCue.Boing"/>; the answer
        /// shown → <see cref="SoundCue.Glow"/>; a tap that isn't an answer → nothing (the tapped letter or word is heard
        /// instead, and no effect may cover its sound), except the <see cref="SoundCue.Drum"/> on
        /// <see cref="ActivityKind.Syllables"/> when the drum was tapped (each beat is such a tap). Throws an
        /// <see cref="ArgumentOutOfRangeException"/> for an outcome it doesn't know. Called by <c>LessonRunner.Answer</c> and
        /// <c>FeedbackEffectsTests</c>.
        /// </summary>
        public static FeedbackEffect Of(ActivityKind kind, in ActivityAnswer answer, FeedbackOutcome outcome)
        {
            switch (outcome)
            {
                case FeedbackOutcome.Neutral:
                    return IsDrumBeat(kind, answer) ? new FeedbackEffect(SoundCue.Drum, Haptic.None) : FeedbackEffect.Nothing;
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

        private static bool IsDrumBeat(ActivityKind kind, in ActivityAnswer answer)
        {
            return kind == ActivityKind.Syllables && answer.Gesture == AnswerGesture.Tap && answer.Choice == SyllablesRules.Drum;
        }
    }
}
