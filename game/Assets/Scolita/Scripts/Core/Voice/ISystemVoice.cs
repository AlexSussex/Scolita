using Scolita.Core.Languages;

namespace Scolita.Core.Voice
{
    /// <summary>
    /// The tablet's own voice, for lines with no clip: the app's is <c>AndroidTextToSpeechVoice</c>
    /// (<c>ro-RO</c>, <c>en-GB</c>), the Editor's <c>SilentVoice</c>; tests use a fake.
    /// </summary>
    public interface ISystemVoice
    {
        /// <summary>
        /// Whether it is still getting ready: Android's engine starts a moment after it is made (longer on a slow tablet,
        /// or after the app comes back and the engine must start again). A line asked for meanwhile waits for it
        /// (<see cref="VoiceRouter"/>), so the first line of a screen is heard, not only shown.
        /// </summary>
        bool IsStarting { get; }

        /// <summary>Whether it can speak <paramref name="language"/> now (ready, and the language installed).</summary>
        bool Speaks(Language language);

        /// <summary>
        /// Says <paramref name="text"/> in <paramref name="language"/>, stopping anything else; false if it can't (then
        /// <see cref="IsStarting"/> says whether it is starting again and the line can wait for it).
        /// </summary>
        bool Say(string text, Language language);

        /// <summary>Whether it is still saying the last text.</summary>
        bool IsSaying { get; }

        /// <summary>Stops at once.</summary>
        void Stop();
    }
}
