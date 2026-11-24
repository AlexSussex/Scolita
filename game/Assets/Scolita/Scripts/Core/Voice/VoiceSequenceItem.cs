using Scolita.Core.Content;

namespace Scolita.Core.Voice
{
    /// <summary>One line of a <see cref="VoiceSequence"/>, with the fills of its slots (none for a line without slots).</summary>
    public sealed class VoiceSequenceItem
    {
        private readonly VoiceFill[] fills = new VoiceFill[VoiceSlots.Count];
        private readonly bool[] filled = new bool[VoiceSlots.Count];

        /// <summary>Creates the item for <paramref name="line"/>; called by <see cref="VoiceSequence"/>.</summary>
        public VoiceSequenceItem(VoiceLineId line)
        {
            Line = line;
        }

        /// <summary>The line.</summary>
        public VoiceLineId Line { get; }

        /// <summary>The fill of <paramref name="slot"/>, if one was given; called by <see cref="VoicePlanner"/>.</summary>
        public bool TryGetFill(VoiceSlot slot, out VoiceFill fill)
        {
            fill = fills[(int)slot];
            return filled[(int)slot];
        }

        /// <summary>Sets the fill of <paramref name="slot"/>; called by <see cref="VoiceSequence.With"/>.</summary>
        public void SetFill(VoiceSlot slot, VoiceFill fill)
        {
            fills[(int)slot] = fill;
            filled[(int)slot] = true;
        }
    }
}
