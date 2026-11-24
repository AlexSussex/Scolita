using System.Collections.Generic;
using Scolita.Core.Voice;

namespace Scolita.Tests.EditMode.Fakes
{
    /// <summary>
    /// An <see cref="IClipVoice"/> for tests: it has the clips it is given, writes what it is asked into
    /// a shared log, and plays until <see cref="FinishPlaying"/>. Used by <c>VoiceRouterTests</c>.
    /// </summary>
    public sealed class FakeClipVoice : IClipVoice
    {
        private readonly HashSet<string> clips;
        private readonly List<string> log;

        /// <summary>Creates the fake with <paramref name="existingClips"/>, logging into <paramref name="log"/>; called by the voice tests.</summary>
        public FakeClipVoice(List<string> log, params string[] existingClips)
        {
            this.log = log;
            clips = new HashSet<string>(existingClips);
        }

        /// <summary>Whether <see cref="Play"/> refuses (a clip that fails to load); set by tests.</summary>
        public bool RefusesToPlay { get; set; }

        /// <inheritdoc/>
        public bool IsPlaying { get; private set; }

        /// <inheritdoc/>
        public bool Has(string clip)
        {
            return clips.Contains(clip);
        }

        /// <inheritdoc/>
        public bool Play(string clip)
        {
            if (RefusesToPlay)
            {
                log.Add("refused clip " + clip);
                return false;
            }

            log.Add("clip " + clip);
            IsPlaying = true;
            return true;
        }

        /// <inheritdoc/>
        public void Stop()
        {
            log.Add("stop clip");
            IsPlaying = false;
        }

        /// <summary>Ends the clip playing, as its end would; called by the voice tests.</summary>
        public void FinishPlaying()
        {
            IsPlaying = false;
        }
    }
}
