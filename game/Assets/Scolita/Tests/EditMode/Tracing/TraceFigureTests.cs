using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Tracing;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Tracing
{
    /// <summary>
    /// Tests <see cref="TraceFigure"/>, <see cref="JoinedWord"/> and <see cref="LineCheck"/> with planet 1's real
    /// shapes: what each stroke is, how a word's letters join, and a line through several strokes.
    /// </summary>
    public sealed class TraceFigureTests
    {
        /// <summary>
        /// Tests that <see cref="TraceFigure.OfLetter"/> reads the handwritten a as three strokes that continue each
        /// other, starting on the baseline, its line ending at the stem; and the print a as two that don't.
        /// </summary>
        [Test]
        public void TestOfLetter_TheHandwrittenAAndThePrintA()
        {
            Curriculum curriculum = RealContent.Read();

            TraceFigure byHand = TraceFigure.OfLetter(Shape(curriculum, "a-lower-handwritten"));
            TraceFigure print = TraceFigure.OfLetter(Shape(curriculum, "a-lower-print"));

            Assert.That(byHand.Strokes.Count, Is.EqualTo(3));
            Assert.That(byHand.Form, Is.EqualTo(LetterForm.Handwritten));
            Assert.That(byHand.Continues(1) && byHand.Continues(2), Is.True);
            Assert.That(byHand.EndsLine(2), Is.True);
            Assert.That(byHand.EndsLine(0) || byHand.EndsLine(1), Is.False);
            Assert.That(byHand.StartsOnTheBaseline, Is.True);
            Assert.That(byHand.ChainCount(0), Is.EqualTo(3));
            Assert.That(byHand.ChainEnd(0, 2), Is.EqualTo(byHand.ChainFrom(0).Length).Within(0.01f));
            Assert.That(print.Continues(1), Is.False);
            Assert.That(print.ChainCount(0), Is.EqualTo(1));
            Assert.That(print.StartsOnTheBaseline, Is.False);
        }

        /// <summary>Tests that <see cref="TraceFigure.OfLetter"/> makes t's bar a mark, after its line, and m start off the baseline.</summary>
        [Test]
        public void TestOfLetter_MarksAndTheMiddleLine()
        {
            Curriculum curriculum = RealContent.Read();

            TraceFigure t = TraceFigure.OfLetter(Shape(curriculum, "t-lower-handwritten"));
            TraceFigure m = TraceFigure.OfLetter(Shape(curriculum, "m-lower-handwritten"));

            Assert.That(t.IsMark(0), Is.False);
            Assert.That(t.IsMark(1), Is.True);
            Assert.That(t.EndsLine(0), Is.True);
            Assert.That(t.EndsLine(1), Is.False);
            Assert.That(m.StartsOnTheBaseline, Is.False);
        }

        /// <summary>
        /// Tests that <see cref="JoinedWord.Of"/> joins mama: each letter after the first starts where the one before
        /// left off, one line through all eight strokes, each letter's line ending at its exit.
        /// </summary>
        [Test]
        public void TestOf_Mama()
        {
            TraceFigure mama = Word(RealContent.Read(), "ro-mama");

            Assert.That(mama.LetterCount, Is.EqualTo(4));
            Assert.That(mama.Strokes.Count, Is.EqualTo(8));
            Assert.That(Enumerable.Range(1, 7).All(mama.Continues), Is.True);
            Assert.That(Enumerable.Range(0, 8).Select(mama.LetterOf), Is.EqualTo(new[] { 0, 1, 1, 1, 2, 3, 3, 3 }));
            Assert.That(Enumerable.Range(0, 8).Where(mama.EndsLine), Is.EqualTo(new[] { 0, 3, 4, 7 }));
            Assert.That(Enumerable.Range(0, 8).Where(mama.BeginsAtJoin), Is.EqualTo(new[] { 1, 4, 5 }));
            Assert.That(mama.ChainCount(0), Is.EqualTo(8));
            Assert.That(mama.Strokes[4].Path.Start.X, Is.GreaterThan(mama.Strokes[3].Path.Start.X));
            Assert.That(mama.MaxX - mama.MinX, Is.GreaterThan(300f));
        }

        /// <summary>
        /// Tests that <see cref="JoinedWord.Of"/> puts tata's two bars after the whole word, each a mark of its t, and
        /// that the a after a t keeps its entry, its first point moved onto the t's exit.
        /// </summary>
        [Test]
        public void TestOf_TataBarsComeLast()
        {
            Curriculum curriculum = RealContent.Read();
            TraceFigure tata = Word(curriculum, "ro-tata");
            LetterShape t = Shape(curriculum, "t-lower-handwritten");

            Assert.That(tata.Strokes.Count, Is.EqualTo(10));
            Assert.That(Enumerable.Range(0, 10).Where(tata.IsMark), Is.EqualTo(new[] { 8, 9 }));
            Assert.That(tata.LetterOf(8), Is.EqualTo(0));
            Assert.That(tata.LetterOf(9), Is.EqualTo(2));
            Assert.That(tata.Continues(8), Is.False);
            Assert.That(tata.Strokes[1].Path.Start.X, Is.EqualTo(t.Strokes[0].Path.End.X).Within(0.01f));
            Assert.That(tata.Strokes[1].Path.Start.Y, Is.EqualTo(t.Strokes[0].Path.End.Y).Within(0.01f));
            Assert.That(tata.Strokes[1].Path.EntryLength, Is.GreaterThan(0f));
        }

        /// <summary>
        /// Tests that <see cref="JoinedWord.Of"/> refuses a print shape and a letter before the last without an exit (the
        /// handwritten T), and takes one without an exit last.
        /// </summary>
        [Test]
        public void TestOf_RefusesWhatCantJoin()
        {
            Curriculum curriculum = RealContent.Read();
            LetterShape print = Shape(curriculum, "a-lower-print");
            LetterShape capital = Shape(curriculum, "t-upper-handwritten");
            LetterShape a = Shape(curriculum, "a-lower-handwritten");

            Assert.That(() => JoinedWord.Of(new[] { print }), Throws.ArgumentException);
            Assert.That(() => JoinedWord.Of(new[] { capital, a }), Throws.ArgumentException);
            Assert.That(() => JoinedWord.Of(new[] { a, capital }), Throws.Nothing);
            Assert.That(() => JoinedWord.Of(new LetterShape[0]), Throws.ArgumentException);
        }

        /// <summary>
        /// Tests that <see cref="LineCheck.Check"/> does all three strokes of the handwritten a in one line, keeps two
        /// when the line is lifted part-way down the stem, and tells a line begun at the round (no entry).
        /// </summary>
        [Test]
        public void TestCheck_ALineThroughTheHandwrittenA()
        {
            TraceFigure a = TraceFigure.OfLetter(Shape(RealContent.Read(), "a-lower-handwritten"));
            StrokeTolerance tolerance = TestLearningRules.Tracing().ForRound(2);
            StrokePath chain = a.ChainFrom(0);
            float stemHalf = a.ChainEnd(0, 1) + (a.ChainEnd(0, 2) - a.ChainEnd(0, 1)) * 0.4f;

            LineResult whole = LineCheck.Check(a, 0, TestStrokes.Along(chain, 0f, 1f, 4f, 0f), tolerance);
            LineResult lifted = LineCheck.Check(a, 0, TestStrokes.Along(chain, 0f, stemHalf / chain.Length, 4f, 0f), tolerance);
            LineResult noEntry = LineCheck.Check(a, 0, TestStrokes.Along(chain, chain.EntryLength / chain.Length, 1f, 4f, 0f), tolerance);
            LineResult backwards = LineCheck.Check(a, 0, TestStrokes.Along(chain, 1f, 0f, 4f, 0f), tolerance);

            Assert.That(whole.Verdict, Is.EqualTo(StrokeVerdict.Good));
            Assert.That(whole.StrokesDone, Is.EqualTo(3));
            Assert.That(whole.WentOn, Is.False);
            Assert.That(lifted.StrokesDone, Is.EqualTo(2));
            Assert.That(lifted.WentOn, Is.True);
            Assert.That(noEntry.Verdict, Is.EqualTo(StrokeVerdict.SkippedEntry));
            Assert.That(backwards.Verdict, Is.EqualTo(StrokeVerdict.WrongDirection));
        }

        /// <summary>
        /// Tests that <see cref="LineCheck.Check"/> judges a print stroke as <see cref="StrokeCheck.Check"/> does: one
        /// stroke at a time, even when her line goes on to the next.
        /// </summary>
        [Test]
        public void TestCheck_PrintIsJudgedStrokeByStroke()
        {
            TraceFigure a = TraceFigure.OfLetter(Shape(RealContent.Read(), "a-lower-print"));
            StrokeTolerance tolerance = TestLearningRules.Tracing().ForRound(0);
            TracePoints round = TestStrokes.Along(a.Strokes[0].Path, 0f, 1f, 4f, 0f);

            LineResult result = LineCheck.Check(a, 0, round, tolerance);

            Assert.That(result.Verdict, Is.EqualTo(StrokeCheck.Check(a.Strokes[0].Path, round, tolerance).Verdict));
            Assert.That(result.StrokesDone, Is.EqualTo(1));
            Assert.That(result.PointsKept, Is.EqualTo(round.Count));
        }

        private static LetterShape Shape(Curriculum curriculum, string id)
        {
            Assert.That(curriculum.Shapes.TryGet(ContentId.Create(id), out LetterShape shape), Is.True, "no shape " + id);
            return shape;
        }

        private static TraceFigure Word(Curriculum curriculum, string id)
        {
            Word word = curriculum.Words.Get(ContentId.Create(id));
            return JoinedWord.Of(word.Letters
                .Select(letter => Shape(curriculum, curriculum.Letters.Get(letter).Strokes + "-lower-handwritten"))
                .ToArray());
        }
    }
}
