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

        /// <summary>
        /// The rules, for Core; throws an <see cref="System.ArgumentOutOfRangeException"/> for a number
        /// out of range. Called by <see cref="LessonRunner"/> when a lesson is built, and the tests.
        /// </summary>
        public LearningRules ToRules()
        {
            return new LearningRules(triesBeforeAnswerShown, praiseNotRepeatedWithin);
        }
    }
}
