using Scolita.Core.Content;

namespace Scolita.Core.Voice
{
    /// <summary>
    /// What goes into a slot of a spliced line: another voice line (a letter's sound, a word), with
    /// the text the bubble shows for it, or a plain text said by the system voice (a profile's name).
    /// </summary>
    public readonly struct VoiceFill
    {
        private VoiceFill(bool hasLine, VoiceLineId line, string text)
        {
            HasLine = hasLine;
            Line = line;
            Text = text ?? string.Empty;
        }

        /// <summary>Whether the fill is a voice line (else a plain text).</summary>
        public bool HasLine { get; }

        /// <summary>The voice line, when <see cref="HasLine"/>.</summary>
        public VoiceLineId Line { get; }

        /// <summary>The text shown for the fill (and said, for a plain text).</summary>
        public string Text { get; }

        /// <summary>
        /// The line <paramref name="line"/>, shown as <paramref name="shownText"/> ("m" for the sound
        /// /m/). Called by the lesson's activities and the voice tests.
        /// </summary>
        public static VoiceFill OfLine(VoiceLineId line, string shownText)
        {
            return new VoiceFill(true, line, shownText);
        }

        /// <summary>A plain text, said by the system voice; called by the greeting (a name) and the voice tests.</summary>
        public static VoiceFill OfText(string text)
        {
            return new VoiceFill(false, default, text);
        }
    }
}
