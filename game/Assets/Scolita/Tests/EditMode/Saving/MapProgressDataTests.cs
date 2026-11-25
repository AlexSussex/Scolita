using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Map;
using Scolita.Game.Map;
using UnityEngine;

namespace Scolita.Tests.EditMode.Saving
{
    /// <summary>Tests <see cref="MapProgressData"/>: 0.10.0's save, read safely to migrate it (<c>ProgressFileTests</c>).</summary>
    public sealed class MapProgressDataTests
    {
        /// <summary>Tests that <see cref="MapProgressData.CreateDefaults"/> holds nothing done and no rocket stop.</summary>
        [Test]
        public void TestCreateDefaults_NothingDone()
        {
            MapProgress progress = MapProgressData.CreateDefaults().ToProgress();

            Assert.That(MapProgressData.CreateDefaults().Version, Is.EqualTo(1));
            Assert.That(progress.DoneLessons, Is.Empty);
            Assert.That(progress.RocketStop.IsSet, Is.False);
        }

        /// <summary>Tests that <see cref="MapProgressData"/> survives JSON with its lessons in order and its rocket stop.</summary>
        [Test]
        public void TestToProgress_RoundTrips()
        {
            MapProgress saved = new MapProgress(new[] { ContentId.Create("ro-a"), ContentId.Create("en-a") }, ContentId.Create("ro-m"));

            string json = JsonUtility.ToJson(new MapProgressData(saved));
            MapProgress read = JsonUtility.FromJson<MapProgressData>(json).Upgraded().ToProgress();

            Assert.That(json, Does.Contain("\"doneLessons\"").And.Contain("\"rocketStop\""));
            Assert.That(read.DoneLessons.Select(id => id.ToString()), Is.EqualTo(new[] { "ro-a", "en-a" }));
            Assert.That(read.RocketStop.ToString(), Is.EqualTo("ro-m"));
        }

        /// <summary>
        /// Tests that <see cref="MapProgressData.Upgraded"/> keeps an id the content doesn't know, drops a text that
        /// can't be an id, merges duplicates, and reads a file with no fields as nothing done.
        /// </summary>
        [Test]
        public void TestUpgraded_KeepsUnknownDropsMalformed()
        {
            MapProgressData odd = JsonUtility.FromJson<MapProgressData>(
                "{\"version\":1,\"doneLessons\":[\"ro-a\",\"Not An Id\",\"planet-99-lesson\",\"ro-a\",\"\"],\"rocketStop\":\"??\"}").Upgraded();
            MapProgressData empty = JsonUtility.FromJson<MapProgressData>("{\"version\":1}").Upgraded();

            Assert.That(odd.DoneLessons, Is.EqualTo(new[] { "ro-a", "planet-99-lesson" }));
            Assert.That(odd.ToProgress().RocketStop.IsSet, Is.False);
            Assert.That(empty.DoneLessons, Is.Empty);
            Assert.That(empty.ToProgress().RocketStop.IsSet, Is.False);
        }
    }
}
