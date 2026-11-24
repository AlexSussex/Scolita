namespace Scolita.Core.Content
{
    /// <summary>
    /// How a letter stands between the two alphabets (<c>docs/curriculum.md</c> §2). It also gives
    /// the languages the letter belongs to (<see cref="Letter.Languages"/>). Written in
    /// <c>letters.csv</c> as its code (<see cref="ContentCodes"/>).
    /// </summary>
    public enum LetterGroup
    {
        /// <summary>In both alphabets with almost the same sound (b d f l m n p s t v z k).</summary>
        SharedClose,

        /// <summary>In both alphabets, a vowel whose sound differs (a e i o u).</summary>
        VowelDiffers,

        /// <summary>In both alphabets, a sound that differs (c g h j r w x y).</summary>
        SoundDiffers,

        /// <summary>Romanian only (ă â î ș ț, and the Romanian groups such as ce, chi).</summary>
        RomanianOnly,

        /// <summary>English only (the English groups such as sh, th, ee).</summary>
        EnglishOnly
    }
}
