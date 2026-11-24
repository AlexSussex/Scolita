namespace Scolita.Core.Tracing
{
    /// <summary>Which form of a letter a shape draws (the <c>form</c> option of a <c>TraceLetter</c> step).</summary>
    public enum LetterForm
    {
        /// <summary>The print letter ("litera de tipar").</summary>
        Print,

        /// <summary>The handwritten letter ("litera de mână"), with its entry and exit strokes.</summary>
        Handwritten
    }
}
