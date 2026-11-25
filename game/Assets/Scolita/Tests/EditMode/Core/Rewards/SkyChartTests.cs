using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Lessons;
using Scolita.Core.Rewards;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Rewards
{
    /// <summary>Tests <see cref="SkyChart"/> and <see cref="SkyConstellation"/> on the real lessons.</summary>
    public sealed class SkyChartTests
    {
        /// <summary>Tests that <see cref="SkyChart.Of"/> has a constellation per planet, with three places per lesson.</summary>
        [Test]
        public void TestOf_APlanetsConstellationHasThreePlacesPerLesson()
        {
            Curriculum curriculum = RealContent.Read();
            LessonOrder order = RewardTestContent.Order(curriculum);

            SkyChart sky = SkyChart.Of(RewardTestContent.Of(3), order, RewardBook.Empty(), RewardTestContent.Rules());

            Assert.That(sky.Constellations.Select(constellation => constellation.Constellation.UnitId.ToString()),
                Is.EqualTo(new[] { "planet-1", "planet-2", "planet-3" }));
            Assert.That(sky.Constellations.Select(constellation => constellation.PlanetIndex), Is.EqualTo(new[] { 0, 1, 2 }));
            Assert.That(sky.Constellations[0].Places.Count, Is.EqualTo(RewardTestContent.LessonsOf(order, "planet-1").Length * 3));
            Assert.That(sky.Constellations.Select(constellation => constellation.LitCount), Is.All.EqualTo(0));
        }

        /// <summary>Tests that a lesson's stars light the next places, in the order she lit them, whatever the lessons' order.</summary>
        [Test]
        public void TestTryGetPlacesOf_TheOrderSheLitThem()
        {
            Curriculum curriculum = RealContent.Read();
            LessonOrder order = RewardTestContent.Order(curriculum);
            Lesson[] planet = RewardTestContent.LessonsOf(order, "planet-2");
            RewardBook book = new RewardBook(6, new[] { planet[3].Id, planet[0].Id }, 2, null, null, null, default, null);

            SkyChart sky = SkyChart.Of(RewardTestContent.Of(3), order, book, RewardTestContent.Rules());

            Assert.That(sky.TryGet(ContentId.Create("planet-2"), out SkyConstellation constellation), Is.True);
            Assert.That(constellation.LitCount, Is.EqualTo(6));
            Assert.That(constellation.TryGetPlacesOf(planet[3].Id, out int first, out int count), Is.True);
            Assert.That(new[] { first, count }, Is.EqualTo(new[] { 0, 3 }));
            Assert.That(constellation.TryGetPlacesOf(planet[0].Id, out first, out count), Is.True);
            Assert.That(new[] { first, count }, Is.EqualTo(new[] { 3, 3 }));
            Assert.That(constellation.TryGetPlacesOf(planet[1].Id, out first, out count), Is.False);
            Assert.That(sky.Constellations[0].LitCount, Is.EqualTo(0));
        }

        /// <summary>Tests that a planet's constellation is complete when every one of its lessons has its stars.</summary>
        [Test]
        public void TestIsComplete_EveryLessonLit()
        {
            Curriculum curriculum = RealContent.Read();
            LessonOrder order = RewardTestContent.Order(curriculum);
            ContentId[] planet = RewardTestContent.LessonsOf(order, "planet-3").Select(lesson => lesson.Id).ToArray();
            RewardBook book = new RewardBook(0, planet, 0, null, null, null, default, null);

            SkyChart sky = SkyChart.Of(RewardTestContent.Of(3), order, book, RewardTestContent.Rules());

            Assert.That(sky.Constellations[2].IsComplete, Is.True);
            Assert.That(sky.Constellations[1].IsComplete, Is.False);
        }
    }
}
