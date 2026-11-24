using System.Collections.Generic;
using Scolita.Core.Activities;
using Scolita.Core.Lessons;

namespace Scolita.UI.Lesson
{
    /// <summary>
    /// The feedback of a row of <see cref="ChoiceTile"/> cards, one of which is the answer
    /// (<c>ListenAndPick</c>, <c>FirstSound</c>): a miss wobbles the card and fades it, the answer shown
    /// glows yellow, the right one sparkles, a card heard bounces. One place, so both kinds feel the same.
    /// </summary>
    public static class ChoiceTileFeedback
    {
        /// <summary>
        /// Shows <paramref name="feedback"/> on <paramref name="tiles"/>; called by
        /// <see cref="ListenAndPickView"/> and <see cref="FirstSoundView"/>.
        /// </summary>
        public static void Show(IReadOnlyList<ChoiceTile> tiles, in StepFeedback feedback)
        {
            if (feedback.Choice < 0 || feedback.Choice >= tiles.Count)
            {
                return;
            }

            ChoiceTile tapped = tiles[feedback.Choice];
            switch (feedback.Outcome)
            {
                case FeedbackOutcome.TryAgain:
                case FeedbackOutcome.ShowAnswer:
                    tapped.Wobble();
                    ShowHint(tiles, feedback.Hint);
                    break;
                case FeedbackOutcome.Right:
                case FeedbackOutcome.Done:
                    tapped.SetGlowing(false);
                    tapped.SetFaded(false);
                    tapped.Sparkle();
                    break;
                default:
                    tapped.Bounce();
                    break;
            }
        }

        private static void ShowHint(IReadOnlyList<ChoiceTile> tiles, ActivityHint hint)
        {
            if (hint.Choice < 0 || hint.Choice >= tiles.Count)
            {
                return;
            }

            if (hint.Cue == HintCue.FadeChoice)
            {
                tiles[hint.Choice].SetFaded(true);
            }
            else if (hint.Cue == HintCue.GlowAnswer)
            {
                tiles[hint.Choice].SetFaded(false);
                tiles[hint.Choice].SetGlowing(true);
            }
        }
    }
}
