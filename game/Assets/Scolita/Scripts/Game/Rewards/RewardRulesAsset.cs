using Scolita.Core.Rewards;
using UnityEngine;

namespace Scolita.Game.Rewards
{
    /// <summary>
    /// The reward rules asset (<c>Assets/Scolita/Settings/RewardRules.asset</c>), beside the learning rules: the numbers
    /// of the rewards in one place, read into Core's <see cref="RewardRules"/>. The stars lines in the content say
    /// "3 stele" / "3 stars": change them with <c>starsPerLesson</c>.
    /// </summary>
    [CreateAssetMenu(fileName = "RewardRules", menuName = "Scolita/Reward rules")]
    public sealed class RewardRulesAsset : ScriptableObject
    {
        [SerializeField, Min(1)] private int starsPerLesson = 3;
        [SerializeField, Min(1)] private int lessonsPerSticker = 2;
        [SerializeField, Min(0.1f)] private float smallestStickerSize = 0.5f;
        [SerializeField, Min(0.1f)] private float largestStickerSize = 2.5f;

        /// <summary>
        /// The rules, for Core; throws an <see cref="System.ArgumentOutOfRangeException"/> for a number out of range.
        /// Called by the runners (<c>LessonRunner</c>, <c>MapRunner</c>) and the tests.
        /// </summary>
        public RewardRules ToRules()
        {
            return new RewardRules(starsPerLesson, lessonsPerSticker, smallestStickerSize, largestStickerSize);
        }
    }
}
