namespace Scolita.Core.Voice
{
    /// <summary>
    /// A part spliced into a voice line's text (<c>voice-lines.csv</c>: "Atinge litera {x}."), named
    /// in the text by its code (<see cref="VoiceSlots"/>).
    /// </summary>
    public enum VoiceSlot
    {
        /// <summary><c>{x}</c>: a letter, usually its sound.</summary>
        X,

        /// <summary><c>{y}</c>: a second letter (a join's next letter).</summary>
        Y,

        /// <summary><c>{word}</c>: a word.</summary>
        Word,

        /// <summary><c>{slow}</c>: a word said slowly, sound by sound.</summary>
        Slow,

        /// <summary><c>{stretched}</c>: a word with its first sound held.</summary>
        Stretched,

        /// <summary><c>{name}</c>: the profile's name, as text (nominative only).</summary>
        Name
    }
}
