using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Map;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Map
{
    /// <summary>Tests <see cref="RocketMove"/> on the real content.</summary>
    public sealed class RocketMoveTests
    {
        /// <summary>Tests that <see cref="RocketMove.Between"/> places the rocket when the map was never shown.</summary>
        [Test]
        public void TestBetween_FirstTimePlaced()
        {
            MapRoute route = MapRoutes.Of(RealContent.Read(), LanguageMix.Both);

            RocketMove move = RocketMove.Between(route, default);

            Assert.That(move.Kind, Is.EqualTo(RocketMoveKind.Placed));
            Assert.That(move.From, Is.Null);
            Assert.That(move.To.Lesson.Id.ToString(), Is.EqualTo("ro-a"));
        }

        /// <summary>Tests that <see cref="RocketMove.Between"/> places the rocket when it was shown on the same stop (a lesson left half-way, or replayed).</summary>
        [Test]
        public void TestBetween_SameStopPlaced()
        {
            MapRoute route = MapRoutes.Before(RealContent.Read(), "ro-m");

            RocketMove move = RocketMove.Between(route, ContentId.Create("ro-m"));

            Assert.That(move.Kind, Is.EqualTo(RocketMoveKind.Placed));
        }

        /// <summary>Tests that <see cref="RocketMove.Between"/> hops to a later stop on the same planet.</summary>
        [Test]
        public void TestBetween_LaterStopOnThePlanetHops()
        {
            MapRoute route = MapRoutes.Before(RealContent.Read(), "ro-m");

            RocketMove move = RocketMove.Between(route, ContentId.Create("en-a"));

            Assert.That(move.Kind, Is.EqualTo(RocketMoveKind.Hop));
            Assert.That(move.From.Lesson.Id.ToString(), Is.EqualTo("en-a"));
            Assert.That(move.To.Lesson.Id.ToString(), Is.EqualTo("ro-m"));
        }

        /// <summary>Tests that <see cref="RocketMove.Between"/> plays the trip when the go stop is on a later planet (planet 1 finished).</summary>
        [Test]
        public void TestBetween_LaterPlanetTrip()
        {
            MapRoute route = MapRoutes.Before(RealContent.Read(), "ro-s");

            RocketMove move = RocketMove.Between(route, ContentId.Create("party-1"));

            Assert.That(move.Kind, Is.EqualTo(RocketMoveKind.Trip));
            Assert.That(move.From.PlanetIndex, Is.EqualTo(0));
            Assert.That(move.To.PlanetIndex, Is.EqualTo(1));
        }

        /// <summary>Tests that <see cref="RocketMove.Between"/> places the rocket, without a trip, on a stop behind the one last shown (every stop done: the first again).</summary>
        [Test]
        public void TestBetween_BackwardsPlaced()
        {
            MapRoute route = MapRoutes.AllDone(RealContent.Read());

            RocketMove move = RocketMove.Between(route, ContentId.Create("party-3"));

            Assert.That(move.Kind, Is.EqualTo(RocketMoveKind.Placed));
            Assert.That(move.To, Is.SameAs(route.Stops[0]));
        }

        /// <summary>Tests that <see cref="RocketMove.Between"/> places the rocket when the stop last shown is no longer on the map.</summary>
        [Test]
        public void TestBetween_UnknownStopPlaced()
        {
            MapRoute route = MapRoutes.Before(RealContent.Read(), "ro-s");

            RocketMove move = RocketMove.Between(route, ContentId.Create("no-such-lesson"));

            Assert.That(move.Kind, Is.EqualTo(RocketMoveKind.Placed));
        }
    }
}
