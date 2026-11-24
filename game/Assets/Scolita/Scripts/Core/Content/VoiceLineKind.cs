namespace Scolita.Core.Content
{
    /// <summary>
    /// What a voice line says (<c>docs/curriculum.md</c> §8): the middle part of its id
    /// (<c>ro-sound-m</c>), written in <c>voice-lines.csv</c> as its code (<see cref="ContentCodes"/>).
    /// </summary>
    public enum VoiceLineKind
    {
        /// <summary>A letter's pure sound (/m/, never "muh").</summary>
        Sound,

        /// <summary>A letter's name in the alphabet ("me", "em").</summary>
        Name,

        /// <summary>A word, said normally.</summary>
        Word,

        /// <summary>A word said slowly, sound by sound (for <see cref="ActivityKind.BuildWord"/>).</summary>
        Slow,

        /// <summary>A word said in syllables or beats (for <see cref="ActivityKind.Syllables"/>).</summary>
        Syllables,

        /// <summary>Anything else the mascot says: instructions, hints, praise, checkpoints.</summary>
        Line
    }
}
