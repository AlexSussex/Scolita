using System.Collections.Generic;
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
        /// for every step it can create (<see cref="ActivityRulesFactory.CanCreate"/>: a kind with rules, and for
        /// tracing the letter's shape), in every lesson.
        /// </summary>
        [Test]
        public void TestCreate_EveryStepOfAKindWithRules()
        {
            Curriculum curriculum = RealContent.Read();
            Step[] steps = curriculum.AllLessons
                .SelectMany(lesson => lesson.Steps)
                .Where(step => ActivityRulesFactory.CanCreate(step, curriculum))
                .ToArray();

            Assert.That(steps.Select(step => step.Kind).Distinct(), Is.EquivalentTo(new[]
            {
                ActivityKind.MeetLetter, ActivityKind.ListenAndPick, ActivityKind.LetterHunt, ActivityKind.FirstSound,
                ActivityKind.TwoSounds, ActivityKind.Syllables, ActivityKind.BuildWord, ActivityKind.ReadAndMatch,
                ActivityKind.WordTwins, ActivityKind.TraceLetter, ActivityKind.TraceWord
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

        /// <summary>
        /// Tests that <see cref="ActivityRulesFactory.CanCreate"/> refuses a <c>TraceWord</c> step whose letters have no
        /// handwritten shape, and <see cref="ActivityRulesFactory.Create"/> throws for it.
        /// </summary>
        [Test]
        public void TestCanCreate_AWordWithoutItsShapes()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "ro-m,TraceWord,,,ro-mama,,");
            Curriculum curriculum = ContentCsvs.Read(csvs);
            Step word = curriculum.AllLessons.SelectMany(lesson => lesson.Steps).First(step => step.Kind == ActivityKind.TraceWord);

            Assert.That(ActivityRulesFactory.Has(ActivityKind.TraceWord), Is.True);
            Assert.That(ActivityRulesFactory.CanCreate(word, curriculum), Is.False);
            Assert.That(() => ActivityRulesFactory.Create(word, curriculum, TestLearningRules.Default()),
                Throws.InstanceOf<KeyNotFoundException>());
        }
    }
}
