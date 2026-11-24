using System;
using System.Linq;
using Scolita.Core.Languages;

namespace Scolita.Core.Content.Rules
{
    /// <summary>
    /// The rules on the voice lines: each id matches its language and kind; a named clip is in
    /// <c>content/voice/</c> (a warning: the system voice covers it); every letter has its sound and
    /// name in each of its languages; every word has its word line, saying the word, and its syllables
    /// line, if it has one, says its syllables. Part of
    /// <see cref="ContentRules"/>; a step's lines are checked by <see cref="StepRules"/>.
    /// </summary>
    public static class VoiceLineRules
    {
        /// <summary>Checks the voice lines and the lines of every letter and word in use; called by <see cref="ContentRules.Check"/>.</summary>
        public static void Check(Curriculum curriculum, IContentFiles files, ContentProblems problems)
        {
            foreach (VoiceLine line in curriculum.VoiceLines.All.Where(line => !line.Retired))
            {
                CheckLine(line, files, problems);
            }

            foreach (Letter letter in curriculum.Letters.All.Where(letter => !letter.Retired))
            {
                foreach (Language language in letter.Languages.Languages())
                {
                    Line(curriculum, letter.SoundLine(language), letter.Source, problems);
                    Line(curriculum, letter.NameLine(language), letter.Source, problems);
                }
            }

            foreach (Word word in curriculum.Words.All.Where(word => !word.Retired))
            {
                VoiceLine line = Line(curriculum, word.WordLine, word.Source, problems);
                if (line != null && !string.Equals(line.Text, word.Text, StringComparison.Ordinal))
                {
                    problems.Error(word.Source, "the voice line " + line.Id + " says '" + line.Text
                        + "', but the word is '" + word.Text + "'.");
                }

                CheckSyllablesLine(curriculum, word, problems);
            }
        }

        /// <summary>
        /// The voice line <paramref name="id"/>, or null (with an error at <paramref name="source"/>)
        /// when it is missing from <c>voice-lines.csv</c> or retired. Called by this class and
        /// <see cref="StepRules"/>.
        /// </summary>
        public static VoiceLine Line(Curriculum curriculum, VoiceLineId id, ContentSource source, ContentProblems problems)
        {
            return References.InUse(curriculum.VoiceLines, id.Id, source, "voice line", ContentFileNames.VoiceLines,
                problems);
        }

        private static void CheckSyllablesLine(Curriculum curriculum, Word word, ContentProblems problems)
        {
            if (!curriculum.VoiceLines.TryGet(word.SyllablesLine.Id, out VoiceLine line) || line.Retired)
            {
                return;
            }

            string syllables = string.Join("-", word.Syllables);
            if (!string.Equals(line.Text, syllables, StringComparison.OrdinalIgnoreCase))
            {
                problems.Error(word.Source, "the voice line " + line.Id + " says '" + line.Text
                    + "', but the word's syllables are '" + syllables + "'.");
            }
        }

        private static void CheckLine(VoiceLine line, IContentFiles files, ContentProblems problems)
        {
            if (!VoiceLineIds.TryParse(line.Id, out Language language, out VoiceLineKind kind)
                || language != line.Language
                || kind != line.Kind)
            {
                problems.Error(line.Source, "the id " + line.Id + " must be "
                    + LanguageCodes.CodeOf(line.Language) + "-" + ContentCodes.CodeOf(line.Kind)
                    + "-<key>, from its language and kind columns.");
            }

            if (line.HasClip && !files.ClipExists(line.Clip))
            {
                problems.Warning(line.Source, "the clip " + line.Clip
                    + " is not in content/voice/ yet: the system voice says the line.");
            }
        }
    }
}
