using NUnit.Framework;
using Scolita.Game.Lessons;
using UnityEditor;

namespace Scolita.Tests.EditMode.Content
{
    /// <summary>Tests the project's <see cref="LearningRulesAsset"/> (<c>Assets/Scolita/Settings/LearningRules.asset</c>).</summary>
    public sealed class LearningRulesAssetTests
    {
        /// <summary>Tests that the learning rules asset holds <c>docs/curriculum.md</c> §5's default: two tries before the answer is shown.</summary>
        [Test]
        public void TestToRules_TheLearningRulesAsset()
        {
            LearningRulesAsset asset = AssetDatabase.LoadAssetAtPath<LearningRulesAsset>("Assets/Scolita/Settings/LearningRules.asset");

            Assert.That(asset, Is.Not.Null);
            Assert.That(asset.ToRules().TriesBeforeAnswerShown, Is.EqualTo(2));
        }
    }
}
