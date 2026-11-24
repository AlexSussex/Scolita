namespace Scolita.Core.Voice
{
    /// <summary>
    /// One piece of a voice line's text, as <see cref="VoiceTemplate"/> splits it: a text piece
    /// (spoken when it holds a letter or a digit, else punctuation only), or a slot to splice into.
    /// </summary>
    public readonly struct VoiceTemplatePiece
    {
        /// <summary>Creates a piece; called by <see cref="VoiceTemplate"/>.</summary>
        public VoiceTemplatePiece(string text, bool isSlot, VoiceSlot slot, int spokenIndex)
        {
            Text = text;
            IsSlot = isSlot;
            Slot = slot;
            SpokenIndex = spokenIndex;
        }

        /// <summary>The piece's text as written (its spaces kept); empty for a slot.</summary>
        public string Text { get; }

        /// <summary>Whether the piece is a slot.</summary>
        public bool IsSlot { get; }

        /// <summary>The slot, when <see cref="IsSlot"/>.</summary>
        public VoiceSlot Slot { get; }

        /// <summary>
        /// The piece's place among the line's spoken segments (0, 1, …), which is also its clip's
        /// place in the line's <c>clip</c> cell; -1 for a slot or a piece of punctuation only.
        /// </summary>
        public int SpokenIndex { get; }

        /// <summary>Whether the piece is text that is said (it holds a letter or a digit).</summary>
        public bool IsSpoken => SpokenIndex >= 0;

        /// <summary>Whether the piece is punctuation holding an ellipsis, said as a short pause.</summary>
        public bool IsPause => !IsSlot && !IsSpoken && Text.IndexOf('\u2026') >= 0;
    }
}
