using System;
using System.Collections.Generic;
using Scolita.Core.Content;

namespace Scolita.Core.Voice
{
    /// <summary>
    /// Lines said one after another, each with the fills of its slots: "Atinge litera {x}." with
    /// <c>{x}</c> the sound /m/ (<c>VoiceSequence.Of(pickLetterX).With(VoiceSlot.X, VoiceFill.OfLine(soundM, "m"))</c>),
    /// or "Acum ascultăm litera în engleză." then the English sound. Made when a line is asked for
    /// (a tap, a step), never in a frame loop.
    /// </summary>
    public sealed class VoiceSequence
    {
        private readonly List<VoiceSequenceItem> items = new List<VoiceSequenceItem>();

        private VoiceSequence()
        {
        }

        /// <summary>The lines, in order.</summary>
        public IReadOnlyList<VoiceSequenceItem> Items => items;

        /// <summary>The first line, which names the whole sequence in <see cref="IVoice.Started"/> and <see cref="IVoice.Finished"/>.</summary>
        public VoiceLineId Lead => items[0].Line;

        /// <summary>
        /// A sequence of <paramref name="lines"/>, in order; throws an <see cref="ArgumentException"/>
        /// for none. Called by the activities, the lesson's voice and the voice tests.
        /// </summary>
        public static VoiceSequence Of(params VoiceLineId[] lines)
        {
            if (lines == null || lines.Length == 0)
            {
                throw new ArgumentException("A voice sequence needs at least one line.", nameof(lines));
            }

            VoiceSequence sequence = new VoiceSequence();
            foreach (VoiceLineId line in lines)
            {
                sequence.items.Add(new VoiceSequenceItem(line));
            }

            return sequence;
        }

        /// <summary>Adds <paramref name="line"/> at the end and returns this sequence; called by the activities and the voice tests.</summary>
        public VoiceSequence Then(VoiceLineId line)
        {
            items.Add(new VoiceSequenceItem(line));
            return this;
        }

        /// <summary>
        /// Fills <paramref name="slot"/> of the last line added and returns this sequence; called by the
        /// activities and the voice tests.
        /// </summary>
        public VoiceSequence With(VoiceSlot slot, VoiceFill fill)
        {
            items[items.Count - 1].SetFill(slot, fill);
            return this;
        }
    }
}
