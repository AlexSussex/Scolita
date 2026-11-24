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
        GlowAnswer,

        /// <summary>Every right choice not done yet twinkles for a moment (<c>LetterHunt</c>'s stars).</summary>
        TwinkleAnswers,

        /// <summary>
        /// The subject word's letters light one by one while their sounds are said, and the wrong choice she
        /// tapped fades (<c>ReadAndMatch</c>).
        /// </summary>
        SpellOut,

        /// <summary>
        /// The pictures of the choice in play (<see cref="ActivityRound.Current"/>) and of its partner
        /// (<see cref="ActivityRound.Answer"/>) appear (<c>WordTwins</c>).
        /// </summary>
        RevealPictures
    }
}
