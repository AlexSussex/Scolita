using System;
using System.Linq;

namespace Scolita.Core.Content.Rules
{
    /// <summary>
    /// The rules on each picture in use: its art, if named, is a PNG in <c>content/pictures/</c>;
    /// one with no art yet is a warning (the importer draws its placeholder). Part of
    /// <see cref="ContentRules"/>.
    /// </summary>
    public static class PictureRules
    {
        private const string ArtExtension = ".png";

        /// <summary>Checks every picture that isn't retired; called by <see cref="ContentRules.Check"/>.</summary>
        public static void Check(Curriculum curriculum, IContentFiles files, ContentProblems problems)
        {
            foreach (Picture picture in curriculum.Pictures.All.Where(picture => !picture.Retired))
            {
                if (!picture.HasArt)
                {
                    problems.Warning(picture.Source, "the picture " + picture.Id
                        + " has no art yet: the importer draws its placeholder.");
                }
                else if (!IsPlainFileName(picture.Art))
                {
                    problems.Error(picture.Source, "the art " + picture.Art
                        + " must be a file name in content/pictures/, with no folder.");
                }
                else if (!picture.Art.EndsWith(ArtExtension, StringComparison.Ordinal))
                {
                    problems.Error(picture.Source, "the art " + picture.Art + " must be a " + ArtExtension + " file.");
                }
                else if (!files.ArtExists(picture.Art))
                {
                    problems.Error(picture.Source, "the art " + picture.Art + " is not in content/pictures/.");
                }
            }
        }

        private static bool IsPlainFileName(string fileName)
        {
            return fileName.IndexOfAny(new[] { '/', '\\', ':' }) < 0 && !fileName.Contains("..");
        }
    }
}
