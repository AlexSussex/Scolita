using System.Globalization;

namespace Scolita.Core.Content
{
    /// <summary>
    /// How a voice line's clips are named (AGENTS.md → "Audio"): <c>&lt;line id&gt;.ogg</c>
    /// (<c>ro-sound-m.ogg</c>), or <c>&lt;line id&gt;.&lt;n&gt;.ogg</c> for the n-th spoken segment of
    /// a line with more than one (<c>ro-line-word-join.2.ogg</c>). Ids have no dots, so the two never
    /// clash.
    /// </summary>
    public static class VoiceClipNames
    {
        /// <summary>The clips' file type: mono OGG Vorbis.</summary>
        public const string Extension = ".ogg";

        /// <summary>
        /// The clip of segment <paramref name="segment"/> (from 0) of <paramref name="line"/>, which has
        /// <paramref name="segments"/> spoken segments. Called by the voice line rule, the recording
        /// scripts and <c>VoiceClipNamesTests</c>.
        /// </summary>
        public static string For(VoiceLineId line, int segment, int segments)
        {
            return segments <= 1
                ? line + Extension
                : line + "." + (segment + 1).ToString(CultureInfo.InvariantCulture) + Extension;
        }
    }
}
