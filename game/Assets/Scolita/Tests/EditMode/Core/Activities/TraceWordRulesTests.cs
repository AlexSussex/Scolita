using System;
using System.Linq;
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
    /// Tests <see cref="TraceWordRules"/> with the real planet 1 words (mama and tata in Romanian, map and mat in
    /// English): the instruction, each letter's checkpoint naming the next letter, the dots and bars after the word,
    /// the rounds, a lifted join, the hints and the answer shown. Tests <see cref="TraceLetterRules"/> by hand too.
    /// </summary>
    public sealed class TraceWordRulesTests
    {
        /// <summary>
        /// Tests that <see cref="TraceWordRules.Judge"/> on mama, letter by letter, says „Bravo, ai făcut litera m! Acum
        /// leag-o de litera a…” at each letter but the last, says nothing new within a letter, then the word's line.
        /// </summary>
        [Test]
        public void TestJudge_MamaLetterByLetter()
        {
            TraceWordRules rules = Rules("ro-words-1", "ro-mama");
            rules.Begin(new FixedRandom());
            TraceState state = rules.Round.Trace;

            Assert.That(SequenceLines.Of(rules.Instruction), Is.EqualTo(new[] { "ro-line-word-write", "ro-slow-mama" }));
            Assert.That(rules.Judge(Stroke(state)), Is.EqualTo(AnswerVerdict.Right));
            Assert.That(SequenceLines.Of(rules.Response), Is.EqualTo(new[] { "ro-line-word-join" }));
            Assert.That(Fill(rules.Response, VoiceSlot.X), Is.EqualTo("m"));
            Assert.That(Fill(rules.Response, VoiceSlot.Y), Is.EqualTo("a"));
            Assert.That(rules.Response.Items[0].TryGetFill(VoiceSlot.X, out VoiceFill sound) && sound.Line.ToString() == "ro-sound-m", Is.True);
            Assert.That(SequenceLines.Of(rules.Prompt), Is.EqualTo(new[] { "ro-line-word-join" }));

            Assert.That(rules.Judge(Stroke(state)), Is.EqualTo(AnswerVerdict.Right));
            Assert.That(rules.Response, Is.Null);
            Assert.That(rules.Judge(Stroke(state)), Is.EqualTo(AnswerVerdict.Right));
            Assert.That(rules.Response, Is.Null);
            Assert.That(rules.Judge(Stroke(state)), Is.EqualTo(AnswerVerdict.Right));
            Assert.That(Fill(rules.Response, VoiceSlot.X), Is.EqualTo("a"));
            Assert.That(Fill(rules.Response, VoiceSlot.Y), Is.EqualTo("m"));
            while (state.RoundsDone == 0)
            {
                rules.Judge(Stroke(state));
            }

            Assert.That(SequenceLines.Of(rules.Response), Is.EqualTo(new[] { "ro-line-word-done", "ro-line-trace-round-2" }));
            Assert.That(Fill(rules.Response, VoiceSlot.Word), Is.EqualTo("mama"));
            Assert.That(state.Guide, Is.EqualTo(TraceGuide.Faint));
        }

        /// <summary>
        /// Tests that <see cref="TraceWordRules.Judge"/> takes mama written in one line, three rounds, the last one
        /// <see cref="AnswerVerdict.Done"/> with the word's line.
        /// </summary>
        [Test]
        public void TestJudge_MamaInOneLineThreeRounds()
        {
            TraceWordRules rules = Rules("ro-words-1", "ro-mama");
            rules.Begin(new FixedRandom());
            TraceState state = rules.Round.Trace;

            Assert.That(rules.Judge(Line(state)), Is.EqualTo(AnswerVerdict.Right));
            Assert.That(rules.Judge(Line(state)), Is.EqualTo(AnswerVerdict.Right));
            Assert.That(rules.Judge(Line(state)), Is.EqualTo(AnswerVerdict.Done));
            Assert.That(SequenceLines.Of(rules.Response), Is.EqualTo(new[] { "ro-line-word-done" }));
            Assert.That(() => rules.Judge(Line(state)), Throws.InstanceOf<InvalidOperationException>());
        }

        /// <summary>
        /// Tests that <see cref="TraceWordRules.Judge"/> on tata asks for the bars after the word („Acum trage liniuța
        /// literei t.”), one for each t.
        /// </summary>
        [Test]
        public void TestJudge_TataBarsAfterTheWord()
        {
            TraceWordRules rules = Rules("ro-words-1", "ro-tata");
            rules.Begin(new FixedRandom());
            TraceState state = rules.Round.Trace;

            Assert.That(rules.Judge(Line(state)), Is.EqualTo(AnswerVerdict.Right));
            Assert.That(state.Stroke, Is.EqualTo(8));
            Assert.That(SequenceLines.Of(rules.Response), Is.EqualTo(new[] { "ro-line-trace-bar-t" }));
            Assert.That(rules.Judge(Stroke(state)), Is.EqualTo(AnswerVerdict.Right));
            Assert.That(SequenceLines.Of(rules.Response), Is.EqualTo(new[] { "ro-line-trace-bar-t" }));
            Assert.That(rules.Judge(Stroke(state)), Is.EqualTo(AnswerVerdict.Right));
            Assert.That(SequenceLines.Of(rules.Response), Is.EqualTo(new[] { "ro-line-word-done", "ro-line-trace-round-2" }));
        }

        /// <summary>
        /// Tests that <see cref="TraceWordRules.Judge"/> forgives a finger lifted at a join and put down near it (within
        /// the join radius, farther than the start radius), and says „Nu ridica degetul la legătură.” for a line lifted
        /// inside a join.
        /// </summary>
        [Test]
        public void TestJudge_ALiftedJoin()
        {
            TraceWordRules rules = Rules("ro-words-1", "ro-mama");
            rules.Begin(new FixedRandom());
            TraceState state = rules.Round.Trace;
            rules.Judge(Stroke(state));
            StrokePath join = state.Figure.Strokes[state.Stroke].Path;
            TracePoints landed = new TracePoints();
            landed.Add(join.Start.X + 24f, join.Start.Y - 30f);
            TestStrokes.AddAlong(landed, join, 0.05f, 1f, 4f, 0f);

            Assert.That(rules.Judge(ActivityAnswer.Traced(landed)), Is.EqualTo(AnswerVerdict.Right));
            Assert.That(state.Stroke, Is.EqualTo(2));

            TraceWordRules again = Rules("ro-words-1", "ro-mama");
            again.Begin(new FixedRandom());
            TraceState next = again.Round.Trace;
            StrokePath chain = next.Figure.ChainFrom(0);
            float intoTheJoin = (next.Figure.ChainEnd(0, 0) + 12f) / chain.Length;
            Assert.That(again.Judge(ActivityAnswer.Traced(TestStrokes.Along(chain, 0f, intoTheJoin, 3f, 0f))), Is.EqualTo(AnswerVerdict.Right));
            Assert.That(Fill(again.Response, VoiceSlot.X), Is.EqualTo("m"));
        }

        /// <summary>
        /// Tests that <see cref="TraceWordRules.HintFor"/> says „Nu ridica degetul la legătură.” for a line that stops
        /// part-way along a join.
        /// </summary>
        [Test]
        public void TestHintFor_ALineStoppedInAJoin()
        {
            TraceWordRules rules = Rules("ro-words-1", "ro-mama");
            rules.Begin(new FixedRandom());
            TraceState state = rules.Round.Trace;
            rules.Judge(Stroke(state));
            TracePoints part = TestStrokes.Along(state.Figure.Strokes[state.Stroke].Path, 0f, 0.3f, 3f, 0f);

            Assert.That(rules.Judge(ActivityAnswer.Traced(part)), Is.EqualTo(AnswerVerdict.Wrong));
            Assert.That(SequenceLines.Of(rules.HintFor(1, ActivityAnswer.Traced(part)).Line),
                Is.EqualTo(new[] { "ro-line-word-hint-join" }));
        }

        /// <summary>
        /// Tests that <see cref="TraceWordRules.HintFor"/> says „Pornește de pe linie.” for tata begun at its body, and
        /// that <see cref="TraceWordRules.AnswerShown"/> animates the letter with „Uite cum se leagă literele.”.
        /// </summary>
        [Test]
        public void TestHintFor_NoEntryAndTheAnswerShown()
        {
            TraceWordRules rules = Rules("ro-words-1", "ro-tata");
            rules.Begin(new FixedRandom());
            StrokePath chain = rules.Round.Trace.Figure.ChainFrom(0);
            TracePoints noEntry = TestStrokes.Along(chain, chain.EntryLength / chain.Length, 0.3f, 4f, 0f);

            Assert.That(rules.Judge(ActivityAnswer.Traced(noEntry)), Is.EqualTo(AnswerVerdict.Wrong));
            ActivityHint hint = rules.HintFor(1, ActivityAnswer.Traced(noEntry));
            Assert.That(SequenceLines.Of(hint.Line), Is.EqualTo(new[] { "ro-line-word-hint-from-line" }));
            Assert.That(hint.Cue, Is.EqualTo(HintCue.PulseStart));
            ActivityHint shown = rules.AnswerShown();
            Assert.That(SequenceLines.Of(shown.Line), Is.EqualTo(new[] { "ro-line-word-hint-watch" }));
            Assert.That(shown.Cue, Is.EqualTo(HintCue.AnimateStroke));
            Assert.That(rules.Round.Trace.AnswerShown, Is.True);
        }

        /// <summary>
        /// Tests that every line <see cref="TraceWordRules"/> can say for planet 1's four words is in the content, in the
        /// word's language, with every {x}–{y} pair of the letter checkpoint.
        /// </summary>
        [Test]
        public void TestLines_PlanetOnesWords()
        {
            Curriculum curriculum = RealContent.Read();
            string[][] pairs =
            {
                new[] { "ro-words-1", "ro-mama", "m-a a-m m-a" }, new[] { "ro-words-1", "ro-tata", "t-a a-t t-a" },
                new[] { "en-words-1", "en-map", "m-a a-p" }, new[] { "en-words-1", "en-mat", "m-a a-t" }
            };

            foreach (string[] word in pairs)
            {
                TraceWordRules rules = Rules(word[0], word[1]);
                string[] joins = rules.Lines
                    .Where(line => line.Lead.ToString().EndsWith("-line-word-join", StringComparison.Ordinal))
                    .Select(line => Fill(line, VoiceSlot.X) + "-" + Fill(line, VoiceSlot.Y))
                    .ToArray();
                Assert.That(string.Join(" ", joins), Is.EqualTo(word[2]), word[1]);
                foreach (VoiceSequenceItem item in rules.Lines.SelectMany(line => line.Items))
                {
                    Assert.That(curriculum.VoiceLines.TryGet(item.Line.Id, out VoiceLine line), Is.True, item.Line.ToString());
                    Assert.That(line.Language, Is.EqualTo(rules.Language), item.Line.ToString());
                }
            }
        }

        /// <summary>
        /// Tests that <see cref="TraceLetterRules"/> by hand says „Scrie litera de mână.” and, for a letter with an entry,
        /// „Linia de jos: de aici pornești.”; takes the handwritten a's three strokes in one line; hints „Pornește de pe
        /// linie.” for a line without its entry; and plays one round for the English step (<c>rounds=1</c>).
        /// </summary>
        [Test]
        public void TestJudge_TraceLetterByHand()
        {
            Curriculum curriculum = RealContent.Read();
            TraceLetterRules a = (TraceLetterRules)Create(curriculum, "ro-a", ActivityKind.TraceLetter, "handwritten");
            TraceLetterRules m = (TraceLetterRules)Create(curriculum, "ro-m", ActivityKind.TraceLetter, "handwritten");
            TraceLetterRules english = (TraceLetterRules)Create(curriculum, "en-a", ActivityKind.TraceLetter, "handwritten");
            a.Begin(new FixedRandom());
            english.Begin(new FixedRandom());
            StrokePath chain = a.Round.Trace.Figure.ChainFrom(0);
            TracePoints noEntry = TestStrokes.Along(chain, chain.EntryLength / chain.Length, 0.6f, 4f, 0f);

            Assert.That(SequenceLines.Of(a.Instruction),
                Is.EqualTo(new[] { "ro-line-trace-hand", "ro-line-trace-baseline", "ro-line-trace-start" }));
            Assert.That(SequenceLines.Of(m.Instruction), Is.EqualTo(new[] { "ro-line-trace-hand", "ro-line-trace-start" }));
            Assert.That(a.Judge(ActivityAnswer.Traced(noEntry)), Is.EqualTo(AnswerVerdict.Wrong));
            Assert.That(SequenceLines.Of(a.HintFor(1, ActivityAnswer.Traced(noEntry)).Line), Is.EqualTo(new[] { "ro-line-trace-hint-from-line" }));
            Assert.That(a.Judge(Line(a.Round.Trace)), Is.EqualTo(AnswerVerdict.Right));
            Assert.That(SequenceLines.Of(a.Response), Is.EqualTo(new[] { "ro-line-trace-done-hand", "ro-line-trace-round-2" }));
            Assert.That(english.Round.Trace.Rounds, Is.EqualTo(1));
            Assert.That(english.Judge(Line(english.Round.Trace)), Is.EqualTo(AnswerVerdict.Done));
            Assert.That(SequenceLines.Of(english.Response), Is.EqualTo(new[] { "en-line-trace-done-hand" }));
        }

        /// <summary>
        /// Tests that <see cref="TraceLetterRules.Judge"/> keeps the handwritten a's entry done by a line that then strays
        /// from the round, says „Rămâi pe linia punctată.” without a miss, and keeps only her points along the entry.
        /// </summary>
        [Test]
        public void TestJudge_ALineThatStraysAfterAStroke()
        {
            Curriculum curriculum = RealContent.Read();
            TraceLetterRules a = (TraceLetterRules)Create(curriculum, "ro-a", ActivityKind.TraceLetter, "handwritten");
            a.Begin(new FixedRandom());
            TraceState state = a.Round.Trace;
            StrokePath chain = state.Figure.ChainFrom(0);
            TracePoints strays = TestStrokes.Along(chain, 0f, state.Figure.ChainEnd(0, 0) / chain.Length, 3f, 0f);
            ShapePoint end = chain.PointAt(state.Figure.ChainEnd(0, 0));
            strays.Add(end.X + 60f, end.Y - 60f);
            strays.Add(end.X + 120f, end.Y - 90f);

            Assert.That(a.Judge(ActivityAnswer.Traced(strays)), Is.EqualTo(AnswerVerdict.Right));
            Assert.That(state.Stroke, Is.EqualTo(1));
            Assert.That(SequenceLines.Of(a.Response), Is.EqualTo(new[] { "ro-line-trace-hint-stay" }));
            Assert.That(state.LastPointsKept, Is.LessThan(strays.Count));
        }

        /// <summary>
        /// Tests that a <c>TraceWord</c> step and the handwritten <c>TraceLetter</c> steps of planet 1 are played now
        /// (<see cref="PlayableSteps"/>), and the lesson run takes mama to its end.
        /// </summary>
        [Test]
        public void TestPlayableSteps_HandwritingPlays()
        {
            Curriculum curriculum = RealContent.Read();
            PlayableSteps playable = new PlayableSteps(new[] { ActivityKind.TraceLetter, ActivityKind.TraceWord }, curriculum);
            Step word = RealContent.FirstStep(curriculum, "ro-words-1", ActivityKind.TraceWord);
            Step byHand = RealContent.Lesson(curriculum, "capitals-1").Steps
                .First(step => step.Kind == ActivityKind.TraceLetter && step.Options.Any(option => option.Value == "handwritten"));

            Assert.That(playable.Includes(word), Is.True);
            Assert.That(playable.Includes(byHand), Is.True);
        }

        private static TraceWordRules Rules(string lessonId, string wordId)
        {
            Curriculum curriculum = RealContent.Read();
            Step step = RealContent.Lesson(curriculum, lessonId).Steps
                .First(candidate => candidate.Kind == ActivityKind.TraceWord && candidate.Words[0].ToString() == wordId);
            return (TraceWordRules)ActivityRulesFactory.Create(step, curriculum, TestLearningRules.Default());
        }

        private static IActivityRules Create(Curriculum curriculum, string lessonId, ActivityKind kind, string form)
        {
            Step step = RealContent.Lesson(curriculum, lessonId).Steps
                .First(candidate => candidate.Kind == kind && candidate.Options.Any(option => option.Value == form));
            return ActivityRulesFactory.Create(step, curriculum, TestLearningRules.Default());
        }

        private static ActivityAnswer Stroke(TraceState state)
        {
            return ActivityAnswer.Traced(TestStrokes.Along(state.Figure.Strokes[state.Stroke].Path, 0f, 1f, 3f, 0f));
        }

        private static ActivityAnswer Line(TraceState state)
        {
            return ActivityAnswer.Traced(TestStrokes.Along(state.Figure.ChainFrom(state.Stroke), 0f, 1f, 3f, 0f));
        }

        private static string Fill(VoiceSequence sequence, VoiceSlot slot)
        {
            Assert.That(sequence.Items[0].TryGetFill(slot, out VoiceFill fill), Is.True, "no " + slot);
            return fill.Text;
        }
    }
}
