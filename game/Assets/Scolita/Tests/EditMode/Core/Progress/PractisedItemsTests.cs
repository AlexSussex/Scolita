using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Progress;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Progress
{
    /// <summary>Tests <see cref="PractisedItems"/> on the real content's steps.</summary>
    public sealed class PractisedItemsTests
    {
        /// <summary>
        /// Tests that <see cref="PractisedItems.Of"/> names the letter of the letter kinds in the step's language: the answer of
        /// <c>ListenAndPick</c>, the letter of <c>LetterHunt</c>, <c>TraceLetter</c> and <c>MeetLetter</c>, and the letter
        /// <c>FirstSound</c>'s word starts with.
        /// </summary>
        [Test]
        public void TestOf_TheLetterKinds()
        {
            Curriculum curriculum = RealContent.Read();

            Assert.That(Items(curriculum, "ro-t", ActivityKind.ListenAndPick), Is.EqualTo(new[] { "letter t (ro)" }));
            Assert.That(Items(curriculum, "en-m", ActivityKind.LetterHunt), Is.EqualTo(new[] { "letter m (en)" }));
            Assert.That(Items(curriculum, "ro-m", ActivityKind.TraceLetter), Is.EqualTo(new[] { "letter m (ro)" }));
            Assert.That(Items(curriculum, "ro-m", ActivityKind.MeetLetter), Is.EqualTo(new[] { "letter m (ro)" }));
            Assert.That(Items(curriculum, "en-t", ActivityKind.FirstSound), Is.EqualTo(new[] { "letter t (en)" }));
        }

        /// <summary>Tests that <see cref="PractisedItems.Of"/> names <c>TwoSounds</c>' letter in both languages.</summary>
        [Test]
        public void TestOf_TwoSoundsInBothLanguages()
        {
            Curriculum curriculum = RealContent.Read();

            Assert.That(Items(curriculum, "en-a", ActivityKind.TwoSounds), Is.EqualTo(new[] { "letter a (ro)", "letter a (en)" }));
        }

        /// <summary>Tests that <see cref="PractisedItems.Of"/> names the word of the word kinds, and every word of <c>WordTwins</c>.</summary>
        [Test]
        public void TestOf_TheWordKinds()
        {
            Curriculum curriculum = RealContent.Read();
            Step twins = curriculum.AllLessons.SelectMany(lesson => lesson.Steps).First(step => step.Kind == ActivityKind.WordTwins);
            Step build = curriculum.AllLessons.SelectMany(lesson => lesson.Steps).First(step => step.Kind == ActivityKind.BuildWord);

            Assert.That(PractisedItems.Of(build, curriculum).Select(item => item.Id), Is.EqualTo(new[] { build.Words[0] }));
            Assert.That(PractisedItems.Of(build, curriculum)[0].Kind, Is.EqualTo(ItemKind.Word));
            Assert.That(PractisedItems.Of(twins, curriculum).Select(item => item.Id), Is.EqualTo(twins.Words));
        }

        /// <summary>Tests that <see cref="PractisedItems.IsAnswered"/> is false only for <c>MeetLetter</c>, a first contact.</summary>
        [Test]
        public void TestIsAnswered_AllButMeetLetter()
        {
            Assert.That(PractisedItems.IsAnswered(ActivityKind.MeetLetter), Is.False);
            Assert.That(PractisedItems.IsAnswered(ActivityKind.ListenAndPick), Is.True);
            Assert.That(PractisedItems.IsAnswered(ActivityKind.TraceWord), Is.True);
        }

        private static string[] Items(Curriculum curriculum, string lessonId, ActivityKind kind)
        {
            return PractisedItems.Of(RealContent.FirstStep(curriculum, lessonId, kind), curriculum)
                .Select(item => item.ToString())
                .ToArray();
        }
    }
}
