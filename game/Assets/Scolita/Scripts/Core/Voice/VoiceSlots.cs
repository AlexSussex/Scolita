using System.Collections.Generic;
using System.Linq;

namespace Scolita.Core.Voice
{
    /// <summary>The codes of the <see cref="VoiceSlot"/>s, as written between braces in a voice line's text.</summary>
    public static class VoiceSlots
    {
        /// <summary>How many slots there are; the size of a <see cref="VoiceSequence"/>'s fills.</summary>
        public const int Count = 6;

        private static readonly IReadOnlyList<KeyValuePair<string, VoiceSlot>> Codes = new[]
        {
            new KeyValuePair<string, VoiceSlot>("x", VoiceSlot.X),
            new KeyValuePair<string, VoiceSlot>("y", VoiceSlot.Y),
            new KeyValuePair<string, VoiceSlot>("word", VoiceSlot.Word),
            new KeyValuePair<string, VoiceSlot>("slow", VoiceSlot.Slow),
            new KeyValuePair<string, VoiceSlot>("stretched", VoiceSlot.Stretched),
            new KeyValuePair<string, VoiceSlot>("name", VoiceSlot.Name)
        };

        /// <summary>Reads a slot from its exact code (<c>x</c>, <c>word</c>); called by <see cref="VoiceTemplate"/>.</summary>
        public static bool TryParse(string code, out VoiceSlot slot)
        {
            foreach (KeyValuePair<string, VoiceSlot> pair in Codes)
            {
                if (pair.Key == code)
                {
                    slot = pair.Value;
                    return true;
                }
            }

            slot = default;
            return false;
        }

        /// <summary>Every code in braces, for messages; called by the voice line rule.</summary>
        public static string AllCodes()
        {
            return string.Join(", ", Codes.Select(pair => "{" + pair.Key + "}"));
        }
    }
}
