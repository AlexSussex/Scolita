using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Progress;
using Scolita.Core.Timing;
using Scolita.Game.Progress;
using Scolita.Tests.EditMode.Fakes;
using UnityEngine;

namespace Scolita.Tests.EditMode.Saving
{
    /// <summary>Tests <see cref="ProgressData"/>: the <c>progress</c> save's shape, its round trip and its upgrade.</summary>
    public sealed class ProgressDataTests
    {
        /// <summary>Tests that <see cref="ProgressData.CreateDefaults"/> is version 1 with nothing done, met or played.</summary>
        [Test]
        public void TestCreateDefaults_Nothing()
        {
            LearnerProgress progress = ProgressData.CreateDefaults().ToProgress();

            Assert.That(ProgressData.CreateDefaults().Version, Is.EqualTo(1));
            Assert.That(progress.Lessons.DoneLessons, Is.Empty);
            Assert.That(progress.Lessons.RocketStop.IsSet, Is.False);
            Assert.That(progress.Items, Is.Empty);
            Assert.That(progress.DaysPlayed, Is.Empty);
        }

        /// <summary>
        /// Tests that <see cref="ProgressData"/> survives JSON with every value: the lessons, the rocket's stop, the days played
        /// and each item's state, rights by kind, practice, days, and reviews, in the shape of the Design note.
        /// </summary>
        [Test]
        public void TestToProgress_RoundTrips()
        {
            CalendarDay day = CalendarDay.FromDate(2026, 11, 25);
            LearnerProgress saved = Progresses.Learned(day, Language.Romanian, "m");
            saved.Lessons.MarkDone(ContentId.Create("ro-a"));
            saved.Lessons.ShowRocketAt(ContentId.Create("en-a"));
            saved.RecordDayPlayed(day);
            saved.RecordPractised(new LearningItem(ItemKind.Word, ContentId.Create("ro-mama"), Language.Romanian));
            saved.RecordReview(LearningItem.Letter(ContentId.Create("m"), Language.Romanian), false, day.AddDays(1),
                TestProgressRules.Default());

            string json = JsonUtility.ToJson(new ProgressData(saved));
            LearnerProgress read = JsonUtility.FromJson<ProgressData>(json).Upgraded().ToProgress();

            Assert.That(json, Does.Contain("\"rights\":[{\"activity\":\"ListenAndPick\",\"count\":3}")
                .And.Contain("\"kind\":\"letter\",\"id\":\"m\",\"language\":\"ro\",\"state\":\"learned\"")
                .And.Contain("\"nextReview\":\"2026-11-29\""));
            Assert.That(read.Lessons.DoneLessons.Select(id => id.ToString()), Is.EqualTo(new[] { "ro-a" }));
            Assert.That(read.Lessons.RocketStop.ToString(), Is.EqualTo("en-a"));
            Assert.That(read.DaysPlayed, Is.EqualTo(new[] { day }));
            ItemProgress m = read.Items[0];
            Assert.That(m.Item, Is.EqualTo(LearningItem.Letter(ContentId.Create("m"), Language.Romanian)));
            Assert.That(m.State, Is.EqualTo(ItemState.Learned));
            Assert.That(m.RightsOf(ActivityKind.ListenAndPick), Is.EqualTo(3));
            Assert.That(m.RightsOf(ActivityKind.FirstSound), Is.EqualTo(3));
            Assert.That(m.RightDays, Is.EqualTo(new[] { day.AddDays(-1), day }));
            Assert.That(m.LearnedOn, Is.EqualTo(day));
            Assert.That(m.ReviewStep, Is.EqualTo(1));
            Assert.That(m.NextReview, Is.EqualTo(day.AddDays(4)));
            Assert.That(read.Items[1].Item.Kind, Is.EqualTo(ItemKind.Word));
            Assert.That(read.Items[1].Practised, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that <see cref="ProgressData.Upgraded"/> keeps ids the content doesn't know, drops what can't be read (an id,
        /// an item kind, a language, a state, an activity kind, a day), raises negative counts to 0, merges duplicates, and
        /// reads a file with no fields as nothing.
        /// </summary>
        [Test]
        public void TestUpgraded_KeepsUnknownDropsMalformed()
        {
            string odd = "{\"version\":1,\"doneLessons\":[\"ro-a\",\"Not An Id\",\"planet-99-lesson\",\"ro-a\"],\"rocketStop\":\"??\","
                + "\"daysPlayed\":[\"2026-11-25\",\"yesterday\",\"2026-11-25\"],\"items\":["
                + "{\"kind\":\"letter\",\"id\":\"zz-later\",\"language\":\"ro\",\"state\":\"learning\",\"practised\":-3,"
                + "\"rights\":[{\"activity\":\"ListenAndPick\",\"count\":-2},{\"activity\":\"SingAlong\",\"count\":4},{\"activity\":\"FirstSound\",\"count\":2}],"
                + "\"rightDays\":[\"2026-11-24\",\"never\"]},"
                + "{\"kind\":\"name\",\"id\":\"m\",\"language\":\"ro\",\"state\":\"learning\"},"
                + "{\"kind\":\"letter\",\"id\":\"m\",\"language\":\"fr\",\"state\":\"learning\"},"
                + "{\"kind\":\"letter\",\"id\":\"m\",\"language\":\"ro\",\"state\":\"forgotten\"},"
                + "{\"kind\":\"letter\",\"id\":\"Bad Id\",\"language\":\"ro\",\"state\":\"learning\"},"
                + "{\"kind\":\"letter\",\"id\":\"zz-later\",\"language\":\"ro\",\"state\":\"learned\"},"
                + "{\"kind\":\"letter\",\"id\":\"t\",\"language\":\"en\",\"state\":\"learned\",\"learnedOn\":\"x\",\"nextReview\":\"\",\"reviewStep\":-1}]}";

            LearnerProgress read = JsonUtility.FromJson<ProgressData>(odd).Upgraded().ToProgress();
            LearnerProgress empty = JsonUtility.FromJson<ProgressData>("{\"version\":1}").Upgraded().ToProgress();

            Assert.That(read.Lessons.DoneLessons.Select(id => id.ToString()), Is.EqualTo(new[] { "ro-a", "planet-99-lesson" }));
            Assert.That(read.Lessons.RocketStop.IsSet, Is.False);
            Assert.That(read.DaysPlayed, Is.EqualTo(new[] { CalendarDay.FromDate(2026, 11, 25) }));
            Assert.That(read.Items.Select(item => item.Item.ToString()), Is.EqualTo(new[] { "letter zz-later (ro)", "letter t (en)" }));
            ItemProgress later = read.Items[0];
            Assert.That(later.State, Is.EqualTo(ItemState.Learning));
            Assert.That(later.Practised, Is.EqualTo(0));
            Assert.That(later.TotalRights, Is.EqualTo(2));
            Assert.That(later.RightDays, Is.EqualTo(new[] { CalendarDay.FromDate(2026, 11, 24) }));
            Assert.That(read.Items[1].IsDue(CalendarDay.FromDate(2026, 11, 25)), Is.True);
            Assert.That(read.Items[1].ReviewStep, Is.EqualTo(0));
            Assert.That(empty.Items, Is.Empty);
            Assert.That(empty.Lessons.DoneLessons, Is.Empty);
        }
    }
}
