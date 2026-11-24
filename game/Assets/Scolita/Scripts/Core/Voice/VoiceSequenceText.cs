using System.Collections.Generic;
using System.Text;
using Scolita.Core.Content;
using Scolita.Core.Languages;

namespace Scolita.Core.Voice
{
    /// <summary>
    /// The words the mascot's bubble shows while a sequence is said (AGENTS.md → "Usability": every
    /// bubble is spoken, in the same words): each line's text with its slots filled by their fills'
    /// text, and a letter's sound shown as the letter itself, held ("a…"). Made when a lesson is built, never
    /// while it is played.
    /// </summary>
    public static class VoiceSequenceText
    {
        /// <summary>
        /// The words of <paramref name="sequence"/> in <paramref name="curriculum"/>; a line that isn't
        /// in the content is left out. Called by the Lesson scene's voice when a lesson is built, and
        /// <c>VoiceSequenceTextTests</c>.
        /// </summary>
        public static string Of(Curriculum curriculum, VoiceSequence sequence)
        {
            List<string> parts = new List<string>();
            foreach (VoiceSequenceItem item in sequence.Items)
            {
                if (curriculum.VoiceLines.TryGet(item.Line.Id, out VoiceLine line))
                {
                    parts.Add(LineText(curriculum, line, item));
                }
            }

            return string.Join(" ", parts);
        }

        /// <summary>
        /// The language of <paramref name="sequence"/>'s first line, the one its bubble's badge shows;
        /// false when that line isn't in the content. Called by the Lesson scene's voice and the tests.
        /// </summary>
        public static bool TryGetLanguage(Curriculum curriculum, VoiceSequence sequence, out Language language)
        {
            bool found = curriculum.VoiceLines.TryGet(sequence.Lead.Id, out VoiceLine line);
            language = found ? line.Language : default;
            return found;
        }

        private static string LineText(Curriculum curriculum, VoiceLine line, VoiceSequenceItem item)
        {
            if (line.Kind == VoiceLineKind.Sound && TryGetLetter(curriculum, line, out Letter letter))
            {
                return letter.Lower + "\u2026";
            }

            StringBuilder text = new StringBuilder();
            foreach (VoiceTemplatePiece piece in VoiceTemplate.Parse(line.Text).Pieces)
            {
                if (!piece.IsSlot)
                {
                    text.Append(piece.Text);
                }
                else if (item.TryGetFill(piece.Slot, out VoiceFill fill))
                {
                    text.Append(fill.Text);
                }
            }

            return text.ToString();
        }

        private static bool TryGetLetter(Curriculum curriculum, VoiceLine line, out Letter letter)
        {
            string[] parts = line.Id.Value.Split(new[] { '-' }, 3);
            letter = null;
            return parts.Length == 3
                && ContentId.TryCreate(parts[2], out ContentId letterId)
                && curriculum.Letters.TryGet(letterId, out letter);
        }
    }
}
