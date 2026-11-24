using System.Collections.Generic;
using Scolita.Core.Activities;
using Scolita.Core.Lessons;
using Scolita.Game.Lessons;
using UnityEngine;

namespace Scolita.UI.Lesson
{
    /// <summary>
    /// The view of <c>ReadAndMatch</c>: the word on a cream card (the round's subject, in the reading typeface,
    /// with its language badge; a tap asks for the prompt, which says the word only after her first try) and 2 to
    /// 3 picture cards. Its feedback is the picture cards' own (<see cref="ChoiceTileFeedback"/>); the first hint
    /// (<see cref="HintCue.SpellOut"/>) also lights the word's letters one by one, in pink, while their sounds are
    /// said. The lit texts are made when a step begins, so lighting them allocates nothing. The prefab is
    /// <c>Prefabs/Lesson/ReadAndMatchView.prefab</c>.
    /// </summary>
    public sealed class ReadAndMatchView : ActivityView
    {
        [SerializeField] private ChoiceTile wordCard;
        [SerializeField] private ChoiceTile[] pictures = new ChoiceTile[0];
        [SerializeField] private string litColour = "#D9408A";
        [SerializeField] private float secondsBeforeTheLetters = 1.6f;
        [SerializeField] private float secondsPerLetter = 0.8f;

        private readonly List<string> litTexts = new List<string>();
        private string plainText = string.Empty;
        private int lit = -1;
        private float nextLetterIn;

        /// <summary>The word card.</summary>
        public ChoiceTile WordCard => wordCard;

        /// <summary>The picture cards, the ones in use active.</summary>
        public IReadOnlyList<ChoiceTile> Pictures => pictures;

        /// <summary>Whether the word's letters are being lit one by one (the first hint).</summary>
        public bool IsSpellingOut => lit >= 0;

        /// <inheritdoc/>
        public override void Show(ActivityRound round, ActivityViewContext context)
        {
            ActivityChoice word = round.Subject;
            plainText = word.Text;
            MakeLitTexts(plainText);
            lit = -1;
            wordCard.ShowSubject(word, plainText, null, context.Input);
            for (int index = 0; index < pictures.Length; index++)
            {
                if (index < round.Choices.Count)
                {
                    ActivityChoice choice = round.Choices[index];
                    pictures[index].Show(index, choice, string.Empty, context.Pictures.Of(choice.Picture), context.Input);
                }
                else
                {
                    pictures[index].gameObject.SetActive(false);
                }
            }

            if (round.Choices.Count > pictures.Length)
            {
                Debug.LogError("Lesson: a ReadAndMatch step has " + round.Choices.Count + " pictures; the view shows "
                    + pictures.Length + ".");
            }

            gameObject.SetActive(true);
        }

        /// <inheritdoc/>
        public override void ShowFeedback(in StepFeedback feedback)
        {
            ChoiceTileFeedback.Show(pictures, feedback);
            if (feedback.Outcome == FeedbackOutcome.TryAgain && feedback.Hint.Cue == HintCue.SpellOut && litTexts.Count > 0)
            {
                lit = 0;
                nextLetterIn = secondsBeforeTheLetters;
                wordCard.Restyle(plainText, false);
            }
        }

        /// <inheritdoc/>
        public override void Hide()
        {
            gameObject.SetActive(false);
        }

        private void MakeLitTexts(string text)
        {
            litTexts.Clear();
            for (int index = 0; index < text.Length; index++)
            {
                if (char.IsLetter(text[index]))
                {
                    litTexts.Add(text.Substring(0, index) + "<color=" + litColour + ">" + text[index] + "</color>"
                        + text.Substring(index + 1));
                }
            }
        }

        private void Update()
        {
            if (lit < 0)
            {
                return;
            }

            nextLetterIn -= Time.unscaledDeltaTime;
            if (nextLetterIn > 0f)
            {
                return;
            }

            if (lit < litTexts.Count)
            {
                wordCard.Restyle(litTexts[lit], false);
                lit++;
                nextLetterIn = secondsPerLetter;
                return;
            }

            wordCard.Restyle(plainText, false);
            lit = -1;
        }
    }
}
