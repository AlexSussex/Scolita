namespace Scolita.Core.Content
{
    /// <summary>
    /// One thing wrong with the content, in words a person can act on, with where it is:
    /// <c>steps.csv, row 7: the word ro-mama uses m, which is not taught yet …</c>.
    /// </summary>
    public readonly struct ContentProblem
    {
        /// <summary>Creates a problem; called by <see cref="ContentProblems"/>.</summary>
        public ContentProblem(ContentSource source, string reason, ContentProblemSeverity severity)
        {
            Source = source;
            Reason = reason;
            Severity = severity;
        }

        /// <summary>The file and row the problem is in.</summary>
        public ContentSource Source { get; }

        /// <summary>What is wrong, and what to do about it.</summary>
        public string Reason { get; }

        /// <summary>Whether it stops the import, or is only a warning.</summary>
        public ContentProblemSeverity Severity { get; }

        /// <summary>Whether it is an <see cref="ContentProblemSeverity.Error"/>.</summary>
        public bool IsError => Severity == ContentProblemSeverity.Error;

        /// <summary><c>words.csv, row 7: …</c>, starting with <c>warning: </c> for a warning.</summary>
        public override string ToString()
        {
            string prefix = IsError ? string.Empty : "warning: ";
            return prefix + Source + ": " + Reason;
        }
    }
}
