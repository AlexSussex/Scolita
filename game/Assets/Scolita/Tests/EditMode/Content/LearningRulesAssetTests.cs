using NUnit.Framework;
using Scolita.Core.Progress;
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

        /// <summary>
        /// Tests that the learning rules asset holds <c>docs/curriculum.md</c> §5's defaults for "learned" and the reviews:
        /// 6 right answers in each language, across 2 kinds, on 2 days; reviews after 1, 3, 7, 14 and 30 days, a miss one
        /// step back; at most 2 warm-ups of 3 letters; and damaged copies kept until 3 fine days.
        /// </summary>
        [Test]
        public void TestToProgressRules_TheLearningRulesAsset()
        {
            LearningRulesAsset asset = AssetDatabase.LoadAssetAtPath<LearningRulesAsset>("Assets/Scolita/Settings/LearningRules.asset");

            ProgressRules rules = asset.ToProgressRules();

            Assert.That(rules.RomanianRightsForLearned, Is.EqualTo(6));
            Assert.That(rules.EnglishRightsForLearned, Is.EqualTo(6));
            Assert.That(rules.KindsForLearned, Is.EqualTo(2));
            Assert.That(rules.DaysForLearned, Is.EqualTo(2));
            Assert.That(rules.ReviewIntervalDays, Is.EqualTo(new[] { 1, 3, 7, 14, 30 }));
            Assert.That(rules.ReviewStepsBackAfterMiss, Is.EqualTo(1));
            Assert.That(rules.WarmUpsAtStart, Is.EqualTo(2));
            Assert.That(rules.WarmUpChoices, Is.EqualTo(3));
            Assert.That(rules.DamagedCopyFineDays, Is.EqualTo(3));
        }
    }
}
