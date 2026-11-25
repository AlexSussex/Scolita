using System.Collections.Generic;
using Scolita.Core.Languages;

namespace Scolita.Core.Parent
{
    /// <summary>One language's row of the parent area's grid: its track's letters, in teaching order.</summary>
    public sealed class LetterRow
    {
        /// <summary>The row of <paramref name="language"/>; called by <see cref="ParentReport"/>.</summary>
        public LetterRow(Language language, IReadOnlyList<LetterCell> cells)
        {
            Language = language;
            Cells = cells;
        }

        /// <summary>The row's language.</summary>
        public Language Language { get; }

        /// <summary>The track's letters, each with its state.</summary>
        public IReadOnlyList<LetterCell> Cells { get; }
    }
}
