namespace Scolita.Core.Content.Rules
{
    /// <summary>
    /// The content validator's rules on a <see cref="Curriculum"/> read by
    /// <see cref="ContentTables"/>: the words, the pictures, the voice lines, the lessons, the
    /// steps and the letters' shapes (AGENTS.md → "Learning"; the brief <c>content-model</c>). Each problem names its file,
    /// its row and the reason.
    /// </summary>
    public static class ContentRules
    {
        /// <summary>
        /// Checks <paramref name="curriculum"/>, with <paramref name="files"/> saying which art and
        /// clip files exist, into <paramref name="problems"/>. Called by the importer, the content
        /// validator test and <c>ContentRulesTests</c>.
        /// </summary>
        public static void Check(Curriculum curriculum, IContentFiles files, ContentProblems problems)
        {
            WordRules.Check(curriculum, problems);
            PictureRules.Check(curriculum, files, problems);
            VoiceLineRules.Check(curriculum, files, problems);
            LessonRules.Check(curriculum, problems);
            StepRules.Check(curriculum, problems);
            ShapeRules.Check(curriculum, problems);
        }
    }
}
