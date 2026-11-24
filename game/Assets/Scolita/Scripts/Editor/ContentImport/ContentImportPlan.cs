using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Scolita.Content;
using Scolita.Core.Content;
using Scolita.Core.Content.Csv;
using Scolita.Core.Content.Rules;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Scolita.Editor.ContentImport
{
    /// <summary>
    /// What importing the content would do: the content read from the CSVs and checked
    /// (<see cref="ContentTables"/>, <see cref="ContentRules"/>), every problem found, and, when there
    /// is no error, the assets and images it would write. <see cref="Apply"/> writes them (an existing
    /// asset is updated in place, so its GUID stays); <see cref="Differences"/> says what would change
    /// without writing (the freshness test).
    /// </summary>
    public sealed class ContentImportPlan
    {
        private const float ClipQuality = 0.7f;

        private readonly ContentFolder source;
        private readonly ContentAssetPaths paths;
        private readonly ContentProblems problems;
        private readonly Curriculum curriculum;
        private readonly IReadOnlyList<PlannedImage> images;
        private readonly IReadOnlyList<PlannedClip> clips;
        private readonly string filledVoiceLines;

        private ContentImportPlan(ContentFolder source, ContentAssetPaths paths, ContentProblems problems,
            Curriculum curriculum, IReadOnlyList<PlannedImage> images, IReadOnlyList<PlannedClip> clips,
            string filledVoiceLines)
        {
            this.source = source;
            this.paths = paths;
            this.problems = problems;
            this.curriculum = curriculum;
            this.images = images;
            this.clips = clips;
            this.filledVoiceLines = filledVoiceLines;
        }

        /// <summary>Every problem found, errors and warnings, in order.</summary>
        public IReadOnlyList<ContentProblem> Problems => problems.All;

        /// <summary>Whether any problem is an error (then nothing is written).</summary>
        public bool HasErrors => problems.HasErrors;

        /// <summary>
        /// Reads and checks the CSVs in <paramref name="source"/> and plans the assets under
        /// <paramref name="paths"/>; an asset there whose id is no longer in its CSV is an error (ids
        /// are retired, never deleted). Called by <see cref="ContentImporter"/> and the content tests.
        /// </summary>
        public static ContentImportPlan Make(ContentFolder source, ContentAssetPaths paths)
        {
            ContentProblems problems = new ContentProblems();
            Dictionary<string, string> csvs = source.ReadCsvs(problems);
            string filledVoiceLines = FilledVoiceLines(source, csvs);
            if (filledVoiceLines != null)
            {
                csvs[ContentFileNames.VoiceLines] = filledVoiceLines;
            }

            Curriculum curriculum = ContentTables.Read(csvs, problems);
            ContentRules.Check(curriculum, source, problems);
            if (problems.HasErrors)
            {
                return new ContentImportPlan(source, paths, problems, curriculum, new PlannedImage[0], new PlannedClip[0],
                    null);
            }

            CheckNothingDeleted(curriculum, paths, problems);
            CheckAssetTypes(curriculum, paths, problems);
            bool good = !problems.HasErrors;
            IReadOnlyList<PlannedImage> images = good ? PlanImages(source, paths, curriculum) : new PlannedImage[0];
            IReadOnlyList<PlannedClip> clips = good ? PlanClips(source, paths, curriculum) : new PlannedClip[0];
            return new ContentImportPlan(source, paths, problems, curriculum, images, clips, filledVoiceLines);
        }

        /// <summary>
        /// What <see cref="Apply"/> would change, one line per asset or image (empty when the assets
        /// match the CSVs); throws an <see cref="InvalidOperationException"/> for a plan with errors.
        /// Called by the freshness test.
        /// </summary>
        public IReadOnlyList<string> Differences()
        {
            ThrowIfErrors();
            List<string> differences = new List<string>();
            if (filledVoiceLines != null)
            {
                differences.Add("content/" + ContentFileNames.VoiceLines + "'s status column is out of date.");
            }

            differences.AddRange(clips.Select(ClipDifference).Where(difference => difference != null));
            differences.AddRange(StaleClips().Select(stale => stale + " is no longer named in the voice lines."));
            foreach (PlannedImage image in images)
            {
                string difference = ImageDifference(image);
                if (difference != null)
                {
                    differences.Add(difference);
                }
            }

            foreach (Func<IReadOnlyList<PlannedAsset>> stage in Stages())
            {
                IReadOnlyList<PlannedAsset> stagePlan = stage();
                try
                {
                    differences.AddRange(stagePlan.Select(AssetDifference).Where(difference => difference != null));
                }
                finally
                {
                    DestroyUnsaved(stagePlan);
                }
            }

            return differences;
        }

        /// <summary>
        /// Writes every planned image and asset, updating existing ones in place; throws an
        /// <see cref="InvalidOperationException"/> for a plan with errors. Called by
        /// <see cref="ContentImporter"/> and the content tests.
        /// </summary>
        public void Apply()
        {
            ThrowIfErrors();
            foreach (string folder in paths.OwnedFolders.Prepend(paths.Folder))
            {
                EnsureFolder(folder);
            }

            if (filledVoiceLines != null)
            {
                source.WriteCsv(ContentFileNames.VoiceLines, filledVoiceLines);
            }

            foreach (PlannedImage image in images)
            {
                WriteImage(image);
            }

            foreach (string stale in StaleClips())
            {
                AssetDatabase.DeleteAsset(stale);
            }

            if (clips.Count > 0)
            {
                EnsureFolder(paths.VoiceClipFolder);
            }

            foreach (PlannedClip clip in clips)
            {
                WriteClip(clip);
            }

            foreach (Func<IReadOnlyList<PlannedAsset>> stage in Stages())
            {
                IReadOnlyList<PlannedAsset> stagePlan = stage();
                try
                {
                    foreach (PlannedAsset planned in stagePlan)
                    {
                        WriteAsset(planned);
                    }
                }
                finally
                {
                    DestroyUnsaved(stagePlan);
                }

                AssetDatabase.SaveAssets();
            }
        }

        private void ThrowIfErrors()
        {
            if (problems.HasErrors)
            {
                throw new InvalidOperationException("The content has errors; nothing is written.");
            }
        }

        private IEnumerable<Func<IReadOnlyList<PlannedAsset>>> Stages()
        {
            yield return PlanItems;
            yield return PlanUnits;
            yield return PlanDomains;
            yield return PlanCatalogs;
        }

        private IReadOnlyList<PlannedAsset> PlanItems()
        {
            List<PlannedAsset> planned = new List<PlannedAsset>();
            foreach (Letter letter in curriculum.Letters.All)
            {
                LetterAsset asset = New<LetterAsset>(letter.Id);
                asset.Fill(letter);
                planned.Add(new PlannedAsset(paths.Letter(letter.Id.Value), asset));
            }

            foreach (Word word in curriculum.Words.All)
            {
                WordAsset asset = New<WordAsset>(word.Id);
                asset.Fill(word);
                planned.Add(new PlannedAsset(paths.Word(word.Id.Value), asset));
            }

            foreach (Picture picture in curriculum.Pictures.All)
            {
                PictureAsset asset = New<PictureAsset>(picture.Id);
                asset.Fill(picture, AssetDatabase.LoadAssetAtPath<Sprite>(paths.PictureImage(picture.Id.Value)));
                planned.Add(new PlannedAsset(paths.Picture(picture.Id.Value), asset));
            }

            foreach (VoiceLine line in curriculum.VoiceLines.All)
            {
                VoiceLineAsset asset = New<VoiceLineAsset>(line.Id);
                asset.Fill(line);
                asset.LinkClips(LinkedClips(line));
                planned.Add(new PlannedAsset(paths.VoiceLine(line.Id.Value), asset));
            }

            foreach (Lesson lesson in curriculum.AllLessons)
            {
                LessonAsset asset = New<LessonAsset>(lesson.Id);
                asset.Fill(lesson);
                planned.Add(new PlannedAsset(paths.Lesson(lesson.Id.Value), asset));
            }

            return planned;
        }

        private IReadOnlyList<PlannedAsset> PlanUnits()
        {
            List<PlannedAsset> planned = new List<PlannedAsset>();
            foreach (Unit unit in curriculum.Domains.SelectMany(domain => domain.Units))
            {
                UnitAsset asset = New<UnitAsset>(unit.Id);
                asset.Fill(unit, unit.Lessons.Select(lesson => Load<LessonAsset>(paths.Lesson(lesson.Id.Value))));
                planned.Add(new PlannedAsset(paths.Unit(unit.Id.Value), asset));
            }

            return planned;
        }

        private IReadOnlyList<PlannedAsset> PlanDomains()
        {
            List<PlannedAsset> planned = new List<PlannedAsset>();
            foreach (Domain domain in curriculum.Domains)
            {
                DomainAsset asset = New<DomainAsset>(domain.Id);
                asset.Fill(domain.Id.Value, domain.Units.Select(unit => Load<UnitAsset>(paths.Unit(unit.Id.Value))));
                planned.Add(new PlannedAsset(paths.Domain(domain.Id.Value), asset));
            }

            return planned;
        }

        private IReadOnlyList<PlannedAsset> PlanCatalogs()
        {
            return new[]
            {
                Catalog<LetterCatalog, LetterAsset>(curriculum.Letters.All.Select(letter => paths.Letter(letter.Id.Value))),
                Catalog<WordCatalog, WordAsset>(curriculum.Words.All.Select(word => paths.Word(word.Id.Value))),
                Catalog<PictureCatalog, PictureAsset>(curriculum.Pictures.All.Select(picture => paths.Picture(picture.Id.Value))),
                Catalog<VoiceLineCatalog, VoiceLineAsset>(curriculum.VoiceLines.All.Select(line => paths.VoiceLine(line.Id.Value))),
                Catalog<DomainCatalog, DomainAsset>(curriculum.Domains.Select(domain => paths.Domain(domain.Id.Value)))
            };
        }

        private PlannedAsset Catalog<TCatalog, TAsset>(IEnumerable<string> itemPaths)
            where TCatalog : ContentCatalog<TAsset>
            where TAsset : ScriptableObject
        {
            TCatalog catalog = ScriptableObject.CreateInstance<TCatalog>();
            catalog.name = typeof(TCatalog).Name;
            catalog.Fill(itemPaths.Select(Load<TAsset>));
            return new PlannedAsset(paths.Catalog(catalog.name), catalog);
        }

        private static TAsset New<TAsset>(ContentId id) where TAsset : ScriptableObject
        {
            TAsset asset = ScriptableObject.CreateInstance<TAsset>();
            asset.name = id.Value;
            return asset;
        }

        private static TAsset Load<TAsset>(string path) where TAsset : Object
        {
            return AssetDatabase.LoadAssetAtPath<TAsset>(path);
        }

        private static void CheckNothingDeleted(Curriculum curriculum, ContentAssetPaths paths, ContentProblems problems)
        {
            HashSet<string> planned = new HashSet<string>(PlannedPaths(curriculum, paths));
            foreach (string folder in paths.OwnedFolders)
            {
                string fullFolder = FullPath(folder);
                if (!Directory.Exists(fullFolder))
                {
                    continue;
                }

                IEnumerable<string> files = Directory.GetFiles(fullFolder)
                    .Where(file => file.EndsWith(".asset", StringComparison.Ordinal) || file.EndsWith(".png", StringComparison.Ordinal))
                    .Select(file => folder + "/" + Path.GetFileName(file));
                foreach (string stale in files.Where(file => !planned.Contains(file)))
                {
                    problems.Error(new ContentSource(stale, 0), "its id is no longer in the content's CSVs: ids are "
                        + "never deleted (her progress names them), so put its row back and mark it retired.");
                }
            }
        }

        private static void CheckAssetTypes(Curriculum curriculum, ContentAssetPaths paths, ContentProblems problems)
        {
            foreach (KeyValuePair<string, Type> planned in PlannedAssetTypes(curriculum, paths))
            {
                Type existing = AssetDatabase.GetMainAssetTypeAtPath(planned.Key);
                if (existing != null && existing != planned.Value)
                {
                    problems.Error(new ContentSource(planned.Key, 0), "it is a " + existing.Name + ", but the importer "
                        + "writes a " + planned.Value.Name + " there: move it out of the content folder.");
                }
            }
        }

        private static IEnumerable<KeyValuePair<string, Type>> PlannedAssetTypes(Curriculum curriculum,
            ContentAssetPaths paths)
        {
            return curriculum.Letters.All.Select(letter => Typed<LetterAsset>(paths.Letter(letter.Id.Value)))
                .Concat(curriculum.Words.All.Select(word => Typed<WordAsset>(paths.Word(word.Id.Value))))
                .Concat(curriculum.Pictures.All.Select(picture => Typed<PictureAsset>(paths.Picture(picture.Id.Value))))
                .Concat(curriculum.VoiceLines.All.Select(line => Typed<VoiceLineAsset>(paths.VoiceLine(line.Id.Value))))
                .Concat(curriculum.AllLessons.Select(lesson => Typed<LessonAsset>(paths.Lesson(lesson.Id.Value))))
                .Concat(curriculum.Domains.SelectMany(domain => domain.Units).Select(unit => Typed<UnitAsset>(paths.Unit(unit.Id.Value))))
                .Concat(curriculum.Domains.Select(domain => Typed<DomainAsset>(paths.Domain(domain.Id.Value))))
                .Append(Typed<LetterCatalog>(paths.Catalog(nameof(LetterCatalog))))
                .Append(Typed<WordCatalog>(paths.Catalog(nameof(WordCatalog))))
                .Append(Typed<PictureCatalog>(paths.Catalog(nameof(PictureCatalog))))
                .Append(Typed<VoiceLineCatalog>(paths.Catalog(nameof(VoiceLineCatalog))))
                .Append(Typed<DomainCatalog>(paths.Catalog(nameof(DomainCatalog))));
        }

        private static KeyValuePair<string, Type> Typed<TAsset>(string path)
        {
            return new KeyValuePair<string, Type>(path, typeof(TAsset));
        }

        private static IEnumerable<string> PlannedPaths(Curriculum curriculum, ContentAssetPaths paths)
        {
            return PlannedAssetTypes(curriculum, paths)
                .Select(planned => planned.Key)
                .Concat(curriculum.Pictures.All.Select(picture => paths.PictureImage(picture.Id.Value)));
        }

        private static IReadOnlyList<PlannedImage> PlanImages(ContentFolder source, ContentAssetPaths paths,
            Curriculum curriculum)
        {
            List<PlannedImage> planned = new List<PlannedImage>();
            foreach (Picture picture in curriculum.Pictures.All)
            {
                byte[] png = picture.HasArt
                    ? File.ReadAllBytes(source.ArtPath(picture.Art))
                    : PlaceholderPicture.Render(picture.Id.Value, PlaceholderLetter(curriculum, picture));
                planned.Add(new PlannedImage(paths.PictureImage(picture.Id.Value), png));
            }

            return planned;
        }

        private static string PlaceholderLetter(Curriculum curriculum, Picture picture)
        {
            Word word = curriculum.Words.All.FirstOrDefault(candidate => candidate.Picture == picture.Id);
            if (word == null || word.Text.Length == 0)
            {
                return string.Empty;
            }

            return word.Letters.Count > 0 && curriculum.Letters.TryGet(word.Letters[0], out Letter letter)
                ? letter.Lower.Substring(0, 1)
                : word.Text.Substring(0, 1).ToLowerInvariant();
        }

        private static string ImageDifference(PlannedImage image)
        {
            string fullPath = FullPath(image.Path);
            if (!File.Exists(fullPath))
            {
                return image.Path + " is missing.";
            }

            if (!File.ReadAllBytes(fullPath).SequenceEqual(image.Png))
            {
                return image.Path + " differs from its art or placeholder.";
            }

            return IsImportedAsSprite(AssetImporter.GetAtPath(image.Path) as TextureImporter)
                ? null
                : image.Path + " is not imported as a single sprite without mipmaps.";
        }

        private static string AssetDifference(PlannedAsset planned)
        {
            ScriptableObject existing = AssetDatabase.LoadAssetAtPath<ScriptableObject>(planned.Path);
            if (existing == null)
            {
                return planned.Path + " is missing.";
            }

            return EditorJsonUtility.ToJson(existing) == EditorJsonUtility.ToJson(planned.Desired)
                ? null
                : planned.Path + " differs from the CSVs.";
        }

        private static void WriteImage(PlannedImage image)
        {
            string fullPath = FullPath(image.Path);
            if (!File.Exists(fullPath) || !File.ReadAllBytes(fullPath).SequenceEqual(image.Png))
            {
                File.WriteAllBytes(fullPath, image.Png);
                AssetDatabase.ImportAsset(image.Path, ImportAssetOptions.ForceUpdate);
            }

            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(image.Path);
            if (!IsImportedAsSprite(importer))
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.mipmapEnabled = false;
                importer.alphaIsTransparency = true;
                importer.SaveAndReimport();
            }
        }

        private static bool IsImportedAsSprite(TextureImporter importer)
        {
            return importer != null
                && importer.textureType == TextureImporterType.Sprite
                && importer.spriteImportMode == SpriteImportMode.Single
                && !importer.mipmapEnabled
                && importer.alphaIsTransparency;
        }

        private static void WriteAsset(PlannedAsset planned)
        {
            ScriptableObject existing = AssetDatabase.LoadAssetAtPath<ScriptableObject>(planned.Path);
            if (existing == null)
            {
                AssetDatabase.CreateAsset(planned.Desired, planned.Path);
                return;
            }

            if (EditorJsonUtility.ToJson(existing) != EditorJsonUtility.ToJson(planned.Desired))
            {
                EditorUtility.CopySerialized(planned.Desired, existing);
                EditorUtility.SetDirty(existing);
            }
        }

        private static void DestroyUnsaved(IEnumerable<PlannedAsset> stagePlan)
        {
            foreach (PlannedAsset planned in stagePlan)
            {
                if (planned.Desired != null && !EditorUtility.IsPersistent(planned.Desired))
                {
                    Object.DestroyImmediate(planned.Desired);
                }
            }
        }

        private IReadOnlyList<AudioClip> LinkedClips(VoiceLine line)
        {
            if (line.Status != VoiceLineStatus.Recorded)
            {
                return new AudioClip[0];
            }

            AudioClip[] linked = line.Clips
                .Select(clip => AssetDatabase.LoadAssetAtPath<AudioClip>(paths.VoiceClip(clip)))
                .ToArray();
            return linked.Any(clip => clip == null) ? new AudioClip[0] : linked;
        }

        private static string FilledVoiceLines(ContentFolder source, Dictionary<string, string> csvs)
        {
            if (!csvs.TryGetValue(ContentFileNames.VoiceLines, out string text))
            {
                return null;
            }

            ContentProblems readProblems = new ContentProblems();
            Curriculum read = ContentTables.Read(csvs, readProblems);
            if (readProblems.HasErrors)
            {
                return null;
            }

            Dictionary<string, string> codes = VoiceLineStatuses.Of(read, source)
                .ToDictionary(status => status.Key.Value, status => ContentCodes.CodeOf(status.Value));
            string filled = CsvCellWriter.Write(ContentFileNames.VoiceLines, text, ContentTables.IdColumn,
                ContentTables.StatusColumn, ContentTables.ClipColumn, codes, readProblems);
            return filled == text ? null : filled;
        }

        private static IReadOnlyList<PlannedClip> PlanClips(ContentFolder source, ContentAssetPaths paths,
            Curriculum curriculum)
        {
            return curriculum.VoiceLines.All
                .Where(line => line.Clips.All(source.ClipExists))
                .SelectMany(line => line.Clips)
                .Distinct()
                .Select(clip => new PlannedClip(paths.VoiceClip(clip), source.ClipPath(clip)))
                .ToArray();
        }

        private IEnumerable<string> StaleClips()
        {
            string folder = FullPath(paths.VoiceClipFolder);
            if (!Directory.Exists(folder))
            {
                return Enumerable.Empty<string>();
            }

            HashSet<string> planned = new HashSet<string>(clips.Select(clip => clip.Path));
            return Directory.GetFiles(folder)
                .Where(file => !file.EndsWith(".meta", StringComparison.Ordinal))
                .Select(file => paths.VoiceClipFolder + "/" + Path.GetFileName(file))
                .Where(file => !planned.Contains(file))
                .ToArray();
        }

        private static string ClipDifference(PlannedClip clip)
        {
            string fullPath = FullPath(clip.Path);
            if (!File.Exists(fullPath))
            {
                return clip.Path + " is missing.";
            }

            if (!File.ReadAllBytes(fullPath).SequenceEqual(File.ReadAllBytes(clip.SourcePath)))
            {
                return clip.Path + " differs from its recording in content/voice/.";
            }

            return IsImportedAsVoice(AssetImporter.GetAtPath(clip.Path) as AudioImporter)
                ? null
                : clip.Path + " is not imported as a voice clip (mono, Vorbis, decompressed on load, not preloaded).";
        }

        private static void WriteClip(PlannedClip clip)
        {
            string fullPath = FullPath(clip.Path);
            byte[] bytes = File.ReadAllBytes(clip.SourcePath);
            if (!File.Exists(fullPath) || !File.ReadAllBytes(fullPath).SequenceEqual(bytes))
            {
                File.WriteAllBytes(fullPath, bytes);
                AssetDatabase.ImportAsset(clip.Path, ImportAssetOptions.ForceUpdate);
            }

            AudioImporter importer = (AudioImporter)AssetImporter.GetAtPath(clip.Path);
            if (!IsImportedAsVoice(importer))
            {
                importer.forceToMono = true;
                importer.loadInBackground = false;
                AudioImporterSampleSettings settings = importer.defaultSampleSettings;
                settings.loadType = AudioClipLoadType.DecompressOnLoad;
                settings.compressionFormat = AudioCompressionFormat.Vorbis;
                settings.quality = ClipQuality;
                settings.sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate;
                settings.preloadAudioData = false;
                importer.defaultSampleSettings = settings;
                importer.SaveAndReimport();
            }
        }

        private static bool IsImportedAsVoice(AudioImporter importer)
        {
            if (importer == null)
            {
                return false;
            }

            AudioImporterSampleSettings settings = importer.defaultSampleSettings;
            return importer.forceToMono
                && !importer.loadInBackground
                && settings.loadType == AudioClipLoadType.DecompressOnLoad
                && settings.compressionFormat == AudioCompressionFormat.Vorbis
                && Math.Abs(settings.quality - ClipQuality) < 0.001f
                && settings.sampleRateSetting == AudioSampleRateSetting.PreserveSampleRate
                && !settings.preloadAudioData;
        }

        private static void EnsureFolder(string folder)
        {
            if (AssetDatabase.IsValidFolder(folder))
            {
                return;
            }

            string parent = folder.Substring(0, folder.LastIndexOf('/'));
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, folder.Substring(parent.Length + 1));
        }

        private static string FullPath(string projectPath)
        {
            return Path.Combine(Directory.GetParent(Application.dataPath).FullName, projectPath);
        }
    }
}
