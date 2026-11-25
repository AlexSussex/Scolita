using Scolita.Core.Content;
using Scolita.Core.Progress;

namespace Scolita.Core.Parent
{
    /// <summary>One letter of the parent area's grid and how far she is with it in its row's language.</summary>
    public sealed class LetterCell
    {
        /// <summary>The cell of <paramref name="letter"/> in <paramref name="state"/>; called by <see cref="ParentReport"/>.</summary>
        public LetterCell(Letter letter, ItemState state)
        {
            Letter = letter;
            State = state;
        }

        /// <summary>The letter.</summary>
        public Letter Letter { get; }

        /// <summary>New, learning or learned.</summary>
        public ItemState State { get; }
    }
}
