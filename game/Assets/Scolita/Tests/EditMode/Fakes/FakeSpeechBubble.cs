using System.Collections.Generic;
using Scolita.Core.Languages;
using Scolita.Core.Voice;

namespace Scolita.Tests.EditMode.Fakes
{
    /// <summary>An <see cref="ISpeechBubble"/> for tests: writes what it shows and hides into a shared log. Used by <c>VoiceRouterTests</c>.</summary>
    public sealed class FakeSpeechBubble : ISpeechBubble
    {
        private readonly List<string> log;

        /// <summary>Creates the fake, logging into <paramref name="log"/>; called by the voice tests.</summary>
        public FakeSpeechBubble(List<string> log)
        {
            this.log = log;
        }

        /// <inheritdoc/>
        public void Show(string text, Language language)
        {
            log.Add("show " + LanguageCodes.CodeOf(language) + " " + text.Trim());
        }

        /// <inheritdoc/>
        public void Hide()
        {
            log.Add("hide");
        }
    }
}
