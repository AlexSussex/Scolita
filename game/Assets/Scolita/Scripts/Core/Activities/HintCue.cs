namespace Scolita.Core.Activities
{
    /// <summary>What a hint shows on screen besides what it says (<see cref="ActivityHint.Cue"/>).</summary>
    public enum HintCue
    {
        /// <summary>Nothing beyond the line.</summary>
        None,

        /// <summary>The wrong choice she tapped fades.</summary>
        FadeChoice,

        /// <summary>The right choice glows yellow: the answer is shown.</summary>
        GlowAnswer
    }
}
