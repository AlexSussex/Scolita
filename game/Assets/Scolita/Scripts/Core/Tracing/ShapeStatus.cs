namespace Scolita.Core.Tracing
{
    /// <summary>Whether a letter's shape has been checked against her school's manual (<see cref="LetterShape.Status"/>).</summary>
    public enum ShapeStatus
    {
        /// <summary>Drawn from the research's drafts, not checked yet: a warning at every import.</summary>
        Draft,

        /// <summary>Checked by the human against her manual.</summary>
        Checked
    }
}
