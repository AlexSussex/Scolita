using Scolita.Core.Languages;

namespace Scolita.Core.Voice
{
    /// <summary>
    /// The tablet's own voice, for lines with no clip: the app's is <c>AndroidTextToSpeechVoice</c>
    /// (<c>ro-RO</c>, <c>en-GB</c>), the Editor's <c>SilentVoice</c>; tests use a fake.
    /// </summary>
    public interface ISystemVoice
    {
        /// <summary>Whether it can speak <paramref name="language"/> now (ready, and the language installed).</summary>
        bool Speaks(Language language);

        /// <summary>Says <paramref name="text"/> in <paramref name="language"/>, stopping anything else; false if it can't.</summary>
        bool Say(string text, Language language);

        /// <summary>Whether it is still saying the last text.</summary>
        bool IsSaying { get; }

        /// <summary>Stops at once.</summary>
        void Stop();
    }
}
