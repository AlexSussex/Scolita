using System.Linq;
using NUnit.Framework;
using Scolita.Editor.Voice;

namespace Scolita.Tests.EditMode.Voice
{
    /// <summary>Tests <see cref="TakeMatcher"/> with lengths only.</summary>
    public sealed class TakeMatcherTests
    {
        /// <summary>Tests that <see cref="TakeMatcher.Match"/> gives piece n to item n, with no flag, when the counts agree.</summary>
        [Test]
        public void TestMatch_CountsAgree()
        {
            TakeMatch match = TakeMatcher.Match(Expected(), Expected().Select(seconds => seconds * 1.1).ToArray());

            Assert.That(Enumerable.Range(0, match.Count).Select(match.SegmentOf), Is.EqualTo(Enumerable.Range(0, Expected().Length)));
            Assert.That(Enumerable.Range(0, match.Count).All(item => match.FlagsOf(item).Count == 0), Is.True);
        }

        /// <summary>Tests that <see cref="TakeMatcher.Match"/> flags a missed line where the lengths say, and never leaves a shifted item unflagged.</summary>
        [Test]
        public void TestMatch_OneMissed()
        {
            double[] pieces = Expected().Where((seconds, index) => index != 5).ToArray();

            TakeMatch match = TakeMatcher.Match(Expected(), pieces);

            Assert.That(match.FlagsOf(5), Does.Contain(ClipFlag.Missed));
            Assert.That(match.SegmentOf(5), Is.EqualTo(-1));
            Assert.That(match.SegmentOf(6), Is.EqualTo(5));
            Assert.That(Enumerable.Range(0, 5).All(item => match.FlagsOf(item).Count == 0), Is.True);
        }

        /// <summary>Tests that <see cref="TakeMatcher.Match"/> flags every possible place when the lengths can't tell where a line was missed.</summary>
        [Test]
        public void TestMatch_OneMissedAmongAlikeLines()
        {
            double[] alike = { 2.5, 0.7, 0.7, 0.7, 0.7, 0.7 };
            double[] pieces = { 2.5, 0.7, 0.7, 0.7, 0.7 };

            TakeMatch match = TakeMatcher.Match(alike, pieces);

            Assert.That(Enumerable.Range(1, 5).All(item => match.FlagsOf(item).Count > 0), Is.True);
            Assert.That(match.FlagsOf(0), Is.Empty);
        }

        /// <summary>Tests that <see cref="TakeMatcher.Match"/> flags a piece too many as a repeat, and every item when the counts are far off.</summary>
        [Test]
        public void TestMatch_OneExtraAndFarOff()
        {
            double[] withRepeat = Expected().Take(4).Append(Expected()[3]).Concat(Expected().Skip(4)).ToArray();

            TakeMatch extra = TakeMatcher.Match(Expected(), withRepeat);
            TakeMatch farOff = TakeMatcher.Match(Expected(), Expected().Take(4).ToArray());

            Assert.That(Enumerable.Range(0, extra.Count).Any(item => extra.FlagsOf(item).Contains(ClipFlag.Repeated)), Is.True);
            Assert.That(extra.SegmentOf(Expected().Length - 1), Is.EqualTo(withRepeat.Length - 1));
            Assert.That(Enumerable.Range(0, farOff.Count).All(item => farOff.FlagsOf(item).Count > 0), Is.True);
        }

        private static double[] Expected()
        {
            return new[] { 2.5, 0.7, 0.7, 1.4, 0.6, 2.8, 1.0, 0.5 };
        }
    }
}
