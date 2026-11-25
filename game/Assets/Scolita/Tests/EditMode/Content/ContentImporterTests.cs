using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Scolita.Content;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Editor;
using Scolita.Editor.ContentImport;
using Scolita.Tests.EditMode.Fakes;
using UnityEditor;

namespace Scolita.Tests.EditMode.Content
{
    /// <summary>
    /// Tests <see cref="ContentImporter.Import"/>, <see cref="ContentImportPlan"/> and
    /// <see cref="ReleasedIdsRecorder"/> on a copy of the test content (with an empty ledger of
    /// released ids unless the test gives one) in a temporary folder, imported into a temporary asset
    /// folder (both deleted after each test).
    /// </summary>
    public sealed class ContentImporterTests
    {
        /// <summary>
        /// Tests that importing a changed <c>words.csv</c> updates the word's asset in place: the same
        /// GUID, the new value, and the plan finds the assets fresh afterwards (the brief's acceptance).
        /// </summary>
        [Test]
        public void TestImport_UpdatesAnAssetInPlaceKeepingItsGuid()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            RunInTemporaryFolders(csvs, (source, paths) =>
            {
                ContentImportPlan first = ContentImporter.Import(source, paths);
                Assert.That(first.HasErrors, Is.False, string.Join("\n", first.Problems));
                string wordPath = paths.Word("ro-mama");
                string guid = AssetDatabase.AssetPathToGUID(wordPath);
                Assert.That(ContentImportPlan.Make(source, paths).Differences(), Is.Empty);

                ContentCsvs.ReplaceRow(csvs, ContentFileNames.Words, "ro-mama,", "ro-mama,ro,mama,m a m a,ma-ma,plane,,,,,");
                WriteCsvs(source.Root, csvs);
                Assert.That(ContentImportPlan.Make(source, paths).Differences(), Does.Contain(wordPath + " differs from the CSVs."));
                ContentImportPlan second = ContentImporter.Import(source, paths);

                Assert.That(second.HasErrors, Is.False);
                Assert.That(AssetDatabase.AssetPathToGUID(wordPath), Is.EqualTo(guid));
                Assert.That(EditorJsonUtility.ToJson(AssetDatabase.LoadAssetAtPath<WordAsset>(wordPath)), Does.Contain("\"picture\":\"plane\""));
                Assert.That(ContentImportPlan.Make(source, paths).Differences(), Is.Empty);
            });
        }

        /// <summary>
        /// Tests that <see cref="ContentImporter.Import"/> writes the whole tree: every item, the
        /// placeholders as sprites, and catalogs that build the same curriculum as the CSVs.
        /// </summary>
        [Test]
        public void TestImport_WritesItemsSpritesAndCatalogs()
        {
            RunInTemporaryFolders(ContentCsvs.Valid(), (source, paths) =>
            {
                ContentImporter.Import(source, paths);

                Curriculum fromAssets = CurriculumAssets.Build(
                    AssetDatabase.LoadAssetAtPath<DomainCatalog>(paths.Catalog("DomainCatalog")),
                    AssetDatabase.LoadAssetAtPath<LetterCatalog>(paths.Catalog("LetterCatalog")),
                    AssetDatabase.LoadAssetAtPath<WordCatalog>(paths.Catalog("WordCatalog")),
                    AssetDatabase.LoadAssetAtPath<PictureCatalog>(paths.Catalog("PictureCatalog")),
                    AssetDatabase.LoadAssetAtPath<VoiceLineCatalog>(paths.Catalog("VoiceLineCatalog")),
                    AssetDatabase.LoadAssetAtPath<LetterShapeCatalog>(paths.Catalog("LetterShapeCatalog")));
                Curriculum fromCsvs = ContentCsvs.Read(ContentCsvs.Valid());

                PictureAsset mother = AssetDatabase.LoadAssetAtPath<PictureAsset>(paths.Picture("mother"));
                Assert.That(mother.Sprite, Is.Not.Null);
                Assert.That(mother.Sprite.texture.width, Is.EqualTo(PlaceholderPicture.Size));
                Assert.That(fromAssets.Words.All.Select(word => word.Id), Is.EqualTo(fromCsvs.Words.All.Select(word => word.Id)));
                Assert.That(fromAssets.VoiceLines.All.Count, Is.EqualTo(fromCsvs.VoiceLines.All.Count));
                Assert.That(fromAssets.AllLessons.Select(lesson => lesson.Id), Is.EqualTo(fromCsvs.AllLessons.Select(lesson => lesson.Id)));
                Assert.That(fromAssets.AllLessons.SelectMany(lesson => lesson.Steps).Select(step => step.Kind),
                    Is.EqualTo(fromCsvs.AllLessons.SelectMany(lesson => lesson.Steps).Select(step => step.Kind)));
                Assert.That(string.Join(" ", fromAssets.Track(ContentId.Create("reading"), Language.English).Letters),
                    Is.EqualTo("a m t"));
            });
        }

        /// <summary>Tests that <see cref="ContentImportPlan.Make"/> refuses a deleted row (its asset stays) and writes nothing.</summary>
        [Test]
        public void TestMake_RefusesADeletedRow()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            RunInTemporaryFolders(csvs, (source, paths) =>
            {
                ContentImporter.Import(source, paths);
                csvs[ContentFileNames.Words] = string.Join("\n", csvs[ContentFileNames.Words].Split('\n')
                    .Where(row => !row.StartsWith("ro-am,", StringComparison.Ordinal)));
                csvs[ContentFileNames.VoiceLines] = string.Join("\n", csvs[ContentFileNames.VoiceLines].Split('\n')
                    .Where(row => !row.StartsWith("ro-word-am,", StringComparison.Ordinal)));
                WriteCsvs(source.Root, csvs);

                ContentImportPlan plan = ContentImporter.Import(source, paths);

                ContentCsvs.AssertError(plan.Problems.Where(problem => problem.IsError).ToArray(), paths.Word("ro-am"), 0,
                    "never deleted", "retired");
                Assert.That(AssetDatabase.LoadAssetAtPath<WordAsset>(paths.Word("ro-am")), Is.Not.Null);
            });
        }

        /// <summary>
        /// Tests that <see cref="ContentImportPlan.Apply"/> keeps a re-imported shape's asset (its GUID), and removes the
        /// asset of a shape whose file is gone (a shape is replaced, never retired).
        /// </summary>
        [Test]
        public void TestApply_ShapesKeepTheirGuidAndGoWithTheirFile()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            RunInTemporaryFolders(csvs, (source, paths) =>
            {
                ContentImporter.Import(source, paths);
                string shapePath = paths.Shape("a-lower-print");
                string guid = AssetDatabase.AssetPathToGUID(shapePath);
                ContentImporter.Import(source, paths);
                Assert.That(AssetDatabase.AssetPathToGUID(shapePath), Is.EqualTo(guid));

                File.Delete(Path.Combine(source.Root, "strokes", "a-lower-print.json"));
                csvs.Remove(ContentCsvs.ShapeFileName);
                csvs[ContentFileNames.Steps] = string.Join("\n", csvs[ContentFileNames.Steps].Split('\n')
                    .Where(row => !row.StartsWith("ro-a,TraceLetter,", StringComparison.Ordinal)));
                WriteCsvs(source.Root, csvs);
                ContentImportPlan plan = ContentImporter.Import(source, paths);

                Assert.That(plan.Problems.Where(problem => problem.IsError).Select(problem => problem.ToString()), Is.Empty);
                Assert.That(AssetDatabase.LoadAssetAtPath<LetterShapeAsset>(shapePath), Is.Null);
                Assert.That(plan.Differences(), Is.Empty);
            });
        }

        /// <summary>
        /// Tests that <see cref="ContentImportPlan.Make"/> refuses a released id (in the ledger) whose row
        /// was deleted together with its asset, and writes nothing.
        /// </summary>
        [Test]
        public void TestMake_RefusesAReleasedIdDeletedWithItsAsset()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            csvs[ContentFileNames.Words] = string.Join("\n", csvs[ContentFileNames.Words].Split('\n')
                .Where(row => !row.StartsWith("ro-am,", StringComparison.Ordinal)));
            csvs[ContentFileNames.VoiceLines] = string.Join("\n", csvs[ContentFileNames.VoiceLines].Split('\n')
                .Where(row => !row.StartsWith("ro-word-am,", StringComparison.Ordinal)));
            csvs[ReleasedIdLedger.FileName] = "# 0.2.0\nwords.csv ro-mama\nwords.csv ro-am\n";
            RunInTemporaryFolders(csvs, (source, paths) =>
            {
                ContentImportPlan plan = ContentImporter.Import(source, paths);

                ContentCsvs.AssertError(plan.Problems.Where(problem => problem.IsError).ToArray(), ContentFileNames.Words, 0,
                    "ro-am", "released", "retired");
                Assert.That(plan.Problems.Count(problem => problem.IsError), Is.EqualTo(1));
                Assert.That(AssetDatabase.IsValidFolder(paths.Folder), Is.False);
            });
        }

        /// <summary>Tests that <see cref="ContentImportPlan.Make"/> refuses content whose ledger of released ids is missing.</summary>
        [Test]
        public void TestMake_RefusesAMissingLedger()
        {
            RunInTemporaryFolders(ContentCsvs.Valid(), (source, paths) =>
            {
                File.Delete(Path.Combine(source.Root, ReleasedIdLedger.FileName));

                ContentImportPlan plan = ContentImporter.Import(source, paths);

                ContentCsvs.AssertError(plan.Problems.Where(problem => problem.IsError).ToArray(), ReleasedIdLedger.FileName, 0,
                    "missing");
                Assert.That(AssetDatabase.IsValidFolder(paths.Folder), Is.False);
            });
        }

        /// <summary>
        /// Tests that <see cref="ReleasedIdsRecorder.Record"/> adds the ids not in the ledger under the
        /// version, then nothing when run again, and that the content still imports with them released.
        /// </summary>
        [Test]
        public void TestRecord_AddsNewIdsUnderTheVersionOnce()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            csvs[ReleasedIdLedger.FileName] = "# the ledger\n\n# 0.2.0\nletters.csv a\n";
            RunInTemporaryFolders(csvs, (source, paths) =>
            {
                ReleasedIdsRecord first = ReleasedIdsRecorder.Record(source, paths, AppVersion.Parse("0.3.1"));
                string recorded = source.ReleasedIdsText(new ContentProblems());
                ReleasedIdsRecord second = ReleasedIdsRecorder.Record(source, paths, AppVersion.Parse("0.3.2"));

                Assert.That(first.HasErrors, Is.False, string.Join("\n", first.Problems));
                Assert.That(first.Added.Select(id => id.ToString()), Does.Not.Contain("letters.csv a"));
                Assert.That(first.Added.Select(id => id.ToString()), Does.Contain("letters.csv m").And.Contain("lessons.csv party"));
                Assert.That(recorded, Does.StartWith("# the ledger\n\n# 0.2.0\nletters.csv a\n\n# 0.3.1\nletters.csv m\n"));
                Assert.That(second.Added, Is.Empty);
                Assert.That(source.ReleasedIdsText(new ContentProblems()), Is.EqualTo(recorded));
                Assert.That(ContentImporter.Import(source, paths).HasErrors, Is.False);
            });
        }

        /// <summary>
        /// Tests that <see cref="ReleasedIdsRecorder.Record"/> refuses to add new ids under a version that
        /// is not after the ledger's latest release (<c>VERSION</c> not raised), leaving the ledger as it was.
        /// </summary>
        [Test]
        public void TestRecord_RefusesAVersionNotRaised()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            csvs[ReleasedIdLedger.FileName] = "# 0.3.1\nletters.csv a\n";
            RunInTemporaryFolders(csvs, (source, paths) =>
            {
                ReleasedIdsRecord record = ReleasedIdsRecorder.Record(source, paths, AppVersion.Parse("0.3.1"));

                ContentCsvs.AssertError(record.Problems, ReleasedIdLedger.FileName, 0, "0.3.1", "raise VERSION");
                Assert.That(record.Added, Is.Empty);
                Assert.That(source.ReleasedIdsText(new ContentProblems()), Is.EqualTo("# 0.3.1\nletters.csv a\n"));
            });
        }

        /// <summary>
        /// Tests that <see cref="ContentImportPlan.Make"/> checks the ledger for deleted ids only once the
        /// CSVs read without an error, so a row that failed to read is never also called deleted.
        /// </summary>
        [Test]
        public void TestMake_ChecksReleasedIdsOnlyWithoutCsvErrors()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            ContentCsvs.ReplaceRow(csvs, ContentFileNames.Pictures, "mat,", "Mat,,,");
            csvs[ReleasedIdLedger.FileName] = "# 0.2.0\npictures.csv mat\n";
            RunInTemporaryFolders(csvs, (source, paths) =>
            {
                ContentImportPlan plan = ContentImportPlan.Make(source, paths);

                Assert.That(plan.HasErrors, Is.True);
                Assert.That(plan.Problems.Where(problem => problem.Reason.Contains("was released")), Is.Empty);
            });
        }

        /// <summary>Tests that <see cref="ReleasedIdsRecorder.Record"/> records nothing when the content has an error.</summary>
        [Test]
        public void TestRecord_AnErrorRecordsNothing()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            ContentCsvs.AddRow(csvs, ContentFileNames.Pictures, "Moon,,,");
            csvs[ReleasedIdLedger.FileName] = "# 0.2.0\nletters.csv a\n";
            RunInTemporaryFolders(csvs, (source, paths) =>
            {
                ReleasedIdsRecord record = ReleasedIdsRecorder.Record(source, paths, AppVersion.Parse("0.3.1"));

                Assert.That(record.HasErrors, Is.True);
                Assert.That(record.Added, Is.Empty);
                Assert.That(source.ReleasedIdsText(new ContentProblems()), Is.EqualTo("# 0.2.0\nletters.csv a\n"));
            });
        }

        /// <summary>Tests that <see cref="ContentImporter.Import"/> writes nothing at all when the content has an error.</summary>
        [Test]
        public void TestImport_AnErrorWritesNothing()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            ContentCsvs.AddRow(csvs, ContentFileNames.Pictures, "Moon,,,");
            RunInTemporaryFolders(csvs, (source, paths) =>
            {
                ContentImportPlan plan = ContentImporter.Import(source, paths);

                Assert.That(plan.HasErrors, Is.True);
                Assert.That(AssetDatabase.IsValidFolder(paths.Folder), Is.False);
                Assert.That(() => plan.Apply(), Throws.InvalidOperationException);
            });
        }

        /// <summary>Tests that <see cref="ContentFolder.ReadTexts"/> reports a file saved as ANSI instead of misreading it, and reads a byte order mark.</summary>
        [Test]
        public void TestReadTexts_RefusesAnsiAndReadsByteOrderMark()
        {
            string root = Path.Combine(Path.GetTempPath(), "scolita-csv-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(root);
            try
            {
                File.WriteAllBytes(Path.Combine(root, ContentFileNames.Units), new byte[] { 0xEF, 0xBB, 0xBF, 0x69, 0x64, 0x0A });
                File.WriteAllBytes(Path.Combine(root, ContentFileNames.Pictures), new byte[] { 0x69, 0x64, 0x0A, 0xBA, 0x0A });
                ContentProblems problems = new ContentProblems();

                Dictionary<string, string> texts = new ContentFolder(root).ReadTexts(problems);

                Assert.That(texts.Keys, Is.EqualTo(new[] { ContentFileNames.Units }));
                Assert.That(texts[ContentFileNames.Units], Is.EqualTo("\uFEFFid\n"));
                Assert.That(problems.All.Single().ToString(), Does.StartWith("pictures.csv: the file is not UTF-8"));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        private static void RunInTemporaryFolders(Dictionary<string, string> csvs, Action<ContentFolder, ContentAssetPaths> test)
        {
            string suffix = Guid.NewGuid().ToString("N").Substring(0, 8);
            string sourceRoot = Path.Combine(Path.GetTempPath(), "scolita-content-" + suffix);
            ContentAssetPaths paths = new ContentAssetPaths("Assets/ScolitaImportTest-" + suffix);
            Directory.CreateDirectory(sourceRoot);
            try
            {
                File.WriteAllText(Path.Combine(sourceRoot, ReleasedIdLedger.FileName), "# nothing released yet\n");
                WriteCsvs(sourceRoot, csvs);
                test(new ContentFolder(sourceRoot), paths);
            }
            finally
            {
                AssetDatabase.DeleteAsset(paths.Folder);
                Directory.Delete(sourceRoot, true);
            }
        }

        private static void WriteCsvs(string root, Dictionary<string, string> csvs)
        {
            foreach (KeyValuePair<string, string> csv in csvs)
            {
                string path = Path.Combine(root, csv.Key);
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, csv.Value);
            }
        }
    }
}
