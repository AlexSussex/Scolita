namespace Scolita.Core.Content
{
    /// <summary>What a kind of step needs its letters and words to be to each other (<see cref="ActivityKindRequirements.ItemRules"/>).</summary>
    public enum ActivityItemRules
    {
        /// <summary>Nothing beyond the counts.</summary>
        None,

        /// <summary>Letters or words, never both (<see cref="ActivityKind.ListenAndPick"/>: letters, or pictures).</summary>
        LettersOrWords,

        /// <summary>
        /// The word starts with one of the step's letters, so the step has a right answer
        /// (<see cref="ActivityKind.MeetLetter"/>'s picture word, <see cref="ActivityKind.FirstSound"/>).
        /// </summary>
        WordStartsWithALetter,

        /// <summary>
        /// One letter whose sound differs between the two languages, and words of both languages, each
        /// starting with it (<see cref="ActivityKind.TwoSounds"/>: she sorts them by their first sound).
        /// </summary>
        SortsByTheLettersTwoSounds
    }
}
