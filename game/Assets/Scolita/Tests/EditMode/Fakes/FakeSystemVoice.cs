using System.Collections.Generic;
using System.Linq;
using Scolita.Core.Languages;
using Scolita.Core.Voice;

namespace Scolita.Tests.EditMode.Fakes
{
    /// <summary>
    /// An <see cref="ISystemVoice"/> for tests: it speaks the languages it is given, writes what it is
    /// asked into a shared log, and speaks until <see cref="FinishSaying"/>; it can be starting (ready late), and
    /// start again when asked to say something. Used by <c>VoiceRouterTests</c>.
    /// </summary>
    public sealed class FakeSystemVoice : ISystemVoice
    {
        private readonly HashSet<Language> languages;
        private readonly List<string> log;

        /// <summary>Creates the fake speaking <paramref name="spoken"/>, logging into <paramref name="log"/>; called by the voice tests.</summary>
        public FakeSystemVoice(List<string> log, params Language[] spoken)
        {
            this.log = log;
            languages = new HashSet<Language>(spoken);
        }

        /// <summary>Whether <see cref="Say"/> refuses (the engine failing); set by tests.</summary>
        public bool RefusesToSay { get; set; }

        /// <summary>
        /// Whether the next <see cref="Say"/> finds the engine gone: it refuses and starts again (<see cref="IsStarting"/>
        /// becomes true), as Android's does after the app comes back; set by tests.
        /// </summary>
        public bool LostOnNextSay { get; set; }

        /// <inheritdoc/>
        public bool IsStarting { get; set; }

        /// <inheritdoc/>
        public bool IsSaying { get; private set; }

        /// <inheritdoc/>
        public bool Speaks(Language language)
        {
            return !IsStarting && languages.Contains(language);
        }

        /// <inheritdoc/>
        public bool Say(string text, Language language)
        {
            if (RefusesToSay)
            {
                return false;
            }

            if (LostOnNextSay)
            {
                LostOnNextSay = false;
                IsStarting = true;
                log.Add("lost, starting again");
                return false;
            }

            log.Add("say " + LanguageCodes.CodeOf(language) + " " + text.Trim());
            IsSaying = true;
            return true;
        }

        /// <inheritdoc/>
        public void Stop()
        {
            log.Add("stop system");
            IsSaying = false;
        }

        /// <summary>Ends what it is saying; called by the voice tests.</summary>
        public void FinishSaying()
        {
            IsSaying = false;
        }

        /// <summary>The languages it speaks, for messages.</summary>
        public override string ToString()
        {
            return string.Join(" ", languages.Select(LanguageCodes.CodeOf));
        }
    }
}
