using System;
using Scolita.Core.Content;
using Scolita.Core.Voice;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scolita.Game.Voice
{
    /// <summary>
    /// Tap to hear: a letter, a word or a picture on screen that says its voice line when tapped
    /// (replacing any line being said), and pulses while its line is said. Bound to the scene's
    /// voice by the composition root (<see cref="Bind"/>); the line is set on the prefab, or given
    /// when binding. The prefab is <c>Prefabs/Voice/Hearable.prefab</c>.
    /// </summary>
    public sealed class Hearable : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private string line;
        [SerializeField] private RectTransform pulsing;
        [SerializeField] private float pulseScale = 0.08f;
        [SerializeField] private float pulsesPerSecond = 2f;

        private IVoice voice;
        private VoiceLineId lineId;
        private bool speaking;
        private Vector3 restingScale = Vector3.one;

        /// <summary>The line it says.</summary>
        public VoiceLineId Line => lineId;

        /// <summary>Whether its line is being said (and it pulses).</summary>
        public bool IsSpeaking => speaking;

        /// <summary>
        /// Binds it to <paramref name="sceneVoice"/>, saying the line set on the prefab. Throws an
        /// <see cref="ArgumentException"/> if that line isn't an id. Called by the composition root
        /// (<c>LessonRunner</c>) and the PlayMode tests.
        /// </summary>
        public void Bind(IVoice sceneVoice)
        {
            BindTo(sceneVoice, new VoiceLineId(ContentId.Create(line)));
        }

        /// <summary>Binds it to <paramref name="sceneVoice"/>, saying <paramref name="spoken"/>; called by the activities.</summary>
        public void Bind(IVoice sceneVoice, VoiceLineId spoken)
        {
            BindTo(sceneVoice, spoken);
        }

        /// <summary>Says the line; called by the EventSystem on a tap.</summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (voice == null)
            {
                Debug.LogError("Voice: the Hearable " + name + " was tapped before it was bound to a voice.");
                return;
            }

            voice.Speak(lineId);
        }

        private void BindTo(IVoice sceneVoice, VoiceLineId spoken)
        {
            Unsubscribe();
            voice = sceneVoice;
            lineId = spoken;
            if (isActiveAndEnabled)
            {
                Subscribe();
            }
        }

        private void Awake()
        {
            if (pulsing == null)
            {
                pulsing = transform as RectTransform;
            }

            restingScale = pulsing.localScale;
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
            StopPulsing();
        }

        private void Update()
        {
            if (speaking)
            {
                float wave = Mathf.Sin(Time.unscaledTime * pulsesPerSecond * 2f * Mathf.PI);
                pulsing.localScale = restingScale * (1f + pulseScale * (0.5f + 0.5f * wave));
            }
        }

        private void Subscribe()
        {
            if (voice == null)
            {
                return;
            }

            voice.Started += OnStarted;
            voice.Finished += OnFinished;
            speaking = voice.IsSpeaking && voice.Current == lineId;
        }

        private void Unsubscribe()
        {
            if (voice == null)
            {
                return;
            }

            voice.Started -= OnStarted;
            voice.Finished -= OnFinished;
        }

        private void OnStarted(VoiceLineId started)
        {
            speaking = started == lineId;
            if (!speaking)
            {
                StopPulsing();
            }
        }

        private void OnFinished(VoiceLineId finished)
        {
            if (finished == lineId)
            {
                StopPulsing();
            }
        }

        private void StopPulsing()
        {
            speaking = false;
            pulsing.localScale = restingScale;
        }
    }
}
