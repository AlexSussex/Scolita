using System;
using Scolita.Core.Audio;
using Scolita.Core.Content;
using Scolita.Core.Voice;
using Scolita.Game.Settings;
using UnityEngine;

namespace Scolita.Game.Audio
{
    /// <summary>
    /// The SoundPlayer prefab's controller, one in each scene that plays sound (as <c>VoiceHost</c>; not a singleton): the
    /// scene's music, streamed and ducked under the voice (Core's <see cref="MusicDucking"/>, following
    /// <see cref="IVoice.Started"/> and <see cref="IVoice.Finished"/>), the effects through a small pool of sources (no
    /// allocation when one plays), and the tablet's vibration (<see cref="AndroidHaptics"/>), each at the tablet's volumes
    /// and switch (Core's <see cref="SoundLevels"/>). A slider at 0 plays nothing at all. Its scene's composition root calls
    /// <see cref="Begin"/>, then <see cref="FollowVoice"/> when the scene speaks.
    /// </summary>
    public sealed class SoundPlayer : MonoBehaviour, ISoundEffects
    {
        [SerializeField] private SoundBank bank;
        [SerializeField] private AudioSource music;
        [SerializeField] private AudioSource[] effects = new AudioSource[0];

        private int[] nextTakes = new int[0];
        private SoundRules rules;
        private MusicDucking ducking;
        private SoundLevels levels;
        private MusicPiece piece;
        private IVoice voice;
        private AndroidHaptics haptics;
        private bool vibrates;
        private bool voiceSpeaking;
        private int nextSource;

        /// <summary>Whether <see cref="Begin"/> was called.</summary>
        public bool IsBegun => rules != null;

        /// <summary>The volumes in use.</summary>
        public SoundLevels Levels => levels;

        /// <summary>The music's source.</summary>
        public AudioSource MusicSource => music;

        /// <summary>The effects' sources.</summary>
        public AudioSource[] EffectSources => effects;

        /// <summary>How many effects have been played (none while the sounds slider is at 0); for the tests.</summary>
        public int EffectsPlayed { get; private set; }

        /// <summary>The last effect played, when <see cref="EffectsPlayed"/> is above 0; for the tests.</summary>
        public SoundCue LastPlayed { get; private set; }

        /// <summary>How many ticks were asked for while the vibration switch was on; for the tests.</summary>
        public int TicksGiven { get; private set; }

        /// <summary>
        /// Starts the scene's sound with <paramref name="soundRules"/> at <paramref name="settings"/>' volumes and switch, and its
        /// music, <paramref name="scenePiece"/>, rising from silence (none when the music slider is at 0). Called once by the
        /// scene's composition root, and the tests.
        /// </summary>
        public void Begin(SoundRules soundRules, TabletSettingsData settings, MusicPiece scenePiece)
        {
            if (IsBegun)
            {
                throw new InvalidOperationException("SoundPlayer.Begin is called once per scene.");
            }

            rules = soundRules ?? throw new ArgumentNullException(nameof(soundRules));
            ducking = new MusicDucking(rules);
            nextTakes = new int[Enum.GetValues(typeof(SoundCue)).Length];
            piece = scenePiece;
            haptics = new AndroidHaptics();
            ApplySettings(settings.MusicVolume, settings.SoundsVolume, settings.VibrationOn);
        }

        /// <summary>
        /// Ducks the music while <paramref name="sceneVoice"/> speaks, and plays the effects lower then. Called by the scene's
        /// composition root after its voice begins, and the tests.
        /// </summary>
        public void FollowVoice(IVoice sceneVoice)
        {
            if (voice != null)
            {
                throw new InvalidOperationException("SoundPlayer follows one voice.");
            }

            voice = sceneVoice ?? throw new ArgumentNullException(nameof(sceneVoice));
            voice.Started += OnVoiceStarted;
            voice.Finished += OnVoiceFinished;
            voiceSpeaking = voice.IsSpeaking;
        }

        /// <summary>
        /// Uses new volumes and switch at once (the parent area's sliders): the music starts or stops as its slider leaves or
        /// reaches 0. Called by <c>ParentRunner</c> and the tests.
        /// </summary>
        public void UseSettings(float musicVolume, float soundsVolume, bool vibrationOn)
        {
            RequireBegun();
            ApplySettings(musicVolume, soundsVolume, vibrationOn);
        }

        /// <inheritdoc/>
        public void Play(SoundCue cue)
        {
            RequireBegun();
            if (!levels.PlaysEffects || effects.Length == 0)
            {
                return;
            }

            AudioClip clip = bank.Take(cue, nextTakes[(int)cue]++);
            if (clip == null)
            {
                Debug.LogWarning("Sound: the bank has no clip for " + cue + "; nothing is played.");
                return;
            }

            AudioSource source = effects[nextSource];
            nextSource = (nextSource + 1) % effects.Length;
            source.Stop();
            source.clip = clip;
            source.volume = levels.EffectAt(voiceSpeaking, rules);
            source.Play();
            EffectsPlayed++;
            LastPlayed = cue;
        }

        /// <summary>
        /// Plays <paramref name="cue"/> on a source of its own that outlives this scene and goes when the sound ends, so a sound
        /// that starts as the scene changes (the rocket's take-off) isn't cut. Called by <c>MapRunner</c> when a lesson opens.
        /// </summary>
        public void PlayAcrossScenes(SoundCue cue)
        {
            RequireBegun();
            AudioClip clip = levels.PlaysEffects ? bank.Take(cue, nextTakes[(int)cue]++) : null;
            if (clip == null)
            {
                return;
            }

            GameObject lingering = new GameObject("Sound across scenes");
            DontDestroyOnLoad(lingering);
            AudioSource source = lingering.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            source.clip = clip;
            source.volume = levels.EffectAt(voiceSpeaking, rules);
            source.Play();
            Destroy(lingering, clip.length + 0.1f);
            EffectsPlayed++;
            LastPlayed = cue;
        }

        /// <inheritdoc/>
        public void Vibrate(Haptic haptic)
        {
            RequireBegun();
            if (haptic == Haptic.None || !vibrates)
            {
                return;
            }

            TicksGiven++;
            haptics.Tick();
        }

        /// <summary>
        /// Moves the ducking on by <paramref name="seconds"/> and sets the music's volume. Called every frame by <see cref="Update"/>,
        /// and by the tests.
        /// </summary>
        public void Advance(float seconds)
        {
            if (!IsBegun)
            {
                return;
            }

            ducking.Tick(seconds, voiceSpeaking);
            music.volume = levels.MusicAt(ducking.Level);
        }

        private void Update()
        {
            Advance(Time.unscaledDeltaTime);
        }

        private void OnDestroy()
        {
            if (voice != null)
            {
                voice.Started -= OnVoiceStarted;
                voice.Finished -= OnVoiceFinished;
            }

            haptics?.Dispose();
        }

        private void ApplySettings(float musicVolume, float soundsVolume, bool vibrationOn)
        {
            bool wasPlayingMusic = levels.PlaysMusic && music.isPlaying;
            levels = new SoundLevels(musicVolume, soundsVolume);
            vibrates = vibrationOn;
            if (!levels.PlaysMusic || piece == MusicPiece.None)
            {
                music.Stop();
                return;
            }

            if (wasPlayingMusic)
            {
                return;
            }

            music.clip = bank.LoopOf(piece);
            if (music.clip == null)
            {
                Debug.LogWarning("Sound: the bank has no loop for " + piece + "; no music plays.");
                return;
            }

            music.loop = true;
            ducking.FadeIn();
            music.volume = levels.MusicAt(ducking.Level);
            music.Play();
        }

        private void RequireBegun()
        {
            if (!IsBegun)
            {
                throw new InvalidOperationException("The sound player is used before SoundPlayer.Begin.");
            }
        }

        private void OnVoiceStarted(VoiceLineId line)
        {
            voiceSpeaking = true;
        }

        private void OnVoiceFinished(VoiceLineId line)
        {
            voiceSpeaking = false;
        }
    }
}
