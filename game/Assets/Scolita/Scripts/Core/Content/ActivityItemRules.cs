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
        SortsByTheLettersTwoSounds,

        /// <summary>
        /// The word has 1 to <see cref="ActivityLimits.MostSyllables"/> syllables (<see cref="ActivityKind.Syllables"/>:
        /// she taps the drum once for each).
        /// </summary>
        CountsSyllables,

        /// <summary>
        /// The word has at most <see cref="ActivityLimits.MostWindows"/> letters, and the step's letters (the
        /// extra tiles) are not in it (<see cref="ActivityKind.BuildWord"/>).
        /// </summary>
        SpellsTheWord,

        /// <summary>
        /// Each word once, and no two words with the same picture, so one picture is right
        /// (<see cref="ActivityKind.ReadAndMatch"/>).
        /// </summary>
        PicturesDiffer,

        /// <summary>
        /// The words pair up, one to one: each one's twin or false friend (never both) is in the step, a false friend
        /// is one she can read, each word is listed once, and there are as many Romanian words as English
        /// (<see cref="ActivityKind.WordTwins"/>; at most <see cref="ActivityLimits.MostPairs"/> pairs, by its count of words).
        /// </summary>
        PairsUp
    }
}
