using System.Collections.Generic;
using Scolita.Core.Activities;
using Scolita.Core.Lessons;
using Scolita.Game.Lessons;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Lesson
{
    /// <summary>
    /// The view of <c>FirstSound</c>: the picture card (the round's subject, with its language badge and
    /// no word written, so its first letter isn't shown; a tap says the word again) and 2 to 4 letter
    /// cards with their badges, and a "listen again" button. Its feedback is the letter cards' own
    /// (<see cref="ChoiceTileFeedback"/>). The prefab is <c>Prefabs/Lesson/FirstSoundView.prefab</c>.
    /// </summary>
    public sealed class FirstSoundView : ActivityView
    {
        [SerializeField] private ChoiceTile pictureCard;
        [SerializeField] private ChoiceTile[] tiles = new ChoiceTile[0];
        [SerializeField] private Button listenAgain;

        private IActivityInput input;

        /// <summary>The picture card.</summary>
        public ChoiceTile PictureCard => pictureCard;

        /// <summary>The letter cards, the ones in use active.</summary>
        public IReadOnlyList<ChoiceTile> Tiles => tiles;

        /// <inheritdoc/>
        public override void Show(ActivityRound round, ActivityViewContext context)
        {
            input = context.Input;
            ActivityChoice subject = round.Subject;
            pictureCard.ShowSubject(subject, string.Empty, context.Pictures.Of(subject.Picture), input);
            for (int index = 0; index < tiles.Length; index++)
            {
                if (index < round.Choices.Count)
                {
                    tiles[index].Show(index, round.Choices[index], round.Choices[index].Text, null, input);
                }
                else
                {
                    tiles[index].gameObject.SetActive(false);
                }
            }

            if (round.Choices.Count > tiles.Length)
            {
                Debug.LogError("Lesson: a FirstSound step has " + round.Choices.Count + " letters; the view shows " + tiles.Length + ".");
            }

            gameObject.SetActive(true);
        }

        /// <inheritdoc/>
        public override void ShowFeedback(in StepFeedback feedback)
        {
            ChoiceTileFeedback.Show(tiles, feedback);
        }

        /// <inheritdoc/>
        public override void Hide()
        {
            gameObject.SetActive(false);
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
