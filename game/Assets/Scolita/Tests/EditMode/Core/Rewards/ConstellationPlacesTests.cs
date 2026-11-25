using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Rewards;

namespace Scolita.Tests.EditMode.Core.Rewards
{
    /// <summary>Tests <see cref="ConstellationPlaces"/>.</summary>
    public sealed class ConstellationPlacesTests
    {
        /// <summary>Tests that <see cref="ConstellationPlaces.Spread"/> gives the first stars when there are fewer places than stars.</summary>
        [Test]
        public void TestSpread_FewerPlacesThanStars()
        {
            IReadOnlyList<StarPoint> places = ConstellationPlaces.Spread(LongL(), 2);

            Assert.That(places.Select(place => place.X), Is.EqualTo(new[] { 0f, 0f }));
            Assert.That(places.Select(place => place.Y), Is.EqualTo(new[] { 0f, 300f }));
        }

        /// <summary>Tests that <see cref="ConstellationPlaces.Spread"/> puts its stars first, then the extra places along the lines by their length.</summary>
        [Test]
        public void TestSpread_ExtraPlacesAlongTheLinesByLength()
        {
            IReadOnlyList<StarPoint> places = ConstellationPlaces.Spread(LongL(), 7);

            Assert.That(places.Count, Is.EqualTo(7));
            Assert.That(places.Take(3).Select(place => place.Y), Is.EqualTo(new[] { 0f, 300f, 300f }));
            StarPoint[] onTheLongLine = places.Skip(3).Take(3).ToArray();
            Assert.That(onTheLongLine.Select(place => place.X), Is.All.EqualTo(0f));
            Assert.That(onTheLongLine.Select(place => place.Y), Is.EqualTo(new[] { 75f, 150f, 225f }));
            Assert.That(places[6].X, Is.EqualTo(50f).Within(0.001f));
            Assert.That(places[6].Y, Is.EqualTo(300f));
        }

        /// <summary>Tests that <see cref="ConstellationPlaces.Spread"/> puts the extra places between consecutive stars when the figure has no lines.</summary>
        [Test]
        public void TestSpread_NoLinesUsesConsecutiveStars()
        {
            Constellation dots = new Constellation(ContentId.Create("dots"), ContentId.Create("planet-1"),
                new[] { new StarPoint(0f, 0f), new StarPoint(100f, 0f) }, new StarLine[0], false, default);

            IReadOnlyList<StarPoint> places = ConstellationPlaces.Spread(dots, 3);

            Assert.That(places[2].X, Is.EqualTo(50f).Within(0.001f));
        }

        private static Constellation LongL()
        {
            return new Constellation(ContentId.Create("l"), ContentId.Create("planet-1"),
                new[] { new StarPoint(0f, 0f), new StarPoint(0f, 300f), new StarPoint(100f, 300f) },
                new[] { new StarLine(0, 1), new StarLine(1, 2) }, false, default);
        }
    }
}
