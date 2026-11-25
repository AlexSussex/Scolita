using Scolita.Core.Lessons;
using Scolita.Core.Parent;
using Scolita.Core.Profiles;
using Scolita.Core.Progress;
using Scolita.Core.Tracing;
using UnityEngine;

namespace Scolita.Game.Lessons
{
    /// <summary>
    /// The learning rules asset (<c>Assets/Scolita/Settings/LearningRules.asset</c>): the numbers of
    /// how she learns, in one place, read into Core's <see cref="LearningRules"/>. The tracing numbers are in a
    /// letter shape's units (a small letter is 100 high). <c>docs/curriculum.md</c> §5's numbers (learned,
    /// the reviews, the warm-ups) and how long a damaged save's copies are kept are read into Core's
    /// <see cref="ProgressRules"/>; the parent gate's and the daily limit's numbers into Core's <see cref="ParentRules"/>; the
    /// most children and the longest name into Core's <see cref="ProfileRules"/>.
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
        [SerializeField, Min(1)] private int romanianRightsForLearned = 6;
        [SerializeField, Min(1)] private int englishRightsForLearned = 6;
        [SerializeField, Min(1)] private int kindsForLearned = 2;
        [SerializeField, Range(1, ItemProgress.MostRightDaysKept)] private int daysForLearned = 2;
        [SerializeField] private int[] reviewIntervalDays = { 1, 3, 7, 14, 30 };
        [SerializeField, Min(0)] private int reviewStepsBackAfterMiss = 1;
        [SerializeField, Min(0)] private int warmUpsAtStart = 2;
        [SerializeField, Range(2, 4)] private int warmUpChoices = 3;
        [SerializeField, Min(1)] private int damagedCopyFineDays = 3;
        [SerializeField, Min(0.5f)] private float gateHoldSeconds = 3f;
        [SerializeField, Min(0)] private int gateLowestNumber = 11;
        [SerializeField, Min(0)] private int gateHighestNumber = 19;
        [SerializeField, Range(ParentRules.FewestGateChoices, ParentRules.MostGateChoices)] private int gateChoices = 3;
        [SerializeField, Min(0f)] private float gateRestSeconds = 30f;
        [SerializeField] private int[] sessionLimitMinutes = { 10, 15, 20 };
        [SerializeField, Range(2, 6)] private int mostChildProfiles = 6;
        [SerializeField, Range(20, 40)] private int nameMostCharacters = 20;

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

        /// <summary>
        /// The numbers of "learned", the reviews and the warm-ups, and the days before a damaged save's copies go, for
        /// Core; throws an <see cref="System.ArgumentOutOfRangeException"/> for a number out of range. Called by
        /// <see cref="LessonRunner"/> when a lesson is built, <c>BootLoader</c> at start, and the tests.
        /// </summary>
        public ProgressRules ToProgressRules()
        {
            return new ProgressRules(romanianRightsForLearned, englishRightsForLearned, kindsForLearned, daysForLearned,
                reviewIntervalDays, reviewStepsBackAfterMiss, warmUpsAtStart, warmUpChoices, damagedCopyFineDays);
        }

        /// <summary>
        /// The parent gate's numbers (the hold, the numbers said, how many are offered, the rest after a wrong one) and the
        /// daily limit's choices, for Core; throws an <see cref="System.ArgumentOutOfRangeException"/> for a number out of
        /// range. Called by <c>MapRunner</c> and <c>ParentRunner</c> when they open, and the tests.
        /// </summary>
        public ParentRules ToParentRules()
        {
            return new ParentRules(gateHoldSeconds, gateLowestNumber, gateHighestNumber, gateChoices, gateRestSeconds,
                sessionLimitMinutes);
        }

        /// <summary>
        /// The most children's profiles and the longest name, for Core; throws an
        /// <see cref="System.ArgumentOutOfRangeException"/> for a number out of range. Called by the profile store and the
        /// tests.
        /// </summary>
        public ProfileRules ToProfileRules()
        {
            return new ProfileRules(mostChildProfiles, nameMostCharacters);
        }
    }
}
