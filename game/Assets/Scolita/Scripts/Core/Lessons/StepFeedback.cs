using Scolita.Core.Activities;
using Scolita.Core.Voice;

namespace Scolita.Core.Lessons
{
    /// <summary>
    /// What one answer gives back (<see cref="LessonRun.Answer"/>): the outcome, the choice tapped,
    /// the hint (its cue and its choice) on a miss, and the lines to say, one after the other. A
    /// value whose lines were prepared when the lesson was built, so an answer allocates nothing.
    /// </summary>
    public readonly struct StepFeedback
    {
        /// <summary>Creates feedback; called by <see cref="LessonRun"/>.</summary>
        public StepFeedback(FeedbackOutcome outcome, int choice, ActivityHint hint, VoiceSequence first, VoiceSequence then)
        {
            Outcome = outcome;
            Choice = choice;
            Hint = hint;
            First = first;
            Then = then;
        }

        /// <summary>The outcome.</summary>
        public FeedbackOutcome Outcome { get; }

        /// <summary>The choice she tapped.</summary>
        public int Choice { get; }

        /// <summary>The hint, for <see cref="FeedbackOutcome.TryAgain"/> and <see cref="FeedbackOutcome.ShowAnswer"/>.</summary>
        public ActivityHint Hint { get; }

        /// <summary>The first line to say; null for none.</summary>
        public VoiceSequence First { get; }

        /// <summary>The line to say once <see cref="First"/> has been said (the praise); null for none.</summary>
        public VoiceSequence Then { get; }
    }
}
