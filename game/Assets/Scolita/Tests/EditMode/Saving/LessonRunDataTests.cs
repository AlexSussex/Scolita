using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Game.Lessons;
using UnityEngine;

namespace Scolita.Tests.EditMode.Saving
{
    /// <summary>Tests <see cref="LessonRunData"/>: its defaults, its upgrade and its ranges.</summary>
    public sealed class LessonRunDataTests
    {
        /// <summary>Tests that <see cref="LessonRunData.CreateDefaults"/> holds no lesson, at step 1.</summary>
        [Test]
        public void TestCreateDefaults_NoLesson()
        {
            LessonRunData defaults = LessonRunData.CreateDefaults();

            Assert.That(defaults.Version, Is.EqualTo(1));
            Assert.That(defaults.TryGetLessonId(out ContentId _), Is.False);
            Assert.That(defaults.StepNumber, Is.EqualTo(1));
            Assert.That(defaults.StepsSignature, Is.Empty);
        }

        /// <summary>Tests that <see cref="LessonRunData"/> survives JSON and keeps its lesson, step and signature.</summary>
        [Test]
        public void TestUpgraded_RoundTrips()
        {
            LessonRunData saved = new LessonRunData(ContentId.Create("ro-t"), 2, "0123456789abcdef");

            LessonRunData read = JsonUtility.FromJson<LessonRunData>(JsonUtility.ToJson(saved)).Upgraded();

            Assert.That(read.TryGetLessonId(out ContentId id), Is.True);
            Assert.That(id.ToString(), Is.EqualTo("ro-t"));
            Assert.That(read.StepNumber, Is.EqualTo(2));
            Assert.That(read.StepsSignature, Is.EqualTo("0123456789abcdef"));
        }

        /// <summary>
        /// Tests that <see cref="LessonRunData.Upgraded"/> reads a step below 1 as 1 and a lesson id that
        /// isn't one as none, and a file from before any field as the defaults.
        /// </summary>
        [Test]
        public void TestUpgraded_ClampsAndIgnores()
        {
            LessonRunData odd = JsonUtility.FromJson<LessonRunData>("{\"version\":1,\"lessonId\":\"Not An Id\",\"stepNumber\":-3}").Upgraded();
            LessonRunData empty = JsonUtility.FromJson<LessonRunData>("{}").Upgraded();

            Assert.That(odd.StepNumber, Is.EqualTo(1));
            Assert.That(odd.TryGetLessonId(out ContentId _), Is.False);
            Assert.That(odd.StepsSignature, Is.Empty);
            Assert.That(empty.Version, Is.EqualTo(1));
            Assert.That(empty.StepNumber, Is.EqualTo(1));
        }
    }
}
