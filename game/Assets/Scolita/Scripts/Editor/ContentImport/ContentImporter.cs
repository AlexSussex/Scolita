using System;
using System.IO;
using System.Linq;
using Scolita.Core.Content;
using UnityEditor;
using UnityEngine;

namespace Scolita.Editor.ContentImport
{
    /// <summary>
    /// The content importer: reads <c>content/*.csv</c>, checks it, and writes the content assets and
    /// catalogs under <c>Assets/Scolita/Content</c> (<see cref="ContentImportPlan"/>), nothing if there
    /// is an error. Run from the Editor's menu (Școlița → Import content) or in batch mode with
    /// <c>-executeMethod Scolita.Editor.ContentImport.ContentImporter.ImportFromCommandLine</c>.
    /// </summary>
    public static class ContentImporter
    {
        /// <summary>The project path of the imported content.</summary>
        public const string AssetFolder = "Assets/Scolita/Content";

        private const string ContentFolderName = "content";
        private const int SuccessExitCode = 0;
        private const int FailureExitCode = 1;

        /// <summary>The repository's <c>content/</c> folder; called by the importer and the content tests.</summary>
        public static ContentFolder RepositoryContent()
        {
            string repositoryRoot = Directory.GetParent(Application.dataPath).Parent.FullName;
            return new ContentFolder(Path.Combine(repositoryRoot, ContentFolderName));
        }

        /// <summary>
        /// Imports the repository's content into <see cref="AssetFolder"/> and returns the plan, with
        /// its problems; writes nothing when there is an error. Called by the menu, the command line
        /// and the content tests.
        /// </summary>
        public static ContentImportPlan Import(ContentFolder source, ContentAssetPaths paths)
        {
            ContentImportPlan plan = ContentImportPlan.Make(source, paths);
            if (!plan.HasErrors)
            {
                plan.Apply();
            }

            return plan;
        }

        /// <summary>Imports from the Editor's menu, logging every problem and showing the outcome.</summary>
        [MenuItem("\u0218coli\u021Ba/Import content")]
        public static void ImportFromMenu()
        {
            ContentImportPlan plan = ImportAndLog();
            EditorUtility.DisplayDialog("\u0218coli\u021Ba", Summary(plan), "OK");
        }

        /// <summary>
        /// Imports in batch mode, logging every problem, and exits with 0, or 1 on any error (or any
        /// exception). Called from the command line with <c>-executeMethod</c>.
        /// </summary>
        public static void ImportFromCommandLine()
        {
            try
            {
                ContentImportPlan plan = ImportAndLog();
                EditorApplication.Exit(plan.HasErrors ? FailureExitCode : SuccessExitCode);
            }
            catch (Exception exception)
            {
                Debug.LogError("Scolita content import failed: " + exception.Message);
                Debug.LogException(exception);
                EditorApplication.Exit(FailureExitCode);
            }
        }

        private static ContentImportPlan ImportAndLog()
        {
            ContentImportPlan plan = Import(RepositoryContent(), new ContentAssetPaths(AssetFolder));
            foreach (ContentProblem problem in plan.Problems)
            {
                if (problem.IsError)
                {
                    Debug.LogError("Content: " + problem);
                }
                else
                {
                    Debug.LogWarning("Content: " + problem);
                }
            }

            Debug.Log("Scolita content import: " + Summary(plan));
            return plan;
        }

        private static string Summary(ContentImportPlan plan)
        {
            int errors = plan.Problems.Count(problem => problem.IsError);
            int warnings = plan.Problems.Count - errors;
            return errors > 0
                ? errors + " errors, " + warnings + " warnings: nothing was imported (the Console lists them)."
                : "imported, with " + warnings + " warnings (the Console lists them).";
        }
    }
}
