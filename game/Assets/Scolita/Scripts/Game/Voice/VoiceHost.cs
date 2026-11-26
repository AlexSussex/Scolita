using System;
using System.Collections.Generic;
using Scolita.Content;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Voice;
using UnityEngine;

namespace Scolita.Game.Voice
{
    /// <summary>
    /// The Voice prefab's controller: makes the scene's voice (<see cref="VoiceRouter"/>) from the
    /// recorded clips (<see cref="RecordedVoice"/>), the tablet's voice (Android's
    /// <c>TextToSpeech</c>, or <see cref="SilentVoice"/> in the Editor) and the bubble, and moves it on
    /// every frame, logging each part said in the Editor and development builds. Its scene's composition root calls <see cref="Begin"/> with the curriculum and
    /// hands <see cref="Voice"/> to what speaks. Not a singleton: each scene that speaks has one.
    /// </summary>
    public sealed class VoiceHost : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private SpeechBubbleView bubble;
        [SerializeField] private VoiceLineCatalog voiceLines;
        [SerializeField] private float pauseSeconds = 0.35f;
        [SerializeField] private float bubbleSecondsPerCharacter = 0.07f;
        [SerializeField] private float bubbleMinimumSeconds = 2f;
        [SerializeField] private float systemVoiceWaitSeconds = 5f;

        private VoiceRouter router;
        private RecordedVoice recorded;
        private ISystemVoice system;
        private AndroidTextToSpeechVoice android;

        /// <summary>
        /// Raised when the voice starts saying a part of a line (how it is said: a clip, the system
        /// voice or the bubble); the lesson's bubble shows the line's words again then.
        /// </summary>
        public event Action<VoiceUtterance> PartStarted;

        /// <summary>The scene's voice, once <see cref="Begin"/> has been called.</summary>
        public IVoice Voice => router ?? throw new InvalidOperationException("The voice is used before VoiceHost.Begin.");

        /// <summary>
        /// Makes the voice for <paramref name="curriculum"/>. Called once by the scene's composition
        /// root (<c>LessonRunner</c>) and the PlayMode tests.
        /// </summary>
        public void Begin(Curriculum curriculum)
        {
            if (router != null)
            {
                throw new InvalidOperationException("VoiceHost.Begin is called once per scene.");
            }

            recorded = new RecordedVoice(audioSource, voiceLines);
            android = Application.platform == RuntimePlatform.Android ? new AndroidTextToSpeechVoice() : null;
            system = android ?? (ISystemVoice)new SilentVoice();
            router = new VoiceRouter(curriculum, recorded, system, bubble,
                new VoiceTiming(pauseSeconds, bubbleSecondsPerCharacter, bubbleMinimumSeconds, systemVoiceWaitSeconds));
            router.LineMissing += OnLineMissing;
            router.PartStarted += OnPartStarted;
            if (Debug.isDebugBuild)
            {
                router.PartStarted += OnPartStartedInADevelopmentBuild;
            }
        }

        /// <summary>
        /// Sets the voice's volume, from 0 (silent) to 1 (full; the tablet's settings, set in the parent area): the clips'
        /// and the tablet's voice's. Called by the scenes' composition roots after <see cref="Begin"/>.
        /// </summary>
        public void SetVolume(float volume)
        {
            float clamped = Mathf.Clamp01(volume);
            audioSource.volume = clamped;
            android?.SetVolume(clamped);
        }

        /// <summary>Loads the clips of <paramref name="lines"/> (a lesson's); called by the Lesson scene's composition root.</summary>
        public void LoadLines(IEnumerable<VoiceLineId> lines)
        {
            recorded.LoadLines(lines);
        }

        private void Update()
        {
            router?.Tick(Time.unscaledDeltaTime);
        }

        private void OnDestroy()
        {
            if (router == null)
            {
                return;
            }

            router.Stop();
            router.LineMissing -= OnLineMissing;
            router.PartStarted -= OnPartStarted;
            router.PartStarted -= OnPartStartedInADevelopmentBuild;
            recorded.UnloadAll();
            android?.Dispose();
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused)
            {
                router?.Stop();
            }
            else
            {
                android?.CheckVoicesAgain();
            }
        }

        private void OnPartStarted(VoiceUtterance part)
        {
            PartStarted?.Invoke(part);
        }

        private static void OnLineMissing(VoiceLineId line)
        {
            Debug.LogError("Voice: the line " + line + " is not in the content (voice-lines.csv); nothing is said.");
        }

        private static void OnPartStartedInADevelopmentBuild(VoiceUtterance part)
        {
            string said = part.Route == VoiceRoute.Clip
                ? part.Clip
                : LanguageCodes.CodeOf(part.Language) + ", " + part.Text.Length + " characters";
            Debug.Log("Voice: " + part.Route + " " + said);
        }
    }
}
