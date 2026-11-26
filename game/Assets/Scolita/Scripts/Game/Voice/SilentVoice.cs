using Scolita.Core.Languages;
using Scolita.Core.Voice;

namespace Scolita.Game.Voice
{
    /// <summary>
    /// A system voice that speaks no language (<see cref="ISystemVoice"/>): it stands in for Android's
    /// in the Editor, so a line without a clip shows in the bubble and <see cref="VoiceHost"/> logs it.
    /// </summary>
    public sealed class SilentVoice : ISystemVoice
    {
        /// <inheritdoc/>
        public bool IsStarting => false;

        /// <inheritdoc/>
        public bool IsSaying => false;

        /// <inheritdoc/>
        public bool Speaks(Language language)
        {
            return false;
        }

        /// <inheritdoc/>
        public bool Say(string text, Language language)
        {
            return false;
        }

        /// <inheritdoc/>
        public void Stop()
        {
        }
    }
}
