using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Scolita.Core.Content;
using UnityEditor;
using UnityEngine;

namespace Scolita.Editor.ContentImport
{
    /// <summary>
    /// Adds the content's new ids to the ledger of released ids (<see cref="ReleasedIdLedger"/>) under
    /// the version in <c>VERSION</c>: the close-out's step, after raising <c>VERSION</c>
    /// (<c>tasks/README.md</c> → "Lifecycle"). Run from the Editor's menu (Școlița → Record released
    /// ids) or in batch mode with
    /// <c>-executeMethod Scolita.Editor.ContentImport.ReleasedIdsRecorder.RecordFromCommandLine</c>.
    /// </summary>
    public static class ReleasedIdsRecorder
    {
        private const string VersionFileName = "VERSION";
        private const int SuccessExitCode = 0;
        private const int FailureExitCode = 1;

        /// <summary>
        /// Checks the content in <paramref name="source"/> as an import into <paramref name="paths"/>
        /// would (<see cref="ContentImportPlan.Make"/>) and, with no error, adds the ids not in the
        /// ledger yet under <paramref name="version"/>; with an error, or nothing new, the ledger is
        /// unchanged. New ids under a version no later than the ledger's latest release (<c>VERSION</c>
        /// not raised yet) are an error. Called by the menu, the command line and <c>ContentImporterTests</c>.
        /// </summary>
        public static ReleasedIdsRecord Record(ContentFolder source, ContentAssetPaths paths, AppVersion version)
        {
            ContentImportPlan plan = ContentImportPlan.Make(source, paths);
            if (plan.HasErrors)
            {
                return new ReleasedIdsRecord(plan.Problems, new ReleasedId[0]);
            }

            ReleasedIdLedger ledger = plan.Ledger;
            IReadOnlyList<ReleasedId> added = ledger.NewIds(plan.Curriculum, plan.Rewards);
            if (added.Count == 0)
            {
                return new ReleasedIdsRecord(plan.Problems, added);
            }

            AppVersion? latest = ledger.LatestRelease;
            if (latest != null && version.VersionCode <= latest.Value.VersionCode)
            {
                ContentProblem stale = new ContentProblem(new ContentSource(ReleasedIdLedger.FileName, 0), "the "
                    + "version " + version + " is not after the ledger's latest release, " + latest.Value
                    + ": raise VERSION first (to the next free number), then record.", ContentProblemSeverity.Error);
                return new ReleasedIdsRecord(plan.Problems.Append(stale).ToArray(), new ReleasedId[0]);
            }

            source.WriteReleasedIds(ledger.WithIds(version.ToString(), added));
            return new ReleasedIdsRecord(plan.Problems, added);
        }

        /// <summary>Records the released ids from the Editor's menu, logging every problem and showing the outcome.</summary>
        [MenuItem("\u0218coli\u021Ba/Record released ids")]
        public static void RecordFromMenu()
        {
            try
            {
                ReleasedIdsRecord record = RecordAndLog();
                EditorUtility.DisplayDialog("\u0218coli\u021Ba", Summary(record), "OK");
            }
            catch (Exception exception) when (exception is FormatException || exception is IOException)
            {
                Debug.LogError("Scolita released ids: " + exception.Message);
                EditorUtility.DisplayDialog("\u0218coli\u021Ba", exception.Message, "OK");
            }
        }

        /// <summary>
        /// Records the released ids in batch mode, logging every problem, and exits with 0, or 1 on any
        /// error (or any exception). Called from the command line with <c>-executeMethod</c>.
        /// </summary>
        public static void RecordFromCommandLine()
        {
            try
            {
                ReleasedIdsRecord record = RecordAndLog();
                EditorApplication.Exit(record.HasErrors ? FailureExitCode : SuccessExitCode);
            }
            catch (Exception exception)
            {
                Debug.LogError("Scolita released ids failed: " + exception.Message);
                Debug.LogException(exception);
                EditorApplication.Exit(FailureExitCode);
            }
        }

        private static ReleasedIdsRecord RecordAndLog()
        {
            ContentFolder source = ContentImporter.RepositoryContent();
            AppVersion version = AppVersion.ReadFile(Path.Combine(Directory.GetParent(source.Root).FullName, VersionFileName));
            ReleasedIdsRecord record = Record(source, new ContentAssetPaths(ContentImporter.AssetFolder), version);
            foreach (ContentProblem problem in record.Problems)
            {
                if (problem.IsError)
                {
                    Debug.LogError("Content: " + problem);
                }
            }

            Debug.Log("Scolita released ids: " + Summary(record));
            return record;
        }

        private static string Summary(ReleasedIdsRecord record)
        {
            if (record.HasErrors)
            {
                return "nothing was recorded: the Console lists why.";
            }

            return record.Added.Count == 0
                ? "every id is in the ledger already; nothing was added."
                : record.Added.Count + " new ids added to content/" + ReleasedIdLedger.FileName + ".";
        }
    }
}
