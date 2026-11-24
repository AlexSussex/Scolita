namespace Scolita.Core.Content
{
    /// <summary>
    /// The kinds of activity a lesson step can be (<c>docs/curriculum.md</c> §4.2). Written in
    /// <c>steps.csv</c> as its code (<see cref="ContentCodes"/>); what a step of each kind needs is in
    /// <see cref="ActivityKinds"/>.
    /// </summary>
    public enum ActivityKind
    {
        /// <summary>Meet a new letter: its sound, a picture, its print and handwritten forms.</summary>
        MeetLetter,

        /// <summary>Hear a sound or a word; tap the right one of 2 to 4 letters or pictures.</summary>
        ListenAndPick,

        /// <summary>Pop the stars that carry the letter, among two other letters.</summary>
        LetterHunt,

        /// <summary>A picture and its word; tap the letter it starts with.</summary>
        FirstSound,

        /// <summary>Trace a letter, in print or by hand.</summary>
        TraceLetter,

        /// <summary>Write a short word by hand, joined.</summary>
        TraceWord,

        /// <summary>Tap the drum (clap) once per syllable of a word heard.</summary>
        Syllables,

        /// <summary>Drag letters into the rocket's windows to spell a word heard.</summary>
        BuildWord,

        /// <summary>Read a word and tap its picture.</summary>
        ReadAndMatch,

        /// <summary>One letter, its two languages' sounds; sort pictures by the sound they start with.</summary>
        TwoSounds,

        /// <summary>Join the Romanian and English twins (and see the false friends).</summary>
        WordTwins
    }
}
