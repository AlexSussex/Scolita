using System.Collections.Generic;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;
using Scolita.Core.Randomness;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Lessons
{
    /// <summary>Tests <see cref="PraisePicker"/>.</summary>
    public sealed class PraisePickerTests
    {
        /// <summary>
        /// Tests that <see cref="PraisePicker.Next"/> on the real content never repeats one of the last
        /// three praise lines, says each of them in time, and stays in the language asked for.
        /// </summary>
        [Test]
        public void TestNext_NeverOneOfTheLastThree()
        {
            Curriculum curriculum = RealContent.Read();
            PraisePicker picker = new PraisePicker(curriculum, TestLearningRules.Default(), new SeededRandom(42));
            List<string> said = new List<string>();

            for (int turn = 0; turn < 300; turn++)
            {
                said.Add(SequenceLines.Of(picker.Next(Language.English))[0]);
            }

            for (int turn = 1; turn < said.Count; turn++)
            {
                for (int back = 1; back <= 3 && turn - back >= 0; back++)
                {
                    Assert.That(said[turn], Is.Not.EqualTo(said[turn - back]), "turn " + turn);
                }
            }

            Assert.That(said, Is.All.StartWith("en-line-praise-"));
            Assert.That(new HashSet<string>(said).Count, Is.EqualTo(LessonLines.Praise(curriculum, Language.English).Count));
        }

        /// <summary>Tests that <see cref="PraisePicker.Next"/> with two lines and a wider window alternates them.</summary>
        [Test]
        public void TestNext_TwoLinesAlternate()
        {
            Curriculum curriculum = ContentCsvs.Read(ContentCsvs.Valid());
            PraisePicker picker = new PraisePicker(curriculum, TestLearningRules.Default(), new SeededRandom(1));

            string first = SequenceLines.Of(picker.Next(Language.Romanian))[0];
            string second = SequenceLines.Of(picker.Next(Language.Romanian))[0];
            string third = SequenceLines.Of(picker.Next(Language.Romanian))[0];

            Assert.That(second, Is.Not.EqualTo(first));
            Assert.That(third, Is.EqualTo(first));
        }

        /// <summary>Tests that <see cref="PraisePicker.Next"/> fails loudly for a language with no praise lines.</summary>
        [Test]
        public void TestNext_NoPraiseLines()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            ContentCsvs.ReplaceRow(csvs, ContentFileNames.VoiceLines, "en-line-praise-1,", "en-line-unused-1,en,line,Unused.,,,");
            PraisePicker picker = new PraisePicker(ContentCsvs.Read(csvs), TestLearningRules.Default(), new SeededRandom(1));

            Assert.That(() => picker.Next(Language.English), Throws.InvalidOperationException);
        }
    }
}
