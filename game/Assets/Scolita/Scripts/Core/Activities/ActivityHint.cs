using Scolita.Core.Voice;

namespace Scolita.Core.Activities
{
    /// <summary>
    /// A hint after a miss, or the answer shown: the line the mascot says, what the screen shows
    /// (<see cref="HintCue"/>) and on which choice. Made by an activity's rules
    /// (<see cref="IActivityRules.HintFor"/>, <see cref="IActivityRules.AnswerShown"/>) from lines
    /// prepared when the lesson is built, so giving one allocates nothing.
    /// </summary>
    public readonly struct ActivityHint
    {
        /// <summary>Creates a hint; called by the activities' rules.</summary>
        public ActivityHint(VoiceSequence line, HintCue cue, int choice)
        {
            Line = line;
            Cue = cue;
            Choice = choice;
        }

        /// <summary>What the mascot says.</summary>
        public VoiceSequence Line { get; }

        /// <summary>What the screen shows.</summary>
        public HintCue Cue { get; }

        /// <summary>The choice the cue is on (the one to fade, or the answer to glow); -1 for none.</summary>
        public int Choice { get; }
    }
}
