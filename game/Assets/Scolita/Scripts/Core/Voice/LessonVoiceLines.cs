using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Content;
using Scolita.Core.Languages;

namespace Scolita.Core.Voice
{
    /// <summary>
    /// The voice lines a lesson can say, so its clips are loaded when it starts and unloaded after
    /// (AGENTS.md → "Performance", audio): in each step's languages, its letters' sounds and names, its
    /// words' lines (word, slow, syllables) and the sounds of their letters, and its kind's instruction
    /// lines. Only lines that are in the content are listed.
    /// </summary>
    public static class LessonVoiceLines
    {
        /// <summary>
        /// The lines of <paramref name="lesson"/>, each once, in the order of its steps. Called by the
        /// Lesson scene's composition root (<c>LessonRunner</c>) and <c>LessonVoiceLinesTests</c>.
        /// </summary>
        public static IReadOnlyList<VoiceLineId> Of(Curriculum curriculum, Lesson lesson)
        {
            List<VoiceLineId> found = new List<VoiceLineId>();
            foreach (Step step in lesson.Steps)
            {
                foreach (Language language in step.Languages.Languages())
                {
                    found.AddRange(StepLines(curriculum, step, language));
                }
            }

            return found
                .Distinct()
                .Where(line => curriculum.VoiceLines.Contains(line.Id))
                .ToArray();
        }

        private static IEnumerable<VoiceLineId> StepLines(Curriculum curriculum, Step step, Language language)
        {
            IEnumerable<VoiceLineId> letters = step.Letters
                .Where(curriculum.Letters.Contains)
                .Select(curriculum.Letters.Get)
                .Where(letter => letter.HasLanguage(language))
                .SelectMany(letter => new[] { letter.SoundLine(language), letter.NameLine(language) });
            IEnumerable<VoiceLineId> words = step.Words
                .Where(curriculum.Words.Contains)
                .Select(curriculum.Words.Get)
                .Where(word => word.Language == language)
                .SelectMany(word => WordLines(curriculum, word));
            IEnumerable<VoiceLineId> instructions = ActivityKinds.Of(step.Kind).InstructionLines
                .Select(key => VoiceLineIds.For(language, VoiceLineKind.Line, key));
            return instructions.Concat(letters).Concat(words);
        }

        private static IEnumerable<VoiceLineId> WordLines(Curriculum curriculum, Word word)
        {
            IEnumerable<VoiceLineId> sounds = word.Letters
                .Where(curriculum.Letters.Contains)
                .Select(letter => curriculum.Letters.Get(letter).SoundLine(word.Language));
            return new[] { word.WordLine, word.SlowLine, word.SyllablesLine }.Concat(sounds);
        }
    }
}
