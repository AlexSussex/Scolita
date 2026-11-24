using System;
using Scolita.Core.Languages;
using Scolita.Game.Saving;
using UnityEngine;

namespace Scolita.Game.Settings
{
    /// <summary>
    /// The tablet's own settings, shared by every profile and saved as <c>settings.json</c> in the
    /// save root through <see cref="SaveStore.ForTablet"/>: music, sounds and vibration on or off,
    /// the voice's volume, and the language of the app's own words (buttons, the parent area).
    /// </summary>
    [Serializable]
    public sealed class TabletSettingsData : ISaveData<TabletSettingsData>
    {
        /// <summary>The save file's name, without its extension.</summary>
        public const string FileName = "settings";

        private const int LatestVersion = 1;
        private const float LowestVoiceVolume = 0f;
        private const float HighestVoiceVolume = 1f;
        private const Language DefaultAppLanguage = Language.Romanian;

        [SerializeField] private int version;
        [SerializeField] private bool musicOn;
        [SerializeField] private bool soundsOn;
        [SerializeField] private float voiceVolume;
        [SerializeField] private bool vibrationOn;
        [SerializeField] private string appLanguage;

        /// <summary>
        /// Creates settings in the current shape, the voice volume clamped to 0–1 (1 when it is not
        /// a number). Called by <see cref="CreateDefaults"/>, <see cref="Upgraded"/> and the tests.
        /// </summary>
        public TabletSettingsData(bool musicOn, bool soundsOn, float voiceVolume, bool vibrationOn, Language appLanguage)
            : this(musicOn, soundsOn, voiceVolume, vibrationOn, LanguageCodes.CodeOf(appLanguage))
        {
        }

        private TabletSettingsData(bool musicOn, bool soundsOn, float voiceVolume, bool vibrationOn, string appLanguageCode)
        {
            version = LatestVersion;
            this.musicOn = musicOn;
            this.soundsOn = soundsOn;
            this.voiceVolume = ClampedVolume(voiceVolume);
            this.vibrationOn = vibrationOn;
            appLanguage = appLanguageCode;
        }

        /// <inheritdoc/>
        public int Version => version;

        /// <inheritdoc/>
        public int CurrentVersion => LatestVersion;

        /// <summary>Whether the music plays.</summary>
        public bool MusicOn => musicOn;

        /// <summary>Whether the sound effects play.</summary>
        public bool SoundsOn => soundsOn;

        /// <summary>The voice's volume, from 0 (silent) to 1 (full).</summary>
        public float VoiceVolume => voiceVolume;

        /// <summary>Whether the tablet vibrates on a right answer or a finished stroke.</summary>
        public bool VibrationOn => vibrationOn;

        /// <summary>The language of the app's own words; Romanian if the saved code is unknown.</summary>
        public Language AppLanguage => LanguageOf(appLanguage);

        /// <summary>
        /// The settings of a first start: everything on, the voice at full volume, the app's words in
        /// Romanian (the anchor language, decision D1). Called by <c>BootLoader</c> and the tests.
        /// </summary>
        public static TabletSettingsData CreateDefaults()
        {
            return new TabletSettingsData(true, true, HighestVoiceVolume, true, DefaultAppLanguage);
        }

        /// <summary>
        /// Returns these settings in the current shape: the voice volume clamped; an unknown language
        /// code (from a later version of the app) is kept, so that version can still use it, and
        /// read as Romanian (<see cref="AppLanguage"/>). <c>JsonUtility</c> leaves a field missing from the file at its type's
        /// default (false, 0, null), not at this class's default, so a migration from a version
        /// without a field must set that field by <see cref="Version"/> here.
        /// Called by <see cref="SaveStore.Load{T}"/>.
        /// </summary>
        public TabletSettingsData Upgraded()
        {
            return new TabletSettingsData(musicOn, soundsOn, voiceVolume, vibrationOn, appLanguage);
        }

        private static float ClampedVolume(float volume)
        {
            return float.IsNaN(volume) ? HighestVoiceVolume : Mathf.Clamp(volume, LowestVoiceVolume, HighestVoiceVolume);
        }

        private static Language LanguageOf(string code)
        {
            return LanguageCodes.TryParseLanguage(code, out Language language) ? language : DefaultAppLanguage;
        }
    }
}
