using System.Collections.Generic;

namespace Scolita.Editor.Voice
{
    /// <summary>
    /// Which piece of a take each item of a session's script got, as <see cref="TakeMatcher"/> matched
    /// them, and the items whose place is in doubt (missed, repeated, uncertain).
    /// </summary>
    public sealed class TakeMatch
    {
        private readonly int[] segments;
        private readonly List<ClipFlag>[] flags;

        /// <summary>Creates a match for <paramref name="items"/> items, none matched yet; called by <see cref="TakeMatcher"/>.</summary>
        public TakeMatch(int items, string summary)
        {
            segments = new int[items];
            flags = new List<ClipFlag>[items];
            for (int index = 0; index < items; index++)
            {
                segments[index] = -1;
                flags[index] = new List<ClipFlag>();
            }

            Summary = summary;
        }

        /// <summary>How the counts compared, in words, for the report.</summary>
        public string Summary { get; }

        /// <summary>The number of items.</summary>
        public int Count => segments.Length;

        /// <summary>The piece item <paramref name="item"/> got, or -1 when it seems missed.</summary>
        public int SegmentOf(int item)
        {
            return segments[item];
        }

        /// <summary>What is in doubt about item <paramref name="item"/>'s place.</summary>
        public IReadOnlyList<ClipFlag> FlagsOf(int item)
        {
            return flags[item];
        }

        /// <summary>Gives item <paramref name="item"/> the piece <paramref name="segment"/>; called by <see cref="TakeMatcher"/>.</summary>
        public void Assign(int item, int segment)
        {
            segments[item] = segment;
        }

        /// <summary>Flags item <paramref name="item"/>, once per flag; called by <see cref="TakeMatcher"/>.</summary>
        public void Flag(int item, ClipFlag flag)
        {
            if (!flags[item].Contains(flag))
            {
                flags[item].Add(flag);
            }
        }
    }
}
