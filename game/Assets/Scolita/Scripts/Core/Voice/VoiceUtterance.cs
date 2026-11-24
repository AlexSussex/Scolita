using Scolita.Core.Languages;

namespace Scolita.Core.Voice
{
    /// <summary>
    /// One thing <see cref="VoiceRouter"/> says in turn, as <see cref="VoicePlanner"/> planned it: a
    /// clip (with its text, should the clip refuse to play), a text for the system voice or the
    /// bubble, or a pause.
    /// </summary>
    public readonly struct VoiceUtterance
    {
        /// <summary>Creates an utterance; called by <see cref="VoicePlanner"/>.</summary>
        public VoiceUtterance(VoiceRoute route, string clip, string text, Language language, float seconds)
        {
            Route = route;
            Clip = clip ?? string.Empty;
            Text = text ?? string.Empty;
            Language = language;
            Seconds = seconds;
        }

        /// <summary>How it is said.</summary>
        public VoiceRoute Route { get; }

        /// <summary>The clip's file name, for <see cref="VoiceRoute.Clip"/>.</summary>
        public string Clip { get; }

        /// <summary>What is said (or shown).</summary>
        public string Text { get; }

        /// <summary>Its language.</summary>
        public Language Language { get; }

        /// <summary>How long a <see cref="VoiceRoute.Pause"/> lasts.</summary>
        public float Seconds { get; }

        /// <summary>This utterance with <paramref name="more"/> added to its text; called by <see cref="VoicePlanner"/>.</summary>
        public VoiceUtterance WithMoreText(string more)
        {
            return new VoiceUtterance(Route, Clip, Text + more, Language, Seconds);
        }
    }
}
