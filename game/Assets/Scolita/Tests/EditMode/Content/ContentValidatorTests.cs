using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Scolita.Content;
using Scolita.Core.Content;
using Scolita.Core.Content.Rules;
using Scolita.Editor.ContentImport;
using UnityEditor;
using UnityEngine;

namespace Scolita.Tests.EditMode.Content
{
    /// <summary>
    /// The content validator: runs <see cref="IValidatableContent.Validate"/> on every
    /// ScriptableObject under <c>Assets/Scolita/Content</c>.
    /// </summary>
    public sealed class ContentValidatorTests
    {
        /// <summary>
        /// Tests that every ScriptableObject under the content folder implements
        /// <see cref="IValidatableContent"/>, so none escapes <see cref="IValidatableContent.Validate"/>.
        /// </summary>
        [Test]
        public void TestValidate_EveryContentAssetIsValidatable()
        {
            string[] notValidatable = LoadContentAssets()
                .Where(asset => !(asset is IValidatableContent))
                .Select(AssetDatabase.GetAssetPath)
                .ToArray();

            Assert.That(notValidatable, Is.Empty);
        }

        /// <summary>Tests that <see cref="IValidatableContent.Validate"/> finds no problem in any content asset.</summary>
        [Test]
        public void TestValidate_EveryContentAssetIsValid()
        {
            string[] problems = LoadContentAssets()
                .OfType<IValidatableContent>()
                .SelectMany(ProblemsOf)
                .ToArray();

            Assert.That(problems, Is.Empty);
        }

        /// <summary>
        /// Tests that <see cref="ContentRules.Check"/> finds no error in the repository's
        /// <c>content/*.csv</c>, and that its only warnings are pictures still on their placeholder
        /// and clips not recorded yet (the tablet covers both).
        /// </summary>
        [Test]
        public void TestCheck_TheContentCsvsHaveNoError()
        {
            ContentFolder folder = ContentImporter.RepositoryContent();
            ContentProblems problems = new ContentProblems();

            Curriculum curriculum = ContentTables.Read(folder.ReadCsvs(problems), problems);
            ContentRules.Check(curriculum, folder, problems);

            Assert.That(problems.All.Where(problem => problem.IsError).Select(problem => problem.ToString()), Is.Empty);
            Assert.That(problems.All
                .Where(problem => problem.Source.File != ContentFileNames.Pictures && problem.Source.File != ContentFileNames.VoiceLines)
                .Select(problem => problem.ToString()), Is.Empty);
        }

        private static IEnumerable<string> ProblemsOf(IValidatableContent content)
        {
            string path = AssetDatabase.GetAssetPath((Object)content);
            return content.Validate()
                .Select(problem => path + ": " + problem);
        }

        private static ScriptableObject[] LoadContentAssets()
        {
            const string contentFolder = ContentImporter.AssetFolder;
            Assert.That(AssetDatabase.IsValidFolder(contentFolder), Is.True, contentFolder + " must exist.");
            return AssetDatabase.FindAssets("t:ScriptableObject", new[] { contentFolder })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<ScriptableObject>)
                .ToArray();
        }
    }
}
