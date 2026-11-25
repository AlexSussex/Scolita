using Scolita.Core.Activities;
using Scolita.Core.Lessons;
using Scolita.Core.Tracing;
using Scolita.Game.Lessons;
using Scolita.Game.Tracing;
using Scolita.UI.Lesson;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Tracing
{
    /// <summary>
    /// The view of <c>TraceLetter</c> (design screen 3, <c>Prefabs/Lesson/TraceLetterView.prefab</c>): the writing
    /// board with its lines, the letter's guide for the round (dotted, a faint shadow, or the start dot only), the
    /// start dots with their numbers, the yellow arrow, her rainbow trail with stardust under the finger drawing
    /// (<see cref="TraceFinger"/>: one finger, palms ignored), the strokes she finished kept in ink, the stroke
    /// drawing itself when the answer is shown, the letter's bounce when a round is done, and the round's stars.
    /// It draws the round's <see cref="ActivityRound.Trace"/> and sends each stroke to Core when the finger
    /// lifts; it never judges. Drawing allocates nothing: the points go into one reused buffer and the lines'
    /// meshes into buffers made once.
    /// </summary>
    public sealed class TraceLetterView : ActivityView, ITraceInput
    {
        private const float LinesSpan = WritingLines.Below - WritingLines.Top;
        private const float LinesMiddle = (WritingLines.Top + WritingLines.Below) * 0.5f;
        private const float ArrowStep = 4f;

        private static readonly string[] Numbers = { "1", "2", "3", "4" };

        [SerializeField] private RectTransform board;
        [SerializeField] private RectTransform letter;
        [SerializeField] private RectTransform topLine;
        [SerializeField] private RectTransform middleLine;
        [SerializeField] private RectTransform baseline;
        [SerializeField] private RectTransform belowLine;
        [SerializeField] private StrokeLineGraphic guideDots;
        [SerializeField] private StrokeLineGraphic guideShadow;
        [SerializeField] private StrokeLineGraphic ink;
        [SerializeField] private StrokeLineGraphic trail;
        [SerializeField] private StrokeLineGraphic drawing;
        [SerializeField] private StrokeLineGraphic arrow;
        [SerializeField] private RectTransform arrowHead;
        [SerializeField] private StartDot[] dots = new StartDot[0];
        [SerializeField] private Stardust stardust;
        [SerializeField] private Graphic fingerGlow;
        [SerializeField] private LanguageBadge badge;
        [SerializeField] private TraceFinger finger;
        [SerializeField] private StarGraphic[] roundStars = new StarGraphic[0];
        [SerializeField] private StrokeLineGraphic dottedPicture;
        [SerializeField] private StrokeLineGraphic shadowPicture;
        [SerializeField] private Color starLit = new Color(1f, 0.851f, 0.239f);
        [SerializeField] private Color starUnlit = new Color(1f, 1f, 1f, 0.3f);
        [SerializeField, Range(0.3f, 1f)] private float letterHeightShare = 0.84f;
        [SerializeField, Range(0.3f, 1f)] private float letterWidthShare = 0.8f;
        [SerializeField, Min(10f)] private float arrowLength = 70f;
        [SerializeField, Min(0.1f)] private float fadeSeconds = 0.45f;
        [SerializeField, Min(0.1f)] private float bounceSeconds = 0.5f;
        [SerializeField, Min(0.1f)] private float roundPauseSeconds = 1.1f;
        [SerializeField, Min(0.1f)] private float drawSeconds = 1.6f;

        private readonly TracePoints points = new TracePoints();
        private IActivityInput input;
        private TraceState state;
        private float scale = 1f;
        private float centreX;
        private int roundsShown;
        private bool tracing;
        private float fadeLeft;
        private float bounceLeft;
        private float roundPauseLeft;
        private float drawLeft;

        /// <summary>The finger reader of the board; used by the PlayMode tests to draw a stroke.</summary>
        public TraceFinger Finger => finger;

        /// <inheritdoc/>
        public override void Show(ActivityRound round, ActivityViewContext context)
        {
            input = context.Input;
            state = round.Trace;
            badge.Show(round.Language);
            gameObject.SetActive(true);
            Layout();
            stardust.Clear();
            fingerGlow.canvasRenderer.SetAlpha(0f);
            DrawPictures();
            ShowRound();
            finger.Listen(this);
        }

        /// <inheritdoc/>
        public override void ShowFeedback(in StepFeedback feedback)
        {
            switch (feedback.Outcome)
            {
                case FeedbackOutcome.Right:
                case FeedbackOutcome.Done:
                    StrokeKept();
                    break;
                case FeedbackOutcome.TryAgain:
                    FadeTrail();
                    ShowDotsAndArrow(true);
                    if (state.Stroke < dots.Length)
                    {
                        dots[state.Stroke].Pulse();
                    }

                    break;
                case FeedbackOutcome.ShowAnswer:
                    FadeTrail();
                    ShowGuide();
                    ShowDotsAndArrow(true);
                    drawLeft = drawSeconds;
                    break;
            }
        }

        /// <inheritdoc/>
        public override void Hide()
        {
            finger.Listen(null);
            stardust.Clear();
            tracing = false;
            fadeLeft = 0f;
            bounceLeft = 0f;
            roundPauseLeft = 0f;
            drawLeft = 0f;
            letter.localScale = Vector3.one;
            gameObject.SetActive(false);
        }

        /// <inheritdoc/>
        public void StrokeBegan(Vector2 local)
        {
            if (state == null || state.RoundsDone >= TracingTuning.Rounds)
            {
                return;
            }

            if (roundPauseLeft > 0f)
            {
                roundPauseLeft = 0f;
                ShowRound();
            }

            tracing = true;
            fadeLeft = 0f;
            drawLeft = 0f;
            drawing.Clear();
            points.Clear();
            trail.canvasRenderer.SetAlpha(1f);
            trail.Clear();
            trail.BeginLine();
            fingerGlow.canvasRenderer.SetAlpha(1f);
            AddPoint(local);
        }

        /// <inheritdoc/>
        public void StrokeMoved(Vector2 local)
        {
            if (tracing)
            {
                AddPoint(local);
            }
        }

        /// <inheritdoc/>
        public void StrokeEnded()
        {
            if (!tracing)
            {
                return;
            }

            tracing = false;
            fingerGlow.canvasRenderer.SetAlpha(0f);
            stardust.Lift();
            input.Answer(ActivityAnswer.Traced(points));
        }

        /// <inheritdoc/>
        public void StrokeCancelled()
        {
            tracing = false;
            fingerGlow.canvasRenderer.SetAlpha(0f);
            stardust.Lift();
            points.Clear();
            trail.Clear();
        }

        /// <summary>
        /// The screen point of <paramref name="point"/> (in the letter's units) on the board; used by the
        /// PlayMode tests to draw along a stroke.
        /// </summary>
        public Vector2 ScreenPointOf(ShapePoint point)
        {
            return RectTransformUtility.WorldToScreenPoint(null, board.TransformPoint(Local(point)));
        }

        private void Update()
        {
            if (state == null)
            {
                return;
            }

            float step = Time.unscaledDeltaTime;
            if (fadeLeft > 0f)
            {
                fadeLeft -= step;
                trail.canvasRenderer.SetAlpha(Mathf.Max(0f, fadeLeft / fadeSeconds));
                if (fadeLeft <= 0f)
                {
                    trail.Clear();
                    trail.canvasRenderer.SetAlpha(1f);
                }
            }

            if (bounceLeft > 0f)
            {
                bounceLeft -= step;
                float share = 1f - Mathf.Max(0f, bounceLeft) / bounceSeconds;
                letter.localScale = Vector3.one * (1f + 0.12f * Mathf.Sin(share * Mathf.PI));
            }

            if (drawLeft > 0f)
            {
                drawLeft -= step;
                DrawStrokeItself(1f - Mathf.Max(0f, drawLeft) / drawSeconds);
            }

            if (roundPauseLeft > 0f)
            {
                roundPauseLeft -= step;
                if (roundPauseLeft <= 0f)
                {
                    ShowRound();
                }
            }
        }

        private void Layout()
        {
            Rect area = board.rect;
            float width = Mathf.Max(state.Shape.MaxX - state.Shape.MinX, WritingLines.Middle);
            scale = Mathf.Min(area.height * letterHeightShare / LinesSpan, area.width * letterWidthShare / width);
            centreX = (state.Shape.MinX + state.Shape.MaxX) * 0.5f;
            PlaceLine(topLine, WritingLines.Top, area.width);
            PlaceLine(middleLine, WritingLines.Middle, area.width);
            PlaceLine(baseline, WritingLines.Baseline, area.width);
            PlaceLine(belowLine, WritingLines.Below, area.width);
        }

        private void PlaceLine(RectTransform line, float y, float boardWidth)
        {
            line.anchoredPosition = new Vector2(0f, (LinesMiddle - y) * scale);
            line.sizeDelta = new Vector2(boardWidth * 0.9f, line.sizeDelta.y);
        }

        private void ShowRound()
        {
            roundsShown = state.RoundsDone;
            ink.Clear();
            trail.Clear();
            drawing.Clear();
            ShowGuide();
            ShowDotsAndArrow(false);
            for (int star = 0; star < roundStars.Length; star++)
            {
                roundStars[star].color = star <= state.Round ? starLit : starUnlit;
            }
        }

        private void ShowGuide()
        {
            guideDots.Clear();
            guideShadow.Clear();
            for (int stroke = 0; stroke < state.Shape.Strokes.Count; stroke++)
            {
                bool shownForAnswer = state.AnswerShown && stroke == state.Stroke;
                if (state.Guide == TraceGuide.Dotted || shownForAnswer)
                {
                    AddPath(guideDots, state.Shape.Strokes[stroke].Path, scale, new Vector2(-centreX * scale, 0f));
                }
                else if (state.Guide == TraceGuide.Faint)
                {
                    AddPath(guideShadow, state.Shape.Strokes[stroke].Path, scale, new Vector2(-centreX * scale, 0f));
                }
            }
        }

        private void ShowDotsAndArrow(bool arrowAlways)
        {
            for (int index = 0; index < dots.Length; index++)
            {
                bool current = index == state.Stroke;
                bool next = index == state.Stroke + 1 && state.Guide == TraceGuide.Dotted;
                if (index < state.Shape.Strokes.Count && (current || next))
                {
                    dots[index].Show(Local(state.Shape.Strokes[index].Path.Start), Numbers[index], current);
                }
                else
                {
                    dots[index].Hide();
                }
            }

            arrow.Clear();
            StrokePath path = state.Shape.Strokes[state.Stroke].Path;
            bool showArrow = (arrowAlways || state.Guide != TraceGuide.StartDotOnly) && !path.IsDot;
            arrowHead.gameObject.SetActive(showArrow);
            if (!showArrow)
            {
                return;
            }

            float from = Mathf.Min(14f, path.Length * 0.2f);
            float to = Mathf.Min(path.Length * 0.5f, from + arrowLength);
            arrow.BeginLine();
            for (float along = from; along < to; along += ArrowStep)
            {
                arrow.Add(Local(path.PointAt(along)));
            }

            Vector2 tip = Local(path.PointAt(to));
            Vector2 behind = Local(path.PointAt(to - ArrowStep));
            arrow.Add(tip);
            arrowHead.anchoredPosition = tip;
            arrowHead.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(tip.y - behind.y, tip.x - behind.x) * Mathf.Rad2Deg);
        }

        private void StrokeKept()
        {
            ink.BeginLine();
            for (int index = 0; index < points.Count; index++)
            {
                ink.Add(Local(new ShapePoint(points.X(index), points.Y(index))));
            }

            trail.Clear();
            drawing.Clear();
            drawLeft = 0f;
            if (state.RoundsDone > roundsShown)
            {
                bounceLeft = bounceSeconds;
                arrow.Clear();
                arrowHead.gameObject.SetActive(false);
                foreach (StartDot dot in dots)
                {
                    dot.Hide();
                }

                if (state.RoundsDone < TracingTuning.Rounds)
                {
                    roundPauseLeft = roundPauseSeconds;
                }
                else
                {
                    roundStars[roundStars.Length - 1].color = starLit;
                }

                return;
            }

            ShowGuide();
            ShowDotsAndArrow(false);
        }

        private void FadeTrail()
        {
            fadeLeft = fadeSeconds;
            fingerGlow.canvasRenderer.SetAlpha(0f);
        }

        private void DrawStrokeItself(float share)
        {
            StrokePath path = state.Shape.Strokes[state.Stroke].Path;
            float reached = path.Length * share;
            drawing.Clear();
            drawing.BeginLine();
            for (int index = 0; index < path.Count && path.DistanceAt(index) <= reached; index++)
            {
                drawing.Add(Local(new ShapePoint(path.X(index), path.Y(index))));
            }

            Vector2 tip = Local(path.PointAt(reached));
            drawing.Add(tip);
            fingerGlow.rectTransform.anchoredPosition = tip;
            fingerGlow.canvasRenderer.SetAlpha(share < 1f ? 1f : 0f);
        }

        private void AddPoint(Vector2 local)
        {
            if (points.Add(local.x / scale + centreX, LinesMiddle - local.y / scale))
            {
                trail.Add(local);
                stardust.Leave(local);
            }

            fingerGlow.rectTransform.anchoredPosition = local;
        }

        private void DrawPictures()
        {
            DrawPicture(dottedPicture);
            DrawPicture(shadowPicture);
        }

        private void DrawPicture(StrokeLineGraphic picture)
        {
            float top = WritingLines.Below;
            float bottom = WritingLines.Top;
            foreach (Stroke stroke in state.Shape.Strokes)
            {
                for (int index = 0; index < stroke.Path.Count; index++)
                {
                    top = Mathf.Min(top, stroke.Path.Y(index));
                    bottom = Mathf.Max(bottom, stroke.Path.Y(index));
                }
            }

            Rect area = picture.rectTransform.rect;
            float width = Mathf.Max(state.Shape.MaxX - state.Shape.MinX, 1f);
            float pictureScale = Mathf.Min(area.height / Mathf.Max(bottom - top, 1f), area.width / width);
            Vector2 offset = new Vector2(-centreX * pictureScale, ((top + bottom) * 0.5f - LinesMiddle) * pictureScale);
            picture.Clear();
            foreach (Stroke stroke in state.Shape.Strokes)
            {
                AddPath(picture, stroke.Path, pictureScale, offset);
            }
        }

        private static void AddPath(StrokeLineGraphic graphic, StrokePath path, float pathScale, Vector2 offset)
        {
            graphic.BeginLine();
            for (int index = 0; index < path.Count; index++)
            {
                graphic.Add(new Vector2(path.X(index) * pathScale, (LinesMiddle - path.Y(index)) * pathScale) + offset);
            }
        }

        private Vector2 Local(ShapePoint point)
        {
            return new Vector2((point.X - centreX) * scale, (LinesMiddle - point.Y) * scale);
        }
    }
}
