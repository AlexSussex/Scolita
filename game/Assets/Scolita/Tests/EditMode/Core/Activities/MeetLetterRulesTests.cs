using System;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Activities;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Tracing;
using Scolita.Core.Voice;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Activities
{
    /// <summary>Tests <see cref="MeetLetterRules"/> with planet 1's real items (<c>ro-a</c>: the letter a and avion).</summary>
    public sealed class MeetLetterRulesTests
    {
        /// <summary>
        /// Tests that <see cref="MeetLetterRules.Instruction"/> introduces the letter, says its sound and
        /// the word, and asks for a tap on the letter.
        /// </summary>
        [Test]
        public void TestInstruction_SaysTheSoundAndTheWord()
        {
            IActivityRules rules = RomanianA();

            VoiceSequence instruction = rules.Instruction;

            Assert.That(SequenceLines.Of(instruction), Is.EqualTo(new[] { "ro-line-meet-new", "ro-line-meet-listen", "ro-line-meet-tap-letter" }));
            Assert.That(instruction.Items[1].TryGetFill(VoiceSlot.X, out VoiceFill sound), Is.True);
            Assert.That(sound.Line.ToString(), Is.EqualTo("ro-sound-a"));
            Assert.That(instruction.Items[1].TryGetFill(VoiceSlot.Word, out VoiceFill word), Is.True);
            Assert.That(word.Line.ToString(), Is.EqualTo("ro-word-avion"));
            Assert.That(word.Text, Is.EqualTo("avion"));
        }

        /// <summary>
        /// Tests that <see cref="MeetLetterRules.Judge"/> takes the letter, then the picture: the letter
        /// says its sound and asks for the picture, the picture says its word and finishes the step.
        /// </summary>
        [Test]
        public void TestJudge_LetterThenPicture()
        {
            IActivityRules rules = RomanianA();
            rules.Begin(new FixedRandom());

            AnswerVerdict letter = rules.Judge(ActivityAnswer.Tap(MeetLetterRules.LetterChoice));
            string[] afterLetter = SequenceLines.Of(rules.Response);
            string[] prompt = SequenceLines.Of(rules.Prompt);
            AnswerVerdict picture = rules.Judge(ActivityAnswer.Tap(MeetLetterRules.PictureChoice));

            Assert.That(letter, Is.EqualTo(AnswerVerdict.Right));
            Assert.That(afterLetter, Is.EqualTo(new[] { "ro-sound-a", "ro-line-meet-tap-picture" }));
            Assert.That(prompt, Is.EqualTo(new[] { "ro-line-meet-tap-picture" }));
            Assert.That(picture, Is.EqualTo(AnswerVerdict.Done));
            Assert.That(SequenceLines.Of(rules.Response), Is.EqualTo(new[] { "ro-word-avion" }));
        }

        /// <summary>
        /// Tests that <see cref="MeetLetterRules.Judge"/> takes the picture first without a miss (it says
        /// the word and asks for the letter), then finishes on the letter.
        /// </summary>
        [Test]
        public void TestJudge_PictureFirstIsNeutral()
        {
            IActivityRules rules = RomanianA();
            rules.Begin(new FixedRandom());

            AnswerVerdict picture = rules.Judge(ActivityAnswer.Tap(MeetLetterRules.PictureChoice));
            string[] afterPicture = SequenceLines.Of(rules.Response);
            AnswerVerdict letter = rules.Judge(ActivityAnswer.Tap(MeetLetterRules.LetterChoice));

            Assert.That(picture, Is.EqualTo(AnswerVerdict.Neutral));
            Assert.That(afterPicture, Is.EqualTo(new[] { "ro-word-avion", "ro-line-meet-tap-letter" }));
            Assert.That(letter, Is.EqualTo(AnswerVerdict.Done));
            Assert.That(SequenceLines.Of(rules.Response), Is.EqualTo(new[] { "ro-sound-a" }));
        }

        /// <summary>Tests that <see cref="MeetLetterRules.Judge"/> takes the letter tapped again as neutral: it just says its sound.</summary>
        [Test]
        public void TestJudge_LetterAgainIsNeutral()
        {
            IActivityRules rules = RomanianA();
            rules.Begin(new FixedRandom());
            rules.Judge(ActivityAnswer.Tap(MeetLetterRules.LetterChoice));

            AnswerVerdict again = rules.Judge(ActivityAnswer.Tap(MeetLetterRules.LetterChoice));

            Assert.That(again, Is.EqualTo(AnswerVerdict.Neutral));
            Assert.That(SequenceLines.Of(rules.Response), Is.EqualTo(new[] { "ro-sound-a" }));
        }

        /// <summary>Tests that <see cref="MeetLetterRules.Begin"/> forgets the taps of an earlier start.</summary>
        [Test]
        public void TestBegin_ForgetsEarlierTaps()
        {
            IActivityRules rules = RomanianA();
            rules.Begin(new FixedRandom());
            rules.Judge(ActivityAnswer.Tap(MeetLetterRules.LetterChoice));

            rules.Begin(new FixedRandom());

            Assert.That(SequenceLines.Of(rules.Prompt), Is.EqualTo(new[] { "ro-line-meet-tap-letter" }));
            Assert.That(rules.Judge(ActivityAnswer.Tap(MeetLetterRules.LetterChoice)), Is.EqualTo(AnswerVerdict.Right));
        }

        /// <summary>
        /// Tests that <see cref="MeetLetterRules.Round"/> shows the letter (both cases), the picture and the "de mână"
        /// button, in Romanian, with no single answer.
        /// </summary>
        [Test]
        public void TestRound_TheLetterAndThePicture()
        {
            IActivityRules rules = RomanianA();

            ActivityRound round = rules.Round;

            Assert.That(round.Choices.Select(choice => choice.Look),
                Is.EqualTo(new[] { ActivityChoiceLook.Letter, ActivityChoiceLook.Picture, ActivityChoiceLook.Control }));
            Assert.That(round.Choices[0].Text, Is.EqualTo("a"));
            Assert.That(round.Choices[0].UpperText, Is.EqualTo("A"));
            Assert.That(round.Choices[1].Picture.ToString(), Is.EqualTo("plane"));
            Assert.That(round.Answer, Is.EqualTo(-1));
            Assert.That(rules.Language, Is.EqualTo(Language.Romanian));
        }

        /// <summary>Tests that <see cref="MeetLetterRules.HintFor"/> and <see cref="MeetLetterRules.AnswerShown"/> fail: there are no misses.</summary>
        [Test]
        public void TestHintFor_NoHints()
        {
            IActivityRules rules = RomanianA();

            Assert.That(() => rules.HintFor(1, ActivityAnswer.Tap(0)), Throws.InstanceOf<InvalidOperationException>());
            Assert.That(() => rules.AnswerShown(), Throws.InstanceOf<InvalidOperationException>());
            Assert.That(() => rules.Judge(ActivityAnswer.Tap(3)), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => rules.Judge(ActivityAnswer.Drop(MeetLetterRules.LetterChoice)), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        /// <summary>
        /// Tests that <see cref="MeetLetterRules.Judge"/> takes a tap on the "de mână" button as neutral, saying „Uite cum
        /// se scrie litera de mână.”, with the handwritten a on the round.
        /// </summary>
        [Test]
        public void TestJudge_TheByHandButton()
        {
            IActivityRules rules = RomanianA();
            rules.Begin(new FixedRandom());

            Assert.That(rules.Judge(ActivityAnswer.Tap(MeetLetterRules.ByHandChoice)), Is.EqualTo(AnswerVerdict.Neutral));
            Assert.That(SequenceLines.Of(rules.Response), Is.EqualTo(new[] { "ro-line-meet-by-hand" }));
            Assert.That(rules.Round.Handwriting.Form, Is.EqualTo(LetterForm.Handwritten));
            Assert.That(rules.Round.Handwriting.Strokes.Count, Is.EqualTo(3));
            Assert.That(rules.Judge(ActivityAnswer.Tap(MeetLetterRules.LetterChoice)), Is.EqualTo(AnswerVerdict.Right));
        }

        private static IActivityRules RomanianA()
        {
            Curriculum curriculum = RealContent.Read();
            return ActivityRulesFactory.Create(RealContent.FirstStep(curriculum, "ro-a", ActivityKind.MeetLetter), curriculum, TestLearningRules.Default());
        }
    }
}
