using System.Collections.Generic;
using Scolita.Core.Activities;
using Scolita.Core.Lessons;
using Scolita.Game.Lessons;
using UnityEngine;

namespace Scolita.UI.Lesson
{
    /// <summary>
    /// The view of <c>BuildWord</c> (design screen 5): the word's picture card (the round's subject, no word
    /// written; a tap says the word again), the rocket lying on its side with one window per letter
    /// (<see cref="RocketWindow"/>, the next one's rim glowing), and the letter tiles in a row under it
    /// (<see cref="DraggedTile"/>), to drag to the rocket. A tile let go on the rocket is placed; elsewhere it
    /// floats back. It draws the round's state after every answer (the tiles still to place, the windows' letters
    /// from <see cref="ActivityRound.PlacedOn"/>), so it keeps none of its own; when the word is built the rocket
    /// takes off. The prefab is <c>Prefabs/Lesson/BuildWordView.prefab</c>.
    /// </summary>
    public sealed class BuildWordView : ActivityView
    {
        [SerializeField] private ChoiceTile pictureCard;
        [SerializeField] private RectTransform rocket;
        [SerializeField] private RectTransform windowsArea;
        [SerializeField] private RocketWindow[] windows = new RocketWindow[0];
        [SerializeField] private DraggedTile[] tiles = new DraggedTile[0];
        [SerializeField] private float largestWindow = 120f;
        [SerializeField] private float windowGap = 24f;
        [SerializeField] private float tileSpacing = 160f;
        [SerializeField] private float tileRowY = -255f;
        [SerializeField] private float takeOffSeconds = 1.6f;
        [SerializeField] private float takeOffHeight = 1000f;
        [SerializeField] private float takeOffDrift = 0.35f;
        [SerializeField] private float takeOffTilt = 25f;
        [SerializeField] private float tileSize = 130f;

        private IActivityInput input;
        private ActivityRound round;
        private Vector2 rocketHome;
        private float takeOffLeft;

        /// <summary>The picture card.</summary>
        public ChoiceTile PictureCard => pictureCard;

        /// <summary>The rocket.</summary>
        public RectTransform Rocket => rocket;

        /// <summary>The windows, the ones in use active.</summary>
        public IReadOnlyList<RocketWindow> Windows => windows;

        /// <summary>The tiles, the ones in use active.</summary>
        public IReadOnlyList<DraggedTile> Tiles => tiles;

        /// <summary>Whether the rocket is taking off (the word is built).</summary>
        public bool IsTakingOff => takeOffLeft > 0f;

        /// <inheritdoc/>
        public override void Show(ActivityRound shownRound, ActivityViewContext context)
        {
            round = shownRound;
            input = context.Input;
            takeOffLeft = 0f;
            rocket.anchoredPosition = rocketHome;
            rocket.localRotation = Quaternion.identity;
            ActivityChoice word = round.Subject;
            pictureCard.ShowSubject(word, string.Empty, context.Pictures.Of(word.Picture), input);
            PlaceWindows(round.Places);
            PlaceTiles(round.Choices);
            if (round.Places > windows.Length || round.Choices.Count > tiles.Length)
            {
                Debug.LogError("Lesson: a BuildWord step has " + round.Places + " letters and " + round.Choices.Count
                    + " tiles; the view shows " + windows.Length + " and " + tiles.Length + ".");
            }

            gameObject.SetActive(true);
            Refresh();
        }

        /// <inheritdoc/>
        public override void ShowFeedback(in StepFeedback feedback)
        {
            int tapped = feedback.Choice;
            bool known = tapped >= 0 && tapped < tiles.Length;
            switch (feedback.Outcome)
            {
                case FeedbackOutcome.TryAgain:
                case FeedbackOutcome.ShowAnswer:
                    if (known)
                    {
                        tiles[tapped].Tile.Wobble();
                    }

                    if (feedback.Hint.Cue == HintCue.GlowAnswer && feedback.Hint.Choice >= 0 && feedback.Hint.Choice < tiles.Length)
                    {
                        tiles[feedback.Hint.Choice].Tile.SetGlowing(true);
                    }

                    break;
                case FeedbackOutcome.Right:
                case FeedbackOutcome.Done:
                    for (int index = 0; index < tiles.Length; index++)
                    {
                        tiles[index].Tile.SetGlowing(false);
                    }

                    if (feedback.Outcome == FeedbackOutcome.Done)
                    {
                        takeOffLeft = takeOffSeconds;
                    }

                    break;
                default:
                    if (known)
                    {
                        tiles[tapped].Tile.Bounce();
                    }

                    break;
            }

            Refresh();
        }

        /// <inheritdoc/>
        public override void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// The tile <paramref name="tile"/> was let go at <paramref name="screenPoint"/>, seen by
        /// <paramref name="eventCamera"/>: placed when it is over the rocket. Called by <see cref="DraggedTile"/>.
        /// </summary>
        public void TileLetGo(int tile, Vector2 screenPoint, Camera eventCamera)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(rocket, screenPoint, eventCamera))
            {
                input?.Answer(ActivityAnswer.Place(tile));
            }
        }

        private void Awake()
        {
            rocketHome = rocket.anchoredPosition;
        }

        private void PlaceWindows(int count)
        {
            float width = windowsArea.rect.width;
            float size = count > 0 ? Mathf.Min(largestWindow, (width - (count - 1) * windowGap) / count) : largestWindow;
            float first = -((count - 1) * (size + windowGap)) / 2f;
            for (int index = 0; index < windows.Length; index++)
            {
                if (index < count)
                {
                    windows[index].Place(first + index * (size + windowGap), size);
                }
                else
                {
                    windows[index].gameObject.SetActive(false);
                }
            }
        }

        private void PlaceTiles(IReadOnlyList<ActivityChoice> choices)
        {
            float room = ((RectTransform)transform).rect.width - tileSize;
            float spacing = choices.Count > 1 ? Mathf.Min(tileSpacing, room / (choices.Count - 1)) : tileSpacing;
            float first = -((choices.Count - 1) * spacing) / 2f;
            for (int index = 0; index < tiles.Length; index++)
            {
                if (index < choices.Count)
                {
                    tiles[index].Tile.Show(index, choices[index], choices[index].Text, null, input);
                    tiles[index].Home(this, new Vector2(first + index * spacing, tileRowY));
                }
                else
                {
                    tiles[index].gameObject.SetActive(false);
                }
            }
        }

        private void Refresh()
        {
            for (int index = 0; index < tiles.Length && index < round.Choices.Count; index++)
            {
                tiles[index].gameObject.SetActive(round.StateOf(index) != ChoiceState.Done);
            }

            for (int window = 0; window < windows.Length && window < round.Places; window++)
            {
                windows[window].Fill(LetterIn(window), window == round.PlacesFilled);
            }
        }

        private string LetterIn(int window)
        {
            for (int index = 0; index < round.Choices.Count; index++)
            {
                if (round.PlacedOn(index) == window)
                {
                    return round.Choices[index].Text;
                }
            }

            return string.Empty;
        }

        private void Update()
        {
            if (takeOffLeft <= 0f)
            {
                return;
            }

            takeOffLeft = Mathf.Max(0f, takeOffLeft - Time.unscaledDeltaTime);
            float done = 1f - takeOffLeft / takeOffSeconds;
            rocket.anchoredPosition = rocketHome + new Vector2(done * takeOffHeight * takeOffDrift, done * done * takeOffHeight);
            rocket.localRotation = Quaternion.Euler(0f, 0f, takeOffTilt * done);
        }
    }
}
