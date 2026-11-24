using System;
using System.Linq;
using Scolita.Core.Content;

namespace Scolita.Content
{
    /// <summary>
    /// Builds Core's <see cref="Curriculum"/> from the six catalogs at run time, through the same
    /// <see cref="CurriculumBuilder"/> the CSVs go through. The content was validated when it was
    /// imported, so a problem here is an impossible state: it throws.
    /// </summary>
    public static class CurriculumAssets
    {
        /// <summary>
        /// The curriculum of the catalogs; throws an <see cref="InvalidOperationException"/> naming
        /// every problem. Called by the scenes' composition roots (later briefs) and
        /// <c>CurriculumAssetsTests</c>.
        /// </summary>
        public static Curriculum Build(DomainCatalog domains, LetterCatalog letters, WordCatalog words,
            PictureCatalog pictures, VoiceLineCatalog voiceLines, LetterShapeCatalog shapes)
        {
            CurriculumBuilder builder = new CurriculumBuilder();
            foreach (LetterAsset letter in letters.Items)
            {
                builder.Add(letter.ToLetter());
            }

            foreach (WordAsset word in words.Items)
            {
                builder.Add(word.ToWord());
            }

            foreach (PictureAsset picture in pictures.Items)
            {
                builder.Add(picture.ToPicture());
            }

            foreach (VoiceLineAsset line in voiceLines.Items)
            {
                builder.Add(line.ToVoiceLine());
            }

            foreach (LetterShapeAsset shape in shapes.Items)
            {
                builder.Add(shape.ToShape());
            }

            foreach (DomainAsset domain in domains.Items)
            {
                domain.AddTo(builder);
            }

            ContentProblems problems = new ContentProblems();
            Curriculum curriculum = builder.Build(problems);
            if (problems.HasErrors)
            {
                throw new InvalidOperationException("The content assets don't make a curriculum (run "
                    + "\u0218coli\u021Ba \u2192 Import content): "
                    + string.Join(" ", problems.All.Select(problem => problem.ToString())));
            }

            return curriculum;
        }
    }
}
