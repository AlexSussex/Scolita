using System.Collections.Generic;
using Scolita.Core.Activities;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;
using Scolita.Game.Lessons;
using UnityEngine;

namespace Scolita.UI.Lesson
{
    /// <summary>
    /// The view of <c>WordTwins</c>: the Romanian words in a column on the left (star badges), the English ones on
    /// the right (ringed-planet badges), each a <see cref="TwinCard"/>. A tap says a word (and joins it to the word
    /// in play of the other column); a line dragged from one card to another joins them
    /// (<see cref="JoinLine"/> follows her finger). It draws the round's state after every answer (the word in
    /// play, a line and both pictures for each pair joined, from <see cref="ActivityRound.PlacedOn"/>), and the
    /// hints' pictures and glow. The prefab is <c>Prefabs/Lesson/WordTwinsView.prefab</c>.
    /// </summary>
    public sealed class WordTwinsView : ActivityView
    {
        [SerializeField] private RectTransform board;
        [SerializeField] private TwinCard[] cards = new TwinCard[0];
        [SerializeField] private JoinLine dragged;
        [SerializeField] private JoinLine[] joined = new JoinLine[0];
        [SerializeField] private float columnX = 300f;
        [SerializeField] private float columnCentreY = -110f;
        [SerializeField] private float rowStep = 136f;
        [SerializeField] private Color draggedColour = new Color(1f, 0.851f, 0.239f, 1f);
        [SerializeField] private Color joinedColour = new Color(0.298f, 0.851f, 0.482f, 1f);

        private readonly bool[] revealed = new bool[2 * ActivityLimits.MostPairs];
        private IActivityInput input;
        private ActivityRound round;

        /// <summary>The cards, the ones in use active.</summary>
        public IReadOnlyList<TwinCard> Cards => cards;

        /// <summary>The lines of the pairs joined, the ones in use active.</summary>
        public IReadOnlyList<JoinLine> Joined => joined;

        /// <summary>The line following her finger; inactive when she isn't drawing one.</summary>
        public JoinLine Dragged => dragged;

        /// <inheritdoc/>
        public override void Show(ActivityRound shownRound, ActivityViewContext context)
        {
            round = shownRound;
            input = context.Input;
            int firstEnglish = round.Choices.Count / 2;
            for (int index = 0; index < cards.Length; index++)
            {
                if (index >= round.Choices.Count)
                {
                    cards[index].gameObject.SetActive(false);
                    continue;
                }

                ActivityChoice word = round.Choices[index];
                bool romanian = word.Language == Language.Romanian;
                int row = romanian ? index : index - firstEnglish;
                Vector2 position = new Vector2(romanian ? -columnX : columnX, columnCentreY + ((firstEnglish - 1) / 2f - row) * rowStep);
                cards[index].Show(this, input, index, word, context.Pictures.Of(word.Picture), position);
            }

            if (round.Choices.Count > cards.Length || round.Choices.Count > revealed.Length)
            {
                Debug.LogError("Lesson: a WordTwins step has " + round.Choices.Count + " words; the view shows " + cards.Length + ".");
            }

            for (int index = 0; index < revealed.Length; index++)
            {
                revealed[index] = false;
            }

            dragged.gameObject.SetActive(false);
            gameObject.SetActive(true);
            Refresh();
        }

        /// <inheritdoc/>
        public override void ShowFeedback(in StepFeedback feedback)
        {
            int word = feedback.Choice;
            switch (feedback.Outcome)
            {
                case FeedbackOutcome.TryAgain:
                case FeedbackOutcome.ShowAnswer:
                    WobbleCard(word);
                    ShowHint(feedback.Hint);
                    break;
                case FeedbackOutcome.Right:
                case FeedbackOutcome.Done:
                    SparkleCard(word);
                    SparkleCard(IsCard(word) ? round.PlacedOn(word) : -1);
                    break;
                default:
                    if (IsCard(word))
                    {
                        cards[word].Tile.Bounce();
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
        /// A line starts from the card <paramref name="word"/> at <paramref name="screenPoint"/>, seen by
        /// <paramref name="eventCamera"/>; called by <see cref="TwinCard"/>.
        /// </summary>
        public void LineStarted(int word, Vector2 screenPoint, Camera eventCamera)
        {
            DrawLine(word, screenPoint, eventCamera);
        }

        /// <summary>
        /// The line from the card <paramref name="word"/> follows her finger to <paramref name="screenPoint"/>; called
        /// by <see cref="TwinCard"/>.
        /// </summary>
        public void LineMoved(int word, Vector2 screenPoint, Camera eventCamera)
        {
            DrawLine(word, screenPoint, eventCamera);
        }

        /// <summary>
        /// The line from the card <paramref name="word"/> was let go at <paramref name="screenPoint"/>: a join with
        /// the card under it, if any. Called by <see cref="TwinCard"/>.
        /// </summary>
        public void LineLetGo(int word, Vector2 screenPoint, Camera eventCamera)
        {
            dragged.gameObject.SetActive(false);
            for (int other = 0; other < cards.Length && other < round.Choices.Count; other++)
            {
                if (other != word && RectTransformUtility.RectangleContainsScreenPoint(cards[other].Rect, screenPoint, eventCamera))
                {
                    input?.Answer(ActivityAnswer.Join(word, other));
                    return;
                }
            }
        }

        private bool IsCard(int word)
        {
            return word >= 0 && word < cards.Length && word < round.Choices.Count;
        }

        private void WobbleCard(int word)
        {
            if (IsCard(word))
            {
                cards[word].Tile.Wobble();
            }
        }

        private void SparkleCard(int word)
        {
            if (IsCard(word))
            {
                cards[word].Tile.SetGlowing(false);
                cards[word].Tile.Sparkle();
            }
        }

        private void ShowHint(ActivityHint hint)
        {
            if (hint.Cue == HintCue.RevealPictures)
            {
                Reveal(hint.Choice);
                Reveal(round.Answer);
            }
            else if (hint.Cue == HintCue.GlowAnswer && IsCard(hint.Choice))
            {
                cards[hint.Choice].Tile.SetGlowing(true);
            }
        }

        private void Reveal(int word)
        {
            if (IsCard(word))
            {
                revealed[word] = true;
            }
        }

        private void DrawLine(int word, Vector2 screenPoint, Camera eventCamera)
        {
            if (IsCard(word) && round.StateOf(word) != ChoiceState.Done
                && RectTransformUtility.ScreenPointToLocalPointInRectangle(board, screenPoint, eventCamera, out Vector2 finger))
            {
                dragged.Connect(EdgeOf(word, finger.x), finger, draggedColour);
            }
        }

        private Vector2 EdgeOf(int word, float towardsX)
        {
            RectTransform card = cards[word].Rect;
            float half = card.rect.width / 2f;
            Vector2 centre = card.anchoredPosition;
            return new Vector2(centre.x + (towardsX >= centre.x ? half : -half), centre.y);
        }

        private void Refresh()
        {
            int line = 0;
            for (int word = 0; word < cards.Length && word < round.Choices.Count; word++)
            {
                int partner = round.PlacedOn(word);
                bool isJoined = round.StateOf(word) == ChoiceState.Done;
                cards[word].Refresh(round.Current == word, revealed[word] || isJoined);
                if (isJoined && partner > word && line < joined.Length)
                {
                    Vector2 here = cards[word].Rect.anchoredPosition;
                    Vector2 there = cards[partner].Rect.anchoredPosition;
                    joined[line].Connect(EdgeOf(word, there.x), EdgeOf(partner, here.x), joinedColour);
                    line++;
                }
            }

            for (; line < joined.Length; line++)
            {
                joined[line].gameObject.SetActive(false);
            }
        }
    }
}
