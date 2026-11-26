using Scolita.Core.Activities;
using Scolita.Core.Lessons;
using Scolita.Core.Tracing;
using Scolita.Game.Lessons;
using Scolita.Game.Tracing;
using Scolita.UI.Lesson;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Tracing
{
    /// <summary>
    /// The writing board, the view of <c>TraceLetter</c> and <c>TraceWord</c> (design screens 3 and 8,
    /// <c>Prefabs/Lesson/TraceLetterView.prefab</c> and <c>TraceWordView.prefab</c>): the board with its lines (for a
    /// handwritten figure the "tip I" lines and faint slant guides), the figure's guide for the round (dots on a pale
    /// road, a faint shadow, or the start dot only; the road and the shadow as wide as what is accepted,
    /// <see cref="TraceState.GuideHalfWidth"/>), the start dots numbered within their letter, the yellow arrow, the
    /// dotted hop through the air from where a stroke ends to the next start dot when she lifts her finger between
    /// them (never a line to trace), her rainbow trail
    /// with stardust under the finger drawing (<see cref="TraceFinger"/>: one finger, palms ignored), what she wrote
    /// kept in ink, the stroke (or, in a word, the rest of the letter) drawing itself when the answer is shown, the
    /// figure's bounce when a round is done, the round's stars, and for a word the word in print. It draws the round's
    /// <see cref="ActivityRound.Trace"/> and sends each line to Core when the finger lifts; it never judges. Drawing
    /// allocates nothing: the points go into one reused buffer and the lines' meshes into buffers made once.
    /// </summary>
    public sealed class WritingBoardView : ActivityView, ITraceInput
    {
        private const float LinesMiddle = FigureFit.LinesMiddle;
        private const float ArrowStep = 4f;
        private const float SlantGuideSpacing = 40f;
        private const float HopStep = 6f;
        private const float HopLift = 0.35f;
        private const float HopGapShare = 0.45f;

        private static readonly string[] Numbers = { "1", "2", "3", "4" };

        [SerializeField] private RectTransform board;
        [SerializeField] private RectTransform letter;
        [SerializeField] private RectTransform topLine;
        [SerializeField] private RectTransform middleLine;
        [SerializeField] private RectTransform baseline;
        [SerializeField] private RectTransform belowLine;
        [SerializeField] private GameObject printLines;
        [SerializeField] private GameObject handwrittenLines;
        [SerializeField] private RectTransform handTopLine;
        [SerializeField] private RectTransform handMiddleLine;
        [SerializeField] private RectTransform handBaseline;
        [SerializeField] private RectTransform handBelowLine;
        [SerializeField] private StrokeLineGraphic slantGuides;
        [SerializeField] private TMP_Text printedWord;
        [SerializeField] private StrokeLineGraphic guideDots;
        [SerializeField] private StrokeLineGraphic guideShadow;
        [SerializeField] private StrokeLineGraphic ink;
        [SerializeField] private StrokeLineGraphic trail;
        [SerializeField] private StrokeLineGraphic drawing;
        [SerializeField] private StrokeLineGraphic arrow;
        [SerializeField] private RectTransform arrowHead;
        [SerializeField] private StrokeLineGraphic hop;
        [SerializeField] private RectTransform hopHead;
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
        private FigureFit fit;
        private int roundsShown;
        private bool tracing;
        private float fadeLeft;
        private float bounceLeft;
        private float roundPauseLeft;
        private float drawLeft;

        /// <summary>Whether the board shows the handwritten "tip I" lines; read by the PlayMode tests.</summary>
        public bool ShowsHandwrittenLines => handwrittenLines != null && handwrittenLines.activeSelf;

        /// <summary>The finger reader of the board; used by the PlayMode tests to draw a stroke.</summary>
        public TraceFinger Finger => finger;

        /// <summary>The guide's pale road and faint shadow, one line per stroke; read by the PlayMode tests.</summary>
        public StrokeLineGraphic GuideRoad => guideShadow;

        /// <summary>The dotted hop from a stroke's end to the next start dot; read by the PlayMode tests.</summary>
        public StrokeLineGraphic Hop => hop;

        /// <summary>Whether the hop's arrowhead shows; read by the PlayMode tests.</summary>
        public bool ShowsHop => hopHead.gameObject.activeSelf;

        /// <inheritdoc/>
        public override void Show(ActivityRound round, ActivityViewContext context)
        {
            input = context.Input;
            state = round.Trace;
            badge.Show(round.Language);
            if (printedWord != null)
            {
                printedWord.text = round.Choices[0].Text;
            }

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
                    dots[state.Stroke % dots.Length].Pulse();
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
            if (state == null || state.IsDone)
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
            TraceFigure figure = state.Figure;
            fit = FigureFit.Of(figure, area, letterHeightShare, letterWidthShare);
            bool byHand = figure.Form == LetterForm.Handwritten && handwrittenLines != null;
            printLines.SetActive(!byHand);
            if (handwrittenLines != null)
            {
                handwrittenLines.SetActive(byHand);
            }

            if (byHand)
            {
                PlaceLine(handTopLine, WritingLines.Top, area.width);
                PlaceLine(handMiddleLine, WritingLines.Middle, area.width);
                PlaceLine(handBaseline, WritingLines.Baseline, area.width);
                PlaceLine(handBelowLine, WritingLines.Below, area.width);
                DrawSlantGuides(area.width);
            }
            else
            {
                PlaceLine(topLine, WritingLines.Top, area.width);
                PlaceLine(middleLine, WritingLines.Middle, area.width);
                PlaceLine(baseline, WritingLines.Baseline, area.width);
                PlaceLine(belowLine, WritingLines.Below, area.width);
            }
        }

        private void DrawSlantGuides(float boardWidth)
        {
            slantGuides.Clear();
            float halfWidth = boardWidth * 0.45f / fit.Scale;
            float rise = (WritingLines.Below - WritingLines.Top) * state.Figure.Slant;
            for (float x = fit.CentreX - halfWidth; x + rise <= fit.CentreX + halfWidth; x += SlantGuideSpacing)
            {
                slantGuides.BeginLine();
                slantGuides.Add(Local(new ShapePoint(x, WritingLines.Below)));
                slantGuides.Add(Local(new ShapePoint(x + rise, WritingLines.Top)));
            }
        }


        private void PlaceLine(RectTransform line, float y, float boardWidth)
        {
            line.anchoredPosition = new Vector2(0f, (LinesMiddle - y) * fit.Scale);
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
                roundStars[star].gameObject.SetActive(star < state.Rounds);
                roundStars[star].color = star <= state.Round ? starLit : starUnlit;
            }
        }

        private void ShowGuide()
        {
            guideDots.Clear();
            guideShadow.Clear();
            guideShadow.SetWidth(2f * state.GuideHalfWidth * fit.Scale);
            Vector2 offset = new Vector2(-fit.CentreX * fit.Scale, 0f);
            int answerEnd = AnswerEnd();
            for (int stroke = 0; stroke < state.Figure.Strokes.Count; stroke++)
            {
                bool shownForAnswer = state.AnswerShown && stroke >= state.Stroke && stroke <= answerEnd;
                StrokePath path = state.Figure.Strokes[stroke].Path;
                if (state.Guide == TraceGuide.Dotted || shownForAnswer)
                {
                    AddPath(guideDots, path, fit.Scale, offset);
                    AddPath(guideShadow, path, fit.Scale, offset);
                }
                else if (state.Guide == TraceGuide.Faint)
                {
                    AddPath(guideShadow, path, fit.Scale, offset);
                }
            }
        }

        private void ShowDotsAndArrow(bool arrowAlways)
        {
            TraceFigure figure = state.Figure;
            foreach (StartDot dot in dots)
            {
                dot.Hide();
            }

            ShowDot(state.Stroke, true);
            if (state.Guide == TraceGuide.Dotted && state.Stroke + 1 < figure.Strokes.Count && !figure.Continues(state.Stroke + 1))
            {
                ShowDot(state.Stroke + 1, false);
            }

            arrow.Clear();
            StrokePath path = figure.Strokes[state.Stroke].Path;
            bool guided = arrowAlways || state.Guide != TraceGuide.StartDotOnly;
            ShowHop(guided);
            bool showArrow = guided && !path.IsDot;
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

        private void ShowHop(bool guided)
        {
            hop.Clear();
            int stroke = state.Stroke;
            bool lifted = stroke > 0 && !state.Figure.Continues(stroke);
            hopHead.gameObject.SetActive(guided && lifted);
            if (!guided || !lifted)
            {
                return;
            }

            Vector2 from = Local(state.Figure.Strokes[stroke - 1].Path.End);
            Vector2 to = Local(state.Figure.Strokes[stroke].Path.Start);
            Vector2 across = to - from;
            float length = across.magnitude;
            Vector2 lift = new Vector2(-across.y, across.x) * HopLift;
            if (lift.y < 0f)
            {
                lift = -lift;
            }

            Vector2 control = (from + to) * 0.5f + lift;
            float gap = ((RectTransform)dots[stroke % dots.Length].transform).rect.width * HopGapShare;
            float end = Mathf.Clamp(1f - gap / Mathf.Max(length, 1f), 0.5f, 1f);
            int steps = Mathf.Max(2, Mathf.CeilToInt(length / (HopStep * fit.Scale)));
            hop.BeginLine();
            Vector2 tip = from;
            for (int step = 0; step <= steps; step++)
            {
                tip = HopPoint(from, control, to, end * step / steps);
                hop.Add(tip);
            }

            Vector2 behind = HopPoint(from, control, to, end * (steps - 1) / steps);
            hopHead.anchoredPosition = tip;
            hopHead.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(tip.y - behind.y, tip.x - behind.x) * Mathf.Rad2Deg);
        }

        private static Vector2 HopPoint(Vector2 from, Vector2 control, Vector2 to, float share)
        {
            float rest = 1f - share;
            return rest * rest * from + 2f * rest * share * control + share * share * to;
        }

        private void ShowDot(int stroke, bool current)
        {
            int number = state.Figure.NumberInLetter(stroke);
            dots[stroke % dots.Length].Show(Local(state.Figure.Strokes[stroke].Path.Start), Numbers[number % Numbers.Length], current);
        }

        private int AnswerEnd()
        {
            int end = state.Stroke;
            if (state.Figure.LetterCount > 1)
            {
                while (end + 1 < state.Figure.Strokes.Count && !state.Figure.EndsLine(end) && !state.Figure.IsMark(end))
                {
                    end++;
                }
            }

            return end;
        }

        private void StrokeKept()
        {
            ink.BeginLine();
            for (int index = 0; index < state.LastPointsKept && index < points.Count; index++)
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
                hop.Clear();
                hopHead.gameObject.SetActive(false);
                foreach (StartDot dot in dots)
                {
                    dot.Hide();
                }

                if (!state.IsDone)
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
            int end = AnswerEnd();
            float total = 0f;
            for (int stroke = state.Stroke; stroke <= end; stroke++)
            {
                total += state.Figure.Strokes[stroke].Path.Length;
            }

            float reached = total * share;
            Vector2 tip = Vector2.zero;
            drawing.Clear();
            for (int stroke = state.Stroke; stroke <= end && reached >= 0f; stroke++)
            {
                StrokePath path = state.Figure.Strokes[stroke].Path;
                drawing.BeginLine();
                for (int index = 0; index < path.Count && path.DistanceAt(index) <= reached; index++)
                {
                    drawing.Add(Local(new ShapePoint(path.X(index), path.Y(index))));
                }

                tip = Local(path.PointAt(reached));
                drawing.Add(tip);
                reached -= path.Length;
            }

            fingerGlow.rectTransform.anchoredPosition = tip;
            fingerGlow.canvasRenderer.SetAlpha(share < 1f ? 1f : 0f);
        }

        private void AddPoint(Vector2 local)
        {
            ShapePoint at = fit.ShapeAt(local);
            if (points.Add(at.X, at.Y))
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
            foreach (Stroke stroke in state.Figure.Strokes)
            {
                for (int index = 0; index < stroke.Path.Count; index++)
                {
                    top = Mathf.Min(top, stroke.Path.Y(index));
                    bottom = Mathf.Max(bottom, stroke.Path.Y(index));
                }
            }

            Rect area = picture.rectTransform.rect;
            float width = Mathf.Max(state.Figure.MaxX - state.Figure.MinX, 1f);
            float pictureScale = Mathf.Min(area.height / Mathf.Max(bottom - top, 1f), area.width / width);
            Vector2 offset = new Vector2(-fit.CentreX * pictureScale, ((top + bottom) * 0.5f - LinesMiddle) * pictureScale);
            picture.Clear();
            foreach (Stroke stroke in state.Figure.Strokes)
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
            return fit.Local(point);
        }
    }
}
