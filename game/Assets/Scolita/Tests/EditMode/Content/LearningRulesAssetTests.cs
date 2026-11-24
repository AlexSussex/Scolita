using NUnit.Framework;
using Scolita.Game.Lessons;
using UnityEditor;

namespace Scolita.Tests.EditMode.Content
{
    /// <summary>Tests the project's <see cref="LearningRulesAsset"/> (<c>Assets/Scolita/Settings/LearningRules.asset</c>).</summary>
    public sealed class LearningRulesAssetTests
    {
        /// <summary>
        /// Tests that the learning rules asset holds <c>docs/curriculum.md</c> §5's default, two tries before the
        /// answer is shown, and <c>LetterHunt</c>'s numbers: 10 stars (the brief: 8 to 12), 4 with the letter,
        /// drifting at most 60 reference pixels a second.
        /// </summary>
        [Test]
        public void TestToRules_TheLearningRulesAsset()
        {
            LearningRulesAsset asset = AssetDatabase.LoadAssetAtPath<LearningRulesAsset>("Assets/Scolita/Settings/LearningRules.asset");

            Assert.That(asset, Is.Not.Null);
            Assert.That(asset.ToRules().TriesBeforeAnswerShown, Is.EqualTo(2));
            Assert.That(asset.ToRules().LetterHunt.Stars, Is.EqualTo(10));
            Assert.That(asset.ToRules().LetterHunt.RightStars, Is.EqualTo(4));
            Assert.That(asset.ToRules().LetterHunt.FastestSpeed, Is.EqualTo(60f).Within(0.001f));
        }
    }
}
