using System;
using NUnit.Framework;
using Scolita.Core.Activities;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;
using Scolita.Core.Tracing;
using Scolita.Core.Voice;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Activities
{
    /// <summary>
    /// Tests <see cref="TraceLetterRules"/> with the valid set's print a (two strokes: the round, then the
    /// stem): the three rounds of every stroke, the hint for what went wrong, the answer shown, the lines,
    /// and the lesson run's ladder. Planet 1's real shapes are traced in <c>RecordedStrokeTests</c>.
    /// </summary>
    public sealed class TraceLetterRulesTests
    {
        /// <summary>
        /// Tests that <see cref="TraceLetterRules.Judge"/> takes both strokes in each of the three rounds, says
        /// the next dot, the round's end and the letter's end, and moves the state on.
        /// </summary>
        [Test]
        public void TestJudge_ThreeRoundsOfEveryStroke()
        {
            TraceLetterRules rules = PrintA();
            rules.Begin(new FixedRandom());
            TraceState state = rules.Round.Trace;

            Assert.That(state.Guide, Is.EqualTo(TraceGuide.Dotted));
            Assert.That(rules.Judge(Good(rules)), Is.EqualTo(AnswerVerdict.Right));
            Assert.That(SequenceLines.Of(rules.Response), Is.EqualTo(new[] { "ro-line-trace-next-2" }));
            Assert.That(state.Stroke, Is.EqualTo(1));
            Assert.That(rules.Judge(Good(rules)), Is.EqualTo(AnswerVerdict.Right));
            Assert.That(SequenceLines.Of(rules.Response), Is.EqualTo(new[] { "ro-line-trace-done", "ro-line-trace-round-2" }));
            Assert.That(rules.Response.Items[0].TryGetFill(VoiceSlot.X, out VoiceFill sound), Is.True);
            Assert.That(sound.Text, Is.EqualTo("a"));
            Assert.That(state.RoundsDone, Is.EqualTo(1));
            Assert.That(state.Guide, Is.EqualTo(TraceGuide.Faint));
            Assert.That(state.Stroke, Is.EqualTo(0));

            Assert.That(rules.Judge(Good(rules)), Is.EqualTo(AnswerVerdict.Right));
            Assert.That(rules.Judge(Good(rules)), Is.EqualTo(AnswerVerdict.Right));
            Assert.That(SequenceLines.Of(rules.Response), Is.EqualTo(new[] { "ro-line-trace-done", "ro-line-trace-round-3" }));
            Assert.That(state.Guide, Is.EqualTo(TraceGuide.StartDotOnly));
            Assert.That(rules.Judge(Good(rules)), Is.EqualTo(AnswerVerdict.Right));
            Assert.That(rules.Judge(Good(rules)), Is.EqualTo(AnswerVerdict.Done));
            Assert.That(SequenceLines.Of(rules.Response), Is.EqualTo(new[] { "ro-line-trace-done" }));
            Assert.That(state.RoundsDone, Is.EqualTo(TracingTuning.Rounds));
            Assert.That(() => rules.Judge(Good(rules)), Throws.InstanceOf<InvalidOperationException>());

            rules.Begin(new FixedRandom());
            Assert.That(state.RoundsDone, Is.EqualTo(0));
            Assert.That(state.Guide, Is.EqualTo(TraceGuide.Dotted));
        }

        /// <summary>
        /// Tests that <see cref="TraceLetterRules.HintFor"/> gives the hint for what went wrong: the green dot,
        /// the arrow, or staying on the dotted line (in round 1; carefully, once there is no dotted line).
        /// </summary>
        [Test]
        public void TestHintFor_WhatWentWrong()
        {
            TraceLetterRules rules = PrintA();
            rules.Begin(new FixedRandom());
            StrokePath round = rules.Round.Trace.Shape.Strokes[0].Path;

            Assert.That(HintAfter(rules, TestStrokes.Along(round, 1f, 0f, 4f, 0f)), Is.EqualTo(new[] { "ro-line-trace-hint-arrow" }));
            Assert.That(HintAfter(rules, TestStrokes.Drawn(54f, 108f)), Is.EqualTo(new[] { "ro-line-trace-hint-arrow" }));
            Assert.That(HintAfter(rules, TestStrokes.Along(round, 0.5f, 1f, 4f, 0f)), Is.EqualTo(new[] { "ro-line-trace-hint-dot" }));
            Assert.That(HintAfter(rules, OffTheRound(round)), Is.EqualTo(new[] { "ro-line-trace-hint-stay" }));
            Assert.That(rules.HintFor(1, Good(rules)).Cue, Is.EqualTo(HintCue.PulseStart));

            rules.Judge(Good(rules));
            rules.Judge(Good(rules));
            Assert.That(rules.Round.Trace.Guide, Is.EqualTo(TraceGuide.Faint));
            Assert.That(HintAfter(rules, OffTheRound(round)), Is.EqualTo(new[] { "ro-line-trace-hint-slow" }));
        }

        /// <summary>
        /// Tests that <see cref="TraceLetterRules.AnswerShown"/> draws the stroke and keeps its guide shown until
        /// she traces it.
        /// </summary>
        [Test]
        public void TestAnswerShown_DrawsTheStroke()
        {
            TraceLetterRules rules = PrintA();
            rules.Begin(new FixedRandom());

            ActivityHint shown = rules.AnswerShown();

            Assert.That(shown.Cue, Is.EqualTo(HintCue.AnimateStroke));
            Assert.That(SequenceLines.Of(shown.Line), Is.EqualTo(new[] { "ro-line-trace-hint-watch" }));
            Assert.That(rules.Round.Trace.AnswerShown, Is.True);
            Assert.That(rules.Judge(Good(rules)), Is.EqualTo(AnswerVerdict.Right));
            Assert.That(rules.Round.Trace.AnswerShown, Is.False);
        }

        /// <summary>Tests that <see cref="TraceLetterRules.Judge"/> refuses a gesture that isn't a drawn stroke.</summary>
        [Test]
        public void TestJudge_RefusesATap()
        {
            TraceLetterRules rules = PrintA();
            rules.Begin(new FixedRandom());

            Assert.That(() => rules.Judge(ActivityAnswer.Tap(0)), Throws.InstanceOf<ArgumentException>());
        }

        /// <summary>
        /// Tests that <see cref="TraceLetterRules.Instruction"/> says to write the print letter from the dot, and
        /// that <see cref="TraceLetterRules.Lines"/> holds every line it can say.
        /// </summary>
        [Test]
        public void TestInstruction_AndLines()
        {
            TraceLetterRules rules = PrintA();

            Assert.That(SequenceLines.Of(rules.Instruction), Is.EqualTo(new[] { "ro-line-trace-print", "ro-line-trace-start" }));
            Assert.That(SequenceLines.Of(rules.Prompt), Is.EqualTo(new[] { "ro-line-trace-start" }));
            Assert.That(rules.Lines.Count, Is.EqualTo(11));
            Assert.That(rules.Round.Choices[0].Text, Is.EqualTo("a"));
            Assert.That(rules.Round.Trace.Shape.Id.ToString(), Is.EqualTo("a-lower-print"));
        }

        /// <summary>
        /// Tests the lesson run's one ladder on a <c>TraceLetter</c> step: a backwards stroke gets a hint, a
        /// second miss shows the answer, and the stroke traced then is practice.
        /// </summary>
        [Test]
        public void TestLessonRun_TheLadder()
        {
            Curriculum curriculum = ContentCsvs.Read(ContentCsvs.Valid());
            Lesson lesson = RealContent.Lesson(curriculum, "ro-a");
            LessonPlan plan = LessonPlan.Of(lesson, LanguageMix.Both,
                new PlayableSteps(new[] { ActivityKind.TraceLetter }, curriculum), new Step[0], 1);
            LessonRun run = new LessonRun(plan, curriculum, TestLearningRules.Default(), new FixedRandom());
            run.Start();
            StrokePath round = run.Current.Round.Trace.Shape.Strokes[0].Path;
            TracePoints backwards = TestStrokes.Along(round, 1f, 0f, 4f, 0f);

            StepFeedback hint = run.Answer(ActivityAnswer.Traced(backwards));
            StepFeedback shown = run.Answer(ActivityAnswer.Traced(backwards));
            StepFeedback practised = run.Answer(ActivityAnswer.Traced(TestStrokes.Along(round, 0f, 1f, 4f, 0f)));

            Assert.That(run.Current.Kind, Is.EqualTo(ActivityKind.TraceLetter));
            Assert.That(hint.Outcome, Is.EqualTo(FeedbackOutcome.TryAgain));
            Assert.That(hint.Hint.Cue, Is.EqualTo(HintCue.PulseStart));
            Assert.That(shown.Outcome, Is.EqualTo(FeedbackOutcome.ShowAnswer));
            Assert.That(shown.Hint.Cue, Is.EqualTo(HintCue.AnimateStroke));
            Assert.That(practised.Outcome, Is.EqualTo(FeedbackOutcome.Right));
            Assert.That(run.Current.Round.Trace.Stroke, Is.EqualTo(1));
        }

        private static TraceLetterRules PrintA()
        {
            Curriculum curriculum = ContentCsvs.Read(ContentCsvs.Valid());
            Letter a = curriculum.Letters.Get(ContentId.Create("a"));
            Assert.That(curriculum.TryGetShape(a, LetterCase.Lower, LetterForm.Print, out LetterShape shape), Is.True);
            return new TraceLetterRules(a, shape, Language.Romanian, TestLearningRules.Tracing());
        }

        private static ActivityAnswer Good(TraceLetterRules rules)
        {
            TraceState state = rules.Round.Trace;
            return ActivityAnswer.Traced(TestStrokes.Along(state.Shape.Strokes[state.Stroke].Path, 0f, 1f, 4f, 0f));
        }

        private static string[] HintAfter(TraceLetterRules rules, TracePoints points)
        {
            ActivityAnswer answer = ActivityAnswer.Traced(points);
            Assert.That(rules.Judge(answer), Is.EqualTo(AnswerVerdict.Wrong));
            return SequenceLines.Of(rules.HintFor(1, answer).Line);
        }

        private static TracePoints OffTheRound(StrokePath round)
        {
            TracePoints points = TestStrokes.Along(round, 0f, 0.3f, 4f, 0f);
            points.Add(100f, 60f);
            points.Add(140f, 40f);
            TestStrokes.AddAlong(points, round, 0.5f, 1f, 4f, 0f);
            return points;
        }
    }
}
