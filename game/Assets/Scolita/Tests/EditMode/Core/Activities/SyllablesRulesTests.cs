using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Activities;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Activities
{
    /// <summary>
    /// Tests <see cref="SyllablesRules"/> with planet 1's real words (<c>ro-words-1</c>: mama and apa;
    /// <c>en-words-1</c>: planet), and a made-up "pepene" for three syllables.
    /// </summary>
    public sealed class SyllablesRulesTests
    {
        /// <summary>
        /// Tests that the round's places are the word's syllables from the content (ma-ma 2, a-pa 2, plan-et 2,
        /// pe-pe-ne 3), its subject the word (written when she can read it, its picture when it is listen-only),
        /// and its choices the drum and the tick.
        /// </summary>
        [Test]
        public void TestRound_CountsTheWordsSyllables()
        {
            Curriculum curriculum = RealContent.Read();
            Step[] steps = Steps(curriculum, "ro-words-1");
            IActivityRules mama = Create(curriculum, steps[0]);
            IActivityRules apa = Create(curriculum, steps[1]);
            IActivityRules planet = Create(curriculum, Steps(curriculum, "en-words-1")[0]);
            SyllablesRules pepene = new SyllablesRules(Made("ro-pepene", "pe-pe-ne"), Language.Romanian);

            Assert.That(mama.Round.Places, Is.EqualTo(2));
            Assert.That(apa.Round.Places, Is.EqualTo(2));
            Assert.That(planet.Round.Places, Is.EqualTo(2));
            Assert.That(pepene.Round.Places, Is.EqualTo(3));
            Assert.That(mama.Round.Subject.Item.ToString(), Is.EqualTo("ro-mama"));
            Assert.That(mama.Round.Subject.Look, Is.EqualTo(ActivityChoiceLook.Word));
            Assert.That(planet.Round.Subject.Look, Is.EqualTo(ActivityChoiceLook.Picture));
            Assert.That(planet.Language, Is.EqualTo(Language.English));
            Assert.That(mama.Round.Choices.Count, Is.EqualTo(2));
            Assert.That(mama.Round.Choices[SyllablesRules.Drum].Look, Is.EqualTo(ActivityChoiceLook.Control));
        }

        /// <summary>
        /// Tests that <see cref="SyllablesRules.Judge"/> counts each drum tap as a beat (never an answer), that
        /// the tick with the right count finishes the step and says the word in syllables, and that the tick
        /// before any beat only asks for the beats.
        /// </summary>
        [Test]
        public void TestJudge_TheRightCountThenTheTick()
        {
            IActivityRules rules = Mama();
            rules.Begin(new FixedRandom());

            Assert.That(rules.Judge(ActivityAnswer.Tap(SyllablesRules.Tick)), Is.EqualTo(AnswerVerdict.Neutral));
            Assert.That(SequenceLines.Of(rules.Response), Is.EqualTo(new[] { "ro-line-syll-hint-once" }));
            Assert.That(rules.Judge(ActivityAnswer.Tap(SyllablesRules.Drum)), Is.EqualTo(AnswerVerdict.Neutral));
            Assert.That(rules.Round.PlacesFilled, Is.EqualTo(1));
            Assert.That(rules.Response, Is.Null);
            Assert.That(rules.Judge(ActivityAnswer.Tap(SyllablesRules.Drum)), Is.EqualTo(AnswerVerdict.Neutral));
            Assert.That(rules.Judge(ActivityAnswer.Tap(SyllablesRules.Tick)), Is.EqualTo(AnswerVerdict.Done));
            Assert.That(SequenceLines.Of(rules.Response), Is.EqualTo(new[] { "ro-syllables-mama" }));
        }

        /// <summary>
        /// Tests that too few or too many beats are a miss that starts the beats again, that beats past
        /// <see cref="SyllablesRules.MostBeats"/> and one more aren't counted (so 7 beats for 6 syllables is still a
        /// miss), and that only taps on the drum and the tick are
        /// taken.
        /// </summary>
        [Test]
        public void TestJudge_TooFewOrTooManyBeats()
        {
            IActivityRules rules = Mama();
            rules.Begin(new FixedRandom());

            rules.Judge(ActivityAnswer.Tap(SyllablesRules.Drum));
            Assert.That(rules.Judge(ActivityAnswer.Tap(SyllablesRules.Tick)), Is.EqualTo(AnswerVerdict.Wrong));
            Assert.That(rules.Round.PlacesFilled, Is.EqualTo(0));
            for (int beat = 0; beat < SyllablesRules.MostBeats + 3; beat++)
            {
                rules.Judge(ActivityAnswer.Tap(SyllablesRules.Drum));
            }

            Assert.That(rules.Round.PlacesFilled, Is.EqualTo(SyllablesRules.MostBeats + 1));
            Assert.That(rules.Judge(ActivityAnswer.Tap(SyllablesRules.Tick)), Is.EqualTo(AnswerVerdict.Wrong));
            SyllablesRules longest = new SyllablesRules(Made("ro-mamamamamama", "ma-ma-ma-ma-ma-ma"), Language.Romanian);
            longest.Begin(new FixedRandom());
            for (int beat = 0; beat < SyllablesRules.MostBeats + 1; beat++)
            {
                longest.Judge(ActivityAnswer.Tap(SyllablesRules.Drum));
            }

            Assert.That(longest.Judge(ActivityAnswer.Tap(SyllablesRules.Tick)), Is.EqualTo(AnswerVerdict.Wrong), "7 beats for 6 syllables");
            Assert.That(() => rules.Judge(ActivityAnswer.Tap(2)), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => rules.Judge(ActivityAnswer.Drop(SyllablesRules.Drum)), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        /// <summary>
        /// Tests the instruction (the word, then the tick), the prompt (the word), the hints (the word in
        /// syllables) and the answer shown (the syllables again, the glowing circles).
        /// </summary>
        [Test]
        public void TestLines_TheWordInSyllables()
        {
            IActivityRules rules = Mama();
            rules.Begin(new FixedRandom());

            Assert.That(SequenceLines.Of(rules.Instruction), Is.EqualTo(new[] { "ro-line-syll", "ro-word-mama", "ro-line-syll-tick" }));
            Assert.That(SequenceLines.Of(rules.Prompt), Is.EqualTo(new[] { "ro-word-mama" }));
            ActivityHint first = rules.HintFor(1, ActivityAnswer.Tap(SyllablesRules.Tick));
            Assert.That(SequenceLines.Of(first.Line), Is.EqualTo(new[] { "ro-line-syll-hint-listen", "ro-syllables-mama" }));
            Assert.That(SequenceLines.Of(rules.HintFor(3, ActivityAnswer.Tap(SyllablesRules.Tick)).Line),
                Is.EqualTo(new[] { "ro-line-syll-hint-count", "ro-syllables-mama" }));
            ActivityHint shown = rules.AnswerShown();
            Assert.That(shown.Cue, Is.EqualTo(HintCue.ShowPlaces));
            Assert.That(SequenceLines.Of(shown.Line), Is.EqualTo(new[] { "ro-syllables-mama", "ro-line-syll-hint-once" }));
            Assert.That(rules.Lines, Has.Member(first.Line).And.Member(shown.Line).And.Member(rules.Instruction));
        }

        /// <summary>
        /// Tests the <c>Syllables</c> step through <see cref="LessonRun.Answer"/>: drum taps are never misses, a
        /// wrong count gets the hint, a second shows the answer, and the right count then counts as practice.
        /// </summary>
        [Test]
        public void TestAnswer_HintThenShowThroughTheRun()
        {
            Curriculum curriculum = RealContent.Read();
            LessonPlan plan = LessonPlan.Of(RealContent.Lesson(curriculum, "ro-words-1"), LanguageMix.Both,
                new[] { ActivityKind.Syllables }, new Step[0], 1);
            LessonRun run = new LessonRun(plan, curriculum, TestLearningRules.Default(), new FixedRandom());
            List<AnswerChecked> answers = new List<AnswerChecked>();
            run.AnswerChecked += answers.Add;
            run.Start();

            Assert.That(run.Answer(ActivityAnswer.Tap(SyllablesRules.Drum)).Outcome, Is.EqualTo(FeedbackOutcome.Neutral));
            Assert.That(run.Answer(ActivityAnswer.Tap(SyllablesRules.Tick)).Outcome, Is.EqualTo(FeedbackOutcome.TryAgain));
            run.Answer(ActivityAnswer.Tap(SyllablesRules.Drum));
            run.Answer(ActivityAnswer.Tap(SyllablesRules.Drum));
            run.Answer(ActivityAnswer.Tap(SyllablesRules.Drum));
            Assert.That(run.Answer(ActivityAnswer.Tap(SyllablesRules.Tick)).Outcome, Is.EqualTo(FeedbackOutcome.ShowAnswer));
            run.Answer(ActivityAnswer.Tap(SyllablesRules.Drum));
            run.Answer(ActivityAnswer.Tap(SyllablesRules.Drum));
            StepFeedback done = run.Answer(ActivityAnswer.Tap(SyllablesRules.Tick));

            Assert.That(done.Outcome, Is.EqualTo(FeedbackOutcome.Done));
            Assert.That(answers.ConvertAll(answer => answer.Result),
                Is.EqualTo(new[] { AnswerResult.Wrong, AnswerResult.Wrong, AnswerResult.Practised }));
        }

        private static IActivityRules Mama()
        {
            Curriculum curriculum = RealContent.Read();
            return Create(curriculum, Steps(curriculum, "ro-words-1")[0]);
        }

        private static IActivityRules Create(Curriculum curriculum, Step step)
        {
            return ActivityRulesFactory.Create(step, curriculum, TestLearningRules.Default());
        }

        private static Step[] Steps(Curriculum curriculum, string lessonId)
        {
            return RealContent.Lesson(curriculum, lessonId).Steps
                .Where(step => step.Kind == ActivityKind.Syllables)
                .ToArray();
        }

        private static Word Made(string id, string syllables)
        {
            return new Word(ContentId.Create(id), Language.Romanian, syllables.Replace("-", string.Empty), null, syllables.Split('-'),
                ContentId.Create("picture"), true, default, default, false, default);
        }
    }
}
