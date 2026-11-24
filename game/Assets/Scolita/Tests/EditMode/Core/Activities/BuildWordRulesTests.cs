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
    /// Tests <see cref="BuildWordRules"/> with planet 1's real items (<c>ro-words-1</c>: tata with the extra m;
    /// <c>en-words-1</c>: map with the extra t); <see cref="FixedRandom"/> keeps the tiles in the word's order,
    /// the extra letters last.
    /// </summary>
    public sealed class BuildWordRulesTests
    {
        /// <summary>
        /// Tests that the tiles are the word's letters (one per letter, a repeated letter twice) and the extra
        /// letters, shuffled, with one window per letter, and the word's picture as the subject.
        /// </summary>
        [Test]
        public void TestBegin_TheTilesAndTheWindows()
        {
            IActivityRules rules = Tata();

            for (ulong seed = 0; seed < 20; seed++)
            {
                rules.Begin(new SeededRandom(seed));

                Assert.That(rules.Round.Choices.Select(choice => choice.Item.ToString()), Is.EquivalentTo(new[] { "t", "a", "t", "a", "m" }));
                Assert.That(rules.Round.Choices[rules.Round.Answer].Item.ToString(), Is.EqualTo("t"));
            }

            Assert.That(rules.Round.Places, Is.EqualTo(4));
            Assert.That(rules.Round.PlacesFilled, Is.EqualTo(0));
            Assert.That(rules.Round.Subject.Item.ToString(), Is.EqualTo("ro-tata"));
            Assert.That(rules.Round.Subject.Picture.ToString(), Is.EqualTo("father"));
            Assert.That(rules.Round.Choices.All(choice => choice.Language == Language.Romanian), Is.True);
        }

        /// <summary>
        /// Tests that a tile with the next window's letter fills it (either of a repeated letter), that the
        /// last one finishes the step with the take-off line, that another tile is a miss, and that a tap only
        /// says a tile's sound.
        /// </summary>
        [Test]
        public void TestJudge_TheWindowsInOrder()
        {
            IActivityRules rules = Tata();
            rules.Begin(new FixedRandom());

            Assert.That(rules.Judge(ActivityAnswer.Tap(4)), Is.EqualTo(AnswerVerdict.Neutral));
            Assert.That(SequenceLines.Of(rules.Response), Is.EqualTo(new[] { "ro-sound-m" }));
            Assert.That(rules.Judge(ActivityAnswer.Place(4)), Is.EqualTo(AnswerVerdict.Wrong));
            Assert.That(rules.Judge(ActivityAnswer.Place(1)), Is.EqualTo(AnswerVerdict.Wrong));
            Assert.That(rules.Judge(ActivityAnswer.Place(2)), Is.EqualTo(AnswerVerdict.Right));
            Assert.That(SequenceLines.Of(rules.Response), Is.EqualTo(new[] { "ro-sound-t" }));
            Assert.That(rules.Round.PlacedOn(2), Is.EqualTo(0));
            Assert.That(rules.Round.StateOf(2), Is.EqualTo(ChoiceState.Done));
            Assert.That(rules.Round.PlacesFilled, Is.EqualTo(1));
            Assert.That(rules.Round.Choices[rules.Round.Answer].Item.ToString(), Is.EqualTo("a"));
            Assert.That(rules.Judge(ActivityAnswer.Place(2)), Is.EqualTo(AnswerVerdict.Neutral), "a tile in a window again");
            Assert.That(rules.Judge(ActivityAnswer.Place(3)), Is.EqualTo(AnswerVerdict.Right));
            Assert.That(rules.Judge(ActivityAnswer.Place(0)), Is.EqualTo(AnswerVerdict.Right));
            Assert.That(rules.Judge(ActivityAnswer.Place(1)), Is.EqualTo(AnswerVerdict.Done));
            Assert.That(SequenceLines.Of(rules.Response), Is.EqualTo(new[] { "ro-line-build-liftoff", "ro-slow-tata", "ro-word-tata" }));
            Assert.That(rules.Round.Answer, Is.EqualTo(-1));
            Assert.That(rules.Round.PlacedOn(1), Is.EqualTo(3));
            Assert.That(() => rules.Judge(ActivityAnswer.Drop(0)), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => rules.Judge(ActivityAnswer.Place(5)), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        /// <summary>
        /// Tests the instruction (the word heard), the hints (the word said slowly, then the sound after the
        /// last letter placed) and the answer shown (a tile with the next letter glows).
        /// </summary>
        [Test]
        public void TestHintFor_TheSlowWordThenTheNextSound()
        {
            IActivityRules rules = Tata();
            rules.Begin(new FixedRandom());

            Assert.That(SequenceLines.Of(rules.Instruction), Is.EqualTo(new[] { "ro-line-build", "ro-word-tata" }));
            Assert.That(SequenceLines.Of(rules.Prompt), Is.EqualTo(new[] { "ro-word-tata" }));
            ActivityHint slow = rules.HintFor(1, ActivityAnswer.Place(4));
            Assert.That(SequenceLines.Of(slow.Line), Is.EqualTo(new[] { "ro-line-build-hint-slow" }));
            Assert.That(SequenceLines.Of(rules.HintFor(2, ActivityAnswer.Place(4)).Line), Is.EqualTo(new[] { "ro-line-build-hint-say" }));
            rules.Judge(ActivityAnswer.Place(0));
            Assert.That(SequenceLines.Of(rules.HintFor(2, ActivityAnswer.Place(4)).Line), Is.EqualTo(new[] { "ro-line-build-hint-after" }));
            ActivityHint shown = rules.AnswerShown();
            Assert.That(shown.Cue, Is.EqualTo(HintCue.GlowAnswer));
            Assert.That(rules.Round.Choices[shown.Choice].Item.ToString(), Is.EqualTo("a"));
            Assert.That(SequenceLines.Of(shown.Line), Is.EqualTo(new[] { "ro-line-build-hint-glow" }));
            Assert.That(rules.Lines, Has.Member(slow.Line).And.Member(shown.Line).And.Member(rules.Instruction));
        }

        /// <summary>Tests that the English step speaks English: map with the extra t.</summary>
        [Test]
        public void TestRound_EnglishMap()
        {
            Curriculum curriculum = RealContent.Read();
            IActivityRules rules = ActivityRulesFactory.Create(RealContent.FirstStep(curriculum, "en-words-1", ActivityKind.BuildWord),
                curriculum, TestLearningRules.Default());
            rules.Begin(new FixedRandom());

            Assert.That(rules.Language, Is.EqualTo(Language.English));
            Assert.That(rules.Round.Choices.Select(choice => choice.Item.ToString()), Is.EqualTo(new[] { "m", "a", "p", "t" }));
            Assert.That(SequenceLines.Of(rules.Instruction), Is.EqualTo(new[] { "en-line-build", "en-word-map" }));
        }

        /// <summary>
        /// Tests the <c>BuildWord</c> step through <see cref="LessonRun.Answer"/>: a right tile is a part done, a
        /// wrong one gets the hint, a second shows the next letter, and the tile then placed counts as practice.
        /// </summary>
        [Test]
        public void TestAnswer_HintThenShowThroughTheRun()
        {
            Curriculum curriculum = RealContent.Read();
            LessonPlan plan = LessonPlan.Of(RealContent.Lesson(curriculum, "ro-words-1"), LanguageMix.Both,
                new PlayableSteps(new[] { ActivityKind.BuildWord }, curriculum), new Step[0], 1);
            LessonRun run = new LessonRun(plan, curriculum, TestLearningRules.Default(), new FixedRandom());
            List<AnswerChecked> answers = new List<AnswerChecked>();
            run.AnswerChecked += answers.Add;
            run.Start();

            Assert.That(run.Answer(ActivityAnswer.Place(0)).Outcome, Is.EqualTo(FeedbackOutcome.Right));
            Assert.That(run.Answer(ActivityAnswer.Place(4)).Outcome, Is.EqualTo(FeedbackOutcome.TryAgain));
            StepFeedback shown = run.Answer(ActivityAnswer.Place(2));
            Assert.That(shown.Outcome, Is.EqualTo(FeedbackOutcome.ShowAnswer));
            Assert.That(shown.Hint.Choice, Is.EqualTo(1));
            Assert.That(run.Answer(ActivityAnswer.Place(1)).Outcome, Is.EqualTo(FeedbackOutcome.Right));
            run.Answer(ActivityAnswer.Place(2));
            Assert.That(run.Answer(ActivityAnswer.Place(3)).Outcome, Is.EqualTo(FeedbackOutcome.Done));
            Assert.That(answers.ConvertAll(answer => answer.Result), Is.EqualTo(new[]
            {
                AnswerResult.Right, AnswerResult.Wrong, AnswerResult.Wrong, AnswerResult.Practised, AnswerResult.Right,
                AnswerResult.Right
            }));
        }

        private static IActivityRules Tata()
        {
            Curriculum curriculum = RealContent.Read();
            return ActivityRulesFactory.Create(RealContent.FirstStep(curriculum, "ro-words-1", ActivityKind.BuildWord), curriculum,
                TestLearningRules.Default());
        }
    }
}
