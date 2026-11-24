using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Scolita.Content;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Editor.ContentImport;
using UnityEditor;

namespace Scolita.Tests.EditMode.Content
{
    /// <summary>
    /// Tests <see cref="CurriculumAssets.Build"/> on the imported catalogs of the repository's content
    /// (planet 1), as the app will load it.
    /// </summary>
    public sealed class CurriculumAssetsTests
    {
        /// <summary>
        /// Tests that <see cref="CurriculumAssets.Build"/> makes planet 1 as the CSVs describe it: its
        /// lessons in teaching order, both languages' tracks, and the letters taught before its word
        /// lessons (<see cref="Curriculum.LettersTaughtBefore"/> with real content).
        /// </summary>
        [Test]
        public void TestBuild_PlanetOneFromTheCatalogs()
        {
            Curriculum curriculum = LoadCurriculum();

            Unit planet = curriculum.Domains.Single().Units.Single();
            curriculum.TryGetLesson(ContentId.Create("ro-words-1"), out Lesson romanianWords);
            curriculum.TryGetLesson(ContentId.Create("ro-m"), out Lesson romanianM);
            IReadOnlyList<ContentId> beforeWords = curriculum.LettersTaughtBefore(romanianWords, Language.Romanian);
            IReadOnlyList<ContentId> beforeM = curriculum.LettersTaughtBefore(romanianM, Language.English);

            Assert.That(planet.Id.Value, Is.EqualTo("planet-1"));
            Assert.That(planet.Lessons.Select(lesson => lesson.Id.Value), Is.EqualTo(new[]
            {
                "ro-a", "en-a", "ro-m", "en-m", "ro-t", "en-t", "ro-p", "en-p", "ro-words-1", "en-words-1", "capitals-1",
                "party-1"
            }));
            Assert.That(string.Join(" ", beforeWords), Is.EqualTo("a m t p"));
            Assert.That(string.Join(" ", beforeM), Is.EqualTo("a"));
            Assert.That(planet.Lessons[0].Steps.Select(step => step.Kind).Take(2),
                Is.EqualTo(new[] { ActivityKind.MeetLetter, ActivityKind.ListenAndPick }));
            Assert.That(curriculum.Words.Get(ContentId.Create("ro-mama")).Syllables, Is.EqualTo(new[] { "ma", "ma" }));
        }

        /// <summary>Tests that every letter and word of the catalogs has its voice lines, by the naming rule.</summary>
        [Test]
        public void TestBuild_EveryLetterAndWordHasItsLines()
        {
            Curriculum curriculum = LoadCurriculum();

            string[] missing = curriculum.Letters.All
                .SelectMany(letter => letter.Languages.Languages().SelectMany(language => new[] { letter.SoundLine(language), letter.NameLine(language) }))
                .Concat(curriculum.Words.All.Select(word => word.WordLine))
                .Where(line => !curriculum.VoiceLines.Contains(line.Id))
                .Select(line => line.ToString())
                .ToArray();

            Assert.That(missing, Is.Empty);
        }

        private static Curriculum LoadCurriculum()
        {
            ContentAssetPaths paths = new ContentAssetPaths(ContentImporter.AssetFolder);
            return CurriculumAssets.Build(
                AssetDatabase.LoadAssetAtPath<DomainCatalog>(paths.Catalog(nameof(DomainCatalog))),
                AssetDatabase.LoadAssetAtPath<LetterCatalog>(paths.Catalog(nameof(LetterCatalog))),
                AssetDatabase.LoadAssetAtPath<WordCatalog>(paths.Catalog(nameof(WordCatalog))),
                AssetDatabase.LoadAssetAtPath<PictureCatalog>(paths.Catalog(nameof(PictureCatalog))),
                AssetDatabase.LoadAssetAtPath<VoiceLineCatalog>(paths.Catalog(nameof(VoiceLineCatalog))));
        }
    }
}
