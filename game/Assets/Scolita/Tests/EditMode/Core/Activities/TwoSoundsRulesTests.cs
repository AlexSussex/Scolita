using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Activities;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;
using Scolita.Core.Randomness;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Activities
{
    /// <summary>
    /// Tests <see cref="TwoSoundsRules"/> with planet 1's real items (<c>en-a</c>: a, with avion, albină,
    /// apple and astronaut); <see cref="FixedRandom"/> keeps the pictures in the step's order.
    /// </summary>
    public sealed class TwoSoundsRulesTests
    {
        /// <summary>
        /// Tests that the round has the Romanian planet, then the English one, then the pictures, shuffled among
        /// themselves, about the letter a, and that it speaks its lesson's language, English.
        /// </summary>
        [Test]
        public void TestBegin_TwoPlanetsThenThePictures()
        {
            IActivityRules rules = EnglishA();

            for (ulong seed = 0; seed < 20; seed++)
            {
                rules.Begin(new SeededRandom(seed));

                Assert.That(rules.Round.Choices[TwoSoundsRules.RomanianPlanet].Language, Is.EqualTo(Language.Romanian));
                Assert.That(rules.Round.Choices[TwoSoundsRules.EnglishPlanet].Language, Is.EqualTo(Language.English));
                Assert.That(rules.Round.Choices.Skip(TwoSoundsRules.FirstPicture).Select(choice => choice.Item.ToString()),
                    Is.EquivalentTo(new[] { "ro-avion", "ro-albina", "en-apple", "en-astronaut" }));
            }

            Assert.That(rules.Language, Is.EqualTo(Language.English));
            Assert.That(rules.Round.Subject.Text, Is.EqualTo("a"));
            Assert.That(rules.Round.Current, Is.EqualTo(-1));
            Assert.That(SequenceLines.Of(rules.Instruction), Is.EqualTo(new[] { "en-line-two-same-letter", "en-line-two-tap" }));
            Assert.That(SequenceLines.Of(rules.Prompt), Is.EqualTo(new[] { "en-line-two-tap" }));
        }

        /// <summary>
        /// Tests that the sorting waits until both planets are heard: a drop before is ignored, each tap says
        /// its planet's sound, and the second planet heard brings the first picture, its word said.
        /// </summary>
        [Test]
        public void TestJudge_TheSortingWaitsForBothSounds()
        {
            IActivityRules rules = EnglishA();
            rules.Begin(new FixedRandom());

            AnswerVerdict early = rules.Judge(ActivityAnswer.Drop(TwoSoundsRules.RomanianPlanet));
            AnswerVerdict romanian = rules.Judge(ActivityAnswer.Tap(TwoSoundsRules.RomanianPlanet));
            string[] romanianSaid = SequenceLines.Of(rules.Response);
            AnswerVerdict again = rules.Judge(ActivityAnswer.Tap(TwoSoundsRules.RomanianPlanet));
            int stillNone = rules.Round.Current;
            AnswerVerdict english = rules.Judge(ActivityAnswer.Tap(TwoSoundsRules.EnglishPlanet));

            Assert.That(early, Is.EqualTo(AnswerVerdict.Neutral));
            Assert.That(romanian, Is.EqualTo(AnswerVerdict.Neutral));
            Assert.That(romanianSaid, Is.EqualTo(new[] { "ro-sound-a" }));
            Assert.That(again, Is.EqualTo(AnswerVerdict.Neutral));
            Assert.That(stillNone, Is.EqualTo(-1));
            Assert.That(english, Is.EqualTo(AnswerVerdict.Neutral));
            Assert.That(SequenceLines.Of(rules.Response), Is.EqualTo(new[] { "en-sound-a", "en-line-two", "ro-word-avion" }));
            Assert.That(rules.Round.StateOf(TwoSoundsRules.RomanianPlanet), Is.EqualTo(ChoiceState.Heard));
            Assert.That(rules.Round.StateOf(TwoSoundsRules.EnglishPlanet), Is.EqualTo(ChoiceState.Heard));
            Assert.That(rules.Round.Current, Is.EqualTo(TwoSoundsRules.FirstPicture));
            Assert.That(rules.Round.Answer, Is.EqualTo(TwoSoundsRules.RomanianPlanet));
            Assert.That(SequenceLines.Of(rules.Prompt), Is.EqualTo(new[] { "ro-word-avion" }));
        }

        /// <summary>
        /// Tests that each picture dropped on the planet of its word's language is one part (the next picture
        /// comes, its word said), on the other planet a miss, and the last one finishes the step.
        /// </summary>
        [Test]
        public void TestJudge_EachPictureToItsPlanet()
        {
            IActivityRules rules = Sorting();

            AnswerVerdict avion = rules.Judge(ActivityAnswer.Drop(TwoSoundsRules.RomanianPlanet));
            string[] next = SequenceLines.Of(rules.Response);
            AnswerVerdict wrong = rules.Judge(ActivityAnswer.Drop(TwoSoundsRules.EnglishPlanet));
            AnswerVerdict albina = rules.Judge(ActivityAnswer.Drop(TwoSoundsRules.RomanianPlanet));
            AnswerVerdict apple = rules.Judge(ActivityAnswer.Drop(TwoSoundsRules.EnglishPlanet));
            AnswerVerdict astronaut = rules.Judge(ActivityAnswer.Drop(TwoSoundsRules.EnglishPlanet));

            Assert.That(avion, Is.EqualTo(AnswerVerdict.Right));
            Assert.That(next, Is.EqualTo(new[] { "ro-word-albina" }));
            Assert.That(wrong, Is.EqualTo(AnswerVerdict.Wrong));
            Assert.That(albina, Is.EqualTo(AnswerVerdict.Right));
            Assert.That(apple, Is.EqualTo(AnswerVerdict.Right));
            Assert.That(astronaut, Is.EqualTo(AnswerVerdict.Done));
            Assert.That(Enumerable.Range(TwoSoundsRules.FirstPicture, 4).All(index => rules.Round.StateOf(index) == ChoiceState.Done),
                Is.True);
            Assert.That(rules.Round.Current, Is.EqualTo(-1));
        }

        /// <summary>
        /// Tests that a tap on the picture in her hand says its word, a tap on a planet while sorting says its
        /// sound, and a drop on a picture is refused.
        /// </summary>
        [Test]
        public void TestJudge_TapsWhileSorting()
        {
            IActivityRules rules = Sorting();

            AnswerVerdict picture = rules.Judge(ActivityAnswer.Tap(TwoSoundsRules.FirstPicture));
            string[] word = SequenceLines.Of(rules.Response);
            AnswerVerdict planet = rules.Judge(ActivityAnswer.Tap(TwoSoundsRules.EnglishPlanet));

            Assert.That(picture, Is.EqualTo(AnswerVerdict.Neutral));
            Assert.That(word, Is.EqualTo(new[] { "ro-word-avion" }));
            Assert.That(planet, Is.EqualTo(AnswerVerdict.Neutral));
            Assert.That(SequenceLines.Of(rules.Response), Is.EqualTo(new[] { "en-sound-a" }));
            Assert.That(rules.Round.Current, Is.EqualTo(TwoSoundsRules.FirstPicture));
            Assert.That(() => rules.Judge(ActivityAnswer.Drop(TwoSoundsRules.FirstPicture)), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => rules.Judge(ActivityAnswer.Tap(6)), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        /// <summary>
        /// Tests that <see cref="TwoSoundsRules.HintFor"/> says both sounds again, one after the other, then asks
        /// for the word's first sound, then explains the badges; and that <see cref="TwoSoundsRules.AnswerShown"/>
        /// glows the right planet.
        /// </summary>
        [Test]
        public void TestHintFor_BothSoundsAgain()
        {
            IActivityRules rules = Sorting();
            ActivityAnswer miss = ActivityAnswer.Drop(TwoSoundsRules.EnglishPlanet);

            ActivityHint first = rules.HintFor(1, miss);
            ActivityHint second = rules.HintFor(2, miss);
            ActivityHint third = rules.HintFor(3, miss);
            ActivityHint shown = rules.AnswerShown();

            Assert.That(SequenceLines.Of(first.Line), Is.EqualTo(new[] { "en-line-two-hint-again", "ro-sound-a", "en-sound-a" }));
            Assert.That(first.Cue, Is.EqualTo(HintCue.None));
            Assert.That(SequenceLines.Of(second.Line), Is.EqualTo(new[] { "en-line-two-hint-start", "ro-word-avion" }));
            Assert.That(SequenceLines.Of(third.Line), Is.EqualTo(new[] { "en-line-two-hint-badges" }));
            Assert.That(shown.Cue, Is.EqualTo(HintCue.GlowAnswer));
            Assert.That(shown.Choice, Is.EqualTo(TwoSoundsRules.RomanianPlanet));
            Assert.That(SequenceLines.Of(shown.Line), Is.EqualTo(new[] { "en-line-two-hint-show" }));
            Assert.That(() => rules.HintFor(0, miss), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        /// <summary>
        /// Tests that <see cref="ActivityRulesFactory.Create"/> makes a <c>TwoSounds</c> step speak Romanian, the
        /// anchor, in a lesson in both languages.
        /// </summary>
        [Test]
        public void TestCreate_RomanianInALessonOfBothLanguages()
        {
            Curriculum curriculum = RealContent.Read();
            Step enA = RealContent.FirstStep(curriculum, "en-a", ActivityKind.TwoSounds);
            Step atTheParty = new Step(ContentId.Create("party-1"), 9, ActivityKind.TwoSounds, LanguageMix.Both, enA.Letters,
                enA.Words, enA.Options, enA.Source);

            IActivityRules rules = ActivityRulesFactory.Create(atTheParty, curriculum, TestLearningRules.Default());

            Assert.That(rules.Language, Is.EqualTo(Language.Romanian));
            Assert.That(SequenceLines.Of(rules.Instruction)[0], Is.EqualTo("ro-line-two-same-letter"));
        }

        /// <summary>
        /// Tests the <c>TwoSounds</c> step through <see cref="LessonRun"/>: hearing the planets is never an
        /// answer, a miss says both sounds, the second shows the planet, and each picture sorted counts.
        /// </summary>
        [Test]
        public void TestLessonRun_HintThenShow()
        {
            Curriculum curriculum = RealContent.Read();
            LessonPlan plan = LessonPlan.Of(RealContent.Lesson(curriculum, "en-a"), LanguageMix.Both,
                new[] { ActivityKind.TwoSounds }, new Step[0], 1);
            LessonRun run = new LessonRun(plan, curriculum, TestLearningRules.Default(), new FixedRandom());
            List<AnswerChecked> answers = new List<AnswerChecked>();
            run.AnswerChecked += answers.Add;
            run.Start();

            StepFeedback heard = run.Answer(ActivityAnswer.Tap(TwoSoundsRules.RomanianPlanet));
            run.Answer(ActivityAnswer.Tap(TwoSoundsRules.EnglishPlanet));
            StepFeedback miss = run.Answer(ActivityAnswer.Drop(TwoSoundsRules.EnglishPlanet));
            StepFeedback shown = run.Answer(ActivityAnswer.Drop(TwoSoundsRules.EnglishPlanet));
            StepFeedback practised = run.Answer(ActivityAnswer.Drop(TwoSoundsRules.RomanianPlanet));
            run.Answer(ActivityAnswer.Drop(TwoSoundsRules.RomanianPlanet));
            run.Answer(ActivityAnswer.Drop(TwoSoundsRules.EnglishPlanet));
            StepFeedback done = run.Answer(ActivityAnswer.Drop(TwoSoundsRules.EnglishPlanet));

            Assert.That(heard.Outcome, Is.EqualTo(FeedbackOutcome.Neutral));
            Assert.That(SequenceLines.Of(heard.First), Is.EqualTo(new[] { "ro-sound-a" }));
            Assert.That(miss.Outcome, Is.EqualTo(FeedbackOutcome.TryAgain));
            Assert.That(SequenceLines.Of(miss.First), Is.EqualTo(new[] { "en-line-two-hint-again", "ro-sound-a", "en-sound-a" }));
            Assert.That(shown.Outcome, Is.EqualTo(FeedbackOutcome.ShowAnswer));
            Assert.That(shown.Hint.Choice, Is.EqualTo(TwoSoundsRules.RomanianPlanet));
            Assert.That(practised.Outcome, Is.EqualTo(FeedbackOutcome.Right));
            Assert.That(SequenceLines.Of(practised.First), Is.EqualTo(new[] { "ro-word-albina" }));
            Assert.That(done.Outcome, Is.EqualTo(FeedbackOutcome.Done));
            Assert.That(SequenceLines.Of(done.Then)[0], Does.StartWith("en-line-praise-"));
            Assert.That(answers.Select(answer => answer.Result), Is.EqualTo(new[]
            {
                AnswerResult.Wrong, AnswerResult.Wrong, AnswerResult.Practised, AnswerResult.Right, AnswerResult.Right,
                AnswerResult.Right
            }));
        }

        private static IActivityRules EnglishA()
        {
            Curriculum curriculum = RealContent.Read();
            return ActivityRulesFactory.Create(RealContent.FirstStep(curriculum, "en-a", ActivityKind.TwoSounds), curriculum,
                TestLearningRules.Default());
        }

        private static IActivityRules Sorting()
        {
            IActivityRules rules = EnglishA();
            rules.Begin(new FixedRandom());
            rules.Judge(ActivityAnswer.Tap(TwoSoundsRules.RomanianPlanet));
            rules.Judge(ActivityAnswer.Tap(TwoSoundsRules.EnglishPlanet));
            return rules;
        }
    }
}
