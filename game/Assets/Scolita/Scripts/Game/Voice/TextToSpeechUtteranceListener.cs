using System;
using UnityEngine;

namespace Scolita.Game.Voice
{
    /// <summary>
    /// Android's <c>TextToSpeech.OnUtteranceCompletedListener</c>, for
    /// <see cref="AndroidTextToSpeechVoice"/>: an interface, which a proxy can implement (the newer
    /// <c>UtteranceProgressListener</c> is an abstract class, which it can't). Android calls it when
    /// an utterance is done or has failed, on its own thread, so the callback must only store the id.
    /// </summary>
    public sealed class TextToSpeechUtteranceListener : AndroidJavaProxy
    {
        private readonly Action<string> completed;

        /// <summary>Creates the listener calling <paramref name="completed"/>; called by <see cref="AndroidTextToSpeechVoice"/>.</summary>
        public TextToSpeechUtteranceListener(Action<string> completed)
            : base("android.speech.tts.TextToSpeech$OnUtteranceCompletedListener")
        {
            this.completed = completed;
        }

        /// <summary>Called by Android with the finished utterance's id.</summary>
        public void onUtteranceCompleted(string utteranceId)
        {
            completed(utteranceId);
        }
    }
}
