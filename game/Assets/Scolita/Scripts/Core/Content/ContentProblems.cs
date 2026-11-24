using System.Collections.Generic;
using System.Linq;

namespace Scolita.Core.Content
{
    /// <summary>
    /// The problems found while reading and checking the content, in the order found. Filled by
    /// <see cref="Csv.CsvReader"/>, <see cref="ContentTables"/>, <see cref="CurriculumBuilder"/> and
    /// <see cref="Rules.ContentRules"/>.
    /// </summary>
    public sealed class ContentProblems
    {
        private readonly List<ContentProblem> problems = new List<ContentProblem>();

        /// <summary>Every problem found so far.</summary>
        public IReadOnlyList<ContentProblem> All => problems;

        /// <summary>Whether any problem is an error.</summary>
        public bool HasErrors => problems.Any(problem => problem.IsError);

        /// <summary>Adds an error at <paramref name="source"/>; called by every reader and rule.</summary>
        public void Error(ContentSource source, string reason)
        {
            problems.Add(new ContentProblem(source, reason, ContentProblemSeverity.Error));
        }

        /// <summary>Adds a warning at <paramref name="source"/>; called by the rules on clips and pictures.</summary>
        public void Warning(ContentSource source, string reason)
        {
            problems.Add(new ContentProblem(source, reason, ContentProblemSeverity.Warning));
        }
    }
}
