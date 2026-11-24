using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Lessons
{
    /// <summary>Tests <see cref="LessonOrder"/> on the real planet 1, with the kinds built so far.</summary>
    public sealed class LessonOrderTests
    {
        /// <summary>
        /// Tests that <see cref="LessonOrder.Playable"/> holds planet 1's lessons with something to play,
        /// in teaching order: the letters in both languages and the party, not the word and capital lessons.
        /// </summary>
        [Test]
        public void TestPlayable_InBothLanguages()
        {
            LessonOrder order = Order(LanguageMix.Both);

            Assert.That(order.Playable.Select(lesson => lesson.Id.ToString()), Is.EqualTo(new[]
            {
                "ro-a", "en-a", "ro-m", "en-m", "ro-t", "en-t", "ro-p", "en-p", "party-1"
            }));
        }

        /// <summary>Tests that <see cref="LessonOrder.Playable"/> leaves out the other language's lessons.</summary>
        [Test]
        public void TestPlayable_RomanianOnly()
        {
            LessonOrder order = Order(LanguageMix.RomanianOnly);

            Assert.That(order.Playable.Select(lesson => lesson.Id.ToString()), Is.EqualTo(new[]
            {
                "ro-a", "ro-m", "ro-t", "ro-p", "party-1"
            }));
        }

        /// <summary>
        /// Tests that <see cref="LessonOrder.After"/> gives the next playable lesson, skipping the ones
        /// with nothing to play, the first after the last, and the first for an unknown id.
        /// </summary>
        [Test]
        public void TestAfter_TheNextPlayableLesson()
        {
            LessonOrder order = Order(LanguageMix.Both);

            Assert.That(order.After(ContentId.Create("ro-a")).Id.ToString(), Is.EqualTo("en-a"));
            Assert.That(order.After(ContentId.Create("en-p")).Id.ToString(), Is.EqualTo("party-1"));
            Assert.That(order.After(ContentId.Create("ro-words-1")).Id.ToString(), Is.EqualTo("party-1"));
            Assert.That(order.After(ContentId.Create("party-1")).Id.ToString(), Is.EqualTo("ro-a"));
            Assert.That(order.After(ContentId.Create("no-such-lesson")).Id.ToString(), Is.EqualTo("ro-a"));
        }

        /// <summary>Tests that <see cref="LessonOrder.TryFind"/> finds a playable lesson only.</summary>
        [Test]
        public void TestTryFind_PlayableOnly()
        {
            LessonOrder order = Order(LanguageMix.Both);

            Assert.That(order.TryFind(ContentId.Create("ro-t"), out Lesson found), Is.True);
            Assert.That(found.Id.ToString(), Is.EqualTo("ro-t"));
            Assert.That(order.TryFind(ContentId.Create("capitals-1"), out Lesson _), Is.False);
        }

        /// <summary>Tests that <see cref="LessonOrder.After"/> gives nothing when no kind can be played.</summary>
        [Test]
        public void TestAfter_NothingToPlay()
        {
            Curriculum curriculum = RealContent.Read();

            LessonOrder order = new LessonOrder(curriculum.Domains[0], LanguageMix.Both, new PlayableSteps(new ActivityKind[0], curriculum));

            Assert.That(order.Playable, Is.Empty);
            Assert.That(order.After(ContentId.Create("ro-a")), Is.Null);
        }

        private static LessonOrder Order(LanguageMix mix)
        {
            Curriculum curriculum = RealContent.Read();
            return new LessonOrder(curriculum.Domains[0], mix, new PlayableSteps(new[] { ActivityKind.MeetLetter, ActivityKind.ListenAndPick }, curriculum));
        }
    }
}
