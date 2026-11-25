using Scolita.Core.Tracing;

namespace Scolita.Core.Activities
{
    /// <summary>
    /// How the tracing kinds (<see cref="TraceLetterRules"/>, <see cref="TraceWordRules"/>) judge one line she drew:
    /// with the round's tolerance (the first round's while the answer is shown), a wider start at a join she
    /// lifted at, along every stroke the line could go through (<see cref="LineCheck"/>). Shared, so both judge alike.
    /// </summary>
    internal static class TraceJudging
    {
        /// <summary>
        /// Judges <paramref name="points"/> from the stroke <paramref name="state"/> is at, records it in the state and
        /// returns it. Called by the tracing rules' <c>Judge</c>.
        /// </summary>
        internal static LineResult Judge(TraceState state, TracingTuning tuning, TracePoints points)
        {
            StrokeTolerance tolerance = tuning.ForRound(state.AnswerShown ? 0 : state.Round);
            if (state.Figure.BeginsAtJoin(state.Stroke))
            {
                tolerance = tolerance.WithStartRadiusAtLeast(tuning.JoinRadius);
            }

            LineResult result = LineCheck.Check(state.Figure, state.Stroke, points, tolerance);
            state.Judged(result);
            return result;
        }
    }
}
