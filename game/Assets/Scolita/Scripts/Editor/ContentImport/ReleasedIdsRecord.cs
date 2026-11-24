using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;

namespace Scolita.Editor.ContentImport
{
    /// <summary>What <see cref="ReleasedIdsRecorder.Record"/> did: the ids it added to the ledger, or the problems that stopped it.</summary>
    public readonly struct ReleasedIdsRecord
    {
        /// <summary>Creates a record; called by <see cref="ReleasedIdsRecorder"/>.</summary>
        public ReleasedIdsRecord(IReadOnlyList<ContentProblem> problems, IReadOnlyList<ReleasedId> added)
        {
            Problems = problems;
            Added = added;
        }

        /// <summary>Every problem the content's import found, errors and warnings.</summary>
        public IReadOnlyList<ContentProblem> Problems { get; }

        /// <summary>The ids added to the ledger; none when there was an error or nothing new.</summary>
        public IReadOnlyList<ReleasedId> Added { get; }

        /// <summary>Whether an error stopped it (then the ledger is unchanged).</summary>
        public bool HasErrors => Problems.Any(problem => problem.IsError);
    }
}
