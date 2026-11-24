using System.Linq;
using NUnit.Framework;
using Scolita.Core.Activities;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Activities
{
    /// <summary>Tests <see cref="ActivityRulesFactory"/> on the real content.</summary>
    public sealed class ActivityRulesFactoryTests
    {
        /// <summary>
        /// Tests that <see cref="ActivityRulesFactory.Create"/> makes rules of the step's kind and language
        /// for every step of a kind with rules, in every lesson.
        /// </summary>
        [Test]
        public void TestCreate_EveryStepOfAKindWithRules()
        {
            Curriculum curriculum = RealContent.Read();
            Step[] steps = curriculum.AllLessons
                .SelectMany(lesson => lesson.Steps)
                .Where(step => ActivityRulesFactory.Has(step.Kind))
                .ToArray();

            Assert.That(steps.Select(step => step.Kind).Distinct(), Is.EquivalentTo(new[]
            {
                ActivityKind.MeetLetter, ActivityKind.ListenAndPick, ActivityKind.LetterHunt, ActivityKind.FirstSound,
                ActivityKind.TwoSounds, ActivityKind.Syllables, ActivityKind.BuildWord, ActivityKind.ReadAndMatch,
                ActivityKind.WordTwins
            }));
            foreach (Step step in steps)
            {
                IActivityRules rules = ActivityRulesFactory.Create(step, curriculum, TestLearningRules.Default());

                Assert.That(rules.Kind, Is.EqualTo(step.Kind));
                Assert.That(step.Languages.Includes(rules.Language), Is.True);
                Assert.That(rules.Round.Choices.All(choice => step.Languages.Includes(choice.Language)), Is.True);
                Assert.That(rules.Round.Choices.All(choice => choice.Language == rules.Language),
                    Is.EqualTo(!ActivityKinds.Of(step.Kind).IsBridge));
                Assert.That(rules.Lines, Has.Member(rules.Instruction));
            }
        }

        /// <summary>Tests that <see cref="ActivityRulesFactory.Create"/> refuses a kind with no rules yet.</summary>
        [Test]
        public void TestCreate_AKindWithoutRules()
        {
            Curriculum curriculum = RealContent.Read();
            Step trace = RealContent.FirstStep(curriculum, "ro-a", ActivityKind.TraceLetter);

            Assert.That(ActivityRulesFactory.Has(ActivityKind.TraceLetter), Is.False);
            Assert.That(() => ActivityRulesFactory.Create(trace, curriculum, TestLearningRules.Default()), Throws.ArgumentException);
        }
    }
}
