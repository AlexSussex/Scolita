using NUnit.Framework;
using Scolita.Core.Rewards;
using Scolita.Game.Rewards;
using UnityEditor;

namespace Scolita.Tests.EditMode.Content
{
    /// <summary>Tests the project's <see cref="RewardRulesAsset"/> (<c>Assets/Scolita/Settings/RewardRules.asset</c>).</summary>
    public sealed class RewardRulesAssetTests
    {
        /// <summary>
        /// Tests that the reward rules asset gives 3 stars a lesson (the stars lines say "3 stele" / "3 stars"), a sticker
        /// every second lesson (PRD §3.6), and a sticker that can be made from half to two and a half times its size.
        /// </summary>
        [Test]
        public void TestToRules_TheRewardRulesAsset()
        {
            RewardRulesAsset asset = AssetDatabase.LoadAssetAtPath<RewardRulesAsset>("Assets/Scolita/Settings/RewardRules.asset");

            Assert.That(asset, Is.Not.Null);
            RewardRules rules = asset.ToRules();
            Assert.That(rules.StarsPerLesson, Is.EqualTo(3));
            Assert.That(rules.LessonsPerSticker, Is.EqualTo(2));
            Assert.That(rules.SmallestStickerSize, Is.EqualTo(0.5f).Within(0.001f));
            Assert.That(rules.LargestStickerSize, Is.EqualTo(2.5f).Within(0.001f));
        }
    }
}
