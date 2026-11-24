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
    /// Tests <see cref="ReadAndMatchRules"/> with planet 1's real words (<c>ro-words-1</c>: mama among tata and
    /// pat; <c>party-1</c>: map among tap and mat, in English); <see cref="FixedRandom"/> keeps the pictures in
    /// the step's order.
    /// </summary>
    public sealed class ReadAndMatchRulesTests
    {
        /// <summary>
        /// Tests that the round shows the first word written (the subject) and the pictures of all the step's
        /// words, shuffled, the answer being the first word's picture.
        /// </summary>
        [Test]
        public void TestBegin_TheWordAndThePictures()
        {
            IActivityRules rules = Mama();

            for (ulong seed = 0; seed < 20; seed++)
            {
                rules.Begin(new SeededRandom(seed));

                Assert.That(rules.Round.Choices.Select(choice => choice.Picture.ToString()), Is.EquivalentTo(new[] { "mother", "father", "bed" }));
                Assert.That(rules.Round.Choices[rules.Round.Answer].Item.ToString(), Is.EqualTo("ro-mama"));
            }

            Assert.That(rules.Round.Subject.Item.ToString(), Is.EqualTo("ro-mama"));
            Assert.That(rules.Round.Subject.Look, Is.EqualTo(ActivityChoiceLook.Word));
            Assert.That(rules.Round.Subject.Text, Is.EqualTo("mama"));
            Assert.That(rules.Round.Choices.All(choice => choice.Look == ActivityChoiceLook.Picture), Is.True);
        }

        /// <summary>
        /// Tests that the word is never said before her first try (the brief's acceptance): the instruction and
        /// the prompt don't name it until a miss, and from then on the prompt is the word.
        /// </summary>
        [Test]
        public void TestPrompt_NeverTheWordBeforeTheFirstTry()
        {
            IActivityRules rules = Mama();
            rules.Begin(new FixedRandom());

            Assert.That(SequenceLines.Of(rules.Instruction), Is.EqualTo(new[] { "ro-line-read" }));
            Assert.That(SequenceLines.Of(rules.Prompt), Is.EqualTo(new[] { "ro-line-read" }));
            Assert.That(rules.Judge(ActivityAnswer.Tap(1)), Is.EqualTo(AnswerVerdict.Wrong));
            Assert.That(SequenceLines.Of(rules.Prompt), Is.EqualTo(new[] { "ro-word-mama" }));
            Assert.That(rules.Judge(ActivityAnswer.Tap(0)), Is.EqualTo(AnswerVerdict.Done));
            Assert.That(rules.Response, Is.Null);

            rules.Begin(new FixedRandom());
            Assert.That(SequenceLines.Of(rules.Prompt), Is.EqualTo(new[] { "ro-line-read" }), "a new beginning forgets the try");
            Assert.That(() => rules.Judge(ActivityAnswer.Tap(3)), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => rules.Judge(ActivityAnswer.Join(0, 1)), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        /// <summary>
        /// Tests that the first hint spells the word out (its letters lit with their sounds, the picture missed
        /// fading), the next one asks her to tap the word, and the answer shown glows the picture and blends the
        /// word.
        /// </summary>
        [Test]
        public void TestHintFor_SpellOutThenTapTheWord()
        {
            IActivityRules rules = Mama();
            rules.Begin(new FixedRandom());

            ActivityHint first = rules.HintFor(1, ActivityAnswer.Tap(2));
            Assert.That(first.Cue, Is.EqualTo(HintCue.SpellOut));
            Assert.That(first.Choice, Is.EqualTo(2));
            Assert.That(SequenceLines.Of(first.Line), Is.EqualTo(new[] { "ro-line-read-hint-sounds", "ro-slow-mama" }));
            ActivityHint second = rules.HintFor(2, ActivityAnswer.Tap(1));
            Assert.That(second.Cue, Is.EqualTo(HintCue.FadeChoice));
            Assert.That(SequenceLines.Of(second.Line), Is.EqualTo(new[] { "ro-line-read-hint-tap" }));
            ActivityHint shown = rules.AnswerShown();
            Assert.That(shown.Cue, Is.EqualTo(HintCue.GlowAnswer));
            Assert.That(shown.Choice, Is.EqualTo(0));
            Assert.That(SequenceLines.Of(shown.Line), Is.EqualTo(new[] { "ro-line-read-hint-blend" }));
            Assert.That(rules.Lines, Has.Member(first.Line).And.Member(shown.Line));
        }

        /// <summary>Tests the English step of the party: it speaks English, its pictures English.</summary>
        [Test]
        public void TestRound_EnglishInTheParty()
        {
            Curriculum curriculum = RealContent.Read();
            Step step = RealContent.Lesson(curriculum, "party-1").Steps
                .First(candidate => candidate.Kind == ActivityKind.ReadAndMatch && candidate.Languages == LanguageMix.EnglishOnly);
            IActivityRules rules = ActivityRulesFactory.Create(step, curriculum, TestLearningRules.Default());

            Assert.That(rules.Language, Is.EqualTo(Language.English));
            Assert.That(rules.Round.Subject.Text, Is.EqualTo("map"));
            Assert.That(SequenceLines.Of(rules.Instruction), Is.EqualTo(new[] { "en-line-read" }));
        }

        /// <summary>
        /// Tests the <c>ReadAndMatch</c> step through <see cref="LessonRun.Answer"/>: a miss gets the spelled-out
        /// hint, a second shows the picture, and the picture then tapped counts as practice.
        /// </summary>
        [Test]
        public void TestAnswer_HintThenShowThroughTheRun()
        {
            Curriculum curriculum = RealContent.Read();
            LessonPlan plan = LessonPlan.Of(RealContent.Lesson(curriculum, "ro-words-1"), LanguageMix.Both,
                new PlayableSteps(new[] { ActivityKind.ReadAndMatch }, curriculum), new Step[0], 1);
            LessonRun run = new LessonRun(plan, curriculum, TestLearningRules.Default(), new FixedRandom());
            List<AnswerChecked> answers = new List<AnswerChecked>();
            run.AnswerChecked += answers.Add;
            run.Start();

            StepFeedback first = run.Answer(ActivityAnswer.Tap(1));
            Assert.That(first.Outcome, Is.EqualTo(FeedbackOutcome.TryAgain));
            Assert.That(first.Hint.Cue, Is.EqualTo(HintCue.SpellOut));
            Assert.That(run.Answer(ActivityAnswer.Tap(2)).Outcome, Is.EqualTo(FeedbackOutcome.ShowAnswer));
            Assert.That(run.Answer(ActivityAnswer.Tap(0)).Outcome, Is.EqualTo(FeedbackOutcome.Done));
            Assert.That(answers.ConvertAll(answer => answer.Result),
                Is.EqualTo(new[] { AnswerResult.Wrong, AnswerResult.Wrong, AnswerResult.Practised }));
        }

        private static IActivityRules Mama()
        {
            Curriculum curriculum = RealContent.Read();
            return ActivityRulesFactory.Create(RealContent.FirstStep(curriculum, "ro-words-1", ActivityKind.ReadAndMatch), curriculum,
                TestLearningRules.Default());
        }
    }
}
