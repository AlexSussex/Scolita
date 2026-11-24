using System.Collections.Generic;
using Scolita.Core.Activities;
using Scolita.Core.Lessons;
using Scolita.Game.Lessons;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Lesson
{
    /// <summary>
    /// The view of <c>LetterHunt</c>: the round's stars drift slowly left and right in lanes across the
    /// star field (the activity area below the top bar, so never over the home button, the step dots,
    /// the mascot or its bubble, and above the "Ascultă" button), wrapping round at its edges, never
    /// faster than <see cref="LetterHuntTuning.FastestSpeed"/> in the canvas's reference pixels. A star
    /// found pops, a miss bounces back, the hint twinkles the stars still to find, and the answer shown
    /// makes them glow (all of them, so one is always in sight: each is the answer) until one is found. Its stars are made with the prefab
    /// (<c>Prefabs/Lesson/LetterHuntView.prefab</c>, <see cref="LetterHuntTuning.MostStars"/> of them) and
    /// moved on plain arithmetic in <c>Update</c>, allocating nothing.
    /// </summary>
    public sealed class LetterHuntView : ActivityView
    {
        [SerializeField] private RectTransform field;
        [SerializeField] private HuntStar[] stars = new HuntStar[0];
        [SerializeField] private Button listenAgain;
        [SerializeField] private float starSize = 150f;
        [SerializeField] private float laneGap = 24f;
        [SerializeField] private float starGap = 40f;
        [SerializeField, Range(0f, 1f)] private float bobShare = 0.25f;
        [SerializeField] private float bobHeight = 10f;
        [SerializeField, Range(0f, 1f)] private float slowLaneShare = 0.8f;

        private readonly List<int> laneOf = new List<int>(LetterHuntTuning.MostStars);
        private readonly List<int> slotOf = new List<int>(LetterHuntTuning.MostStars);
        private readonly float[] laneShift = new float[LetterHuntTuning.MostStars];
        private IActivityInput input;
        private ActivityRound round;
        private float fastest;
        private int lanes;
        private int perLane;
        private float loopWidth;
        private float time;

        /// <summary>The stars, the ones in use active.</summary>
        public IReadOnlyList<HuntStar> Stars => stars;

        /// <summary>The star field the stars move in.</summary>
        public RectTransform Field => field;

        /// <summary>The fastest a star may move, in the canvas's reference pixels per second.</summary>
        public float FastestSpeed => fastest;

        /// <inheritdoc/>
        public override void Show(ActivityRound shownRound, ActivityViewContext context)
        {
            round = shownRound;
            input = context.Input;
            fastest = context.Rules.LetterHunt.FastestSpeed;
            if (round.Choices.Count > stars.Length)
            {
                Debug.LogError("Lesson: a LetterHunt step has " + round.Choices.Count + " stars; the view holds " + stars.Length + ".");
            }

            for (int index = 0; index < stars.Length; index++)
            {
                if (index < round.Choices.Count)
                {
                    stars[index].Show(index, round.Choices[index], input);
                }
                else
                {
                    stars[index].gameObject.SetActive(false);
                }
            }

            gameObject.SetActive(true);
            PlaceInLanes();
        }

        /// <inheritdoc/>
        public override void ShowFeedback(in StepFeedback feedback)
        {
            HuntStar tapped = feedback.Choice >= 0 && feedback.Choice < stars.Length ? stars[feedback.Choice] : null;
            switch (feedback.Outcome)
            {
                case FeedbackOutcome.Right:
                case FeedbackOutcome.Done:
                    MarkStarsLeft(false, false);
                    tapped?.Pop();
                    break;
                case FeedbackOutcome.TryAgain:
                case FeedbackOutcome.ShowAnswer:
                    tapped?.Bounce();
                    ShowHint(feedback.Hint);
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
            if (hint.Cue == HintCue.TwinkleAnswers)
            {
                MarkStarsLeft(true, false);
            }
            else if (hint.Cue == HintCue.GlowAnswer)
            {
                MarkStarsLeft(false, true);
            }
        }

        private void MarkStarsLeft(bool twinkle, bool glow)
        {
            for (int index = 0; index < round.Choices.Count && index < stars.Length; index++)
            {
                if (round.Choices[index].Item != round.AnswerItem || round.StateOf(index) == ChoiceState.Done)
                {
                    continue;
                }

                if (twinkle)
                {
                    stars[index].Twinkle();
                }

                stars[index].SetGlowing(glow);
            }
        }

        private void PlaceInLanes()
        {
            Rect area = field.rect;
            int count = Mathf.Min(round.Choices.Count, stars.Length);
            float laneHeight = starSize + laneGap;
            lanes = Mathf.Clamp(Mathf.FloorToInt(area.height / laneHeight), 1, 3);
            perLane = Mathf.CeilToInt(count / (float)lanes);
            loopWidth = Mathf.Max(area.width + starSize, perLane * (starSize + starGap));
            laneOf.Clear();
            slotOf.Clear();
            for (int index = 0; index < count; index++)
            {
                laneOf.Add(index % lanes);
                slotOf.Add(index / lanes);
            }

            for (int lane = 0; lane < laneShift.Length; lane++)
            {
                laneShift[lane] = lane * loopWidth / (perLane * 2f);
            }

            time = 0f;
            MoveStars(0f);
        }

        private void Update()
        {
            if (round == null)
            {
                return;
            }

            float delta = Time.unscaledDeltaTime;
            time += delta;
            MoveStars(delta);
        }

        private void MoveStars(float delta)
        {
            Rect area = field.rect;
            float spacing = loopWidth / perLane;
            float bobSpeed = fastest * bobShare / bobHeight;
            for (int lane = 0; lane < lanes; lane++)
            {
                float speed = fastest * (lane % 2 == 0 ? 1f - bobShare : (1f - bobShare) * slowLaneShare);
                float direction = lane % 2 == 0 ? 1f : -1f;
                laneShift[lane] = Mathf.Repeat(laneShift[lane] + direction * speed * delta, loopWidth);
            }

            for (int index = 0; index < laneOf.Count; index++)
            {
                HuntStar star = stars[index];
                if (!star.gameObject.activeSelf)
                {
                    continue;
                }

                int lane = laneOf[index];
                float x = area.xMin - starSize * 0.5f + Mathf.Repeat(slotOf[index] * spacing + laneShift[lane], loopWidth);
                float laneY = area.yMax - (area.height / lanes) * (lane + 0.5f);
                float y = laneY + bobHeight * Mathf.Sin(time * bobSpeed + index);
                star.Rect.anchoredPosition = new Vector2(x - area.center.x, y - area.center.y);
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
