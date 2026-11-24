using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Activities;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;
using Scolita.Core.Randomness;
using Scolita.Core.Voice;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Activities
{
    /// <summary>
    /// Tests <see cref="LetterHuntRules"/> with planet 1's real items (<c>ro-m</c>: m among a), with the
    /// default 10 stars, 4 of them with m; <see cref="FixedRandom"/> keeps the stars in the order they are
    /// made: m, m, M, M, then a, A, a, A, a, A.
    /// </summary>
    public sealed class LetterHuntRulesTests
    {
        /// <summary>
        /// Tests that <see cref="LetterHuntRules.Begin"/> sets up the stars: 4 with m (2 of them capitals), the
        /// other 6 with a in both cases, all Romanian and open, shuffled by the seed.
        /// </summary>
        [Test]
        public void TestBegin_TheStars()
        {
            IActivityRules rules = RomanianM();

            for (ulong seed = 0; seed < 10; seed++)
            {
                rules.Begin(new SeededRandom(seed));
                IReadOnlyList<ActivityChoice> stars = rules.Round.Choices;

                Assert.That(stars.Select(star => star.Text), Is.EquivalentTo(new[] { "m", "m", "M", "M", "a", "A", "a", "A", "a", "A" }));
                Assert.That(stars.Where(star => star.Item.ToString() == "m").Count(star => star.IsCapital), Is.EqualTo(2));
                Assert.That(stars.All(star => star.Language == Language.Romanian), Is.True);
                Assert.That(Enumerable.Range(0, stars.Count).All(index => rules.Round.StateOf(index) == ChoiceState.Open), Is.True);
                Assert.That(rules.Round.AnswerItem.ToString(), Is.EqualTo("m"));
            }
        }

        /// <summary>
        /// Tests that <see cref="LetterHuntRules.Judge"/> takes each star with m as one part, said with its sound;
        /// a popped star again is not an answer, a star with a is a miss, and the last m finishes the step.
        /// </summary>
        [Test]
        public void TestJudge_EachStarIsAPart()
        {
            IActivityRules rules = RomanianM();
            rules.Begin(new FixedRandom());

            AnswerVerdict first = rules.Judge(ActivityAnswer.Tap(0));
            VoiceSequence said = rules.Response;
            AnswerVerdict again = rules.Judge(ActivityAnswer.Tap(0));
            AnswerVerdict wrong = rules.Judge(ActivityAnswer.Tap(4));
            AnswerVerdict second = rules.Judge(ActivityAnswer.Tap(1));
            AnswerVerdict capital = rules.Judge(ActivityAnswer.Tap(2));
            AnswerVerdict last = rules.Judge(ActivityAnswer.Tap(3));

            Assert.That(first, Is.EqualTo(AnswerVerdict.Right));
            Assert.That(SequenceLines.Of(said), Is.EqualTo(new[] { "ro-sound-m" }));
            Assert.That(again, Is.EqualTo(AnswerVerdict.Neutral));
            Assert.That(wrong, Is.EqualTo(AnswerVerdict.Wrong));
            Assert.That(second, Is.EqualTo(AnswerVerdict.Right));
            Assert.That(capital, Is.EqualTo(AnswerVerdict.Right));
            Assert.That(last, Is.EqualTo(AnswerVerdict.Done));
            Assert.That(rules.Round.StateOf(0), Is.EqualTo(ChoiceState.Done));
            Assert.That(rules.Round.StateOf(4), Is.EqualTo(ChoiceState.Open));
        }

        /// <summary>Tests that <see cref="LetterHuntRules.Judge"/> refuses a drop and a star that isn't there.</summary>
        [Test]
        public void TestJudge_OnlyTapsOnItsStars()
        {
            IActivityRules rules = RomanianM();
            rules.Begin(new FixedRandom());

            Assert.That(() => rules.Judge(ActivityAnswer.Drop(0)), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => rules.Judge(ActivityAnswer.Tap(10)), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        /// <summary>
        /// Tests that <see cref="LetterHuntRules.Instruction"/> asks for the stars with the sound /m/ and their
        /// capitals, and that <see cref="LetterHuntRules.Prompt"/> asks for more once one is found.
        /// </summary>
        [Test]
        public void TestPrompt_FindThenMore()
        {
            IActivityRules rules = RomanianM();
            rules.Begin(new FixedRandom());

            string[] instruction = SequenceLines.Of(rules.Instruction);
            string[] before = SequenceLines.Of(rules.Prompt);
            rules.Judge(ActivityAnswer.Tap(0));
            string[] after = SequenceLines.Of(rules.Prompt);

            Assert.That(instruction, Is.EqualTo(new[] { "ro-line-hunt", "ro-line-hunt-capitals" }));
            Assert.That(rules.Instruction.Items[0].TryGetFill(VoiceSlot.X, out VoiceFill sound), Is.True);
            Assert.That(sound.Line.ToString(), Is.EqualTo("ro-sound-m"));
            Assert.That(before, Is.EqualTo(new[] { "ro-line-hunt" }));
            Assert.That(after, Is.EqualTo(new[] { "ro-line-hunt-hint-more" }));
        }

        /// <summary>
        /// Tests that <see cref="LetterHuntRules.HintFor"/> twinkles the stars left, saying the stars with m are
        /// twinkling, then that it is another letter; with only capitals left, it says first that the capital
        /// looks different.
        /// </summary>
        [Test]
        public void TestHintFor_TwinklesTheStarsLeft()
        {
            IActivityRules rules = RomanianM();
            rules.Begin(new FixedRandom());
            ActivityAnswer miss = ActivityAnswer.Tap(5);

            ActivityHint first = rules.HintFor(1, miss);
            ActivityHint second = rules.HintFor(2, miss);
            rules.Judge(ActivityAnswer.Tap(0));
            rules.Judge(ActivityAnswer.Tap(1));
            ActivityHint capitalsLeft = rules.HintFor(1, miss);

            Assert.That(first.Cue, Is.EqualTo(HintCue.TwinkleAnswers));
            Assert.That(SequenceLines.Of(first.Line), Is.EqualTo(new[] { "ro-line-hunt-hint-twinkle" }));
            Assert.That(second.Cue, Is.EqualTo(HintCue.TwinkleAnswers));
            Assert.That(SequenceLines.Of(second.Line), Is.EqualTo(new[] { "ro-line-hunt-hint-not" }));
            Assert.That(SequenceLines.Of(capitalsLeft.Line), Is.EqualTo(new[] { "ro-line-hunt-hint-capital", "ro-line-hunt-hint-twinkle" }));
            Assert.That(() => rules.HintFor(0, miss), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        /// <summary>Tests that <see cref="LetterHuntRules.AnswerShown"/> glows the first star left with m and says which letter it is.</summary>
        [Test]
        public void TestAnswerShown_GlowsAStarLeft()
        {
            IActivityRules rules = RomanianM();
            rules.Begin(new FixedRandom());
            rules.Judge(ActivityAnswer.Tap(0));

            ActivityHint shown = rules.AnswerShown();

            Assert.That(shown.Cue, Is.EqualTo(HintCue.GlowAnswer));
            Assert.That(shown.Choice, Is.EqualTo(1));
            Assert.That(SequenceLines.Of(shown.Line), Is.EqualTo(new[] { "ro-line-pick-hint-show" }));
        }

        /// <summary>
        /// Tests that the <see cref="LetterHuntRules"/> constructor takes its numbers from the learning rules,
        /// spreads the wrong stars over the other letters, and needs a wrong letter.
        /// </summary>
        [Test]
        public void TestLetterHuntRules_TheTuningAndTheLetters()
        {
            Curriculum curriculum = RealContent.Read();
            Letter[] letters = { Letter(curriculum, "t"), Letter(curriculum, "a"), Letter(curriculum, "m") };

            LetterHuntRules rules = new LetterHuntRules(letters, Language.English, new LetterHuntTuning(8, 3, 40f));

            Assert.That(rules.Round.Choices.Count, Is.EqualTo(8));
            Assert.That(rules.Round.Choices.Select(star => star.Text), Is.EqualTo(new[] { "t", "t", "T", "a", "m", "A", "M", "a" }));
            Assert.That(rules.Round.Choices.All(star => star.Language == Language.English), Is.True);
            Assert.That(() => new LetterHuntRules(new[] { letters[0] }, Language.English, new LetterHuntTuning(8, 3, 40f)),
                Throws.ArgumentException);
        }

        /// <summary>
        /// Tests the <c>LetterHunt</c> step through <see cref="LessonRun"/>: a miss twinkles the stars, the
        /// second shows one, and the star found with it shown counts as practice.
        /// </summary>
        [Test]
        public void TestLessonRun_HintThenShow()
        {
            Curriculum curriculum = RealContent.Read();
            LessonPlan plan = LessonPlan.Of(RealContent.Lesson(curriculum, "ro-m"), LanguageMix.Both,
                new[] { ActivityKind.LetterHunt }, new Step[0], 1);
            LessonRun run = new LessonRun(plan, curriculum, TestLearningRules.Default(), new FixedRandom());
            List<AnswerChecked> answers = new List<AnswerChecked>();
            run.AnswerChecked += answers.Add;
            run.Start();

            StepFeedback right = run.Answer(ActivityAnswer.Tap(0));
            StepFeedback miss = run.Answer(ActivityAnswer.Tap(4));
            StepFeedback shown = run.Answer(ActivityAnswer.Tap(5));
            run.Answer(ActivityAnswer.Tap(1));
            run.Answer(ActivityAnswer.Tap(2));
            StepFeedback done = run.Answer(ActivityAnswer.Tap(3));

            Assert.That(right.Outcome, Is.EqualTo(FeedbackOutcome.Right));
            Assert.That(SequenceLines.Of(right.First), Is.EqualTo(new[] { "ro-sound-m" }));
            Assert.That(miss.Outcome, Is.EqualTo(FeedbackOutcome.TryAgain));
            Assert.That(miss.Hint.Cue, Is.EqualTo(HintCue.TwinkleAnswers));
            Assert.That(shown.Outcome, Is.EqualTo(FeedbackOutcome.ShowAnswer));
            Assert.That(shown.Hint.Choice, Is.EqualTo(1));
            Assert.That(done.Outcome, Is.EqualTo(FeedbackOutcome.Done));
            Assert.That(SequenceLines.Of(done.Then)[0], Does.StartWith("ro-line-praise-"));
            Assert.That(answers.Select(answer => answer.Result), Is.EqualTo(new[]
            {
                AnswerResult.Right, AnswerResult.Wrong, AnswerResult.Wrong, AnswerResult.Practised, AnswerResult.Right,
                AnswerResult.Right
            }));
        }

        private static IActivityRules RomanianM()
        {
            Curriculum curriculum = RealContent.Read();
            return ActivityRulesFactory.Create(RealContent.FirstStep(curriculum, "ro-m", ActivityKind.LetterHunt), curriculum,
                TestLearningRules.Default());
        }

        private static Letter Letter(Curriculum curriculum, string id)
        {
            return curriculum.Letters.Get(ContentId.Create(id));
        }
    }
}
