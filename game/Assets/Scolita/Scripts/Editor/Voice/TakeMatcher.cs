using System;
using System.Collections.Generic;
using System.Linq;

namespace Scolita.Editor.Voice
{
    /// <summary>
    /// Matches a take's pieces to its script's items, in order. When the counts agree, piece n is
    /// item n. When one piece is missing (a line skipped) or one too many (a repeat, a noise), every
    /// place it could be is tried, scored by how well each piece's length fits its item's expected
    /// length; the best guess is used, and every item whose place differs between the guesses that
    /// score nearly as well is flagged, so a miss is never shifted silently. With two or more pieces
    /// off, every item is flagged.
    /// </summary>
    public static class TakeMatcher
    {
        /// <summary>How much worse a guess may score and still count as possible (the sum of the log length ratios).</summary>
        public const double DoubtMargin = 0.7;

        private const double ShortestLength = 0.01;
        private const double MostLengthRatio = 3.0;

        /// <summary>
        /// Matches <paramref name="segmentSeconds"/> (each piece's length) to items expected to last
        /// <paramref name="expectedSeconds"/>. Called by the session import and <c>TakeMatcherTests</c>.
        /// </summary>
        public static TakeMatch Match(IReadOnlyList<double> expectedSeconds, IReadOnlyList<double> segmentSeconds)
        {
            int items = expectedSeconds.Count;
            int pieces = segmentSeconds.Count;
            if (pieces == items)
            {
                TakeMatch exact = new TakeMatch(items, pieces + " pieces for " + items + " lines: they match.");
                for (int item = 0; item < items; item++)
                {
                    exact.Assign(item, item);
                    if (Math.Abs(Math.Log(Math.Max(ShortestLength, segmentSeconds[item]) / Math.Max(ShortestLength, expectedSeconds[item])))
                        > Math.Log(MostLengthRatio))
                    {
                        exact.Flag(item, ClipFlag.Uncertain);
                    }
                }

                return exact;
            }

            if (pieces == items - 1)
            {
                return OneMissing(expectedSeconds, segmentSeconds);
            }

            if (pieces == items + 1)
            {
                return OneExtra(expectedSeconds, segmentSeconds);
            }

            return FarOff(items, pieces);
        }

        private static TakeMatch OneMissing(IReadOnlyList<double> expected, IReadOnlyList<double> pieces)
        {
            int items = expected.Count;
            double[] costs = Enumerable.Range(0, items)
                .Select(missed => Cost(expected, pieces, item => PieceIfMissed(item, missed)))
                .ToArray();
            int best = Array.IndexOf(costs, costs.Min());
            TakeMatch match = new TakeMatch(items, pieces.Count + " pieces for " + items
                + " lines: a line seems to be missed (most likely number " + best + "). Listen to the flagged ones.");
            for (int item = 0; item < items; item++)
            {
                match.Assign(item, PieceIfMissed(item, best));
            }

            match.Flag(best, ClipFlag.Missed);
            FlagDoubts(match, costs, best, 0);
            return match;
        }

        private static TakeMatch OneExtra(IReadOnlyList<double> expected, IReadOnlyList<double> pieces)
        {
            int items = expected.Count;
            double[] costs = Enumerable.Range(0, pieces.Count)
                .Select(extra => Cost(expected, pieces, item => PieceIfExtra(item, extra)))
                .ToArray();
            int best = Array.IndexOf(costs, costs.Min());
            int nearest = Math.Min(items - 1, best);
            TakeMatch match = new TakeMatch(items, pieces.Count + " pieces for " + items
                + " lines: there seems to be a piece too many (a repeat or a noise, most likely near number "
                + nearest + "). Listen to the flagged ones.");
            for (int item = 0; item < items; item++)
            {
                match.Assign(item, PieceIfExtra(item, best));
            }

            match.Flag(nearest, ClipFlag.Repeated);
            FlagDoubts(match, costs, best, -1);
            return match;
        }

        private static TakeMatch FarOff(int items, int pieces)
        {
            TakeMatch match = new TakeMatch(items, pieces + " pieces for " + items
                + " lines: too far off to match. Record the session again, with a pause of about two seconds after each line.");
            for (int item = 0; item < items; item++)
            {
                match.Assign(item, item < pieces ? item : -1);
                match.Flag(item, item < pieces ? ClipFlag.Uncertain : ClipFlag.Missed);
            }

            return match;
        }

        private static int PieceIfMissed(int item, int missed)
        {
            if (item == missed)
            {
                return -1;
            }

            return item < missed ? item : item - 1;
        }

        private static int PieceIfExtra(int item, int extra)
        {
            return item < extra ? item : item + 1;
        }

        private static void FlagDoubts(TakeMatch match, double[] costs, int best, int shift)
        {
            int[] possible = Enumerable.Range(0, costs.Length)
                .Where(place => costs[place] <= costs[best] + DoubtMargin)
                .ToArray();
            int from = Math.Max(0, possible.Min() + shift);
            int to = Math.Min(match.Count - 1, possible.Max());
            for (int item = from; item <= to; item++)
            {
                if (!match.FlagsOf(item).Contains(ClipFlag.Missed))
                {
                    match.Flag(item, ClipFlag.Uncertain);
                }
            }
        }

        private static double Cost(IReadOnlyList<double> expected, IReadOnlyList<double> pieces, Func<int, int> pieceOf)
        {
            double cost = 0.0;
            for (int item = 0; item < expected.Count; item++)
            {
                int piece = pieceOf(item);
                if (piece >= 0)
                {
                    cost += Math.Abs(Math.Log(Math.Max(ShortestLength, pieces[piece]) / Math.Max(ShortestLength, expected[item])));
                }
            }

            return cost;
        }
    }
}
