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
    /// Tests <see cref="FirstSoundRules"/> with planet 1's real items (<c>ro-m</c>: the picture of mama with
    /// m and a; <c>ro-p</c>: pat with p, a, m and t); <see cref="FixedRandom"/> keeps the letters in the
    /// step's order.
    /// </summary>
    public sealed class FirstSoundRulesTests
    {
        /// <summary>
        /// Tests that the round shows the step's letters, shuffled, and the word's picture as its subject, the
        /// answer being the letter the word starts with.
        /// </summary>
        [Test]
        public void TestBegin_TheLettersAndThePicture()
        {
            IActivityRules rules = Step("ro-p");

            for (ulong seed = 0; seed < 20; seed++)
            {
                rules.Begin(new SeededRandom(seed));

                Assert.That(rules.Round.Choices.Select(choice => choice.Item.ToString()), Is.EquivalentTo(new[] { "p", "a", "m", "t" }));
                Assert.That(rules.Round.Choices[rules.Round.Answer].Item.ToString(), Is.EqualTo("p"));
            }

            Assert.That(rules.Round.Subject.Item.ToString(), Is.EqualTo("ro-pat"));
            Assert.That(rules.Round.Subject.Look, Is.EqualTo(ActivityChoiceLook.Picture));
            Assert.That(rules.Round.Subject.Picture.ToString(), Is.EqualTo("bed"));
            Assert.That(rules.Round.Choices.All(choice => choice.Language == Language.Romanian), Is.True);
        }

        /// <summary>Tests that <see cref="FirstSoundRules.Judge"/> finishes the step on the first letter and misses on another.</summary>
        [Test]
        public void TestJudge_TheFirstLetterIsDone()
        {
            IActivityRules rules = Step("ro-m");
            rules.Begin(new FixedRandom());

            Assert.That(rules.Judge(ActivityAnswer.Tap(1)), Is.EqualTo(AnswerVerdict.Wrong));
            Assert.That(rules.Judge(ActivityAnswer.Tap(0)), Is.EqualTo(AnswerVerdict.Done));
            Assert.That(rules.Response, Is.Null);
            Assert.That(() => rules.Judge(ActivityAnswer.Tap(2)), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => rules.Judge(ActivityAnswer.Drop(0)), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        /// <summary>
        /// Tests that <see cref="FirstSoundRules.Instruction"/> asks for the first sound of the word said, and
        /// that <see cref="FirstSoundRules.Prompt"/> says the word again.
        /// </summary>
        [Test]
        public void TestInstruction_SaysTheWord()
        {
            IActivityRules rules = Step("ro-m");

            Assert.That(SequenceLines.Of(rules.Instruction),
                Is.EqualTo(new[] { "ro-line-first-sound", "ro-word-mama", "ro-line-first-sound-tap" }));
            Assert.That(SequenceLines.Of(rules.Prompt), Is.EqualTo(new[] { "ro-word-mama" }));
        }

        /// <summary>
        /// Tests that <see cref="FirstSoundRules.HintFor"/> fades the letter missed and says the word with its
        /// first sound stretched, then which sound to look for; and that <see cref="FirstSoundRules.AnswerShown"/>
        /// glows the answer and says which letter it is.
        /// </summary>
        [Test]
        public void TestHintFor_TheStretchedWordThenTheSound()
        {
            IActivityRules rules = Step("ro-m");
            rules.Begin(new FixedRandom());
            ActivityAnswer miss = ActivityAnswer.Tap(1);

            ActivityHint first = rules.HintFor(1, miss);
            ActivityHint second = rules.HintFor(3, miss);
            ActivityHint shown = rules.AnswerShown();

            Assert.That(SequenceLines.Of(first.Line), Is.EqualTo(new[] { "ro-line-first-hint-stretch" }));
            Assert.That(first.Line.Items[0].TryGetFill(VoiceSlot.Stretched, out VoiceFill stretched), Is.True);
            Assert.That(stretched.Line.ToString(), Is.EqualTo("ro-stretched-mama"));
            Assert.That(stretched.Text, Is.EqualTo("mmmama"));
            Assert.That(first.Cue, Is.EqualTo(HintCue.FadeChoice));
            Assert.That(first.Choice, Is.EqualTo(1));
            Assert.That(SequenceLines.Of(second.Line), Is.EqualTo(new[] { "ro-line-first-hint-which", "ro-sound-m" }));
            Assert.That(shown.Cue, Is.EqualTo(HintCue.GlowAnswer));
            Assert.That(shown.Choice, Is.EqualTo(0));
            Assert.That(SequenceLines.Of(shown.Line), Is.EqualTo(new[] { "ro-line-pick-hint-show" }));
            Assert.That(() => rules.HintFor(0, miss), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        /// <summary>
        /// Tests that <see cref="FirstSoundRules.FirstLetterOf"/> finds the letter a word starts with, the
        /// longest when several match (a group such as ch before c), and refuses a word that starts with none.
        /// </summary>
        [Test]
        public void TestFirstLetterOf_TheLongestThatMatches()
        {
            Curriculum curriculum = RealContent.Read();
            Word pat = curriculum.Words.Get(ContentId.Create("ro-pat"));
            Word chip = new Word(ContentId.Create("en-chip"), Language.English, "chip", new ContentId[0], new[] { "chip" },
                default, true, default, default, false, default);
            Letter c = new Letter(ContentId.Create("c"), "c", "C", LetterGroup.SoundDiffers, default, false, default);
            Letter ch = new Letter(ContentId.Create("ch"), "ch", "Ch", LetterGroup.EnglishOnly, default, false, default);
            Letter[] planetOne = { Letter(curriculum, "a"), Letter(curriculum, "p"), Letter(curriculum, "t") };

            Assert.That(FirstSoundRules.FirstLetterOf(pat, planetOne).Id.ToString(), Is.EqualTo("p"));
            Assert.That(FirstSoundRules.FirstLetterOf(chip, new[] { c, ch }).Id.ToString(), Is.EqualTo("ch"));
            Assert.That(() => FirstSoundRules.FirstLetterOf(chip, planetOne), Throws.ArgumentException);
        }

        /// <summary>
        /// Tests the <c>FirstSound</c> step through <see cref="LessonRun"/>: a miss says the stretched word, the
        /// second shows the answer, which then counts as practice.
        /// </summary>
        [Test]
        public void TestLessonRun_HintThenShow()
        {
            Curriculum curriculum = RealContent.Read();
            LessonPlan plan = LessonPlan.Of(RealContent.Lesson(curriculum, "ro-m"), LanguageMix.Both,
                new[] { ActivityKind.FirstSound }, new Step[0], 1);
            LessonRun run = new LessonRun(plan, curriculum, TestLearningRules.Default(), new FixedRandom());
            List<AnswerChecked> answers = new List<AnswerChecked>();
            run.AnswerChecked += answers.Add;
            run.Start();

            StepFeedback miss = run.Answer(ActivityAnswer.Tap(1));
            StepFeedback shown = run.Answer(ActivityAnswer.Tap(1));
            StepFeedback done = run.Answer(ActivityAnswer.Tap(0));

            Assert.That(miss.Outcome, Is.EqualTo(FeedbackOutcome.TryAgain));
            Assert.That(SequenceLines.Of(miss.First), Is.EqualTo(new[] { "ro-line-first-hint-stretch" }));
            Assert.That(shown.Outcome, Is.EqualTo(FeedbackOutcome.ShowAnswer));
            Assert.That(shown.Hint.Choice, Is.EqualTo(0));
            Assert.That(done.Outcome, Is.EqualTo(FeedbackOutcome.Done));
            Assert.That(answers.Select(answer => answer.Result), Is.EqualTo(new[]
            {
                AnswerResult.Wrong, AnswerResult.Wrong, AnswerResult.Practised
            }));
        }

        private static IActivityRules Step(string lessonId)
        {
            Curriculum curriculum = RealContent.Read();
            return ActivityRulesFactory.Create(RealContent.FirstStep(curriculum, lessonId, ActivityKind.FirstSound), curriculum,
                TestLearningRules.Default());
        }

        private static Letter Letter(Curriculum curriculum, string id)
        {
            return curriculum.Letters.Get(ContentId.Create(id));
        }
    }
}
