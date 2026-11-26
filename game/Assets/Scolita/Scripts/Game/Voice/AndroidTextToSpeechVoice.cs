using System;
using System.Globalization;
using System.Threading;
using Scolita.Core.Languages;
using Scolita.Core.Voice;
using UnityEngine;

namespace Scolita.Game.Voice
{
    /// <summary>
    /// The tablet's own voice (<see cref="ISystemVoice"/>): Android's <c>TextToSpeech</c> through
    /// <see cref="AndroidJavaObject"/>, with no plugin and no permission, in <c>ro-RO</c> and
    /// <c>en-GB</c> (decision D4). It speaks only with a voice installed on the tablet, never a
    /// network voice, so nothing it says (a profile's name included) leaves the tablet and it works
    /// offline; a language with no installed voice is not spoken (the bubble shows the line). The
    /// engine gets ready a moment after it is made (<see cref="IsStarting"/>, while lines wait for it);
    /// it checks the voices when the engine is ready and again when the app comes back, logging what it
    /// found, and starts the engine again when Android has let it go while the app was away (no voices
    /// to list, or a line refused). Android calls back on its own threads, so the callbacks only set
    /// numbers that the Unity thread reads. Made by <see cref="VoiceHost"/> on Android only.
    /// </summary>
    public sealed class AndroidTextToSpeechVoice : ISystemVoice, IDisposable
    {
        private const int Success = 0;
        private const int MostRestartsInARow = 2;
        private const int QueueFlush = 0;
        private const int InitPending = int.MinValue;
        private const float SecondsBeforeGivingUp = 4f;
        private const float SecondsPerCharacterBeforeGivingUp = 0.2f;
        private const string NotInstalledFeature = "notInstalled";
        private const string VolumeParameter = "volume";

        private readonly AndroidJavaObject speakParameters;
        private AndroidJavaObject engine;
        private AndroidJavaObject romanianVoice;
        private AndroidJavaObject englishVoice;
        private int initStatus = InitPending;
        private int engineNumber;
        private int restartsInARow;
        private int completedUtterance;
        private int currentUtterance;
        private bool voicesChecked;
        private bool failureLogged;
        private bool listening;
        private bool hasVoiceSet;
        private Language voiceSet;
        private float giveUpAt;

        /// <summary>Starts the engine (it gets ready a moment later); called by <see cref="VoiceHost"/> on Android.</summary>
        public AndroidTextToSpeechVoice()
        {
            StartEngine();
            speakParameters = new AndroidJavaObject("android.os.Bundle");
        }

        /// <inheritdoc/>
        public bool IsStarting => Volatile.Read(ref initStatus) == InitPending;

        /// <inheritdoc/>
        public bool IsSaying
        {
            get
            {
                if (Volatile.Read(ref completedUtterance) == currentUtterance)
                {
                    return false;
                }

                if (Time.unscaledTime < giveUpAt)
                {
                    return true;
                }

                Debug.LogWarning("Voice: the system voice never said it had finished; going on.");
                currentUtterance = Volatile.Read(ref completedUtterance);
                return false;
            }
        }

        /// <inheritdoc/>
        public bool Speaks(Language language)
        {
            return IsReady() && VoiceFor(language) != null;
        }

        /// <inheritdoc/>
        public bool Say(string text, Language language)
        {
            if (!IsReady() || !UseVoice(language))
            {
                return false;
            }

            int utterance = currentUtterance + 1;
            int result = engine.Call<int>("speak", text, QueueFlush, speakParameters, utterance.ToString(CultureInfo.InvariantCulture));
            if (result != Success)
            {
                Debug.LogWarning("Voice: the system voice refused to speak (" + result + ").");
                RestartEngine();
                return false;
            }

            restartsInARow = 0;
            currentUtterance = utterance;
            giveUpAt = Time.unscaledTime + SecondsBeforeGivingUp + text.Length * SecondsPerCharacterBeforeGivingUp;
            return true;
        }

        /// <summary>
        /// Says every later line at <paramref name="volume"/> (0 to 1), through the engine's volume parameter. Called by
        /// <see cref="VoiceHost.SetVolume"/>.
        /// </summary>
        public void SetVolume(float volume)
        {
            speakParameters.Call("putFloat", VolumeParameter, volume);
        }

        /// <inheritdoc/>
        public void Stop()
        {
            if (IsReady())
            {
                engine.Call<int>("stop");
            }

            Volatile.Write(ref completedUtterance, currentUtterance);
        }

        /// <summary>
        /// Looks for the installed voices again at the next line (a voice may have been installed while
        /// the app was away, or Android may have let the engine go, which starts it again); called by
        /// <see cref="VoiceHost"/> when the app comes back.
        /// </summary>
        public void CheckVoicesAgain()
        {
            voicesChecked = false;
            restartsInARow = 0;
        }

        /// <summary>Shuts the engine down; called by <see cref="VoiceHost"/> when its scene closes.</summary>
        public void Dispose()
        {
            StopEngine();
            speakParameters.Dispose();
            romanianVoice?.Dispose();
            englishVoice?.Dispose();
        }

        private bool IsReady()
        {
            int status = Volatile.Read(ref initStatus);
            if (status != Success)
            {
                if (status != InitPending && !failureLogged)
                {
                    failureLogged = true;
                    Debug.LogWarning("Voice: the system voice didn't start (" + status + "); the lines show in the bubble.");
                }

                return false;
            }

            if (!voicesChecked)
            {
                CheckVoices();
            }

            return Volatile.Read(ref initStatus) == Success;
        }

        private AndroidJavaObject VoiceFor(Language language)
        {
            return language == Language.Romanian ? romanianVoice : englishVoice;
        }

        private void StartEngine()
        {
            int number = ++engineNumber;
            Volatile.Write(ref initStatus, InitPending);
            voicesChecked = false;
            failureLogged = false;
            listening = false;
            hasVoiceSet = false;
            using (AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                engine = new AndroidJavaObject("android.speech.tts.TextToSpeech", activity,
                    new TextToSpeechInitListener(status => OnInit(number, status)));
            }
        }

        private void StopEngine()
        {
            engine.Call("shutdown");
            engine.Dispose();
        }

        private void RestartEngine()
        {
            if (restartsInARow >= MostRestartsInARow)
            {
                return;
            }

            restartsInARow++;
            Debug.LogWarning("Voice: the system voice has let go of its engine; starting it again.");
            Volatile.Write(ref completedUtterance, currentUtterance);
            StopEngine();
            StartEngine();
        }

        private void CheckVoices()
        {
            voicesChecked = true;
            hasVoiceSet = false;
            if (!HasVoiceList())
            {
                RestartEngine();
                return;
            }

            romanianVoice?.Dispose();
            englishVoice?.Dispose();
            romanianVoice = InstalledVoice("ro", "RO");
            englishVoice = InstalledVoice("en", "GB");
            if (!listening)
            {
                listening = true;
                engine.Call<int>("setOnUtteranceCompletedListener", new TextToSpeechUtteranceListener(OnUtteranceCompleted));
            }
        }

        private AndroidJavaObject InstalledVoice(string language, string country)
        {
            AndroidJavaObject best = null;
            int bestQuality = int.MinValue;
            using (AndroidJavaObject voices = engine.Call<AndroidJavaObject>("getVoices"))
            {
                if (voices != null)
                {
                    using (AndroidJavaObject iterator = voices.Call<AndroidJavaObject>("iterator"))
                    {
                        while (iterator.Call<bool>("hasNext"))
                        {
                            AndroidJavaObject voice = iterator.Call<AndroidJavaObject>("next");
                            int quality = voice.Call<int>("getQuality");
                            if (IsInstalled(voice, language, country) && quality > bestQuality)
                            {
                                best?.Dispose();
                                best = voice;
                                bestQuality = quality;
                            }
                            else
                            {
                                voice.Dispose();
                            }
                        }
                    }
                }
            }

            string code = language + "-" + country;
            if (best == null)
            {
                Debug.LogWarning("Voice: the system voice has no installed " + code + " voice; its lines show in the bubble. "
                    + "Install it in Settings, Text-to-speech, Google, Install voice data.");
            }
            else
            {
                Debug.Log("Voice: the system voice speaks " + code + " with " + best.Call<string>("getName") + ".");
            }

            return best;
        }

        private static bool IsInstalled(AndroidJavaObject voice, string language, string country)
        {
            using (AndroidJavaObject locale = voice.Call<AndroidJavaObject>("getLocale"))
            using (AndroidJavaObject features = voice.Call<AndroidJavaObject>("getFeatures"))
            {
                return locale.Call<string>("getLanguage") == language
                    && locale.Call<string>("getCountry") == country
                    && !voice.Call<bool>("isNetworkConnectionRequired")
                    && !features.Call<bool>("contains", NotInstalledFeature);
            }
        }

        private bool UseVoice(Language language)
        {
            AndroidJavaObject voice = VoiceFor(language);
            if (voice == null)
            {
                return false;
            }

            if (hasVoiceSet && voiceSet == language)
            {
                return true;
            }

            int result = engine.Call<int>("setVoice", voice);
            if (result != Success)
            {
                Debug.LogWarning("Voice: the system voice can't switch to " + LanguageCodes.CodeOf(language) + " (" + result + ").");
                return false;
            }

            hasVoiceSet = true;
            voiceSet = language;
            return true;
        }

        private bool HasVoiceList()
        {
            using (AndroidJavaObject voices = engine.Call<AndroidJavaObject>("getVoices"))
            {
                return voices != null;
            }
        }

        private void OnInit(int number, int status)
        {
            if (number == Volatile.Read(ref engineNumber))
            {
                Volatile.Write(ref initStatus, status);
            }
        }

        private void OnUtteranceCompleted(string utterance)
        {
            if (int.TryParse(utterance, NumberStyles.Integer, CultureInfo.InvariantCulture, out int finished)
                && finished > Volatile.Read(ref completedUtterance))
            {
                Volatile.Write(ref completedUtterance, finished);
            }
        }
    }
}
