using Scolita.Core.Lessons;
using UnityEngine;

namespace Scolita.Game.Lessons
{
    /// <summary>
    /// The learning rules asset (<c>Assets/Scolita/Settings/LearningRules.asset</c>): the numbers of
    /// how she learns, in one place, read into Core's <see cref="LearningRules"/>.
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

        /// <summary>
        /// The rules, for Core; throws an <see cref="System.ArgumentOutOfRangeException"/> for a number
        /// out of range. Called by <see cref="LessonRunner"/> when a lesson is built, and the tests.
        /// </summary>
        public LearningRules ToRules()
        {
            return new LearningRules(triesBeforeAnswerShown, praiseNotRepeatedWithin,
                new LetterHuntTuning(huntStars, huntRightStars, huntFastestSpeed));
        }
    }
}
