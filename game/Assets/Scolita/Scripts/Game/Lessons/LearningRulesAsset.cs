using Scolita.Core.Lessons;
using Scolita.Core.Tracing;
using UnityEngine;

namespace Scolita.Game.Lessons
{
    /// <summary>
    /// The learning rules asset (<c>Assets/Scolita/Settings/LearningRules.asset</c>): the numbers of
    /// how she learns, in one place, read into Core's <see cref="LearningRules"/>. The tracing numbers are in a
    /// letter shape's units (a small letter is 100 high).
    /// <c>progress-review</c> adds <c>docs/curriculum.md</c> §5's numbers here.
    /// </summary>
    [CreateAssetMenu(fileName = "LearningRules", menuName = "Scolita/Learning rules")]
    public sealed class LearningRulesAsset : ScriptableObject
    {
        [SerializeField, Min(1)] private int triesBeforeAnswerShown = 2;
        [SerializeField, Min(0)] private int praiseNotRepeatedWithin = 3;
        [SerializeField, Range(LetterHuntTuning.FewestStars, LetterHuntTuning.MostStars)] private int huntStars = 10;
        [SerializeField, Min(1)] private int huntRightStars = 4;
        [SerializeField, Min(1f)] private float huntFastestSpeed = 60f;
        [SerializeField, Min(1f)] private float traceDottedCorridor = 32f;
        [SerializeField, Min(1f)] private float traceDottedStartRadius = 34f;
        [SerializeField, Range(0.1f, 1f)] private float traceDottedCoverage = 0.8f;
        [SerializeField, Min(1f)] private float traceFaintCorridor = 28f;
        [SerializeField, Min(1f)] private float traceFaintStartRadius = 30f;
        [SerializeField, Range(0.1f, 1f)] private float traceFaintCoverage = 0.85f;
        [SerializeField, Min(1f)] private float traceStartDotCorridor = 24f;
        [SerializeField, Min(1f)] private float traceStartDotStartRadius = 28f;
        [SerializeField, Range(0.1f, 1f)] private float traceStartDotCoverage = 0.9f;
        [SerializeField, Min(1f)] private float traceJoinRadius = 44f;

        /// <summary>
        /// The rules, for Core; throws an <see cref="System.ArgumentOutOfRangeException"/> for a number
        /// out of range. Called by <see cref="LessonRunner"/> when a lesson is built, and the tests.
        /// </summary>
        public LearningRules ToRules()
        {
            return new LearningRules(triesBeforeAnswerShown, praiseNotRepeatedWithin,
                new LetterHuntTuning(huntStars, huntRightStars, huntFastestSpeed),
                new TracingTuning(new StrokeTolerance(traceDottedCorridor, traceDottedStartRadius, traceDottedCoverage),
                    new StrokeTolerance(traceFaintCorridor, traceFaintStartRadius, traceFaintCoverage),
                    new StrokeTolerance(traceStartDotCorridor, traceStartDotStartRadius, traceStartDotCoverage),
                    traceJoinRadius));
        }
    }
}
