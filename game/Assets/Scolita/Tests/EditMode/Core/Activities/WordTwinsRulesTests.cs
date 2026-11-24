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
    /// Tests <see cref="WordTwinsRules"/> with planet 1's real pairs (<c>party-1</c>: the false friends pat / pat
    /// and the twins planetă / planet); <see cref="FixedRandom"/> keeps each column in the step's order: ro-pat,
    /// ro-planeta, then en-pat, en-planet.
    /// </summary>
    public sealed class WordTwinsRulesTests
    {
        /// <summary>
        /// Tests that the Romanian words come first and the English ones after, each column shuffled on its own;
        /// a readable word is written and a listen-only one is its picture; and it speaks Romanian (D1).
        /// </summary>
        [Test]
        public void TestBegin_TwoColumns()
        {
            WordTwinsRules rules = Party();

            for (ulong seed = 0; seed < 20; seed++)
            {
                rules.Begin(new SeededRandom(seed));

                Assert.That(rules.Round.Choices.Take(2).Select(choice => choice.Item.ToString()), Is.EquivalentTo(new[] { "ro-pat", "ro-planeta" }));
                Assert.That(rules.Round.Choices.Skip(2).Select(choice => choice.Item.ToString()), Is.EquivalentTo(new[] { "en-pat", "en-planet" }));
            }

            Assert.That(rules.RomanianWords, Is.EqualTo(2));
            Assert.That(rules.Language, Is.EqualTo(Language.Romanian));
            Assert.That(rules.Round.Choices.First(choice => choice.Item.ToString() == "ro-pat").Look, Is.EqualTo(ActivityChoiceLook.Word));
            Assert.That(rules.Round.Choices.First(choice => choice.Item.ToString() == "ro-planeta").Look,
                Is.EqualTo(ActivityChoiceLook.Picture));
            Assert.That(rules.Round.Current, Is.EqualTo(-1));
            Assert.That(SequenceLines.Of(rules.Instruction), Is.EqualTo(new[] { "ro-line-twins" }));
        }

        /// <summary>
        /// Tests that a tap says a word and puts it in play (its partner the answer), that a tap on its partner
        /// joins them while a tap on another word only says that one (she may be listening: never a miss), and that
        /// partners stay joined with the line about which they are.
        /// </summary>
        [Test]
        public void TestJudge_TapThenTapJoins()
        {
            WordTwinsRules rules = Party();
            rules.Begin(new FixedRandom());

            Assert.That(rules.Judge(ActivityAnswer.Tap(0)), Is.EqualTo(AnswerVerdict.Neutral));
            Assert.That(SequenceLines.Of(rules.Response), Is.EqualTo(new[] { "ro-word-pat" }));
            Assert.That(rules.Round.Current, Is.EqualTo(0));
            Assert.That(rules.Round.Answer, Is.EqualTo(2));
            Assert.That(rules.Judge(ActivityAnswer.Tap(3)), Is.EqualTo(AnswerVerdict.Neutral), "not its partner: heard, not a miss");
            Assert.That(SequenceLines.Of(rules.Response), Is.EqualTo(new[] { "en-word-planet" }));
            Assert.That(rules.Round.Current, Is.EqualTo(3));
            Assert.That(rules.Judge(ActivityAnswer.Tap(2)), Is.EqualTo(AnswerVerdict.Neutral), "the same column: another word in play");
            Assert.That(rules.Round.Current, Is.EqualTo(2));
            Assert.That(rules.Judge(ActivityAnswer.Tap(1)), Is.EqualTo(AnswerVerdict.Neutral), "not its partner either");
            Assert.That(rules.Round.Current, Is.EqualTo(1));
            Assert.That(rules.Round.Answer, Is.EqualTo(3));
            Assert.That(rules.Judge(ActivityAnswer.Tap(3)), Is.EqualTo(AnswerVerdict.Right));
            Assert.That(SequenceLines.Of(rules.Response), Is.EqualTo(new[] { "ro-line-twins-done" }));
            Assert.That(rules.Round.StateOf(1), Is.EqualTo(ChoiceState.Done));
            Assert.That(rules.Round.PlacedOn(1), Is.EqualTo(3));
            Assert.That(rules.Round.PlacedOn(3), Is.EqualTo(1));
            Assert.That(rules.Round.Current, Is.EqualTo(-1));
            Assert.That(rules.Judge(ActivityAnswer.Tap(3)), Is.EqualTo(AnswerVerdict.Neutral), "a joined word is said again");
            Assert.That(SequenceLines.Of(rules.Response), Is.EqualTo(new[] { "en-word-planet" }));
            Assert.That(rules.Judge(ActivityAnswer.Join(2, 0)), Is.EqualTo(AnswerVerdict.Done));
            Assert.That(SequenceLines.Of(rules.Response), Is.EqualTo(new[] { "ro-line-false-friends" }));
        }

        /// <summary>
        /// Tests that a wrong join is a miss that keeps the first word in play, that a line within one column or
        /// to a joined word is not an answer, and that only taps and joins are taken.
        /// </summary>
        [Test]
        public void TestJudge_AWrongJoin()
        {
            WordTwinsRules rules = Party();
            rules.Begin(new FixedRandom());

            Assert.That(rules.Judge(ActivityAnswer.Join(0, 3)), Is.EqualTo(AnswerVerdict.Wrong));
            Assert.That(rules.Round.Current, Is.EqualTo(0));
            Assert.That(rules.Round.Answer, Is.EqualTo(2));
            Assert.That(rules.Judge(ActivityAnswer.Join(0, 1)), Is.EqualTo(AnswerVerdict.Neutral));
            rules.Judge(ActivityAnswer.Join(1, 3));
            Assert.That(rules.Judge(ActivityAnswer.Join(0, 3)), Is.EqualTo(AnswerVerdict.Neutral));
            Assert.That(() => rules.Judge(ActivityAnswer.Drop(0)), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => rules.Judge(ActivityAnswer.Join(0, 4)), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        /// <summary>
        /// Tests that the hint reveals the pictures of the word in play and its partner, saying to look at them for
        /// twins and to look at the spelling for false friends; and that the answer shown glows the partner.
        /// </summary>
        [Test]
        public void TestHintFor_ThePairsPictures()
        {
            WordTwinsRules rules = Party();
            rules.Begin(new FixedRandom());

            rules.Judge(ActivityAnswer.Join(0, 3));
            ActivityHint falseFriend = rules.HintFor(1, ActivityAnswer.Join(0, 3));
            Assert.That(falseFriend.Cue, Is.EqualTo(HintCue.RevealPictures));
            Assert.That(falseFriend.Choice, Is.EqualTo(0));
            Assert.That(SequenceLines.Of(falseFriend.Line), Is.EqualTo(new[] { "ro-line-twins-hint-spelled" }));
            rules.Judge(ActivityAnswer.Join(1, 2));
            Assert.That(SequenceLines.Of(rules.HintFor(1, ActivityAnswer.Join(1, 2)).Line), Is.EqualTo(new[] { "ro-line-twins-hint-pictures" }));
            Assert.That(SequenceLines.Of(rules.HintFor(2, ActivityAnswer.Join(1, 2)).Line), Is.EqualTo(new[] { "ro-line-twins-hint-sound" }));
            ActivityHint shown = rules.AnswerShown();
            Assert.That(shown.Cue, Is.EqualTo(HintCue.GlowAnswer));
            Assert.That(shown.Choice, Is.EqualTo(3));
            Assert.That(SequenceLines.Of(shown.Line), Is.EqualTo(new[] { "ro-line-twins-hint-show" }));
            Assert.That(rules.Lines, Has.Member(falseFriend.Line).And.Member(shown.Line));
        }

        /// <summary>
        /// Tests the <c>WordTwins</c> step through <see cref="LessonRun.Answer"/>: each pair joined is a part, a
        /// wrong join gets the hint, a second shows the partner, and the pair then joined counts as practice.
        /// </summary>
        [Test]
        public void TestAnswer_HintThenShowThroughTheRun()
        {
            Curriculum curriculum = RealContent.Read();
            LessonPlan plan = LessonPlan.Of(RealContent.Lesson(curriculum, "party-1"), LanguageMix.Both,
                new PlayableSteps(new[] { ActivityKind.WordTwins }, curriculum), new Step[0], 1);
            LessonRun run = new LessonRun(plan, curriculum, TestLearningRules.Default(), new FixedRandom());
            List<AnswerChecked> answers = new List<AnswerChecked>();
            run.AnswerChecked += answers.Add;
            run.Start();

            Assert.That(run.Answer(ActivityAnswer.Tap(0)).Outcome, Is.EqualTo(FeedbackOutcome.Neutral));
            Assert.That(run.Answer(ActivityAnswer.Tap(3)).Outcome, Is.EqualTo(FeedbackOutcome.Neutral), "listening is never a miss");
            Assert.That(run.Answer(ActivityAnswer.Join(0, 3)).Outcome, Is.EqualTo(FeedbackOutcome.TryAgain));
            StepFeedback shown = run.Answer(ActivityAnswer.Join(0, 3));
            Assert.That(shown.Outcome, Is.EqualTo(FeedbackOutcome.ShowAnswer));
            Assert.That(shown.Hint.Choice, Is.EqualTo(2));
            Assert.That(run.Answer(ActivityAnswer.Tap(2)).Outcome, Is.EqualTo(FeedbackOutcome.Right));
            Assert.That(run.Answer(ActivityAnswer.Join(3, 1)).Outcome, Is.EqualTo(FeedbackOutcome.Done));
            Assert.That(answers.ConvertAll(answer => answer.Result), Is.EqualTo(new[]
            {
                AnswerResult.Wrong, AnswerResult.Wrong, AnswerResult.Practised, AnswerResult.Right
            }));
        }

        private static WordTwinsRules Party()
        {
            Curriculum curriculum = RealContent.Read();
            return (WordTwinsRules)ActivityRulesFactory.Create(RealContent.FirstStep(curriculum, "party-1", ActivityKind.WordTwins),
                curriculum, TestLearningRules.Default());
        }
    }
}
