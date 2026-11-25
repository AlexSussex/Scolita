using Scolita.Core.Activities;
using Scolita.Core.Lessons;
using Scolita.Core.Tracing;
using Scolita.Game.Lessons;
using Scolita.UI.Tracing;
using UnityEngine;

namespace Scolita.UI.Lesson
{
    /// <summary>
    /// The view of <c>MeetLetter</c> (design screen 2): the letter card, small and capital in the
    /// reading typeface, and the picture card with its word, its first letter in pink; both carry the
    /// language badge, and both are tapped to be heard. Beside the letter, the "de mână" tile shows the letter by hand
    /// (<see cref="HandwritingView"/>, drawn from its stroke data); a tap writes it again before her eyes. The prefab is
    /// <c>Prefabs/Lesson/MeetLetterView.prefab</c>.
    /// </summary>
    public sealed class MeetLetterView : ActivityView
    {
        [SerializeField] private ChoiceTile letterCard;
        [SerializeField] private ChoiceTile pictureCard;
        [SerializeField] private ChoiceTile byHandTile;
        [SerializeField] private HandwritingView handwriting;
        [SerializeField] private Color firstLetterColour = new Color(0.851f, 0.251f, 0.541f);

        private TraceFigure handwritten;

        /// <inheritdoc/>
        public override void Show(ActivityRound round, ActivityViewContext context)
        {
            ActivityChoice letter = round.Choices[MeetLetterRules.LetterChoice];
            ActivityChoice word = round.Choices[MeetLetterRules.PictureChoice];
            letterCard.Show(MeetLetterRules.LetterChoice, letter, letter.Text, null, context.Input);
            pictureCard.Show(MeetLetterRules.PictureChoice, word, WithFirstLetterColoured(word.Text),
                context.Pictures.Of(word.Picture), context.Input);
            handwritten = round.Handwriting;
            byHandTile.gameObject.SetActive(handwritten != null);
            if (handwritten != null)
            {
                byHandTile.Show(MeetLetterRules.ByHandChoice, round.Choices[MeetLetterRules.ByHandChoice], string.Empty, null,
                    context.Input);
                handwriting.Show(handwritten);
            }

            gameObject.SetActive(true);
        }

        /// <inheritdoc/>
        public override void ShowFeedback(in StepFeedback feedback)
        {
            if (feedback.Choice == MeetLetterRules.ByHandChoice && handwritten != null)
            {
                byHandTile.Bounce();
                handwriting.Write(handwritten);
                return;
            }

            ChoiceTile tapped = feedback.Choice == MeetLetterRules.LetterChoice ? letterCard : pictureCard;
            if (feedback.Outcome == FeedbackOutcome.Neutral)
            {
                tapped.Bounce();
            }
            else
            {
                tapped.Sparkle();
            }
        }

        /// <inheritdoc/>
        public override void Hide()
        {
            handwriting.Clear();
            handwritten = null;
            gameObject.SetActive(false);
        }

        private string WithFirstLetterColoured(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            return "<color=#" + ColorUtility.ToHtmlStringRGB(firstLetterColour) + ">" + text.Substring(0, 1) + "</color>"
                + text.Substring(1);
        }
    }
}
