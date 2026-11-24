using System;
using UnityEngine;

namespace Scolita.Game.Voice
{
    /// <summary>
    /// Android's <c>TextToSpeech.OnInitListener</c>, for <see cref="AndroidTextToSpeechVoice"/>.
    /// Android calls it on its own thread, so the callback must only store the status.
    /// </summary>
    public sealed class TextToSpeechInitListener : AndroidJavaProxy
    {
        private readonly Action<int> initialised;

        /// <summary>Creates the listener calling <paramref name="initialised"/>; called by <see cref="AndroidTextToSpeechVoice"/>.</summary>
        public TextToSpeechInitListener(Action<int> initialised)
            : base("android.speech.tts.TextToSpeech$OnInitListener")
        {
            this.initialised = initialised;
        }

        /// <summary>Called by Android with the engine's status (0 when ready).</summary>
        public void onInit(int status)
        {
            initialised(status);
        }
    }
}
