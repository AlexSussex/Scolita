using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Activities;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;
using Scolita.Core.Progress;
using Scolita.Core.Timing;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Progress
{
    /// <summary>Tests <see cref="WarmUps"/> on the real planets 1–3.</summary>
    public sealed class WarmUpsTests
    {
        /// <summary>Tests that <see cref="WarmUps.For"/> gives nothing when no learned letter is due.</summary>
        [Test]
        public void TestFor_NothingDueNothingGiven()
        {
            Curriculum curriculum = RealContent.Read();
            CalendarDay today = CalendarDay.FromDate(2026, 11, 25);
            LearnerProgress progress = Progresses.Learned(today, Language.Romanian, "a", "m");

            Assert.That(Steps(curriculum, progress, today, LanguageMix.Both, "ro-p"), Is.Empty);
            Assert.That(Steps(curriculum, Progresses.Empty(), today, LanguageMix.Both, "ro-p"), Is.Empty);
        }

        /// <summary>
        /// Tests that <see cref="WarmUps.For"/> makes a due letter with a <c>FirstSound</c> word of its own a step on it among
        /// letters taught before the lesson (only taught letters as wrong choices), in the letter's language.
        /// </summary>
        [Test]
        public void TestFor_ADueLetterAmongTaughtLetters()
        {
            Curriculum curriculum = RealContent.Read();
            CalendarDay today = CalendarDay.FromDate(2026, 11, 25);
            LearnerProgress progress = Progresses.Learned(today.AddDays(-1), Language.Romanian, "m");

            IReadOnlyList<Step> steps = Steps(curriculum, progress, today, LanguageMix.Both, "ro-p");

            Assert.That(steps.Count, Is.EqualTo(1));
            Assert.That(steps[0].Kind, Is.EqualTo(ActivityKind.FirstSound));
            Assert.That(steps[0].Languages, Is.EqualTo(LanguageMix.RomanianOnly));
            Assert.That(steps[0].Letters[0].ToString(), Is.EqualTo("m"));
            Assert.That(steps[0].Letters.Count, Is.EqualTo(3));
            Assert.That(steps[0].Letters.Select(id => id.ToString()), Is.SubsetOf(new[] { "a", "m", "t" }));
            Assert.That(steps[0].Words.Select(id => id.ToString()), Is.EqualTo(new[] { "ro-mama" }));
        }

        /// <summary>Tests that <see cref="WarmUps.For"/> makes a <c>ListenAndPick</c> for a letter that has no <c>FirstSound</c> word.</summary>
        [Test]
        public void TestFor_ListenAndPickWithoutAWord()
        {
            Curriculum curriculum = RealContent.Read();
            CalendarDay today = CalendarDay.FromDate(2026, 11, 25);
            LearnerProgress progress = Progresses.Learned(today.AddDays(-1), Language.Romanian, "a");

            IReadOnlyList<Step> steps = Steps(curriculum, progress, today, LanguageMix.Both, "ro-t");

            Assert.That(steps.Count, Is.EqualTo(1));
            Assert.That(steps[0].Kind, Is.EqualTo(ActivityKind.ListenAndPick));
            Assert.That(steps[0].Letters.Select(id => id.ToString()), Is.EqualTo(new[] { "a", "m" }));
            Assert.That(steps[0].Words, Is.Empty);
        }

        /// <summary>
        /// Tests that <see cref="WarmUps.For"/> makes only steps that can be played: a <c>ListenAndPick</c> when
        /// <c>FirstSound</c> can't be, and nothing when neither can.
        /// </summary>
        [Test]
        public void TestFor_OnlyStepsThatCanBePlayed()
        {
            Curriculum curriculum = RealContent.Read();
            CalendarDay today = CalendarDay.FromDate(2026, 11, 25);
            LearnerProgress progress = Progresses.Learned(today.AddDays(-1), Language.Romanian, "m");

            IReadOnlyList<Step> pickOnly = Steps(curriculum, progress, today, LanguageMix.Both, "ro-p",
                new[] { ActivityKind.ListenAndPick });
            IReadOnlyList<Step> none = Steps(curriculum, progress, today, LanguageMix.Both, "ro-p",
                new[] { ActivityKind.MeetLetter });

            Assert.That(pickOnly.Single().Kind, Is.EqualTo(ActivityKind.ListenAndPick));
            Assert.That(none, Is.Empty);
        }

        /// <summary>Tests that <see cref="WarmUps.For"/> gives at most two warm-ups, the most overdue first.</summary>
        [Test]
        public void TestFor_AtMostTwoMostOverdueFirst()
        {
            Curriculum curriculum = RealContent.Read();
            CalendarDay today = CalendarDay.FromDate(2026, 11, 25);
            LearnerProgress progress = new LearnerProgress(Progresses.Empty().Lessons,
                Progresses.Learned(today.AddDays(-1), Language.Romanian, "t").Items
                    .Concat(Progresses.Learned(today.AddDays(-3), Language.Romanian, "a").Items)
                    .Concat(Progresses.Learned(today.AddDays(-2), Language.Romanian, "m").Items),
                null);

            IReadOnlyList<Step> steps = Steps(curriculum, progress, today, LanguageMix.Both, "ro-p");

            Assert.That(steps.Select(step => step.Letters[0].ToString()), Is.EqualTo(new[] { "a", "m" }));
        }

        /// <summary>
        /// Tests that <see cref="WarmUps.For"/> gives only letters in the lesson's language (either, for a lesson in both) and
        /// in her mix: a Romanian letter waits for a Romanian lesson, and an English one never comes in Romanian only.
        /// </summary>
        [Test]
        public void TestFor_TheLessonsLanguageAndHerMix()
        {
            Curriculum curriculum = RealContent.Read();
            CalendarDay today = CalendarDay.FromDate(2026, 11, 25);
            LearnerProgress progress = new LearnerProgress(Progresses.Empty().Lessons,
                Progresses.Learned(today.AddDays(-1), Language.Romanian, "m").Items
                    .Concat(Progresses.Learned(today.AddDays(-2), Language.English, "t").Items),
                null);

            Assert.That(Languages(Steps(curriculum, progress, today, LanguageMix.Both, "en-words-1")), Is.EqualTo(new[] { "en" }));
            Assert.That(Languages(Steps(curriculum, progress, today, LanguageMix.Both, "ro-words-1")), Is.EqualTo(new[] { "ro" }));
            Assert.That(Languages(Steps(curriculum, progress, today, LanguageMix.Both, "party-1")), Is.EqualTo(new[] { "en", "ro" }));
            Assert.That(Languages(Steps(curriculum, progress, today, LanguageMix.RomanianOnly, "party-1")), Is.EqualTo(new[] { "ro" }));
            Assert.That(Steps(curriculum, progress, today, LanguageMix.RomanianOnly, "en-words-1"), Is.Empty);
        }

        /// <summary>
        /// Tests that <see cref="WarmUps.For"/> gives only letters taught before the lesson (a lesson played again from the
        /// map doesn't review what comes after it), never the lesson's own letter.
        /// </summary>
        [Test]
        public void TestFor_OnlyLettersTaughtBefore()
        {
            Curriculum curriculum = RealContent.Read();
            CalendarDay today = CalendarDay.FromDate(2026, 11, 25);
            LearnerProgress progress = Progresses.Learned(today.AddDays(-1), Language.Romanian, "m", "t", "p");

            Assert.That(Steps(curriculum, progress, today, LanguageMix.Both, "ro-a"), Is.Empty);
            Assert.That(Steps(curriculum, progress, today, LanguageMix.Both, "ro-t").Select(step => step.Letters[0].ToString()),
                Is.EqualTo(new[] { "m" }));
        }

        /// <summary>
        /// Tests that <see cref="WarmUps.For"/> brings a learned letter back as a warm-up 1 day after it was learned, then 3
        /// days after that warm-up, then 7 (the brief's acceptance), with the reviews recorded.
        /// </summary>
        [Test]
        public void TestFor_OneThenThreeThenSevenDays()
        {
            Curriculum curriculum = RealContent.Read();
            FakeClock clock = new FakeClock(CalendarDay.FromDate(2026, 11, 25));
            LearnerProgress progress = Progresses.Learned(clock.Today, Language.Romanian, "m");
            List<int> warmUpDays = new List<int>();

            for (int day = 0; day <= 11; day++)
            {
                if (Steps(curriculum, progress, clock.Today, LanguageMix.Both, "ro-p").Count > 0)
                {
                    warmUpDays.Add(day);
                    progress.RecordReview(LearningItem.Letter(ContentId.Create("m"), Language.Romanian), false, clock.Today,
                        TestProgressRules.Default());
                }

                clock.PassDays(1);
            }

            Assert.That(warmUpDays, Is.EqualTo(new[] { 1, 4, 11 }));
        }

        private static IReadOnlyList<Step> Steps(Curriculum curriculum, LearnerProgress progress, CalendarDay today,
            LanguageMix mix, string lessonId)
        {
            return Steps(curriculum, progress, today, mix, lessonId, ActivityRulesFactory.Kinds);
        }

        private static IReadOnlyList<Step> Steps(Curriculum curriculum, LearnerProgress progress, CalendarDay today,
            LanguageMix mix, string lessonId, IEnumerable<ActivityKind> playableKinds)
        {
            WarmUps warmUps = new WarmUps(progress, curriculum, mix, new PlayableSteps(playableKinds, curriculum),
                TestProgressRules.Default(), today, new FixedRandom());
            return warmUps.For(RealContent.Lesson(curriculum, lessonId));
        }

        private static string[] Languages(IReadOnlyList<Step> steps)
        {
            return steps.Select(step => LanguageCodes.CodeOf(step.Languages)).OrderBy(code => code).ToArray();
        }
    }
}
