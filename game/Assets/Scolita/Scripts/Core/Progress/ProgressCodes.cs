using System;

namespace Scolita.Core.Progress
{
    /// <summary>
    /// The stable codes the <c>progress</c> save file writes for <see cref="ItemKind"/> (<c>letter</c>) and
    /// <see cref="ItemState"/> (<c>learned</c>), so renaming an enum value never changes a file. Activity
    /// kinds use <c>ContentCodes</c>, languages <c>LanguageCodes</c>, days <c>CalendarDay</c>'s ISO form.
    /// </summary>
    public static class ProgressCodes
    {
        private const string LetterCode = "letter";
        private const string WordCode = "word";
        private const string LearningCode = "learning";
        private const string LearnedCode = "learned";

        /// <summary>The code of <paramref name="kind"/>; called by the save file, <see cref="LearningItem"/> and the tests.</summary>
        public static string CodeOf(ItemKind kind)
        {
            switch (kind)
            {
                case ItemKind.Letter:
                    return LetterCode;
                case ItemKind.Word:
                    return WordCode;
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, "No code for this item kind.");
            }
        }

        /// <summary>
        /// The code of <paramref name="state"/>; throws an <see cref="ArgumentOutOfRangeException"/> for
        /// <see cref="ItemState.New"/>, which is never saved (a new item has no progress). Called by the save
        /// file and the tests.
        /// </summary>
        public static string CodeOf(ItemState state)
        {
            switch (state)
            {
                case ItemState.Learning:
                    return LearningCode;
                case ItemState.Learned:
                    return LearnedCode;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, "No code for this item state.");
            }
        }

        /// <summary>Reads an item kind from its exact code; called by the save file and the tests.</summary>
        public static bool TryParseKind(string code, out ItemKind kind)
        {
            switch (code)
            {
                case LetterCode:
                    kind = ItemKind.Letter;
                    return true;
                case WordCode:
                    kind = ItemKind.Word;
                    return true;
                default:
                    kind = default;
                    return false;
            }
        }

        /// <summary>Reads a saved item state (learning or learned) from its exact code; called by the save file and the tests.</summary>
        public static bool TryParseState(string code, out ItemState state)
        {
            switch (code)
            {
                case LearningCode:
                    state = ItemState.Learning;
                    return true;
                case LearnedCode:
                    state = ItemState.Learned;
                    return true;
                default:
                    state = ItemState.New;
                    return false;
            }
        }
    }
}
