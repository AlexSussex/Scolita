using System;
using Scolita.Core.Content;
using Scolita.Core.Progress;
using UnityEngine;

namespace Scolita.Game.Progress
{
    /// <summary>One item's right answers from one activity kind, in the <c>progress</c> save (<see cref="ProgressItemData"/>).</summary>
    [Serializable]
    public sealed class ProgressCountData
    {
        [SerializeField] private string activity;
        [SerializeField] private int count;

        /// <summary>The data of <paramref name="kindCount"/>; called by <see cref="ProgressItemData"/>.</summary>
        public ProgressCountData(KindCount kindCount)
        {
            activity = ContentCodes.CodeOf(kindCount.Kind);
            count = kindCount.Count;
        }

        /// <summary>
        /// Core's count; false for an activity kind this version doesn't know (dropped: a version that adds a kind
        /// raises the file's version). Called by <see cref="ProgressItemData"/>.
        /// </summary>
        public bool TryToCount(out KindCount kindCount)
        {
            bool known = ContentCodes.TryParseKind(activity, out ActivityKind kind);
            kindCount = new KindCount(kind, count);
            return known;
        }
    }
}
