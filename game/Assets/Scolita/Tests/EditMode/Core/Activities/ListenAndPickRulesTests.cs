using System;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Activities;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Randomness;
using Scolita.Core.Voice;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Activities
{
    /// <summary>
    /// Tests <see cref="ListenAndPickRules"/> with planet 1's real items (<c>ro-t</c>: t among a and m;
    /// the pictures of mama and tata).
    /// </summary>
    public sealed class ListenAndPickRulesTests
    {
        /// <summary>
        /// Tests that <see cref="ListenAndPickRules.Begin"/> shuffles the step's letters and still knows
        /// the answer: the step's first letter.
        /// </summary>
        [Test]
        public void TestBegin_ShufflesAndKeepsTheAnswer()
        {
            IActivityRules rules = RomanianT();

            for (ulong seed = 0; seed < 20; seed++)
            {
                rules.Begin(new SeededRandom(seed));

                Assert.That(rules.Round.Choices.Select(choice => choice.Item.ToString()), Is.EquivalentTo(new[] { "t", "a", "m" }));
                Assert.That(rules.Round.Choices[rules.Round.Answer].Item.ToString(), Is.EqualTo("t"));
            }
        }

        /// <summary>Tests that <see cref="ListenAndPickRules.Judge"/> finishes the step on the answer and misses on another letter.</summary>
        [Test]
        public void TestJudge_TheAnswerIsDone()
        {
            IActivityRules rules = RomanianT();
            rules.Begin(new FixedRandom());
            int other = (rules.Round.Answer + 1) % rules.Round.Choices.Count;

            Assert.That(rules.Judge(ActivityAnswer.Tap(other)), Is.EqualTo(AnswerVerdict.Wrong));
            Assert.That(rules.Judge(ActivityAnswer.Tap(rules.Round.Answer)), Is.EqualTo(AnswerVerdict.Done));
            Assert.That(rules.Response, Is.Null);
            Assert.That(() => rules.Judge(ActivityAnswer.Tap(3)), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        /// <summary>
        /// Tests that <see cref="ListenAndPickRules.Instruction"/> and <see cref="ListenAndPickRules.Prompt"/>
        /// ask for the letter by its sound.
        /// </summary>
        [Test]
        public void TestInstruction_AsksForTheSound()
        {
            IActivityRules rules = RomanianT();

            Assert.That(SequenceLines.Of(rules.Instruction), Is.EqualTo(new[] { "ro-line-pick-letter", "ro-line-pick-letter-x" }));
            Assert.That(rules.Instruction.Items[1].TryGetFill(VoiceSlot.X, out VoiceFill sound), Is.True);
            Assert.That(sound.Line.ToString(), Is.EqualTo("ro-sound-t"));
            Assert.That(sound.Text, Is.EqualTo("t"));
            Assert.That(SequenceLines.Of(rules.Prompt), Is.EqualTo(new[] { "ro-line-pick-letter-x" }));
        }

        /// <summary>
        /// Tests that <see cref="ListenAndPickRules.HintFor"/> fades the wrong letter and climbs its
        /// ladder: the sound again, then "try another", then "look at its shape", which stays.
        /// </summary>
        [Test]
        public void TestHintFor_TheLadder()
        {
            IActivityRules rules = RomanianT();
            rules.Begin(new FixedRandom());
            ActivityAnswer miss = ActivityAnswer.Tap(2);

            ActivityHint first = rules.HintFor(1, miss);
            ActivityHint second = rules.HintFor(2, miss);
            ActivityHint fifth = rules.HintFor(5, miss);

            Assert.That(SequenceLines.Of(first.Line), Is.EqualTo(new[] { "ro-line-listen-again", "ro-sound-t" }));
            Assert.That(first.Cue, Is.EqualTo(HintCue.FadeChoice));
            Assert.That(first.Choice, Is.EqualTo(2));
            Assert.That(SequenceLines.Of(second.Line), Is.EqualTo(new[] { "ro-line-pick-hint-other" }));
            Assert.That(SequenceLines.Of(fifth.Line), Is.EqualTo(new[] { "ro-line-pick-hint-shape" }));
            Assert.That(() => rules.HintFor(0, miss), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        /// <summary>Tests that <see cref="ListenAndPickRules.AnswerShown"/> glows the answer and says which letter it is.</summary>
        [Test]
        public void TestAnswerShown_GlowsTheAnswer()
        {
            IActivityRules rules = RomanianT();
            rules.Begin(new SeededRandom(7));

            ActivityHint shown = rules.AnswerShown();

            Assert.That(shown.Cue, Is.EqualTo(HintCue.GlowAnswer));
            Assert.That(shown.Choice, Is.EqualTo(rules.Round.Answer));
            Assert.That(SequenceLines.Of(shown.Line), Is.EqualTo(new[] { "ro-line-pick-hint-show" }));
            Assert.That(shown.Line.Items[0].TryGetFill(VoiceSlot.X, out VoiceFill sound), Is.True);
            Assert.That(sound.Line.ToString(), Is.EqualTo("ro-sound-t"));
        }

        /// <summary>
        /// Tests that <see cref="ListenAndPickRules.OfWords"/> asks for a picture by its word, with the
        /// word again as its hint and the answer shown with the word.
        /// </summary>
        [Test]
        public void TestOfWords_AsksForThePicture()
        {
            Curriculum curriculum = RealContent.Read();
            Word[] words = { curriculum.Words.Get(ContentId.Create("ro-mama")), curriculum.Words.Get(ContentId.Create("ro-tata")) };

            IActivityRules rules = ListenAndPickRules.OfWords(words, Language.Romanian);
            rules.Begin(new FixedRandom());

            Assert.That(SequenceLines.Of(rules.Instruction), Is.EqualTo(new[] { "ro-line-pick-picture", "ro-word-mama" }));
            Assert.That(rules.Round.Choices.Select(choice => choice.Look), Is.All.EqualTo(ActivityChoiceLook.Picture));
            Assert.That(rules.Round.Choices[rules.Round.Answer].Picture.ToString(), Is.EqualTo("mother"));
            Assert.That(SequenceLines.Of(rules.HintFor(2, ActivityAnswer.Tap(1)).Line), Is.EqualTo(new[] { "ro-line-listen-again", "ro-word-mama" }));
            Assert.That(SequenceLines.Of(rules.AnswerShown().Line), Is.EqualTo(new[] { "ro-line-show-answer", "ro-word-mama" }));
        }

        /// <summary>Tests that <see cref="ListenAndPickRules.OfLetters"/> refuses a single choice.</summary>
        [Test]
        public void TestOfLetters_NeedsTwoChoices()
        {
            Curriculum curriculum = RealContent.Read();
            Letter[] letters = { curriculum.Letters.Get(ContentId.Create("a")) };

            Assert.That(() => ListenAndPickRules.OfLetters(letters, Language.Romanian), Throws.ArgumentException);
        }

        private static IActivityRules RomanianT()
        {
            Curriculum curriculum = RealContent.Read();
            return ActivityRulesFactory.Create(RealContent.FirstStep(curriculum, "ro-t", ActivityKind.ListenAndPick), curriculum, TestLearningRules.Default());
        }
    }
}
