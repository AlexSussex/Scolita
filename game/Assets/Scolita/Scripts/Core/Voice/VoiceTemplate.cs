using System.Collections.Generic;

namespace Scolita.Core.Voice
{
    /// <summary>
    /// A voice line's text split at its slots: "Atinge litera {x}." is the spoken segment
    /// "Atinge litera ", the slot <see cref="VoiceSlot.X"/>, and the punctuation ".". Each spoken
    /// segment is recorded as a clip of its own, so a spliced line plays its clips and its parts in
    /// turn. A brace pair that isn't a known slot is kept as text and listed in
    /// <see cref="UnknownSlots"/> (the content rules report it).
    /// </summary>
    public sealed class VoiceTemplate
    {
        private VoiceTemplate(IReadOnlyList<VoiceTemplatePiece> pieces, int spokenSegments, bool hasSlots,
            IReadOnlyList<string> unknownSlots)
        {
            Pieces = pieces;
            SpokenSegments = spokenSegments;
            HasSlots = hasSlots;
            UnknownSlots = unknownSlots;
        }

        /// <summary>The pieces, in order.</summary>
        public IReadOnlyList<VoiceTemplatePiece> Pieces { get; }

        /// <summary>How many pieces are said (and so how many clips the line can have).</summary>
        public int SpokenSegments { get; }

        /// <summary>Whether the text has a slot.</summary>
        public bool HasSlots { get; }

        /// <summary>The names in braces that aren't slots (<c>{foo}</c>), in order.</summary>
        public IReadOnlyList<string> UnknownSlots { get; }

        /// <summary>
        /// Splits <paramref name="text"/>. Called by <see cref="VoicePlanner"/> (once per line, when
        /// the voice is made), the voice line rule, the recording scripts and <c>VoiceTemplateTests</c>.
        /// </summary>
        public static VoiceTemplate Parse(string text)
        {
            string source = text ?? string.Empty;
            List<VoiceTemplatePiece> pieces = new List<VoiceTemplatePiece>();
            List<string> unknown = new List<string>();
            int spoken = 0;
            bool hasSlots = false;
            int textStart = 0;
            int index = 0;
            while (index < source.Length)
            {
                int close = source[index] == '{' ? source.IndexOf('}', index + 1) : -1;
                if (close < 0)
                {
                    index++;
                    continue;
                }

                string name = source.Substring(index + 1, close - index - 1);
                if (!VoiceSlots.TryParse(name, out VoiceSlot slot))
                {
                    unknown.Add(name);
                    index = close + 1;
                    continue;
                }

                spoken = AddText(pieces, source.Substring(textStart, index - textStart), spoken);
                pieces.Add(new VoiceTemplatePiece(string.Empty, true, slot, -1));
                hasSlots = true;
                index = close + 1;
                textStart = index;
            }

            spoken = AddText(pieces, source.Substring(textStart), spoken);
            return new VoiceTemplate(pieces, spoken, hasSlots, unknown);
        }

        private static int AddText(List<VoiceTemplatePiece> pieces, string text, int spoken)
        {
            if (text.Length == 0)
            {
                return spoken;
            }

            bool isSpoken = HasLetterOrDigit(text);
            pieces.Add(new VoiceTemplatePiece(text, false, default, isSpoken ? spoken : -1));
            return isSpoken ? spoken + 1 : spoken;
        }

        private static bool HasLetterOrDigit(string text)
        {
            foreach (char character in text)
            {
                if (char.IsLetterOrDigit(character))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
