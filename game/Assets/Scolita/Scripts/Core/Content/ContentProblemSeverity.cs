namespace Scolita.Core.Content
{
    /// <summary>How much a <see cref="ContentProblem"/> matters.</summary>
    public enum ContentProblemSeverity
    {
        /// <summary>The content is wrong: the importer writes nothing and the validator fails.</summary>
        Error,

        /// <summary>
        /// Something the tablet covers with a fallback (a clip not recorded yet: the system voice; a
        /// picture with no art yet: its placeholder); reported, and the import goes on.
        /// </summary>
        Warning
    }
}
