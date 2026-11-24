using System.Collections.Generic;
using Scolita.Core.Activities;
using Scolita.Core.Lessons;
using Scolita.Game.Lessons;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Lesson
{
    /// <summary>
    /// The view of <c>ListenAndPick</c>: 2 to 4 round cream cards in a row, each a letter (or a
    /// picture) with its language badge, and a "listen again" button. A miss wobbles the card and fades
    /// it; the answer shown glows yellow; the right one sparkles. Its four cards are made with the
    /// prefab (<c>Prefabs/Lesson/ListenAndPickView.prefab</c>) and reused step after step.
    /// </summary>
    public sealed class ListenAndPickView : ActivityView
    {
        [SerializeField] private ChoiceTile[] tiles = new ChoiceTile[0];
        [SerializeField] private Button listenAgain;

        private IActivityInput input;

        /// <summary>The cards, the ones in use active.</summary>
        public IReadOnlyList<ChoiceTile> Tiles => tiles;

        /// <inheritdoc/>
        public override void Show(ActivityRound round, ActivityViewContext context)
        {
            input = context.Input;
            for (int index = 0; index < tiles.Length; index++)
            {
                if (index >= round.Choices.Count)
                {
                    tiles[index].gameObject.SetActive(false);
                    continue;
                }

                ActivityChoice choice = round.Choices[index];
                Sprite sprite = choice.Look == ActivityChoiceLook.Picture ? context.Pictures.Of(choice.Picture) : null;
                string text = choice.Look == ActivityChoiceLook.Picture ? string.Empty : choice.Text;
                tiles[index].Show(index, choice, text, sprite, input);
            }

            if (round.Choices.Count > tiles.Length)
            {
                Debug.LogError("Lesson: a ListenAndPick step has " + round.Choices.Count + " choices; the view shows "
                    + tiles.Length + ".");
            }

            gameObject.SetActive(true);
        }

        /// <inheritdoc/>
        public override void ShowFeedback(in StepFeedback feedback)
        {
            ChoiceTile tapped = tiles[feedback.Choice];
            switch (feedback.Outcome)
            {
                case FeedbackOutcome.TryAgain:
                case FeedbackOutcome.ShowAnswer:
                    tapped.Wobble();
                    ShowHint(feedback.Hint);
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

        /// <inheritdoc/>
        public override void Hide()
        {
            gameObject.SetActive(false);
        }

        private void ShowHint(ActivityHint hint)
        {
            if (hint.Choice < 0 || hint.Choice >= tiles.Length)
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

        private void Awake()
        {
            listenAgain.onClick.AddListener(OnListenAgain);
        }

        private void OnDestroy()
        {
            listenAgain.onClick.RemoveListener(OnListenAgain);
        }

        private void OnListenAgain()
        {
            input?.RepeatPrompt();
        }
    }
}
