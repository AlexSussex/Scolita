using Scolita.Core.Activities;
using Scolita.Core.Lessons;
using Scolita.Game.Lessons;
using UnityEngine;

namespace Scolita.UI.Lesson
{
    /// <summary>
    /// The view of <c>MeetLetter</c> (design screen 2): the letter card, small and capital in the
    /// reading typeface, and the picture card with its word, its first letter in pink; both carry the
    /// language badge, and both are tapped to be heard. The prefab is
    /// <c>Prefabs/Lesson/MeetLetterView.prefab</c>.
    /// </summary>
    public sealed class MeetLetterView : ActivityView
    {
        [SerializeField] private ChoiceTile letterCard;
        [SerializeField] private ChoiceTile pictureCard;
        [SerializeField] private Color firstLetterColour = new Color(0.851f, 0.251f, 0.541f);

        /// <inheritdoc/>
        public override void Show(ActivityRound round, ActivityViewContext context)
        {
            ActivityChoice letter = round.Choices[MeetLetterRules.LetterChoice];
            ActivityChoice word = round.Choices[MeetLetterRules.PictureChoice];
            letterCard.Show(MeetLetterRules.LetterChoice, letter, letter.Text, null, context.Input);
            pictureCard.Show(MeetLetterRules.PictureChoice, word, WithFirstLetterColoured(word.Text),
                context.Pictures.Of(word.Picture), context.Input);
            gameObject.SetActive(true);
        }

        /// <inheritdoc/>
        public override void ShowFeedback(in StepFeedback feedback)
        {
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
