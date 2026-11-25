using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Rewards;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Rewards
{
    /// <summary>Tests <see cref="RewardBook"/>: the sticker scene's rules, the trail chosen, and a saved book brought into its rules.</summary>
    public sealed class RewardBookTests
    {
        /// <summary>Tests that <see cref="RewardBook.TryPlace"/> places a sticker only as many times as it was given.</summary>
        [Test]
        public void TestTryPlace_OnlyAsManyCopiesAsGiven()
        {
            ContentId guitar = ContentId.Create("guitar");
            RewardBook book = new RewardBook(0, null, 0, new[] { guitar, ContentId.Create("whale"), guitar }, null, null,
                default, null);
            RewardRules rules = RewardTestContent.Rules();

            bool first = book.TryPlace(guitar, 0.2f, 0.3f, 1f, rules);
            bool second = book.TryPlace(guitar, 0.4f, 0.5f, 1f, rules);
            bool third = book.TryPlace(guitar, 0.6f, 0.7f, 1f, rules);

            Assert.That(new[] { first, second, third }, Is.EqualTo(new[] { true, true, false }));
            Assert.That(book.CopiesLeft(guitar), Is.EqualTo(0));
            Assert.That(book.Placed.Count, Is.EqualTo(2));
            Assert.That(book.TryPlace(ContentId.Create("comet"), 0.5f, 0.5f, 1f, rules), Is.False);
        }

        /// <summary>Tests that <see cref="RewardBook.Move"/> keeps the sticker inside the scene and its size within the rules, and brings it on top.</summary>
        [Test]
        public void TestMove_KeepsItInsideAndBringsItOnTop()
        {
            ContentId guitar = ContentId.Create("guitar");
            ContentId whale = ContentId.Create("whale");
            RewardBook book = new RewardBook(0, null, 0, new[] { guitar, whale }, null, null, default, null);
            RewardRules rules = RewardTestContent.Rules();
            book.TryPlace(guitar, 0.5f, 0.5f, 1f, rules);
            book.TryPlace(whale, 0.5f, 0.5f, 1f, rules);

            book.Move(0, 1.4f, -0.2f, 9f, rules);

            PlacedSticker moved = book.Placed.Last();
            Assert.That(moved.StickerId, Is.EqualTo(guitar));
            Assert.That(moved.X, Is.EqualTo(1f));
            Assert.That(moved.Y, Is.EqualTo(0f));
            Assert.That(moved.Size, Is.EqualTo(3f));
            Assert.That(book.Placed[0].StickerId, Is.EqualTo(whale));
        }

        /// <summary>Tests that <see cref="RewardBook.TakeOff"/> takes a sticker off the scene and keeps it in the book.</summary>
        [Test]
        public void TestTakeOff_KeepsItInTheBook()
        {
            ContentId guitar = ContentId.Create("guitar");
            RewardBook book = new RewardBook(0, null, 0, new[] { guitar }, null, null, default, null);
            book.TryPlace(guitar, 0.5f, 0.5f, 1f, RewardTestContent.Rules());

            book.TakeOff(0);

            Assert.That(book.Placed, Is.Empty);
            Assert.That(book.OwnedStickers, Is.EqualTo(new[] { guitar }));
            Assert.That(book.CopiesLeft(guitar), Is.EqualTo(1));
        }

        /// <summary>Tests that <see cref="RewardBook.ChooseTrail"/> only takes a trail she earned.</summary>
        [Test]
        public void TestChooseTrail_OnlyAnEarnedOne()
        {
            ContentId rainbow = ContentId.Create("rainbow");
            ContentId stars = ContentId.Create("stars");
            RewardBook book = new RewardBook(0, null, 0, null, null, new[] { rainbow, stars }, rainbow, null);

            bool earned = book.ChooseTrail(stars);
            bool notEarned = book.ChooseTrail(ContentId.Create("bubbles"));

            Assert.That(earned, Is.True);
            Assert.That(notEarned, Is.False);
            Assert.That(book.ChosenTrail, Is.EqualTo(stars));
        }

        /// <summary>Tests that the <see cref="RewardBook"/> constructor brings saved data into its rules and keeps unknown ids.</summary>
        [Test]
        public void TestConstructor_BringsSavedDataIntoItsRules()
        {
            ContentId guitar = ContentId.Create("guitar");
            ContentId lesson = ContentId.Create("ro-a");
            PlacedSticker[] placed =
            {
                new PlacedSticker(guitar, 2f, float.NaN, -1f),
                new PlacedSticker(guitar, 0.5f, 0.5f, 1f)
            };

            RewardBook book = new RewardBook(-4, new[] { lesson, lesson, ContentId.Create("unknown-lesson") }, -1,
                new[] { guitar }, null, new[] { ContentId.Create("rainbow") }, ContentId.Create("bubbles"), placed);

            Assert.That(book.StarsEarned, Is.EqualTo(0));
            Assert.That(book.LessonsFinished, Is.EqualTo(0));
            Assert.That(book.LitLessons.Select(id => id.ToString()), Is.EqualTo(new[] { "ro-a", "unknown-lesson" }));
            Assert.That(book.ChosenTrail.IsSet, Is.False);
            Assert.That(book.Placed.Count, Is.EqualTo(1));
            Assert.That(book.Placed[0].X, Is.EqualTo(1f));
            Assert.That(book.Placed[0].Y, Is.EqualTo(0.5f));
            Assert.That(book.Placed[0].Size, Is.EqualTo(1f));
        }
    }
}
